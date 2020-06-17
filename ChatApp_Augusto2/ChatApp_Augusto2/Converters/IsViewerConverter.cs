using ChatApp_Augusto2.Helpers;
using ChatApp_Augusto2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ChatApp_Augusto2.Converters
{
    public class IsViewerConverter
    {
        DataClass dataClass = DataClass.GetInstance;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retVal = false;

            if(value != null)
            {
                ConversationModel conversation = value as ConversationModel;
                if (conversation.converseeID.Equals(dataClass.loggedInUser.uid))
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
