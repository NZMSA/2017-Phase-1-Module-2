# Integrating Camera in Xamarin Forms

## 1. Installing the Nuget Package

* Firstly search for and install ```Xam.Plugin.Media``` in all your projects.

## 2. Permissions

For Android, since API 23, much like on iOS, if you want to access the camera you have to request permissions during runtime.

* Expand your Android project and open 'MainActivity.cs'
* Add the following lines of code

```
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
{
	PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```

* If your application is targetting Android N (API 24) then you must add the following lines to your manifest file

```
<provider android:name="android.support.v4.content.FileProvider" 
                android:authorities="YOUR_APP_PACKAGE_NAME.fileprovider" 
                android:exported="false" 
                android:grantUriPermissions="true">
            <meta-data android:name="android.support.FILE_PROVIDER_PATHS" 
                android:resource="@xml/file_paths"></meta-data>
</provider>
```

* Inside the resouce file within your Android project, create a new folder called 'xml' and inside that folder, create a new .xml file called 'file_paths.xml'
* Add the following lines of code to that file.

```
<?xml version="1.0" encoding="utf-8"?>
<paths xmlns:android="http://schemas.android.com/apk/res/android">
    <external-path name="my_images" path="Android/data/YOUR_APP_PACKAGE_NAME/files/Pictures" />
    <external-path name="my_movies" path="Android/data/YOUR_APP_PACKAGE_NAME/files/Movies" />
</paths>
```

For iOS, open info.plst and add the following lines to that file.

```
<key>NSCameraUsageDescription</key>
<string>This app needs access to the camera to take photos.</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>This app needs access to photos.</string>
<key>NSMicrophoneUsageDescription</key>
<string>This app needs access to microphone.</string>
```

## 3. Taking photos

* First we'll modify the layout of our main page.
* Add the following lines of code

```
<StackLayout BackgroundColor="White">
      <Button Text="Take Picture" TextColor="White" BackgroundColor="Green" Clicked="TakePicture_Clicked" />
      <Image x:Name="image" VerticalOptions="Start" />
    </StackLayout>
```

* Navigate to your portable project and inside MainPage.xaml.cs, after creating our button that's going to trigger the camera, you're going to first check if we have permissions to use the camera before takeing a photo. Add the following lines of code.

```
var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
{
    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] {Permission.Camera, Permission.Storage});
    cameraStatus = results[Permission.Camera];
    storageStatus = results[Permission.Storage];
}
```

* To take a photo add the following lines of code

```
if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
{
	var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
	{
		DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
		Directory = "Moodify",
		Name = $"{DateTime.UtcNow}.jpg",
		CompressionQuality = 92
	});

	if (file == null)
		return;
}
else
{
	await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
	//On iOS you may want to send your user to the settings screen.
	//CrossPermissions.Current.OpenAppSettings();
}
```

* To diplay the image add the following lines of code.

```
image.Source = ImageSource.FromStream(() =>
{
	var stream = file.GetStream();
	file.Dispose();
	return stream;
});
```