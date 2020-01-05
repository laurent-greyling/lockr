using IdentityServerLockr.Models.Consent;

namespace IdentityServerLockr.Models.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}