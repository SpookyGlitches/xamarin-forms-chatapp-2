using ChatApp_Augusto2.DependencyServices;
using ChatApp_Augusto2.Models;
using ChatApp_Augusto2.Validations;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ChatApp_Augusto2.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        public SignInPageViewModel(INavigationService navigationService,IPageDialogService pageDialogService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            Email = new ValidatableString();
            Password = new ValidatableString();
            Email.IsValid = Password.IsValid = true;
            IsNotVisible = true;
        }
        private ValidatableString email;
        private ValidatableString password;
        private bool isNotVisible;
        public INavigationService _navigationService { get; private set; }
        public IPageDialogService _pageDialogService { get; private set; }
        public ValidatableString Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }
        public ValidatableString Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        public bool IsNotVisible
        {
            get { return isNotVisible; }
            set { SetProperty(ref isNotVisible, value); }
        }
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        public ICommand SignInCommand => new Command(SignIn);
        public ICommand TogglePasswordCommand => new Command(() => IsNotVisible = !IsNotVisible);
        public ICommand FocusedCommand => new Command(Focused);

        private void Focused(object obj)
        {
            var str = obj as string;
            switch(str)
            {
                case "Email": Email.IsValid = true;break;
                case "Password": Password.IsValid = true;break;
            }
        }
        async private void SignIn()
        {
            Email.Validate();
            Password.Validate();
            if (!(Email.IsValid && Password.IsValid))
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
                    await _navigationService.NavigateAsync("/TabbedPageMenu");
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", res.Response,"Okay.");
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
    }
}
