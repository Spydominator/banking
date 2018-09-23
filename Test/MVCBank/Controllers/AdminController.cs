using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBank;
using System.Text;
using System.Web.Security;

namespace MVCBank.Controllers
{
    public class AdminController : Controller
    {
        private BankEntities db = new BankEntities();
        private static string GetLuhnCheckDigit(string number)
        {
            var sum = 0;
            var alt = true;
            var digits = number.ToCharArray();
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                var curDigit = (digits[i] - 48);
                if (alt)
                {
                    curDigit *= 2;
                    if (curDigit > 9)
                        curDigit -= 9;
                }
                sum += curDigit;
                alt = !alt;
            }
            if ((sum % 10) == 0)
            {
                return "0";
            }
            return (10 - (sum % 10)).ToString();
        }
        public static string generate_card(Customer user, string acc_gen)
        {
            String first_digit, two_to_four, eight_to_fifteen, five_to_seven, sixteen, card;
            Random rnd = new Random();
            first_digit = "4";  //one digit
            switch (user.b_branch_id.ToString().Length)
            {
                case 1:
                    {
                        two_to_four = "00" + user.b_branch_id.ToString();
                        break;
                    }
                case 2:
                    {
                        two_to_four = "0" + user.b_branch_id.ToString();
                        break;
                    }
                default:
                    {
                        two_to_four = user.b_branch_id.ToString();
                        break;
                    }
            }
                    five_to_seven = rnd.Next(100, 999).ToString(); //3 digits 
                    eight_to_fifteen = acc_gen;  //8 digits
                    card = first_digit + two_to_four + five_to_seven + eight_to_fifteen;
                    sixteen = GetLuhnCheckDigit(card);
                    card = card + sixteen;
                    return card;
            
        }

        [Authorize]
        // GET: Admin
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.Bank);
            return View(customers.ToList());
        }

        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Admin/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name");
            ViewBag.a_c_type = new SelectList(
            new List<SelectListItem>
            {
                new SelectListItem{Selected=true, Text="debit",Value="debit"},
                new SelectListItem{Selected=false, Text="credit", Value="credit"}
            },"Value", "Text");
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cust_id,cust_name,Dob,contact_no,cust_add,a_c_no,a_c_type,card_no,b_branch_id")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                Random rnd = new Random();
                string acc_gen = rnd.Next(10000000, 99999999).ToString();
                int intIdt = db.Customers.Max(u => u.cust_id) + 1; 
                customer.cust_id=intIdt;
                customer.Bal = 0; //starts at 0
                while(db.Customers.Any(o =>o.a_c_no==acc_gen))
                {
                    acc_gen = rnd.Next(10000000, 99999999).ToString();
                
                }
                customer.a_c_no = "RO52" + "BANK" + "00009999" + acc_gen;
                customer.card_no = generate_card(customer, acc_gen);
                customer.card_pin = rnd.Next(1000, 9999).ToString(); //has to be random
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name", customer.b_branch_id);
            //ViewBag.acc_type = new SelectList(new SelectListItem { Text = "debit", Value = "debit" },
            //                                      new SelectListItem { Text = "credit", Value = "credit" });
            ViewBag.a_c_type = new SelectList(
            new List<SelectListItem>
            {
                new SelectListItem{Selected=true, Text="debit",Value="debit"},
                new SelectListItem{Selected=false, Text="credit", Value="credit"}
            }, "Value", "Text");
            return View(customer);
        }
        // GET: Admin/Search
        [Authorize]
        public ActionResult Search()
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem{Text="Name", Value="1"},
                new SelectListItem{Text="IBAN", Value="2"},
                new SelectListItem{Text="Branch", Value="3"},
                new SelectListItem{Text="Address", Value="4"},
                new SelectListItem{Text="Phone", Value="5"}
                };
            ViewBag.dropvalue = new SelectList(list, "Value", "Text", "1");

            return View();
        }
        [HttpPost]
        public ActionResult Search(string tbvalue, string dropvalue)
        {

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem{Text="Name", Value="1"},
                new SelectListItem{Text="IBAN", Value="2"},
                new SelectListItem{Text="Branch", Value="3"},
                new SelectListItem{Text="Address", Value="4"},
                new SelectListItem{Text="Phone", Value="5"}
                };
            ViewBag.dropvalue = new SelectList(list, "Value", "Text", "1");

            if (tbvalue != null)
                switch (dropvalue)
                {
                    case "1":
                        {
                            var customers = from c in db.Customers
                                            where c.cust_name.Contains(tbvalue)
                                            select c;
                            return View(customers);
                        }
                    case "2":
                        {
                            var customers = from c in db.Customers
                                            where c.a_c_no == tbvalue
                                            select c;
                            return View(customers);
                        }
                    case "3":
                        {
                            var customers = from c in db.Customers
                                            where c.Bank.b_branch_name.Contains(tbvalue)
                                            select c;
                            return View(customers);
                        }
                    case "4":
                        {
                            var customers = from c in db.Customers
                                            where c.cust_add.Contains(tbvalue)
                                            select c;
                            return View(customers);
                        }
                    case "5":
                        {
                            var customers = from c in db.Customers
                                            where c.contact_no == tbvalue
                                            select c;
                            return View(customers);
                        }
                }
            var custdefault = from c in db.Customers
                            select c;
            return View(custdefault);
        }
        // GET: Admin/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> list = new List<SelectListItem>
            {

                new SelectListItem{Text ="debit", Value ="debit"},
                new SelectListItem{Text="credit", Value="credit"}
            };
            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name", customer.b_branch_id);
            ViewBag.a_c_type = new SelectList(list, "Value", "Text", "debit");
            return View(customer);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cust_id,cust_name,Dob,contact_no,cust_add,a_c_no,a_c_type,Bal,card_no,card_pin,b_branch_id")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            bool select1, select2;
            if (customer.a_c_type == "debit")
            {
                select1 = true;
                select2 = false;
            }
            else
            {
                select1 = false;
                select2 = true;
            }
            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name", customer.b_branch_id);
            ViewBag.a_c_type = new SelectList(
            new List<SelectListItem>
            {
                new SelectListItem{Selected=select1, Text="debit",Value="debit"},
                new SelectListItem{Selected=select2, Text="credit", Value="credit"}
            }, "Value", "Text");
            return View(customer);
        }

        // GET: Admin/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult History(string acc_no)
        {
            var transaction = (from c in db.Transactions where c.a_c_no==acc_no select c);
            
            return View(transaction);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Admin","Home");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
