﻿using Blazor.WEB.Repositories.Web.Repositories;
using System.Text.Json;
using System.Text;

namespace Blazor.WEB.Repositories
{
    public class Repository : IRepository
    {
        private readonly HttpClient _httpClient;

        private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public Repository(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            var responseHttp = await _httpClient.GetAsync(url);


            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<T>(responseHttp, _jsonDefaultOptions);

                return new HttpResponseWrapper<T>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<T>(default, true, responseHttp);
        }


        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T model)
        {
            var mesageJSON = JsonSerializer.Serialize(model);

            var messageContet = new StringContent(mesageJSON, Encoding.UTF8, "application/json");

            var responseHttp = await _httpClient.PostAsync(url, messageContet);

            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T model)
        {
            var messageJSON = JsonSerializer.Serialize(model);

            var messageContet = new StringContent(messageJSON, Encoding.UTF8, "application/json");

            var responseHttp = await _httpClient.PostAsync(url, messageContet);

            if (responseHttp.IsSuccessStatusCode)
            {

                var response = await UnserializeAnswer<TResponse>(responseHttp, _jsonDefaultOptions);

                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<TResponse>(default, !responseHttp.IsSuccessStatusCode, responseHttp);
        }


        private async Task<T> UnserializeAnswer<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
        {
            var respuestaString = await httpResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(respuestaString, jsonSerializerOptions)!;
        }

    }
}
