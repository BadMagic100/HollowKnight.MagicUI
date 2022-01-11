using UnityEngine;

namespace MagicUI.Core
{
    /// <summary>
    /// An element that implements visible components by wrapping a Unity <see cref="UnityEngine.GameObject"/>
    /// </summary>
    public interface IGameObjectWrapper
    {
        /// <summary>
        /// The <see cref="UnityEngine.GameObject"/> underlying this UI element
        /// </summary>
        public GameObject GameObject { get; }
    }
}
