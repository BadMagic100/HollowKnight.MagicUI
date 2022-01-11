namespace MagicUI.Core
{
    public enum Visibility
    {
        /// <summary>
        /// The element is visible
        /// </summary>
        Visible, 
        /// <summary>
        /// The element is still visible to the layout system (i.e. for measurement and arrangement), but not to the user
        /// </summary>
        Hidden,
        /// <summary>
        /// The element is not visible to the layout system (i.e. for measurement and arrangment) or the user
        /// </summary>
        Collapsed
    }
}
