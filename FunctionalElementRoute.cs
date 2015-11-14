using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FunctionalTreeLibrary
{
    internal class FunctionalElementRoute : IEnumerable<IFunctionalTreeElement>
    {

        #region Variables

        private List<IFunctionalTreeElement> _elements;

        #endregion

        internal FunctionalElementRoute()
        {
            _elements = new List<IFunctionalTreeElement>();
        }

        public void Add(IFunctionalTreeElement element)
        {
            _elements.Add(element);
        }

        public void AddRange(IEnumerable<IFunctionalTreeElement> elements)
        {
            _elements.AddRange(elements);
        }

        public int IndexOf(IFunctionalTreeElement element)
        {
            return _elements.IndexOf(element);
        }

        public void Reverse()
        {
            _elements.Reverse();
        }

        public IEnumerator<IFunctionalTreeElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
