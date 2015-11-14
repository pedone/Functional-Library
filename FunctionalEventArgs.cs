using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    public class FunctionalEventArgs
    {

        #region Variables

        /// <summary>
        /// Gets or sets a value that indicates the present state of the event handling for a routed event as it travels the route. 
        /// </summary>
        public bool Handled { get; set; }
        /// <summary>
        /// Gets the FunctionalEvent.
        /// </summary>
        public FunctionalEvent FunctionalEvent { get; internal set; }
        /// <summary>
        /// Gets reference to the functional element that raised the event.
        /// </summary>
        public IFunctionalTreeElement Source { get; internal set; }

        #endregion

        public FunctionalEventArgs()
        { }

        public FunctionalEventArgs(FunctionalEvent functionalEvent)
        {
            if (functionalEvent == null)
                throw new ArgumentNullException("functionalEvent", "functionalEvent is null.");

            FunctionalEvent = functionalEvent;
        }

    }
}
