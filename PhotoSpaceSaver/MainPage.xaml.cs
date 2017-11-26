using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace PhotoSpaceSaver
{
    public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                lbAuthToken.Text = "Acquiring Token";
                var ar = await App.PCA.AcquireTokenAsync(App.Scopes, App.PCA.Users.FirstOrDefault(), App.UIParent);
                lbAuthToken.Text = ar.AccessToken + ";" + ar.UniqueId;
            }
            catch (Exception ex)
            {
                var foo = ex.Message;
                // doesn't matter, we go in interactive more
            }
        }
    }
}
