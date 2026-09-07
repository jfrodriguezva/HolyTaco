using System.Data;
using HolyTac.SharedKernel;
using Microsoft.Data.SqlClient;

namespace HolyTac.Reservations.Infrastructure;

public class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqlConnection(connectionString);
}
