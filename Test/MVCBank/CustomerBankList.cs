using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MVCBank
{
    [Table("Customers")]
    public partial class CustomerBankList
    {
        
        
        public Customer Name { get; set; }
        
        public Bank Bnk { get; set; }
        public ATM atm { get; set; }
        public IEnumerable<ATM> ListATM { get; set; }
        public Bank_Admin Bank_A {get;set;}
        public IEnumerable<Bank> BankList { get; set; }
        public Transaction Trans { get; set; }
        public IEnumerable<Transaction> ListTrans { get; set; }
        public bool RememberMe { get; internal set; }
    }
}