using MagicUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class HotkeyListener : MonoBehaviour
    {
        public KeyCode key;
        public ModifierKeys modifiers = ModifierKeys.None;
        public Func<bool>? enableCondition;
        public Action? execute;

        private IEnumerable<ModifierKeys> GetPresentModifiers(ModifierKeys modifiers)
        {
            foreach (ModifierKeys flag in Enum.GetValues(typeof(ModifierKeys)))
            {
                if (!flag.Equals(ModifierKeys.None) && modifiers.HasFlag(flag))
                {
                    yield return flag;
                }
            }
        }

        private bool AllModifiersHeld(ModifierKeys modifiers)
        {
            return GetPresentModifiers(modifiers).All(key =>
            {
                string name = key switch
                {
                    ModifierKeys.Ctrl => "ctrl",
                    ModifierKeys.Alt => "alt",
                    ModifierKeys.Shift => "shift",
                    _ => throw new NotImplementedException("Can't handle the requested modifier key."),
                };
                return Input.GetKey("left " + name) || Input.GetKey("right " + name);
            });
        }

        private void Update()
        {
            bool enable = enableCondition?.Invoke() ?? true;
            if (enable && AllModifiersHeld(modifiers) && Input.GetKeyDown(key))
            {
                execute?.Invoke();
            }
        }
    }
}
