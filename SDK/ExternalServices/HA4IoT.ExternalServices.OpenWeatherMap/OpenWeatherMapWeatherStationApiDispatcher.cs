﻿using System;
using HA4IoT.Contracts.Api;
using HA4IoT.Contracts.Services.WeatherService;

namespace HA4IoT.ExternalServices.OpenWeatherMap
{
    public class OpenWeatherMapWeatherStationApiDispatcher
    {
        // TODO: Consider add API handling methods to service interface.
        private readonly OpenWeatherMapWeatherService _weatherStation;
        private readonly IApiController _apiController;

        public OpenWeatherMapWeatherStationApiDispatcher(OpenWeatherMapWeatherService weatherStation, IApiController apiController)
        {
            if (weatherStation == null) throw new ArgumentNullException(nameof(weatherStation));
            if (apiController == null) throw new ArgumentNullException(nameof(apiController));

            _weatherStation = weatherStation;
            _apiController = apiController;
        }

        public void ExposeToApi()
        {
            _apiController.RouteRequest("weatherStation", HandleApiGet);
            _apiController.RouteCommand("weatherStation", HandleApiPost);
        }

        private void HandleApiPost(IApiContext apiContext)
        {
            var situationValue = apiContext.Request.GetNamedString("situation", WeatherSituation.Unknown.ToString());
            var situation = (WeatherSituation)Enum.Parse(typeof (WeatherSituation), situationValue, true);

            var temperature = (float)apiContext.Request.GetNamedNumber("temperature");
            var humidity = (float)apiContext.Request.GetNamedNumber("humidity");
            var sunrise = TimeSpan.Parse(apiContext.Request.GetNamedString("sunrise"));
            var sunset = TimeSpan.Parse(apiContext.Request.GetNamedString("sunset"));

            _weatherStation.SetStatus(situation, temperature, humidity, sunrise, sunset);
        }

        private void HandleApiGet(IApiContext apiContext)
        {
            apiContext.Response = _weatherStation.ExportStatusToJsonObject();
        }
    }
}
