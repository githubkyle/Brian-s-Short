using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using NGP.Models;
using NGP.Models.ViewModels;

namespace NGP.Controllers
{
    public class ProductsController : Controller
    {
        private NGPEntities db = new NGPEntities();

        private ApplicationDbContext context = new ApplicationDbContext();

        [Authorize]
        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id, DateTime? todaydate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (!todaydate.HasValue)
            {
                todaydate = DateTime.Today;
            }

            var productvm = new ProductDetailsViewModel();
            productvm.Product = product;
            productvm.Calendar = product.GetCalendar(todaydate.Value.Year, todaydate.Value.Month);
            productvm.ProductID = product.id;
            productvm.Month = todaydate.Value;
            return View(productvm);
        }

        //Reserve
        public ActionResult Reserve(int? id, DateTime? todaydate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

           if (!todaydate.HasValue)
            {
                todaydate = DateTime.Today;
            }

            var productvm = new ProductDetailsViewModel();
            productvm.Product = product;
            productvm.Calendar = product.GetCalendar(todaydate.Value.Year, todaydate.Value.Month);

            productvm.Product = product;
            productvm.Calendar = product.GetCalendar(todaydate.Value.Year, todaydate.Value.Month);
            productvm.ProductID = product.id;
            productvm.Month = todaydate.Value;
            return View(productvm);

            return View(productvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] [Authorize]
        public ActionResult Reserve(ProductDetailsViewModel moviemodel, DateTime? todaydate)
        {
            var product = db.Products.Find(moviemodel.ProductID);
            moviemodel.Product = product;
            if (todaydate.HasValue)
            {                
               
                    moviemodel.Month = todaydate.Value;
                moviemodel.Calendar = product.GetCalendar(todaydate.Value.Year, todaydate.Value.Month);
                return View(moviemodel);
            }
            if (ModelState.IsValid)
            {
                bool isvalid = true;
                //var user = db.Customers.Where(x => x.UserID == myUser).Select(c => c.Name);

                var useridTest = User.Identity.GetUserId();
                var user = context.Users.FirstOrDefault(u => u.Id == useridTest);
                String em = user.Email;
                EmailUtility.registerProduct(product.Name +" has been booked from "+moviemodel.StartDate+" until "+moviemodel.EndDate+ " by "+ em);
                //start date and end date validtions, ifs, startdate.hasvalue if null will be false, datetime.today=today's date
                //startdate must be at least today, if missing, if not today seperate errorr
                //enddate check if null,  along with or after startdate, must be later than start date


                for(DateTime d = moviemodel.StartDate??moviemodel.EndDate??DateTime.Today; d <= (moviemodel.EndDate??moviemodel.StartDate??DateTime.Today); d=d.AddDays(1))
                {
                    if (product.OpnInvDay(d) < moviemodel.Quantity)
                    {
                        //error, not enough available
                        ModelState.AddModelError("Quantity", "Sorry, we're out of stock on those days");
                            isvalid=false;
                        break;
                        
                    }

                    if (DateTime.Now > moviemodel.StartDate)
                    {
                        ModelState.AddModelError("StartDate", "Select a present or future start date");
                        isvalid = false;
                        break;
                    }

                   



                }
                if ( moviemodel.StartDate < DateTime.Now /*|| moviemodel.EndDate > DateTime.Now*/)
                {
                    ModelState.AddModelError("EndDate", "End date must be after start date");
                    isvalid = false;
                }


                if (!isvalid)
                {
                    moviemodel.Calendar = product.GetCalendar(moviemodel.Month.Year, moviemodel.Month.Month);
                    return View(moviemodel);
                }
                var userid = User.Identity.GetUserId();
                var cart = db.Carts.FirstOrDefault(x => x.UserID == userid && x.OrderDate == null);


                if (cart == null)
                {
                    cart = new Cart();
                    cart.UserID = userid;

                }

                var OLI = new OrderLineItem();
                OLI.Cart = cart;
                OLI.EndDate = moviemodel.EndDate.Value;
                OLI.StartDate = moviemodel.StartDate.Value;
                OLI.ProductID = moviemodel.ProductID;
                OLI.Quantity = moviemodel.Quantity;
                db.OrderLineItems.Add(OLI);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(moviemodel);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Name,Details,PricePerDay,PricePerWeek,PricePerMonth,Inventory")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Name,Details,PricePerDay,PricePerWeek,PricePerMonth,Inventory")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
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
