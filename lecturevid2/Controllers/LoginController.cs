using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lecturevid2.Models;
using lecturevid2.DataAccess;
using System.Data;
//for session
using Microsoft.AspNetCore.Http;

namespace lecturevid2.Controllers
{
 
    public class LoginController : Controller
    {
        private readonly DatabaseHelper _databaseHelper;

        public LoginController()
        {
            string connString = "Server=localhost;Database=ipt;User Id=root;Password=''";
            _databaseHelper = new DatabaseHelper(connString);
        }
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                string checkQuery = $"SELECT * FROM users WHERE user_email='{user.user_email}' ";
                DataTable dt = _databaseHelper.SelectQuery(checkQuery);
                if (dt.Rows.Count > 0)
                {
                    ModelState.AddModelError("", "Email alreay exist");
                    return View("user");
                    
                }
                string query = $"INSERT into users (user_fullname,user_email,user_pwd) VALUES ('{user.user_fullname}','{user.user_email}','{user.user_pwd}')";
                int result = _databaseHelper.ExecuteQuery(query);


                if (result > 0)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Registration  failed.");
                }
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                return RedirectToAction("Index", "Product");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login lg)
        {
            if (ModelState.IsValid)
            {
                string query = $"SELECT * FROM users WHERE user_email='{lg.user_email}'";
                DataTable dt = _databaseHelper.SelectQuery(query);
                if (dt.Rows.Count > 0)
                {
                    string storedPwd = dt.Rows[0]["user_pwd"].ToString();
                    if (lg.user_pwd == storedPwd)
                    {
                        HttpContext.Session.SetString("UserEmail", lg.user_email.ToString());
                        return RedirectToAction("Index","Product");
                    }
                    else
                    {
                        ModelState.AddModelError("","Incorrect email or Password");
                    }
                }
                else
                {
                    return Content("User not Found");
                }
                

            } 
            return View(lg);
        }

    }
}
