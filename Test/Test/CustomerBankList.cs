using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCBank
{
    public partial class CustomerBankList
    {
        public Customer Name { get; set; }
        public Bank Bnk { get; set; }
        public ATM atm { get; set; }
        public Bank_Admin Bank_A {get;set;}
        public IEnumerable<Bank> BankList { get; set; }
        public bool RememberMe { get; internal set; }
    }
}