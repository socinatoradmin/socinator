using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.PDModel
{
    public class CreateAccountInfo : BindableBase, IPost
    {
        private string _email;

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _age;

        public string Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }

        private bool _male = true;

        public bool Male
        {
            get { return _male; }
            set { SetProperty(ref _male, value); }
        }

        private bool  _female;

        public bool  Female
        {
            get { return _female; }
            set { SetProperty(ref _female, value); }
        }

        private string _gender;

        public string Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }
        private bool _IsCheckedToAccountManager=true;
        public bool IsCheckedToAccountManager
        {
            get => _IsCheckedToAccountManager;
            set=>SetProperty(ref  _IsCheckedToAccountManager, value);
        }
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
    }
}
