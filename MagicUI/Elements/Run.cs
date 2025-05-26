using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MagicUI.Elements;

/// <summary>
/// Represents a segment of text with optional formatting attributes like bold, italic, size, and color.
/// Used within <see cref="TextObject"/> to create rich text with mixed formatting.
/// </summary>
public record Run
{
    /// <summary>
    /// Gets the text content of this run.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Gets whether the text should be displayed in bold.
    /// </summary>
    public bool Bold { get; init; } = false;

    /// <summary>
    /// Gets whether the text should be displayed in italic.
    /// </summary>
    public bool Italic { get; init; } = false;

    /// <summary>
    /// Gets the optional font size override for this text run. If null, uses the parent TextObject's font size.
    /// </summary>
    public int? Size { get; init; } = null;

    /// <summary>
    /// Gets the optional color override for this text run. If null, uses the parent TextObject's color.
    /// </summary>
    public Color? Color { get; init; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Run"/> class with the specified text.
    /// </summary>
    /// <param name="text">The text content of this run.</param>
    public Run(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Converts the run to its rich text string representation with all formatting applied.
    /// </summary>
    /// <returns>A string containing the text with Unity rich text tags for the applied formatting.</returns>
    public override string ToString()
    {
        string formatted = Text;
        if (Size != null)
        {
            formatted = WrapTag(formatted, "size", Size.Value.ToString());
        }
        if (Color != null)
        {
            string colorText = "#" + ColorUtility.ToHtmlStringRGBA(Color.Value);
            formatted = WrapTag(formatted, "color", colorText);
        }
        if (Bold)
        {
            formatted = WrapTag(formatted, "b");
        }
        if (Italic)
        {
            formatted = WrapTag(formatted, "i");
        }
        return formatted;
    }

    /// <summary>
    /// Wraps text content with a Unity rich text tag.
    /// </summary>
    /// <param name="text">The text to wrap with the tag.</param>
    /// <param name="tag">The tag name to use.</param>
    /// <param name="value">Optional value for the tag.</param>
    /// <returns>The text wrapped in the specified tag.</returns>
    private string WrapTag(string text, string tag, string? value = null)
    {
        StringBuilder sb = new();
        sb.Append($"<{tag}");
        if (value != null)
        {
            sb.Append($"={value}");
        }
        sb.Append('>');
        sb.Append(text);
        sb.Append($"</{tag}>");
        return sb.ToString();
    }
}

/// <summary>
/// Represents a modifiable collection of <see cref="Run"/> objects used to create rich text with mixed formatting.
/// When attached to a <see cref="TextObject"/>, changes to the collection automatically update the text display.
/// </summary>
public class RunCollection : IList<Run>
{
    private readonly List<Run> runs;

    /// <summary>
    /// Occurs when items in the collection are added, removed, or when the collection is cleared.
    /// </summary>
    public event EventHandler? CollectionChanged;

    /// <summary>
    /// Initializes a new empty instance of the <see cref="RunCollection"/> class.
    /// </summary>
    /// <example>
    /// <code>
    /// var runs = new RunCollection();
    /// runs.Add(new Run("Hello") { Bold = true });
    /// runs.Add(new Run("World") { Color = Color.red });
    /// </code>
    /// </example>
    public RunCollection()
    {
        runs = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RunCollection"/> class with the specified array of runs.
    /// </summary>
    /// <param name="runs">An array of <see cref="Run"/> objects to initialize the collection with.</param>
    /// <example>
    /// <code>
    /// var runs = new RunCollection(
    ///     new Run("Hello") { Bold = true },
    ///     new Run("World") { Color = Color.red }
    /// );
    /// </code>
    /// </example>
    public RunCollection(params Run[] runs)
    {
        this.runs = [.. runs];
    }

    private void OnCollectionChanged()
    {
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public Run this[int index]
    {
        get => runs[index];
        set
        {
            runs[index] = value;
            OnCollectionChanged();
        }
    }

    /// <inheritdoc/>
    public int Count => runs.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public IEnumerator<Run> GetEnumerator() => ((IEnumerable<Run>)runs).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public int IndexOf(Run item) => runs.IndexOf(item);

    /// <inheritdoc/>
    public void Insert(int index, Run item)
    {
        runs.Insert(index, item);
        OnCollectionChanged();
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        runs.RemoveAt(index);
        OnCollectionChanged();
    }

    /// <inheritdoc/>
    public void Add(Run item)
    {
        runs.Add(item);
        OnCollectionChanged();
    }

    /// <summary>
    /// Adds a collection of <see cref="Run"/> items to the current collection.
    /// </summary>
    /// <remarks>After adding the items, this method triggers a collection changed notification.</remarks>
    /// <param name="items">The collection of <see cref="Run"/> items to add. Cannot be null.</param>
    public void AddRange(IEnumerable<Run> items)
    {
        runs.AddRange(items);
        OnCollectionChanged();
    }

    /// <inheritdoc/>
    public void Clear()
    {
        runs.Clear();
        OnCollectionChanged();
    }

    /// <inheritdoc/>
    public bool Contains(Run item) => runs.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(Run[] array, int arrayIndex) => runs.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Remove(Run item)
    {
        bool result = runs.Remove(item);
        if (result)
        {
            OnCollectionChanged();
        }
        return result;
    }

    /// <summary>
    /// Joins the given runs together using the specified separator runs between each element.
    /// </summary>
    /// <param name="separator">A collection of runs to insert between each element.</param>
    /// <param name="runs">The sequence of runs to join together.</param>
    /// <returns>A new <see cref="RunCollection"/> containing all elements with the separator between them.</returns>
    public static RunCollection Join(RunCollection separator, IEnumerable<Run> runs)
    {
        IEnumerable<Run> result = runs.Aggregate(new List<Run>(), (acc, next) =>
        {
            if (acc.Count != 0)
            {
                acc.AddRange(separator);
            }
            acc.Add(next);
            return acc;
        });
        return [.. result];
    }

    /// <summary>
    /// Joins the given runs together using the specified string separator between each element.
    /// </summary>
    /// <param name="separator">The string to insert between each element.</param>
    /// <param name="runs">The sequence of runs to join together.</param>
    /// <returns>A new <see cref="RunCollection"/> containing all elements with the separator between them.</returns>
    public static RunCollection Join(string separator, IEnumerable<Run> runs)
    {
        return Join([new Run(separator)], runs);
    }

    /// <summary>
    /// Joins the given run collections together using the specified separator runs between each collection.
    /// </summary>
    /// <param name="separator">A collection of runs to insert between each collection.</param>
    /// <param name="runs">The sequence of run collections to join together.</param>
    /// <returns>A new <see cref="RunCollection"/> containing all elements with the separator between them.</returns>
    public static RunCollection Join(RunCollection separator, IEnumerable<RunCollection> runs)
    {
        IEnumerable<Run> result = runs.Aggregate(new List<Run>(), (acc, next) =>
        {
            if (acc.Count != 0)
            {
                acc.AddRange(separator);
            }
            acc.AddRange(next);
            return acc;
        });
        return [.. result];
    }

    /// <summary>
    /// Joins the given run collections together using the specified string separator between each collection.
    /// </summary>
    /// <param name="separator">The string to insert between each collection.</param>
    /// <param name="runs">The sequence of run collections to join together.</param>
    /// <returns>A new <see cref="RunCollection"/> containing all elements with the separator between them.</returns>
    public static RunCollection Join(string separator, IEnumerable<RunCollection> runs)
    {
        return Join([new Run(separator)], runs);
    }
}
