using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using OpenWeatherMap;

namespace Amaya.Modules
{
    public class weatherCommands : ModuleBase<SocketCommandContext>
    {

        [Command("weather")]
        [Alias("w")]
        public async Task WeatherForecast(string cityname = null)
        {
            if (cityname != null)
            {
                OpenWeatherMapClient client = new OpenWeatherMapClient("27ee363c7f1047c573fa8baeebb65bf3");
                var weather = await client.CurrentWeather.GetByName(cityname, MetricSystem.Metric, OpenWeatherMapLanguage.EN);

                EmbedBuilder embed = new EmbedBuilder();
                embed.WithAuthor($"Here is the weather forecast for {weather.City.Name}", Context.Guild.IconUrl);
                embed.WithTitle($"{weather.City.Name}, {weather.Temperature.Value}°C, {weather.Clouds.Name}");

                EmbedFieldBuilder humidity = new EmbedFieldBuilder();
                EmbedFieldBuilder temp = new EmbedFieldBuilder();
                EmbedFieldBuilder wind = new EmbedFieldBuilder();


                temp.WithName("***Temperature:***").WithValue($"**Max:** *{weather.Temperature.Max}°C*\n**Min:** *{weather.Temperature.Min}°C*").WithIsInline(false);
                humidity.WithName("***Humidity:***").WithValue($"*{weather.Humidity.Value}%*").WithIsInline(false);
                wind.WithName("***Wind:***").WithValue($"*{weather.Wind.Speed.Name}*\n**Direction:**\n*{weather.Wind.Direction.Name}*").WithIsInline(false);


                embed.WithFields(temp, wind, humidity);

                await ReplyAsync("", false, embed.Build());
            }
            else
            {
                await ReplyAsync("You have to specify the location!\nExample `a.weather London`");
            }
        }

    }
}
