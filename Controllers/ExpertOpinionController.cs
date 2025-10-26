using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpertOpinionController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string[] _sources = new[]
        {
            "aljazeera",
            "cnn",
            "youm7",
            "arabicnews",
            "alarabiya"
        };

        public ExpertOpinionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://arabic-news-api.p.rapidapi.com/");
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "arabic-news-api.p.rapidapi.com");
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", "58c8037881msh8f9170709a3c4ccp13b57bjsnba81bf8d69fa");
        }

        [HttpGet("business")]
        public async Task<IActionResult> GetBusinessOpinions()
        {
            try
            {
                var allArticles = new List<dynamic>();

                foreach (var source in _sources)
                {
                    var response = await _httpClient.GetAsync(source);
                    if (!response.IsSuccessStatusCode) continue;

                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var a in doc.RootElement.EnumerateArray())
                        {
                            a.TryGetProperty("title", out var titleProp);
                            a.TryGetProperty("link", out var linkProp);
                            a.TryGetProperty("source", out var sourceProp);
                            a.TryGetProperty("published", out var publishedProp);

                            allArticles.Add(new
                            {
                                title = titleProp.GetString() ?? "",
                                link = linkProp.GetString() ?? "",
                                source = sourceProp.GetString() ?? source,
                                published = publishedProp.GetString() ?? ""
                            });
                        }
                    }
                    else if (doc.RootElement.TryGetProperty("articles", out var articlesProp))
                    {
                        foreach (var a in articlesProp.EnumerateArray())
                        {
                            a.TryGetProperty("title", out var titleProp);
                            a.TryGetProperty("link", out var linkProp);
                            a.TryGetProperty("source", out var sourceProp);
                            a.TryGetProperty("published", out var publishedProp);

                            allArticles.Add(new
                            {
                                title = titleProp.GetString() ?? "",
                                link = linkProp.GetString() ?? "",
                                source = sourceProp.GetString() ?? source,
                                published = publishedProp.GetString() ?? ""
                            });
                        }
                    }
                }

                return Ok(new { status = 200, count = allArticles.Count, data = allArticles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "❌ Something went wrong", error = ex.Message });
            }
        }

    }
}