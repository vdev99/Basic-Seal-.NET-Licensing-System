using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using BasicSealClient.Dtos;

namespace BasicSealClient
{
    internal class Network
    {
        private static HttpClient httpClient = new HttpClient();
        private const string serverBaseAddress = "http://localhost:7556";

        public bool CheckConnectivity()
        {
            string address = "www.google.com"; //rest server address here (except when running on localhost)

            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(address, 2000);
                if(reply.Status == IPStatus.Success) { return true; }
                else { return false; }
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<GetLicenseResponseDto> GetLicenseAsync(ClientLicenseVerifyDto clientLicenseVerifyDto)
        {
            try
            {
                string address = $"{serverBaseAddress}/api/client/licenses/activate";
                var postContent = new StringContent(JsonSerializer.Serialize(clientLicenseVerifyDto), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(address, postContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseObj = JsonSerializer.Deserialize<GetLicenseResponseDto>(await response.Content.ReadAsStringAsync());

                    return responseObj;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<VerifyResponseDto> VerifyLicenseAsync(LicenseVerifyDataDto clientLicenseDto)
        {
            try
            {
                string address = $"{serverBaseAddress}/api/client/licenses/verify";
                var postContent = new StringContent(JsonSerializer.Serialize(clientLicenseDto), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(address, postContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseObj = JsonSerializer.Deserialize<VerifyResponseDto>(await response.Content.ReadAsStringAsync());

                    return responseObj;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DateTime?> GetServerTimeAsync()
        {
            try
            {
                string address = $"{serverBaseAddress}/api/client/serverTime";

                var response = await httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseObj = JsonSerializer.Deserialize<TimeResponseDto>(await response.Content.ReadAsStringAsync());

                    return responseObj.serverTime;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
