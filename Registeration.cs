using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace cSharp_BankSystemUsingSQLServer
{
    internal class Registeration
    {
        LoginPage loginPage = new LoginPage();
        public void Register() 
        {
            Console.WriteLine(">> Registeration <<\n\n");
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            if (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email address.");
                return;
            }

            if (!IsValidPassword(password))
            {
                Console.WriteLine("Invalid password. Password must meet certain requirements.");
                return;
            }

            string hashedPassword = HashPassword(password); //hashing the password 

            // If email and password are valid, insert data into the database
            InsertUserRegistrationData(name, email, hashedPassword);

            Console.WriteLine("User registration successful.");
            Console.WriteLine("\n\n\n\n\n\nPress any key to go.....");
            Console.ReadLine();
            Console.Clear();
            // >>>>>>>>>>>>>>>>Go to profile page<<<<<<<<<<<<<<
            loginPage.Login();
        }

        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$"; //Regular expression for email validation
            return Regex.IsMatch(email, pattern);
        }

        private static bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"; //Uppercase and Lowercase Letters, Digits, and Special Characters (Minimum Length 8):

            Regex regex = new Regex(pattern);
            return regex.IsMatch(password); // Return true if password meets your requirements
        }

        // Insert user registration data into the database
        private static void InsertUserRegistrationData(string name, string email, string password)
        {
            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertSql = "INSERT INTO Users (user_Name, Email, Password) VALUES (@Name, @Email, @Password)";
                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private static string HashPassword(string password)
        {
            // BCrypt to hash the password
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
