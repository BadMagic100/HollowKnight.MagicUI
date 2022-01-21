# MagicUIExamples

Various examples for how to build UIs with MagicUI. `ExampleMod.cs` is the home of the mod itself;
you should start there. It contains a basic toggleable mod and shows how to create a persistent layout.
It then calls several otherwise self-contained examples to demonstrate various features on the layout.

* `SimpleLayoutExample.cs` is a basic example of using the layout system to render text on the screen
* `ComplexLayoutExample.cs` is a more in-depth example demonstrating more advanced features of the layout system
* `NonPersistentLayoutExample.cs` explains how and when to create non-persistent layouts
* `InteractivityAndDynamicSizingExample.cs` demonstrates the usage of interactive controls and shows
  the layout system dynamically updating the size and location of elements as their properties change
