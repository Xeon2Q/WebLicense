using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Companies
{
    [Table("Companies")]
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(ReferenceId), IsUnique = true)]
    public sealed class Company
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(200), Comment("The Name of the Customer")]
        public string Name { get; set; }

        [Required, MaxLength(60), Comment("Secure ID of the Customer.")]
        public string Code { get; set; }

        [Required, MaxLength(40), Comment("Reference ID is using to register new users for the Customer.")]
        public string ReferenceId { get; set; }

        // navigation
        [Comment("List of Companies which provides services to this Company")]
        public ICollection<Company> Providers { get; set; }
        [Comment("List of Companies which uses services from this Company")]
        public ICollection<Company> Clients { get; set; }
        public ICollection<CompanySettings> Settings { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<CompanyUser> CompanyUsers { get; set; }

        public ICollection<CompanyUpdate> Updates { get; set; }
    }
}