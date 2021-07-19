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
    [Index(nameof(Processed))]
    public sealed class CompanyUserInvite
    {
        [Required]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public bool IsManager { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; } = $"{Guid.NewGuid():N}{Guid.NewGuid():N}".ToUpper();

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public bool Processed { get; set; } = false;
    }
}