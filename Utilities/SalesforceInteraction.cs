using CourseProject.Database;
using CourseProject.ViewModels.Salesforce;
using Microsoft.Extensions.Localization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CourseProject.Utilities
{
    public class SalesforceInteraction
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private string? accessToken;
        public SalesforceInteraction(AppDbContext context, IConfiguration configuration, IStringLocalizer<SharedResources> localizer)
        {
            _context = context;
            _configuration = configuration;
            _localizer = localizer;
        }
        public async Task<(string AccountId, string ContactId)> Create(SalesforceFormViewModel model)
        {
            //if (string.IsNullOrEmpty(accessToken) || !await CheckToken(accessToken))
            //{
                accessToken = await GetSalesforceAccessToken();
            //}

            var userData = _context.Users.FirstOrDefault(x => x.Id == model.UserId);

            string accountId = userData.SalesforceAccountId;
            string contactId = userData.SalesforceContactId;

            if (!string.IsNullOrEmpty(accountId))
            {
                await UpdateSalesforceAccount(model, accountId, accessToken);
            }
            else
            {
                accountId = await CreateSalesforceAccount(model, accessToken);
            }

            if (!string.IsNullOrEmpty(contactId))
            {
                await UpdateSalesforceContact(model, contactId, accessToken);
            }
            else
            {
                contactId = await CreateSalesforceContact(model, accountId, accessToken);
            }

            return (accountId, contactId);
        }
        private async Task<string> GetSalesforceAccessToken()
        {
            var client = new HttpClient();
            var response = await client.PostAsync("https://login.salesforce.com/services/oauth2/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", _configuration.GetValue<string>("Salesforce:ClientId") },
                    { "client_secret", _configuration.GetValue<string>("Salesforce:ClientSecret") },
                    { "username", _configuration.GetValue<string>("Salesforce:Username") },
                    { "password", _configuration.GetValue<string>("Salesforce:Password") + _configuration.GetValue<string>("Salesforce:SecurityToken") }
                }));

            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return tokenData["access_token"].ToString();
        }

        private async Task<string> CreateSalesforceAccount(SalesforceFormViewModel model, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new
            {
                Name = model.AccountName,
                Phone = model.Phone
            };

            var response = await client.PostAsJsonAsync("https://karagos-dev-ed.develop.my.salesforce.com/services/data/v62.0/sobjects/Account", payload);

            var json = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return responseData["id"].ToString();
        }

        private async Task UpdateSalesforceAccount(SalesforceFormViewModel model, string accountId, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new
            {
                Name = model.AccountName,
                Phone = model.Phone
            };

            var response = await client.PatchAsJsonAsync($"https://karagos-dev-ed.develop.my.salesforce.com/services/data/v62.0/sobjects/Account/{accountId}", payload);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> CreateSalesforceContact(SalesforceFormViewModel model, string accountId, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                AccountId = accountId
            };

            var response = await client.PostAsJsonAsync("https://karagos-dev-ed.develop.my.salesforce.com/services/data/v62.0/sobjects/Contact", payload);

            var json = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return responseData["id"].ToString();
        }
        private async Task UpdateSalesforceContact(SalesforceFormViewModel model, string contactId, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };

            var response = await client.PatchAsJsonAsync($"https://karagos-dev-ed.develop.my.salesforce.com/services/data/v62.0/sobjects/Contact/{contactId}", payload);
            response.EnsureSuccessStatusCode();
        }

        private async Task<bool> CheckToken(string accessToken)
        {
            var client = new HttpClient();
            var response = await client.PostAsync("https://login.salesforce.com/services/oauth2/introspect",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "token", accessToken },
                    { "token_type_hint", "access_token"},
                    { "client_id", _configuration.GetValue<string>("Salesforce:ClientId") },
                    { "client_secret", _configuration.GetValue<string>("Salesforce:ClientSecret") }
                }));

            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return Convert.ToBoolean(tokenData["active"]);
        }

        public async Task<SalesforceFormViewModel?> GetSalesforceDataAsync(string accountId)
        {
            accessToken = await GetSalesforceAccessToken();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // 1. Получение данных Account
            var accountResponse = await client.GetAsync($"https://karagos-dev-ed.develop.my.salesforce.com/services/data/v62.0/sobjects/Account/{accountId}");
            accountResponse.EnsureSuccessStatusCode();

            var accountJson = await accountResponse.Content.ReadAsStringAsync();
            var accountData = JsonSerializer.Deserialize<Dictionary<string, object>>(accountJson);

            // 2. Поиск связанного Contact
            var contactQuery = $"SELECT Id, FirstName, LastName, Email FROM Contact WHERE AccountId = '{accountId}' LIMIT 1";
            var contactResponse = await client.GetAsync($"https://karagos-dev-ed.develop.my.salesforce.com/services/data/v62.0/query?q={Uri.EscapeDataString(contactQuery)}");
            contactResponse.EnsureSuccessStatusCode();

            var contactJson = await contactResponse.Content.ReadAsStringAsync();
            var contactData = JsonSerializer.Deserialize<Dictionary<string, object>>(contactJson);

            // Проверяем наличие записей Contact
            var contactRecords = contactData["records"] as JsonElement?;
            var firstContact = contactRecords?.EnumerateArray().FirstOrDefault();

            var viewModel = new SalesforceFormViewModel
            {
                AccountName = accountData["Name"]?.ToString(),
                Phone = accountData["Phone"]?.ToString(),
                FirstName = firstContact?.GetProperty("FirstName").GetString(),
                LastName = firstContact?.GetProperty("LastName").GetString(),
                Email = firstContact?.GetProperty("Email").GetString()
            };
            return viewModel;
        }
    }
}
