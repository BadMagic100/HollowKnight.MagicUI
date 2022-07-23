using System;
using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class InteractivityController : MonoBehaviour
    {
        public Func<bool>? condition;
        public bool interactiveWhileVisible = true;

        private bool hasCachedAlpha = false;
        private float cachedAlpha;

        private void Update()
        {
            CanvasGroup grp = GetComponent<CanvasGroup>();
            if (condition == null || condition())
            {
                // only restore opacity once, when flipping from invisible to visible.
                if (hasCachedAlpha)
                {
                    grp.alpha = cachedAlpha;
                    hasCachedAlpha = false;
                }
                grp.interactable = interactiveWhileVisible;
                grp.blocksRaycasts = interactiveWhileVisible;
            }
            else
            {
                // stop any ongoing fade operations
                GetComponent<FadeController>()?.Cancel();
                if (!hasCachedAlpha)
                {
                    cachedAlpha = grp.alpha;
                    hasCachedAlpha = true;
                }
                grp.alpha = 0;
                grp.interactable = false;
                grp.blocksRaycasts = false;
            }
        }
    }
}
