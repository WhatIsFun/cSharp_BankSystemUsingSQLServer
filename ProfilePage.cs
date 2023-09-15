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
        Transaction transaction = new Transaction();
        User user = new User();
        private static string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

        public void profileMenu()
        {
            Console.WriteLine($"Welcome, {user.Name}\n\n");

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
            
            //Console.ForegroundColor = ConsoleColor.Blue;
            //Console.WriteLine("\n$ $$ Operations $$ $\n");
            //Console.ResetColor();
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine("3) Deposite\n4) Withdraw\n5) Transfer Money\n6) Account history");
            //Console.ResetColor();
            

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("1) Create new account\n2) Make a transaction\n3) Account history4) Delete account\n5) Delete User\n6) Logout");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("7) Logout");
                Console.ResetColor();
                Console.Write("\n\nSelect an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        createAccount();
                        break;
                    case "2":
                        transaction.transactionMenu();
                        break;
                    //case "3":
                    //    ();
                    //    break;
                    //case "4":
                    //    ();
                    //    break;
                    //case "5":
                    //    ();
                    //    break;
                    //case "6":
                    //    ();
                    //    break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.Clear();
            }
        }
        private void createAccount()
        {
            Console.Write("Enter initial balance: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
            {
                decimal balance = initialBalance;
                int UserID = user.UserId;
                string AccountHolderName = user.Name;
                insertAccount(balance, UserID, AccountHolderName);
                return;
            }
            else
            {
                Console.WriteLine("Invalid initial balance.");
                return;
            }

        }
        private static void insertAccount(decimal balance, int UserID, string AccountHolderName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertSql = "INSERT INTO Account (Balance, User_Id, AccountHolderName) VALUES (@Balance, @UserID, @AccountHolderName)";// Check the code
                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@Balance", balance);
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@AccountHolderName", AccountHolderName);

                        command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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
                                Balance = Convert.ToDecimal(reader["Balance"]),
                                AccountHolderName = reader["AccountHolderName"].ToString()
                            });
                        }
                    }
                }
                connection.Close();
            }

            return userAccounts;
        }

    }
}
