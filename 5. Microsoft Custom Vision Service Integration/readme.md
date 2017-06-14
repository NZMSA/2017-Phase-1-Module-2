# 5. Microsoft Custom Vision Service Integration
## Introduction
This repo contains the Windows client library & sample for the Microsoft Custom Vision Service, an offering within [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services)

## Learning Outcomes
* Understand Microsoft Custom Vision Service to an introductory level.
* Use Microsoft Custom Vision Service in your xamarin forms app.

We have used the Prediction Endpoint to Test Images Programmatically

## Step 1: Train your model

## Step 2: Obtain the prediction endpoint URL for a specific iteration:

1. Click the "PERFORMANCE" tab, which is shown inside a red rectangle in the following image.
2. In the left pane, click on the iteration you want to use for testing images.
3. In the upper part of the screen, click "Prediction URL", which is also shown in a red rectangle in the following image.

![Performance and Prediction tabs](img/performance-tab-and-prediction-url.png)

## Step 3: Test an image in C#

1. First add the Newtonsoft json NuGet package to your shared project.
2. Now we'll be adding the model which we will use to deserialise our response from custom vision services to. Right click on your shared project and add a new folder. Call it 'Model'.
3. Right click on your newly created folder and add a new CS file. Call it 'EvaluationModel.cs'.
4. Replace the contents of EvaluationModel.cs with the following:

```csharp
using System;
using System.Collections.Generic;

namespace Tabs.Model
{
    public class EvaluationModel
    {
		public string Id { get; set; }
		public string Project { get; set; }
		public string Iteration { get; set; }
		public string Created { get; set; }
		public List<Prediction> Predictions { get; set; }
    }

	public class Prediction
	{
		public string TagId { get; set; }
		public string Tag { get; set; }
		public double Probability { get; set; }
	}
}
```

Your `CustomVision.xaml.cs` file should look like this

```csharp
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Tabs.Model;
using Xamarin.Forms;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });


            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "a51ac8a57d4e4345ab0a48947a4a90ac");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/4da1555c-14ca-4aaf-af01-d6e1e97e5fa6/image?iterationId=7bc76035-3825-4643-917e-98f9d9f79b71";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = responseModel.Predictions.Max(m => m.Probability);

                    TagLabel.Text = (max >= 0.5) ? "Hotdog" : "Not hotdog";

                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
    }
}

```

And your `CustomVision.xaml` file should look like this

```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Tabs.CustomVision" Title="Custom Vision">
    <StackLayout Margin="20" Orientation="Vertical">
        <Button Text="Take Photo and Analyze" Clicked="loadCamera" />
        <StackLayout Orientation="Horizontal">
            <Label x:Name="TagLabel">
            </Label>
            <Label x:Name="PredictionLabel">
            </Label>
        </StackLayout>
        <Image x:Name="image" Aspect="AspectFit"/>
    </StackLayout>
</ContentPage>
```
## Resources
### Bootcamp Content
* [Slide Deck](http://link.com) (Will be up soon!)
* [Video](http://link.com) (Will be up soon!)

## [MORE INFO] The Prediction Client Library
The prediction client library is an automatically generated C\# wrapper that allows you to make predictions against trained endpoints.

The easiest way to get the prediction client library is to get the [Microsoft.Cognitive.CustomVision.Prediction](https://www.nuget.org/packages/Microsoft.Cognitive.CustomVision.Prediction/) package from [nuget](<http://nuget.org>). 

## The Training Client Library
The training client library is automatically generated C\# wrapper that allows you to create, manage and train Custom Vision projects programatically. All operations on the [website](<https://customvision.ai>) are exposed through this library, allowing you to automate all aspects of the Custom Vision Service.

The easiest way to get the training client library is to get the [Microsoft.Cognitive.CustomVision.Training](https://www.nuget.org/packages/Microsoft.Cognitive.CustomVision.Training/) package from [nuget](<http://nuget.org>).
