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
                    Console.WriteLine($"Account Number: {account.AccountId}");
                    Console.WriteLine($"Account Holder Name: {user.Name}");
                    Console.WriteLine($"Account Balance: {account.Balance} OMR");
                    Console.WriteLine("____________________________________");
                    Console.WriteLine();
                }
            }
            else { Console.WriteLine("You dont have any accounts. Please add one\n\n"); }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1) Create new account\n2) Manage my accounts");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n$ $$ Operations $$ $\n");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("3) Deposite\n4) Withdraw\n5) Transfer Money\n6) Account history");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("7) Logout");
            Console.ResetColor();

        }


        private static List<Account> GetUserAccounts(int userId)
        {
            List<Account> userAccounts = new List<Account>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query the database to retrieve accounts for the user
                string selectSql = "SELECT * FROM Account WHERE user_ID = @UserId";
                using (SqlCommand command = new SqlCommand(selectSql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userAccounts.Add(new Account
                            {
                                AccountId = Convert.ToInt32(reader["Account_Id"]),
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
