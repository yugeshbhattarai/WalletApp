using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YourWallet.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Entered amount should be greater than 0.")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcons 
        {
            get
            {
                return Category==null?"":Category.Icon+" "+Category.Title;
            }
                
        }

        [NotMapped]
        public string? TypeofAmount
        {
            get
            {
                return ((Category == null || Category.Type=="Expense" )? "- " : "+ ") + Amount.ToString("C2");
            }

        }
    }
}
