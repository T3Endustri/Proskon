using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace _01_Data.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();

        _ = optionsBuilder.UseSqlServer("Server=.;Initial Catalog=DBProskon;User ID=t3;Password=X-XP+45qAZrc+VmYLVB;Connect Timeout=60;MultipleActiveResultSets=True;TrustServerCertificate=true;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
