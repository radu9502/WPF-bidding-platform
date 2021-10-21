using DataWpf.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DataWpf.ViewModel
{
    public class LoginWindowViewModel : INotifyPropertyChanged
    {
        
        private User currentUser;
        private Mediator mediator;


        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                if (currentUser == value)
                {
                    return;
                }
                currentUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUser"));
            }
        }
        public Action CloseAction { get; set; }
        public Action OpenAction { get; set; }

        private void OpenCommandFunction()
        {
            OpenAction();
        }
        private void CloseCommandFunction()
        {
            CloseAction();
        }

     

        public LoginWindowViewModel()
        {
            
            LoginCommand = new RelayCommand(LoginExecute, CanLogin);
            CurrentUser = new User();
            
            Console.WriteLine("test1");

        }

        private ICommand loginCommand;

        public ICommand LoginCommand
        {
            get { return loginCommand; }
            set
            {
                if (loginCommand == value)
                {
                    return;
                }
                loginCommand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LoginCommand"));
            }
        }

        void LoginExecute(object obj)
        {

            Console.WriteLine(CurrentUser.UserName);
            CurrentUser = CurrentUser.CheckUser();
            if (CurrentUser == null)
            {
                MessageBox.Show("Login failed. Please try again!");
                CurrentUser = new User();
                


            }
            else {

                OpenCommandFunction();
                CloseCommandFunction();


            }
        }

        bool CanLogin(object obj)
        {
          return true;  
        }
       
        public delegate void DoneEventHandler(object sender, DoneEventArgs e);

        public class DoneEventArgs : EventArgs
        {
            private string message;

            public string Message
            {
                get { return message; }
                set
                {
                    if (message == value)
                    {
                        return;
                    }
                    message = value;
                }
            }

            public DoneEventArgs(string message)
            {
                this.message = message;
            }
        }


        public event DoneEventHandler Done;

        public void OnDone(DoneEventArgs e)
        {
            if (Done != null)
            {
                Done(this, e);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
