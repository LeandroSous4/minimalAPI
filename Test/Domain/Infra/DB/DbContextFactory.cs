using System;
using Microsoft.EntityFrameworkCore;
using minimalAPI.Infra.DB; 

namespace Test.Domain.Infra.DB
{
    public static class DbContextFactory
    {
        public static DbContexto CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<DbContexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var ctx = new DbContexto(options);
            return ctx;
        }
    }
}
