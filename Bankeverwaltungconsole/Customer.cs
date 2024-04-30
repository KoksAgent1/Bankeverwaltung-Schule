using System.Collections.Generic;

namespace Bankeverwaltungconsole
{
    internal class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Customer(string name,string Id)
        {
            Name = name;
            this.Id = Id;
        }
    }

}
