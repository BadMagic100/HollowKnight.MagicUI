using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MagicUIExamples
{
    // A simple example demonstrating the use of a non-persistent layout. The gameobjects in this
    // layout will automatically be destroyed on a new scene load, which can be useful if you don't need or
    // want to manage the lifecycle of the layout yourself. for example, RandoStats uses this to display
    // a UI during the completion cutscene.
    public static class NonPersistentLayoutExample
    {
        // a persistent layout is not required to create a non-persistent layout; this is just used
        // for the sake of example to create a hotkey binding
        public static void Setup(LayoutRoot rootPersistentLayout)
        {
            // this is called in HeroController.Awake, which happens before the scene actually begins - be careful when you
            // create non-persistent stuff as it may go away before you ever see it
            // to work around this, we'll create a keybind for it.
            rootPersistentLayout.ListenForHotkey(KeyCode.N, () =>
            {
                LayoutRoot noPersist = new(false);
                new TextObject(noPersist)
                {
                    Text = "This text will go away when you leave the scene",
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }, ModifierKeys.Ctrl);
        }
    }
}
