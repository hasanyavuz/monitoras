using System.Collections.Generic;
using Monitoras.Web.Models;

namespace Monitoras.Web.Providers
{
    public interface IWeatherProvider
    {
        List<WeatherForecast> GetForecasts();
    }
}
