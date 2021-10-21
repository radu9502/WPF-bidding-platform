using DataWpf.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataWpf.ViewModel
{
    public class ProductEditWindowViewModel : INotifyPropertyChanged
    {

        private Product currentProduct;
 
        private Mediator mediator;

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

     
    



        public ProductEditWindowViewModel(Mediator mediator)
        {

            this.mediator = mediator;
            SaveCommand = new RelayCommand(SaveExecute, CanSave);

            CurrentProduct = new Product();
        }

        private ICommand saveCommand;

        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set
            {
                if (saveCommand == value)
                {
                    return;
                }
                saveCommand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SaveCommand"));
            }
        }

        void SaveExecute(object obj)
        {

            if (CurrentProduct != null && !CurrentProduct.HasErrors)
            {
                currentProduct.Time = GetTime();
                CurrentProduct.Save();

               
                OnDone(new DoneEventArgs("Product Saved."));
                
                mediator.Notify("ProductChange", CurrentProduct);
            }
            else
            {
                OnDone(new DoneEventArgs("Check your input."));
            }
        }
      static public int GetTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int time = (int)t.TotalSeconds + 120;
            return time;
        }


        bool CanSave(object obj)
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
