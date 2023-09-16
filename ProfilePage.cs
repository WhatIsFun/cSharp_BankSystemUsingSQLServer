using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using cSharp_BankSystemUsingSQLServer;

namespace cSharp_BankSystemUsingSQLServer
{
    internal class ProfilePage
    {
        
        private static string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
        public List<Account> userAccounts = new List<Account>();

        public void profileMenu(User authenticatedUser, List<Account> userAccounts)
        {
            Transaction transaction = new Transaction();
            HomePage homePage = new HomePage();
            if (authenticatedUser != null)
            {
                Console.WriteLine($"Welcome, {authenticatedUser.Name}\n\n");

                userAccounts = GetUserAccounts(authenticatedUser.UserId);
                if (userAccounts != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Your Accounts:");
                    Console.ResetColor();

                    foreach (var account in userAccounts)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"Account Number: {account.AccountId}");
                        Console.WriteLine($"Account Holder Name: {authenticatedUser.Name}");
                        Console.WriteLine($"Account Balance: {account.Balance} OMR");
                        Console.ResetColor();
                        Console.WriteLine("____________________________________");
                        Console.WriteLine();
                    }
                }
                else { Console.WriteLine("You dont have any accounts. Please add one\n\n"); }
            }
            else
            {
                Console.WriteLine("Login failed");
                homePage.mainMenu();
            }


            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("1) Create new account\n2) Make a transaction\n3) View Transaction History\n4) Delete account\n5) Delete User\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("6) Logout");
                Console.ResetColor();
                Console.Write("\n\nSelect an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        createAccount(authenticatedUser);
                        break;
                    case "2":
                        Console.Clear();
                        transaction.transactionMenu(userAccounts, authenticatedUser);
                        break;
                    case "3":
                        transaction.history(userAccounts, authenticatedUser);
                        break;
                    case "4":
                        Console.Clear();
                        deleteAccount(authenticatedUser, userAccounts);
                        break;
                    case "5":
                        Console.Clear();
                        deleteUser(authenticatedUser);
                        break;
                    case "6":
                        Console.Clear();
                        homePage.mainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                //Console.Clear();
            }
        }
        private void createAccount(User authenticatedUser)
        {
            Console.Write("Enter initial balance: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
            {
                decimal balance = initialBalance;
                int UserID = authenticatedUser.UserId;
                string AccountHolderName = authenticatedUser.Name;
                insertAccount(balance, UserID, AccountHolderName);
                Console.WriteLine("\n\n\n\n\n\nPress any key to go back.....");
                Console.ReadLine();
                profileMenu(authenticatedUser, userAccounts);                
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
            List<Account> accounts = new List<Account>();
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
                            accounts.Add(new Account
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

            return accounts;
        }

        private void deleteAccount(User authenticatedUser, List<Account> userAccounts)
        {
            Console.Clear();
            Console.WriteLine("Delete Account");
            Console.Write("Enter the account number to delete: ");
            int accountIdToDelete;
            if (!int.TryParse(Console.ReadLine(), out accountIdToDelete))
            {
                Console.WriteLine("Invalid account ID. Account deletion failed.");
                Console.WriteLine("Press Enter to go back ...");
                Console.ReadLine();
                return;
            }
            // Check if the provided account ID exists in the user's accounts
            if (userAccounts != null)
            {

                foreach (var account in userAccounts)
                {
                    if (userAccounts.Any(account => account.AccountId == accountIdToDelete))
                    {
                        // Verify the provided email and password against the authenticated user's credentials
                        Console.Write("Enter your password to confirm deletion: ");
                        string passwordInput = Console.ReadLine();

                        if (VerifyPassword(passwordInput, authenticatedUser.Password))
                        {
                            deleteAccountServer(accountIdToDelete);
                        }
                        else
                        {
                            Console.WriteLine("Invalid password. Account deletion failed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Account with the specified ID does not exist in your accounts. Account deletion failed.");
                    }

                    Console.WriteLine("Press Enter to go back...");
                    Console.ReadLine();
                    Console.Clear();
                    profileMenu(authenticatedUser, userAccounts);
                }
            }
        }
        private void deleteAccountServer(int accountIdToDelete)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // delete the account from the accounts table
                    string deletesql = "delete from Account where Account_Id = @accountid";
                    using (SqlCommand deletecommand = new SqlCommand(deletesql, connection))
                    {
                        deletecommand.Parameters.AddWithValue("@accountid", accountIdToDelete);
                        deletecommand.ExecuteNonQuery();
                    }

                    Console.WriteLine($"account with id {accountIdToDelete} deleted successfully.\nVisit nearest ATM to withdraw your balance");

                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void deleteUser(User authenticatedUser)
        {
            Console.WriteLine("Delete User\n\nSoryy to hear that");
            int userToDelete = authenticatedUser.UserId;
            if (authenticatedUser != null)
            {
                Console.Write("Enter your password to confirm deletion: ");
                string passwordInput = Console.ReadLine();

                if (VerifyPassword(passwordInput, authenticatedUser.Password))
                {
                    deleteUserServer(userToDelete);
                }
                else
                {
                    Console.WriteLine("Invalid password. Account deletion failed.");
                }
            }
            Console.WriteLine("Press Enter to go back...");
            Console.ReadLine();
            Console.Clear();
            profileMenu(authenticatedUser, userAccounts);


        }
        private void deleteUserServer(int userToDelete)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // delete the account from the accounts table
                    string deletesql = "delete from Users where User_ID = @userId";
                    using (SqlCommand deletecommand = new SqlCommand(deletesql, connection))
                    {
                        deletecommand.Parameters.AddWithValue("@userId", userToDelete);
                        deletecommand.ExecuteNonQuery();
                    }

                    Console.WriteLine($"account with id {userToDelete} deleted successfully.");

                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
        private static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }


    }
}
