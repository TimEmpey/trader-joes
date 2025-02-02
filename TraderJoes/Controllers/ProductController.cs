using Microsoft.AspNetCore.Mvc;
using TraderJoes.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace TraderJoes.Controllers
{
  public class ProductsController : Controller
  {
    private readonly TraderJoesContext _db;

    public ProductsController(TraderJoesContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Product> sortedProducts = _db.Products.OrderBy(product => product.DueDate).ToList();
      return View(sortedProducts);
    }

    public ActionResult Create()
    {
      ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "Name");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Product product, int DepartmentId, bool Completed, DateTime DueDate)
    {
      _db.Products.Add(product);
      _db.SaveChanges();
      if (DepartmentId != 0)
      {
        _db.DepartmentProduct.Add(new DepartmentProduct() { DepartmentId = DepartmentId, ProductId = product.ProductId });
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisProduct = _db.Products
        .Include(product => product.JoinEntities)
        .ThenInclude(join => join.Department)
        .FirstOrDefault(product => product.ProductId == id);
      return View(thisProduct);
    }

    public ActionResult Edit(int id)
    {
      Product thisProduct = _db.Products.FirstOrDefault(product => product.ProductId == id);
      ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "Name");
      return View(thisProduct);
    }

    [HttpPost]
    public ActionResult Edit(Product product, int DepartmentId)
    {
      if (DepartmentId != 0)
      {
        _db.DepartmentProduct.Add(new DepartmentProduct() { DepartmentId = DepartmentId, ProductId = product.ProductId });
      }
      _db.Entry(product).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddDepartment(int id)
    {
      var thisProduct = _db.Products.FirstOrDefault(product => product.ProductId == id);
      ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "Name");
      return View(thisProduct);
    }

    [HttpPost]
    public ActionResult AddDepartment(Product product, int DepartmentId)
    {
      if (DepartmentId != 0)
      {
        _db.DepartmentProduct.Add(new DepartmentProduct() { DepartmentId = DepartmentId, ProductId = product.ProductId });
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      Product thisProduct = _db.Products.FirstOrDefault(product => product.ProductId == id);
      return View(thisProduct);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Product thisProduct = _db.Products.FirstOrDefault(product => product.ProductId == id);
      _db.Products.Remove(thisProduct);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteDepartment(int joinId)
    {
      var joinEntry = _db.DepartmentProduct.FirstOrDefault(entry => entry.DepartmentProductId == joinId);
      _db.DepartmentProduct.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}