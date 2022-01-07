using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class Persist : MonoBehaviour
    {
        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
