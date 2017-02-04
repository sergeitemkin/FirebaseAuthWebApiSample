using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace WebApi
{
    public class FirebaseIssuerKeyProvider
    {
        public async Task<IEnumerable<X509SecurityKey>> GetSigningKeys()
        {
            // Per https://firebase.google.com/docs/auth/admin/verify-id-tokens, the keys used can be 
            // any of the ones hosted at the below URL.
            var client = new HttpClient();
            // This url returns Json objects containing public keys for X509Certificates used to sign 
            // the id tokens we need to verify
            var jsonResult = await client.GetStringAsync("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com");

            // Convert the Json result into something we can use
            var issuerSigningKeys = JObject
                .Parse(jsonResult)
                .Children()
                .Cast<JProperty>()
                // The actual public key values require some extra parsing (check out X509Metadata below)
                .Select(i => new FirebaseIssuerX509Metadata(i.Path, i.Value.ToString()))
                .Select(m => m.ToX509SecurityKey());

            return issuerSigningKeys;
        }
    }
}