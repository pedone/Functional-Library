using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FunctionalTreeLibrary
{
    public class FunctionalPropertyMetadata
    {

        #region Variables

        public object DefaultValue { get; internal set; }
        public CoerceValueCallback CoerceValueCallback { get; private set; }
        public PropertyChangedCallback PropertyChangedCallback { get; private set; }

        private FunctionalPropertyMetadataOptions _flags = FunctionalPropertyMetadataOptions.None;

        #endregion

        #region Properties

        public bool Inherits
        {
            get { return (_flags & FunctionalPropertyMetadataOptions.Inherits) == FunctionalPropertyMetadataOptions.Inherits; }
            set
            {
                if (value)
                {
                    if ((_flags & FunctionalPropertyMetadataOptions.Inherits) != FunctionalPropertyMetadataOptions.Inherits)
                        _flags |= FunctionalPropertyMetadataOptions.Inherits;
                }
                else
                {
                    if ((_flags & FunctionalPropertyMetadataOptions.Inherits) == FunctionalPropertyMetadataOptions.Inherits)
                        _flags |= FunctionalPropertyMetadataOptions.Inherits;
                }
            }
        }

        #endregion

        public FunctionalPropertyMetadata(object defaultValue)
            : this(defaultValue, FunctionalPropertyMetadataOptions.None, null, null)
        { }

        public FunctionalPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : this(null, FunctionalPropertyMetadataOptions.None, propertyChangedCallback, null)
        { }

        public FunctionalPropertyMetadata(object defaultValue, FunctionalPropertyMetadataOptions flags)
            : this(defaultValue, flags, null, null)
        { }

        public FunctionalPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
            : this(defaultValue, FunctionalPropertyMetadataOptions.None, propertyChangedCallback, null)
        { }

        public FunctionalPropertyMetadata(PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : this(null, FunctionalPropertyMetadataOptions.None, propertyChangedCallback, coerceValueCallback)
        { }

        public FunctionalPropertyMetadata(object defaultValue, FunctionalPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback)
            : this(defaultValue, flags, propertyChangedCallback, null)
        { }

        public FunctionalPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : this(defaultValue, FunctionalPropertyMetadataOptions.None, propertyChangedCallback, coerceValueCallback)
        { }

        public FunctionalPropertyMetadata(object defaultValue, FunctionalPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
        {
            DefaultValue = defaultValue;
            PropertyChangedCallback = propertyChangedCallback;
            CoerceValueCallback = coerceValueCallback;
            _flags = flags;
        }

    }
}
