using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{

    public sealed class FunctionalEvent
    {

        #region Variables

        internal FunctionalEventTracer EventTracer { get; private set; }

        public Type HandlerType { get; private set; }
        public Type OwnerType { get; private set; }
        public FunctionalStrategy FunctionalStretegy { get; private set; }
        public string Name { get; private set; }

        #endregion

        internal FunctionalEvent(string name, FunctionalStrategy functionalStretegy, Type handlerType, Type ownerType)
        {
            Name = name;
            FunctionalStretegy = functionalStretegy;
            HandlerType = handlerType;
            OwnerType = ownerType;

            EventTracer = new FunctionalEventTracer();
        }

        public IFunctionalEventTracer GetEventTracer()
        {
            return EventTracer;
        }

    }
}
