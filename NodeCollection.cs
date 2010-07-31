﻿/*
* Vha.AOML
* Copyright (C) 2010 Remco van Oosterhout
* See Credits.txt for all aknowledgements.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; version 2 of the License only.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Vha.AOML
{
    public class NodeCollection : IEnumerable<Node>
    {
        /// <summary>
        /// Returns the amount of nodes currently contained within this object
        /// </summary>
        public int Count { get { return this._nodes.Count; } }

        /// <summary>
        /// Returns the current position of the 
        /// This value is always 0 or greater and less or equal to Count.
        /// </summary>
        public int Current
        {
            get { return this._current; }
            set
            {
                if (value < 0) throw new ArgumentException("Expecting value greater than zero");
                if (value > this.Count) throw new ArgumentException("Expecting value smaller or equal to Count");
                this._current = value;
            }
        }

        /// <summary>
        /// Clears all data contained within this parser, including parse results
        /// </summary>
        public void Clear()
        {
            this._nodes.Clear();
            this._current = 0;
        }

        /// <summary>
        /// Resets the parser back to its initial state after Parse was called
        /// </summary>
        public void Reset()
        {
            this._current = 0;
        }

        /// <summary>
        /// Returns the next Node in the buffer and advances the internal state
        /// </summary>
        /// <returns>A valid Node instance or null if the end is reached</returns>
        public Node Next()
        {
            if (this.Count == 0) return null;
            if (this.Current == this.Count) return null;
            Node node = this._nodes[this._current];
            this._current++;
            return node;
        }

        /// <summary>
        /// Returns the next Node without advancing the internal state
        /// </summary>
        /// <param name="count">The amount of nodes to skip before returning the next node</param>
        /// <returns>A valid Node instance or null if the end is reached</returns>
        public Node Peek(int count)
        {
            int offset = this._current + count;
            if (offset + 1 >= this.Count) return null;
            if (offset < 0) return null;
            return this._nodes[offset];
        }

        public void Add(Node node)
        {
            // Prevent sillyness
            if (node == null) throw new ArgumentNullException();
            // Merge content nodes
            if (this._nodes.Count > 0 && node.Type == NodeType.Content)
            {
                Node top = this._nodes[this._nodes.Count - 1];
                if (top.Type == NodeType.Content)
                {
                    ContentNode c1 = (ContentNode)top;
                    ContentNode c2 = (ContentNode)node;
                    c1 = new ContentNode(c1.Value + c2.Value, c1.Raw + c2.Raw);
                    this._nodes[this._nodes.Count - 1] = c1;
                    return;
                }
            }
            // Just append it
            this._nodes.Add(node);
        }

        public void InsertBefore(Node target, Node node)
        {
            int index = this._nodes.IndexOf(target);
            // Prevent sillyness
            if (index < 0)
                throw new ArgumentException("target is not part of this collection");
            // Insert
            this._nodes.Insert(index, node);
            // Update current so we don't double an element
            if (index < this._current)
                this._current++;
        }

        public void InsertAfter(Node target, Node node)
        {
            int index = this._nodes.IndexOf(target);
            // Prevent sillyness
            if (index < 0)
                throw new ArgumentException("target is not part of this collection");
            // Insert
            this._nodes.Insert(index + 1, node);
            // Update current so we don't double an element
            if (index + 1 < this._current)
                this._current++;
        }

        public void Replace(Node target, Node node)
        {
            int index = this._nodes.IndexOf(target);
            // Prevent sillyness
            if (index < 0)
                throw new ArgumentException("target is not part of this collection");
            // Replace
            this._nodes[index] = node;
        }

        public void Remove(Node target)
        {
            int index = this._nodes.IndexOf(target);
            // Prevent sillyness
            if (index < 0)
                throw new ArgumentException("target is not part of this collection");
            this._nodes.RemoveAt(index);
            // Update current so we don't accidentally skip an element
            if (index < this._current)
                this._current--;
            if (this._current > this.Count)
                this._current = this.Count;
        }

        /// <summary>
        /// Converts this collection into a flat array
        /// </summary>
        /// <returns>A new array containing the elements contained in this collection</returns>
        public Node[] ToArray()
        {
            return this._nodes.ToArray();
        }

        /// <summary>
        /// Returns a type safe enumerator that iterates through this collection
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            return this._nodes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through this collection
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._nodes.GetEnumerator();
        }

        #region Internal
        private List<Node> _nodes = new List<Node>();
        private int _current = 0;
        #endregion
    }
}
