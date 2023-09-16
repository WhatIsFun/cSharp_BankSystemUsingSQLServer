using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace cSharp_BankSystemUsingSQLServer
{ 
    public class Transaction
    {
        
        public int TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public int SourceAccountNumber { get; set; }
        public int TargetAccountNumber { get; set; }
        private static string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
        
        Account targetAccount;
        public Transaction()
        {
            Timestamp = DateTime.Now;
        }
        

        public void transactionMenu(List<Account> userAccounts, User authenticatedUser)
        {
            ProfilePage profilePage = new ProfilePage();
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
                        deposit(userAccounts);
                        break;
                    case "2":
                        withdraw(userAccounts);
                        break;
                    case "3":
                        transfer(userAccounts);
                        break;
                    case "4":
                        Console.Clear();
                        profilePage.profileMenu(authenticatedUser, userAccounts);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.Clear();
            }
        }
        void deposit(List<Account> userAccounts)
        {
            Console.Write("Enter the account number to deposit into: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNum))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }
            if (userAccounts.Count == 0)
            {
                Console.WriteLine("There is no accounts.");
                return;
            }

            Console.Write("Enter the amount to deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid deposit amount.");
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))   
            {
                try
                {
                    connection.Open();
                    int type = (int)TransactionType.Deposit;

                    string Updatebalance = "Update Account set Balance = Balance + @Amount where Account_Id = @Account_Id;";
                    SqlCommand Command = new SqlCommand(Updatebalance, connection);
                    Command.Parameters.AddWithValue("@Amount", amount);
                    Command.Parameters.AddWithValue("@Account_Id", accountNum);

                    int returns = Command.ExecuteNonQuery();

                    int rowaffected = Command.ExecuteNonQuery();
                    if (rowaffected > 0)
                    {

                        using (SqlCommand command = new SqlCommand("insert into transactions (Timestamp, Type, Amount, SourceAccountId, TargetAccountId, Account_Id) values (@Timestamp, @Type, @Amount,@SourceAccountId,@TargetAccountId,@Account_Id);"))
                        {
                            command.Parameters.AddWithValue("@Timestamp", Timestamp);
                            command.Parameters.AddWithValue("@Type", type);
                            command.Parameters.AddWithValue("@Amount", amount);
                            command.Parameters.AddWithValue("@SourceAccountId", accountNum);
                            command.Parameters.AddWithValue("@TargetAccountId", accountNum);
                            command.Parameters.AddWithValue("@Account_Id", accountNum);

                            Console.WriteLine("Depositing successful");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid withdrawal amount or insufficient funds.");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }

            }
            return;
        }
        void withdraw(List<Account> userAccounts)
        {
            Console.Write("Enter the account number to withdraw from: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNum))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (userAccounts.Count == 0)
            {
                Console.WriteLine("There is no accounts.");
                return;
            }
            Console.Write("Enter the amount to withdraw: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid  amount.");
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    int type = (int)TransactionType.Withdrawal;

                    string Updatebalance = "Update Account set Balance = Balance - @Amount where Account_Id = @Account_Id;";
                    SqlCommand Command = new SqlCommand(Updatebalance, connection);
                    Command.Parameters.AddWithValue("@Amount", amount);
                    Command.Parameters.AddWithValue("@Account_Id", accountNum);

                    int returns = Command.ExecuteNonQuery();

                    int rowaffected = Command.ExecuteNonQuery();
                    if (rowaffected > 0)
                    {

                        using (SqlCommand command = new SqlCommand("insert into transactions (Timestamp, Type, Amount, SourceAccountId, TargetAccountId, Account_Id) values (@Timestamp, @Type, @Amount,@SourceAccountId,@TargetAccountId,@Account_Id);"))
                        {
                            command.Parameters.AddWithValue("@Timestamp", Timestamp);
                            command.Parameters.AddWithValue("@Type", type);
                            command.Parameters.AddWithValue("@Amount", amount);
                            command.Parameters.AddWithValue("@SourceAccountId", accountNum);
                            command.Parameters.AddWithValue("@TargetAccountId", accountNum);
                            command.Parameters.AddWithValue("@Account_Id", accountNum);

                            Console.WriteLine("Withdrawing successful");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid withdrawal amount or insufficient funds.");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
                return;
            }
        }
        void transfer(List<Account> userAccounts)
        {
            Console.Write("Enter the account number to transfer from: ");
            if (!int.TryParse(Console.ReadLine(), out int sourceAccountId))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (userAccounts.Count == 0)
            {
                Console.WriteLine("Add account first");
                return;
            } else if (userAccounts.Any(account => account.AccountId != sourceAccountId))
            {
                Console.WriteLine("Enter a valid account number ");
                return;
            }
            else
            {
                Console.Write("Enter the account number want to transfer to: ");
                if (!int.TryParse(Console.ReadLine(), out int targetAccountId))
                {
                    Console.WriteLine("Invalid target account number.");
                    return;
                }

                if (GetTargetAccount(targetAccountId) == null)
                {
                    Console.WriteLine("Target account not found.");
                    return;
                }
                else
                {
                    Console.Write("Enter the amount to transfer: ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                    {
                        Console.WriteLine("Invalid transfer amount.");
                        return;
                    }
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            int type = (int)TransactionType.Transfer;
                            //string DateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            int targetAccountid = targetAccount.AccountId;
                            // Source
                            string UpdateSourBalance = "Update Account set Balance = Balance - @Amount where Balance>@Amount and Account_Id = @Account_Id;";
                            SqlCommand Command = new SqlCommand(UpdateSourBalance, connection);
                            Command.Parameters.AddWithValue("@Amount", amount);
                            Command.Parameters.AddWithValue("@Account_Id", sourceAccountId);

                            Console.WriteLine("Transering successful");
                            int rowaffected = Command.ExecuteNonQuery();
                            if (rowaffected > 0)
                            {

                                using (SqlCommand command = new SqlCommand("insert into transactions (Timestamp, Type, Amount, SourceAccountId, TargetAccountId, Account_Id) values (@Timestamp, @Type, @Amount,@SourceAccountId,@TargetAccountId,@Account_Id);"))
                                {
                                    command.Parameters.AddWithValue("@Timestamp", Timestamp);
                                    command.Parameters.AddWithValue("@Type", type);
                                    command.Parameters.AddWithValue("@Amount", amount);
                                    command.Parameters.AddWithValue("@SourceAccountId", sourceAccountId);
                                    command.Parameters.AddWithValue("@TargetAccountId", targetAccountid);
                                    command.Parameters.AddWithValue("@Account_Id", sourceAccountId);


                                    return;
                                }
                            }
                            // Target 
                            string UpdateTargBalance = "Update Account set Balance = Balance + @Amount where Account_Id = @Account_Id;";
                            SqlCommand Command1 = new SqlCommand(UpdateTargBalance, connection);
                            Command.Parameters.AddWithValue("@Amount", amount);
                            Command.Parameters.AddWithValue("@Account_Id", sourceAccountId);

                            
                            int rowaffected1 = Command1.ExecuteNonQuery();
                            if (rowaffected1 > 0)
                            {

                                using (SqlCommand command = new SqlCommand("insert into transactions (Timestamp, Type, Amount, SourceAccountId, TargetAccountId, Account_Id) values (@Timestamp, @Type, @Amount,@SourceAccountId,@TargetAccountId,@Account_Id);"))
                                {
                                    command.Parameters.AddWithValue("@Timestamp", Timestamp);
                                    command.Parameters.AddWithValue("@Type", type);
                                    command.Parameters.AddWithValue("@Amount", amount);
                                    command.Parameters.AddWithValue("@SourceAccountId", sourceAccountId);
                                    command.Parameters.AddWithValue("@TargetAccountId", targetAccountid);
                                    command.Parameters.AddWithValue("@Account_Id", sourceAccountId);


                                    return;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid withdrawal amount or insufficient funds.");
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                        return;
                    }
                }
            }
        }
        private static List<Account> GetTargetAccount(int targetAccountId)
        {
            List<Account> targetAccount = new List<Account>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string selectSql = "SELECT Account_Id, AccountHolderName FROM Account WHERE Account_Id = @Account_Id";
                    using (SqlCommand command = new SqlCommand(selectSql, connection))
                    {
                        command.Parameters.AddWithValue("@Account_Id", targetAccountId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                targetAccount.Add(new Account
                                {
                                    AccountId = Convert.ToInt32(reader["Account_Id"]),
                                    AccountHolderName = reader["AccountHolderName"].ToString()
                                });
                            }
                            return targetAccount;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void history(List <Account> userAccounts, User authenticatedUser)
        {
            Console.WriteLine("Transaction History\n\n");
            Console.Write("Enter the account number to show the history: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNum))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }
            if (userAccounts.Count == 0)
            {
                Console.WriteLine("Add account first");
                return;
            }
            else if (userAccounts.Any(account => account.AccountId != accountNum))
            {
                Console.WriteLine("Enter a valid account number ");
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1) Last transaction \n2) Last (5) transactions\n3) Last day transactions\n4) Last month transactions\n5) Go Back\n\n\n If you want more than one month please contact or visit the nearest branch");
                Console.ResetColor();
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowTransactionHistory(accountNum, TransactionFilter.LastTransaction);
                        break;

                    case "2":
                        ShowTransactionHistory(accountNum, TransactionFilter.Last5Transactions);
                        break;

                    case "3":
                        ShowTransactionHistory(accountNum, TransactionFilter.LastDay);
                        break;

                    case "4":
                        ShowTransactionHistory(accountNum, TransactionFilter.LastMonth);
                        break;

                    case "5":
                        ProfilePage profilePage = new ProfilePage();
                        Console.Clear();
                        profilePage.profileMenu(authenticatedUser, userAccounts);
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
        private void ShowTransactionHistory(int accountNum, TransactionFilter filter)
        {
            Transaction transaction = new Transaction();
            List<Transaction> transactionHistory = transaction.GetTransactionHistory(accountNum, filter);

            // Display the transaction history
            Console.WriteLine("Transaction History:");
            foreach (var transactions in transactionHistory)
            {
                Console.WriteLine($"Transaction ID: {transactions.TransactionId}");
                Console.WriteLine($"Timestamp: {transactions.Timestamp}");
                Console.WriteLine($"Type: {transactions.Type}");
                Console.WriteLine($"Amount: {transactions.Amount}");
                Console.WriteLine($"Source Account: {transactions.SourceAccountNumber}");
                Console.WriteLine($"Target Account: {transactions.TargetAccountNumber}");
                Console.WriteLine("------------------------------");
            }
        }
        public List<Transaction> GetTransactionHistory(int accountNum, TransactionFilter filter)
        {
            List<Transaction> transactionHistory = new List<Transaction>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                        connection.Open();
                        string selectSql = "";

                        switch (filter)
                        {
                            case TransactionFilter.LastTransaction:
                                
                                selectSql = "SELECT TOP 1 * FROM Transactions WHERE (SourceAccountId = @AccountNum OR TargetAccountId = @AccountNum) ORDER BY Timestamp DESC";
                                break;

                            case TransactionFilter.Last5Transactions:
                               
                                selectSql = "SELECT TOP 5 * FROM Transactions WHERE (SourceAccountId = @AccountNum OR TargetAccountId = @AccountNum) ORDER BY Timestamp DESC";
                                break;

                            case TransactionFilter.LastDay:
                                
                                selectSql = "SELECT * FROM Transactions WHERE (SourceAccountId = @AccountNum OR TargetAccountId = @AccountNum) AND DATEDIFF(DAY, Timestamp, GETDATE()) = 0";
                                break;

                            case TransactionFilter.LastMonth:
                                
                                selectSql = "SELECT * FROM Transactions WHERE (SourceAccountId = @AccountNum OR TargetAccountId = @AccountNum) AND DATEDIFF(MONTH, Timestamp, GETDATE()) = 0";
                                break;
                        }

                        using (SqlCommand command = new SqlCommand(selectSql, connection))
                        {
                            command.Parameters.AddWithValue("@AccountNum", accountNum);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Transaction transaction = new Transaction
                                    {
                                        TransactionId = Convert.ToInt32(reader["TransactionId"]),
                                        Timestamp = Convert.ToDateTime(reader["Timestamp"]),
                                        Type = (TransactionType)Convert.ToInt32(reader["Type"]),
                                        Amount = Convert.ToDecimal(reader["Amount"]),
                                        SourceAccountNumber = Convert.ToInt32(reader["SourceAccountId"]),
                                        TargetAccountNumber = Convert.ToInt32(reader["TargetAccountId"])
                                    };

                                    transactionHistory.Add(transaction);
                                }
                            }
                        }
                    return transactionHistory;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new List<Transaction>();
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }
    public enum TransactionFilter
    {
        LastTransaction,
        Last5Transactions,
        LastDay,
        LastMonth
    }
}
