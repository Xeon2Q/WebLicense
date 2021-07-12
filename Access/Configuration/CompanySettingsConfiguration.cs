using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Models.Companies;

namespace WebLicense.Access.Configuration
{
    public sealed class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
    {
        public void Configure(EntityTypeBuilder<CompanySettings> builder)
        {
            builder.HasKey(q => new {q.CompanyId, q.ProviderCompanyId});
        }
    }
}