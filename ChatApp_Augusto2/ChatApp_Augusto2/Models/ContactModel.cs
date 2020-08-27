﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Xamarin.Forms;

namespace ChatApp_Augusto2.Models
{
    public class ContactModel : INotifyPropertyChanged
    {
        string _id { get; set; }
        public string id
        {
            get { return _id;  }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(id));
            }
        }
        string[] _contactID { get; set; }
        public string[] contactID
        {
            get { return _contactID; }
            set
            {
                _contactID = value;
                OnPropertyChanged(nameof(contactID));
            }
        }
        string[] _contactName { get; set; }
        public string[] contactName {
            get { return _contactName; }
            set
            {
                _contactName = value;
                OnPropertyChanged(nameof(contactName));
            }
        }
        string[] _contactEmail { get; set; }
        public string[] contactEmail
        {
            get { return _contactEmail; }
            set
            {
                _contactEmail = value;
                OnPropertyChanged(nameof(contactEmail));
            }
        }
        long _created_at { get; set; }
        public long created_at
        {
            get { return _created_at; }
            set
            {
                _created_at = value;
                OnPropertyChanged(nameof(created_at));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}