using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTreeLibrary
{
    internal class FunctionalPropertyManager
    {

        #region Variables

        internal static FunctionalPropertyManager Instance { get; private set; }

        private Dictionary<Type, List<FunctionalProperty>> FunctionalPropertiesInternal { get; set; }
        private Dictionary<FunctionalProperty, Dictionary<IFunctionalTreeElement, object>> FunctionalPropertyValuesInternal { get; set; }

        #endregion

        static FunctionalPropertyManager()
        {
            Instance = new FunctionalPropertyManager();
        }

        private FunctionalPropertyManager()
        {
            FunctionalPropertiesInternal = new Dictionary<Type, List<FunctionalProperty>>();
            FunctionalPropertyValuesInternal = new Dictionary<FunctionalProperty, Dictionary<IFunctionalTreeElement, object>>();

            FunctionalTreeManager.Instance.FunctionalElementAttachedToTree += FunctionalElementAttachedToTree;
            FunctionalTreeManager.Instance.FunctionalElementDetachedFromTree += FunctionalElementDetachedFromTree;
        }

        private void FunctionalElementDetachedFromTree(FunctionalTreeElement element, FunctionalTreeElement parent, FunctionalTree functionalTree)
        {
            foreach (FunctionalProperty property in FunctionalPropertyValuesInternal.Keys)
            {
                if (property.DefaultMetadata == null || !property.DefaultMetadata.Inherits || property.DefaultMetadata.PropertyChangedCallback == null)
                    continue;

                if (HasValue(property, element.Element))
                    continue;

                FunctionalTreeElement curParent = parent;
                while (curParent != null)
                {
                    if (HasValue(property, curParent.Element))
                    {
                        object oldValue = GetValue(property, curParent.Element);
                        object newValue = property.DefaultMetadata.DefaultValue;

                        property.DefaultMetadata.PropertyChangedCallback(element.Element,
                            new FunctionalPropertyChangedEventArgs(property, oldValue, newValue));

                        if (property.DefaultMetadata.Inherits)
                            NotifyChildrenAboutInheritedValue(FunctionalTreeHelper.GetFunctionalChildren(element.Element), property, oldValue, newValue);

                        break;
                    }

                    curParent = curParent.Parent;
                }
            }
        }

        private void FunctionalElementAttachedToTree(FunctionalTreeElement element, FunctionalTreeElement parent, FunctionalTree functionalTree)
        {
            foreach (FunctionalProperty property in FunctionalPropertyValuesInternal.Keys)
            {
                if (property.DefaultMetadata == null || !property.DefaultMetadata.Inherits || property.DefaultMetadata.PropertyChangedCallback == null)
                    continue;

                if (HasValue(property, element.Element))
                    continue;

                FunctionalTreeElement curParent = parent;
                while (curParent != null)
                {
                    if (HasValue(property, curParent.Element))
                    {
                        object oldValue = property.DefaultMetadata.DefaultValue;
                        object newValue = GetValue(property, curParent.Element);

                        property.DefaultMetadata.PropertyChangedCallback(element.Element,
                            new FunctionalPropertyChangedEventArgs(property, oldValue, newValue));

                        if (property.DefaultMetadata.Inherits)
                            NotifyChildrenAboutInheritedValue(FunctionalTreeHelper.GetFunctionalChildren(element.Element), property, oldValue, newValue);

                        break;
                    }

                    curParent = curParent.Parent;
                }
            }
        }

        internal FunctionalProperty Register(string name, Type propertyType, Type ownerType, FunctionalPropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "name is null.");
            if (propertyType == null)
                throw new ArgumentNullException("propertyType", "propertyType is null.");
            if (ownerType == null)
                throw new ArgumentNullException("ownerType", "ownerType is null.");
            if (FunctionalPropertiesInternal.ContainsKey(ownerType))
            {
                if (FunctionalPropertiesInternal[ownerType].Any(cur => cur.Name == name))
                    throw new ArgumentException(String.Format("FunctionalProperty Name '{0}' for OwnerType '{1}' already used.", name, ownerType));
            }
            if (typeMetadata != null)
            {
                if (typeMetadata.DefaultValue != null && propertyType.IsAssignableFrom(typeMetadata.DefaultValue.GetType()) == false)
                    throw new ArgumentException(String.Format("Default value type does not match type of property '{0}'.", name));
                
                if (typeMetadata.DefaultValue == null && propertyType.IsValueType)
                    typeMetadata.DefaultValue = Activator.CreateInstance(propertyType);
                
                if (validateValueCallback != null && validateValueCallback(typeMetadata.DefaultValue) == false)
                    throw new ArgumentException(String.Format("Default value for '{0}' property is not valid because ValidateValueCallback failed.", name));
            }

            FunctionalPropertyMetadata actualMetadata = typeMetadata ?? new FunctionalPropertyMetadata(propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null);
            FunctionalProperty functionalProperty = new FunctionalProperty(name, propertyType, ownerType, actualMetadata, validateValueCallback);
            if (!FunctionalPropertiesInternal.ContainsKey(ownerType))
                FunctionalPropertiesInternal[ownerType] = new List<FunctionalProperty>();

            FunctionalPropertiesInternal[ownerType].Add(functionalProperty);
            return functionalProperty;
        }

        internal object GetValue(FunctionalProperty functionalProperty, IFunctionalTreeElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element is null.");
            if (functionalProperty == null)
                throw new ArgumentNullException("functionalProperty", "functionalProperty is null.");
            if (FunctionalPropertyValuesInternal.ContainsKey(functionalProperty) == false)
            {
                return functionalProperty.DefaultMetadata.DefaultValue;
            }

            if(!HasValue(functionalProperty, element))
            {
                //Get inherited Value
                if (functionalProperty.DefaultMetadata.Inherits)
                {
                    IFunctionalTreeElement curParent = FunctionalTreeHelper.GetFunctionalParent(element);
                    while (curParent != null)
                    {
                        if (HasValue(functionalProperty, curParent))
                            return FunctionalPropertyValuesInternal[functionalProperty][curParent];

                        curParent = FunctionalTreeHelper.GetFunctionalParent(curParent);
                    }
                }

                //Return default value
                return functionalProperty.DefaultMetadata.DefaultValue;
            }

            return FunctionalPropertyValuesInternal[functionalProperty][element];
        }

        public void ClearValue(FunctionalProperty functionalProperty, IFunctionalTreeElement element)
        {
            if (functionalProperty == null)
                throw new ArgumentNullException("functionalProperty", "functionalProperty is null.");
            if (element == null)
                throw new ArgumentNullException("element", "element is null.");

            if (FunctionalPropertyValuesInternal.ContainsKey(functionalProperty) && HasValue(functionalProperty, element))
            {
                object oldValue = FunctionalPropertyValuesInternal[functionalProperty][element];
                FunctionalPropertyValuesInternal[functionalProperty].Remove(element);

                //Fire PropertyChanged
                if (functionalProperty.DefaultMetadata.PropertyChangedCallback != null)
                {
                    functionalProperty.DefaultMetadata.PropertyChangedCallback(element,
                        new FunctionalPropertyChangedEventArgs(functionalProperty, oldValue, functionalProperty.DefaultMetadata.DefaultValue));

                    if (functionalProperty.DefaultMetadata.Inherits)
                        NotifyChildrenAboutInheritedValue(FunctionalTreeHelper.GetFunctionalChildren(element), functionalProperty, oldValue, functionalProperty.DefaultMetadata.DefaultValue);
                }
            }
        }

        private bool HasValue(FunctionalProperty functionalProperty, IFunctionalTreeElement element)
        {
            return FunctionalPropertyValuesInternal.ContainsKey(functionalProperty) &&
                FunctionalPropertyValuesInternal[functionalProperty].ContainsKey(element);
        }

        internal void SetValue(FunctionalProperty functionalProperty, IFunctionalTreeElement element, object value)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element is null.");
            if (functionalProperty == null)
                throw new ArgumentNullException("functionalProperty", "functionalProperty is null.");
            if (value != null && functionalProperty.PropertyType.IsAssignableFrom(value.GetType()) == false)
            {
                throw new ArgumentException(String.Format("'{0}' is not a valid value for property '{1}'.", value, functionalProperty.Name));
            }

            //Validate value
            if (functionalProperty.ValidateValueCallback != null &&
                functionalProperty.ValidateValueCallback(value) == false)
            {
                throw new ArgumentException(String.Format("'{0}' is not a valid value for property '{1}'.", value, functionalProperty.Name));
            }

            //Coerce value
            object coercedValue = value;
            if (functionalProperty.DefaultMetadata.CoerceValueCallback != null)
            {
                coercedValue = functionalProperty.DefaultMetadata.CoerceValueCallback(element, value);
                if (coercedValue != value &&
                    functionalProperty.ValidateValueCallback != null &&
                    functionalProperty.ValidateValueCallback(coercedValue) == false)
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid value for property '{1}'.", coercedValue, functionalProperty.Name));
                }
            }

            //Init dictionary for functional property
            if (!FunctionalPropertyValuesInternal.ContainsKey(functionalProperty))
                FunctionalPropertyValuesInternal[functionalProperty] = new Dictionary<IFunctionalTreeElement, object>();

            //Save old value
            object oldValue = null;
            if (HasValue(functionalProperty, element))
            {
                oldValue = FunctionalPropertyValuesInternal[functionalProperty][element];
            }
            else
            {
                oldValue = functionalProperty.DefaultMetadata.DefaultValue;
            }

            //Set new value
            FunctionalPropertyValuesInternal[functionalProperty][element] = coercedValue;

            //Fire PropertyChanged
            if (functionalProperty.DefaultMetadata.PropertyChangedCallback != null)
            {
                functionalProperty.DefaultMetadata.PropertyChangedCallback(element, new FunctionalPropertyChangedEventArgs(functionalProperty, oldValue, coercedValue));
                if (functionalProperty.DefaultMetadata.Inherits)
                    NotifyChildrenAboutInheritedValue(FunctionalTreeHelper.GetFunctionalChildren(element), functionalProperty, oldValue, coercedValue);
            }
        }

        private void NotifyChildrenAboutInheritedValue(IEnumerable<IFunctionalTreeElement> children, FunctionalProperty functionalProperty, object oldValue, object newValue)
        {
            if (children == null || children.Count() == 0)
                return;

            foreach (var child in children)
            {
                if (!HasValue(functionalProperty, child))
                {
                    functionalProperty.DefaultMetadata.PropertyChangedCallback(child, new FunctionalPropertyChangedEventArgs(functionalProperty, oldValue, newValue));
                    NotifyChildrenAboutInheritedValue(FunctionalTreeHelper.GetFunctionalChildren(child), functionalProperty, oldValue, newValue);
                }
            }
        }

    }
}
