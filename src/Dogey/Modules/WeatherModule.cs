using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireEnabled]
    public class WeatherModule : DogeyModuleBase
    {
        private readonly WeatherService _weather;

        public WeatherModule(WeatherService weather)
        {
            _weather = weather;
        }

        [Command("weather")]
        public async Task WeatherAsync([Remainder]string city)
        {
            var forecast = await _weather.GetForecastAsync(city, WeatherUnit.Imperial);
            if (forecast == null)
            {
                await ReplyAsync($"Could not find a city like `{city}`");
                return;
            }

            var weather = forecast.Weather.First();

            var embed = new EmbedBuilder()
                .WithColor(Color.DarkBlue)
                .WithImageUrl(WeatherService.GetIconUrl(weather.IconId))
                .WithTitle(forecast.Name + " Weather")
                .WithDescription(weather.Description)
                .WithCurrentTimestamp()
                .AddInlineField("Pressure", forecast.Measurements.Pressure + "**hPA**")
                .AddInlineField("Humidity", forecast.Measurements.Humidity + "%")
                .AddField("Temp", MathHelper.KelvinToFahrenheit(forecast.Measurements.Temperature))
                .AddInlineField("Temp Max", MathHelper.KelvinToFahrenheit(forecast.Measurements.TemperatureMax))
                .AddInlineField("Temp Min", MathHelper.KelvinToFahrenheit(forecast.Measurements.TemperatureMin));
            await ReplyEmbedAsync(embed);
        }
    }
}
