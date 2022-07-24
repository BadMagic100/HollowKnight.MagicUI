using System;
namespace MagicUI.Styles
{
    /// <summary>
    /// Indicates that a style should be generated for a given class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class StylableAttribute : Attribute { }

    /// <summary>
    /// Indicates that a property of a class should not be included in the style.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class StyleIgnoreAttribute : Attribute { }

    /// <summary>
    /// A generic style that can apply a set of modifications to an object
    /// </summary>
    public interface IStyle<T>
    {
        /// <summary>
        /// Applies the style to the target object
        /// </summary>
        /// <param name="target">The object to apply the style to</param>
        void Apply(T target);
    }
}
