using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace cSharp_BankSystemUsingSQLServer
{
    internal class LoginPage
    {
        private List<Account> userAccounts;
        private static string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
        public LoginPage(List<Account> userAccounts)
        {
            this.userAccounts = userAccounts; // Initialize userAccounts in the constructor
        }

        public LoginPage()
        {
        }

        public void Login(List<Account> userAccounts)
        {
            ProfilePage profilePage = new ProfilePage();
            // Prompt the user for email and password
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();

            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            // Authenticate the user
            User authenticatedUser = AuthenticateUser(email, password);

            if (authenticatedUser == null)
            {
                Console.WriteLine("Invalid email or password. Please try again.");
                Console.WriteLine("\n\n\n\n\n\nPress any key to try again.....");
                Console.ReadLine();
                return;
            }
            else
            {
                // User is authenticated; grant access to the program
                Console.WriteLine("Login successful! Welcome, " + authenticatedUser.Name);
                loading();
                Console.Clear();
                profilePage.profileMenu(authenticatedUser, userAccounts);
            }
        }
        private static User AuthenticateUser(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query the database to find a user with the provided email
                string selectSql = "SELECT * FROM Users WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(selectSql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // User with the provided email exists; check the password
                            string storedPassword = reader["Password"].ToString(); // Get the stored hashed password from the database

                            // Verify the provided password against the stored hash
                            if (VerifyPassword(password, storedPassword))
                            {
                                // Password is correct; return the user object
                                return new User()
                                {
                                    UserId = Convert.ToInt32(reader["user_ID"]),
                                    Name = reader["user_Name"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Password = reader["Password"].ToString()

                                };
                                
                            }//reader.Close();
                        }
                        else
                        {
                            Console.WriteLine("No data retrieved");
                        }
                    }
                }
                connection.Close();
            }

            // No matching user or incorrect password; return null
            return null;
        }

        // Implement a password verification method
        private static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }
        public void loading()
        {
            string[] spinner = { "-", "\\", "|", "/" };

            Console.Write("Loading ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(spinner[i % spinner.Length]);
                Thread.Sleep(200);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Account> userAccount { get; set; }
        public User()
        {
            userAccount = new List<Account>();
        }
        
    }


}

