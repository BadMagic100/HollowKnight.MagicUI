using MagicUI.Core;
using MagicUI.Styles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicUI.Elements
{
    /// <summary>
    /// A layout that places elements adjacent to each other in a grid with a dynamic number of rows/columns
    /// </summary>
    /// <remarks>
    /// Each panel in the grid is the size of the largest element; the grid will be as large as needed to fit one panel per
    /// child with the required number of rows/columns and spacing based on the specified parameters
    /// </remarks>
    [Stylable]
    public sealed class DynamicUniformGrid : Layout
    {
        private float verticalSpacing = 0;
        /// <summary>
        /// The layout's spacing between rows
        /// </summary>
        public float VerticalSpacing
        {
            get => verticalSpacing;
            set
            {
                if (value != verticalSpacing)
                {
                    verticalSpacing = value;
                    InvalidateMeasure();
                }
            }
        }

        private float horizontalSpacing = 0;
        /// <summary>
        /// The layout's spacing between columns
        /// </summary>
        public float HorizontalSpacing
        {
            get => horizontalSpacing;
            set
            {
                if (value != horizontalSpacing)
                {
                    horizontalSpacing = value;
                    InvalidateMeasure();
                }
            }
        }

        private Orientation orientation = Orientation.Vertical;
        /// <summary>
        /// The orientation/flow direction of the layout
        /// </summary>
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                if (value != orientation)
                {
                    orientation = value;
                    InvalidateMeasure();
                }
            }
        }

        private int childrenBeforeRollover = 2;
        /// <summary>
        /// The number of children before spilling over to the next row/column. In other words, the maximum number
        /// of columns if <see cref="Orientation"/> is <see cref="Orientation.Vertical"/>, or the maximum number of
        /// rows if <see cref="Orientation"/> is <see cref="Orientation.Horizontal"/>
        /// </summary>
        /// <remarks>
        /// By default, the value is 2, and cannot be set lower. If you want to use this with one row/column,
        /// you should use a <see cref="StackLayout"/> instead
        /// </remarks>
        public int ChildrenBeforeRollover
        {
            get => childrenBeforeRollover;
            set
            {
                if (value < 2)
                {
                    throw new ArgumentException("Value must be at least 2", nameof(ChildrenBeforeRollover));
                }
                if (value != childrenBeforeRollover)
                {
                    childrenBeforeRollover = value;
                    InvalidateMeasure();
                }
            }
        }

        private List<ArrangableElement> arrangedChildren = [];
        private List<ArrangableElement> collapsedChildren = [];

        /// <summary>
        /// Creates a dynamic uniform grid layout
        /// </summary>
        /// <param name="onLayout">The layout root to draw the grid on</param>
        /// <param name="name">The name of the grid</param>
        public DynamicUniformGrid(LayoutRoot onLayout, string name = "New DynamicUniformGrid") : base(onLayout, name) { }

        /// <summary>
        /// Repackages an XY vector in order based on the flow direction/orientation - whichever direction will stretch
        /// is returned in the X component, while the fixed size direction will be the Y component. Note that this function
        /// is its own inverse.
        /// </summary>
        /// <param name="xyVector">The vector to repackage</param>
        /// <returns>If orientation is horizontal, (X, Y). If orientation is vertical, (Y, X)</returns>
        private Vector2 RepackInFlowOrder(Vector2 xyVector)
        {
            float flow = orientation == Orientation.Horizontal ? xyVector.x : xyVector.y;
            float nonFlow = orientation == Orientation.Horizontal ? xyVector.y : xyVector.x;
            return new Vector2(flow, nonFlow);
        }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            arrangedChildren.Clear();
            collapsedChildren.Clear();
            if (Children.Count == 0)
            {
                return Vector2.zero;
            }

            float panelWidth = 0;
            float panelHeight = 0;
            foreach (ArrangableElement child in Children)
            {
                (float childWidth, float childHeight) = child.Measure();
                if (childWidth > panelWidth)
                {
                    panelWidth = childWidth;
                }
                if (childHeight > panelHeight)
                {
                    panelHeight = childHeight;
                }

                if (child.Visibility == Visibility.Collapsed)
                {
                    collapsedChildren.Add(child);
                }
                else
                {
                    arrangedChildren.Add(child);
                }
            }
            int numPanelsFlowDirection = (arrangedChildren.Count - 1) / childrenBeforeRollover + 1;
            int numPanelsNonFlowDirection = arrangedChildren.Count >= childrenBeforeRollover ? childrenBeforeRollover : arrangedChildren.Count;

            (float flowSpacing, float nonFlowSpacing) = RepackInFlowOrder(new Vector2(horizontalSpacing, verticalSpacing));
            (float panelFlow, float panelNonFlow) = RepackInFlowOrder(new Vector2(panelWidth, panelHeight));

            float flowSize = numPanelsFlowDirection * panelFlow + (numPanelsFlowDirection - 1) * flowSpacing;
            float nonFlowSize = numPanelsNonFlowDirection * panelNonFlow + (numPanelsNonFlowDirection - 1) * nonFlowSpacing;

            return RepackInFlowOrder(new Vector2(flowSize, nonFlowSize));
        }

        private float GetAlignedNonflowStartOffset(float nonFlowStart, float nonFlowTotalSize, float nonFlowSize)
        {
            if (Orientation == Orientation.Vertical)
            {
                return HorizontalAlignment switch
                {
                    HorizontalAlignment.Left => nonFlowStart,
                    HorizontalAlignment.Center => nonFlowStart + nonFlowTotalSize / 2 - nonFlowSize / 2,
                    HorizontalAlignment.Right => nonFlowStart + nonFlowTotalSize - nonFlowSize,
                    _ => throw new NotImplementedException("Can't handle the current horizontal alignment")
                };
            }
            else
            {
                return VerticalAlignment switch
                {
                    VerticalAlignment.Top => nonFlowStart,
                    VerticalAlignment.Center => nonFlowStart + nonFlowTotalSize / 2 - nonFlowSize / 2,
                    VerticalAlignment.Bottom => nonFlowStart + nonFlowTotalSize - nonFlowSize,
                    _ => throw new NotImplementedException("Can't handle the current vertical alignment")
                };
            }
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            (float flowStart, float nonFlowStart) = RepackInFlowOrder(alignedTopLeftCorner);

            int numPanelsFlowDirection = (arrangedChildren.Count - 1) / childrenBeforeRollover + 1;
            int numPanelsNonFlowDirection = arrangedChildren.Count >= childrenBeforeRollover ? childrenBeforeRollover : arrangedChildren.Count;

            (float flowSpacing, float nonFlowSpacing) = RepackInFlowOrder(new Vector2(horizontalSpacing, verticalSpacing));
            (float flowTotalSize, float nonFlowTotalSize) = RepackInFlowOrder(ContentSize);

            float childFlowSize = (flowTotalSize - (numPanelsFlowDirection - 1) * flowSpacing) / numPanelsFlowDirection;
            float childNonFlowSize = (nonFlowTotalSize - (numPanelsNonFlowDirection - 1) * nonFlowSpacing) / numPanelsNonFlowDirection;

            foreach (ArrangableElement child in collapsedChildren)
            {
                (float left, float top) = RepackInFlowOrder(alignedTopLeftCorner);
                (float width, float height) = RepackInFlowOrder(new Vector2(childFlowSize, childNonFlowSize));
                child.Arrange(new Rect(left, top, width, height));
            }

            for (int flowPanel = 0; flowPanel < numPanelsFlowDirection; flowPanel++)
            {
                float childFlowStart = flowPanel * (childFlowSize + flowSpacing) + flowStart;
                int childrenAvailable = arrangedChildren.Count - flowPanel * numPanelsNonFlowDirection;
                int childrenThisFlow = Math.Min(childrenBeforeRollover, childrenAvailable);
                float nonFlowSize = childrenThisFlow * childNonFlowSize + (childrenThisFlow - 1) * nonFlowSpacing;
                for (int nonFlowPanel = 0; nonFlowPanel < childrenThisFlow; nonFlowPanel++)
                {
                    float childNonflowStart = nonFlowPanel * (childNonFlowSize + nonFlowSpacing) + 
                        GetAlignedNonflowStartOffset(nonFlowStart, nonFlowTotalSize, nonFlowSize);
                    int childIndex = flowPanel * childrenBeforeRollover + nonFlowPanel;
                    (float left, float top) = RepackInFlowOrder(new Vector2(childFlowStart, childNonflowStart));
                    (float width, float height) = RepackInFlowOrder(new Vector2(childFlowSize, childNonFlowSize));
                    arrangedChildren[childIndex].Arrange(new Rect(left, top, width, height));
                }
            }
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            Children.Clear();
            arrangedChildren.Clear();
            collapsedChildren.Clear();
        }
    }
}
