using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1
{

    public class AccountRepository : IRepository
    {
        string dir;

        string tempFileName;
        public AccountRepository()
        {
            dir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            tempFileName = Path.Combine(dir, "Accounts.json");
        }

        private enum TransactionType
        {
            Deposit,
            Withdraw
        }

        public HttpResponseMessage SaveAccountData(Account account)
        {
            List<Account> allAccount = new List<Account>();
            try
            {
                if (CheckFileExist())
                {
                    allAccount= GetAccounts();
                    if (allAccount.FindAll(a => a.AccountName == account.AccountName).Count > 0)
                    {
                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ReasonPhrase = "Account Exist"
                        };

                    }
                    allAccount.Add(account);
                    WriteToFile(tempFileName, allAccount.AsReadOnly());
                }
                else
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    allAccount.Add(account);
                    WriteToFile(tempFileName, allAccount.AsReadOnly());
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage()
                {
                    StatusCode= HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                ReasonPhrase="Account Created"
            };
        }
        private void WriteToFile(string fileName, ReadOnlyCollection<Account> allAccount)
        {
            using(StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, allAccount); 
            }
        }
        private bool CheckFileExist()
        {
             if(!File.Exists(tempFileName))
            {
                return false;
            }
            return true;
        }

        private List<Account> GetAccounts()
        {
            var text = System.IO.File.ReadAllText(tempFileName);
            return !string.IsNullOrEmpty(text) ? JsonConvert.DeserializeObject<Account[]>(text).ToList() : new List<Account>();
        }
        public IEnumerable<Account> GetAll()
        {
            return GetAccounts();
        }
        public HttpResponseMessage DeleteAccount(string accountName)
        {
            if (CheckFileExist())
            {
                var allAccount = GetAccounts();
                var account = allAccount.FindAll(a => a.AccountName == accountName);
                try
                {
                    if (allAccount.Remove(account.First()))
                    {
                        WriteToFile(tempFileName, allAccount.AsReadOnly());
                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.OK,
                            ReasonPhrase = "Account Deleted"
                        };
                    }
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ReasonPhrase = "Account does not exist"
                    };
                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
       

        public HttpResponseMessage TransactAccount(TransactAccount account)
        {
            TransactionType type;
            try
            {
                if (CheckFileExist())
                {
                    Enum.TryParse(account.type, true, out type);
                    var allAccount = GetAccounts();
                    var transactionAccount = allAccount.FindAll(a => a.AccountName == account.AccountName).First();
                    var amount = Convert.ToInt32(account.Amount);
                    if (amount > 0)
                    {
                        if (type == TransactionType.Deposit)
                        {
                            if (amount > 10000)
                            {
                                return new HttpResponseMessage()
                                {
                                    StatusCode = HttpStatusCode.BadRequest,
                                    ReasonPhrase = "More than 10000 not allowed in single transanction :  " + account.AccountName
                                };
                            }
                            else
                            {
                                allAccount.Where(a => a.AccountName == account.AccountName).First().AccountBalance =
                                    (amount + Convert.ToInt32(transactionAccount.AccountBalance)).ToString();
                                WriteToFile(tempFileName, allAccount.AsReadOnly());
                                return new HttpResponseMessage()
                                {
                                    StatusCode = HttpStatusCode.OK,
                                    ReasonPhrase = "Account Balance Updated by :" + account.Amount
                                };
                            }
                        }
                        if (type == TransactionType.Withdraw)
                        {
                            var newBal = Convert.ToInt32(transactionAccount.AccountBalance)-amount;
                            if (newBal < 100)
                            {
                                return new HttpResponseMessage()
                                {
                                    StatusCode = HttpStatusCode.BadRequest,
                                    ReasonPhrase = "Invalid amount. Balance cannot be less than $100 :  " + account.AccountName
                                };
                            }
                            else
                            {
                                if (amount >= (.90) * Convert.ToInt32(transactionAccount.AccountBalance))
                                {
                                    return new HttpResponseMessage()
                                    {
                                        StatusCode = HttpStatusCode.BadRequest,
                                        ReasonPhrase = "cannot withdraw more than 90% of their total balance from an account in a single transaction. "
                                    };
                                }
                                else
                                {
                                    allAccount.Where(a => a.AccountName == account.AccountName).First().AccountBalance =
                                   newBal.ToString();
                                    WriteToFile(tempFileName, allAccount.AsReadOnly());
                                    return new HttpResponseMessage()
                                    {
                                        StatusCode = HttpStatusCode.OK,
                                        ReasonPhrase = "Account Balance Updated by :" + account.Amount
                                    };
                                }
                            }
                        }
                    }
                    else
                    {
                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ReasonPhrase = "Incorrect Amount balance " + account.Amount
                        };
                    }
                }
                else
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "No account exist with name " + account.AccountName
                    };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }
    
}
