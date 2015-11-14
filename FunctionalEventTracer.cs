using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    internal class FunctionalEventTracer : IFunctionalEventTracer
    {

        #region Events

        public event FunctionalEventTracingHandler EventRaising;
        public event FunctionalEventTracingHandler EventRaised;

        #endregion

        internal FunctionalEventTracer()
        { }

        internal void RaiseEventRaising(FunctionalEvent functionalEvent, FunctionalEventTracingArgs eventRaisingArgs)
        {
            if (EventRaising != null)
                EventRaising(functionalEvent, eventRaisingArgs);
        }

        internal void RaiseEventRaised(FunctionalEvent functionalEvent, FunctionalEventTracingArgs eventRaisedArgs)
        {
            if (EventRaised != null)
                EventRaised(functionalEvent, eventRaisedArgs);
        }

    }
}
