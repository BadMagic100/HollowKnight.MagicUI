using MagicUI.Behaviours;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace MagicUI
{
    /// <summary>
    /// Entry point to create layouts
    /// </summary>
    public class LayoutRoot
    {
        /// <summary>
        /// The name of the layout root
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Whether this is a persistent layout
        /// </summary>
        public bool IsPersistent { get; private set; }

        private readonly GameObject rootCanvas;
        internal readonly LayoutOrchestrator layoutOrchestrator;

        public GameObject Canvas { get => rootCanvas; }

        /// <summary>
        /// Creates a new layout root
        /// </summary>
        /// <param name="persist">Whether the layout will persist across scene transitions.</param>
        /// <param name="name">The name of the layout root and underlying canvas</param>
        public LayoutRoot(bool persist, string name = "New LayoutRoot")
        {
            Name = name;
            IsPersistent = persist;

            rootCanvas = new(name);
            Canvas canvasComponent = rootCanvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scale = rootCanvas.AddComponent<CanvasScaler>();
            scale.referenceResolution = UI.Screen.size;
            rootCanvas.AddComponent<GraphicRaycaster>();
            layoutOrchestrator = rootCanvas.AddComponent<LayoutOrchestrator>();

            if (persist)
            {
                UObject.DontDestroyOnLoad(rootCanvas);
            }
        }

        /// <summary>
        /// Destroys the layout root and all of its child elements
        /// </summary>
        public void Destroy()
        {
            UObject.Destroy(rootCanvas);
            UObject.Destroy(layoutOrchestrator);
            // todo: a bit concerned about memory here - pretty sure we still hold reference to the orchestrator which holds
            // references to the objects even though they've been destroyed - user needs to set their reference to null
            // to even have a chance to GC
        }
    }
}
