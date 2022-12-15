using EncodingDecodingApi.Filter;
using EncodingDecodingApi.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EncodingDecodingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("Hello")]
        public IActionResult Index(User model)
        {
            User json12 = model;
            User res = new User
            {
                age = 0,
                name = "zero"
            };

            List<User> list = new List<User>();
            list.Add(res);
            list.Add(res);
            list.Add(res);
            list.Add(res);
            list.Add(res);
            list.Add(res);
            list.Add(res);
            list.Add(res);
            list.Add(res);
            return Ok(list);
        }
    }
}