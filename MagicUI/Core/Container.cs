namespace MagicUI.Core
{
    public abstract class Container : ArrangableElement
    {
        /// <summary>
        /// The child of this container
        /// </summary>
        public LayoutRoot? Child { get; set; }

        public Container(LayoutRoot onLayout, string name = "New Container") : base(onLayout, name)
        {
        }
    }
}
