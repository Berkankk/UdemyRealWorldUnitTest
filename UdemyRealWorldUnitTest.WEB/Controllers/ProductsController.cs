using Microsoft.AspNetCore.Mvc;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;

namespace UdemyRealWorldUnitTest.WEB.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productsRepository;

        public ProductsController(IRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _productsRepository.GetAll());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _productsRepository.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _productsRepository.Create(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _productsRepository.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _productsRepository.Update(product);

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productsRepository.GetById((int)id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productsRepository.GetById((int)id);
            if (product != null)
            {
                _productsRepository.Delete(product);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            var products = _productsRepository.GetById(id).Result;

            if (products == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
