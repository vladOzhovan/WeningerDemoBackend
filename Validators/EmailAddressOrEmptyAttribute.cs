using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Validators
{
    public class EmailAddressOrEmptyAttribute : ValidationAttribute
    {
        public EmailAddressOrEmptyAttribute()
        {
            ErrorMessage = "Invalid email address.";
        }

        public override bool IsValid(object? value)
        {
            var str = value as string;

            if (string.IsNullOrWhiteSpace(str))
                return true;

            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(str);
        }
    }
}
