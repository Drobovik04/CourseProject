using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace CourseProject.Utilities
{
    public class OdooIntegration
    {
        private readonly IConfiguration _configuration;
        private readonly string _url;
        private readonly string _apiKey;

        public OdooIntegration(IConfiguration configuration)
        {
            _configuration = configuration;
            _url = _configuration.GetValue<string>("Odoo:Url");
            _apiKey = _configuration.GetValue<string>("Odoo:ApiKey");
        }

        public async Task CreateTemplateAsync(string title, string author, int questionCount)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var payload = new
            {
                name = title,
                author = author,
                question_count = questionCount
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_url}/api/templates", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create template: {error}");
            }
        }
    }
}
