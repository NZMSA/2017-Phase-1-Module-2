# 6.4 Integrating Easy Tables with Xamarin.Forms (POST Request) 

## 6.4.1 Posting timeline data
To post a new timeline entry to our backend, we can invoke a `InsertAsync(timeline)` method call, where `timeline` is a Timeline object.

Lets create a `AddTimeline` method in our `AzureManager.cs` file

```Csharp
    public async Task AddTimeline(Timeline timeline) {
        await this.timelineTable.InsertAsync(timeline);
    }
``` 

NOTE: If a unique `Id` is not included in the `timeline` object when we insert it, the server generates one for us.


Now to can call our `AddTimeline` function, we can do the following in our `HomePageXaml.cs` class at the end of the `TakePicture_Clicked` method so that each response from cognitive services is uploaded

Add this code after the  line `EmotionView.ItemsSource = result[0].Scores.ToRankedList();`

```Csharp
    var temp = result[0].Scores;

    Timeline emo = new Timeline()
    {
        Anger = temp.Anger,
        Contempt = temp.Contempt,
        Disgust = temp.Disgust,
        Fear = temp.Fear,
        Happiness = temp.Happiness,
        Neutral = temp.Neutral,
        Sadness = temp.Sadness,
        Surprise = temp.Surprise,
        Date = DateTime.Now
    };

    await AzureManager.AzureManagerInstance.AddTimeline(emo);
``` 

This creates a `Timeline` object and sets up the values from the `result` (from cognitive services) and then adds it to backends database

### 6.4.2 [More Info] Updating and deleting timeline data
To edit an existing timeline entry in our backend, we can invoke a `UpdateAsync(timeline)` method call, where `timeline` is a Timeline object. 

The `Id` of the timeline object needs to match the one we want to edit as the backend uses the `id` field to identify which row to update. This applies to delete as well.

Timeline entries that we retrieve by `ToListAsync()`, will have all the object's corresponding `Id` attached and the object returned by the result of `InsertAsync()` will also have its `Id` attached.

Lets create a `UpdateTimeline` method in our `AzureManager` activity 
```Csharp
    public async Task UpdateTimeline(Timeline timeline) {
        await this.timelineTable.UpdateAsync(timeline);
    }
``` 

NOTE: If no `Id` is present, an `ArgumentException` is raised.


To edit an existing timeline entry in our backend, we can invoke a `DeleteAsync(timeline)` method call, where `timeline` is a Timeline object. 
Likewise information concerning `Id` applies to delete as well.

Lets create a `DeleteTimeline` method in our `AzureManager` activity 
```Csharp
    public async Task DeleteTimeline(Timeline timeline) {
        await this.timelineTable.DeleteAsync(timeline);
    }
``` 

### Extra Learning Resources
* [Using App Service with Xamarin by Microsoft](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/)
* [Using App Service with Xamarin by Xamarin - Outdated but good to understand](https://blog.xamarin.com/getting-started-azure-mobile-apps-easy-tables/)
* [ListView in Xamarin](https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/)