using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    public enum FunctionalStrategy
    {
        Bubble,
        Tunnel,
        /// <summary>
        /// Start with the root, all tree elements from left to right.
        /// </summary>
        Spread,
        /// <summary>
        /// All children of the parent except for the calling
        /// </summary>
        Siblings,
        /// <summary>
        /// All direct children of the tree element
        /// </summary>
        Children,
        /// <summary>
        /// All descendents of the tree element from left to right
        /// </summary>
        Descendents,
        /// <summary>
        /// Only the parent in the functional tree
        /// </summary>
        Parent
    }
}
