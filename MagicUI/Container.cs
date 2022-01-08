using UnityEngine;

namespace MagicUI
{
    public abstract class Container : ArrangableElement
    {
        /// <summary>
        /// The child of this container
        /// </summary>
        public ArrangableElement? Child { get; set; }

        /// <inheritdoc/>
        public override GameObject? VisualParent
        {
            get => base.VisualParent;
            set
            {
                // reassign the visual parent of any child that hasn't been tampered with - i.e. the visual parent is the same as this
                // element's visual parent. otherwise, someone has modified a branch of the visual tree by hand and we'll let them have their fun.
                if (Child != null && Child.VisualParent == VisualParent)
                {
                    Child.VisualParent = value;
                }
                base.VisualParent = value;
            }
        }

        public Container(string name = "New Container") : base(name)
        {
        }
    }
}
