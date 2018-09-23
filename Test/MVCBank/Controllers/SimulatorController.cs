using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBank.Controllers
{ 
    public class SimulatorController : Controller
    {
        BankEntities db = new BankEntities();
        // GET: Simulator
        [Authorize]
        [HttpGet]
        public ActionResult Welcome(string card_no, int branch1, string message = "NA")
        {
            var customers = (from c in db.Customers
                             where c.card_no == card_no
                             select c).FirstOrDefault();
            CustomerBankList cust = new CustomerBankList();
            cust.Name = customers;
            cust.Bnk = (from c in db.Banks where c.b_branch_id == branch1 select c).FirstOrDefault();
            ViewBag.Message = message;
            return View(cust);
        }
        public ActionResult Deposit(int cust_id, int branch)
        {

            CustomerBankList cust = new CustomerBankList();
            cust.Name = (from c in db.Customers
                         where c.cust_id == cust_id
                         select c).FirstOrDefault();
            cust.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            cust.ListATM = (from c in db.ATMs where c.b_branch_id == branch select c).ToList();
            return View(cust);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Deposit(int cust_id, int branch, decimal balance, int atm)
        {
            string message;
            CustomerBankList cust = new CustomerBankList();
            cust.Name = (from c in db.Customers
                         where c.cust_id == cust_id
                         select c).FirstOrDefault();
            cust.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            cust.atm = (from c in db.ATMs where c.b_branch_id == branch && c.a_branch_id==atm select c).FirstOrDefault();
            cust.ListATM = (from c in db.ATMs where c.b_branch_id == branch select c).ToList();

            if (balance % 5 != 0)
            {
                ViewBag.Message = "Amount has to be a multiple of 5";
                return View(cust);
            }
            else
            {


                cust.Name.Bal = cust.Name.Bal + balance;
                cust.atm.available_cash = cust.atm.available_cash + balance;
                Transaction tran = new Transaction();
                tran.a_c_no = cust.Name.a_c_no;
                tran.credit = balance;
                tran.debit = 0;
                tran.Date = DateTime.Now;
                tran.total = cust.Name.Bal;
                tran.message = "Deposit of " + tran.credit + " lei at branch " + cust.Bnk.b_branch_name + " and on ATM " + cust.atm.a_branch_name;
                tran.Transaction_ID = db.Transactions.Max(u => u.Transaction_ID) + 1;
                db.Transactions.Add(tran);
            }

            db.SaveChanges();
            message = "You have successfuly deposited " + balance + " lei to this account!";
            return RedirectToAction("Welcome", "Simulator", new { card_no = cust.Name.card_no, branch1 = cust.Bnk.b_branch_id, message = message });
        }


        [HttpGet]
        [Authorize]
        public ActionResult Withdraw(int cust_id, int branch)
        {
            CustomerBankList cust = new CustomerBankList();
            cust.Name = (from c in db.Customers
                         where c.cust_id == cust_id
                         select c).FirstOrDefault();
            cust.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            cust.ListATM = (from c in db.ATMs where c.b_branch_id == branch select c).ToList();
            return View(cust);
        }

        [HttpPost]
        [Authorize]

        public ActionResult Withdraw(int cust_id, int branch, decimal balance, int atm)
        {
            string message;
            CustomerBankList cust = new CustomerBankList();
            cust.Name = (from c in db.Customers
                         where c.cust_id == cust_id
                         select c).FirstOrDefault();
            cust.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            cust.atm = (from c in db.ATMs where c.b_branch_id == branch && c.a_branch_id == atm select c).FirstOrDefault();
            cust.ListATM = (from c in db.ATMs where c.b_branch_id == branch select c).ToList();
            if (balance % 5 != 0)
            {
                ViewBag.Message = "Amount has to be a multiple of 5";
                return View(cust);
            }
            else
            if (cust.atm.available_cash >= balance && cust.Name.Bal >= balance)
            {
                message= "You have withdrawn " + balance + " lei from this account!";
                cust.Name.Bal = cust.Name.Bal - balance;

                cust.atm.available_cash = cust.atm.available_cash - balance;

                Transaction tran = new Transaction();
                tran.a_c_no = cust.Name.a_c_no;
                tran.credit = 0;
                tran.debit = balance;
                tran.Date = DateTime.Now;
                tran.total = cust.Name.Bal;
                tran.message = "Withdrawal of " + tran.debit + " lei at branch " + cust.Bnk.b_branch_name + " and on ATM " + cust.atm.a_branch_name;
                tran.Transaction_ID = db.Transactions.Max(u => u.Transaction_ID) + 1;

                db.Transactions.Add(tran);
                db.SaveChanges();
                return RedirectToAction("Welcome", "Simulator", new { card_no = cust.Name.card_no, branch1 = cust.Bnk.b_branch_id, message = message });
            }
            else
            if (cust.Name.Bal < balance)
            {
                ViewBag.Message= "Current balance is too low to withdraw that amount!";
                return View(cust);
            }
                
            else
                {
                ViewBag.Message = "ATM is out of cash, please come back later!";
                return View(cust);
            }
            }

        
        [HttpGet]
        [Authorize]

        public ActionResult Transfer(int cust_id, int branch)
        {
            CustomerBankList cust = new CustomerBankList();
            cust.Name = (from c in db.Customers
                         where c.cust_id == cust_id
                         select c).FirstOrDefault();
            cust.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            cust.atm = (from c in db.ATMs where c.b_branch_id == branch select c).FirstOrDefault();
            return View(cust);
        }
        [HttpPost]
        [Authorize]

        public ActionResult Transfer(int cust_id, int branch, string IBAN, decimal amount)
        {
            string message = string.Empty;
            CustomerBankList currentcustomer = new CustomerBankList();
            currentcustomer.Name = (from c in db.Customers
                         where c.cust_id == cust_id
                         select c).FirstOrDefault();
            currentcustomer.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            currentcustomer.atm = (from c in db.ATMs where c.b_branch_id == branch select c).FirstOrDefault();
            var targetcustomer = (from d in db.Customers
                                  where d.a_c_no == IBAN
                                  select d).SingleOrDefault();

            if (currentcustomer.Name.Bal < amount)
            {
                ViewBag.Message = "Current balance is too low to withdraw that amount!";
                return View(currentcustomer);
            }
            else
            if (targetcustomer == null)
            {
                ViewBag.Message = "IBAN is incorrect or no longer exists!";
                return View(currentcustomer);
            }
            else
            if(currentcustomer.Name.cust_id==targetcustomer.cust_id)
            {
                ViewBag.Message = "You cannot transfer to your active account!";
                return View(currentcustomer);
            }
            
            else
            if (currentcustomer.Name.Bal >= amount &&targetcustomer!=null)
            {
                currentcustomer.Name.Bal = currentcustomer.Name.Bal - amount;
                db.SaveChanges();

                Transaction trancurrent = new Transaction();
                trancurrent.a_c_no = currentcustomer.Name.a_c_no;
                trancurrent.credit = 0;
                trancurrent.debit = amount;
                trancurrent.Date = DateTime.Now;
                trancurrent.total = currentcustomer.Name.Bal;
                trancurrent.message = "Transferred " + trancurrent.debit + " lei at branch " + currentcustomer.Bnk.b_branch_name + " to " + targetcustomer.a_c_no;
                trancurrent.Transaction_ID = db.Transactions.Max(u => u.Transaction_ID) + 1;
                db.Transactions.Add(trancurrent);
                db.SaveChanges();

                targetcustomer.Bal = targetcustomer.Bal + amount;

                Transaction trantarget = new Transaction();
                trantarget.a_c_no = targetcustomer.a_c_no;
                trantarget.credit = amount;
                trantarget.debit = 0;
                trantarget.Date = DateTime.Now;
                trantarget.total = targetcustomer.Bal;
                trantarget.message = "Received " + trantarget.credit + " lei from " + currentcustomer.Name.a_c_no;
                trantarget.Transaction_ID = db.Transactions.Max(u => u.Transaction_ID) + 1;

                
                
                db.Transactions.Add(trantarget);
                db.SaveChanges();
                message = "You have successfully transferred " + amount + " lei to " + targetcustomer.a_c_no;
            }
            else
                message = "An error has occured in the system!";
            return RedirectToAction("Welcome", "Simulator", new { card_no = currentcustomer.Name.card_no, branch1 = currentcustomer.Name.b_branch_id, message=message });
        }
        public ActionResult ChangePin(int cust_id, int branch)
        {
            CustomerBankList user = new CustomerBankList();
                user.Name= (from c in db.Customers
                             where c.cust_id == cust_id
                             select c).FirstOrDefault();
            user.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            user.atm = (from c in db.ATMs where c.b_branch_id == branch select c).FirstOrDefault();
            return View(user);
        }
        [HttpPost]
        public ActionResult ChangePin(int cust_id, string oldpin, string newpin, string confirmPin, int branch)
        {
            string message;
            CustomerBankList user = new CustomerBankList();
            user.Name = (from c in db.Customers
                         where c.cust_id == cust_id
                         select c).FirstOrDefault();
            user.Bnk = (from c in db.Banks where c.b_branch_id == branch select c).FirstOrDefault();
            user.atm = (from c in db.ATMs where c.b_branch_id == branch select c).FirstOrDefault();
            if (user.Name.card_pin != oldpin)
            {
                ViewBag.Message = "Old PIN is incorrect";
                return View(user);
            }
            else
            if(newpin != confirmPin)
            {
                ViewBag.Message = "Old PIN and new PIN do not match!";
                return View(user);
            }
            else
            {
                message = "You have successfully changed your PIN code!";
                user.Name.card_pin = newpin;
                db.SaveChanges();
                return RedirectToAction("Welcome", "Simulator", new { card_no = user.Name.card_no, branch1 = user.Bnk.b_branch_id, message = message });
            }
            
        }


    }

}
    
