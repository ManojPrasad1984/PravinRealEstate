using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstateManagement.Functions.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddDbContext<FunctionEstateContext>(options =>
            options.UseSqlServer(
                Environment.GetEnvironmentVariable("EstateConnectionString")
                ?? "server=(LocalDB)\\MSSQLLocalDB;database=EstateDBExam;trusted_connection=true;trust server certificate=true"));
    })
    .Build();

host.Run();
