using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    public interface IFunctionalEventTracer
    {
        event FunctionalEventTracingHandler EventRaising;
        event FunctionalEventTracingHandler EventRaised;
    }
}
