using Microsoft.Extensions.Configuration;
using System;
using Npgsql;

namespace AmbroBlogProject.Data
{
    public class ConnectionService
    {
        public static string GetConnectionString(IConfiguration config) {
            var connectionString = config.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var dbUri = new Uri(databaseUrl);
            var userInfo = dbUri.UserInfo.Split(":");
            return new NpgsqlConnectionStringBuilder()
            {
                Host = dbUri.Host,
                Port = dbUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = dbUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            }.ToString();
        }
    }
}
