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

        public byte[] Logo { get; set; }

        [Required, MaxLength(40), Comment("Reference ID is using to register new users for the Customer.")]
        public string ReferenceId { get; set; }

        // navigation
        public ICollection<CompanySettings> Settings { get; set; }
        public ICollection<CompanySettings> ClientSettings { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<CompanyUser> CompanyUsers { get; set; }
        public ICollection<CompanyUserInvite> CompanyUserInvites { get; set; }

        public ICollection<CompanyUpdate> Updates { get; set; }
    }
}