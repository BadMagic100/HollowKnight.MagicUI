using MagicUI.Styles;

namespace MagicUI.Core
{
    /// <summary>
    /// Root class for an arrangable element with multiple children
    /// </summary>
    public abstract class Layout : ArrangableElement, ILayoutParent
    {
        /// <summary>
        /// The children of this layout
        /// </summary>
        [StyleIgnore]
        public ParentedElementList Children { get; }

        /// <summary>
        /// Creates a layout
        /// </summary>
        /// <param name="onLayout">The layout root to draw the layout on</param>
        /// <param name="name">The name of the layout</param>
        public Layout(LayoutRoot onLayout, string name = "New Layout") : base(onLayout, name)
        {
            Children = new ParentedElementList(this);
        }

        /// <inheritdoc/>
        public void HandleChildDestroyed(ArrangableElement child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
            }
        }
    }
}
