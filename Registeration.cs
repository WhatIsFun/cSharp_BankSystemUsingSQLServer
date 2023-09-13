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
        public void Register() 
        {
            Console.Clear();
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

            // If email and password are valid, insert data into the database
            InsertUserRegistrationData(name, email, password);

            Console.WriteLine("User registration successful.");
        }

        // Regex pattern for email validation
        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";
            return Regex.IsMatch(email, pattern);
        }

        // Custom rules for password validation
        private static bool IsValidPassword(string password)
        {
            string pattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password); // Return true if password meets your requirements
        }

        // Insert user registration data into the database
        private static void InsertUserRegistrationData(string name, string email, string password)
        {
            string connectionString = "Data Source=(local);Initial Catalog=ITI; Integrated Security=true";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertSql = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";
                using (SqlCommand command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
