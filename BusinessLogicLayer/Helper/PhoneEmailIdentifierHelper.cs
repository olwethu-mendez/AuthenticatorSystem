using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Helper
{
    public class ContactType
    {
        public const string Phone = "Phone";
        public const string Email = "Email";
        public const string Invalid = "Invalid";
    }
    public class IsPhoneEmailRes
    {
        public string? contactType { get; set; }
        public string? contactValue { get; set; }
    }
    public class PhoneEmailIdentifierHelper
    {
        public IsPhoneEmailRes TypeIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value)) return new IsPhoneEmailRes { contactType = ContactType.Invalid, contactValue = null };
            string phoneNumberRegex = @"^\d{9}$";
            //string countryCodedPhoneNumberRegex = @"^\+\d{7,15}$";
            string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (Regex.IsMatch(value, emailRegex)) return new IsPhoneEmailRes
            {
                contactType = ContactType.Email,
                contactValue = value
            };
            if (Regex.IsMatch(value, phoneNumberRegex)) return new IsPhoneEmailRes
            {
                contactType = ContactType.Phone,
                contactValue = value
            };
            return new IsPhoneEmailRes { contactType = ContactType.Invalid, contactValue = null };
        }
        //public IsPhoneEmailRes TypeIdentifier(string value)
        //{
        //    string phoneNumberRegex = @"^\d{9}$";
        //    string countryCodeRegex = @"^\+\d{1,3}$";
        //    string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        //    if (Regex.IsMatch(value, emailRegex)) return new IsPhoneEmailRes
        //    {
        //        contactType = ContactType.Email,
        //        contactValue = value
        //    };
        //    else
        //    {
        //        value.Length == 9
        //    }
        //}
    }
}
