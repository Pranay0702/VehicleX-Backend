using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VehicleX.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=aws-1-ap-northeast-1.pooler.supabase.com;" +
            "Port=6543;" +
            "Database=postgres;" +
            "Username=postgres.hnhjdpwffatesxppysoo;" +
            "Password=sNQOfvaD23vP;" +
            "SSL Mode=Require;" +
            "Trust Server Certificate=true"
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}