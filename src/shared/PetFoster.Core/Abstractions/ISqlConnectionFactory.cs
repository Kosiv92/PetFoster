using System.Data;

namespace PetFoster.Core.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
