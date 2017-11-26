
using Android.App;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using PhotoSpaceSaver;
using PhotoSpaceSaver.Droid;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace PhotoSpaceSaver.Droid
{
    class MainPageRenderer : PageRenderer
    {
        MainPage page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as MainPage;
            var activity = this.Context as Activity;
        }

    }
}
