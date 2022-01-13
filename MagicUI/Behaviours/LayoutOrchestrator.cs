using MagicUI.Core;
using MagicUI.Core.Internal;
using Modding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagicUI.Behaviours
{
    internal class LayoutOrchestrator : MonoBehaviour
    {
        private static readonly Loggable log = LogHelper.GetLogger();

        private readonly List<ArrangableElement> elements = new();
        private readonly Dictionary<string, List<ArrangableElement>> elementLookup = new();

        public int measureBatch = 2;
        public int arrangeBatch = 5;

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
                log.LogDebug($"Measure/Arrange starting for {element.Name} of type {element.GetType().Name}");
                // tree roots should always be top-level stuff - we'll allocate the entire screen size for arrangement
                // and allow them to go where they need to go.
                element.Measure();
                element.Arrange(UI.Screen);
                log.LogDebug($"Measure/Arrange completed for {element.Name}");
            }

            // rearrange the specified number of elements. arrange invalidation does not propagate up the tree, so we can generally
            // process larger batches.
            IEnumerable<ArrangableElement> elementsToRearrange = elements
                .Where(x => x.MeasureIsValid && !x.ArrangeIsValid) // ensure the element has been measured before arranging
                .Take(arrangeBatch);
            foreach (ArrangableElement element in elementsToRearrange)
            {
                log.LogDebug($"Arrange starting for {element.Name} of type {element.GetType().Name}");
                // an invalidated arrange indicates the element wants to place itself in a different location within the same available space.
                element.Arrange(element.PrevPlacementRect);
                log.LogDebug($"Arrange completed for {element.Name}");
            }
        }
    }
}
