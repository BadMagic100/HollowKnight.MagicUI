using UnityEngine;
using UnityEngine.UI;
using UButton = UnityEngine.UI.Button;

namespace MagicUI.Behaviours
{
    internal class MultiGraphicButton : UButton
    {
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            Color targetColor = state switch
            {
                SelectionState.Normal => colors.normalColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Disabled => colors.disabledColor,
                _ => Color.white
            };
            foreach (var graphic in GetComponentsInChildren<Graphic>())
            {
                graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }
    }
}
