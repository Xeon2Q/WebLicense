using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebLicense.Core.Models.Customers
{
    [Table("Customers")]
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(ReferenceId), IsUnique = true)]
    public sealed class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Comment("The Name of the Customer")]
        public string Name { get; set; }

        [Required, MaxLength(60)]
        [Comment("Secure ID of the Customer.")]
        public string Code { get; set; }

        [Required, MaxLength(40)]
        [Comment("Reference ID is using to register new users for the Customer.")]
        public string ReferenceId { get; set; }

        // navigation
        public CustomerSettings Settings { get; set; }
    }
}