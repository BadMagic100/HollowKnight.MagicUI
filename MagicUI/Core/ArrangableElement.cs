using MagicUI.Core.Internal;
using MagicUI.Styles;
using System;
using UnityEngine;

namespace MagicUI.Core
{
    /// <summary>
    /// Root class for arrangeable UI elements
    /// </summary>
    [Stylable]
    public abstract class ArrangableElement
    {
        private static readonly SettingsBoundLogger log = LogHelper.GetLogger();

        /// <summary>
        /// The visual layout parent of this element
        /// </summary>
        public LayoutRoot LayoutRoot { get; private set; }

        /// <summary>
        /// Indicates whether an element's ability to accurately measure itself is sensitive to the display resolution.
        /// Such elements are automatically remeasured when the resolution changes
        /// </summary>
        public virtual bool MeasureIsResolutionSensitive => false;

        /// <summary>
        /// Whether the most recent measurement can be treated as accurate
        /// </summary>
        public bool MeasureIsValid { get; private set; } = false;

        /// <summary>
        /// Whether the most recent arrangement can be treated as accurate
        /// </summary>
        public bool ArrangeIsValid { get; private set; } = false;

        /// <summary>
        /// Whether the element was successfully arranged at any point
        /// </summary>
        public bool WasEverArranged { get; private set; } = false;

        /// <summary>
        /// Whether the element is currently in the process of being destroyed
        /// </summary>
        public bool DestroyInProgress { get; private set; } = false;

        /// <summary>
        /// The name of the arrangeable for lookup purposes
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The current rect this element is placed in. This is set before <see cref="ArrangeOverride(Vector2)"/> is called, so you can access the placement
        /// space for custom implementations of the aligned top-left corner if needed
        /// </summary>
        public Rect PlacementRect { get; private set; }

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

        private Visibility visibility = Visibility.Visible;
        /// <summary>
        /// The actual visibility of the element
        /// </summary>
        public Visibility Visibility
        {
            get => visibility;
            set
            {
                if (visibility != value)
                {
                    // if we're going into or out of collapsed state, that may change our effective size
                    if (visibility == Visibility.Collapsed || value == Visibility.Collapsed)
                    {
                        InvalidateMeasure();
                    }
                    // if we're going into or out of fully visible state, that may affect our arrangement/rendering even if no size change
                    if (visibility == Visibility.Visible || value == Visibility.Visible)
                    {
                        InvalidateArrange();
                    }
                    selfEffectiveVisibilityMayChange = true;
                    visibility = value;
                }
            }
        }

        private Padding padding = Padding.Zero;
        /// <summary>
        /// The padding around 
        /// </summary>
        public Padding Padding
        {
            get => padding;
            set
            {
                if (padding != value)
                {
                    padding = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Whether the element's content will be rendered, i.e. if this elements ancestors in the logical tree are all visible
        /// </summary>
        public bool IsEffectivelyVisible
        {
            get
            {
                ArrangableElement? next = this;
                while (next != null)
                {
                    if (next.Visibility != Visibility.Visible)
                    {
                        return false;
                    }

                    next = next.LogicalParent;
                }
                return true;
            }
        }

        private bool selfEffectiveVisibilityMayChange = false;
        // lifecycle check for propagating visibility changes to children
        private bool EffectiveVisibilityMayChange
        {
            get
            {
                return selfEffectiveVisibilityMayChange || LogicalParent?.EffectiveVisibilityMayChange == true;
            }
        }

        /// <summary>
        /// The cached size of this element's actual content. Set from the last result of <see cref="Measure"/>.
        /// </summary>
        public Vector2 ContentSize { get; private set; }

        /// <summary>
        /// The cached effective size in the layout system including padding and visibility. Set from the last result of <see cref="Measure"/>
        /// </summary>
        public Vector2 EffectiveSize { get; private set; }

        /// <summary>
        /// This element's parent in the layout hierarchy, if any
        /// </summary>
        public ArrangableElement? LogicalParent { get; private set; } = null;

        /// <summary>
        /// Sets the logical parent of the provided child to this element. Usable by any derived arrangable.
        /// </summary>
        /// <param name="child">The new child object</param>
        protected internal void SetLogicalChild(ArrangableElement child)
        {
            child.LogicalParent = this;
        }

        /// <summary>
        /// Detaches a child from this element in the logical tree if this element is its parent.
        /// </summary>
        /// <param name="child">The child object</param>
        protected internal void DetachLogicalChild(ArrangableElement child)
        {
            if (child.LogicalParent == this)
            {
                child.LogicalParent = null;
            }
        }

        /// <summary>
        /// Creates an arrangable element
        /// </summary>
        /// <param name="onLayout">The layout to draw the element on</param>
        /// <param name="name">The name of the elemtent</param>
        /// <remarks>
        /// Arrangable elements should be placed on the same layout as their parents. This is not a hard requirement, so
        /// in theory, it should work to put different parts of the logical tree in different layouts.
        /// However, if you do this, know that it may cause unexpected behavior and use with caution.
        /// </remarks>
        public ArrangableElement(LayoutRoot onLayout, string name = "New ArrangableElement")
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
        /// Helper method to get the position of the top left corner during arrangement, given the component's vertical and horizontal alignments and padding
        /// </summary>
        internal Vector2 GetAlignedTopLeftCorner(Rect availableSpace)
        {
            float x = horizontalAlignment switch
            {
                HorizontalAlignment.Left => availableSpace.xMin + Padding.Left,
                HorizontalAlignment.Center => availableSpace.xMin + availableSpace.width / 2 - ContentSize.x / 2 + (Padding.Left - Padding.Right) / 2,
                HorizontalAlignment.Right => availableSpace.xMax - ContentSize.x - Padding.Right,
                _ => throw new NotImplementedException("Can't handle the current horizontal alignment"),
            };

            float y = verticalAlignment switch
            {
                VerticalAlignment.Top => availableSpace.yMin + Padding.Top,
                VerticalAlignment.Center => availableSpace.yMin + availableSpace.height / 2 - ContentSize.y / 2 + (Padding.Top - Padding.Bottom) / 2,
                VerticalAlignment.Bottom => availableSpace.yMax - ContentSize.y - Padding.Bottom,
                _ => throw new NotImplementedException("Can't handle the current horizontal alignment"),
            };

            Vector2 vec = new(x, y);
            if (!ArrangeIsValid)
            {
                log.Log($"{Name} top-left corner aligned and adjusted to {vec}");
            }
            return vec;
        }

        /// <summary>
        /// Calculates the desired size of the object and caches it in <see cref="ContentSize"/> for later reference in this UI build cycle.
        /// </summary>
        public Vector2 Measure()
        {
            if (!MeasureIsValid)
            {
                log.Log($"Measure triggered for {Name}");
                ContentSize = MeasureOverride();
                EffectiveSize = ContentSize + new Vector2(Padding.AddedWidth, Padding.AddedHeight);
                if (Visibility == Visibility.Collapsed)
                {
                    EffectiveSize = Vector2.zero;
                }
                MeasureIsValid = true;
                InvalidateArrange();
                log.Log($"Computed {Name} size as {EffectiveSize}, adjusted from {ContentSize}");
            }
            return EffectiveSize;
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
            // only rearrange if we're put into a new space, changing visibility, or explicitly told to rearrange.
            if (!ArrangeIsValid || EffectiveVisibilityMayChange || PlacementRect != availableSpace)
            {
                log.Log($"Arrange triggered for {Name} in {availableSpace}");
                PlacementRect = availableSpace;
                ArrangeOverride(GetAlignedTopLeftCorner(availableSpace));
                ArrangeIsValid = true;
                WasEverArranged = true;
                selfEffectiveVisibilityMayChange = false;
            }
        }

        /// <summary>
        /// Internal implementation to position the object within the allocated space.
        /// </summary>
        /// <param name="alignedTopLeftCorner">The space available for the element.</param>
        protected abstract void ArrangeOverride(Vector2 alignedTopLeftCorner);

        /// <summary>
        /// Destroys this element
        /// </summary>
        /// <remarks>
        /// Generally, it assumed when you destroy an element, it, its children, and any underlying <see cref="GameObject"/>s are not intended to be reused.
        /// Accordingly, a destroyed element will be removed from its parent in most cases and unregistered from the layout system.
        /// Attempting to use a destroyed element will result in undefined behavior and may give unwanted results.
        /// </remarks>
        public void Destroy()
        {
            if (!DestroyInProgress)
            {
                log.Log($"Destroy triggered for {Name}");
                DestroyInProgress = true;
                // don't waste measure/arrange time on me while i'm destroying myself
                MeasureIsValid = true;
                ArrangeIsValid = true;
                LayoutRoot.layoutOrchestrator.RemoveElement(this);
                // additional custom handling for my specific type of component and my parent type
                DestroyOverride();
                if (LogicalParent is ILayoutParent layoutParent)
                {
                    layoutParent.HandleChildDestroyed(this);
                }
            }
        }

        /// <summary>
        /// Internal implementation to perform any additional cleanup when the element is destroyed
        /// </summary>
        protected abstract void DestroyOverride();
    }
}
