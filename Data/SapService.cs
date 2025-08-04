using RestSharp;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace CvsServiceLayer.Data
{
    public class SapService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public SapService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private async Task<(string SessionId, string RouteId)> LoginAsync()
        {
            string _url = _config["SapSettings:Url"];
            string _companyName = _config["SapSettings:CompanyDB"];
            string _username = _config["SapSettings:UserName"];
            string _password = _config["SapSettings:Password"];

            string json = "{\"CompanyDB\": \"" + _companyName + "\", \"UserName\": \"" + _username + "\", \"Password\": \"" + _password + "\"}";
            var client = new RestClient(_url + "Login");
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new Exception($"Login failed: {response.StatusCode} - {response.Content}");
            }
            /*var client = new RestClient(_url + "Login");
            Console.WriteLine($"Calling: {_httpClient.BaseAddress}Login");*/
            //var response = await _httpClient.PostAsJsonAsync("Login", json);
            //response.EnsureSuccessStatusCode();

            // Get session cookies
            // Get Cookies
            var sessionId = response.Cookies.FirstOrDefault(c => c.Name == "B1SESSION")?.Value;
            var routeId = response.Cookies.FirstOrDefault(c => c.Name == "ROUTEID")?.Value;

            //var cookies = response.Cookies;
            //string sessionId = "";
            //string routeId = "";
            //string sessionId = cookies.FirstOrDefault(c => c.Contains("B1SESSION"))?.Split(';')[0].Split('=')[1];
            //string routeId = cookies.FirstOrDefault(c => c.Contains("ROUTEID"))?.Split(';')[0].Split('=')[1];

            return (sessionId, routeId);
        }
        
        public async Task<string> CompanyLogin(string username, string password, string companyName)
        {
            string _url = _config["SapSettings:Url"];
            /*string _companyName = _config["SapSettings:CompanyDB"];
            string _username = _config["SapSettings:UserName"];
            string _password = _config["SapSettings:Password"];*/

            string json = "{\"CompanyDB\": \"" + companyName + "\", \"UserName\": \"" + username + "\", \"Password\": \"" + password + "\"}";
            var client = new RestClient(_url + "Login");
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            
            if (!response.IsSuccessful)
            {
                //throw new Exception($"Login failed: {response.StatusCode} - {response.Content}");
                return "Login failed: {response.StatusCode} - {response.Content}";
            }

            return "Success";
        }

        public async Task<string> GetBusinessPartnersAsync(string cardType, int pageSize, int pagenumber)
        {
            var (sessionId, routeId) = await LoginAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, "BusinessPartners?$filter=CardType eq '"+ cardType + "'&$select=CardCode,CardName,GroupCode,Address&$top="+pageSize+"&$skip="+pageSize*pagenumber+"&$orderby=CardCode");
            request.Headers.Add("Cookie", $"B1SESSION={sessionId}; ROUTEID={routeId}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateBusinessPartnerAsync(object payload)
        {
            var (sessionId, routeId) = await LoginAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, "BusinessPartners")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Add("Cookie", $"B1SESSION={sessionId}; ROUTEID={routeId}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetItemMasterDataAsync(int pageSize, int pageNumber)
        {
            var (sessionId, routeId) = await LoginAsync();

            string requestUrl = $"Items?$select=ItemCode,ItemName,ItemType,ItemsGroupCode,ItemType,MaterialType&$top={pageSize}&$skip={pageSize * pageNumber}&$orderby=ItemCode";
            //Items?$select=ItemCode,ItemName,ItemType,ItemsGroupCode,ItemType,MaterialType&$top="+pageSize+"&$skip="+pageSize * pageNumber+"&$orderby=ItemCode

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Cookie", $"B1SESSION={sessionId}; ROUTEID={routeId}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }


        public async Task<List<CompanyInfo>> GetDatabaseListAsync()
        {
            var (sessionId, routeId) = await LoginAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, "CompanyService_GetCompanyList");
            request.Headers.Add("Cookie", $"B1SESSION={sessionId}; ROUTEID={routeId}");
            //request.Headers.Add("Cookie", $"{sessionId}; {routeId}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<CompanyListResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data.Value;
        }


/*
        {StatusCode: 400, ReasonPhrase: 'Bad Request', Version: 1.1, Content: System.Net.Http.HttpConnectionResponseContent, Headers:
{
  Date: Fri, 01 Aug 2025 12:31:20 GMT
  Server: Apache/2.4.51 (Unix)
  DataServiceVersion: 3.0
  Vary: Accept-Encoding
  Connection: close
  Transfer-Encoding: chunked
  Content-Type: application/json;charset=utf-8
}
}

*/

//Model
public class CompanyInfo
        {
            public string CompanyName { get; set; }
            public string DatabaseName { get; set; }
        }

        public class CompanyListResponse
        {
            public List<CompanyInfo> Value { get; set; }
        }

    }
}
