using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBank;
using System.Web.Security;

namespace MVCBank.Controllers
{
    public class ATMsController : Controller
    {
        private BankEntities db = new BankEntities();

        // GET: ATMs
        public ActionResult Index()
        {
            var aTMs = db.ATMs.Include(a => a.A_ATM).Include(a => a.Bank);
            return View(aTMs.ToList());
        }

        // GET: ATMs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATM aTM = db.ATMs.Find(id);
            if (aTM == null)
            {
                return HttpNotFound();
            }
            return View(aTM);
        }

        // GET: ATMs/Create
        public ActionResult Create()
        {
            ViewBag.a_admin_id = new SelectList(db.A_ATM, "a_admin_id", "a_admin_name");
            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name");
            return View();
        }

        // POST: ATMs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "a_branch_id,a_branch_name,a_branch_add,a_admin_id,available_cash,b_branch_id")] ATM aTM)
        {
            if (ModelState.IsValid)
            {
                aTM.a_branch_id = db.ATMs.Max(u => u.a_branch_id) + 1;
                db.ATMs.Add(aTM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.a_admin_id = new SelectList(db.A_ATM, "a_admin_id", "a_admin_name", aTM.a_admin_id);
            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name", aTM.b_branch_id);
            return View(aTM);
        }

        // GET: ATMs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATM aTM = db.ATMs.Find(id);
            if (aTM == null)
            {
                return HttpNotFound();
            }
            ViewBag.a_admin_id = new SelectList(db.A_ATM, "a_admin_id", "a_admin_name", aTM.a_admin_id);
            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name", aTM.b_branch_id);
            return View(aTM);
        }

        // POST: ATMs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "a_branch_id,a_branch_name,a_branch_add,a_admin_id,available_cash,b_branch_id")] ATM aTM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aTM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.a_admin_id = new SelectList(db.A_ATM, "a_admin_id", "a_admin_name", aTM.a_admin_id);
            ViewBag.b_branch_id = new SelectList(db.Banks, "b_branch_id", "b_branch_name", aTM.b_branch_id);
            return View(aTM);
        }

        // GET: ATMs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATM aTM = db.ATMs.Find(id);
            if (aTM == null)
            {
                return HttpNotFound();
            }
            return View(aTM);
        }

        // POST: ATMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ATM aTM = db.ATMs.Find(id);
            db.ATMs.Remove(aTM);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Search()
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem{Text="ATM Branch", Value="1"},
                new SelectListItem{Text="Cash less than", Value="2"},
                new SelectListItem{Text="Cash more than", Value="3"},
                new SelectListItem{Text="Bank Branch", Value="4"}
                };
            ViewBag.dropvalue = new SelectList(list, "Value", "Text", "1");

            return View();
        }
        [HttpPost]
        public ActionResult Search(string tbvalue, string dropvalue)
        {

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem{Text="ATM Branch", Value="1"},
                new SelectListItem{Text="Cash less than", Value="2"},
                new SelectListItem{Text="Cash more than", Value="3"},
                new SelectListItem{Text="Bank Branch", Value="4"}
                };
            ViewBag.dropvalue = new SelectList(list, "Value", "Text", "1");

            if (tbvalue != null)
                switch (dropvalue)
                {
                    case "1":
                        {
                           
                            var customers = from c in db.ATMs
                                            where c.a_branch_name.Contains(tbvalue)
                                            select c;
                            return View(customers);
                        }
                    case "2":
                        {
                            decimal aux = Convert.ToDecimal(tbvalue);
                                var customers = from c in db.ATMs
                                                where c.available_cash < aux
                                                select c;
                                return View(customers);
                            

                        }
                    case "3":
                        {
                            decimal aux = Convert.ToDecimal(tbvalue);
                            var customers = from c in db.ATMs
                                            where c.available_cash > aux
                                            select c;
                            return View(customers);
                        }
                    case "4":
                        {
                            var customers = from c in db.ATMs
                                            where c.Bank.b_branch_name.Contains(tbvalue)
                                            select c;
                            return View(customers);
                        }

                }
            var custdefault = from c in db.Customers
                              select c;
            return View(custdefault);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("ATM_A", "Home");
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
