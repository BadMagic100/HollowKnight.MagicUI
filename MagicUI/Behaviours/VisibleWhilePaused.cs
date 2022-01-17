using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class VisibleWhilePaused : MonoBehaviour
    {
        private void Update()
        {
            CanvasGroup grp = GetComponent<CanvasGroup>();
            if (GameManager.instance.IsGamePaused())
            {
                grp.alpha = 1;
                grp.interactable = true;
            }
            else
            {
                grp.alpha = 0;
                grp.interactable = false;
            }
        }
    }
}
