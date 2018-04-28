using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using QuickDemo.Common.Certificate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Azure
{
    public class KeyVaultTester
    {
        private string _vaultBaseUrl;
        private ClientAssertionCertificate _assertionCert;

        public KeyVaultTester(string clientId, string certThumbprint, string vaultBaseUrl)
        {
            _vaultBaseUrl = vaultBaseUrl;
            // make sure the cert is authorized for you.
            // For local: certmgr -> right click and manage private keys -> add Everyone
            var clientAssertionCertPfx = CertificateHelper.FindCertificateByThumbprint(certThumbprint);
            _assertionCert = new ClientAssertionCertificate(clientId, clientAssertionCertPfx);
        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority, TokenCache.DefaultShared);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, _assertionCert);

            if (result == null)
                throw new InvalidOperationException("Fail to obtain keyvault access token");

            return result.AccessToken;
        }

        public async Task<string> GetSecretAsync(string name)
        {
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            var sec = await kv.GetSecretAsync(_vaultBaseUrl, name);
            var cert = await kv.GetCertificateAsync(_vaultBaseUrl, name);
            return sec?.Value;
        }

        public async Task SetSecretAsync(string name, string secret)
        {
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            await kv.SetSecretAsync(_vaultBaseUrl, name, secret);
        }

        public async Task<byte[]> GetCertificatesAsync(string name)
        {
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            var sec = await kv.GetCertificateAsync(_vaultBaseUrl, name);
            return sec.Cer;
        }
    }
}
