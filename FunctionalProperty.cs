using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    public class FunctionalProperty
    {

        #region Variables

        public string Name { get; private set; }
        public Type OwnerType { get; private set; }
        public Type PropertyType { get; private set; }
        public ValidateValueCallback ValidateValueCallback { get; private set; }

        public FunctionalPropertyMetadata DefaultMetadata { get; private set; }

        #endregion

        internal FunctionalProperty(string name, Type propertyType, Type ownerType, FunctionalPropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)
        {
            Name = name;
            PropertyType = propertyType;
            OwnerType = ownerType;
            DefaultMetadata = typeMetadata;
            ValidateValueCallback = validateValueCallback;
        }

        public static FunctionalProperty Register(string name, Type propertyType, Type ownerType, FunctionalPropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)
        {
            return FunctionalPropertyManager.Instance.Register(name, propertyType, ownerType, typeMetadata, validateValueCallback);
        }

        public static FunctionalProperty Register(string name, Type propertyType, Type ownerType, FunctionalPropertyMetadata typeMetadata)
        {
            return Register(name, propertyType, ownerType, typeMetadata, null);
        }

        public static FunctionalProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, null, null);
        }

    }
}
