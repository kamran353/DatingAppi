using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApi.Controllers
{
    public class AdminDashboardController : Controller
    {
        // GET: AdminDashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Matches()
        {
            return View();
        }
        public ActionResult NewAndOnlineUser()
        {
            return View();
        }
        public ActionResult UserProfile()
        {
            return View();
        }

        public ActionResult Subscriptions()
        {
            return View();
        }
        public ActionResult Support()
        {
            return View();
        }
        public ActionResult SignIn()
        {
            return View();
        }
        public ActionResult Demographics()
        {
            return View();
        }
        public ActionResult ViewChat()
        {
            return View();
        }
    }
}