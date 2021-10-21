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
using System.Windows.Threading;

namespace DataWpf.ViewModel
{
    public class AuctionWindowViewModel : INotifyPropertyChanged
    {

        #region Fields
        
        private User loggedUser;
        private Product currentProduct;
        private ProductCollection productList;
        private ListCollectionView productListView;
        private Mediator mediator;

        Dictionary<Product, DispatcherTimer> timerInstances = new Dictionary<Product, DispatcherTimer>();

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
        public Product CurrentProduct
        {
            get { return currentProduct; }
            set
            {
                if (currentProduct == value)
                {
                    return;
                }
                currentProduct = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentProduct"));
            }
        }

        public ProductCollection ProductList
        {
            get { return productList; }
            set
            {
                if (productList == value)
                {
                    return;
                }
                productList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProductList"));
            }
        }

        public ListCollectionView ProductListView
        {
            get { return productListView; }
            set
            {
                if (productListView == value)
                {
                    return;
                }
                productListView = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProductListView"));
            }
        }




        #endregion
        public Action RefreshAction { get; set; }

        private void RefreshCommandFunction()
        {
            RefreshAction();
        }

        private ICommand bidCommand;
        private ICommand deleteComand;
        private ICommand updateWinnerCommand;
        public ICommand UpdateWinnerCommand
        {
            get { return updateWinnerCommand; }
            set
            {
                if (updateWinnerCommand == value)
                {
                    return;
                }
                updateWinnerCommand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UpdateWinnerComand"));
            }
        }
        public ICommand DeleteCommand
        {
            get { return deleteComand; }
            set
            {
                if (deleteComand == value)
                {
                    return;
                }
                deleteComand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeleteComand"));
            }
        }
        public ICommand BidCommand
        {
            get { return bidCommand; }
            set
            {
                if (bidCommand == value)
                {
                    return;
                }
                bidCommand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BidCommand"));
            }
        }

        void BidExecute(object obj)
        {
            if (LoggedUser.UserName == "") MessageBox.Show("You need to log in first!");
            else
            {
                CurrentProduct.LastBid = 1;
                CurrentProduct.LastBidder = LoggedUser.UserName;
                CurrentProduct.Time = ProductEditWindowViewModel.GetTime();
                CurrentProduct.Price = currentProduct.Price + 1;
                CurrentProduct.UpdateProduct();
                StartTimer(CurrentProduct);
                //RefreshAction();
                
            }
        }
        void DeleteExecute(object obj)
        {
            if (loggedUser.IsAdmin == 1)
            {
                CurrentProduct.DeleteProduct();
                ProductList.Remove(CurrentProduct);
                MessageBox.Show("Product Deleted");
            }
            else
            {
                MessageBox.Show("Only admins can delete products!");
            }
        }
        void UpdateWinnerExecute(object obj)
        {
            if (CurrentProduct != null && CurrentProduct.Winner != CurrentProduct.LastBidder)
            {
                CurrentProduct.Winner = CurrentProduct.LastBidder;
                CurrentProduct.UpdateProduct();
                MessageBox.Show($"{currentProduct.Winner} has won the auction for {CurrentProduct.Name}!");
            }
        }
        void UpdateWinner(Product prod)
        {
            if (prod != null && prod.Winner != prod.LastBidder)
            {
                prod.Winner = prod.LastBidder;
                prod.UpdateProduct();
                MessageBox.Show($"{prod.Winner} has won the auction for {prod.Name}!");
            }
        }

        bool CanDelete(object obj)
        {

            if (CurrentProduct == null) return false;

            return true;
        }
        bool CanBid(object obj)
        {

            if (CurrentProduct == null) return false;
            if (CurrentProduct.Winner != "N/A" || currentProduct.Timer == 0) return false;
            return true;
        }
        bool CanUpdateWinner(object obj)
        {

            return true;
        }

        public void StartTimer(Product item)
        {
            DispatcherTimer dt = new DispatcherTimer();
        
            int timeNow = GetTime();
            int _timer = item.Time - timeNow;
            item.Timer = _timer;
            if (timerInstances.ContainsKey(item))
            {
                dt = timerInstances[item];

            }
            else
            {
                timerInstances.Add(item, dt);
                dt.Interval = TimeSpan.FromSeconds(1);
                dt.Tick += (object _sender, EventArgs _e) =>
                {
                    if (item.Timer > 0)
                    {
                        item.Timer--;
                     

                    }
                    else
                    {
                        UpdateWinner(item);
                        dt.Stop();
                    }
                };
                dt.Start();
            }
        }

        private int GetTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int timeNow = (int)t.TotalSeconds;
            return timeNow;
        }

        #region Constructors

        public AuctionWindowViewModel(Mediator mediator)
        {
            this.loggedUser = new User();
            this.mediator = mediator;

            BidCommand = new RelayCommand(BidExecute, CanBid);
            DeleteCommand = new RelayCommand(DeleteExecute, CanDelete);
            UpdateWinnerCommand = new RelayCommand(UpdateWinnerExecute, CanUpdateWinner);

            ProductList = ProductCollection.GetAllProducts();
            ProductCollection temp = ProductList;

            int timeNow = GetTime();
            foreach (Product item in temp)
            {
               
                if (item.Time > timeNow)
                {
                    if (item != null)
                    {

                        StartTimer(item);
                       
                    }
                }


            }
            productList = temp;
            ProductListView = new ListCollectionView(ProductList);

            // CurrentProduct = new Product();
            
            mediator.Register("ProductChange", ProductChange);

        }

        private void ProductChange(object obj)
        {
            Product product = (Product)obj;

                ProductList.Add(product);
            if (timerInstances.ContainsKey(product)) timerInstances.Remove(product);
                
                StartTimer(product);

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
