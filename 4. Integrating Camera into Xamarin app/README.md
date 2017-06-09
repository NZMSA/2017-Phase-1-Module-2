# 4. Integrating Camera in Xamarin Forms

## 1. Installing the Nuget Package

* Firstly search for and install `Xam.Plugin.Media` in *`all`* your projects.

## 2. Permissions

The WRITE_EXTERNAL_STORAGE & READ_EXTERNAL_STORAGE permissions are required, but the library will automatically add this for you. Additionally, if your users are running Marshmallow the Plugin will automatically prompt them for runtime permissions. You must add the Permission Plugin code into your Main or Base Activities:

```
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
{
    PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```

By adding these permissions Google Play will automatically filter out devices without specific hardware. You can get around this by adding the following to your AssemblyInfo.cs file in your Android project:

```
[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
```

* If your application is targeting Android N (API 24) then you must add the following lines to your manifest file inside the application tag.

```xml
<provider android:name="android.support.v4.content.FileProvider" 
                android:authorities="YOUR_APP_PACKAGE_NAME.fileprovider" 
                android:exported="false" 
                android:grantUriPermissions="true">
            <meta-data android:name="android.support.FILE_PROVIDER_PATHS" 
                android:resource="@xml/file_paths"></meta-data>
</provider>
```

`YOUR_APP_PACKAGE_NAME` must be set to your app package name!

* Add a new folder called xml into your Resources folder and add a new XML file called file_paths.xml

```
<?xml version="1.0" encoding="utf-8"?>
<paths xmlns:android="http://schemas.android.com/apk/res/android">
    <external-path name="my_images" path="Android/data/YOUR_APP_PACKAGE_NAME/files/Pictures" />
    <external-path name="my_movies" path="Android/data/YOUR_APP_PACKAGE_NAME/files/Movies" />
</paths>
```

`YOUR_APP_PACKAGE_NAME` must be set to your app package name!

For iOS, open info.plst and add the following lines to that file.

```xml
<key>NSCameraUsageDescription</key>
<string>This app needs access to the camera to take photos.</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>This app needs access to photos.</string>
<key>NSMicrophoneUsageDescription</key>
<string>This app needs access to microphone.</string>
```

## 3. Taking photos

* First we'll modify the layout of our main page.
* Add the following lines of code to our `CustomVision.xaml` file

```xml
<StackLayout Margin="20" Orientation="Vertical">
	<Button Text="Take Photo and Analyze" Clicked="loadCamera" />
	<Image x:Name="image" />
</StackLayout>
```

* Notice the Clicked method will create a method called loadCamera in your `CustomVision.xaml.cs`

* Navigate to your portable project and inside `CustomVision.xaml.cs`, after creating our button that's going to trigger the camera, you're going to first check if we have permissions to use the camera before taking a photo. Add the following lines of code.

```csharp
await CrossMedia.Current.Initialize();

if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
{
	await DisplayAlert("No Camera", ":( No camera available.", "OK");
	return;
}
```

* To take a photo add the following lines of code

```csharp
 MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
{
	PhotoSize = PhotoSize.Medium,
	Directory = "Sample",
	Name = $"{DateTime.UtcNow}.jpg"
});
```

* To diplay the image add the following lines of code.

```csharp
if (file == null)
	return;

image.Source = ImageSource.FromStream(() =>
{
	return file.GetStream();
});

file.Dispose();
```

Your `CustomVision.xaml.cs` file should look like this now: 

```csharp
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

            file.Dispose();
        }
    }
}

```