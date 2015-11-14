using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FunctionalTreeLibrary
{
    public static class FunctionalTreeHelper
    {

        public static FunctionalTree GetFunctionalTree(IFunctionalTreeElement source)
        {
            if (source == null)
                return null;

            return FunctionalTreeManager.Instance.GetOrCreateFunctionalTree(source);
        }

        public static T FindFunctionalAncestor<T>(IFunctionalTreeElement element, Predicate<T> predicate = null)
            where T : class, IFunctionalTreeElement
        {
            try
            {
                IFunctionalTreeElement ancestor = GetFunctionalParent(element);
                while (ancestor != null)
                {
                    T ancestorAsT = ancestor as T;
                    if (ancestorAsT != null && (predicate == null || predicate(ancestorAsT)))
                        return ancestorAsT;

                    ancestor = GetFunctionalParent(ancestor);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static IFunctionalTreeElement GetFunctionalParent(IFunctionalTreeElement element)
        {
            if (element == null)
                return null;

            FunctionalTreeElement treeElement = GetFunctionalTree(element).GetFunctionalElement(element);
            if (treeElement.Parent != null)
                return treeElement.Parent.Element;

            return null;
        }

        public static IEnumerable<IFunctionalTreeElement> GetFunctionalChildren(IFunctionalTreeElement element)
        {
            if (element == null)
                return Enumerable.Empty<IFunctionalTreeElement>();

            FunctionalTreeElement treeElement = GetFunctionalTree(element).GetFunctionalElement(element);
            return treeElement.Children.Select(cur => cur.Element);
        }

        public static T FindFunctionalDescendent<T>(IFunctionalTreeElement element, Predicate<T> predicate = null)
            where T : class
        {
            IEnumerable<IFunctionalTreeElement> children = GetFunctionalChildren(element);
            for (int i = 0; i < children.Count(); i++)
            {
                IFunctionalTreeElement child = children.ElementAtOrDefault(i);
                if (child is T)
                    return child as T;
                else
                {
                    T childOfChild = FindFunctionalDescendent<T>(child);
                    if (childOfChild != null && (predicate == null || predicate(childOfChild)))
                        return childOfChild;
                }
            }
            return null;
        }

        public static IFunctionalTreeElement GetFunctionalRoot(IFunctionalTreeElement source)
        {
            if (source == null)
                return null;

            return GetFunctionalTree(source).Root;
        }

    }
}
