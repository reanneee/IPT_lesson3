using lecturevid2.DataAccess;
using lecturevid2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace lecturevid2.Controllers
{
    public class CustomerController : Controller
    {
        private readonly string _xmlpath;
        private readonly DatabaseHelper _databaseHelper;
        public CustomerController(IWebHostEnvironment _webHostEnvironment)
        {
            _xmlpath = Path.Combine(_webHostEnvironment.WebRootPath, "Schemas", "Customers.xml");
            string _connstring = "Server=localhost;Database=ipt;User Id=root;Password='';Port=3306;";
            _databaseHelper = new DatabaseHelper(_connstring);
        }


        public IActionResult ImportXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_xmlpath);
            XmlNodeList customers = doc.SelectNodes("/Customers/Customer");
            foreach(XmlNode node in customers)
            {
                int id = int.Parse(node["id"].InnerText);
                string fullname = node["firstname"].InnerText + " " + node["lastname"].InnerText;
                string email = node["email"].InnerText;

                string query = $@"INSERT INTO customers (cust_id,fullname,email) VALUES ({id},'{fullname}','{email}')
                   ON DUPLICATE KEY UPDATE fullname='{fullname}',email='{email}'";
                _databaseHelper.ExecuteQuery(query);
            }
            return Content("meron na");
        }

        public IActionResult ShowData()
        {
            string query = "SELECT * FROM customers";

            List<object> customers = new List<object>();
            DataTable dt=_databaseHelper.SelectQuery(query);
           foreach (DataRow row in dt.Rows)
            {
                customers.Add(
                    new {   id=row["cust_id"],
                            fullname=row["fullname"],
                            email=row["email"],
                        });
            }
            return Json(customers);
            /* return View(customers);*/
        }

        public IActionResult Index()
        {
            string query = "SELECT * FROM customers";

            List<CustomerModel> customers = new List<CustomerModel>();
            DataTable dt = _databaseHelper.SelectQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                customers.Add(
                    new CustomerModel
                    {
                        cust_id = int.Parse(row["cust_id"].ToString()),
                        fullname = row["fullname"].ToString(),
                        email = row["email"].ToString(),
                    });
            }
            return View(customers);
            /* return View(customers);*/
        }

      
    }
}
