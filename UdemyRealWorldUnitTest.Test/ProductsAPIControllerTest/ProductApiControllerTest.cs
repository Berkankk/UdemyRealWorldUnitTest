using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;
using UdemyRealWorldUnitTest.WEB.Controllers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UdemyRealWorldUnitTest.WEB.Helpers;

namespace UdemyRealWorldUnitTest.Test.ProductsAPIControllerTest
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsAPIController _controllerTest;
        private readonly Helper _helper;


        private List<Product> products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controllerTest = new ProductsAPIController(_mockRepo.Object);
            _helper = new Helper();
            products = new List<Product>()
            {
                new Product
                { Id = 1,Name="Kalem",Price=100,Stock=150,Color="Kırmızı"},
                new Product
                { Id = 2,Name="Silgi",Price=50,Stock=300,Color="Beyaz"}
            };
        }

        [Theory]
        [InlineData(4, 5, 9)]
        public void Add_SampleValues_ReturnTotal(int a, int b, int total)
        {
            var result = _helper.add(a, b);

            Assert.Equal(total, result);
        }
        //Bu şekilde iş yapan kodları da test edeebiliriz 

        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProduct() //Başarılı ve içinde product var mı yok mu ona baktık
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products); //Bu products yukarıdan geldi

            var result = await _controllerTest.GetProducts(); //Api içinde olan metot çalış dedik

            var okResult = Assert.IsType<OkObjectResult>(result); //tipine baktık

            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2, returnProduct.ToList().Count());

        }

        [Theory]
        [InlineData(0)]

        public async void GetProducts_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controllerTest.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product); //idler eşit mi

            var result = await _controllerTest.GetProduct(productId);

            var okResult = Assert.IsType<OkObjectResult>(result); //içinde ürün olduğu için obje döndük

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);
        }

        [Theory]
        [InlineData(1)]

        public void PutProduct_IdIsNotEqual_ReturnBadRequestResult(int productId)
        {
            var product = products.First(x => x.Id == productId);

            var result = _controllerTest.PutProduct(2, product);//idler tutmayacak badrequest dönecek

            Assert.IsType<BadRequestResult>(result);//içinde data yok o yüzden badrequest döndük

        }

        [Theory]
        [InlineData(1)]

        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.Update(product));


            var result = _controllerTest.PutProduct(productId, product);

            _mockRepo.Verify(x => x.Update(product), Times.Once);//bir kere çalışsın

            Assert.IsType<NoContentResult>(result);

        }


        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreateAtAction()
        {
            var product = products.First();
            _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);


            var result = await _controllerTest.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(x => x.Create(product), Times.Once);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);

        }

    }
}
