using ChatApp_Augusto2.Models;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using ImTools;
using ChatApp_Augusto2.Helpers;
using System.Collections.ObjectModel;
using ChatApp_Augusto2.Views;

namespace ChatApp_Augusto2.ViewModels
{
    public class MessagesPageViewModel : ViewModelBase
    {
        DataClass dataClass = DataClass.GetInstance;
        public MessagesPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            PopModalCommand = new Command(PopModalAsync);
            _navigationService = navigationService;
            SendCommand = new Command(Send);
            ConversationsList = new ObservableCollection<ConversationModel>();
        }

        private ObservableCollection<ConversationModel> conversationsList;
        public ObservableCollection<ConversationModel> ConversationsList
        {
            get { return conversationsList; }
            set { SetProperty(ref conversationsList, value); }
        }
        public ICommand SendCommand { get; set; }
        private ContactModel conversationPartner;
        public ContactModel ConversationPartner
        {
            get { return conversationPartner; }
            set { SetProperty(ref conversationPartner, value); }
        }
        private string message;
        public string Message
        {
            get { return message; }
            set {
                SetProperty(ref message, value);
            }
        }
        private void GetConversations()
        {
            //CrossCloudFirestore.Current
            //    .Instance.GetCollection("contacts")
            //    .GetDocument(ConversationPartner.id)
            //    .GetCollection("conversations")
            //    .OrderBy("created_at", false)
            //    .AddSnapshotListener((snapshot, error) =>
            //    {
            //        if (snapshot != null)
            //        {
            //            foreach (var documentChange in snapshot.DocumentChanges)
            //            {   
            //                var json = JsonConvert.SerializeObject(documentChange.Document.Data);
            //                var obj = JsonConvert.DeserializeObject<ConversationModel>(json);
            //                switch (documentChange.Type)
            //                {
            //                    case DocumentChangeType.Added:
            //                        ConversationsList.Add(obj);
            //                        break;
            //                    case DocumentChangeType.Modified:
            //                        if (ConversationsList.Where(c => c.id == obj.id).Any())
            //                        {
            //                            var item = ConversationsList.Where(c => c.id == obj.id).FirstOrDefault();
            //                        }
            //                        break;
            //                    case DocumentChangeType.Removed:
            //                        if (ConversationsList.Where(c => c.id == obj.id).Any())
            //                        {
            //                            var item = ConversationsList.Where(c => c.id == obj.id).FirstOrDefault();
            //                            ConversationsList.Remove(item);
            //                        }
            //                        break;
            //                }
            //                //missing
            //            }
            //        }
            //    });
        }

        private async void PopModalAsync()
        {
            await _navigationService.GoBackAsync();
        }
        //public override void Initialize(INavigationParameters param)
        //{
        //    ConversationPartner = param.GetValue<ContactModel>("obj");
        //}
        public override void OnNavigatedTo(INavigationParameters param)
        {
            ConversationPartner = param.GetValue<ContactModel>("obj");
            CrossCloudFirestore.Current
                .Instance.GetCollection("contacts")
                .GetDocument(ConversationPartner.id)
                .GetCollection("conversations")
                .OrderBy("created_at", false)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (snapshot != null)
                    {
                        foreach (var documentChange in snapshot.DocumentChanges)
                        {
                            var json = JsonConvert.SerializeObject(documentChange.Document.Data);
                            var obj = JsonConvert.DeserializeObject<ConversationModel>(json);
                            switch (documentChange.Type)
                            {
                                case DocumentChangeType.Added:
                                    ConversationsList.Add(obj);
                                    break;
                                case DocumentChangeType.Modified:
                                    if (ConversationsList.Where(c => c.id == obj.id).Any())
                                    {
                                        var item = ConversationsList.Where(c => c.id == obj.id).FirstOrDefault();
                                    }
                                    break;
                                case DocumentChangeType.Removed:
                                    if (ConversationsList.Where(c => c.id == obj.id).Any())
                                    {
                                        var item = ConversationsList.Where(c => c.id == obj.id).FirstOrDefault();
                                        ConversationsList.Remove(item);
                                    }
                                    break;
                            }
                            //missing

                        }
                        //my implementation
                        if(ConversationsList.Count !=0)
                        {
                            MessagingCenter.Send<object, object>(this, "LastMessage", ConversationsList.Last());
                        }
                    }
                });
        }
        public ICommand PopModalCommand { get; set; }

        public INavigationService _navigationService { get; private set; }
        private async void  Send()
        {
            Guid guid = Guid.NewGuid();
            string ID = guid.ToString();
            ConversationModel conversationObject = new ConversationModel()
            {
                id = ID,
                converseeID = dataClass.loggedInUser.uid,
                message = Message,
                created_at = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
            await CrossCloudFirestore.Current
                    .Instance
                    .GetCollection("contacts")
                    .GetDocument(ConversationPartner.id)
                    .GetCollection("conversations")
                    .GetDocument(ID)
                    .SetDataAsync(conversationObject);
            Message = string.Empty;
            MessagingCenter.Send<object,object>(this, "LastMessage",conversationObject);
        }
    }
}
