using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataWpf.Model
{
    public class User : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        
        #region Fields

        private int _id;
        private string _userName;
        private string _userPass;
        private int _isAdmin;
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

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName == value)
                {
                    return;
                }
                _userName = value;

                List<string> errors = new List<string>();
                bool valid = true;

                if (value == null || value == "")
                {
                    errors.Add(" Username can't be empty.");
                    SetErrors("UserName", errors);
                    valid = false;
                }


                if (!Regex.Match(value, @"^[a-zA-Z]+$").Success)
                {
                    errors.Add("Username can only contain letters.");
                    SetErrors("UserName", errors);
                    valid = false;
                }

                if (valid)
                {
                    ClearErrors("UserName");
                }


                OnPropertyChanged(new PropertyChangedEventArgs("UserName"));
            }
        }
        public string UserPass
        {
            get { return _userPass; }
            set
            {
                if (_userPass == value)
                {
                    return;
                }
                _userPass = value;

                List<string> errors = new List<string>();
                bool valid = true;

                if (value == null || value == "")
                {
                    errors.Add("Password can't be empty.");
                    SetErrors("UserPass", errors);
                    valid = false;
                }



                if (valid)
                {
                    ClearErrors("UserPass");
                }


                OnPropertyChanged(new PropertyChangedEventArgs("UserPass"));
            }
        }

        public int IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;

                List<string> errors = new List<string>();
                bool valid = true;


                if (value != 1 && value != 0)
                {
                    errors.Add("Is Admin can only be 0 or 1");
                    SetErrors("IsAdmin", errors);
                    valid = false;
                }

                if (valid)
                {
                    ClearErrors("IsAdmin");
                }



                OnPropertyChanged(new PropertyChangedEventArgs("IsAdmin"));
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
        public User(string UserName, string UserPass)
        {
            this.UserName = UserName;
            this.UserPass = UserPass;
   
        }
        public User(string UserName, string UserPass, int IsAdmin)
        {
            this.UserName = UserName;
            this.UserPass = UserPass;
            this.IsAdmin = IsAdmin;
        }

        public User(int Id, string UserName, string UserPass,int IsAdmin)
        {
            this.UserName = UserName;
            this.UserPass = UserPass;
            this.IsAdmin = IsAdmin;
            this.Id = Id;
        }

        public User()
        {
            UserName = "";
            UserPass = "";
            IsAdmin = 3;
        }


        #endregion

        #region Data Access


        public static User GetUserFromResultSet(SqlDataReader reader)
        {
            User user = new User((int)reader["Id"], (string)reader["UserName"], (string)reader["UserPass"], (int)reader["IsAdmin"]);
            return user;
        }



        public void DeleteUser()
        {
            using (SqlConnection conn = new SqlConnection())
            {

                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand("DELETE FROM Users WHERE Id=@Id", conn);

                SqlParameter myParam = new SqlParameter("@Id", SqlDbType.Int, 11);
                myParam.Value = this.Id;

                command.Parameters.Add(myParam);

                int rows = command.ExecuteNonQuery();

            }
        }

        public void UpdateUser()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand($"UPDATE Users SET UserName = '{this.UserName}', UserPass = '{this.UserPass}', IsAdmin = '{this.IsAdmin}' WHERE Id={this.Id}", conn);

                int rows = command.ExecuteNonQuery();

            }
        }


        public void Insert()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Users(UserName, UserPass, IsAdmin) VALUES(@UserName, @UserPass, @IsAdmin); SELECT IDENT_CURRENT('Users');", conn);

                SqlParameter userNameParam = new SqlParameter("@UserName", SqlDbType.NVarChar);
                userNameParam.Value = this.UserName;

                SqlParameter userPassParam = new SqlParameter("@UserPass", SqlDbType.NVarChar);
                userPassParam.Value = this.UserPass;

                SqlParameter isAdminParam = new SqlParameter("@IsAdmin", SqlDbType.Int);
                isAdminParam.Value = this.IsAdmin;

                command.Parameters.Add(userNameParam);
                command.Parameters.Add(userPassParam);
                command.Parameters.Add(isAdminParam);

                var id = command.ExecuteScalar();

                if (id != null)
                {
                    this.Id = Convert.ToInt32(id);
                }

            }
        }
        public User CheckUser()
        {
            User loggedAs = new User(); //{ Id= 1, UserName = "asd", UserPass ="21231", IsAdmin =1};
             using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=DESKTOP-BV1K2CO\\SQLEXPRESS;Database=auction;Integrated Security=True;";
                conn.Open();
                                                            //top 1
                SqlCommand command = new SqlCommand($"SELECT top 1 * FROM Users Where UserName = '{this.UserName}' and UserPass = '{this.UserPass}'", conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loggedAs = GetUserFromResultSet(reader);
                        
                    }
                }
               
                if (this.UserName != loggedAs.UserName) return null;
                return loggedAs;

            }
        }
        public void Save()
        {
            if (Id == 0)
            {
                Insert();
            }
            else
            {
                UpdateUser();
            }
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

        public User Clone()
        {
            User clonedUser = new User();
            clonedUser.UserName = this.UserName;
            clonedUser.UserPass = this.UserPass;
            clonedUser.IsAdmin = this.IsAdmin;
            clonedUser.Id = this.Id;

            return clonedUser;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            User objUser = (User)obj;

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
