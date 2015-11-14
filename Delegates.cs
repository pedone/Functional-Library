using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    
    internal delegate void FunctionalChildChangedInternalEventHandler(FunctionalTreeElement child, FunctionalTreeElement parent, FunctionalTree functionalTree);
    public delegate void FunctionalChildChangedEventHandler(IFunctionalTreeElement child, IFunctionalTreeElement parent, FunctionalTree functionalTree);

    /// <summary>
    /// Represents the method that will handle various functional events.
    /// </summary>
    /// <param name="sender">The element where the event handler is defined.</param>
    /// <param name="e">The event data.</param>
    public delegate void FunctionalEventHandler(IFunctionalTreeElement sender, FunctionalEventArgs e);

    public delegate object CoerceValueCallback(IFunctionalTreeElement element, object baseValue);
    
    public delegate void PropertyChangedCallback(IFunctionalTreeElement element, FunctionalPropertyChangedEventArgs e);

    public delegate bool ValidateValueCallback(object value);

}
