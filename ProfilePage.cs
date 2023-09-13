using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace cSharp_BankSystemUsingSQLServer
{
    internal class ProfilePage
    {
        User user = new User();
        private static string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

        public void profile()
        {
            Console.WriteLine($"Welcome, {user.Name}");

            List<Account> userAccounts = GetUserAccounts(user.UserId);
            if ( userAccounts != null )
            {
                Console.WriteLine("Your Accounts:");
                foreach (var account in userAccounts)
                {
                    Console.WriteLine($"Account Number: {account.AccountNumber}");
                    Console.WriteLine($"Account Holder Name: {account.AccountHolderName}");
                    Console.WriteLine($"Account Balance: {account.Balance} OMR");
                    Console.WriteLine("____________________________________");
                    Console.WriteLine();
                }
            }
            else { Console.WriteLine("You dont have any accounts. Please add one"); }
            
        }


        private static List<Account> GetUserAccounts(int userId)
        {
            List<Account> userAccounts = new List<Account>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query the database to retrieve accounts for the user
                string selectSql = "SELECT * FROM Account WHERE UserId = @UserId";
                using (SqlCommand command = new SqlCommand(selectSql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userAccounts.Add(new Account
                            {
                                AccountNumber = Convert.ToInt32(reader["AccountNumber"]),
                                AccountHolderName = reader["AccountHolderName"].ToString(),
                                Balance = Convert.ToDecimal(reader["Balance"])
                            });
                        }
                    }
                }
            }

            return userAccounts;
        }

    }
}
