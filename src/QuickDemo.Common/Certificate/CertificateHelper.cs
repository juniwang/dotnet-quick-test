using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Common.Certificate
{
    public class CertificateHelper
    {
        public static X509Certificate2 FindCertificateByThumbprint(string thumbprint)
        {
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            x509Store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);
            if (collection.Count == 0)
                throw new Exception("Unable to find certificate or you are not authorized to access the certificate.");
            return collection[0];
        }
    }
}
