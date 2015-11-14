using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    public class FunctionalPropertyChangedEventArgs
    {

        #region Variables

        public object NewValue { get; private set; }
        public object OldValue { get; private set; }
        public FunctionalProperty Property { get; private set; }

        #endregion

        public FunctionalPropertyChangedEventArgs(FunctionalProperty property, Object oldValue, Object newValue)
        {
            if (property == null)
                throw new ArgumentNullException("property", "property is null.");

            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

    }
}
