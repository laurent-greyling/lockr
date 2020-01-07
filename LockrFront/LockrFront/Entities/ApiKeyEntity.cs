using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockrFront.Entities
{
    [Table("ApiKeys")]
    public class ApiKeyEntity
    {
        /// <summary>
        ///ID to find the corresponding APIKey for a domain 
        /// </summary>
        [Key]
        public string Id { get; set; }        
        /// <summary>
        /// ApiKey for a domain
        /// </summary>
        public string ApiKey { get; set; }
    }
}
