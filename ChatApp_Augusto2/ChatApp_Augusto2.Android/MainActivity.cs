using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase;
using Prism;
using Prism.Ioc;
using Xamarin.Forms;
namespace ChatApp_Augusto2.Droid
{
    [Activity(Label = "ChatApp_Augusto2", Theme = "@style/MainTheme", Icon = "@mipmap/ic_launcher", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            FirebaseApp.InitializeApp(this);
            base.OnCreate(savedInstanceState);
            var density = Resources.DisplayMetrics.Density;
            App.screenWidth = Resources.DisplayMetrics.WidthPixels / density;
            App.screenHeight = Resources.DisplayMetrics.HeightPixels / density;

            if (Device.Idiom == TargetIdiom.Phone)
                App.screenHeight = (16 * App.screenWidth) / 9;

            if (Device.Idiom == TargetIdiom.Tablet)
                App.screenWidth = (9 * App.screenHeight) / 16;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
            LoadApplication(new App(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}

