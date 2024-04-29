using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankeverwaltungconsole
{
    internal class Bankingmanger
    {
        public enum AccountType
        {
            girokonto,
            sparkonto,
            tagesgeldkonto,
            kreditkonto,
        }

        public enum Transaktiontype
        {
            Deposit,
            Withdrawal,
            Transfer,
        }
    }
}
