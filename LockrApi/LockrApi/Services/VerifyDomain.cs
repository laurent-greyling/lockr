using System.Threading.Tasks;
using DnsClient;
using System;
using LockrApi.Entities;
using System.Text;
using LockrApi.Database;
using System.Globalization;
using LockrApi.Utilities;

namespace LockrApi.Services
{
    public class VerifyDomain : IVerifyDomain
    {
        public IDomainQueries _domainQueries;
        public IDomainValidation _domainValidation;

        public VerifyDomain(
            IDomainQueries domainQueries,
            IDomainValidation domainValidation) 
        {
            _domainQueries = domainQueries;
            _domainValidation = domainValidation;
        }

        public async Task<DomainEntity> Verify(string domain)
        {
            var domainToVerify = string.Empty;
            var domainEntity = new DomainEntity();

            if (_domainValidation.IsValidEmail(domain))
            {
                domainToVerify = _domainValidation.DomainFromEmail(domain);
            }

            if (_domainValidation.IsValidDomainName(domain))
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

                domainEntity.Id = domainToVerify;

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
    }
}
