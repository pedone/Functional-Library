using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    internal class FunctionalEventHandlerContainer
    {

        #region Variables

        public Delegate Handler { get; private set; }
        public bool HandledEventsToo { get; private set; }

        #endregion

        public FunctionalEventHandlerContainer(Delegate handler, bool handledEventsToo = false)
        {
            Handler = handler;
            HandledEventsToo = handledEventsToo;
        }
    }
}
