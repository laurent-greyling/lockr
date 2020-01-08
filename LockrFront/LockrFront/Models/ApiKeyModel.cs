namespace LockrFront.Models
{
    public class ApiKeyModel
    {
        /// <summary>
        ///ID to find the corresponding APIKey for a domain 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// ApiKey for a domain
        /// </summary>
        public string ApiKey { get; set; }
    }
}
