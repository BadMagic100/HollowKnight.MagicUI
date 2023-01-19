using System;
using UnityEngine.EventSystems;

namespace MagicUI.Behaviours
{
    internal class SelectionEventProxy : UIBehaviour, ISelectHandler, IDeselectHandler
    {
        public Action? OnSelectProxy;
        public Action? OnDeselectProxy;

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectProxy?.Invoke();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            OnDeselectProxy?.Invoke();
        }
    }
}
