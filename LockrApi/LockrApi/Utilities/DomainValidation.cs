using System;
using System.Net.Mail;

namespace LockrApi.Utilities
{
    public class DomainValidation : IDomainValidation
    {
       
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        public bool IsValidDomainName(string name)
        {
            return Uri.CheckHostName(name) != UriHostNameType.Unknown;
        }

        public string DomainFromEmail(string email)
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Host;
        }
    }
}
