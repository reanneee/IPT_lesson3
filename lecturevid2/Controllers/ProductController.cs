using lecturevid2.DataAccess;
using lecturevid2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
//using session
using Microsoft.AspNetCore.Http;
//using xml another videolecture
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace lecturevid2.Controllers
{
    public class ProductController : Controller
    {
      /*  private readonly DatabaseHelper _databaseHelper;*/

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _xmlFilePath;
        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _xmlFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Schemas", "Products.xml");
        }

     
        [HttpGet]
        public IActionResult LiveSearch(string? keyword)
        {

            if (string.IsNullOrEmpty(keyword))
            {
                /* keyword = "";*/
                 var products = ReadProductsFromXml();
                return Json(products);
            }
            else
            {
            var xmlDoc = XDocument.Load(_xmlFilePath);
            var products = xmlDoc.Descendants("Product")
                .Where(x => x.Element("Name").Value.ToLower().Contains(keyword.ToLower())).Select(x => new 
            {
                id = x.Element("Id").Value,
                name = x.Element("Name").Value,
                price = x.Element("Price").Value,
            }).ToList();
            return Json(products);
            }
            
        }
   public IActionResult ProductsView()
        {
            var products = ReadProductsFromXml();
            return View(products);
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(Products product)
        {
            if (ModelState.IsValid)
            {
                var products = ReadProductsFromXml();
                product.Id = products.Count + 1;
                products.Add(product);
                WriteProductsToXml(products);
                return RedirectToAction("ProductsView");
            }
            return View(product);
        }

        private List<Products> ReadProductsFromXml()
        {
            if (!System.IO.File.Exists(_xmlFilePath))
            {
                return new List<Products>();
            }
            XDocument xmlDoc = XDocument.Load(_xmlFilePath);//XDocument part of the namespace ,link, under xml. convert c#objects
            return xmlDoc.Descendants("Product").Select(p => new Products 
            {
                Id=int.Parse(p.Element("Id")?.Value),//? null reference
                Name=p.Element("Name")?.Value,
                Price = decimal.Parse(p.Element("Price")?.Value),
            }).ToList();//converts into a lsit of products
        }
        private void WriteProductsToXml(List<Products> products)
        {
            XDocument xmlDoc = new XDocument(
                new XElement("Products",
                    products.Select(p =>
                        new XElement("Product",
                            new XElement("Id", p.Id),
                            new XElement("Name", p.Name),
                            new XElement("Price", p.Price)
                        )
                    )
                )
            );
            xmlDoc.Save(_xmlFilePath);
        }

            /*   public ProductController(IWebHostEnvironment webHostEnvironment)
               {
                   string connString = "Server=localhost;Database=ipt;User Id=root;Password='';";
                   _databaseHelper = new DatabaseHelper(connString);
                   _webHostEnvironment = webHostEnvironment;
               }

               public IActionResult UploadXml()
               {
                   return View();
               }
               [HttpPost]
               public ActionResult UploadXml(IFormFile file)//iformfile represents the uploaded file from a html form
               {
                   if (file == null || file.Length == 0)
                   {
                       ViewBag.Message = "Please select a valid XML file.";
                       return View();
                   }
                   string schemaPath = Path.Combine(_webHostEnvironment.WebRootPath, "Schemas", "Products.xsd");
                   bool isValid= ValidateXml(file, schemaPath, out string validationMessage) ; //out parameter capture any validations errors encounter
                   if (isValid) {
                       ViewBag.Message = "XML file is valid.";
                   }
                   else
                   {
                       ViewBag.Message = $"XML file is invalid. Error: {validationMessage}";
                   }
                   return View(); 
               }

               private bool ValidateXml(IFormFile file, string xsdPath, out string validationMessage)//out doesn't have a direct control over the output variable
               {
                   validationMessage = string.Empty;
                   bool isValid = true;
                   string errorMessage = string.Empty;
                   XmlSchemaSet schemaSet = new XmlSchemaSet();//validate xml file,creating instance xml schema set.hold xsd
                   schemaSet.Add("", xsdPath);
                   using (var stream=file.OpenReadStream()) //allow other file to be read directly from the memory w/o saving it to the server
                   {
                       XmlReaderSettings settings = new XmlReaderSettings();//configuration xml settings, how xml read and validated
                       settings.Schemas = schemaSet;//specifies xsd schema to be used for validation
                       settings.ValidationType =ValidationType.Schema;//tell the xml reader to validate the xml file using xsd schema(type of validation :schemaa document type definition)
                       settings.ValidationEventHandler += (sender, args) =>//
                       {
                           isValid=false;
                           errorMessage = args.Message;//args.Message contains the exact error messaage from our xsd validation
                       };   
                       using (XmlReader reader = XmlReader.Create(stream, settings))//creating xml reader
                       {
                           while (reader.Read()) { }//read the entire xml file node by node,until end of xml file
                       }

                   }
                   validationMessage = errorMessage;
                   return isValid;//true or false
               }



               public IActionResult Index()
               {
                   if (HttpContext.Session.GetString("UserEmail") == null)
                   {
                       return RedirectToAction("Login", "Login");
                   }
                   string query = "SELECT * FROM products";
                   DataTable dt = _databaseHelper.SelectQuery(query);

                   List<ProductModel> products = new List<ProductModel>();
                   foreach(DataRow row in dt.Rows)
                   {
                       products.Add( 
                      new ProductModel
                      {
                          prod_id=Convert.ToInt32(row["prod_id"]),
                          prod_name=row["prod_name"].ToString(),
                          prod_price=Convert.ToInt32(row["prod_price"]),
                      }
                       );
                   }
                   return View(products);
               }


               public IActionResult Details(int id)
               {
                   if (HttpContext.Session.GetString("UserEmail") == null)
                   {
                       return RedirectToAction("Login", "Login");
                   }
                   string query = $"SELECT * FROM products WHERE prod_id={id}";
                   DataTable dt = _databaseHelper.SelectQuery(query);

                   if (dt.Rows.Count == 0)
                   {
                       return NotFound();
                   }
                   DataRow row = dt.Rows[0];
                   ProductModel product = new ProductModel
                   {
                       prod_id = Convert.ToInt32(row["prod_id"]),
                       prod_name = row["prod_name"].ToString(),
                       prod_price = Convert.ToInt32(row["prod_price"]),
                   };
                   return View(product);
               }
               public IActionResult Create()
               {

                   if (HttpContext.Session.GetString("UserEmail") == null)
                   {
                       return RedirectToAction("Login", "Login");
                   }
                   return View();
               }


               [HttpPost]
               [ValidateAntiForgeryToken]
               public IActionResult Create(ProductModel prod)
               {

                   if (!ModelState.IsValid)
                   {
                       return View(prod);
                   }
                   string query = $"INSERT INTO products (prod_name,prod_price) VALUES('{prod.prod_name}','{prod.prod_price}')";
                   _databaseHelper.ExecuteQuery(query);
                   return RedirectToAction("Index");
               }

               public IActionResult Edit(int id)
               {
                   if (HttpContext.Session.GetString("UserEmail") == null)
                   {
                       return RedirectToAction("Login", "Login");
                   }
                   string query = $"SELECT * FROM products WHERE prod_id={id}";
                   _databaseHelper.SelectQuery(query);
                   DataTable dt = _databaseHelper.SelectQuery(query);
                   if (dt.Rows.Count==0)
                   {
                       return NotFound();
                   }
                   DataRow row = dt.Rows[0];
                   ProductModel product = new ProductModel
                  {
                       prod_id = Convert.ToInt32(row["prod_id"]),
                       prod_name = row["prod_name"].ToString(),
                       prod_price = Convert.ToInt32(row["prod_price"]),
                   };
                   return View(product);
               }

               [HttpPost]
               [ValidateAntiForgeryToken]
               public IActionResult edit(ProductModel prod)
               {
                   if (!ModelState.IsValid)
                   {
                       return View(prod);
                   }
                   string query = $"UPDATE products SET prod_name='{prod.prod_name}', prod_price='{prod.prod_price}' WHERE prod_id={prod.prod_id}";
                   _databaseHelper.ExecuteQuery(query);
                   return View();
               }


               public IActionResult delete(int id)
               {
                   if (HttpContext.Session.GetString("UserEmail") == null)
                   {
                       return RedirectToAction("Login", "Login");
                   }
                   string query = $"SELECT * FROM products WHERE prod_id={id}";
                   DataTable dt = _databaseHelper.SelectQuery(query);
                   if (dt.Rows.Count==0)
                   {
                       return NotFound();
                   }
                   DataRow row = dt.Rows[0];
                   ProductModel product = new ProductModel
                   {
                       prod_id=Convert.ToInt32(row["prod_id"]),
                       prod_name=row["prod_name"].ToString(),
                       prod_price=Convert.ToInt32(row["prod_price"]),
                   };
                   return View(product);
               }
               [HttpPost]
               [ValidateAntiForgeryToken]
               public IActionResult delete(ProductModel prod)
               {

                   string query = $"DELETE FROM products WHERE prod_id={prod.prod_id}";
                   _databaseHelper.SelectQuery(query);
                   return RedirectToAction("Index");
               }

               public IActionResult logout()
               {
                   HttpContext.Session.Clear();

                   return RedirectToAction("Login", "Login");
               }*/
        }
}
