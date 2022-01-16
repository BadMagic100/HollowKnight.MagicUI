﻿using MagicUI.Behaviours;
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
        internal readonly LayoutOrchestrator layoutOrchestrator;

        /// <summary>
        /// The unity <see cref="UnityEngine.Canvas"/> underlying the layout
        /// </summary>
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
            scale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            rootCanvas.AddComponent<GraphicRaycaster>();
            layoutOrchestrator = rootCanvas.AddComponent<LayoutOrchestrator>();

            if (persist)
            {
                UObject.DontDestroyOnLoad(rootCanvas);
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
