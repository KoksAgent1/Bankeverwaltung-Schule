using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankeverwaltungconsole
{
    internal class Customer
    {
        public string Name { get; set; }
        public List<Account> Accounts { get; set; }

        public Customer(string name)
        {
            Name = name;
            Accounts = new List<Account>();
        }

        public void AddAccount(Account account)
        {
            Accounts.Add(account);
        }
    }

}
