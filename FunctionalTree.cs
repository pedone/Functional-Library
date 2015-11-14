using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{

    public class FunctionalTree
    {

        #region Events

        internal event FunctionalChildChangedInternalEventHandler FunctionalChildAttachedInternal;
        internal event FunctionalChildChangedInternalEventHandler FunctionalChildDetachedInternal;
        public event FunctionalChildChangedEventHandler FunctionalChildAttached;
        public event FunctionalChildChangedEventHandler FunctionalChildDetached;

        #endregion

        #region Variables

        internal FunctionalTreeElement RootElement { get; private set; }

        #endregion

        #region Properties

        public IFunctionalTreeElement Root
        {
            get
            {
                if (RootElement != null)
                    return RootElement.Element as IFunctionalTreeElement;

                return null;
            }
        }

        #endregion

        internal FunctionalTree(IFunctionalTreeElement root)
        {
            RootElement = new FunctionalTreeElement(root, null);
        }
        internal FunctionalTree(FunctionalTreeElement root)
        {
            RootElement = root;
        }

        internal FunctionalTreeElement GetFunctionalElement(IFunctionalTreeElement element)
        {
            if (RootElement == null)
                return null;
            if (RootElement.Element == element)
                return RootElement;

            return GetFunctionalElement(RootElement.Children, element);
        }

        private FunctionalTreeElement GetFunctionalElement(IEnumerable<FunctionalTreeElement> items, IFunctionalTreeElement element)
        {
            foreach (var item in items)
            {
                if (item.Element == element)
                    return item;

                FunctionalTreeElement subElement = GetFunctionalElement(item.Children, element);
                if (subElement != null)
                    return subElement;
            }

            return null;
        }

        internal bool IsChild(IFunctionalTreeElement element)
        {
            return GetFunctionalElement(element) != null;
        }

        internal void FireFunctionalChildAttached(FunctionalTreeElement newChild, FunctionalTreeElement parent)
        {
            if (FunctionalChildAttachedInternal != null && IsChild(newChild.Element))
                FunctionalChildAttachedInternal(newChild, parent, this);
            if (FunctionalChildAttached != null && IsChild(newChild.Element))
                FunctionalChildAttached(newChild.Element, parent.Element, this);
        }

        internal void FireFunctionalChildDetached(FunctionalTreeElement oldChild, FunctionalTreeElement parent)
        {
            if (FunctionalChildDetachedInternal != null)
                FunctionalChildDetachedInternal(oldChild, parent, this);
            if (FunctionalChildDetached != null)
                FunctionalChildDetached(oldChild.Element, parent.Element, this);
        }

    }

}
