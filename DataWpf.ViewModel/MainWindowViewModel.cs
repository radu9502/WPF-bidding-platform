using DataWpf.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DataWpf.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        #region Fields
        private User loggedUser;
        private User currentUser;
        private UserCollection userList;
        private ListCollectionView userListView;
        private Mediator mediator;


        #endregion

        #region Properties

        public User LoggedUser
        {
            get { return loggedUser; }
            set
            {
                if (loggedUser == value)
                {
                    return;
                }
                loggedUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LoggedUser"));
            }
        }
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

        public UserCollection UserList
        {
            get { return userList; }
            set
            {
                if (userList == value)
                {
                    return;
                }
                userList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UserList"));
            }
        }

        public ListCollectionView UserListView
        {
            get { return userListView; }
            set
            {
                if (userListView == value)
                {
                    return;
                }
                userListView = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UserListView"));
            }
        }




        #endregion


        private ICommand deleteCommand;

        public ICommand DeleteCommand
        {
            get { return deleteCommand; }
            set
            {
                if (deleteCommand == value)
                {
                    return;
                }
                deleteCommand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeleteCommand"));
            }
        }

        void DeleteExecute(object obj)
        {
           
                CurrentUser.DeleteUser();
                UserList.Remove(CurrentUser);
            
        }

        bool CanDelete(object obj)
        {

            if (CurrentUser == null) return false;

            return true;
        }

        

        #region Constructors
        public MainWindowViewModel(User _loggedUser, Mediator mediator)
        {

            this.loggedUser = _loggedUser;

            this.mediator = mediator;

            DeleteCommand = new RelayCommand(DeleteExecute, CanDelete);


            UserList = UserCollection.GetAllUsers();

            UserListView = new ListCollectionView(UserList);

            CurrentUser = new User();

            mediator.Register("UserChange", UserChange);


        }
        public MainWindowViewModel(Mediator mediator)
        {

            this.mediator = mediator;

            DeleteCommand = new RelayCommand(DeleteExecute, CanDelete);


            UserList = UserCollection.GetAllUsers();

            UserListView = new ListCollectionView(UserList);

            CurrentUser = new User();

            mediator.Register("UserChange", UserChange);

        }

        private void UserChange(object obj)
        {
            User user = (User)obj;

            int index = UserList.IndexOf(user);

            if (index != -1)
            {
                UserList.RemoveAt(index);
                UserList.Insert(index, user);
            }
            else
            {
                UserList.Add(user);
            }


        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("FilteringText"))
            {
                UserListView.Refresh();
            }
        }

 
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
