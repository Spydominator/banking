//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCBank
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public int cust_id { get; set; }
        public string cust_name { get; set; }
        public System.DateTime Dob { get; set; }
        public string contact_no { get; set; }
        public string cust_add { get; set; }
        public string a_c_no { get; set; }
        public string a_c_type { get; set; }
        public Nullable<decimal> Bal { get; set; }
        public string card_no { get; set; }
        public string card_pin { get; set; }
        public int b_branch_id { get; set; }
    
        public virtual Bank Bank { get; set; }
    }
}
