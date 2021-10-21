using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataWpf.Model
{
   public class ProductCollection : ObservableCollection<Product>
    {
        public static ProductCollection GetAllProducts()
        {

            ProductCollection products = new ProductCollection();
            Product product = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Products", conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        product = Product.GetProductFromResultSet(reader);
                        if (GetTime() > product.Time) 
                        {
                            if (product.Winner != product.LastBidder)
                            {
                                product.Winner = product.LastBidder;
                                product.UpdateProduct();
                            }
                        }
                                products.Add(product);
                    }
                }

            }
        
            return products;
        }
       static private int GetTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int time = (int)t.TotalSeconds;
            return time;
        }
    }
    
} 
