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
            Username = new ValidatableString();
            Password = new ValidatableString();
            Email = new ValidatableString();
            ConfirmPassword = new ValidatableString();

            SignUpCommand = new Command(SignUpSave);
            SignInCommand = new Command(SignIn);

            TogglePasswordCommand = new Command(() => IsNotVisible = !IsNotVisible);

            isNotVisible = true;

            Username.IsValid = Password.IsValid = ConfirmPassword.IsValid = Email.IsValid = true;

            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set 
            { 
                SetProperty(ref isBusy, value);
                RaisePropertyChanged();
            }
        }
        private ValidatableString username;
        public ValidatableString Username
        {
            get { return username; }
            set 
            {
                SetProperty(ref username, value);
                RaisePropertyChanged();
            }
        }

        private ValidatableString password;
        public ValidatableString Password
        {
            get { return password; }
            set 
            { 
                SetProperty(ref password, value);
                RaisePropertyChanged();
            }
        }
        private ValidatableString confirmPassword;
        public ValidatableString ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                SetProperty(ref confirmPassword, value);
                RaisePropertyChanged();
            }
        }

        public Command SignUpCommand { get; private set; }
        public Command SignInCommand { get; private set; }
        public Command TogglePasswordCommand { get; private set; }

        private ValidatableString email;
        public ValidatableString Email
        {
            get { return email; }
            set 
            {
                SetProperty(ref email, value);
                //RaisePropertyChanged();
            }
        }
        private bool isNotVisible;
        public bool IsNotVisible
        {
            get { return isNotVisible; }
            set 
            {
                SetProperty(ref isNotVisible, value);
                //RaisePropertyChanged();
            }
        }

        public INavigationService _navigationService { get; private set; }
        public IPageDialogService _pageDialogService { get; private set; }

        private async void SignIn(object obj)
        {
            await _navigationService.GoBackAsync();
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
                // missing fields
                await _pageDialogService.DisplayAlertAsync("Error", "Missing field/s.", "Okay");
            }
            else
            {
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
