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
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
namespace ChatApp_Augusto2.ViewModels
{
    public class ChatPageViewModel : ViewModelBase
    {
        public ChatPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            SendCommand = new Command(Send);
            Users = new ObservableCollection<UserModel>();
            _pageDialogService = pageDialogService;
            SearchCommand = new Command(Search);
            _navigationService = navigationService;
            //Messages.Add(new ViewModels.Text(1,"1ST"));
        }

        private async void Search()
        {
            var param = new Prism.Navigation.NavigationParameters();
            param.Add("search", Text);
            await _navigationService.NavigateAsync("SearchPage",param) ;
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
        public ICommand SendCommand { get; set; }
        private void Send()
        {
            //GetResults();
            //Messages.Add(new ViewModels.Text(2,text));
        }
        private async void GetResults()
        {
            var documents = await CrossCloudFirestore.Current
                                .Instance
                                .GetCollection("users")
                                .WhereEqualsTo("email",Text)
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
