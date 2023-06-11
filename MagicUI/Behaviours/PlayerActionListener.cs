using InControl;
using System;
using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class PlayerActionListener : MonoBehaviour
    {
        public PlayerAction? playerAction;
        public Func<bool>? enableCondition;
        public Action? execute;
        private bool previousPressedState = false;

        private bool CurrentPressedState() {
            return playerAction?.IsPressed ?? false;
        }

        private void Update()
        {
            var shouldExecute = CurrentPressedState() && !previousPressedState;
            previousPressedState = CurrentPressedState();

            bool enable = enableCondition?.Invoke() ?? true;
            if (enable && shouldExecute)
            {
                execute?.Invoke();
            }
        }
    }
}
