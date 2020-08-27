using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ChatApp_Augusto2.CustomRenderers;
using Android.Graphics.Drawables;
using Android.Content;

[assembly: ExportRenderer(typeof(NoUEntry), typeof(ChatApp_Augusto2.Droid.NoUEntryRenderer))]
namespace ChatApp_Augusto2.Droid
{
    class NoUEntryRenderer : EntryRenderer
    {
        public NoUEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                Control.SetBackground(gd);
            }

        }
    }
}
