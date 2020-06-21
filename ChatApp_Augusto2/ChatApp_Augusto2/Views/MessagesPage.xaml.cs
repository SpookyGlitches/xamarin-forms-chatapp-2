using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChatApp_Augusto2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagesPage : ContentPage
    {
        public MessagesPage()
        {
            InitializeComponent();
            //MessagingCenter.Subscribe<object>(this, "LastMessage", (sender) => {
            //    MainScreenMessagesListView.ScrollTo(..., ScrollToPosition.End, true);
            //});
            
            MessagingCenter.Subscribe<object,object>(this, "LastMessage", (sender,arg) =>
              {
                  ConversationsListView.ScrollTo(arg, ScrollToPosition.End, true);
              });
        }
    }
}