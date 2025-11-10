using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace InsightMed.LabRpcServer;

internal sealed class LabDbService : ILabDbService
{
    private readonly string _connectionString;
    private readonly ILogger<LabDbService> _logger;

    public LabDbService(IConfiguration config, ILogger<LabDbService> logger)
    {
        _connectionString = config.GetConnectionString("LabDb")
            ?? throw new InvalidOperationException("Connection string 'LabDb' is missing.");

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDatabaseExistsAsync(cancellationToken);
        await EnsureTableAndSeedAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LabParameter>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<LabParameter>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, LabParameterReferenceJson FROM dbo.LabParameters ORDER BY Id";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var referenceJson = reader.GetString(2);

            LabParameterReference? reference = null;

            try
            {
                reference = JsonSerializer.Deserialize<LabParameterReference>(referenceJson);

                if (reference is null || !reference.IsModelValid())
                {
                    _logger.LogError("Invalid or null LabParameterReference for Id {Id}. JSON: {Json}", id, referenceJson);

                    throw new InvalidOperationException($"Invalid LabParameterReference data for LabParameter Id {id}");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize LabParameterReference for Id {Id}. JSON: {Json}", id, referenceJson);
                throw;
            }

            result.Add(new LabParameter(id, name, reference));
        }

        return result;
    }

    private async Task EnsureDatabaseExistsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            var dbName = connectionStringBuilder.InitialCatalog;

            var masterCsb = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };

            await using var connection = new SqlConnection(masterCsb.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText =
              @"
                IF DB_ID(@db) IS NULL
                BEGIN
                  EXEC('CREATE DATABASE [' + @db + ']');
                END
                ";

            command.Parameters.Add(new SqlParameter("@db", SqlDbType.NVarChar, 128) { Value = dbName });

            await command.ExecuteNonQueryAsync(cancellationToken);

            _logger.LogInformation("Database {Db} ensured.", dbName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    private async Task EnsureTableAndSeedAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var sql =
              @"
                IF OBJECT_ID(N'dbo.LabParameters', N'U') IS NULL
                BEGIN
                  CREATE TABLE dbo.LabParameters
                  (
                    Id   INT NOT NULL PRIMARY KEY,
                    Name NVARCHAR(200) NOT NULL,
                    LabParameterReferenceJson NVARCHAR(MAX)
                  );
                END

                IF NOT EXISTS (SELECT 1 FROM dbo.LabParameters)
                BEGIN
                  INSERT INTO dbo.LabParameters (Id, Name, LabParameterReferenceJson)
                  VALUES (
                    1, 
                    N'test_name',
                    N'{""MinThreshold"":10.0,""MaxThreshold"":100.0,""Positive"":null}'
                  );
                END
                ";

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync(cancellationToken);

            _logger.LogInformation("Table dbo.LabParameters ensured and seeded if empty.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
