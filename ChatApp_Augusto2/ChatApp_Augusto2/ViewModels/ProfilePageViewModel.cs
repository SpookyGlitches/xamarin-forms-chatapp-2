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
            _navigationService = navigationService;
        }
        public ICommand LogOutCommand => new Command(LogOut);
        private INavigationService _navigationService;
        public string Username => dataClass.loggedInUser.name;
        public string Email => dataClass.loggedInUser.email;
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
