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
        public ParentedElementList Children { get; }

        public Layout(LayoutRoot onLayout, string name = "New Layout") : base(onLayout, name)
        {
            Children = new ParentedElementList(this);
        }

        public void HandleChildDestroyed(ArrangableElement child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
            }
        }
    }
}
