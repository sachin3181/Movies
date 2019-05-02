using Microsoft.AspNetCore.JsonPatch;
using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class PartialUpdateService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        private const string ACCEPT_FORMAT = "application/json";

        public PartialUpdateService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }
        public async Task Run()
        {
            await PatchResource();
        }

        private async Task PatchResource()
        {
            var patch = new JsonPatchDocument<MovieForUpdate>();

            patch.Replace(m => m.Title, "Updated Title");
            patch.Remove(m => m.Description);

            var serializedChangeSet = JsonConvert.SerializeObject(patch);

            var request = new HttpRequestMessage(HttpMethod.Patch,
            "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ACCEPT_FORMAT));
            request.Content = new StringContent(serializedChangeSet);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
        }
    }
}
