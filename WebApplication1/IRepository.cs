namespace WebApplication1
{
    public interface IRepository
    {

        public HttpResponseMessage SaveAccountData(Account account);
        public HttpResponseMessage DeleteAccount(string accountName);
        public HttpResponseMessage TransactAccount(TransactAccount account);
        public IEnumerable<Account> GetAll();
    }
}