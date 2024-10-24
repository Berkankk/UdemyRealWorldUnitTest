using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NuGet.ContentModel;
using System.Linq;
using UdemyRealWorldUnitTest.WEB.Controllers;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;

namespace UdemyRealWorldUnitTest.Test.ProductsControllerTest
{

    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> products;
        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);

            products = new List<Product>
            {
                   new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
    new Product { Id = 2, Name = "Defter", Price = 200, Stock = 150, Color = "Mavi" }
            };




            //products = new List<Product>() { new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" } };
            //products = new List<Product>() { new Product { Id = 2, Name = "Defter", Price = 200, Stock = 150, Color = "Mavi" } };
        }


        [Fact] //Parametre almadığı için bu attribute u geçtik
        public async void Index_ActionExecutes_ReturnView() //index metodunun aksiyonunda view dönüyor mu test??
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result); //index metodun gelen cevap viewresult mu ona bakıyoruz 

            //Type in üstüne gelince viewresult döndüğünü veriyor 
        }

        [Fact]
        public async void Index_ActionExecutes_ProductList()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products); //Taklit ettiğimiz metot
            //products ı dönerek yukarıda verdiğimiz dataları bize döncek

            var result = await _controller.Index();//metot çalıştır 

            var viewResult = Assert.IsType<ViewResult>(result); //index metodundan dönen sonucun tipi viewresult olacak bunu yazdık 


            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            //viewresulttan gelen data ile benim verdiğim aynı mı ???

            Assert.Equal<int>(2, productList.Count());
            //Eğer gelen veriler eşitse ve 2 tane productlist geliyor mu bunları test ettik
        }


        [Fact]
        public async void Details_IdisNull_ReturnRedirectToIndexAction()
        {

            var result = await _controller.Details(null); //Null dönerse 

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            //Dönüş tipi redirectoactionresult mu karşılaştırmasını yazdık

            Assert.Equal("Index", redirect.ActionName);

        }
        [Fact]
        public async void Details_IdInValid_ReturnNotFount()
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);

        }

        [Theory]
        [InlineData(2)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {

            Product product = products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(productId, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);

        }


        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();//Metotu Çalıştırdık çalış köle dedik :(

            Assert.IsType<ViewResult>(result); //Resulttan gelen değer view mi ona baktık
            //Tipi view mi değil mi ona baktık 
        }

        [Fact]
        public async void CreatePOST_InvalidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");
            //Hatayı yazdık ve hatanın sebebini yazdık

            var result = await _controller.Create(products.First());
            //metodun marşına dokunduk
            var viewResult = Assert.IsType<ViewResult>(result);
            //Gelen result viewresult mı onu verdik burada 

            Assert.IsType<Product>(viewResult.Model);
            //Viewresulttan gelen model product mı onu kontrol ettik burada 

        }


        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirecToIndexAction()
        {
            var result = await _controller.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }


        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {

            Product product = null;


            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => product = x);

            var result = await _controller.Create(products.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once());

            Assert.Equal(products.First().Id, product.Id);

        }


        [Fact]
        public async void Edit_IdNull_ReturnRedirecttoIndexAction()
        {
            var result = await _controller.Edit(null); //Edit metotunu çalıştırdık

            var redirect = Assert.IsType<RedirectToActionResult>(result);


            Assert.Equal("Index", redirect.ActionName);

        }


        [Theory]
        [InlineData(3)]

        public async void Edit_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);


            var redirect = Assert.IsType<NotFoundResult>(result);
            //Gelen datanın tipinin notfoundresult olup olmadığını görüyoruz

            Assert.Equal(404, redirect.StatusCode);
        }


        [Theory]
        [InlineData(2)]

        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId); //var olan ıd değerini verdik 

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);

            Assert.Equal(product.Name, resultProduct.Name);


        }


        [Theory]
        [InlineData(2)]
        public void EditPOST_IdIsNullEqualProduct_ReturnNotFount(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);

            var result = _controller.Edit(10, product);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, redirect.StatusCode);

        }


        [Theory]
        [InlineData(1)]

        public void EditPOST_InvalidModelState_ReturnView(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);
            _controller.ModelState.AddModelError("Name", "");
            if (product == null) //product null geldiğinden buraya girecek
            {
                var result = new NotFoundResult();
                Assert.IsType<NotFoundResult>(result);//Ve notfoundresult dönecek
                return;
            }

        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {

            var products = new List<Product>()
            {
                new Product { Id = 1, Name = "Test product" },
                new Product { Id = 2, Name = "Test product 2" }
            };

            var product = products.FirstOrDefault(x => x.Id == productId);
            Assert.NotNull(product);

            var result = _controller.Edit(productId, product);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);


            //var mockRepo = new Mock<IRepository<Product>>();
            //mockRepo.Setup(repo => repo.GetAll()).Returns(new List<Product>
            //{
            //    new Product { Id = 1, Name = "Test Product 1" },
            //    new Product { Id = 2, Name = "Test Product 2" }
            //});
            //var _controller = new ProductsController(mockRepo.Object);
        }



        [Fact]
        public async void Delete_IdIsNull_ReturnNotfound()
        {
            var result = await _controller.Delete(null);

            var redirect = Assert.IsType<NotFoundResult>(result);

            //Notfound sayfası geliyor 

        }


        [Theory]
        [InlineData(0)] //Product null gelecek

        public async void Delete_IdIsNullEqualProduct_ReturnNotFound(int prdouctId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(prdouctId)).ReturnsAsync(product);

            var result = await _controller.Delete(prdouctId);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, redirect.StatusCode);
        }


        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);

        }





    }
}
