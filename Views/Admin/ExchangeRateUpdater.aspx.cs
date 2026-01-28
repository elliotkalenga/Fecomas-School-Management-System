using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.UI;

namespace SMSWEBAPP.Views.Admin
{
    public partial class ExchangeRateUpdater : System.Web.UI.Page
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _connectionString;

        public ExchangeRateUpdater()
        {
            _httpClient = new HttpClient();
            _baseUrl = "https://api.exchangerate-api.com/v4/latest/";
            _connectionString = AppConnection.GetConnectionString();
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            await UpdateExchangeRatesAsync();
        }

        protected async void btnUpdateExchangeRates_Click(object sender, EventArgs e)
        {
            await UpdateExchangeRatesAsync();
        }

        private async Task UpdateExchangeRatesAsync()
        {
            try
            {
                if (await IsExchangeRateUpdatedToday())
                {
                    lblStatus.Text = "Exchange rates have already been updated today.";
                    lblStatus.CssClass = "mt-3 d-block text-info";
                }
                else
                {
                    await UpdateExchangeRates();
                    lblStatus.Text = "Exchange rates updated successfully.";
                    lblStatus.CssClass = "mt-3 d-block text-success";
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                lblStatus.Text = "Error updating exchange rates: " + ex.Message;
                lblStatus.CssClass = "mt-3 d-block text-error";
            }
        }

        private async Task<bool> IsExchangeRateUpdatedToday()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT TOP 1 UpdatedDate FROM ExchangeRates ORDER BY UpdatedDate DESC";
                SqlCommand command = new SqlCommand(query, connection);

                object result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    DateTime lastUpdatedDate = Convert.ToDateTime(result);
                    return lastUpdatedDate.Date == DateTime.Today;
                }
            }

            return false;
        }

        private async Task UpdateExchangeRates()
        {
            string baseCurrency = await GetBaseCurrencyAsync();

            _httpClient.BaseAddress = new Uri(_baseUrl);
            var response = await _httpClient.GetAsync(baseCurrency);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var exchangeRateResponse = JsonSerializer.Deserialize<ExchangeRateResponse>(responseBody);

                await UpdateExchangeRatesInDatabase(exchangeRateResponse);
            }
            else
            {
                throw new Exception("Failed to fetch exchange rates from API.");
            }
        }

        private async Task<string> GetBaseCurrencyAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT CurrencyCode FROM Currencies WHERE BaseCurrency = 1";
                SqlCommand command = new SqlCommand(query, connection);

                object result = await command.ExecuteScalarAsync();
                return result?.ToString() ?? string.Empty;
            }
        }

        private async Task<int> GetCurrencyIdAsync(string currencyCode)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT CurrencyId FROM Currencies WHERE CurrencyCode = @CurrencyCode";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);

                object result = await command.ExecuteScalarAsync();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        private async Task UpdateExchangeRatesInDatabase(ExchangeRateResponse exchangeRateResponse)
        {
            string baseCurrency = exchangeRateResponse.@base;
            int currencyFromId = await GetCurrencyIdAsync(baseCurrency);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var rate in exchangeRateResponse.rates)
                {
                    int currencyToId = await GetCurrencyIdAsync(rate.Key);
                    decimal exchangeRate = rate.Value;

                    await UpdateExchangeRateAsync(connection, currencyFromId, currencyToId, exchangeRate);
                }
            }
        }

        private async Task UpdateExchangeRateAsync(SqlConnection connection, int currencyFromId, int currencyToId, decimal exchangeRate)
        {
            if (currencyFromId > 0 && currencyToId > 0)
            {
                string query = @"
                    IF EXISTS (SELECT 1 FROM ExchangeRates WHERE CurrencyFromId = @CurrencyFromId AND CurrencyToId = @CurrencyToId)
                    BEGIN
                        UPDATE ExchangeRates
                        SET ExchangeRate = @ExchangeRate, UpdatedDate = GETDATE()
                        WHERE CurrencyFromId = @CurrencyFromId AND CurrencyToId = @CurrencyToId
                    END
                    ELSE
                    BEGIN
                        INSERT INTO ExchangeRates (CurrencyFromId, CurrencyToId, ExchangeRate, EffectiveDate)
                        VALUES (@CurrencyFromId, @CurrencyToId, @ExchangeRate, CAST(GETDATE() AS DATE))
                    END";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CurrencyFromId", currencyFromId);
                command.Parameters.AddWithValue("@CurrencyToId", currencyToId);
                command.Parameters.AddWithValue("@ExchangeRate", exchangeRate);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public class ExchangeRateResponse
    {
        public string @base { get; set; }
        public DateTime date { get; set; }
        public Dictionary<string, decimal> rates { get; set; }
    }
}