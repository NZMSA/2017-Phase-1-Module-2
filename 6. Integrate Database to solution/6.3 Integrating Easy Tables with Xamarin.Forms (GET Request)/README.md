# 6.3 Integrating Easy Tables with Xamarin.Forms (GET Request) 

### 6.3.1 Referencing Azure Mobile Services
At the earlier sections, we would have already added it to our Nuget Packages. If not

- For Visual Studio: Right-click your project, click Manage NuGet Packages, search for the `Microsoft.Azure.Mobile.Client` package, then click Install.
- For Xamarin Studio: Right-click your project, click Add > Add NuGet Packages, search for the `Microsoft.Azure.Mobile.Client` package, and then click Add Package.

#### `NOTE: Make sure to add it to all your solutions!`

If we want to use this SDK we add the following using statement
```Csharp
using Microsoft.WindowsAzure.MobileServices;
``` 

### 6.3.2 Creating Model Classes
Lets now create model class `NotHotDogModel` to represent the tables in our database. 
So in `Moodify (Portable)`, create a folder named `DataModels` and then create a `NotHotDogModel.cs` file with,

NOTE: If your table in your backend is not called NotHotDogModel, rename it or rename this class and file to match.

```Csharp
public class NotHotDogModel
{
    [JsonProperty(PropertyName = "Id")]
    public string ID { get; set; }

    [JsonProperty(PropertyName = "Longitude")]
    public float Longitude { get; set; }

    [JsonProperty(PropertyName = "Latitude")]
    public float Latitude { get; set; }
}
``` 

#### You might have noticed that in our database we defined the "Longitude" and "Latitude" values as string but C sharp is smart enough to convert his to a float if it sees fit.  

- `JsonPropertyAttribute` is used to define the PropertyName mapping between the client type and the table 
- Important that they match the field names that we got from our postman request (else it wont map properly)
- Our field names for our client types can then be renamed if we want (like the field `date`)
- All client types must contain a field member mapped to `Id` (default a string). The `Id` is required to perform CRUD operations and for offline sync (not discussed)
 

### 6.3.3 Initalize the Azure Mobile Client
Lets now create a singleton class named `AzureManager` that will look after our interactions with our web server. Add this to the class
(NOTE: replace `MOBILE_APP_URL` with your server name, for this demo its "https://nothotdoginformation.azurewebsites.net/")


So in `Moodify (Portable)`, create a `AzureManager.cs` file with,

```Csharp
public class AzureManager
{

    private static AzureManager instance;
    private MobileServiceClient client;

    private AzureManager()
    {
        this.client = new MobileServiceClient("MOBILE_APP_URL");
    }

    public MobileServiceClient AzureClient
    {
        get { return client; }
    }

    public static AzureManager AzureManagerInstance
    {
        get
        {
            if (instance == null) {
                instance = new AzureManager();
            }

            return instance;
        }
    }
}
``` 

Now if we want to access our `MobileServiceClient` in an activity we can add the following line, for the purpose of this tutorial we could add it to our `AzureTables.xaml.cs` file 
```Csharp
public partial class AzureTable : ContentPage
{
    
    MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

    public AzureTable()
    {
        InitializeComponent();

    }

}
``` 

### 6.3.4 Creating a table references
For this demo we will consider a database table a `table`, so all code that accesses (READ) or modifies (CREATE, UPDATE) the table calls functions on a `MobileServiceTable` object. 
These can be obtained by calling the `GetTable` on our `MobileServiceClient` object.

Lets add our `notHotDogTable` field to our `AzureManager` activity 
```Csharp
    private IMobileServiceTable<NotHotDogModel> notHotDogTable;
``` 

And then the following line at the end of our `private AzureManager()` function
```Csharp
    this.notHotDogTable = this.client.GetTable<NotHotDogModel>();
```

This grabs a reference to the data in our `NotHotDogModel` table in our backend and maps it to our client side model defined earlier.

We can then use this table to actually get data, get filtered data, get a NotHotDogModel by id, create new NotHotDogModel, edit NotHotDogModel and much more.

### 6.3.5 Grabbing NotHotDogModel data
To retrieve information about the table, we can invoke a `ToListAsync()` method call, this is asynchronous and allows us to do LINQ querys.

Lets create a `GetHotDogInformation` method in our `AzureManager.cs` file
```Csharp
public async Task<List<NotHotDogModel>> GetHotDogInformation() {
    return await this.notHotDogTable.ToListAsync();
}
``` 
#### Your AzureManager.cs Class should like like the code snippet below now
```Csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Tabs
{
	public class AzureManager
	{

		private static AzureManager instance;
		private MobileServiceClient client;
		private IMobileServiceTable<NotHotDogModel> notHotDogTable;

		private AzureManager()
		{
			this.client = new MobileServiceClient("https://nothotdoginformation.azurewebsites.net");
            this.notHotDogTable = this.client.GetTable<NotHotDogModel>();
		}

		public MobileServiceClient AzureClient
		{
			get { return client; }
		}

		public static AzureManager AzureManagerInstance
		{
			get
			{
				if (instance == null)
				{
					instance = new AzureManager();
				}

				return instance;
			}
		}

		public async Task<List<NotHotDogModel>> GetHotDogInformation()
		{
			return await this.notHotDogTable.ToListAsync();
		}
	}
}
```

Add the following to your `AzureTable.xaml` 

```xml
  <?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Tabs.AzureTable" Title="Information">
    <ContentPage.Padding>
        <OnPlatform x:TypeArguments="Thickness" iOS="0, 20, 0, 0" />
    </ContentPage.Padding>
    <ContentPage.Content>
        <StackLayout>
            <Button Text="See Photo Information" TextColor="White" BackgroundColor="Red" Clicked="Handle_ClickedAsync" />
            <ListView x:Name="HotDogList" HasUnevenRows="True">
                <ListView.ItemTemplate>
                    <DataTemplate>
                        <ViewCell>
                            <StackLayout Orientation="Horizontal">
                                <Label Text="{Binding Longitude, StringFormat='Longitude: {0:N}'}" HorizontalOptions="FillAndExpand" Margin="20,0,0,0" VerticalTextAlignment="Center" />
                                <Label Text="{Binding Latitude, StringFormat='Latitude: {0:N}'}" VerticalTextAlignment="Center" Margin="0,0,20,0" />
                            </StackLayout>
                        </ViewCell>
                    </DataTemplate>
                </ListView.ItemTemplate>
            </ListView>
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```
 - Here we added a template for the NotHotDogModel object values, showing the `Longitude`, and `Latitude` values by using `Binding` ie `"{Binding Longitude, StringFormat='Longitude: {0:N}'}"`. This is a very simple way to display all our values and can be futher extended to display it in a aesthetic manner.
This associates the value of the field of the NotHotDogModel object and displays it. The string formatting used is conventional C# syntax. 
- Since The Longitude and Latitude values are of type float the 0:N syntax will automatically round the information to two decimal places, unless specified otherwise. 
- The Padding on the ContentPage tag is set to 20 units on the top, but that’s only to avoid overlapping the status bar on the iPhone. That padding isn’t required on Android and Windows Phone. (Nor is it required when the page is navigated to through a NavigationPage as the pages in XamlSamples are, but let’s continue as if this were a standalone page in a single-page application.) Fortunately there is a way to embed some platform-specific markup in a XAML file using a class named OnPlatform<T>. This is a generic class that has three properties named iOS, Android, and WinPhone of type T. The OnPlatform<T> class also defines an implicit cast of itself to type T that returns the appropriate object depending on which platform it’s running on. It sounds complicated but the XAML syntax is actually quite straightforward. As seen in the code snippet below and above.

```xml
<ContentPage.Padding>
        <OnPlatform x:TypeArguments="Thickness" iOS="0, 20, 0, 0" />
</ContentPage.Padding>
```
Refer to [Essential XAML syntax](https://developer.xamarin.com/guides/xamarin-forms/xaml/xaml-basics/essential_xaml_syntax/) for more information about how you can add application specific code to other platforms as well.

Now to can call our `GetHotDogInformation` function, we can add the following method in our `AzureTables.xaml.cs` class
```Csharp
    async void Handle_ClickedAsync(object sender, System.EventArgs e)
    {
        List<NotHotDogModel> notHotDogInformation = await AzureManager.AzureManagerInstance.GetHotDogInformation();

        HotDogList.ItemsSource = notHotDogInformation;
    }
``` 

This will then set the source of the list view  `NotHotDogModel` to the list of NotHotDogModel information we got from our backend.

Your `AzureTables.xaml.cs` class should now look like this: 

```Csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace Tabs
{
    public partial class AzureTable : ContentPage
    {
       
		public AzureTable()
        {
            InitializeComponent();

		}

		async void Handle_ClickedAsync(object sender, System.EventArgs e)
		{
			List<NotHotDogModel> notHotDogInformation = await AzureManager.AzureManagerInstance.GetHotDogInformation();

			HotDogList.ItemsSource = notHotDogInformation;
		}

    }
}
```

[More Info on ListView](https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/) about customising the appearance of your list view

## Extra for experts 
### Reverse geocode a street address
#### How to convert latitude and longitude coordinates to a street address
##### This tutorial shows how to reverse geocode user supplied latitude and longitude coordinates into a street address by using the `Geocoder` class included in Xamarin.Forms.Maps.

### Overview
The Xamarin.Forms.Maps NuGet package is used to add maps to a Xamarin.Forms app, and uses the native map APIs on each platform. This NuGet package provides the Geocoder class that converts between string addresses and latitude and longitudes.

`By using the native map APIs on each platform Xamarin.Forms.Maps provides a fast, familiar maps experience for users, but means that some configuration steps are required to adhere to each platforms specific API requirements. For information about these configuration steps see Working with Maps in Xamarin.Forms.`

In the code building the user interface for a page, import the Xamarin.Forms.Maps namespace and create an instance of the Geocoder class.

Then for user supplied latitude and longitude coordinates call the GetAddressesForPositionAsync method on the Geocoder instance in order to asynchronously get a list of addresses near the position.

Your AzureTable.xaml.cs file should look like this: 

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabs
{
    public partial class AzureTable : ContentPage
    {
		Geocoder geoCoder;
       
        public AzureTable()
        {
            InitializeComponent();
            geoCoder = new Geocoder();

		}

		async void Handle_ClickedAsync(object sender, System.EventArgs e)
		{
			List<NotHotDogModel> notHotDogInformation = await AzureManager.AzureManagerInstance.GetHotDogInformation();

			foreach (NotHotDogModel model in notHotDogInformation)
			{
				var position = new Position(model.Latitude, model.Longitude);
				var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
				foreach (var address in possibleAddresses)
                    model.City = address;
			}

			HotDogList.ItemsSource = notHotDogInformation;

		}

    }
}
```

Notice we set the address to the model property `City` so that it can be used as a binding in the xaml file. 

```csharp
model.City = address;
```

Too accomodate the extra street address we will update the `AzureTables.xaml` file to look like this:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Tabs.AzureTable" Title="Information">
    <ContentPage.Padding>
        <OnPlatform x:TypeArguments="Thickness" iOS="0, 20, 0, 0" />
    </ContentPage.Padding>
    <ContentPage.Content>
        <StackLayout>
            <Button Text="See Photo Information" TextColor="White" BackgroundColor="Red" Clicked="Handle_ClickedAsync" />
            <ListView x:Name="HotDogList" HasUnevenRows="true">
                <ListView.ItemTemplate>
                    <DataTemplate>
                        <ViewCell>
                            <Grid>
                                <Grid.RowDefinitions>
                                    <RowDefinition Height="Auto" />
                                </Grid.RowDefinitions>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="50*" />
                                    <ColumnDefinition Width="50*" />
                                </Grid.ColumnDefinitions>
                                <Label Grid.Column="0" Text="{Binding City}" Margin="20,0,0,0"/>
                                <Label Text="{Binding Happiness}" />
                                <StackLayout Grid.Column="1" Orientation="Vertical" Margin="0,0,20,0">
                                    <Label Text="{Binding Longitude, StringFormat='Longitude: {0:N}'}" HorizontalTextAlignment="End"/>
                                    <Label Text="{Binding Latitude, StringFormat='Latitude: {0:N}'}" HorizontalTextAlignment="End"/>
                                </StackLayout>
                            </Grid>
                        </ViewCell>
                    </DataTemplate>
                </ListView.ItemTemplate>
            </ListView>
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```
Notice we have added a grid view with two columns and setting the margins on either side to 20px to make it look nicer. Below is a representation of where the padding/Margin is applied to. 

```xml
Margin = "Left, Top, Right, Bottom"
```

The text in the second grid is right aligned due to the following line

```xml
HorizontalTextAlignment="End"
```

## `IMPORTANT`

## Maps Initialization

When adding maps to a Xamarin.Forms application, Xamarin.Forms.Maps is a a separate NuGet package that you should add to every project in the solution. On Android, this also has a dependency on GooglePlayServices (another NuGet) which is downloaded automatically when you add Xamarin.Forms.Maps.

After installing the NuGet package, some initialization code is required in each application project, after the `Xamarin.Forms.Forms.Init` method call. For iOS use the following code:

```csharp
Xamarin.FormsMaps.Init();
```

On Android you must pass the same parameters as Forms.Init:

```csharp
Xamarin.FormsMaps.Init(this, bundle);
```

For the Windows Runtime (WinRT) and the Universal Windows Platform (UWP) use the following code:

```csharp
Xamarin.FormsMaps.Init("INSERT_AUTHENTICATION_TOKEN_HERE");
```

Add this call in the following files for each platform:

iOS - AppDelegate.cs file, in the FinishedLaunching method.
Android - MainActivity.cs file, in the OnCreate method.
WinRT and UWP - MainPage.xaml.cs file, in the MainPage constructor.
Once the NuGet package has been added and the initialization method called inside each applcation, Xamarin.Forms.Maps APIs can be used in the common PCL or Shared Project code.

Source: [Maps Initialization](https://developer.xamarin.com/guides/xamarin-forms/user-interface/map/#Maps_Initialization)

#### `[Extra Information]` 
##### *Example doesn't apply in this tutorial, just information to let you know that this is an option available*
A LINQ query we may want to achieve is if we want to filter the data to only return high happiness songs. 
We could do this by the following line, this grabs the NotHotDogModel information if it has a happiness of 0.5 or higher
```Csharp
public async Task<List<NotHotDogModel>> GetHotDogInformation() {
    return await notHotDogTable.Where(notHotDogInformation => notHotDogInformation.Happiness > 0.5).ToListAsync();
}
``` 

If you are stuck have a look at the completed project for this section.

### Extra Learning Resources
* [Using App Service with Xamarin by Microsoft](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/)
* [Using App Service with Xamarin by Xamarin - Outdated but good to understand](https://blog.xamarin.com/getting-started-azure-mobile-apps-easy-tables/)
* [ListView in Xamarin](https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/)