namespace MagicUI.Core
{
    /// <summary>
    /// Root class for an arrangable element with a single child
    /// </summary>
    public abstract class Container : ArrangableElement, ILayoutParent
    {
        private ArrangableElement? child;
        /// <summary>
        /// The child of this container.Replacing or removing the child will destroy it.
        /// </summary>
        public ArrangableElement? Child 
        {
            get => child; 
            set
            {
                child?.Destroy();
                child = value;
                if (child != null)
                {
                    SetLogicalChild(child);
                }
            }
        }

        /// <summary>
        /// Creates a container
        /// </summary>
        /// <param name="onLayout">The layout root to draw the container on</param>
        /// <param name="name">The name of the container</param>
        public Container(LayoutRoot onLayout, string name = "New Container") : base(onLayout, name)
        {
        }

        /// <inheritdoc/>
        public void HandleChildDestroyed(ArrangableElement child)
        {
            if (child.Equals(Child))
            {
                DetachLogicalChild(child);
                Child = null;
            }
        }
    }
}
