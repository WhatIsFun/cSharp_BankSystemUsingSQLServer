using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cSharp_BankSystemUsingSQLServer
{
    public class Account
    {
        User User = new User();
        public int AccountId { get; set; }
        public string AccountHolderName { get; set; }
        public decimal Balance { get; set; }
    }
}
