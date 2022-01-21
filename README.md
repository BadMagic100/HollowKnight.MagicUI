# MagicUI

![GitHub Build Workflow Status](https://img.shields.io/github/workflow/status/BadMagic100/HollowKnight.MagicUI/Build)
![GitHub Docs Workflow Status](https://img.shields.io/github/workflow/status/BadMagic100/HollowKnight.MagicUI/Docs?label=docs)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/BadMagic100/HollowKnight.MagicUI)
![Downloads GitHub all releases](https://img.shields.io/github/downloads/BadMagic100/HollowKnight.MagicUI/total)

A core mod for Hollow Knight that takes all the fuss of dealing with GameObjects behind the scenes,
allowing mod authors to seamlessly create in-game UIs for their mod with an easy-to-use hierarchical
layout system inspired by WPF.

## Features

* Layout & component system, with no GameObjects (but you can access them if you really want to)
* Several built-in elements and layouts for general use, such as:
  * Stack layout
  * Images
  * Buttons
  * Text display
  * Text input fields
* API to create custom components and layouts to suit your needs
* Persistent and non-persistent layout types
* Ability to show UI elements only while the game is paused
* Utility to listen for specific hotkeys given modifier keys and an optional condition

## Get started

To use MagicUI, install it like you would any other mod and declare a dependency in your mod project. See
the [MagicUIExamples](/MagicUIExamples) project for samples of what you can do, or view 
[the full docs](https://badmagic100.github.io/HollowKnight.MagicUI).