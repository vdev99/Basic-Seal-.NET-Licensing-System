using BasicSealBlazor.Dtos;
using BasicSealBlazor.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BasicSealBlazor.Services
{
    public class APIService : IAPIService
    {
        private readonly HttpClient _httpClient;

        public APIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginUser(UserAuthDto userAuthDto)
        {
            string apiName = "api/auth/users/login";
            var postData = JsonConvert.SerializeObject(userAuthDto);
            var response = await _httpClient.PostAsync(apiName, new StringContent(postData, Encoding.Unicode, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JObject responseObj = JObject.Parse(responseContent);

                return responseObj["token"].ToString();
            }
            return null;
        }

        public async Task<bool> RegisterUser(UserAuthDto userRegister)
        {
            string apiName = "api/auth/users/register";
            var postData = JsonConvert.SerializeObject(userRegister);
            var response = await _httpClient.PostAsync(apiName, new StringContent(postData, Encoding.Unicode, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<List<ApplicationsNavDto>> GetUserApps(string token) 
        {
            string apiName = "api/management/software";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<List<ApplicationsNavDto>>(responseContent);

                    return result;
                }
                return null;
            }
        }

        public async Task<bool> AddApplication(string token, string appName)
        {
            string apiName = $"api/management/software/new?appName={appName}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> DeleteApplication(string token, long appId)
        {
            string apiName = $"api/management/software?appId={appId}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<SoftwareVersionsDto> GetSoftwareVersions(string token, long appId)
        {
            string apiName = $"api/management/software/versions?appId={appId}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<SoftwareVersionsDto>(responseContent);

                    return result;
                }
                return null;
            }
        }

        public async Task<bool> AddSoftwareVersion(string token, AddSoftwareVersionDto softwareVersion)
        {
            string apiName = "api/management/software/versions/new";
            var postData = JsonConvert.SerializeObject(softwareVersion);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                requestMessage.Content = new StringContent(postData, Encoding.Unicode, "application/json");

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> DeleteSoftwareVersion(string token, long versionId)
        {
            string apiName = $"api/management/software/versions?versionId={versionId}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> GenerateLicenseKey(string token, GenerateLicenseKeyDto generateLicenseKeyParam)
        {
            string apiName = "api/management/licenses/generateKey";
            var postData = JsonConvert.SerializeObject(generateLicenseKeyParam);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                requestMessage.Content = new StringContent(postData, Encoding.Unicode, "application/json");

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<LicenseRespDto> GetLicenses(string token, GetLicenseReqDto getLicenseReq)
        {
            string apiName = "api/management/licenses";
            var postData = JsonConvert.SerializeObject(getLicenseReq);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                requestMessage.Content = new StringContent(postData, Encoding.Unicode, "application/json");

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<LicenseRespDto>(responseContent);

                    return result;
                }
                return null;
            }
        }

        public async Task<bool> DeleteSoftwareLicense(string token, long softwareId, long licenseId)
        {
            string apiName = $"api/management/licenses?softwareId={softwareId}&licenseId={licenseId}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<SoftwareLicenseDto> GetLicenseByKey(string token, long softwareId, string key)
        {
            string apiName = $"api/management/licenses?softwareId={softwareId}&key={key}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<SoftwareLicenseDto>(responseContent);

                    return result;
                }
                return null;
            }
        }

        public async Task<bool> UpdateSoftwareLicense(string token, SoftwareLicenseDto softwareLicense)
        {
            string apiName = "api/management/licenses/update";
            var postData = JsonConvert.SerializeObject(softwareLicense);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiName))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                requestMessage.Content = new StringContent(postData, Encoding.Unicode, "application/json");

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
