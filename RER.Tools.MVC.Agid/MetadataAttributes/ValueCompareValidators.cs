using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RER.Tools.MVC.Agid.MetadataAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class GreatherOrEqualToAttribute : RequiredAttribute
    {
        private String PropertyToCheckName { get; set; }
        private Object PropertyToCheckDesiredValue { get; set; }

        public GreatherOrEqualToAttribute(String propertyName, Object desiredvalue)
        {
            PropertyToCheckName = propertyName;
            PropertyToCheckDesiredValue = desiredvalue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            ValidationResult result = ValidationResult.Success;
            Object instance = context.ObjectInstance;
            Type type = instance.GetType();
            Object proprtyvalue = type.GetProperty(PropertyToCheckName).GetValue(instance, null);
            if (proprtyvalue?.ToString() == PropertyToCheckDesiredValue?.ToString())
            {
                result = base.IsValid(value, context);
            }

            return result;
        }
    }   
}
