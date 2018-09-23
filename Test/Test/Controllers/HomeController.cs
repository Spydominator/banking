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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Admin()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Simulator()
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
                    message = "Username and/or password is incorrect.";
                    break;

                case 0:
                    FormsAuthentication.SetAuthCookie(user.Name.card_no, user.RememberMe);
                    return RedirectToAction("Welcome", "Home", new { card_no = user.Name.card_no, branch1 = user.Bnk.b_branch_id });
            }
            ViewBag.Message = message;
            return View(user);
        }
    }
}