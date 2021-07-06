using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebLicense.Core.Models.Auxiliary;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Companies
{
    [Table("CompanyUpdates")]
    [Index(nameof(CompanyId))]
    [Index(nameof(UserId))]
    [Index(nameof(Created))]
    public sealed class CompanyUpdate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Required]
        public long UserId { get; set; }
        public User User { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Created { get; set; }

        [Required, MaxLength(2000)]
        public string Description { get; set; }

        public ICollection<ValueUpdateInfo> Changes { get; set; }
    }
}