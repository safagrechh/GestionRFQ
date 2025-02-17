using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    public class CommentaireController : Controller
    {
        // GET: CommentaireController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CommentaireController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CommentaireController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommentaireController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentaireController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentaireController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentaireController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CommentaireController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
