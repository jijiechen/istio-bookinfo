using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Reviews.Controllers
{
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReviewsController> _logger;
        private readonly Settings _settings;

        public ReviewsController(ILogger<ReviewsController> logger, IHttpClientFactory httpClientFactory, Settings settings)
        {
            _httpClient = httpClientFactory.CreateClient("microservices");
            _logger = logger;
            _settings = settings;
        }

        [HttpGet]
        [Route("/reviews/{productId}")]
        public async Task<IEnumerable<Review>> Get(int productId)
        {
            var reviews = new[]
            {
                new Review
                {
                    Reviewer = "Reviewer1",
                    Text = "An extremely entertaining play by Shakespeare. The slapstick humour is refreshing!",
                },
                new Review()
                {
                    Reviewer = "Reviewer2",
                    Text = "Absolutely fun and entertaining. The play lacks thematic depth when compared to other plays by Shakespeare.",
                }
            };
            
            if (_settings.RatingsEnabled)
            {
                var ratings = await GetRatings(productId);
                reviews[0].Rating = ratings[0];
                reviews[1].Rating = ratings[1];
            }

            return reviews;
        }

        [HttpGet]
        [Route("/health")]
        public object Health()
        {
            return new { Status = "Reviews is healthy" };
        }


        private async Task<Rating[]> GetRatings(int productId)
        {
            var ratings = new[]
            {
                new Rating() { Error = "Ratings service is currently unavailable" },
                new Rating() { Error = "Ratings service is currently unavailable" },
            };

            var domain = string.IsNullOrEmpty(_settings.ServiceDomain) ? "" : "." + _settings.ServiceDomain;
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{_settings.RatingsHostName}{domain}:9080/ratings/{productId}");
            try
            {
                var resp = await _httpClient.SendAsync(request);
                resp.EnsureSuccessStatusCode();
                var respTxt = await resp.Content.ReadAsStringAsync();
                var respObj = JsonSerializer.Deserialize<RatingsResponse>(respTxt);

                if (respObj != null)
                {
                    ratings[0].Error = null;
                    ratings[0].StarCounts = respObj.ratings.Reviewer1;
                    ratings[0].StarColor = _settings.StarColor;

                    ratings[1].Error = null;
                    ratings[1].StarCounts = respObj.ratings.Reviewer2;
                    ratings[1].StarColor = _settings.StarColor;
                }

                return ratings;
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Error when invoking ratings service at {@url}", request.RequestUri);
            }
            
            return ratings;
        }

        class RatingsResponse
        {
            public RatingsResponseRating ratings { get; set; } 
        }
        
        class RatingsResponseRating
        {
            public int Reviewer1 { get; set; }
            public int Reviewer2 { get; set; }
        }

    }
}