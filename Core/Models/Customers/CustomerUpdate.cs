using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Customers
{
    [Table("CustomerUpdates")]
    public sealed class CustomerUpdate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public long UserId { get; set; }
        public User User { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Created { get; set; }

        public ICollection<> Type { get; set; }
    }
}