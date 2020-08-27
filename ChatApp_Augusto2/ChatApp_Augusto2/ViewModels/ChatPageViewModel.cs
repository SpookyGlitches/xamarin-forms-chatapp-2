using ChatApp_Augusto2.Helpers;
using ChatApp_Augusto2.Models;
using ImTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.CloudFirestore;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
namespace ChatApp_Augusto2.ViewModels
{
    public class ChatPageViewModel : ViewModelBase
    {
        DataClass dataClass = DataClass.GetInstance;
        public ChatPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _navigationService = navigationService;
            ContactList = new ObservableCollection<ContactModel>();
        }
        public override void Initialize(INavigationParameters parameters)
        {
            CrossCloudFirestore.Current
                .Instance
                .GetCollection("contacts")
                .WhereArrayContains("contactID", dataClass.loggedInUser.uid)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (snapshot != null)
                    {
                        foreach (var documentChange in snapshot.DocumentChanges)
                        {
                            var json = JsonConvert.SerializeObject(documentChange.Document.Data);
                            var obj = JsonConvert.DeserializeObject<ContactModel>(json);
                            switch (documentChange.Type)
                            {
                                case DocumentChangeType.Added:
                                    ContactList.Add(obj);
                                    break;
                                case DocumentChangeType.Modified:
                                    if (ContactList.Where(c => c.id == obj.id).Any())
                                    {
                                        var item = ContactList.Where(c => c.id == obj.id).FirstOrDefault();
                                        item = obj;
                                    }
                                    break;
                                case DocumentChangeType.Removed:
                                    if (ContactList.Where(c => c.id == obj.id).Any())
                                    {
                                        var item = ContactList.Where(c => c.id == obj.id).FirstOrDefault();
                                        string idOfRemoved = item.contactID[0] == dataClass.loggedInUser.uid ? item.contactID[1] : item.contactID[0];
                                        bool test = dataClass.loggedInUser.contacts.Remove(idOfRemoved);
                                        ContactList.Remove(item);
                                    }
                                    break;

                            }
                        }
                        ContactListIsEmpty = ContactList.Count == 0;
                    }
                });
        }
        private bool contactListIsEmpty;
        public bool ContactListIsEmpty
        {
            get { return contactListIsEmpty; }
            set { SetProperty(ref contactListIsEmpty, value); }
        }
        private string text;
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }
        private ContactModel selectedItem;
        public ContactModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (value != null)
                {
                    NavigateToMessages();
                }
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<ContactModel> contactList;
        public ObservableCollection<ContactModel> ContactList
        {
            get { return contactList; }
            set { SetProperty(ref contactList, value); }
        }
        public ICommand SearchCommand => new Command(Search);
        public INavigationService _navigationService { get; private set; }
        public IPageDialogService _pageDialogService { get; private set; }
        private async void Search()
        {
            var param = new Prism.Navigation.NavigationParameters();
            param.Add("search", Text);
            await _navigationService.NavigateAsync("SearchPage", param, useModalNavigation:true);
            Text = String.Empty;
        }
        private async void NavigateToMessages()
        {
            var param = new Prism.Navigation.NavigationParameters();
            param.Add("obj", SelectedItem);
            await _navigationService.NavigateAsync("MessagesPage",param, useModalNavigation:true);
            SelectedItem = null;
        }
    }
}
