using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    public class MarketSegmentController : Controller
    {
        // GET: MarketSegmentController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MarketSegmentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MarketSegmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MarketSegmentController/Create
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

        // GET: MarketSegmentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MarketSegmentController/Edit/5
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

        // GET: MarketSegmentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MarketSegmentController/Delete/5
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
