# Understanding the Layout System

This article outlines how the layout system works. This will help users gain an understanding of common concepts you'll need to
know to work with MagicUI and some terms you'll see throughout this documentation. The ultimate goal is to build the foundations
needed to create more complex UIs and custom components.

## Fundamental Vocabulary

* **LayoutRoot**: The LayoutRoot is the top-level layout orchestrator. It handles the lifecycle of the elements you create so you
don't have to.
* **ArrangableElement, component, element, or similar**: An ArrangableElement is the most molecular building
block in MagicUI's layout system. It's an abstraction of something that has a measurable size and contains logic to be placed
at a given location on the screen.
* **Layout**: Not to be confused with LayoutRoot, a Layout is an ArrangableElement that has multiple children and subdivides its
space in a specific way.
* **Container**: An ArrangableElement with a single child that implements additional behavior and/or properties.
* **Effective size**: The size of an element with additional padding added. Parent elements treat this as the size of the child.
* **Content size**: The size of an element without additional padding added. Elements treat this as their own size.

## The Element Lifecycle

Broadly speaking, elements have 3 parts in their lifecycle:

1. The element is created. At this time, you set the element's name and LayoutRoot. You can also set additional properties at
this time to set initial state or desired traits (I recommend using initializer block syntax).
2. The element exists and is rendered. As its properties change, it changes in size and position. The element's `Measure`
method is called to determine its size, then its `Arrange` method is called to determine its position. This is handled
automatically by the layout system. If a property cannot cause an element's size to change, only `Arrange` will be called.
If an element has children, it will also measure and arrange its children as needed to determine its size and subdivide its
space.
3. The element is destroyed by calling its `Destroy` method. Elements specify what happens to their children when they are
destroyed, and can also specify how to handle when one of their children is destroyed. In most cases, destroying an element
will remove it from its parent and destroy all of its children.

This lifecycle is visualized in the figure below (todo: add figure)