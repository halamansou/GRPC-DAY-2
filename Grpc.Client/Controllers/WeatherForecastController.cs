using Microsoft.AspNetCore.Mvc;

namespace GRPC.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] WeatherSummaries = new[]
        {
            "Snowy", "Windy", "Cold", "Chilly", "Moderate", "Pleasant", "Warm", "Hot", "Humid", "Blazing"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Generating weather forecast");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-30, 50),
                Summary = WeatherSummaries[Random.Shared.Next(WeatherSummaries.Length)]
            })
            .ToArray();
        }
    }
}
