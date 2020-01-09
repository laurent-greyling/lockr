using System.Threading.Tasks;
using DnsClient;
using System.Net.Mail;
using System;
using LockrApi.Entities;
using System.Text;
using LockrApi.Database;
using System.Globalization;

namespace LockrApi.Services
{
    public class VerifyDomain : IVerifyDomain
    {
        public IDomainQueries _domainQueries;

        public VerifyDomain(IDomainQueries domainQueries) 
        {
            _domainQueries = domainQueries;
        }

        public async Task<DomainEntity> Verify(string domain)
        {
            var domainToVerify = string.Empty;
            var domainEntity = new DomainEntity();

            if (IsValidEmail(domain))
            {
                var mailAddress = new MailAddress(domain);
                domainToVerify = mailAddress.Host;
            }

            if (IsValidDomainName(domain))
            {
                domainToVerify = domain;
            }
            
            //Fill domain entity with domain details if domain was verified as domain
            if (!string.IsNullOrEmpty(domainToVerify))
            {
                var client = new LookupClient
                {
                    UseCache = true
                };

                var result = await client.QueryAsync(domainToVerify, QueryType.TXT);
                var resultAnswer = result.Answers[0].ToString();
                var spfRecord = resultAnswer.Substring(resultAnswer.IndexOf("\""))
                    .Replace("\"", "")
                    .Replace("\\", "")
                    .Split(";");

                domainEntity.Id = Guid.NewGuid().ToString();
                domainEntity.Address = domainToVerify;

                var mxDomains = new StringBuilder();

                //Add spf record details to domain entity, if a domain does not have this specified spf, then all these are null and domain is invalid
                foreach (var item in spfRecord)
                {
                    switch (item)
                    {
                        case string a when a.Contains("v=") :
                            domainEntity.SpfVersion = item.Replace("v=","");
                            break;
                        case string a when a.Contains("version"):
                            domainEntity.Version = item.Replace("version:", "");
                            break;
                        case string a when a.Contains("expire"):
                            domainEntity.ExpiryData = item.Replace("expire:", "");
                            break;
                        case string a when a.Contains("provider"):
                            domainEntity.Provider = item.Replace("provider:", "");
                            break;
                        case string a when a.Contains("ntamx"):
                            mxDomains.AppendLine(item.Replace("ntamx:", ""));
                            break;
                        default:
                            break;
                    }
                }

                domainEntity.NtaMxList = mxDomains.ToString();
                                
                var isValid = IsValidSpf(domainEntity);
                domainEntity.IsValid = isValid ? "Valid" : "InValid";
            }

            return domainEntity;
        }

        /// <summary>
        /// Determine if domain spf is still valid based on expiry date and if the spfverion is NTA7516
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool IsValidSpf(DomainEntity entity)
        {
            try
            {
                bool isValid;

                var expiry = DateTime.ParseExact(entity.ExpiryData, "yyyy-MM", CultureInfo.InvariantCulture);
                var currentDate = DateTime.UtcNow;
                var result = DateTime.Compare(expiry, currentDate);

                //If date and spf version is valid then record is valid, else invalid
                if ((result > 0 || result == 0) && entity.SpfVersion == "NTA7516")
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

                return isValid;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate if string is email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool IsValidEmail(string email)
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

        /// <summary>
        /// validate if it is domain name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsValidDomainName(string name)
        {
            return Uri.CheckHostName(name) != UriHostNameType.Unknown;
        }
    }
}
