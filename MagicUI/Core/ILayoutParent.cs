namespace MagicUI.Core
{
    /// <summary>
    /// An element that is expected to have logical children
    /// </summary>
    public interface ILayoutParent
    {
        public void HandleChildDestroyed(ArrangableElement child);
    }
}
