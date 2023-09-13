using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cSharp_BankSystemUsingSQLServer
{
    internal class Transactions
    {
        public int TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public int SourceAccountNumber { get; set; }
        public int TargetAccountNumber { get; set; }

        

        public Transactions(int transactionId, DateTime timestamp, TransactionType type, decimal amount, int sourceAccountNumber, int targetAccountNumber)
        {
            TransactionId = transactionId;
            Timestamp = timestamp;
            Type = type;
            Amount = amount;
            SourceAccountNumber = sourceAccountNumber;
            TargetAccountNumber = targetAccountNumber;
        }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }
}
}
