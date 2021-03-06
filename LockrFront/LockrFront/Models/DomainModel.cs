﻿namespace LockrFront.Models
{
    public class DomainModel
    {
        /// <summary>
        /// Domian Identifier (Domain name/address)
        /// Domain names are unique
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// document version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Date till when spf is valid
        /// </summary>
        public string ExpiryData { get; set; }
        /// <summary>
        /// Provider of spf
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// Mx List
        /// </summary>
        public string NtaMxList { get; set; }

        /// <summary>
        /// spfversion, this should be NTA7516
        /// </summary>
        public string SpfVersion { get; set; }

        /// <summary>
        /// Is the current domain valid
        /// </summary>
        public string IsValid { get; set; }
    }
}
