using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using MovieWebApplication.Configuration;
using MovieWebApplication.Models;

namespace MovieWebApplication.Services
{
    public class OmdbMovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly OmdbApiConfiguration _configuration;
        private readonly ILogger<OmdbMovieService> _logger;
        private readonly string _apiKeyParameter;
        public OmdbMovieService(IOptions<OmdbApiConfiguration> options, ILogger<OmdbMovieService> logger)
        {
            _configuration = options.Value;
            _logger = logger;
            var _apiKey = _configuration.ApiKey;
            _apiKeyParameter = $"?apikey={_apiKey}";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://www.omdbapi.com/");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        }

        public async Task<TResult> GetAndDeserializeAsync<TResult>(string endpoint)
        {
            var response = await _httpClient.GetAsync(_apiKeyParameter + endpoint);
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Success call to omdb API: {RequestUri}", response.RequestMessage?.RequestUri?.ToString());
            return JsonConvert.DeserializeObject<TResult>(content);
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            var error = new ErrorResponseModel();
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                error = JsonConvert.DeserializeObject<ErrorResponseModel>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing omdbapi error response");
            }
            finally
            {
                if (error != null)
                {
                    error.StatusCode = (int)response.StatusCode;
                }
                else
                {
                    error = new ErrorResponseModel() { StatusCode = (int)response.StatusCode };
                }
            }
            var errorMessage = $"Unsuccessful call to omdb API. Error code: {error.StatusCode} Error: {error.Error}";
            _logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }
}
