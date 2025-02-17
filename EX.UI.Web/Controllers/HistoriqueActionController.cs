using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    public class HistoriqueActionController : Controller
    {
        // GET: HistoriqueActionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: HistoriqueActionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HistoriqueActionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HistoriqueActionController/Create
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

        // GET: HistoriqueActionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HistoriqueActionController/Edit/5
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

        // GET: HistoriqueActionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HistoriqueActionController/Delete/5
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
