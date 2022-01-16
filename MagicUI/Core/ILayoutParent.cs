namespace MagicUI.Core
{
    /// <summary>
    /// An element that is expected to have logical children
    /// </summary>
    public interface ILayoutParent
    {
        /// <summary>
        /// Additional handling for when a child is being destroyed
        /// </summary>
        /// <param name="child">The child being destroyed</param>
        void HandleChildDestroyed(ArrangableElement child);
    }
}
