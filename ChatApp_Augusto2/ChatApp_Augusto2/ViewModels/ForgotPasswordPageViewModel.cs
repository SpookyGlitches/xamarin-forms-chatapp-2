using ChatApp_Augusto2.DependencyServices;
using ChatApp_Augusto2.Models;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ChatApp_Augusto2.ViewModels
{
    public class ForgotPasswordPageViewModel : ViewModelBase
    {
        public ForgotPasswordPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            ForgotPasswordCommand = new Command(ForgotPassword);
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged();
            }
        }
        public ICommand ForgotPasswordCommand { get; set; }
        public INavigationService _navigationService { get; private set; }
        public IPageDialogService _pageDialogService { get; private set; }

        async private void ForgotPassword()
        {
            if (!String.IsNullOrWhiteSpace(Email))
            {
                FirebaseAuthResponseModel res = new FirebaseAuthResponseModel() { };
                res = await Xamarin.Forms.DependencyService.Get<IFirebaseAuth>().ResetPassword(Email);

                if (res.Status == true)
                {
                    await _pageDialogService.DisplayAlertAsync("Success", res.Response, "Okay");
                    await _navigationService.GoBackAsync();
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", res.Response, "Okay");
                }
            }
            else
            {
                await _pageDialogService.DisplayAlertAsync("Error", "Please fill in your email address.", "Okay");
            }
        }
    }
}
