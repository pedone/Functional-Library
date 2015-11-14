using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FunctionalTreeLibrary
{

    internal class FunctionalTreeElement
    {

        #region Variables

        public FunctionalTreeElement Parent { get; private set; }
        public IFunctionalTreeElement Element { get; private set; }

        private List<FunctionalTreeElement> _children;
        public ReadOnlyCollection<FunctionalTreeElement> Children { get; private set; }

        internal Delegate AttachedToTreeHandlerList { get; private set; }
        internal Delegate DetachedFromTreeHandlerList { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// True, if the element contains any data that has to be preserved.
        /// </summary>
        internal bool HasData
        {
            get
            {
                if (Element == null)
                    return false;

                return Children.Count > 0 ||
                    (AttachedToTreeHandlerList != null && AttachedToTreeHandlerList.GetInvocationList().Count() > 0) ||
                    (DetachedFromTreeHandlerList != null && DetachedFromTreeHandlerList.GetInvocationList().Count() > 0);
            }
        }

        #endregion

        internal FunctionalTreeElement(IFunctionalTreeElement element, FunctionalTreeElement parent)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element is null.");

            Element = element;
            Parent = parent;

            _children = new List<FunctionalTreeElement>();
            Children = new ReadOnlyCollection<FunctionalTreeElement>(_children);
        }

        internal void AddChild(IFunctionalTreeElement child)
        {
            if (Children.Any(cur => cur.Element == child))
                return;

            if (FunctionalTreeHelper.GetFunctionalParent(child) != null)
                throw new ArgumentException("Element is already part of a functional tree.");

            FunctionalTreeElement childElement = FunctionalTreeHelper.GetFunctionalTree(child).RootElement;
            childElement.Parent = this;
            _children.Add(childElement);

            childElement.FireAttachedToTreeHandler();

            FunctionalTree parentTree = FunctionalTreeHelper.GetFunctionalTree(Element);
            parentTree.FireFunctionalChildAttached(childElement, this);
        }

        internal bool RemoveChild(IFunctionalTreeElement child)
        {
            FunctionalTreeElement childElement = Children.FirstOrDefault(cur => cur.Element == child);
            if (childElement == null)
                return false;

            _children.Remove(childElement);
            childElement.Parent = null;

            childElement.FireDetachedFromTreeHandler();

            FunctionalTree parentTree = FunctionalTreeHelper.GetFunctionalTree(Element);
            parentTree.FireFunctionalChildDetached(childElement, this);

            return true;
        }

        internal void DisconnectFromTree()
        {
            if (Parent != null)
                Parent.RemoveChild(Element);
        }

        internal void AddAttachedToTreeHandler(FunctionalTreeEventHandler handler)
        {
            if (AttachedToTreeHandlerList == null)
                AttachedToTreeHandlerList = handler;
            else
                Delegate.Combine(AttachedToTreeHandlerList, handler);

            //Handlers added to the root are different, since they're not fired on addChild/removeChild since they're not really added.
            //That's why handlers from the root are fired right away
            //TODO not sure
            bool isFunctionalTreeRoot = Parent == null;
            if (isFunctionalTreeRoot && handler != null)
                handler(FunctionalTreeHelper.GetFunctionalTree(Element));
        }

        internal void RemoveAttachedToTreeHandler(FunctionalTreeEventHandler handler)
        {
            if (AttachedToTreeHandlerList != null)
                Delegate.Remove(AttachedToTreeHandlerList, handler);

            //Handlers added to the root are different, since they're not fired on addChild/removeChild since they're not really added.
            //That's why handlers from the root are fired right away
            bool isFunctionalTreeRoot = Parent == null;
            if (isFunctionalTreeRoot && handler != null)
                handler(FunctionalTreeHelper.GetFunctionalTree(Element));
        }

        internal void AddDetachedFromTreeHandler(FunctionalTreeEventHandler handler)
        {
            if (DetachedFromTreeHandlerList == null)
                DetachedFromTreeHandlerList = handler;
            else
                Delegate.Combine(DetachedFromTreeHandlerList, handler);
        }

        internal void RemoveDetachedFromTreeHandler(FunctionalTreeEventHandler handler)
        {
            if (DetachedFromTreeHandlerList != null)
                Delegate.Remove(DetachedFromTreeHandlerList, handler);
        }

        private void FireAttachedToTreeHandler()
        {
            if (AttachedToTreeHandlerList == null)
                return;

            foreach (Delegate handler in AttachedToTreeHandlerList.GetInvocationList())
                handler.Method.Invoke(Element, new object[] { FunctionalTreeHelper.GetFunctionalTree(Element) });
        }

        private void FireDetachedFromTreeHandler()
        {
            if (DetachedFromTreeHandlerList == null)
                return;

            foreach (Delegate handler in DetachedFromTreeHandlerList.GetInvocationList())
                handler.Method.Invoke(Element, new object[] { FunctionalTreeHelper.GetFunctionalTree(Element) });
        }

    }

}
