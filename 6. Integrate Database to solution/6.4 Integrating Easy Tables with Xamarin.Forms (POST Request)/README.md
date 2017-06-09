# 6.4 Integrating Easy Tables with Xamarin.Forms (POST Request) 

## 6.4.1 Posting Hot Dog Information
To post a new notHotDogModel entry to our backend, we can invoke a `InsertAsync(notHotDogModel)` method call, where `notHotDogModel` is a NotHotDogModel object.

Lets create a `PostHotDogInformation` method in our `AzureManager.cs` file

```Csharp
public async Task PostHotDogInformation(NotHotDogModel notHotDogModel)
{
    await this.notHotDogTable.InsertAsync(notHotDogModel);
}
``` 

NOTE: If a unique `Id` is not included in the `notHotDogModel` object when we insert it, the server generates one for us.

Now to call our `PostHotDogInformation` function, we can do the following in our `CustomVision.xaml.cs` class at the end of the `loadCamera` method so that once an image is taken, we can upload the location of where that image was taken.

## [GeoLocation Platform Setup](https://blog.xamarin.com/geolocation-for-ios-android-and-windows-made-easy/)
Add the [Geolocator Plugin for Xamarin and Windows](https://www.nuget.org/packages/Xam.Plugin.Geolocator) NuGet to your cross-platform portable class library and each of the platform-specific projects. Geolocation taps into the native location services for both iOS and Android. Because a user’s location is private, we must configure our applications to ask the user if it’s okay if we access their current location.

###  Android
To allow our application to access location services, we need to enable two Android permissions: `ACCESS_COARSE_LOCATION` and `ACCESS_FINE_LOCATION`. If you have added the NuGet to the Android project directly, this should be enabled for you by default. Double-check to make sure the following permissions are enabled:

If you are targeting Android Marshmallow (Android M) or above, users will also be prompted automatically for runtime permissions.

### iOS
Depending on if you will be always using geolocation (such as a maps app), or just at certain points in a user’s workflow, you will either need to add the key NSLocationWhenInUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist, along with a new string entry for the key that describes exactly what you’ll be doing with the user’s location. When the permission is prompted to the user at runtime, the description listed here will display.

Additionally, if you wish to support background updates (iOS9+ only), you must enable the AllowsBackgroundUpdates property of the Geolocator. The presence of the UIBackgroundModes key with the location value is also required for background updates.

## Detecting a User’s Location
The Geolocator Plugin for Xamarin and Windows boils all of the complexity of location services down to a single asychronous method:

Add this code `await postLocationAsync();` after 

```csharp
	image.Source = ImageSource.FromStream(() =>
    {
        return file.GetStream();
    });
```

Ensure you add `using Plugin.Geolocator;` where all your other using statements are then copy and paste the following code: 

```Csharp
    async Task postLocationAsync()
    {

        var locator = CrossGeolocator.Current;
        locator.DesiredAccuracy = 50;

        var position = await locator.GetPositionAsync(10000);

        NotHotDogModel model = new NotHotDogModel()
        {
            Longitude = (float)position.Longitude,
            Latitude = (float)position.Latitude

        };

        await AzureManager.AzureManagerInstance.PostHotDogInformation(model);
    }
``` 

Since our Model is the same as our Easy table column names azure manager knows that we can just create a new row and insert the appropriate information accordingly. 

This creates a `NotHotDogModel` object and sets up the values from the `position` (from CrossGeolocator) and then adds it to backends database

### 6.4.2 [More Info] Updating and deleting notHotDogModel data
To edit an existing notHotDogModel entry in our backend, we can invoke a `UpdateAsync(notHotDogModel)` method call, where `notHotDogModel` is a NotHotDogModel object. 

The `Id` of the notHotDogModel object needs to match the one we want to edit as the backend uses the `id` field to identify which row to update. This applies to delete as well.

NotHotDogModel entries that we retrieve by `ToListAsync()`, will have all the object's corresponding `Id` attached and the object returned by the result of `InsertAsync()` will also have its `Id` attached.

Lets create a `UpdateHotDogInformation` method in our `AzureManager` activity 
```Csharp
    public async Task UpdateHotDogInformation(NotHotDogModel notHotDogModel) {
        await this.notHotDogTable.UpdateAsync(notHotDogModel);
    }
``` 

NOTE: If no `Id` is present, an `ArgumentException` is raised.


To edit an existing notHotDogModel entry in our backend, we can invoke a `DeleteAsync(notHotDogModel)` method call, where `notHotDogModel` is a NotHotDogModel object. 
Likewise information concerning `Id` applies to delete as well.

Lets create a `DeleteHotDogInformation` method in our `AzureManager` activity 
```Csharp
    public async Task DeleteHotDogInformation(NotHotDogModel notHotDogModel) {
        await this.notHotDogTable.DeleteAsync(notHotDogModel);
    }
``` 

### Extra Learning Resources
* [Using App Service with Xamarin by Microsoft](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/)
* [Using App Service with Xamarin by Xamarin - Outdated but good to understand](https://blog.xamarin.com/getting-started-azure-mobile-apps-easy-tables/)
