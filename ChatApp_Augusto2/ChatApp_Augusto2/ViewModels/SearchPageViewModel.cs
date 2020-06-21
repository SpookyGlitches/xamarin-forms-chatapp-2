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

namespace ChatApp_Augusto2.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        DataClass dataClass = DataClass.GetInstance;
        public SearchPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            Users = new ObservableCollection<UserModel>();
        }
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                SetProperty(ref email, value);
            }
        }
        private UserModel selectedItem;
        public UserModel SelectedItem
        {
            get { return selectedItem; }
            set 
            { 
                if(value != null)
                {
                    SetProperty(ref selectedItem, value);
                    AddDialog();
                }
            }
        }

        private async void AddDialog()
        {
            bool choice = await _pageDialogService.DisplayAlertAsync("Add contact", "Would you like to add " + selectedItem.name, "Yes", "No");
            if (choice)
            {
                if (dataClass.loggedInUser.uid.Equals(SelectedItem.uid))
                {
                    await _pageDialogService.DisplayAlertAsync("Hey!", "That is you. You can't add yourself dummy!", "Sorry");
                }
                else
                {
                    var query = await CrossCloudFirestore.Current
                                        .Instance
                                        .GetCollection("users")
                                        .GetDocument(dataClass.loggedInUser.uid)
                                        .GetDocumentAsync();
                    //
                    var loggedInUserContacts = query.ToObject<UserModel>().contacts;
                    bool exists = false;
                    if(loggedInUserContacts != null)
                    {
                        string y =  loggedInUserContacts.FirstOrDefault(x => x.Equals(SelectedItem.uid));
                        dataClass.loggedInUser.contacts = loggedInUserContacts;
                        exists = !(String.IsNullOrEmpty(y));
                    }
                    if (!exists)
                    {
                        IsBusy = true;
                        Guid guid = Guid.NewGuid();
                        ContactModel contact = new ContactModel()
                        {
                            id = Guid.NewGuid().ToString(),
                            contactID = new string[] { dataClass.loggedInUser.uid, SelectedItem.uid },
                            contactEmail = new string[] { dataClass.loggedInUser.email, SelectedItem.email },
                            contactName = new string[] { dataClass.loggedInUser.name, SelectedItem.name },
                            created_at = DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss.fffffffK"),
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
                                .UpdateDataAsync(new { contacts = loggedInUserContacts } );
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
                    else
                    {
                        await _pageDialogService.DisplayAlertAsync("Error", "You already both have a connection", "Okay");
                    }


                }
            }
        }

        private ObservableCollection<UserModel> users;
        public ObservableCollection<UserModel> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged();
            }
        }

        public IPageDialogService _pageDialogService { get; private set; }

        public override void Initialize(INavigationParameters param)
        {
            Email = param.GetValue<string>("search");
            GetResults();
        }
        private async void GetResults()
        {
            var documents = await CrossCloudFirestore.Current
                                .Instance
                                .GetCollection("users")
                                .WhereEqualsTo("email", Email)
                                .GetDocumentsAsync();
            foreach (var documentChange in documents.DocumentChanges)
            {
                var json = JsonConvert.SerializeObject(documentChange.Document.Data);
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                Users.Add(user);
            }
        }
    }
}
