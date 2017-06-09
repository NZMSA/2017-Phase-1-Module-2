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

        public async Task PostHotDogInformation(NotHotDogModel notHotDogModel)
		{
			await this.notHotDogTable.InsertAsync(notHotDogModel);
		}
	}
}
