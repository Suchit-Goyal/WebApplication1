using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using WebApplication1;
using WebApplication1.Controllers;

namespace TestProject1
{
    public class Tests
    {
        AccountController accountController;
        IRepository repository;

        public Tests()
        {
            repository = new AccountRepository();
           accountController = new AccountController(repository);
        }
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GetAccounts()
        {
            var mock = new Mock<IRepository>();
            mock.Setup(p => p.GetAll()).Returns(
                new List<Account>() { 
                    new Account() {AccountName="Test",AccountBalance="2000"},
                    new Account() {AccountName="Test1",AccountBalance="3000"}
                });
            AccountController b = new AccountController(mock.Object);
            var result = b.GetAllAccounts();
            mock.Verify(p => p.GetAll(), Times.Once);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var list = result.Result as OkObjectResult;
            Assert.IsInstanceOf<List<Account>>(list.Value);
            var accounts = list.Value as List<Account>;
            Assert.AreEqual(2, accounts.Count);
        }

        [Test]
        public void CreateAccount()
        {
            var account = new Account()
            {
                AccountName = "Test",
                AccountBalance = "20000"
            };
            var mock = new Mock<IRepository>();
            mock.Setup(p => p.SaveAccountData(account)).Returns(
                new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    ReasonPhrase = "Account Created"
                });
            AccountController b = new AccountController(mock.Object);
            var result = b.CreateAccount(account);
            mock.Verify(p => p.SaveAccountData(account), Times.Once);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(((HttpResponseMessage)((OkObjectResult)result).Value).ReasonPhrase, "Account Created");

            account = new Account()
            {
                AccountName = "",
                AccountBalance = ""
            };
            mock.Setup(p => p.SaveAccountData(account)).Returns(
               new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   ReasonPhrase = "Account Created"

               });
            b.ModelState.AddModelError("AccountName", "AccountName is required field");
            result = b.CreateAccount(account);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}