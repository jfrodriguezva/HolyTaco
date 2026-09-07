using System.Data;

namespace HolyTac.SharedKernel;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
