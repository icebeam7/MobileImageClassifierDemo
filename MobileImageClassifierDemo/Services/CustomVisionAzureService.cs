using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using MobileImageClassifierDemo.Models;
using MobileImageClassifierDemo.Helpers;

using Newtonsoft.Json;
using Plugin.Media.Abstractions;

namespace MobileImageClassifierDemo.Services
{
    public static class CustomVisionAzureService
    {
        private static readonly HttpClient client = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Constants.PredictionBaseURL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Prediction-Key", Constants.PredictionKey);
            return client;
        }

        public async static Task<string> ClassifyImage(MediaFile picture)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var stream = new MemoryStream();
                await picture.GetStreamWithImageRotatedForExternalStorage().CopyToAsync(stream);

                using (var content = new ByteArrayContent(stream.ToArray()))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    var response = await client.PostAsync(Constants.PredictionProjectRequest, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var predictionResult = await response.Content.ReadAsStringAsync();
                        var customVisionResult = JsonConvert.DeserializeObject<CustomVisionResult>(predictionResult);

                        if (customVisionResult.Predictions.Count > 0)
                        {
                            var topPrediction = customVisionResult.Predictions.OrderByDescending(x => x.Probability).First();
                            return topPrediction.Probability > 0.5
                                ? $"{topPrediction.TagName} ({Math.Round(topPrediction.Probability * 100, 2):0.##} %) --API--"
                                : "N/A";
                        }
                        else
                        {
                            return "There was an error. Try again.";
                        }
                    }
                    else
                    {
                        return "There was an error. Try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"There was an exception: {ex.Message}";
            }
        }
    }
}
