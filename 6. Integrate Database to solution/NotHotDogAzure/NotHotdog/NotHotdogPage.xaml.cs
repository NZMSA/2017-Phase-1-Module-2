using System;
using System.Diagnostics;
using Plugin.Media;
using Xamarin.Forms;

namespace NotHotdog
{
    public partial class NotHotdogPage : ContentPage
    {
        public NotHotdogPage()
        {
            InitializeComponent();
            loadCamera();
        }

        private async void loadCamera() 
        {
            await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
				});
                return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{
				PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
				Directory = "Sample",
                Name = String.Format("{0}.jpg", DateTime.Now.Date.ToString())
			});

			if (file == null)
				return;

            await Navigation.PushAsync(new ResultPage(file));
        }
    }
}
