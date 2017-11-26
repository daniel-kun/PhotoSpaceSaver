using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PhotoSpaceSaver
{
	public partial class App : Application
	{
		public App ()
		{
            const string ClientID = "0001ed0f-62df-47f6-aa50-e61e028e4d58";
            Logger.Level = Logger.LogLevel.Verbose;
            Logger.PiiLoggingEnabled = true;
            Logger.LogCallback = (level, message, containsPii) =>
            {
                //System.Console.WriteLine(message);
            };
            PCA = new PublicClientApplication(ClientID)
            {
                RedirectUri = "msal0001ed0f-62df-47f6-aa50-e61e028e4d58://auth"
            };

            InitializeComponent();

			MainPage = new NavigationPage(new PhotoSpaceSaver.MainPage());
        }

        public static UIParent UIParent = null;
        public static PublicClientApplication PCA = null;
        public static string[] Scopes = new string[]
        {
                "Files.Read",
                "Files.Read.All",
                "Files.ReadWrite",
                "Files.ReadWrite.All",
                "User.Read"
        };

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
