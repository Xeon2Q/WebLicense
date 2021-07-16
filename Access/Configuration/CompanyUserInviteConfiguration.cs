using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Models.Companies;

namespace WebLicense.Access.Configuration
{
    public sealed class CompanyUserInviteConfiguration : IEntityTypeConfiguration<CompanyUserInvite>
    {
        public void Configure(EntityTypeBuilder<CompanyUserInvite> builder)
        {
            builder.HasKey(q => new {q.CompanyId, q.Email});

            builder.HasOne(q => q.Company).WithMany(q => q.CompanyUserInvites).HasForeignKey(q => q.CompanyId);
        }
    }
}