using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cSharp_BankSystemUsingSQLServer
{ 
    public class Transaction
    {
        User user = new User();
        ProfilePage profilePage = new ProfilePage();
        public int TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public int SourceAccountNumber { get; set; }
        public int TargetAccountNumber { get; set; }
        private static string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

        //public Transaction(TransactionType transactionType, decimal amount, int sourceAccountNumber, int targetAccountNumber)
        //{
        //    Type = transactionType;
        //    Amount = amount;
        //    Timestamp = DateTime.Now;
        //    SourceAccountNumber = sourceAccountNumber;
        //    TargetAccountNumber = targetAccountNumber;

        //}

        public void transactionMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("$$$ $$ $ Transaction $ $$ $$$\n");
            Console.ResetColor();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1) Deposite\n2) Withdraw\n3) Transfer Money\n4) Go Back\n");
                Console.ResetColor();
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        deposit();
                        break;
                    case "2":
                        withdraw();
                        break;
                    case "3":
                        transfer();
                        break;
                    case "4":
                        profilePage.profileMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.Clear();
            }
        }
        void deposit()
        {

        }
        void withdraw()
        {

        }
        void transfer()
        {

        }
        void transactionHistory()
        {

        }
        //public void ExecuteTransaction()
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SqlTransaction transaction = connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                // Retrieve the current account balance
        //                decimal currentBalance = GetAccountBalance(accountId, connection, transaction);

        //                // Perform the specified transaction operation
        //                switch (TransactionType)
        //                {
        //                    case TransactionType.Deposit:
        //                        currentBalance += Amount;
        //                        break;
        //                    case TransactionType.Withdrawal:
        //                        if (Amount > currentBalance)
        //                        {
        //                            throw new InvalidOperationException("Insufficient funds.");
        //                        }
        //                        currentBalance -= Amount;
        //                        break;
        //                    case TransactionType.Transfer:
        //                        if (Amount > currentBalance)
        //                        {
        //                            throw new InvalidOperationException("Insufficient funds.");
        //                        }
        //                        // For simplicity, this assumes you provide a targetAccountId as well
        //                        int targetAccountId = GetTargetAccountId(); // Implement a method to get the target account ID
        //                        currentBalance -= Amount;
        //                        UpdateAccountBalance(targetAccountId, Amount, connection, transaction);
        //                        break;
        //                    default:
        //                        throw new InvalidOperationException("Invalid transaction type.");
        //                }

        //                // Update the account balance in the database
        //                UpdateAccountBalance(AccountId, currentBalance, connection, transaction);

        //                // Log the transaction
        //                LogTransaction(connection, transaction);

        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                throw ex;
        //            }
        //        }
        //    }
        //}
        private decimal GetAccountBalance(int accountId, SqlConnection connection, SqlTransaction transaction)
        {
            // Implement code to retrieve the account balance from the database
            decimal currentBalance = 0; // Replace with actual retrieval logic
            return currentBalance;
        }

        private void UpdateAccountBalance(int accountId, decimal newBalance, SqlConnection connection, SqlTransaction transaction)
        {
            // Implement code to update the account balance in the database
            // You should use a SQL UPDATE statement here
        }

        private void LogTransaction(SqlConnection connection, SqlTransaction transaction)
        {
            
        }

        private int GetTargetAccountId()
        {
            // Implement a method to get the target account ID (for transfers)
            // This can be user input or fetched from your application logic
            return 2; // Replace with actual target account ID
        }
    }
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }

}
