using Prism;
using Prism.Ioc;
using ChatApp_Augusto2.ViewModels;
using ChatApp_Augusto2.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ChatApp_Augusto2.Helpers;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ChatApp_Augusto2
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public static float screenWidth { get; set; }
        public static float screenHeight { get; set; }
        public static float appScale { get; set; }
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            DataClass dataClass = DataClass.GetInstance;
            if (dataClass.isSignedIn)
            {
                await NavigationService.NavigateAsync("/TabbedPageMenu");
            }
            else
            {
                await NavigationService.NavigateAsync("NavigationPage/SignInPage");
            }   
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<SignInPage>();
            containerRegistry.RegisterForNavigation<SignUpPage>();
            containerRegistry.RegisterForNavigation<TabbedPageMenu>();
            containerRegistry.RegisterForNavigation<SearchPage>();
            containerRegistry.RegisterForNavigation<ForgotPasswordPage>();
            containerRegistry.RegisterForNavigation<MessagesPage>();
        }
    }
}
