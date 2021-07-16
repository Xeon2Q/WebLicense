using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebLicense.Core.Models.Companies
{
    [Index(nameof(CompanyId))]
    [Index(nameof(Email))]
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(Created))]
    public sealed class CompanyUserInvite
    {
        [Required]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; }
    }
}