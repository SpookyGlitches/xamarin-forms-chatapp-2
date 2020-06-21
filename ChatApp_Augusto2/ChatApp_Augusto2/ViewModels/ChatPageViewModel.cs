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
            ContactList = new ObservableCollection<ContactModel>();
            _pageDialogService = pageDialogService;
            SearchCommand = new Command(Search);
            _navigationService = navigationService;
            CrossCloudFirestore.Current
                .Instance
                .GetCollection("contacts")
                .WhereArrayContains("contactID", dataClass.loggedInUser.uid)
                .AddSnapshotListener((snapshot, error) =>
                {
                    IsBusy = true;
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
                                        ContactList.Remove(item);
                                    }
                                    break;

                            }
                        }
                    }
                    IsBusy = false;
                });
        }
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        private async void Search()
        {
            var param = new Prism.Navigation.NavigationParameters();
            param.Add("search", Text);
            await _navigationService.NavigateAsync("SearchPage",param,true,true) ;
        }

        public ICommand SearchCommand { get; set; }
        public INavigationService _navigationService { get; private set; }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                RaisePropertyChanged();
            }
        }
        private ContactModel selectedItem;
        public ContactModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if(value != null)
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
            set
            {
                contactList = value;
                RaisePropertyChanged();
            }
        }
        public IPageDialogService _pageDialogService { get; private set; }
        private async void NavigateToMessages()
        {

            var param = new Prism.Navigation.NavigationParameters();
            param.Add("obj", SelectedItem);
            await _navigationService.NavigateAsync("MessagesPage",param, true, true);
            SelectedItem = null;
        }
        private void GetResults()
        {
            //CrossCloudFirestore.Current
            //    .Instance
            //    .GetCollection("contacts")
            //    .WhereArrayContains("contactID", dataClass.loggedInUser.uid)
            //    .AddSnapshotListener((snapshot, error) =>
            //    {
            //        IsBusy = true;
            //        if(snapshot != null)
            //        {
            //            foreach(var documentChange in snapshot.DocumentChanges)
            //            {
            //                var json = JsonConvert.SerializeObject(documentChange.Document.Data);
            //                var obj = JsonConvert.DeserializeObject<ContactModel>(json);
            //                switch (documentChange.Type) 
            //                {
            //                    case DocumentChangeType.Added:
            //                        ContactList.Add(obj);
            //                        break;
            //                    case DocumentChangeType.Modified:
            //                        if(ContactList.Where(c => c.id == obj.id).Any())
            //                        {
            //                            var item = ContactList.Where(c => c.id == obj.id).FirstOrDefault();
            //                            item = obj;
            //                        }
            //                        break;
            //                    case DocumentChangeType.Removed:
            //                        if(ContactList.Where(c => c.id == obj.id).Any())
            //                        {
            //                            var item = ContactList.Where(c => c.id == obj.id).FirstOrDefault();
            //                        }
            //                        break;
                            
            //                }
            //            }
            //        }
            //        Count = ContactList.Count;
            //        IsBusy = false;
            //    });


        }
    }
}
