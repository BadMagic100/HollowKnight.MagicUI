namespace MagicUI.Core
{

    /// <summary>
    /// Possible actions on the layout system when a property value changes
    /// </summary>
    public enum ChangeAction
    {
        /// <summary>
        /// Rearrange the element holding the property value
        /// </summary>
        Arrange,
        /// <summary>
        /// Remeasure and rearrange the element holding the property value
        /// </summary>
        Measure,
        /// <summary>
        /// Rearrange the parent of the element holding the property value
        /// </summary>
        ParentArrange,
        /// <summary>
        /// Remeasure and rearrange the parent of the element holding the property value
        /// </summary>
        ParentMeasure,
        /// <summary>
        /// Take no action
        /// </summary>
        None
    }

    /// <summary>
    /// Extension methods to provide behavior to <see cref="ChangeAction"/>s
    /// </summary>
    public static class ChangeActionExtensions
    {
        /// <summary>
        /// Notify an element with the given action type
        /// </summary>
        /// <param name="actionOnChange">This action type</param>
        /// <param name="element">The element to notify</param>
        public static void Notify(this ChangeAction actionOnChange, ArrangableElement element)
        {
            switch (actionOnChange)
            {
                case ChangeAction.Arrange:
                    element.InvalidateArrange();
                    break;
                case ChangeAction.Measure:
                    element.InvalidateMeasure();
                    break;
                case ChangeAction.ParentArrange:
                    element.LogicalParent?.InvalidateArrange();
                    break;
                case ChangeAction.ParentMeasure:
                    element.LogicalParent?.InvalidateMeasure();
                    break;
                default:
                    break;
            }
        }
    }
}
