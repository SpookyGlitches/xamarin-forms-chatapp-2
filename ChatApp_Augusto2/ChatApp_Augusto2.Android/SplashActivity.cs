using Android.App;
using Android.Support.V7.App;
using ChatApp_Augusto2.Droid;

[Activity(Label = "ChatApp_Augusto2", Icon = "@mipmap/ic_launcher", Theme = "@style/splashscreen", MainLauncher = true, NoHistory = true)]
public class SplashActivity : AppCompatActivity
{
    protected override void OnResume()
    {
        base.OnResume();
        StartActivity(typeof(MainActivity));
    }
}