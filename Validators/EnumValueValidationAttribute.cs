using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Validators
{
    public class EnumValueValidationAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public EnumValueValidationAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || !Enum.IsDefined(_enumType, value))
                return new ValidationResult($"Invalid value for {_enumType.Name}");

            return ValidationResult.Success;
        }
    }
}
