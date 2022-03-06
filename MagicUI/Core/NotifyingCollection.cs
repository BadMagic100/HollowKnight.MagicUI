using System.Collections;
using System.Collections.Generic;

namespace MagicUI.Core
{
    /// <summary>
    /// A collection that can notify its owner's layout system when its contents are updated
    /// </summary>
    /// <typeparam name="T">The type of contents</typeparam>
    public class NotifyingCollection<T> : ICollection<T>
    {
        private readonly ArrangableElement owner;
        private readonly ChangeAction actionOnChange;

        private readonly List<T> items = new();

        /// <summary>
        /// Creates a notifying collection
        /// </summary>
        /// <param name="owner">The collection owner</param>
        /// <param name="actionOnChange">The action to take when a change occurs</param>
        public NotifyingCollection(ArrangableElement owner, ChangeAction actionOnChange)
        {
            this.owner = owner;
            this.actionOnChange = actionOnChange;
        }

        /// <inheritdoc/>
        public int Count => items.Count;

        /// <summary>
        /// Indexing operator for this collection
        /// </summary>
        /// <param name="idx">The index</param>
        public T this[int idx]
        {
            get => items[idx];
            set => items[idx] = value;
        }

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(T item)
        {
            items.Add(item);
            actionOnChange.Notify(owner);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            items.Clear();
            actionOnChange.Notify(owner);
        }

        /// <inheritdoc/>
        public bool Contains(T item) => items.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            if (items.Remove(item))
            {
                actionOnChange.Notify(owner);
                return true;
            }
            else
            {
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
