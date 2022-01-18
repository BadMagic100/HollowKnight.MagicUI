using UnityEngine.UI;

namespace MagicUI.Core
{
    /// <summary>
    /// A controller-interactable element
    /// </summary>
    public interface IControllerInteractable
    {
        /// <summary>
        /// Gets the Unity <see cref="Selectable"/> underlying the element
        /// </summary>
        public Selectable GetSelectable();
    }
}
