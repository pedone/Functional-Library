using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FunctionalTreeLibrary
{
    public static class FunctionalTreeElementExtensions
    {

        public static void RaiseFunctionalEvent(this IFunctionalTreeElement source, FunctionalEventArgs e)
        {
            RaiseFunctionalEvent(source, e, null);
        }

        public static void RaiseFunctionalEvent(this IFunctionalTreeElement source, FunctionalEventArgs e, Predicate<IFunctionalTreeElement> predicate)
        {
            if (e == null || source == null)
                return;

            e.Source = source;
            FunctionalEventManager.Instance.RaiseEvent(e, predicate);
        }

        public static void RaiseFunctionalEventPair(this IFunctionalTreeElement source, 
            FunctionalEvent tunnelEvent, 
            FunctionalEvent bubbleEvent, 
            FunctionalEventArgs e)
        {
            RaiseFunctionalEventPair(source, tunnelEvent, bubbleEvent, e, null);
        }

        public static void RaiseFunctionalEventPair(this IFunctionalTreeElement source, 
            FunctionalEvent tunnelEvent, 
            FunctionalEvent bubbleEvent, 
            FunctionalEventArgs e,
            Predicate<IFunctionalTreeElement> predicate)
        {
            if (source == null || tunnelEvent == null || bubbleEvent == null || e == null)
                return;

            e.Source = source;
            FunctionalEventManager.Instance.RaiseEventPair(tunnelEvent, bubbleEvent, e, predicate);
        }

        /// <summary>
        /// Adds a handler for the element that is raised when the element is attached to a functional tree.
        /// </summary>
        public static void AddAttachedToFunctionalTreeHandler(this IFunctionalTreeElement source, FunctionalTreeEventHandler handler)
        {
            if (source == null)
                return;
            if (handler == null)
                throw new ArgumentNullException("handler", "handler is null.");

            FunctionalTreeHelper.GetFunctionalTree(source).GetFunctionalElement(source).AddAttachedToTreeHandler(handler);
        }

        /// <summary>
        /// Removes a handler for the element that is raised when the element is attached to a functional tree.
        /// </summary>
        public static void RemoveAttachedToFunctionalTreeHandler(this IFunctionalTreeElement source, FunctionalTreeEventHandler handler)
        {
            if (source == null)
                return;
            if (handler == null)
                throw new ArgumentNullException("handler", "handler is null.");

            FunctionalTreeHelper.GetFunctionalTree(source).GetFunctionalElement(source).RemoveAttachedToTreeHandler(handler);
        }

        /// <summary>
        /// Adds a handler for the element that is raised when the element is detached from a functional tree.
        /// </summary>
        public static void AddDetachedFromFunctionalTreeHandler(this IFunctionalTreeElement source, FunctionalTreeEventHandler handler)
        {
            if (source == null)
                return;
            if (handler == null)
                throw new ArgumentNullException("handler", "handler is null.");

            FunctionalTreeHelper.GetFunctionalTree(source).GetFunctionalElement(source).AddDetachedFromTreeHandler(handler);
        }

        /// <summary>
        /// Removes a handler for the element that is raised when the element is detached from a functional tree.
        /// </summary>
        public static void RemoveDetachedFromFunctionalTreeHandler(this IFunctionalTreeElement source, FunctionalTreeEventHandler handler)
        {
            if (source == null)
                return;
            if (handler == null)
                throw new ArgumentNullException("handler", "handler is null.");

            FunctionalTreeHelper.GetFunctionalTree(source).GetFunctionalElement(source).RemoveDetachedFromTreeHandler(handler);
        }

        public static void AddFunctionalHandler(this IFunctionalTreeElement source, FunctionalEvent functionalEvent, Delegate handler)
        {
            if (source == null)
                return;

            AddFunctionalHandler(source, functionalEvent, handler, false);
        }

        public static void AddFunctionalHandler(this IFunctionalTreeElement source, FunctionalEvent functionalEvent, Delegate handler, bool handledEventsToo)
        {
            if (source == null)
                return;
            if (functionalEvent == null)
                throw new ArgumentNullException("functionalEvent", "functionalEvent is null.");

            FunctionalEventManager.Instance.AddHandler(functionalEvent, source, handler, handledEventsToo);
        }

        public static void RemoveFunctionalHandler(this IFunctionalTreeElement source, FunctionalEvent functionalEvent, Delegate handler)
        {
            if (source == null)
                return;
            if (functionalEvent == null)
                throw new ArgumentNullException("functionalEvent", "functionalEvent is null.");

            FunctionalEventManager.Instance.RemoveHandler(functionalEvent, source, handler);
        }

        public static void AddFunctionalChild(this IFunctionalTreeElement source, IFunctionalTreeElement element)
        {
            if (source == null)
                return;

            FunctionalTreeElement sourceElement = FunctionalTreeHelper.GetFunctionalTree(source).GetFunctionalElement(source);
            sourceElement.AddChild(element);
        }

        public static void DisconnectFromFunctionalTree(this IFunctionalTreeElement source)
        {
            if (source == null)
                return;

            FunctionalTreeHelper.GetFunctionalTree(source).GetFunctionalElement(source).DisconnectFromTree();
        }

        public static bool RemoveFunctionalChild(this IFunctionalTreeElement source, IFunctionalTreeElement child)
        {
            if (source == null)
                return false;

            FunctionalTreeElement sourceElement = FunctionalTreeHelper.GetFunctionalTree(source).GetFunctionalElement(source);
            return sourceElement.RemoveChild(child);
        }

        public static object GetFunctionalValue(this IFunctionalTreeElement source, FunctionalProperty fp)
        {
            if (source == null)
                return false;

            return FunctionalPropertyManager.Instance.GetValue(fp, source);
        }

        public static void SetFunctionalValue(this IFunctionalTreeElement source, FunctionalProperty fp, object value)
        {
            if (source == null)
                return;

            FunctionalPropertyManager.Instance.SetValue(fp, source, value);
        }

        public static void ClearFunctionalValue(this IFunctionalTreeElement source, FunctionalProperty fp)
        {
            if (source == null)
                return;

            FunctionalPropertyManager.Instance.ClearValue(fp, source);
        }

    }
}
