using ChatApp_Augusto2.DependencyServices;
using ChatApp_Augusto2.Models;
using ChatApp_Augusto2.Validations;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp_Augusto2.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        public SignInPageViewModel(INavigationService navigationService,IPageDialogService pageDialogService)
            : base(navigationService)
        {

            Email = new ValidatableString();
            Password = new ValidatableString();

            Email.IsValid = Password.IsValid = true;

            IsNotVisible = true;

            TogglePasswordCommand = new DelegateCommand(ToggleIsVisible);
            SignInCommand = new DelegateCommand(SignIn);
            SignUpCommand = new DelegateCommand(SignUp);
            ForgotPasswordCommand = new DelegateCommand(ForgotPassword);

            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

        }

        private async void ForgotPassword()
        {
            await _navigationService.NavigateAsync("ForgotPasswordPage");
        }

        private INavigationService _navigationService { get; set; }
        public IPageDialogService _pageDialogService { get; private set; }

        private ValidatableString email;
        private ValidatableString password;
        private bool isNotVisible;


        public ValidatableString Email
        {
            get { return email; }
            set
            {
                SetProperty(ref email, value);
                RaisePropertyChanged();
            }
        }
        public ValidatableString Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value);
                RaisePropertyChanged();
            }
        }
        public bool IsNotVisible
        {
            get { return isNotVisible; }
            set
            {
                SetProperty(ref isNotVisible, value);
                RaisePropertyChanged();
            }
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
        public DelegateCommand SignInCommand { get; private set; }
        public DelegateCommand TogglePasswordCommand { get; private set; }
        public DelegateCommand SignUpCommand { get; private set; }
        public DelegateCommand ForgotPasswordCommand { get; private set; }

        private bool RequiredValidate()
        {
            Email.Validate();
            Password.Validate();
            return Email.IsValid && Password.IsValid;
        }
        void ToggleIsVisible()
        {
            IsNotVisible = !IsNotVisible;
        }
        async public void SignIn()
        {
            if (!RequiredValidate()) //if not all fields are filled
            {
                await _pageDialogService.DisplayAlertAsync("Error", "Missing field/s.","Okay");
            }
            else
            {
                IsBusy = true;
                FirebaseAuthResponseModel res = new FirebaseAuthResponseModel() { };
                res = await Xamarin.Forms.DependencyService.Get<IFirebaseAuth>().LoginWithEmailPassword(Email.Value, Password.Value);
                ResetFields();
                IsBusy = false;
                if (res.Status == true)
                {
                    await _pageDialogService.DisplayAlertAsync("Success", "You can fucking login bitch", "Fuck you too");
                    await _navigationService.NavigateAsync("TabbedPageMenu");
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", "Nice try bitch","Okay bitch.");
                    //await Application.Current.MainPage.DisplayAlert("Error", res.Response, "Okay");
                }
            }
        }
        private void ResetFields()
        {
            Email.Value = string.Empty;
            Password.Value = string.Empty;
            Email.IsValid = true;
            Password.IsValid = true;
        }
        private async void SignUp()
        {
            
            //await _navigationService.NavigateAsync("SignUpPage");
            await NavigationService.NavigateAsync("SignUpPage");
            ResetFields();
        }
    }
}
