using UnityEngine;

namespace MagicUI
{
    /// <summary>
    /// Root class for an arrangable element with multiple children
    /// </summary>
    public abstract class Layout : ArrangableElement
    {
        /// <summary>
        /// The children of this layout
        /// </summary>
        public ParentedElementList Children { get; }

        /// <inheritdoc/>
        public override GameObject? VisualParent 
        { 
            get => base.VisualParent;
            set
            {
                // reassign the visual parent of any child that hasn't been tampered with - i.e. the visual parent is the same as this
                // element's visual parent. otherwise, someone has modified a branch of the visual tree by hand and we'll let them have their fun.
                foreach (ArrangableElement child in Children)
                {
                    if (child.VisualParent == VisualParent)
                    {
                        child.VisualParent = value;
                    }
                }
                base.VisualParent = value;
            }
        }

        public Layout(string name = "New Layout") : base(name)
        {
            Children = new ParentedElementList(this);
        }
    }
}
