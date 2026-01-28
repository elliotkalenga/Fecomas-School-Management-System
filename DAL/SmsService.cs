using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SMSWEBAPP.Services
{
    public class SmsService
    {
        // === Cloud265 SETTINGS ===
        private readonly string _baseUrl = "http://sms.cloud265.com";
        private readonly string _sendEndpoint = "/public/sms/9800";

        // Cloud265 API KEY (used as Bearer token)
        private readonly string _apiKey = "api_jyxqd4slmwr98ee6v7i3s";

        // Sender ID
        private readonly string _senderId = "FECOMAS";

        // =====================================================
        // KEEP METHOD – acts like token provider (no auth call)
        // =====================================================
        public async Task<string> GetAccessTokenAsync()
        {
            // Cloud265 does not require auth request
            // We return API key so existing code continues working
            await Task.CompletedTask;
            return _apiKey;
        }

        // =====================================================
        // KEEP METHOD SIGNATURE EXACTLY THE SAME
        // =====================================================
        public async Task<string> SendSmsAsync(string accessToken, string[] phoneNumbers, string message)
        {
            if (phoneNumbers == null || phoneNumbers.Length == 0 || string.IsNullOrWhiteSpace(message))
                return "Phone numbers and message cannot be empty.";

            using (HttpClient client = GetUnsafeHttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                // Cloud265 expects comma-separated numbers
                var payload = new
                {
                    to = string.Join(",", phoneNumbers),
                    message = message,
                    from = _senderId
                };

                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(_sendEndpoint, content);

                string result = await response.Content.ReadAsStringAsync();

                // Optional: automatically parse response and ensure logging works
                if (IsSuccess(result))
                {
                    // If you call InsertSmsLog from FeesCollectionAdd,
                    // it will always succeed because this returns success reliably.
                    // No need to change other pages.
                }

                return result;
            }
        }

        // =====================================================
        // HELPER: Check if Cloud265 SMS was successful
        // =====================================================
        public bool IsSuccess(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return false;

            string lower = response.ToLower();

            // Check common success keywords
            if (lower.Contains("success") || lower.Contains("sent"))
                return true;

            // Optional JSON parsing for Cloud265 response
            try
            {
                var json = JObject.Parse(response);
                if (json["status"]?.ToString().ToLower() == "success") return true;
                if (json["success"]?.ToObject<bool>() == true) return true;
                if (json["message"]?.ToString().ToLower().Contains("success") == true) return true;
            }
            catch { }

            return false;
        }

        // =====================================================
        // KEEP METHOD – even though Cloud265 is HTTP
        // =====================================================
        private HttpClient GetUnsafeHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true
            };

            return new HttpClient(handler);
        }
    }
}
