using MagicUI.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace MagicUI.Core
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
        private readonly CanvasGroup grp;
        internal readonly LayoutOrchestrator layoutOrchestrator;
        private readonly InteractivityController interactivityController;
        private readonly FadeController fadeController;

        /// <summary>
        /// The unity <see cref="UnityEngine.Canvas"/> underlying the layout.
        /// </summary>
        public GameObject Canvas { get => rootCanvas; }

        /// <summary>
        /// A read-only collection of elements registered to this layout.
        /// </summary>
        public IEnumerable<ArrangableElement> Elements => layoutOrchestrator.Elements;

        /// <summary>
        /// Whether the elements in this layout hierarchy should be interactive. When true (by default), the elements in the hierarchy
        /// will block interaction with other UI elements.
        /// </summary>
        public bool Interactive
        {
            get => interactivityController.interactiveWhileVisible;
            set => interactivityController.interactiveWhileVisible = value;
        }

        /// <summary>
        /// A predicate that determines whether the layout should be visible. By default (i.e. when there is no condition), the layout is
        /// always visible.
        /// </summary>
        public Func<bool>? VisibilityCondition
        {
            get => interactivityController.condition;
            set => interactivityController.condition = value;
        }

        /// <summary>
        /// The current opacity of elements in the layout hierarchy.
        /// </summary>
        public float Opacity
        {
            get => grp.alpha;
            set
            {
                fadeController.Cancel();
                grp.alpha = value;
            }
        }

        /// <summary>
        /// Whether to render the layout system bounds of elements in this layout for debugging purposes
        /// </summary>
        public bool RenderDebugLayoutBounds
        {
            get => layoutOrchestrator.shouldRenderDebugBounds;
            set => layoutOrchestrator.shouldRenderDebugBounds = value;
        }

        /// <summary>
        /// Creates a new layout root
        /// </summary>
        /// <param name="persist">Whether the layout will persist across scene transitions</param>
        /// <param name="name">The name of the layout root and underlying canvas</param>
        public LayoutRoot(bool persist, string name = "New LayoutRoot")
        {
            Name = name;
            IsPersistent = persist;

            rootCanvas = new(name);
            Canvas canvasComponent = rootCanvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.pixelPerfect = true;

            CanvasScaler scale = rootCanvas.AddComponent<CanvasScaler>();
            scale.referenceResolution = UI.Screen.size;
            scale.dynamicPixelsPerUnit = 1000;
            scale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            // workaround to force a rescale on the first frame
            scale.enabled = false;
            scale.enabled = true;

            rootCanvas.AddComponent<GraphicRaycaster>();

            grp = rootCanvas.AddComponent<CanvasGroup>();

            fadeController = rootCanvas.AddComponent<FadeController>();
            // it is important the interactivity controller comes later so it takes higher precedence while the layout should be invisible.
            // this allows it to suppress any unintended changes in fade while visibility condition evaluates to false.
            interactivityController = rootCanvas.AddComponent<InteractivityController>();
            layoutOrchestrator = rootCanvas.AddComponent<LayoutOrchestrator>();

            if (persist)
            {
                UObject.DontDestroyOnLoad(rootCanvas);
            }
        }

        /// <summary>
        /// Creates a new layout root
        /// </summary>
        /// <param name="persist">Whether the layout will persist across scene transitions</param>
        /// <param name="pauseOnly">Whether the layout will be visible only while the game is paused</param>
        /// <param name="name">The name of the layout root and underlying canvas</param>
        [Obsolete("This constructor provides a flag to set the layout conditionally visible while the game is paused. Please use the (string, bool) constructor "
            + "and VisibilityCondition property instead. You can set VisibilityCondition to GameManager.instance.IsGamePaused for equivalent behavior.")]
        public LayoutRoot(bool persist, bool pauseOnly, string name = "New LayoutRoot") : this(persist, name)
        {
            if (pauseOnly)
            {
                VisibilityCondition = GameManager.instance.IsGamePaused;
            }
        }

        /// <summary>
        /// Initializes a hotkey listener that performs an action when a given key combination is pressed
        /// </summary>
        /// <param name="key">The keypress to listen for</param>
        /// <param name="execute">The action to perform when the key is pressed</param>
        /// <param name="modifiers">Other required modifier keys that must be held to trigger the event</param>
        /// <param name="condition">The condition in which this hotkey should be enabled</param>
        public void ListenForHotkey(KeyCode key, Action execute, ModifierKeys modifiers = ModifierKeys.None, Func<bool>? condition = null)
        {
            HotkeyListener listener = rootCanvas.AddComponent<HotkeyListener>();
            listener.key = key;
            listener.modifiers = modifiers;
            listener.execute = execute;
            listener.enableCondition = condition;
        }

        /// <summary>
        /// Linearly fades the elements in this layout hierarchy to a new opacity value.
        /// </summary>
        /// <param name="targetOpacity">The target opacity.</param>
        /// <param name="fadeDuration">The duration of time the fade should take to complete, in seconds.</param>
        public void BeginFade(float targetOpacity, float fadeDuration)
        {
            fadeController.Cancel();
            fadeController.BeginFade(targetOpacity, fadeDuration);
        }

        /// <summary>
        /// Gets an element in this layout by name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>The first element in this layout with the given name if any exist, or else null</returns>
        public ArrangableElement? GetElement(string name)
        {
            return GetElements(name).FirstOrDefault();
        }

        /// <summary>
        /// Gets all elements in this layout by name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>All elements in this layout with the given name if any exist, or else an empty enumerable</returns>
        public IEnumerable<ArrangableElement> GetElements(string name)
        {
            return layoutOrchestrator.Find(name);
        }

        /// <summary>
        /// Gets an element in this layout by name and type
        /// </summary>
        /// <typeparam name="T">The type of element to search for</typeparam>
        /// <param name="name">The name to search for</param>
        /// <returns>The first element in this layout with the given name and type if any exist, or else null</returns>
        public T? GetElement<T>(string name) where T : ArrangableElement
        {
            return GetElements<T>(name).FirstOrDefault();
        }

        /// <summary>
        /// Gets all elements in this layout by name and type
        /// </summary>
        /// <typeparam name="T">The type of element to search for</typeparam>
        /// <param name="name">The name to search for</param>
        /// <returns>All elements in this layout with the given name and type if any exist, or else an empty enumerable</returns>
        public IEnumerable<T> GetElements<T>(string name) where T : ArrangableElement
        {
            return layoutOrchestrator.Find<T>(name);
        }

        /// <summary>
        /// Forces the interactivity controller to immediately re-evaluate interactivity conditions without needing to wait a frame
        /// </summary>
        public void ForceInteractivityRefresh()
        {
            interactivityController.ForceUpdate();
        }

        /// <summary>
        /// Destroys the layout root and all of its child elements
        /// </summary>
        public void Destroy()
        {
            rootCanvas.SetActive(false);
            layoutOrchestrator.Clear();
            UObject.Destroy(rootCanvas);
            UObject.Destroy(layoutOrchestrator);
        }
    }
}
