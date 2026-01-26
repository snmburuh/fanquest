using FanQuest.Infrastructure.Payments.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public class MpesaClient : IMpesaClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MpesaClient> _logger;
        private readonly IMemoryCache _cache;

        public MpesaClient(HttpClient httpClient, ILogger<MpesaClient> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<string> GetAccessTokenAsync(MpesaConfig config)
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

        public async Task<StkPushResponse> InitiateStkPushAsync(MpesaConfig config, StkPushRequest request)
        {
            try
            {
                var token = await GetAccessTokenAsync(config);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    $"{config.BaseUrl}/mpesa/stkpush/v1/processrequest");
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
                    _logger.LogError("STK Push failed: {Content}", content);
                    throw new MpesaException($"STK Push failed: {content}");
                }

                return JsonSerializer.Deserialize<StkPushResponse>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating STK Push");
                throw;
            }
        }

        public async Task<B2CResponse> InitiateB2CAsync(MpesaConfig config, B2CRequest request)
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
            MpesaConfig config,
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
    }
}
