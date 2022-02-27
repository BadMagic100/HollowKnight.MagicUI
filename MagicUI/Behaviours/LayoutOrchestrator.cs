using MagicUI.Core;
using MagicUI.Core.Internal;
using MagicUI.Graphics.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class LayoutOrchestrator : MonoBehaviour
    {
        private static readonly SettingsBoundLogger log = LogHelper.GetLogger();

        private readonly List<ArrangableElement> elements = new();
        private readonly Dictionary<string, List<ArrangableElement>> elementLookup = new();

        public int measureBatch = 2;
        public int arrangeBatch = 5;
        public bool shouldRenderDebugBounds = false;

        /// <summary>
        /// Registers an element in layout for arrangement and later lookup
        /// </summary>
        /// <param name="element">The element to add</param>
        public void RegisterElement(ArrangableElement element)
        {
            if (!elements.Contains(element))
            {
                elements.Add(element);
                if (!elementLookup.ContainsKey(element.Name))
                {
                    elementLookup[element.Name] = new List<ArrangableElement>();
                }
                elementLookup[element.Name].Add(element);
            }
        }

        /// <summary>
        /// Removes an element from the layout
        /// </summary>
        /// <param name="element">The element to remove</param>
        public void RemoveElement(ArrangableElement element)
        {
            if (elements.Contains(element))
            {
                elements.Remove(element);
                elementLookup[element.Name].Remove(element);
                if (elementLookup[element.Name].Count == 0)
                {
                    elementLookup.Remove(element.Name);
                }
            }
        }

        /// <summary>
        /// Cleans up the orchestrator
        /// </summary>
        public void Clear()
        {
            elements.Clear();
            elementLookup.Clear();
        }

        /// <summary>
        /// Looks up several elements by name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>All elements with the given name, if any exist</returns>
        public IEnumerable<ArrangableElement> Find(string name)
        {
            if (elementLookup.ContainsKey(name))
            {
                return elementLookup[name].AsReadOnly();
            }
            else
            {
                return Enumerable.Empty<ArrangableElement>();
            }
        }

        /// <summary>
        /// Looks up several elements with a given type by name
        /// </summary>
        /// <typeparam name="T">The type of element to search for</typeparam>
        /// <param name="name">The name to search for</param>
        /// <returns>All elements of the given type with the given name, if any exist</returns>
        public IEnumerable<T> Find<T>(string name) where T : ArrangableElement
        {
            if (elementLookup.ContainsKey(name))
            {
                return elementLookup[name].OfType<T>();
            }
            else
            {
                return Enumerable.Empty<T>();
            }
        }

        private void Update()
        {
            // remeasure the specified number of elements. since measure invalidation propagates up the visual tree,
            // we can take only elements that have no parents (i.e. are roots of trees).
            IEnumerable<ArrangableElement> elementsToRemeasure = elements
                .Where(x => x.LogicalParent == null && !x.MeasureIsValid)
                .Take(measureBatch);
            foreach (ArrangableElement element in elementsToRemeasure)
            {
                log.Log($"Measure/Arrange starting for {element.Name} of type {element.GetType().Name}");
                // tree roots should always be top-level stuff - we'll allocate the entire screen size for arrangement
                // and allow them to go where they need to go.
                element.Measure();
                element.Arrange(UI.Screen);
                log.Log($"Measure/Arrange completed for {element.Name}");
            }

            // rearrange the specified number of elements. arrange invalidation does not propagate up the tree, so we can generally
            // process larger batches.
            IEnumerable<ArrangableElement> elementsToRearrange = elements
                .Where(x => x.MeasureIsValid && !x.ArrangeIsValid) // ensure the element has been measured before arranging
                .Take(arrangeBatch);
            foreach (ArrangableElement element in elementsToRearrange)
            {
                log.Log($"Arrange starting for {element.Name} of type {element.GetType().Name}");
                // an invalidated arrange indicates the element wants to place itself in a different location within the same available space.
                element.Arrange(element.PlacementRect);
                log.Log($"Arrange completed for {element.Name}");
            }
        }

        private Vector2 LocalToScreenPoint(Vector2 point)
        {
            Canvas c = GetComponent<Canvas>();
            Vector2 result = c.transform.TransformPoint(point - UI.Screen.size / 2);
            return new Vector2(Mathf.Round(result.x), Mathf.Round(result.y));
        }

        private void DrawRect(Rect rect, Color color)
        {
            float width = Mathf.Max(1, GetComponent<Canvas>().transform.GetScaleX());

            float left = rect.xMin;
            float right = rect.xMax;
            float top = rect.yMin;
            float bottom = rect.yMax;

            Vector2 topLeft = LocalToScreenPoint(new(left, top));
            Vector2 topRight = LocalToScreenPoint(new(right, top));
            Vector2 bottomRight = LocalToScreenPoint(new(right, bottom));
            Vector2 bottomLeft = LocalToScreenPoint(new(left, bottom));

            Drawing.DrawLine(topLeft, topRight, color, width, true);
            Drawing.DrawLine(topRight, bottomRight, color, width, true);
            Drawing.DrawLine(bottomRight, bottomLeft, color, width, true);
            Drawing.DrawLine(bottomLeft, topLeft, color, width, true);
        }

        private void OnGUI()
        {
            if (Event.current?.type != EventType.Repaint || !shouldRenderDebugBounds)
            {
                return;
            }

            GUI.depth = int.MaxValue;
            foreach (ArrangableElement element in elements)
            {
                if (element.EffectiveSize.magnitude > 0 && element.ArrangeIsValid)
                {
                    Rect placementRect = element.PlacementRect;
                    Rect contentRect = new(element.GetAlignedTopLeftCorner(placementRect), element.ContentSize);

                    Rect effectiveRect = contentRect;
                    effectiveRect.xMin -= element.Padding.Left;
                    effectiveRect.xMax += element.Padding.Right;
                    effectiveRect.yMin -= element.Padding.Top;
                    effectiveRect.yMax += element.Padding.Bottom;

                    if (!placementRect.Equals(UI.Screen))
                    {
                        DrawRect(placementRect, Color.green);
                    }
                    DrawRect(effectiveRect, Color.yellow);
                    DrawRect(contentRect, Color.cyan);
                }
            }
        }
    }
}
