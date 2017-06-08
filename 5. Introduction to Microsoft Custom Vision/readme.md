# 4. Introduction to Microsoft Custom Vision Service
## Introduction
This repo contains the Windows client library & sample for the Microsoft Custom Vision Service, an offering within [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services)

## The Prediction Client Library
The prediction client library is an automatically generated C\# wrapper that allows you to make predictions against trained endpoints.

The easiest way to get the prediction client library is to get the [Microsoft.Cognitive.CustomVision.Prediction](https://www.nuget.org/packages/Microsoft.Cognitive.CustomVision.Prediction/) package from [nuget](<http://nuget.org>). 

## The Training Client Library
The training client library is automatically generated C\# wrapper that allows you to create, manage and train Custom Vision projects programatically. All operations on the [website](<https://customvision.ai>) are exposed through this library, allowing you to automate all aspects of the Custom Vision Service.

The easiest way to get the training client library is to get the [Microsoft.Cognitive.CustomVision.Training](https://www.nuget.org/packages/Microsoft.Cognitive.CustomVision.Training/) package from [nuget](<http://nuget.org>).

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

Your `CustomVision.xaml.cs` file should look like this

```csharp
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System.Linq;

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
           
                    JObject rss = JObject.Parse(responseString);

					//Querying with LINQ
                    //Get all Prediction Values
					var Probability = from p in rss["Predictions"] select (string)p["Probability"];
                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];

                    //Truncate values to labels in XAML
                    foreach (var item in Tag)
					{
						TagLabel.Text += item + ": \n";
					}

                    foreach (var item in Probability)
                    {
                        PredictionLabel.Text += item + "\n";
                    }

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

### Tools

### Extra Learning Resources
