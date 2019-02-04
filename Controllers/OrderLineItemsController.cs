using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NGP.Models;

namespace NGP.Controllers
{
    public class OrderLineItemsController : Controller
    {
        private NGPEntities db = new NGPEntities();

        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: OrderLineItems
        [Authorize]
        public ActionResult Index()
        {
            var useridTest = User.Identity.GetUserId();
            //var user = context.Users.FirstOrDefault(u => u.Id == useridTest);
            var orderLineItems = db.OrderLineItems.Where(x=>useridTest == x.Cart.UserID).Include(o => o.Cart).Include(o => o.Product);
           
            return View(orderLineItems.ToList());
        }

        // GET: OrderLineItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLineItem orderLineItem = db.OrderLineItems.Find(id);
            if (orderLineItem == null)
            {
                return HttpNotFound();
            }
            return View(orderLineItem);
        }

        // GET: OrderLineItems/Create
        public ActionResult Create()
        {
            ViewBag.CartID = new SelectList(db.Carts, "id", "UserID");
            ViewBag.ProductID = new SelectList(db.Products, "id", "Name");
            return View();
        }

        // POST: OrderLineItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CartID,ProductID,Quantity,StartDate,EndDate")] OrderLineItem orderLineItem)
        {
            if (ModelState.IsValid)
            {
                db.OrderLineItems.Add(orderLineItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CartID = new SelectList(db.Carts, "id", "UserID", orderLineItem.CartID);
            ViewBag.ProductID = new SelectList(db.Products, "id", "Name", orderLineItem.ProductID);
            return View(orderLineItem);
        }

        // GET: OrderLineItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLineItem orderLineItem = db.OrderLineItems.Find(id);
            if (orderLineItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.CartID = new SelectList(db.Carts, "id", "UserID", orderLineItem.CartID);
            ViewBag.ProductID = new SelectList(db.Products, "id", "Name", orderLineItem.ProductID);
            return View(orderLineItem);
        }

        // POST: OrderLineItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CartID,ProductID,Quantity,StartDate,EndDate")] OrderLineItem orderLineItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderLineItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CartID = new SelectList(db.Carts, "id", "UserID", orderLineItem.CartID);
            ViewBag.ProductID = new SelectList(db.Products, "id", "Name", orderLineItem.ProductID);
            return View(orderLineItem);
        }

        // GET: OrderLineItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLineItem orderLineItem = db.OrderLineItems.Find(id);
            if (orderLineItem == null)
            {
                return HttpNotFound();
            }
            return View(orderLineItem);
        }

        // POST: OrderLineItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderLineItem orderLineItem = db.OrderLineItems.Find(id);
            db.OrderLineItems.Remove(orderLineItem);
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
