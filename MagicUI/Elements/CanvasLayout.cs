using MagicUI.Core;
using UnityEngine;

namespace MagicUI.Elements
{
    /// <summary>
    /// A layout that places elements absolutely in screen space.
    /// </summary>
    /// <remarks>
    /// Usually this is not a very idiomatic way to use MagicUI, but is helpful in the rare cases where you just need to move things around by hand.
    /// One particularly good use case for this is to ease migrations for people who have existing CanvasUtil UIs and want to migrate to create
    /// more complex layouts. <br/><br/>
    /// This layout has some unusual properties to be aware of compared to other elements:
    /// <ul>
    /// <li>Because this layout will request to fill the entire screen, it may create unexpected results if it is set as the child of another element, and changing
    /// its alignment and padding may result in children being placed off-screen</li>
    /// <li>The alignment of children will not have any effect, as it will be arranged at its offset with exactly its effective size</li>
    /// <li>The right and bottom paddings of children will not have any effect. It's recommended to use <see cref="XOffset"/> and <see cref="YOffset"/>
    /// attached properties instead of padding to position children.</li>
    /// </ul>
    /// </remarks>
    public class CanvasLayout : Layout
    {
        /// <summary>
        /// The child's X offset in the canvas
        /// </summary>
        public static AttachedProperty<float> XOffset = new(0, ChangeAction.Arrange);
        /// <summary>
        /// The child's Y offset in the canvas
        /// </summary>
        public static AttachedProperty<float> YOffset = new(0, ChangeAction.Arrange);

        /// <summary>
        /// Creates a canvas layout
        /// </summary>
        /// <param name="onLayout">The layout root to draw the stack layout on</param>
        /// <param name="name">The name of the stack layout</param>
        public CanvasLayout(LayoutRoot onLayout, string name = "New CanvasLayout") : base(onLayout, name) { }

        /// <inheritdoc/>
        protected override Vector2 MeasureOverride()
        {
            foreach (ArrangableElement element in Children)
            {
                element.Measure();
            }
            return UI.Screen.size;
        }

        /// <inheritdoc/>
        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            foreach (ArrangableElement element in Children)
            {
                Vector2 pos = alignedTopLeftCorner + new Vector2(XOffset.Get(element), YOffset.Get(element));
                Vector2 size = element.EffectiveSize;
                element.Arrange(new Rect(pos, size));
            }
        }

        /// <inheritdoc/>
        protected override void DestroyOverride()
        {
            Children.Clear();
        }
    }
}
