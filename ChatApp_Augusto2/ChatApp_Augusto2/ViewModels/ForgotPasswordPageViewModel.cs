using ChatApp_Augusto2.DependencyServices;
using ChatApp_Augusto2.Models;
using ChatApp_Augusto2.Validations;
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
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            Email = new ValidatableString();
            Email.IsValid = true;
        }

        private ValidatableString email;
        public ValidatableString Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }
        public ICommand ForgotPasswordCommand => new Command(ForgotPassword);
        public INavigationService _navigationService { get; private set; }
        public IPageDialogService _pageDialogService { get; private set; }
        public ICommand FocusedCommand => new Command(() => { Email.IsValid = true; });
        async private void ForgotPassword()
        {
            Email.Validate();
            if (Email.IsValid)
            {
                FirebaseAuthResponseModel res = new FirebaseAuthResponseModel() { };
                res = await Xamarin.Forms.DependencyService.Get<IFirebaseAuth>().ResetPassword(Email.Value);

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
