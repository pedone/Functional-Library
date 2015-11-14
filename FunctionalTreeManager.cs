using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace FunctionalTreeLibrary
{
    internal class FunctionalTreeManager
    {

        #region Events

        internal event FunctionalChildChangedInternalEventHandler FunctionalElementAttachedToTree;
        internal event FunctionalChildChangedInternalEventHandler FunctionalElementDetachedFromTree;

        #endregion

        #region Variables

        internal static FunctionalTreeManager Instance { get; private set; }
        private List<FunctionalTree> FunctionalTreeList { get; set; }

        #endregion

        static FunctionalTreeManager()
        {
            Instance = new FunctionalTreeManager();
        }

        private FunctionalTreeManager()
        {
            FunctionalTreeList = new List<FunctionalTree>();
        }

        private FunctionalTree GetFunctionalTree(IFunctionalTreeElement element)
        {
            return FunctionalTreeList.FirstOrDefault(cur => cur.IsChild(element));
        }

        internal FunctionalTree GetOrCreateFunctionalTree(IFunctionalTreeElement element)
        {
            if (element == null)
                return null;

            FunctionalTree tree = GetFunctionalTree(element);
            if (tree != null)
                return tree;

            tree = new FunctionalTree(element);
            tree.FunctionalChildAttachedInternal += Tree_FunctionalChildAttached;
            tree.FunctionalChildDetachedInternal += Tree_FunctionalChildDetached;

            FunctionalTreeList.Add(tree);
            return tree;
        }

        private int RemoveTreesWithElement(IFunctionalTreeElement element)
        {
            IEnumerable<FunctionalTree> treesToRemove = FunctionalTreeList.Where(cur => cur.IsChild(element));
            int treeCount = treesToRemove.Count();
            foreach (FunctionalTree tree in treesToRemove.ToList())
            {
                tree.FunctionalChildAttachedInternal -= Tree_FunctionalChildAttached;
                tree.FunctionalChildDetachedInternal -= Tree_FunctionalChildDetached;

                FunctionalTreeList.Remove(tree);
            }

            return treeCount;
        }

        private void Tree_FunctionalChildDetached(FunctionalTreeElement child, FunctionalTreeElement parent, FunctionalTree functionalTree)
        {
            //Make sure there are no other trees with child in it
            int removedTreeCount = RemoveTreesWithElement(child.Element);
            //TODO fix and remove unused trees effectively
            //Debug.Assert(removedTreeCount == 0, "There shouldn't have been any trees left with child in it.");

            if (child.HasData)
            {
                //save child in new functional tree to save the data
                FunctionalTree tree = new FunctionalTree(child);
                tree.FunctionalChildAttachedInternal += Tree_FunctionalChildAttached;
                tree.FunctionalChildDetachedInternal += Tree_FunctionalChildDetached;

                FunctionalTreeList.Add(tree);
            }

            if (FunctionalElementDetachedFromTree != null)
                FunctionalElementDetachedFromTree(child, parent, functionalTree);
        }

        private void Tree_FunctionalChildAttached(FunctionalTreeElement child, FunctionalTreeElement parent, FunctionalTree functionalTree)
        {
            //remove all other trees with the element or any child in it
            IEnumerable<FunctionalTreeElement> childSubElements = GetAllElements(child);
            IEnumerable<FunctionalTree> treesToRemove = FunctionalTreeList.Where(tree => childSubElements.Any(subElement => tree.IsChild(subElement.Element)));
            //Debug.Assert((treesToRemove.Count() - 1) <= 1, "There shouldn't be more than one tree to remove.");

            foreach (FunctionalTree tree in treesToRemove.ToList())
            {
                if (tree == functionalTree)
                    continue;

                tree.FunctionalChildAttachedInternal -= Tree_FunctionalChildAttached;
                tree.FunctionalChildDetachedInternal -= Tree_FunctionalChildDetached;

                FunctionalTreeList.Remove(tree);
            }

            if (FunctionalElementAttachedToTree != null)
                FunctionalElementAttachedToTree(child, parent, functionalTree);
        }

        private IEnumerable<FunctionalTreeElement> GetAllElements(FunctionalTreeElement element)
        {
            List<FunctionalTreeElement> elements = new List<FunctionalTreeElement>();
            elements.Add(element);

            foreach (FunctionalTreeElement child in element.Children)
                elements.AddRange(GetAllElements(child));

            return elements;
        }

    }
}
