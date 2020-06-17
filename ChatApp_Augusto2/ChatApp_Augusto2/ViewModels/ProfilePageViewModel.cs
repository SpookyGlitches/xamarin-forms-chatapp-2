using ChatApp_Augusto2.DependencyServices;
using ChatApp_Augusto2.Helpers;
using ChatApp_Augusto2.Models;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ChatApp_Augusto2.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        DataClass dataClass = DataClass.GetInstance;
        public ProfilePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Username = dataClass.loggedInUser.name;
            Email = dataClass.loggedInUser.email;
            LogOutCommand = new Command(LogOut);
            _navigationService = navigationService;
        }
        public ICommand LogOutCommand { get; set; }

        private INavigationService _navigationService;

        public string Username { get; set; }
        public string Email { get; set; }
        async public void LogOut()
        {
            FirebaseAuthResponseModel res = new FirebaseAuthResponseModel() { };
            res = Xamarin.Forms.DependencyService.Get<IFirebaseAuth>().SignOut();

            if (res.Status == true)
            {
                await _navigationService.NavigateAsync("/NavigationPage/SignInPage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", res.Response, "Okay");
            }
        }
    }
}
