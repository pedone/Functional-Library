using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{

    public class FunctionalEventManager
    {

        #region Variables

        internal static FunctionalEventManager Instance { get; private set; }

        private Dictionary<Type, List<FunctionalEvent>> FunctionalEventsInternal { get; set; }
        private Dictionary<FunctionalEvent, Dictionary<IFunctionalTreeElement, List<FunctionalEventHandlerContainer>>> HandlerListInternal { get; set; }
        
        #endregion

        static FunctionalEventManager()
        {
            Instance = new FunctionalEventManager();
        }

        private FunctionalEventManager()
        {
            FunctionalEventsInternal = new Dictionary<Type, List<FunctionalEvent>>();
            HandlerListInternal = new Dictionary<FunctionalEvent, Dictionary<IFunctionalTreeElement, List<FunctionalEventHandlerContainer>>>();
        }

        /// <summary>
        /// Registers a new functional event.
        /// </summary>
        /// <param name="name">The name of the functional event. The name must be unique within the owner type and cannot be null or an empty string.</param>
        /// <param name="functionalStretegy">The functional strategy of the event as a value of the enumeration.</param>
        /// <param name="handlerType">The type of the event handler. This must be a delegate type and cannot be null.</param>
        /// <param name="ownerType">The owner class type of the routed event. This cannot be null.</param>
        private FunctionalEvent RegisterEventInternal(string name, FunctionalStrategy functionalStrategy, Type handlerType, Type ownerType)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "name is null.");
            if (handlerType == null)
                throw new ArgumentNullException("handlerType", "handlerType is null.");
            if (ownerType == null)
                throw new ArgumentNullException("ownerType", "ownerType is null.");
            if (FunctionalEventsInternal.ContainsKey(ownerType))
            {
                if (FunctionalEventsInternal[ownerType].Any(cur => cur.Name == name))
                    throw new ArgumentException(String.Format("RoutedEvent Name '{0}' for OwnerType '{1}' already used.", name, ownerType));
            }
            
            FunctionalEvent functionalEvent = new FunctionalEvent(name, functionalStrategy, handlerType, ownerType);
            if (!FunctionalEventsInternal.ContainsKey(ownerType))
                FunctionalEventsInternal[ownerType] = new List<FunctionalEvent>();

            FunctionalEventsInternal[ownerType].Add(functionalEvent);
            return functionalEvent;
        }

        internal void AddHandler(FunctionalEvent functionalEvent, IFunctionalTreeElement element, Delegate handler, bool handledEventsToo = false)
        {
            if (functionalEvent == null)
                throw new ArgumentNullException("functionalEvent", "functionalEvent is null.");
            if (element == null)
                throw new ArgumentNullException("element", "element is null.");
            if (handler == null)
                throw new ArgumentNullException("handler", "handler is null.");
            if (handler.GetType() != functionalEvent.HandlerType && handler.GetType() != typeof(FunctionalEventHandler))
                throw new ArgumentException("handler", "Handler type mismatched.");

            if (!HandlerListInternal.ContainsKey(functionalEvent))
            {
                HandlerListInternal[functionalEvent] = new Dictionary<IFunctionalTreeElement, List<FunctionalEventHandlerContainer>>();
            }
            if (!HandlerListInternal[functionalEvent].ContainsKey(element))
            {
                HandlerListInternal[functionalEvent][element] = new List<FunctionalEventHandlerContainer>();
            }

            GetHandlerList(functionalEvent, element).Add(new FunctionalEventHandlerContainer(handler, handledEventsToo));
        }

        internal void RemoveHandler(FunctionalEvent functionalEvent, IFunctionalTreeElement element, Delegate handler)
        {
            if (functionalEvent == null)
                throw new ArgumentNullException("functionalEvent", "functionalEvent is null.");
            if (element == null)
                throw new ArgumentNullException("element", "element is null.");
            if (handler == null)
                throw new ArgumentNullException("handler", "handler is null.");
            
            List<FunctionalEventHandlerContainer> handlerList = GetHandlerList(functionalEvent, element);
            if (handlerList != null)
                handlerList.RemoveAll(cur => cur.Handler == handler);
        }

        internal List<FunctionalEventHandlerContainer> GetHandlerList(FunctionalEvent functionalEvent, IFunctionalTreeElement element)
        {
            if (HandlerListInternal.ContainsKey(functionalEvent) && HandlerListInternal[functionalEvent].ContainsKey(element))
                return HandlerListInternal[functionalEvent][element];

            return null;
        }

        internal void RaiseEvent(FunctionalEventArgs eventArgs, Predicate<IFunctionalTreeElement> predicate = null)
        {
            if (eventArgs == null)
                throw new ArgumentNullException("eventArgs", "eventArgs is null.");
            if (eventArgs.FunctionalEvent == null)
                throw new ArgumentNullException("eventArgs", "eventArgs is null.");

            if (!FunctionalEventsInternal.ContainsKey(eventArgs.FunctionalEvent.OwnerType))
                return;

            FunctionalEvent functionalEvent = eventArgs.FunctionalEvent;
            FunctionalElementRoute route = null;
            switch (functionalEvent.FunctionalStretegy)
            {
                case FunctionalStrategy.Bubble:
                    route = BuildBubbleEventRoute(eventArgs.Source, eventArgs);
                    break;
                case FunctionalStrategy.Tunnel:
                    route = BuildTunnelEventRoute(eventArgs.Source, eventArgs);
                    break;
                case FunctionalStrategy.Spread:
                    route = BuildSpreadEventRoute(eventArgs.Source, eventArgs);
                    break;
                case FunctionalStrategy.Children:
                    route = BuildChildrenEventRoute(eventArgs.Source, eventArgs);
                    break;
                case FunctionalStrategy.Descendents:
                    route = BuildDescendentsEventRoute(eventArgs.Source, eventArgs);
                    break;
                case FunctionalStrategy.Siblings:
                    route = BuildSiblingsEventRoute(eventArgs.Source, eventArgs);
                    break;
                case FunctionalStrategy.Parent:
                    route = BuildParentEventRoute(eventArgs.Source, eventArgs);
                    break;
            }

            FunctionalEventTracingArgs tracingEventArgs = new FunctionalEventTracingArgs(route);
            foreach (IFunctionalTreeElement element in route)
            {
                tracingEventArgs.UpdateCurrentIndex(route.IndexOf(element));
                RaiseEventOnElement(element, eventArgs, tracingEventArgs, predicate);
                if (tracingEventArgs.Abort)
                    break;
            }
        }

        /// <summary>
        /// The event will be raised only on objects of type T.
        /// </summary>
        internal void RaiseEventPair(FunctionalEvent tunnelEvent, FunctionalEvent bubbleEvent, FunctionalEventArgs eventArgs, Predicate<IFunctionalTreeElement> predicate = null)
        {
            if (tunnelEvent == null)
                throw new ArgumentNullException("tunnelEvent", "tunnelEvent is null.");
            if (bubbleEvent == null)
                throw new ArgumentNullException("bubbleEvent", "bubbleEvent is null.");
            if (eventArgs == null)
                throw new ArgumentNullException("eventArgs", "eventArgs is null.");

            if (tunnelEvent.FunctionalStretegy != FunctionalStrategy.Tunnel)
                throw new ArgumentNullException("tunnelEvent", "tunnelEvent is not tunnel.");
            if (bubbleEvent.FunctionalStretegy != FunctionalStrategy.Bubble)
                throw new ArgumentNullException("bubbleEvent", "bubbleEvent is bubble.");

            if (!FunctionalEventsInternal.ContainsKey(eventArgs.FunctionalEvent.OwnerType))
                return;

            eventArgs.FunctionalEvent = tunnelEvent;
            RaiseEvent(eventArgs, predicate);

            eventArgs.FunctionalEvent = bubbleEvent;
            RaiseEvent(eventArgs, predicate);
        }

        private FunctionalElementRoute BuildBubbleEventRoute(IFunctionalTreeElement element, FunctionalEventArgs eventArgs)
        {
            FunctionalElementRoute route = new FunctionalElementRoute();

            IFunctionalTreeElement curElement = element;
            while (curElement != null)
            {
                route.Add(curElement);
                curElement = FunctionalTreeHelper.GetFunctionalParent(curElement);
            }

            return route;
        }

        private FunctionalElementRoute BuildParentEventRoute(IFunctionalTreeElement element, FunctionalEventArgs eventArgs)
        {
            FunctionalElementRoute route = new FunctionalElementRoute();
            route.Add(FunctionalTreeHelper.GetFunctionalParent(element));

            return route;
        }
        private FunctionalElementRoute BuildTunnelEventRoute(IFunctionalTreeElement element, FunctionalEventArgs eventArgs)
        {
            FunctionalElementRoute route = BuildBubbleEventRoute(element, eventArgs);
            route.Reverse();

            return route;
        }

        private FunctionalElementRoute BuildSpreadEventRoute(IFunctionalTreeElement element, FunctionalEventArgs eventArgs)
        {
            FunctionalElementRoute route = new FunctionalElementRoute();

            IFunctionalTreeElement treeRoot = FunctionalTreeHelper.GetFunctionalTree(element).Root;
            route.Add(treeRoot);
            route.AddRange(GetDescendents(treeRoot));

            return route;
        }

        private FunctionalElementRoute BuildChildrenEventRoute(IFunctionalTreeElement element, FunctionalEventArgs eventArgs)
        {
            FunctionalElementRoute route = new FunctionalElementRoute();
            route.AddRange(FunctionalTreeHelper.GetFunctionalChildren(element));
            return route;
        }

        private FunctionalElementRoute BuildDescendentsEventRoute(IFunctionalTreeElement element, FunctionalEventArgs eventArgs)
        {
            FunctionalElementRoute route = new FunctionalElementRoute();
            route.AddRange(GetDescendents(element));

            return route;
        }

        private FunctionalElementRoute BuildSiblingsEventRoute(IFunctionalTreeElement element, FunctionalEventArgs eventArgs)
        {
            FunctionalElementRoute route = new FunctionalElementRoute();

            IFunctionalTreeElement parent = FunctionalTreeHelper.GetFunctionalParent(element);
            if (parent == null)
                return route;

            route.AddRange(FunctionalTreeHelper.GetFunctionalChildren(parent).Where(cur => cur != element));
            return route;
        }

        private IEnumerable<IFunctionalTreeElement> GetDescendents(IFunctionalTreeElement element)
        {
            List<IFunctionalTreeElement> descendents = new List<IFunctionalTreeElement>();
            foreach (IFunctionalTreeElement item in FunctionalTreeHelper.GetFunctionalChildren(element))
            {
                descendents.Add(item);
                descendents.AddRange(GetDescendents(item));
            }

            return descendents;
        }

        private void RaiseEventOnElement(IFunctionalTreeElement element, 
            FunctionalEventArgs eventArgs, 
            FunctionalEventTracingArgs tracingEventArgs, 
            Predicate<IFunctionalTreeElement> predicate = null)
        {
            if (element == null || eventArgs == null || (predicate != null && !predicate(element)))
                return;

            List<FunctionalEventHandlerContainer> eventHandlerList = GetHandlerList(eventArgs.FunctionalEvent, element);
            if (eventHandlerList != null && eventHandlerList.Count() > 0)
            {
                foreach (FunctionalEventHandlerContainer handlerContainer in eventHandlerList)
                {
                    if (!eventArgs.Handled || handlerContainer.HandledEventsToo)
                    {
                        eventArgs.FunctionalEvent.EventTracer.RaiseEventRaising(eventArgs.FunctionalEvent, tracingEventArgs);
                        if (tracingEventArgs.Abort)
                            return;

                        foreach (Delegate handler in handlerContainer.Handler.GetInvocationList())
                            handler.Method.Invoke(element, new object[] { element, eventArgs });

                        eventArgs.FunctionalEvent.EventTracer.RaiseEventRaised(eventArgs.FunctionalEvent, tracingEventArgs);
                        if (tracingEventArgs.Abort)
                            return;
                    }
                }
            }
        }

        public static FunctionalEvent RegisterEvent(string name, FunctionalStrategy functionalStrategy, Type handlerType, Type ownerType)
        {
            return Instance.RegisterEventInternal(name, functionalStrategy, handlerType, ownerType);
        }

    }
}