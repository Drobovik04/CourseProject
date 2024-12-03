using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Controllers
{
    public class TemplatesController : Controller
    {
        // GET: TemplatesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TemplatesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TemplatesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TemplatesController/Create
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

        // GET: TemplatesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TemplatesController/Edit/5
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

        // GET: TemplatesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TemplatesController/Delete/5
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
