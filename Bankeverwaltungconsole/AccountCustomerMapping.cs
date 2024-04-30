using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankeverwaltungconsole
{
    internal class AccountCustomerMapping
    {
        public string AccountId { get; set; }
        public List<string> CustomerIds { get; set; }

        public AccountCustomerMapping()
        {
            CustomerIds = new List<string>();
        }
    }
}
