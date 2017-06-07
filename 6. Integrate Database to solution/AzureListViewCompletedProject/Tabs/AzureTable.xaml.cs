using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace Tabs
{
    public partial class AzureTable : ContentPage
    {
       
		MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

		public AzureTable()
        {
            InitializeComponent();

		}

		async void Handle_ClickedAsync(object sender, System.EventArgs e)
		{
			List<NotHotDogModel> timelines = await AzureManager.AzureManagerInstance.GetHotDogInformation();

			HotDogList.ItemsSource = timelines;
		}

    }
}
