using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatApp_Augusto2.Validations
{
    public class ValidatableString : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _value;
        private bool isValid;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (isValid != true || string.IsNullOrEmpty(value))
                {
                    IsValid = true;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                OnPropertyChanged();
            }
        }
        public void Validate()
        {
            IsValid = !string.IsNullOrWhiteSpace(Value) ? true : false;
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
