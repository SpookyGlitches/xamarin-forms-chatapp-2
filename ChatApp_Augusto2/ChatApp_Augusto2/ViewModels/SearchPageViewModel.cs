using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections.ObjectModel;
using ChatApp_Augusto2.Models;
using Plugin.CloudFirestore;
using Newtonsoft.Json;
using System.ComponentModel;
using Prism.Services;
using ChatApp_Augusto2.Helpers;
using Xamarin.Forms;
using ImTools;
using System.Linq;
using System.Windows.Input;

namespace ChatApp_Augusto2.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        DataClass dataClass = DataClass.GetInstance;
        public SearchPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _navigationService = navigationService;
            Users = new ObservableCollection<UserModel>();
        }
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        private UserModel selectedItem;
        public UserModel SelectedItem
        {
            get { return selectedItem; }
            set 
            {
                selectedItem = value;
                AddDialog();
                RaisePropertyChanged();
            }
        }
        public ICommand BackCommand => new Command(async () => await _navigationService.GoBackAsync(useModalNavigation: true));
        private ObservableCollection<UserModel> users;
        public ObservableCollection<UserModel> Users
        {
            get { return users; }
            set { SetProperty(ref users, value); }
        }
        public IPageDialogService _pageDialogService { get; private set; }
        public INavigationService _navigationService { get; private set; }
        public override void Initialize(INavigationParameters param)
        {
            GetResults(param.GetValue<string>("search"));
        }
        private async void AddDialog()
        {
            if (SelectedItem == null) { return; }
            bool choice = await _pageDialogService.DisplayAlertAsync("Add contact", "Would you like to add " + SelectedItem.name, "Yes", "No");
            if (choice)
            {
                if (dataClass.loggedInUser.uid.Equals(SelectedItem.uid))
                {
                    await _pageDialogService.DisplayAlertAsync("Error", "You are not allowed to add yourself.", "Okay");
                }
                else
                {
                    bool exists = false;
                    if (dataClass.loggedInUser.contacts != null && dataClass.loggedInUser.contacts.Count != 0)
                    {
                        //Checks if it is in his/her contacts.
                        string y = dataClass.loggedInUser.contacts.FirstOrDefault(x => x.Equals(SelectedItem.uid));
                        exists = !(String.IsNullOrEmpty(y));
                    }
                    if (exists)
                    {
                        await _pageDialogService.DisplayAlertAsync("Error", "You already both have a connection.", "Okay");
                    }
                    else
                    {
                        IsBusy = true;
                        Guid guid = Guid.NewGuid();
                        ContactModel contact = new ContactModel()
                        {
                            id = Guid.NewGuid().ToString(),
                            contactID = new string[] { dataClass.loggedInUser.uid, SelectedItem.uid },
                            contactEmail = new string[] { dataClass.loggedInUser.email, SelectedItem.email },
                            contactName = new string[] { dataClass.loggedInUser.name, SelectedItem.name },
                            created_at = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        };
                        await CrossCloudFirestore.Current
                                .Instance
                                .GetCollection("contacts")
                                .GetDocument(contact.id)
                                .SetDataAsync(contact);
                        if (dataClass.loggedInUser.contacts == null)
                            dataClass.loggedInUser.contacts = new List<string>();
                        dataClass.loggedInUser.contacts.Add(SelectedItem.uid);
                        await CrossCloudFirestore.Current
                                .Instance
                                .GetCollection("users")
                                .GetDocument(dataClass.loggedInUser.uid)
                                .UpdateDataAsync(new { contacts = dataClass.loggedInUser.contacts });
                        if (SelectedItem.contacts == null)
                            selectedItem.contacts = new List<string>();
                        SelectedItem.contacts.Add(dataClass.loggedInUser.uid);
                        await CrossCloudFirestore.Current
                                .Instance
                                .GetCollection("users")
                                .GetDocument(SelectedItem.uid)
                                .UpdateDataAsync(new { contacts = SelectedItem.contacts });
                        IsBusy = false;
                        await _pageDialogService.DisplayAlertAsync("Success", "Contact Added", "Okay");
                    }
                }
            }
            SelectedItem = null;
        }
        private async void GetResults(string email)
        {
            var documents = await CrossCloudFirestore.Current
                                .Instance
                                .GetCollection("users")
                                .WhereEqualsTo("email", email)
                                .GetDocumentsAsync();
            int x = 0;
            foreach (var documentChange in documents.DocumentChanges)
            {
                var json = JsonConvert.SerializeObject(documentChange.Document.Data);
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                Users.Add(user);
                x++; 
            }
            if (x == 0)
            {
                await _pageDialogService.DisplayAlertAsync("", "User not found.", "Okay");
                await _navigationService.GoBackAsync(useModalNavigation:true);
            }
        }
    }
}
