using FanQuest.Domain.Entities;
using FanQuest.Infrastructure.Payments.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public class MpesaClient : IMpesaClient
    {
        private readonly HttpClient _httpClient;
        private readonly IMpesaConfigService _configService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MpesaClient> _logger;
        private readonly IMemoryCache _cache;

        public MpesaClient(HttpClient httpClient, IMpesaConfigService configService,
        IConfiguration configuration, ILogger<MpesaClient> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _configService = configService;
            _configuration = configuration;
            _logger = logger;
            _cache = cache;
        }

        public async Task<string> GetAccessTokenAsync(MpesaConfiguration config)
        {
            var cacheKey = $"mpesa_token_{config.ConsumerKey}";

            if (_cache.TryGetValue(cacheKey, out string cachedToken))
            {
                return cachedToken;
            }

            try
            {
                var credentials = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{config.ConsumerKey}:{config.ConsumerSecret}")
                );

                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"{config.BaseUrl}/oauth/v1/generate?grant_type=client_credentials");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);

                // Cache for 50 minutes (token valid for 60)
                _cache.Set(cacheKey, tokenResponse.AccessToken, TimeSpan.FromMinutes(50));

                return tokenResponse.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get M-Pesa access token");
                throw;
            }
        }

        public async Task<StkPushResponse> InitiateStkPushAsync(
         StkPushRequest request,
         Guid tenantId)
        {
            var config = await _configService.GetConfigurationAsync(tenantId);
            var token = await GetAccessTokenAsync(config);

            var baseUrl = config.Environment == MpesaEnvironment.Sandbox
                ? _configuration["Mpesa:SandboxUrl"]
                : _configuration["Mpesa:ProductionUrl"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync(
                $"{baseUrl}/mpesa/stkpush/v1/processrequest",
                request);

            return await response.Content.ReadFromJsonAsync<StkPushResponse>()
                ?? throw new InvalidOperationException("Failed to parse M-Pesa response");
        }

        public async Task<B2CResponse> InitiateB2CAsync(
            B2CRequest request,
            Guid tenantId)
        {
            var config = await _configService.GetConfigurationAsync(tenantId);
            var token = await GetAccessTokenAsync(config);

            var baseUrl = config.Environment == MpesaEnvironment.Sandbox
                ? _configuration["Mpesa:SandboxUrl"]
                : _configuration["Mpesa:ProductionUrl"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync(
                $"{baseUrl}/mpesa/b2c/v1/paymentrequest",
                request);

            return await response.Content.ReadFromJsonAsync<B2CResponse>()
                ?? throw new InvalidOperationException("Failed to parse M-Pesa response");
        }

        public async Task<B2CResponse> InitiateB2CAsync(MpesaConfiguration config, B2CRequest request)
        {
            try
            {
                var token = await GetAccessTokenAsync(config);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    $"{config.BaseUrl}/mpesa/b2c/v3/paymentrequest");
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(httpRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("B2C request failed: {Content}", content);
                    throw new MpesaException($"B2C request failed: {content}");
                }

                return JsonSerializer.Deserialize<B2CResponse>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating B2C");
                throw;
            }
        }

        public async Task<TransactionStatusResponse> QueryTransactionStatusAsync(
            MpesaConfiguration config,
            TransactionStatusRequest request)
        {
            try
            {
                var token = await GetAccessTokenAsync(config);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    $"{config.BaseUrl}/mpesa/transactionstatus/v1/query");
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(httpRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Transaction status query failed: {Content}", content);
                    throw new MpesaException($"Transaction status query failed: {content}");
                }

                return JsonSerializer.Deserialize<TransactionStatusResponse>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying transaction status");
                throw;
            }
        }

        public Task<TransactionStatusResponse> QueryTransactionStatusAsync(string transactionId, Guid tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessTokenAsync(Guid tenantId)
        {
            throw new NotImplementedException();
        }
    }
}
