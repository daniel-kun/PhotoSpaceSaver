using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoSpaceSaver.Core.Logic
{
    public class OneDriveAuthorizationService
    {

        private static string[] _scopes = new string[]
        {
                        "Files.Read",
                        "Files.Read.All",
                        "User.Read"
        };

        public async Task<string> RefreshAuthorization(ClientApplicationBase oneDriveApp)
        {
            try
            {
                AuthenticationResult authResult = await oneDriveApp.AcquireTokenSilentAsync(_scopes, oneDriveApp.Users?.FirstOrDefault());
                return authResult.AccessToken;
            } catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> Authorize(ClientApplicationBase oneDriveApp)
        {
            return null;
        }
    }
}
