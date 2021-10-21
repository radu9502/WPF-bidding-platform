using DataWpf.Model;
using DataWpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Users.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
   
    public partial class AuctionWindow : Window
    {
        
    
        AuctionWindowViewModel mvvm;
    
        public AuctionWindow()
        {
            InitializeComponent();
            mvvm = new AuctionWindowViewModel(Mediator.Instance);
            this.DataContext = mvvm;
            newBtn.IsEnabled = false;
            mngAcc.IsEnabled = false;
    

        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            ProductEditWindow newWindow = new ProductEditWindow();
            newWindow.DataContext = new ProductEditWindowViewModel(Mediator.Instance);
            
            newWindow.ShowDialog();

        }




        private void login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindowViewModel mvm = new LoginWindowViewModel();
            LoginWindow window = new LoginWindow();

            window.DataContext = mvm;
            mvm.OpenAction = new Action(() =>
          {
              mvvm.LoggedUser = mvm.CurrentUser;
              if (mvm.CurrentUser.IsAdmin == 1)
              {
                  mngAcc.IsEnabled = true;
                  newBtn.IsEnabled = true;
              }
              else
              {
                  mngAcc.IsEnabled = false;
                  newBtn.IsEnabled = false;
              }
              login.Visibility = Visibility.Hidden;
              this.DataContext = mvvm;
          });

            mvm.CloseAction = new Action(() => window.Close());
            window.ShowDialog();

        



        }
        private void mngAcc_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.ShowDialog();


        }


       
        private void bid_Click(object sender, RoutedEventArgs e)
        {

        }


        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private int GetTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int time = (int)t.TotalSeconds;
            return time;
        }


    }
}
