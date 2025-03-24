using Microsoft.Extensions.Configuration;
using Npgsql;
using PetFoster.Application.Interfaces;
using System.Data;

namespace PetFoster.Infrastructure
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_configuration.GetConnectionString("Database"));
        }
    }
}
