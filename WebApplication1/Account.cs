using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApplication1
{
    public class Account
    {
        [Required]
        public string AccountName { get; set; }
        [Required]
        [Range(100,Int32.MaxValue,ErrorMessage ="Cannot be less than 100")]
        public string AccountBalance { get; set; }
    }
    public class TransactAccount
    {
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string Amount { get; set; }
        [Required]
        public string type { get; set; }
    }
}