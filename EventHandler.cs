using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    
    public delegate void FunctionalTreeEventHandler(FunctionalTree oldTree);
    public delegate void FunctionalEventTracingHandler(FunctionalEvent functionalEvent, FunctionalEventTracingArgs e);

}
