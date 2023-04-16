using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        public readonly IRepository repository;

        public AccountController(IRepository _repository)
        {
                repository = _repository;
               // repository = _repository;
        }
        [HttpGet("~/getallaccounts",Name = "GetAccounts")]
        public ActionResult<List<Account>> GetAllAccounts()
        {
            var r = repository.GetAll();
            return Ok(r);
        }

        [HttpPost(Name = "CreateAccount"),ActionName("CreateAccount")]
        public ActionResult CreateAccount([FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var r = repository.SaveAccountData(account);
            return Ok(r);
        }

        [HttpDelete(Name = "DeleteAccount")]
        public ActionResult DeleteAccount(string account)
        {
            return Ok(account);
        }
        [HttpPost("~/transactaccount",Name = "TransactAccount"),ActionName("Transact")]
        public HttpResponseMessage TransactAccount([FromBody] TransactAccount account)
        {
            return repository.TransactAccount(account);
        }
    }
}