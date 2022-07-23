using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class FadeController : MonoBehaviour
    {
        private CanvasGroup? group;

        private float startOpacity;
        private float targetOpacity;

        private float fadeTime;
        private float elapsedTime;

        private bool fading = false;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (fading)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= fadeTime)
                {
                    // cap the elapsed time
                    elapsedTime = fadeTime;
                    fading = false;
                }
                // in the unity lifecycle this should be non-null; if it is, we should throw because we're not allowed to have this component on the GO anyway.
                group!.alpha = (elapsedTime / fadeTime) * (targetOpacity - startOpacity) + startOpacity;
            }
        }

        public void BeginFade(float targetOpacity, float fadeTime)
        {
            this.startOpacity = group!.alpha;
            this.targetOpacity = targetOpacity;

            this.fadeTime = fadeTime;
            this.elapsedTime = 0;

            fading = true;
        }

        public void Cancel()
        {
            fading = false;
        }
    }
}
