using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    public class FunctionalEventTracingArgs
    {

        #region Variables

        public bool Abort { get; set; }

        internal FunctionalElementRoute Route { get; private set; }
        public int CurrentRouteIndex { get; private set; }

        #endregion

        #region Properties

        public IFunctionalTreeElement CurrentElement
        {
            get { return Route.ElementAtOrDefault(CurrentRouteIndex); }
        }
        public IFunctionalTreeElement PreviousElement
        {
            get { return Route.ElementAtOrDefault(CurrentRouteIndex - 1); }
        }
        public IFunctionalTreeElement NextElement
        {
            get { return Route.ElementAtOrDefault(CurrentRouteIndex + 1); }
        }

        public IEnumerable<IFunctionalTreeElement> RemainingElements
        {
            get
            {
                int curIndex = CurrentRouteIndex;
                while (true)
                {
                    curIndex++;
                    if (curIndex >= Route.Count())
                        break;

                    yield return Route.ElementAt(curIndex);
                }
            }
        }

        public int EventRouteLength
        {
            get { return Route.Count(); }
        }

        #endregion
        
        internal FunctionalEventTracingArgs(FunctionalElementRoute eventRoute)
        {
            if (eventRoute == null)
                throw new ArgumentNullException("eventRoute", "eventRoute is null.");

            Route = eventRoute;
        }

        public void UpdateCurrentIndex(int index)
        {
            CurrentRouteIndex = index;
        }

    }
}
