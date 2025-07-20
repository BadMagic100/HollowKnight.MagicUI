# Quick Start

## What is MagicUI?

MagicUI is a hierarchical UI framework for Hollow Knight mods to abstract away the details of working with GameObjects
and Unity UI directly. Its structure is heavily inspired by [WPF](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf?view=vs-2022),
and it aims to be built of modular components. This allows mod developers to create elegant UIs in a very simple and expressive
way. MagicUI is particularly powerful in cases where your UI elements have dynamic sizing or need to be positioned relative to
each other (i.e. saying "these 2 things should be in the bottom-left corner 5px apart" is more natural than "these 2 things
should be at (x1, y1) and (x2, y2)").

MagicUI offers the following features:

* Several built-in UI components and layouts for general use
* Interface for creating custom UI components and layouts
* Texture and Sprite utilities to simplify loading in embedded image assets
* Flexibility to interact with the underlying unity GameObjects if desired

## Installation

MagicUI is a mod like any other Hollow Knight mod for 1.5. This means you can install it from Scarab, and declare it as a
dependency when adding your mod to Scarab. You can also install it manually from the [latest GitHub release](https://github.com/BadMagic100/HollowKnight.MagicUI/releases/latest).
Released versions on GitHub will be more frequent than Scarab releases to minimize churn, and may include non-critical bugfixes
or feature additions for developer preview.

## Basic Usage

See the [MagicUIExamples](https://github.com/BadMagic100/HollowKnight.MagicUI/tree/master/MagicUIExamples) project on GitHub for
several basic examples of creating UI with MagicUI. In general, you will need to do the following:

1. Create a [LayoutRoot](~/api/MagicUI.Core.LayoutRoot.yml). This is the top-level UI orchestrator that you add elements to. Usually
    you only need one, but you can create as many as you'd like to create logical separation between different UIs.
    ```csharp
    LayoutRoot myLayoutRoot = new(true, "Persistent example layout");
    ```
2. Create [ArrangableElement](~/api/MagicUI.Core.ArrangableElement.yml) components in your LayoutRoot and set their properties
    during initialization.
    ```csharp
    new TextObject(myLayoutRoot)
    {
        TextAlignment = HorizontalAlignment.Center,
        Text = "This is center-aligned text in the\ntop-left",
        Padding = new(10)
    };
    ```
3. Modify the properties of elements using your mod logic - or don't! Either way, you've created a UI only by declaring some
basic parameters.