using Modding;
using System;
using UnityEngine;

namespace MagicUI
{
    /// <summary>
    /// Root class for arrangeable UI elements
    /// </summary>
    public abstract class ArrangableElement
    {
        private static readonly Loggable log = LogHelper.GetLogger();

        private bool neverMeasured = true;
        private bool neverArranged = true;

        internal Rect PrevPlacementRect { get; private set; }

        public LayoutRoot LayoutRoot { get; private set; }

        /// <summary>
        /// Whether the most recent measurement is accurate
        /// </summary>
        public bool MeasureIsValid { get; private set; } = false;

        /// <summary>
        /// Whether the most recent arrangement is accurate
        /// </summary>
        public bool ArrangeIsValid { get; private set; } = false;

        /// <summary>
        /// The name of the arrangeable for lookup purposes
        /// </summary>
        public string Name { get; private set; }

        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
        /// <summary>
        /// The arrangeable's horizontal alignment
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                if (horizontalAlignment != value)
                {
                    horizontalAlignment = value;
                    InvalidateArrange();
                }
            }
        }

        private VerticalAlignment verticalAlignment = VerticalAlignment.Top;
        /// <summary>
        /// The arrangeable's vertical alignment
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                if (verticalAlignment != value)
                {
                    verticalAlignment = value;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// The cached desired size. Set from the last result in <see cref="Measure"/>.
        /// </summary>
        public Vector2 DesiredSize { get; private set; }

        /// <summary>
        /// This element's parent in the layout hierarchy, if any
        /// </summary>
        public ArrangableElement? LogicalParent { get; protected internal set; } = null;

        public ArrangableElement(LayoutRoot onLayout, string name = "New ArrangeableElement")
        {
            Name = name;
            LayoutRoot = onLayout;
            onLayout.layoutOrchestrator.RegisterElement(this);
        }

        /// <summary>
        /// Indicates the measure is no longer valid; will trigger a full re-render of the visual tree.
        /// </summary>
        public void InvalidateMeasure()
        {
            MeasureIsValid = false;
            LogicalParent?.InvalidateMeasure();
        }

        /// <summary>
        /// Indicates the arrange is no longer valid; will trigger a rearrange of this element and its children.
        /// </summary>
        public void InvalidateArrange()
        {
            ArrangeIsValid = false;
        }

        /// <summary>
        /// Helper method to get the position of the top left corner during arrangement, given the component's vertical and horizontal alignments.
        /// </summary>
        protected Vector2 GetAlignedTopLeftCorner(Rect availableSpace)
        {
            float x = horizontalAlignment switch
            {
                HorizontalAlignment.Left => availableSpace.xMin,
                HorizontalAlignment.Center => availableSpace.xMin + availableSpace.width / 2 - DesiredSize.x / 2,
                HorizontalAlignment.Right => availableSpace.xMax - DesiredSize.x,
                _ => throw new NotImplementedException("Can't handle the current horizontal alignment"),
            };

            float y = verticalAlignment switch
            {
                VerticalAlignment.Top => availableSpace.yMin,
                VerticalAlignment.Center => availableSpace.yMin + availableSpace.height / 2 - DesiredSize.y / 2,
                VerticalAlignment.Bottom => availableSpace.yMax - DesiredSize.y,
                _ => throw new NotImplementedException("Can't handle the current horizontal alignment"),
            };

            return new Vector2(x, y);
        }

        /// <summary>
        /// Calculates the desired size of the object and caches it in <see cref="DesiredSize"/> for later reference in this UI build cycle.
        /// </summary>
        public Vector2 Measure()
        {
            if (!MeasureIsValid)
            {
                if (!neverMeasured)
                {
                    log.LogDebug($"Re-measure triggered for {Name}");
                }
                DesiredSize = MeasureOverride();
                MeasureIsValid = true;
                neverMeasured = false;
                InvalidateArrange();
                LogicalParent?.InvalidateMeasure();
            }
            return DesiredSize;
        }

        /// <summary>
        /// Internal implementation to calculate desired size.
        /// </summary>
        protected abstract Vector2 MeasureOverride();

        /// <summary>
        /// Positions the object within the allocated space.
        /// </summary>
        /// <param name="availableSpace">The space available for the element.</param>
        public void Arrange(Rect availableSpace)
        {
            // only rearrange if we're either put into a new space or explicitly told to rearrange.
            if (!ArrangeIsValid || PrevPlacementRect != availableSpace)
            {
                if (!neverArranged)
                {
                    log.LogDebug($"Re-arrange triggered for {Name}");
                }
                ArrangeOverride(availableSpace);
                neverArranged = false;
                PrevPlacementRect = availableSpace;
                ArrangeIsValid = true;
            }
        }

        /// <summary>
        /// Internal implementation to position the object within the allocated space.
        /// </summary>
        /// <param name="availableSpace">The space available for the element.</param>
        protected abstract void ArrangeOverride(Rect availableSpace);
    }
}
