using System.Collections;
using System.Collections.Generic;

namespace MagicUI.Core
{
    /// <summary>
    /// A list of arrangable elements with reference to hierarchical parents
    /// </summary>
    /// <inheritdoc/>
    public class ParentedElementList : IList<ArrangableElement>
    {
        private readonly List<ArrangableElement> logicalChildren = new();
        private readonly ArrangableElement logicalParent;

        internal ParentedElementList(ArrangableElement logicalParent)
        {
            this.logicalParent = logicalParent;
        }

        /// <inheritdoc/>
        public ArrangableElement this[int index] 
        { 
            get => logicalChildren[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        /// <inheritdoc/>
        public int Count => logicalChildren.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(ArrangableElement item) => Insert(logicalChildren.Count, item);

        /// <summary>
        /// Removes and destroys all elements in the list.
        /// </summary>
        /// <inheritdoc/>
        public void Clear()
        {
            while (logicalChildren.Count > 0)
            {
                RemoveAt(0);
            }
        }

        /// <inheritdoc/>
        public bool Contains(ArrangableElement item) => logicalChildren.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(ArrangableElement[] array, int arrayIndex) => logicalChildren.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<ArrangableElement> GetEnumerator() => logicalChildren.GetEnumerator();

        /// <inheritdoc/>
        public int IndexOf(ArrangableElement item) => logicalChildren.IndexOf(item);

        /// <inheritdoc/>
        public void Insert(int index, ArrangableElement item)
        {
            item.LogicalParent = logicalParent;
            logicalChildren.Insert(index, item);
            logicalParent.InvalidateMeasure();
        }

        /// <summary>
        /// Removes and destroys the first occurrence of a specific element from the list.
        /// </summary>
        /// <inheritdoc/>
        public bool Remove(ArrangableElement item)
        {
            int idx = IndexOf(item);
            if (idx >= 0)
            {
                RemoveAt(idx);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes and destroys the element at the specified index.
        /// </summary>
        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            ArrangableElement element = logicalChildren[index];
            logicalChildren.RemoveAt(index);
            element.Destroy();
            element.LogicalParent = null;
            logicalParent.InvalidateMeasure();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
