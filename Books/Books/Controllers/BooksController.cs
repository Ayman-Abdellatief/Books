using Books.Models;
using Books.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace Books.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Books
        public ActionResult Index()
        {
            var books = _context.books.Include(m =>m.category).ToList();
            return View(books);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var Book = _context.books.Include(m => m.category).SingleOrDefault(m => m.Id == id) ;

            if(Book == null)
                return HttpNotFound();

            return View(Book);
        }
        public ActionResult Create()
        {
            var ViewModel = new BookFormViewModel()
            {
                categories = _context.categories.Where(x =>x.IsActive).ToList()
        };

            return View("BookForm", ViewModel);
        }

        public ActionResult Edit(int? id)
        {

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var Book = _context.books.Find(id);

            if (Book == null)
                return HttpNotFound();

            var ViewModel = new BookFormViewModel
            {
                Id = Book.Id,
                Title = Book.Title,
                Author = Book.Author,
                categoryId = Book.categoryId,
                Description = Book.Description,
                categories = _context.categories.Where(M => M.IsActive).ToList()

            };

            return View("BookForm", ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BookFormViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.categories = _context.categories.Where(x => x.IsActive).ToList();
                return View("BookForm", model);
            }
            if(model.Id == 0)
            {
                var book = new Book
                {
                    Title = model.Title,
                    Author = model.Author,
                    categoryId = model.categoryId,
                    Description = model.Description
                };
               
                _context.books.Add(book);
              
            }else
            {
                var book = _context.books.Find(model.Id);

                if (book == null)
                    return HttpNotFound();

                book.Title = model.Title;
                book.Author = model.Author;
                book.categoryId = model.categoryId;
                book.Description = model.Description;

            }
            _context.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}