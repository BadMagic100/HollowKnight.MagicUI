using System.Collections;
using System.Collections.Generic;

namespace MagicUI
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

        public ArrangableElement this[int index] 
        { 
            get => logicalChildren[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public int Count => logicalChildren.Count;

        public bool IsReadOnly => false;

        public void Add(ArrangableElement item) => Insert(logicalChildren.Count, item);

        public void Clear()
        {
            while (logicalChildren.Count > 0)
            {
                RemoveAt(0);
            }
        }

        public bool Contains(ArrangableElement item) => logicalChildren.Contains(item);

        public void CopyTo(ArrangableElement[] array, int arrayIndex) => logicalChildren.CopyTo(array, arrayIndex);

        public IEnumerator<ArrangableElement> GetEnumerator() => logicalChildren.GetEnumerator();

        public int IndexOf(ArrangableElement item) => logicalChildren.IndexOf(item);

        public void Insert(int index, ArrangableElement item)
        {
            item.LogicalParent = logicalParent;
            logicalChildren.Insert(index, item);
            logicalParent.InvalidateMeasure();
        }

        public bool Remove(ArrangableElement item)
        {
            if (IndexOf(item) >= 0)
            {
                RemoveAt(IndexOf(item));
                return true;
            }
            else
            {
                return false;
            }
        }

        // todo: only allow removing when stuff is destroyed and/or destroy stuff when elements are removed
        public void RemoveAt(int index)
        {
            ArrangableElement element = logicalChildren[index];
            logicalChildren.RemoveAt(index);
            element.LogicalParent = null;
            logicalParent.InvalidateMeasure();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
