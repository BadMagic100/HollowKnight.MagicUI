using MagicUI.Core;
using MagicUI.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagicUI.Elements
{
    /// <summary>
    /// Units of measure for grid rows and columns
    /// </summary>
    public enum GridUnit
    {
        /// <summary>
        /// An absolute minimum size value in screen-relative pixels
        /// </summary>
        AbsoluteMin,
        /// <summary>
        /// A size defined proportional to other rows/columns with proportional units
        /// </summary>
        Proportional
    }

    /// <summary>
    /// A measure of a grid row/column with a unitt
    /// </summary>
    public struct GridDimension
    {
        /// <summary>
        /// The size of the dimension
        /// </summary>
        public float Size { get; init; }
        /// <summary>
        /// The unit of the dimension
        /// </summary>
        public GridUnit Unit { get; init; }

        /// <summary>
        /// Convenience constructor to initialize a GridDimension
        /// </summary>
        public GridDimension(float size, GridUnit unit)
        {
            Size = size;
            Unit = unit;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Size:G3}{(Unit == GridUnit.Proportional ? "*" : "px")}";
        }
    }

    /// <summary>
    /// A generic grid layout with user-specified rows and columns
    /// </summary>
    /// <remarks>
    /// Grid rows and columns are sized semi-dynamically. A row/column with the unit AbsoluteMin will be sized at the specified size and
    /// grow to fit children as needed. If a child spans multiple of these rows/columns, each row/column will be allocated height/width
    /// proportional to the child's size (assuming it needs to grow at all). Rows/columns with the Proportional unit will proportionally split
    /// the remaining available minimum width/height, and grow with each other in step until all children are able to fit.
    /// </remarks>
    public class GridLayout : Layout
    {
        private static readonly SettingsBoundLogger log = LogHelper.GetLogger();

        private static bool PositiveIntValidator(int x) => x > 0;
        private static bool UintValidator(int x) => x >= 0;

        /// <summary>
        /// Attached property for row. This is the first row that the item will appear in the grid. Defaults to 0 and is capped
        /// at the last row.
        /// </summary>
        public static AttachedProperty<int> Row = new(0, ChangeAction.ParentMeasure, UintValidator);
        /// <summary>
        /// Attached property for column. This is the first column that the item will appear in the grid. Defaults to 0 and is capped
        /// at the last column.
        /// </summary>
        public static AttachedProperty<int> Column = new(0, ChangeAction.ParentMeasure, UintValidator);
        /// <summary>
        /// Attached property for row span. This is the maximum number of rows that an item will occupy. Defaults to 1.
        /// </summary>
        public static AttachedProperty<int> RowSpan = new(1, ChangeAction.ParentMeasure, PositiveIntValidator);
        /// <summary>
        /// Attached property for column span. This is the maximum number of columns that an item will occupy. Defaults to 1.
        /// </summary>
        public static AttachedProperty<int> ColumnSpan = new(1, ChangeAction.ParentMeasure, PositiveIntValidator);

        private float[] rowSizes;
        private float[] colSizes;

        private NotifyingCollection<GridDimension> rowDefs;
        /// <summary>
        /// Definition of the number of rows, their sizes, and their types. By default this is empty. If the grid is measured
        /// with empty row definitions, one will be added for you with a minimum height of 0px.
        /// </summary>
        public ICollection<GridDimension> RowDefinitions => rowDefs;

        private NotifyingCollection<GridDimension> colDefs;
        /// <summary>
        /// Definition of the number of columns, their sizes, and their types. By default this is empty. If the grid is measured
        /// with empty column definitions, one will be added for you with a minimum height of 0px.
        /// </summary>
        public ICollection<GridDimension> ColumnDefinitions => colDefs;

        private float minWidth = 0;
        /// <summary>
        /// The minimum width to be occupied by the grid
        /// </summary>
        public float MinWidth
        {
            get => minWidth;
            set
            {
                if (minWidth != value)
                {
                    minWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        private float minHeight = 0;
        /// <summary>
        /// The minimum height to be occupied by the grid
        /// </summary>
        public float MinHeight
        {
            get => minHeight;
            set
            {
                if (minHeight != value)
                {
                    minHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Creates a grid layout
        /// </summary>
        /// <param name="onLayout">The layout root to draw the grid layout on</param>
        /// <param name="name">the name of the grid layout</param>
        public GridLayout(LayoutRoot onLayout, string name = "New GridLayout") : base(onLayout, name)
        {
            rowDefs = new(this, ChangeAction.Measure);
            colDefs = new(this, ChangeAction.Measure);

            rowSizes = new float[0];
            colSizes = new float[0];
        }

        private IEnumerable<int> SpannedIndices(int start, int span, int maxExclusive)
        {
            int realStart = Math.Min(start, maxExclusive - 1);
            yield return realStart;
            for (int i = realStart + 1; i < start + span && i < maxExclusive; i++)
            {
                yield return i;
            }
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            // add default rows if they're not provided
            if (rowDefs.Count == 0)
            {
                rowDefs.Add(new GridDimension(0, GridUnit.AbsoluteMin));
                MagicUI.Instance.LogDebug($"Adding default min-size row def to {Name}");
            }
            if (colDefs.Count == 0)
            {
                colDefs.Add(new GridDimension(0, GridUnit.AbsoluteMin));
                MagicUI.Instance.LogDebug($"Adding default min-size column def to {Name}");
            }

            rowSizes = new float[rowDefs.Count];
            colSizes = new float[colDefs.Count];
            float[] rowProportions = new float[rowDefs.Count];
            float rowProportionSum = 0;
            float[] colProportions = new float[colDefs.Count];
            float colProportionalSum = 0;

            // pass over each row; if it's an absolute row set its minimum preferred size, if it's proportional set its proportion
            for (int i = 0; i < rowDefs.Count; i++)
            {
                float rowSize = rowDefs[i].Size;
                if (rowDefs[i].Unit == GridUnit.AbsoluteMin)
                {
                    rowSizes[i] = rowSize;
                }
                else
                {
                    rowSizes[i] = 0;
                    rowProportions[i] = rowSize;
                    rowProportionSum += rowSize;
                }
            }
            // normalize sum of proportional rows
            if (rowProportionSum > 0)
            {
                for (int i = 0; i < rowDefs.Count; i++)
                {
                    rowProportions[i] /= rowProportionSum;
                }
            }

            // same as above, but for columns
            for (int i = 0; i < colDefs.Count; i++)
            {
                float colSize = colDefs[i].Size;
                if (colDefs[i].Unit == GridUnit.AbsoluteMin)
                {
                    colSizes[i] = colSize;
                }
                else
                {
                    colSizes[i] = 0;
                    colProportions[i] = colSize;
                    colProportionalSum += colSize;
                }
            }
            if (colProportionalSum > 0)
            {
                for (int i = 0; i < colDefs.Count; i++)
                {
                    colProportions[i] /= colProportionalSum;
                }
            }

            log.Log($"Grid property precompute");
            log.Log($"Rows: {string.Join(", ", rowDefs)}");
            log.Log($"Row sizes: {string.Join(", ", rowSizes)}");
            log.Log($"Row proportions: {string.Join(", ", rowProportions)}");
            log.Log($"Cols: {string.Join(", ", colDefs)}");
            log.Log($"Col sizes: {string.Join(", ", colSizes)}");
            log.Log($"Col proportions: {string.Join(", ", colProportions)}");

            // measure each child and sort it in a way that we can access each child of a given row/column easily. for children that span
            // multiple rows/columns, include it in both/all.
            foreach (ArrangableElement child in Children)
            {
                int row = Row.Get(child);
                int col = Column.Get(child);
                int rowSpan = RowSpan.Get(child);
                int colSpan = ColumnSpan.Get(child);
                Vector2 size = child.Measure();
                foreach (int i in SpannedIndices(row, rowSpan, rowDefs.Count))
                {
                    // prefer a size according to the largest portion of a child allocated here. for absolute rows,
                    // the final size of the row will be set by this. for proportional rows, we may have to grow them
                    // to maintain proportionality or fill additional space in the grid's min size.
                    rowSizes[i] = Math.Max(rowSizes[i], size.y / rowSpan);
                }
                foreach (int i in SpannedIndices(col, colSpan, colDefs.Count))
                {
                    colSizes[i] = Math.Max(colSizes[i], size.x / colSpan);
                }
            }

            log.Log($"Grid property post child measure");
            log.Log($"Row sizes: {string.Join(", ", rowSizes)}");
            log.Log($"Col sizes: {string.Join(", ", colSizes)}");

            // so each child is accessible by the rows and columns it occupies, and the width/height of each absolutemin col/row is computed.
            // now we still need to handle proportional rows/columns
            // specifically, we need to:
            // 1. maximize the ratio between the required minimum size and the proportion to find the largest needed "pixels per unit"
            // 2. find the new minimum space needed for a panel by multiplying the discovered ppu with the proportional size
            // 3. find the actual width/height of the grid now - if it's less than the min, divide up the remaining space amongst the proportional cols
            float rowRequiredPpu = rowProportions.Select((p, i) => p > 0f ? rowSizes[i] / p : 0).Max();
            for (int row = 0; row < rowDefs.Count; row++)
            {
                if (rowProportions[row] != 0)
                {
                    rowSizes[row] = rowRequiredPpu * rowProportions[row];
                }
            }
            float colRequiredPpu = colProportions.Select((p, i) => p > 0f ? colSizes[i] / p : 0).Max();
            for (int col = 0; col < colDefs.Count; col++)
            {
                if (colProportions[col] != 0)
                {
                    colSizes[col] = colRequiredPpu * colProportions[col];
                }
            }

            float requiredWidth = colSizes.Sum();
            float requiredHeight = rowSizes.Sum();

            log.Log($"Grid property post resolve proportions");
            log.Log($"Row sizes: {string.Join(", ", rowSizes)}");
            log.Log($"Col sizes: {string.Join(", ", colSizes)}");

            float remainingWidth = minWidth - requiredWidth;
            if (remainingWidth > 0)
            {
                for (int col = 0; col < colDefs.Count; col++)
                {
                    colSizes[col] += remainingWidth * colProportions[col];
                }
            }

            float remainingHeight = minHeight - requiredHeight;
            if (remainingHeight > 0)
            {
                for (int row = 0; row < rowDefs.Count; row++)
                {
                    rowSizes[row] += remainingHeight * rowProportions[row];
                }
            }

            log.Log($"Grid property post fill space");
            log.Log($"Row sizes: {string.Join(", ", rowSizes)}");
            log.Log($"Col sizes: {string.Join(", ", colSizes)}");

            return new Vector2(colSizes.Sum(), rowSizes.Sum());
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            float accum = alignedTopLeftCorner.y;
            float[] rowStarts = new float[rowDefs.Count];
            for (int row = 0; row < rowDefs.Count; row++)
            {
                rowStarts[row] = accum;
                accum += rowSizes[row];
            }

            accum = alignedTopLeftCorner.x;
            float[] colStarts = new float[colDefs.Count];
            for (int col = 0; col < colDefs.Count; col++)
            {
                colStarts[col] = accum;
                accum += colSizes[col];
            }

            foreach (ArrangableElement child in Children)
            {
                int row = Row.Get(child);
                int col = Column.Get(child);
                int rowSpan = RowSpan.Get(child);
                int colSpan = ColumnSpan.Get(child);

                IEnumerable<int> spannedRows = SpannedIndices(row, rowSpan, rowDefs.Count);
                IEnumerable<int> spannedCols = SpannedIndices(col, colSpan, colDefs.Count);

                float top = rowStarts[spannedRows.First()];
                float left = colStarts[spannedCols.First()];
                float width = spannedCols.Select(i => colSizes[i]).Sum();
                float height = spannedRows.Select(i => rowSizes[i]).Sum();

                child.Arrange(new Rect(left, top, width, height));
            }
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            Children.Clear();
        }
    }
}
