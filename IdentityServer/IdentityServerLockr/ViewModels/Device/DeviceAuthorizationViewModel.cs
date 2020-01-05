using IdentityServerLockr.Models.Consent;
using IdentityServerLockr.ViewModels.Consent;

namespace IdentityServerLockr.ViewModels.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}