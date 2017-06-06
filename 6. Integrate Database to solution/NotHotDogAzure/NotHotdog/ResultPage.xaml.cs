using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace NotHotdog
{
    public partial class ResultPage : ContentPage
    {
        public ResultPage(MediaFile file)
        {
            InitializeComponent();

			image.Source = ImageSource.FromStream(() =>
			{
				var stream = file.GetStream();
				file.Dispose();
				return stream;
			});

            MakePredictionRequest(file);
        }

		static byte[] GetImageAsByteArray(MediaFile file)
		{
            var stream = file.GetStream();
			BinaryReader binaryReader = new BinaryReader(stream);
			return binaryReader.ReadBytes((int)stream.Length);
		}

		static async Task MakePredictionRequest(MediaFile file)
		{
			var client = new HttpClient();

			// Request headers - replace this example key with your valid subscription key.
			client.DefaultRequestHeaders.Add("Prediction-Key", "a51ac8a57d4e4345ab0a48947a4a90ac");

			// Prediction URL - replace this example URL with your valid prediction URL.
			string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/4da1555c-14ca-4aaf-af01-d6e1e97e5fa6/image?iterationId=7bc76035-3825-4643-917e-98f9d9f79b71";

			HttpResponseMessage response;

			// Request body. Try this sample with a locally stored image.
			byte[] byteData = GetImageAsByteArray(file);

			using (var content = new ByteArrayContent(byteData))
			{
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(url, content);
				Debug.WriteLine(await response.Content.ReadAsStringAsync());
			}
		}
    }
}
