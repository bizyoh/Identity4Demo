using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static async Task Main()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:6001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }


            var tokenResponse2 = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client2",
                ClientSecret = "secret2",

                Scope = "api2"
            });

            if (tokenResponse2.IsError)
            {
                Console.WriteLine(tokenResponse2.Error);
                return;
            }

            Console.WriteLine(tokenResponse2.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient2 = new HttpClient();
            apiClient2.SetBearerToken(tokenResponse2.AccessToken);

            var response2 = await apiClient2.GetAsync("https://localhost:6001/identity");
            if (!response2.IsSuccessStatusCode)
            {
                Console.WriteLine(response2.StatusCode);
            }
            else
            {
                var content2 = await response2.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content2));
            }
        }
    }
}