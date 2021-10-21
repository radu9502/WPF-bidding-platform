using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataWpf.Model
{
   public class Product : INotifyPropertyChanged, INotifyDataErrorInfo
    {

        #region Fields

        private int _id;
        private string _name;
        private int _price;
        private int _lastBid;
        private string _lastBidder;
        private int _time;
        private string _winner;
        private int _timer;
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }



        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Id"));

            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;

                List<string> errors = new List<string>();
                bool valid = true;

                if (value == null || value == "")
                {
                    errors.Add(" Product Name can't be empty.");
                    SetErrors("UserName", errors);
                    valid = false;
                }



                if (valid)
                {
                    ClearErrors("Name");
                }


                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }
        public int Price
        {
            get { return _price; }
            set
            {
                if (_price == value)
                {
                    return;
                }
                _price = value;

                List<string> errors = new List<string>();
                bool valid = true;

                if (value <= 0 )
                {
                    errors.Add("Invalid Price.");
                    SetErrors("Price", errors);
                    valid = false;
                }



                if (valid)
                {
                    ClearErrors("Price");
                }


                OnPropertyChanged(new PropertyChangedEventArgs("Price"));
            }
        }

        public int LastBid
        {
            get { return _lastBid; }
            set
            {
                _lastBid = value;

            
                OnPropertyChanged(new PropertyChangedEventArgs("LastBid"));
            }
        }
        public string LastBidder
        {
            get { return _lastBidder; }
            set
            {
                _lastBidder = value;


                OnPropertyChanged(new PropertyChangedEventArgs("LastBidder"));
            }
        }
        public int Time
        {
            get { return _time; }
            set
            {
                _time = value;


                OnPropertyChanged(new PropertyChangedEventArgs("Time"));
            }
        }
        public string Winner
        {
            get { return _winner; }
            set
            {
                _winner = value;


                OnPropertyChanged(new PropertyChangedEventArgs("Winner"));
            }
        }
        public int Timer
        {
            get { return _timer; }
            set
            {
                _timer = value;


                OnPropertyChanged(new PropertyChangedEventArgs("Timer"));
            }
        }

        public bool HasErrors
        {
            get
            {
                return (errors.Count > 0);
            }
        }


        #endregion

        #region Constructor
        public Product(string Name, int Price)
        {
            this.Name = Name;
            this.Price = Price;

        }
        public Product(int Id, string Name, int Price, int LastBid, string LastBidder, int Time, string Winner)
        {
            this.Id = Id;
            this.Name = Name;
            this.Price = Price;
            this.LastBid = LastBid;
            this.LastBidder = LastBidder;
            this.Time = Time;
            this.Winner = Winner;
        }

      

        public Product()
        {
            
            LastBid = 0;
            LastBidder = "";
            Time = 1;
            Winner = "N/A";
        }


        #endregion

        #region Data Access


        public static Product GetProductFromResultSet(SqlDataReader reader)
        {
            Product product = new Product((int)reader["Id"], (string)reader["Name"], (int)reader["Price"], (int)reader["LastBid"], (string)reader["LastBidder"], (int)reader["Time"], (string)reader["Winner"]);
            return product;
        }



        public void DeleteProduct()
        {
            using (SqlConnection conn = new SqlConnection())
            {

                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand("DELETE FROM Products WHERE Id=@Id", conn);

                SqlParameter myParam = new SqlParameter("@Id", SqlDbType.Int, 11);
                myParam.Value = this.Id;

                command.Parameters.Add(myParam);

                int rows = command.ExecuteNonQuery();

            }
        }

        public void UpdateProduct()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand($"UPDATE Products SET Name = '{this.Name}', Price = {this.Price}, LastBid = {this.LastBid}, LastBidder='{this.LastBidder}', Time = {this.Time}, Winner = '{this.Winner}' WHERE Id={this.Id}", conn);

                int rows = command.ExecuteNonQuery();

            }
        }


        public void Insert()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand($"INSERT INTO Products(Name, Price,LastBid,LastBidder, Time, Winner) VALUES('{this.Name}', {this.Price}, 0, 'N/A', {this.Time}, 'N/A'); SELECT IDENT_CURRENT('Users');", conn);

                // command.ExecuteNonQuery();
                //command.ExecuteScalar();
                var id = command.ExecuteScalar();

                if (id != null)
                {
                    this.Id = Convert.ToInt32(id);
                   
                }



            }
        }
        public void Save()
        {
           
                Insert();
            
         
        }


        private void SetErrors(string propertyName, List<string> propertyErrors)
        {
            // Clear any errors that already exist for this property.
            errors.Remove(propertyName);
            // Add the list collection for the specified property.
            errors.Add(propertyName, propertyErrors);
            // Raise the error-notification event.
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
        private void ClearErrors(string propertyName)
        {
            // Remove the error list for this property.
            errors.Remove(propertyName);
            // Raise the error-notification event.
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }



        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                // Provide all the error collections.
                return (errors.Values);
            }
            else
            {
                // Provice the error collection for the requested property
                // (if it has errors).
                if (errors.ContainsKey(propertyName))
                {
                    return (errors[propertyName]);
                }
                else
                {
                    return null;
                }
            }

        }

        public Product Clone()
        {
            Product clonedProduct = new Product();
            clonedProduct.Name = this.Name;
            clonedProduct.Price = this.Price;
            clonedProduct.LastBid = this.LastBid;
            clonedProduct.LastBidder = this.LastBidder;
            clonedProduct.Time = this.Time;
            clonedProduct.Winner = this.Winner;
            clonedProduct.Id = this.Id;

            return clonedProduct;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Product objUser = (Product)obj;

            if (objUser.Id == this.Id) return true;

            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

   

        #endregion

    }
    
    
}
