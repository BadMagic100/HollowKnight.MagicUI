﻿using System;
using System.Collections.Generic;

namespace MagicUI.Core
{
    /// <summary>
    /// A property declaration on a parent object that requires its children to specify parameters
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    public class AttachedProperty<TProperty>
    {
        private readonly Dictionary<ArrangableElement, TProperty> valueLookup;
        private readonly TProperty defaultValue;
        private readonly ChangeAction actionOnChange;
        private readonly Predicate<TProperty>? validate;

        /// <summary>
        /// Creates a new attached property
        /// </summary>
        /// <param name="defaultValue">The default value of the property</param>
        /// <param name="actionOnChange">The action to take in the layout system when this property changes</param>
        /// <param name="validate"></param>
        public AttachedProperty(TProperty defaultValue, ChangeAction actionOnChange = ChangeAction.None, Predicate<TProperty>? validate = null)
        {
            valueLookup = new Dictionary<ArrangableElement, TProperty>();
            this.defaultValue = defaultValue;
            this.actionOnChange = actionOnChange;
            this.validate = validate;
        }

        private void SetValueAndNotify(ArrangableElement element, TProperty value)
        {
            valueLookup[element] = value;
            actionOnChange.Notify(element);
        }

        /// <summary>
        /// Sets the value of this property on an element
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="value">The new value</param>
        public void Set(ArrangableElement element, TProperty value)
        {
            if (validate?.Invoke(value) == false)
            {
                throw new ArgumentException($"Property failed validation for value {value}", nameof(value));
            }
            // if we have the element, we need to check if it's a different value before notifying
            if (valueLookup.ContainsKey(element))
            {
                TProperty oldValue = valueLookup[element];
                bool areValuesDifferentNullState = (oldValue == null && value != null) || (oldValue != null && value == null);
                bool newValueIsDifferent = oldValue != null && !oldValue.Equals(value);
                if (areValuesDifferentNullState || newValueIsDifferent)
                {
                    SetValueAndNotify(element, value);
                }
            }
            else
            {
                SetValueAndNotify(element, value);
            }
        }

        /// <summary>
        /// Gets the value of this property on an element
        /// </summary>
        /// <param name="element">The element</param>
        public TProperty Get(ArrangableElement element)
        {
            if (valueLookup == null || !valueLookup.ContainsKey(element))
            {
                return defaultValue;
            }
            return valueLookup[element];
        }
    }

    /// <summary>
    /// Chainable extension methods to apply attached properties to elements inline at declaration time
    /// </summary>
    public static class ApplyAttachedPropertyChainables
    {
        /// <summary>
        /// Applies a value to an attached property on a given element and returns the element for chaining
        /// </summary>
        /// <typeparam name="T">The type of the element</typeparam>
        /// <typeparam name="U">The type of the property</typeparam>
        /// <param name="element">The element to attach a property to</param>
        /// <param name="property">The property to attach</param>
        /// <param name="value">The new value of the property</param>
        /// <returns>The original element for chaining</returns>
        public static T WithProp<T, U>(this T element, AttachedProperty<U> property, U value) where T : ArrangableElement
        {
            property.Set(element, value);
            return element;
        }
    }
}
