using ChatApp_Augusto2.DependencyServices;
using ChatApp_Augusto2.Helpers;
using ChatApp_Augusto2.Models;
using ChatApp_Augusto2.Validations;
using Plugin.CloudFirestore;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace ChatApp_Augusto2.ViewModels
{
    public class SignUpPageViewModel : ViewModelBase
    {
        DataClass dataClass = DataClass.GetInstance;

        public SignUpPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService):base(navigationService)
        {

            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

            username = new ValidatableString();
            password = new ValidatableString();
            confirmPassword = new ValidatableString();
            email = new ValidatableString();

            isNotVisible = true;
            username.IsValid = password.IsValid = confirmPassword.IsValid = email.IsValid = true;
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        private ValidatableString email;
        public ValidatableString Email
        {
            get { return email; }
            set { SetProperty(ref email, value);  }
        }
        private ValidatableString password;
        public ValidatableString Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        private ValidatableString confirmPassword;
        public ValidatableString ConfirmPassword
        {
            get { return confirmPassword; }
            set { SetProperty(ref confirmPassword, value); }
        }
        private ValidatableString username;
        public ValidatableString Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }
        public Command SignUpCommand => new Command(SignUpSave);
        public Command TogglePasswordCommand => new Command(() => IsNotVisible = !IsNotVisible);
        public Command FocusedCommand => new Command(Focused);

        private bool isNotVisible;
        public bool IsNotVisible
        {
            get { return isNotVisible; }
            set { SetProperty(ref isNotVisible, value); }
        }

        public INavigationService _navigationService { get; private set; }
        public IPageDialogService _pageDialogService { get; private set; }
        private void Focused(object obj)
        {
            var str = obj as string;
            switch (str) {
                case "Username": Username.IsValid = true; break;
                case "Password": Password.IsValid = true; break;
                case "ConfirmPassword": ConfirmPassword.IsValid = true; break;
                case "Email": Email.IsValid = true; break;
            }
        }
        private bool RequiredValidate()
        {
            Username.Validate();
            Password.Validate();
            Email.Validate();
            ConfirmPassword.Validate();
            return
                Username.IsValid && Password.IsValid
                && Email.IsValid && ConfirmPassword.IsValid; 
        }
        private async void SignUpSave(object obj)
        {
            if (!RequiredValidate())
            {
                await _pageDialogService.DisplayAlertAsync("Error", "Missing field/s.", "Okay");
            }
            else
            {
                if (!Password.Value.Equals(ConfirmPassword.Value))
                {
                    await _pageDialogService.DisplayAlertAsync("Error", "Passwords dont match.", "Okay");
                    return;
                }
                IsBusy = true;
                FirebaseAuthResponseModel res = new FirebaseAuthResponseModel() { };
                res = await Xamarin.Forms.DependencyService.Get<IFirebaseAuth>().SignUpWithEmailPassword(Username.Value, Email.Value, Password.Value);
                if(res.Status != true)
                {
                    await _pageDialogService.DisplayAlertAsync("Error", res.Response, "Okay");
                }
                else
                {
                    try
                    {
                        await CrossCloudFirestore.Current
                             .Instance
                             .GetCollection("users")
                             .GetDocument(dataClass.loggedInUser.uid)
                             .SetDataAsync(dataClass.loggedInUser);
                        await _pageDialogService.DisplayAlertAsync("Success", "Sign up successful. Verification email sent.", "Okay");
                        await _navigationService.GoBackAsync();
                    }
                    catch(Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Okay");
                    }
                }
                IsBusy = false;
            }
        }

    }
}
