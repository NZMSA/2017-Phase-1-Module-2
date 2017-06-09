# Introduction to Xamarin UI Creation

## `NOT A STEP BY STEP GUIDE`

The contents of this file in particular are just for your knowledge.

You will learn the key elements we use to create User Interfaces in Xamarin.Forms.

# Making a new Xamarin Forms Project

File > New Project > Visual C# > Blank Xaml App (Xamarin.Forms Portable) > Name it App1.

# 1. Layouts
Can usually be added using Xaml or programatically (C#).
## A) Stack Layouts

They are used for a basic horizontal or vertical layout.

Replace the code in Main.xaml with the following code:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:App1"
             x:Class="App1.MainPage"
             Title="StackLayout Demo">
  <ContentPage.Content>
    <StackLayout Spacing="0" x:Name="layout">
      <Button Text="StackLayout" VerticalOptions="Start"
        HorizontalOptions="FillAndExpand" />
      <BoxView Color="#FFF724" VerticalOptions="FillAndExpand"
        HorizontalOptions="FillAndExpand" />
      <BoxView Color="#FF001F" VerticalOptions="FillAndExpand"
  HorizontalOptions="FillAndExpand" />
      <BoxView HeightRequest="75" Color="#1491CC" VerticalOptions="End"
  HorizontalOptions="FillAndExpand" />
    </StackLayout>
  </ContentPage.Content>
</ContentPage>

```

Try running the code for Android and Windows phone. You will notice that the button looks and behaves differently on both platforms.

### Size 


* CenterAndExpand – centers the view within the layout and expands to take up as much space as the layout will give it.
* EndAndExpand – places the view at the end of the layout (bottom or right-most boundary) and expands to take up as much space as the layout will give it.
* FillAndExpand – places the view so that it has no padding and takes up as much space as the layout will give it.
* StartAndExpand – places the view at the start of the layout and takes up as much space as the parent will give.


### Position


* Center – centers the view within the layout.
* End – places the view at the end of the layout (bottom or right-most boundary).
* Fill – places the view so that it has no padding.
* Start – places the view at the start of the layout.

Let's add:

```csharp
Clicked="OnButtonClicked"
```

inside the Xaml of the button.

and 

```csharp
async void OnButtonClicked(object sender, EventArgs args)
{
    Button button = (Button)sender;
    await DisplayAlert("Clicked!",
        "The button labeled '" + button.Text + "' has been clicked",
        "OK");
}
```

to MainPage.xaml.cs.

## B) Grids

Right-click App1 (Portable) > Add > New Item > Forms Page > Change name to Grid1.cs > Add

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace App1
{
    public class Grid1 : ContentPage
    {
        int count = 1;

        public Grid1()
        {
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 20
            };

            var grid = new Grid
            {
                RowSpacing = 50
            };

            grid.Children.Add(new Label { Text = "This" }, 0, 0); // Left, First element
            grid.Children.Add(new Label { Text = "text is" }, 1, 0); // Right, First element
            grid.Children.Add(new Label { Text = "in a" }, 0, 1); // Left, Second element
            grid.Children.Add(new Label { Text = "grid!" }, 1, 1); // Right, Second element

            var gridButton = new Button { Text = "So is this Button!\nClick me." };
            gridButton.Clicked += delegate
            {
                gridButton.Text = string.Format("Thanks! {0} clicks.", count++);
            };

            layout.Children.Add(grid);
            layout.Children.Add(gridButton);
            Content = layout;

        }
    }
}

```
To link this to the first page, change the code inside the OnButtonClicked method inside MainPage.xaml.cs to: 

```csharp
async void OnButtonClicked(object sender, EventArgs args)
{
    Button button = (Button)sender;
    await DisplayAlert("Clicked!",
        "The button labeled '" + button.Text + "' has been clicked",
        "OK");
    await Navigation.PushModalAsync(new Grid1());
}
```

# 2. Other Important Elements

## Labels

Display text.

## Buttons

React to touch input. Look and behaviour depends on the platform.

An alternative to the code above is:

```csharp 
x:Name="myButton"
```

```csharp
myButton.Clicked += async (s, e) =>
{
    Button button = (Button)s;
    await DisplayAlert("Clicked!",
        "The button labeled '" + button.Text + "' has been clicked",
        "OK");
    await Navigation.PushModalAsync(new Grid1());
};
}
```
## Content View

An element that contains a single child element.

## Content Page

Contains only 1 view.


# Advanced Features

## Custom Renders

Allows you to render an element without using the native controls of the target platform. You do this
by implementing your own custom Renderer classes instead of using a Renderer class that creates a native control. 

## List Views

Somewhat advanced because of data binding. Using MVVM pattern. 

But can be made simpler using an ItemsSource.

```csharp
var listView = new ListView();
listView.ItemsSource = new string[]{
  "mono",
  "monodroid",
  "monotouch",
  "monorail",
  "monodevelop",
  "monotone",
  "monopoly",
  "monomodal",
  "mononucleosis"
};

//monochrome will not appear in the list because it was added
//after the list was populated.
listView.ItemsSource.Add("monochrome");
```

If you want this ItemSource to keep update when the array is changed, you need to use an Observable Collection:

```csharp
ObservableCollection<Employees> employeeList = new ObservableCollection<Employees>();
listView.ItemsSource = employeeList;

//Mr. Mono will be added to the ListView because it uses an ObservableCollection
employeeList.Add(new Employee(){ DisplayName="Mr. Mono"});
```
