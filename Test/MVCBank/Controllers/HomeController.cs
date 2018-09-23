using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCBank.Controllers
{
    public class HomeController : Controller
    {
        
        private BankEntities db = new BankEntities();
        public ActionResult Index()
        {
            return View();
        }

     
        public ActionResult Simulator(int? value=null)
        {
            CustomerBankList model = new CustomerBankList();
            model.BankList = db.Banks.ToList();
            ViewBag.Branches = new SelectList(db.Banks, "b_branch_id", "b_branch_name");
            return View(model);
           
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Simulator(CustomerBankList user)
        {
            BankEntities usersEntities = new BankEntities();
            int? userId = usersEntities.VALIDATE_USER(user.Name.card_no.ToString(), user.Name.card_pin.ToString()).FirstOrDefault();
            string card_number = user.Name.card_no;
            int branch = (int)user.Bnk.b_branch_id;
            string message = string.Empty;
            switch (userId.Value)
            {
                case -1:
                    {
                        message = "Card Number or PIN is incorrect";
                        user.BankList = db.Banks.ToList();
                        break;
                    }

                case 0:

                    FormsAuthentication.SetAuthCookie(user.Name.card_no, user.RememberMe);
                    return RedirectToAction("Welcome", "Simulator", new { card_no = user.Name.card_no, branch1 = user.Bnk.b_branch_id });
            }
            ViewBag.Message = message;
            return View(user);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Simulator");
        }

        public ActionResult Admin()
        {
 

            return View();
        }
        [HttpPost]
        public ActionResult Admin(Bank_Admin admin)
        {
            BankEntities usersEntities = new BankEntities();
            if(admin.b_admin_name ==null)
                ModelState.AddModelError(admin.b_admin_name, "The Username field cannot be empty");
            if (admin.b_admin_pin.ToString() == null)
                ModelState.AddModelError(admin.b_admin_pin.ToString(), "The PIN field cannot be empty");
            int? userId = usersEntities.VALIDATE_ADMIN(admin.b_admin_name.ToString(), admin.b_admin_pin.ToString()).FirstOrDefault();

            string message = string.Empty;
            switch (userId.Value)
            {
                case -1:
                    message = "Username or password incorrect";
                    break;

                case 0:
                    FormsAuthentication.SetAuthCookie(admin.b_admin_name, admin.RememberMe);
                    return RedirectToAction("Index", "Admin", admin);
            }
            ViewBag.Message = message;

            return View(admin);
        }

        public ActionResult ATM_A()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ATM_A(A_ATM user)
        {
            BankEntities usersEntities = new BankEntities();
            int? userId = usersEntities.VALIDATE_A_ADMIN(user.a_admin_name.ToString(), user.a_admin_pin.ToString()).FirstOrDefault();

            string message = string.Empty;
            switch (userId.Value)
            {
                case -1:
                    message = "Username or password is incorrect";
                    break;

                case 0:
                    FormsAuthentication.SetAuthCookie(user.a_admin_name, user.RememberMe);
                    return RedirectToAction("Index", "ATMs", user);
            }
            ViewBag.Message = message;

            return View(user);
        }


    }
}