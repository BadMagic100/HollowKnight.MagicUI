namespace MagicUI.Core
{
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

        public Container(LayoutRoot onLayout, string name = "New Container") : base(onLayout, name)
        {
        }

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
