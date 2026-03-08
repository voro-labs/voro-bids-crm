using VoroBidsCrm.Infrastructure.Factories;

namespace VoroBidsCrm.Infrastructure.Seeds
{
    public interface IDataSeeder
    {
        Task SeedAsync(JasmimDbContext context);
    }
}