# Debugging Layouts with Global Settings

If your layout just isn't looking right, MagicUI provides a few global settings you can use to configure debug logging
to see exactly what's going on behind the scenes. First, we'll see what settings are available; then we'll see how to
use MagicUI's logs to troubleshoot unexpected behavior.

## Available settings

You'll have to modify the global settings file manually like the old days; these settings don't need to be exposed 
to the average player so there's no menu for them. After first running the game with MagicUI installed, the default
settings will be available with 

| Setting | Description | Type | Default value |
| --- | --- | --- | --- |
| LogLayoutInformation | Whether MagicUI should log information about the layout lifecycle | bool | false |
| LogLevel | The severity to log the information when it is logged | `Fine`, `Debug`, `Info`, `Warn`, or `Error` | `Fine` |

## Debugging logs

MagicUI logs layout information from two key classes: `LayoutOrchestrator` and [`ArrangableElement`](xref:MagicUI.Core.ArrangableElement).
LayoutOrchestrator is an internal class that manages the layout lifecycle. It will log when it detects an element with an invalid
measurement or arrangement, and log again when it is done measuring and/or arranging that element and its children. This means
you should look for a block like this to start diagnosing issues with a given element:

```text
[FINE]:[MagicUI.LayoutOrchestrator] - Measure/Arrange starting for <Element name> of type <type>
...
[FINE]:[MagicUI.LayoutOrchestrator] - Measure/Arrange completed for <Element name>
```

Once you've found the corresponding orchestrator block for the change you made, you can look for information about that element
and its children logged by `ArrangableElement`. Each element in the logical hierarchy will be log at the start and finish of
each of its measures and arrangements. A block for a single element might look like this (as a reminder, elements are
recursively measured and then recursively arranged, so complex layouts may be nested):

```text
[DEBUG]:[MagicUI.ArrangableElement] - Measure triggered for <Element name>
[DEBUG]:[MagicUI.ArrangableElement] - Computed <Element name> size as (0.0, 80.0), adjusted from (0.0, 40.0)
[DEBUG]:[MagicUI.ArrangableElement] - Arrange triggered for <Element name> in (x:0.00, y:0.00, width:1920.00, height:1080.00)
[DEBUG]:[MagicUI.ArrangableElement] - <Element name> top-left corner aligned and adjusted to (960.0, 1020.0)
```

This lists the following information:
* The first line notes that the measure is starting
* The second line notes that the measure has finished; it lists the actual content size (returned from [`MeasureOverride`](xref:MagicUI.Core.ArrangableElement#MagicUI_Core_ArrangableElement_MeasureOverride))
  and the adjusted size with padding that will be used by the layout system
* The third line notes that the arrange is starting, and the space available to the element as allocated by its parent
* The fourth line notes the position of the top-left corner of the element's content, calculated from the available space,
  alignment, and padding; this value is passed to [`ArrangeOverride`](xref:MagicUI.Core.ArrangableElement#MagicUI_Core_ArrangableElement_ArrangeOverride_UnityEngine_Vector2_).
  
You can use this information to understand how the element is being measured and arranged, and why it isn't going where you think
it should be.