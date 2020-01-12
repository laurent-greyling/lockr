namespace LockrApi.Utilities
{
    public interface IDomainValidation
    {
        /// <summary>
        /// Validate if string is email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool IsValidEmail(string email);

        /// <summary>
        /// validate if it is domain name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsValidDomainName(string name);

        /// <summary>
        /// Return the domain name from Email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        string DomainFromEmail(string email);
    }
}
