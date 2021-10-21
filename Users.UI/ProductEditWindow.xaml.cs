using DataWpf.Model;
using DataWpf.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Users.UI
{
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProductEditWindowViewModel viewModel = (ProductEditWindowViewModel)DataContext;
            viewModel.Done += ViewModel_Done;
        }

        private void ViewModel_Done(object sender, ProductEditWindowViewModel.DoneEventArgs e)
        {
            MessageBox.Show(e.Message);
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
