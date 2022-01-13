using MagicUI.Core;
using UnityEngine;

namespace MagicUI.Components
{
    public sealed class StackLayout : Layout
    {
        private float spacing = 0;
        /// <summary>
        /// The spacing between elements in this layout
        /// </summary>
        public float Spacing
        {
            get => spacing;
            set
            {
                if (value != spacing)
                {
                    spacing = value;
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

        public StackLayout(LayoutRoot onLayout, string name = "New StackLayout") : base(onLayout, name) { }

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

        protected override Vector2 MeasureOverride()
        {
            if (Children.Count == 0) return Vector2.zero;
            float nonFlowMeasure = 0;
            float orientationFlowMeasure = (Children.Count - 1) * spacing;
            foreach (ArrangableElement child in Children)
            {
                (float childFlow, float childNonFlow) = RepackInFlowOrder(child.Measure());
                orientationFlowMeasure += childFlow;
                if (childNonFlow > nonFlowMeasure)
                {
                    nonFlowMeasure = childNonFlow;
                }
            }
            return RepackInFlowOrder(new Vector2(orientationFlowMeasure, nonFlowMeasure));
        }

        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            (float flowStart, float nonFlowStart) = RepackInFlowOrder(alignedTopLeftCorner);
            foreach (ArrangableElement child in Children)
            {
                (float childFlowSize, _) = RepackInFlowOrder(child.EffectiveSize);
                (_, float nonFlowSize) = RepackInFlowOrder(ContentSize);
                (float left, float top) = RepackInFlowOrder(new Vector2(flowStart, nonFlowStart));
                (float width, float height) = RepackInFlowOrder(new Vector2(childFlowSize, nonFlowSize));
                child.Arrange(new Rect(left, top, width, height));
                flowStart += childFlowSize + spacing;
            }
        }

        protected override void DestroyOverride()
        {
            Children.Clear();
        }
    }
}
