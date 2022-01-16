namespace MagicUI.Core
{
    /// <summary>
    /// Root class for an arrangable element with a single child
    /// </summary>
    public abstract class Container : ArrangableElement, ILayoutParent
    {
        private LayoutRoot? child;
        /// <summary>
        /// The child of this container
        /// </summary>
        public LayoutRoot? Child 
        {
            get => child; 
            set
            {
                child?.Destroy();
                child = value;
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
                child.LogicalParent = null;
                Child = null;
            }
        }
    }
}
