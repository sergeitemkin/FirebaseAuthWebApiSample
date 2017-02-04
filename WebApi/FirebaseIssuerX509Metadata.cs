using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace WebApi
{
    public class FirebaseIssuerX509Metadata
    {
        public string Kid { get; }
        public string RawCertificate { get; }

        public FirebaseIssuerX509Metadata(string kid, string rawCertificate)
        {
            Kid = kid;
            RawCertificate = rawCertificate;
        }

        public X509SecurityKey ToX509SecurityKey()
        {
            // The public key is surrounded by -----BEGIN CERTIFICATE----- & -----END CERTIFICATE-----
            // I kinda stole this from somewhere, forget where. Anyway, check out the format of the keys 
            // at https://firebase.google.com/docs/auth/admin/verify-id-tokens
            var lines = RawCertificate.Split('\n');
            var selectedLines = lines.Skip(1).Take(lines.Length - 3);
            var base64CertificateKey = string.Join(Environment.NewLine, selectedLines);

            return new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(base64CertificateKey)));
        }
    }
}