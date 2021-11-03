using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// Generic editor history
    /// </summary>
    /// <typeparam name="T">The type of data to save in the history</typeparam>
    public class EditorHistory<T>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="applyChange">The action to use to apply a change from the history</param>
        public EditorHistory(Action<T> applyChange)
        {
            // Set properties
            ApplyChange = applyChange;
            History = new List<EditorHistoryEntry<T>>();

            // Clear
            ClearHistory();
        }

        /// <summary>
        /// The action to use to apply a change from the history
        /// </summary>
        protected Action<T> ApplyChange { get; }

        /// <summary>
        /// The max number of entries to save in the history
        /// </summary>
        protected virtual int MaxHistoryEntries => 50;

        /// <summary>
        /// The history of entries
        /// </summary>
        protected List<EditorHistoryEntry<T>> History { get; set; }

        /// <summary>
        /// The current index in the history list
        /// </summary>
        protected int HistoryIndex { get; set; }

        /// <summary>
        /// Undo and restore to the state of the previous entry
        /// </summary>
        public void Undo()
        {
            // Make sure we can undo
            if (HistoryIndex < 1)
                return;

            // Go back a step
            HistoryIndex--;

            // Apply the change
            ApplyChange(History[HistoryIndex].Before);

            Debug.Log($"History undo with length {History.Count} and index {HistoryIndex}");
        }

        /// <summary>
        /// Redo and restore to the state of the next entry
        /// </summary>
        public void Redo()
        {
            // Make sure we can redo
            if (!(HistoryIndex < History.Count))
                return;

            // Go forward a step
            HistoryIndex++;

            // Apply the change
            ApplyChange(History[HistoryIndex - 1].After);

            Debug.Log($"History redo with length {History.Count} and index {HistoryIndex}");
        }

        /// <summary>
        /// Add a new entry to the history
        /// </summary>
        /// <param name="entry">The entry to add</param>
        public void AddToHistory(EditorHistoryEntry<T> entry)
        {
            // Remove entries above current index
            History.RemoveRange(HistoryIndex, History.Count - HistoryIndex);

            // Add the entry
            History.Add(entry);

            // Make sure we don't exceed the limit
            if (History.Count > MaxHistoryEntries)
                History.RemoveRange(0, History.Count - MaxHistoryEntries);

            // Update index
            HistoryIndex = History.Count;

            Debug.Log($"Entry has been added to history with length {History.Count} and index {HistoryIndex}");
        }

        /// <summary>
        /// Clears the history of entries
        /// </summary>
        public void ClearHistory()
        {
            // Clear the collection
            History.Clear();

            // Reset the index
            HistoryIndex = 0;
        }
    }
}