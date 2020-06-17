using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace ChatApp_Augusto2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPageMenu : Xamarin.Forms.TabbedPage
    {
        public TabbedPageMenu()
        {
            InitializeComponent();
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            BarBackgroundColor = Color.FromHex("#1976D2");
            SelectedTabColor = Color.White;
            UnselectedTabColor = Color.LightGray;
        }
    }
}