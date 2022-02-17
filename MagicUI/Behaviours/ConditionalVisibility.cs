using System;
using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class ConditionalVisibility : MonoBehaviour
    {
        public Func<bool>? condition;

        private void Update()
        {
            CanvasGroup grp = GetComponent<CanvasGroup>();
            if (condition == null || condition())
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
