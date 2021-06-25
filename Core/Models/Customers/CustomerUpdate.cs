using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebLicense.Core.Models.Auxiliary;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Customers
{
    [Table("CustomerUpdates")]
    [Index(nameof(CustomerId))]
    [Index(nameof(UserId))]
    [Index(nameof(Created))]
    public sealed class CustomerUpdate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

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