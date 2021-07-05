using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebLicense.Core.Models.Customers;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Companies
{
    [Table("Companies")]
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(ReferenceId), IsUnique = true)]
    public sealed class Company
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(200), Comment("The Name of the Company")]
        public string Name { get; set; }

        [Required, MaxLength(40), Comment("Reference ID is using to register new users for the Company.")]
        public string ReferenceId { get; set; }

        // navigation
        public ICollection<User> Users { get; set; }
        public ICollection<CompanyUser> CompanyUsers { get; set; }

        public ICollection<Customer> Customers { get; set; }
        public ICollection<CustomerSettings> CompanyCustomers { get; set; }
    }
}