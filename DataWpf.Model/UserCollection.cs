using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataWpf.Model
{
    public class UserCollection : ObservableCollection<User>
    {
        public static UserCollection GetAllUsers()
        {

            UserCollection users = new UserCollection();
            User user = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Users", conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        user = User.GetUserFromResultSet(reader);
                        users.Add(user);
                    }
                }

            }
            return users;
        }

    }
}
