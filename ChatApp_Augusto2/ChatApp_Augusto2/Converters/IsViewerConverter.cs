using ChatApp_Augusto2.Helpers;
using ChatApp_Augusto2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace ChatApp_Augusto2.Converters
{
    public class IsViewerConverter : IValueConverter
    {
        DataClass dataClass = DataClass.GetInstance;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retVal = false;

            if(value != null)
            {
                string converseeID = value as string;
                //ConversationModel conversation = value as ConversationModel;
                if (converseeID.Equals(dataClass.loggedInUser.uid))
                    retVal = true;
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
