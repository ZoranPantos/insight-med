using InsightMed.LabRpcServer.Models;
using InsightMed.LabRpcServer.Services.Abstractions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace InsightMed.LabRpcServer.Services;

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
        command.CommandText = await File.ReadAllTextAsync("Sql/GetAllLabParameters.sql", cancellationToken);

        await using var reader = await command
            .ExecuteReaderAsync(cancellationToken)
            .ConfigureAwait(false);

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

    public async Task<IReadOnlyList<LabParameter>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids?.Distinct().ToList() ?? [];

        if (idList.Count == 0) return [];

        var result = new List<LabParameter>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        var sqlTemplate = await File.ReadAllTextAsync("Sql/GetLabParametersByIds.sql", cancellationToken);

        var parameterNames = new List<string>();

        for (int i = 0; i < idList.Count; i++)
        {
            var paramName = $"@id{i}";
            parameterNames.Add(paramName);
            command.Parameters.AddWithValue(paramName, idList[i]);
        }

        command.CommandText = sqlTemplate.Replace("{ID_LIST}", string.Join(", ", parameterNames));

        await using var reader = await command
            .ExecuteReaderAsync(cancellationToken)
            .ConfigureAwait(false);

        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
            command.CommandText = await File.ReadAllTextAsync("Sql/CreateDatabaseIfNotExists.sql", cancellationToken);

            command.Parameters.Add(new SqlParameter("@db", SqlDbType.NVarChar, 128) { Value = dbName });

            await command
                .ExecuteNonQueryAsync(cancellationToken)
                .ConfigureAwait(false);

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

            await using var command = connection.CreateCommand();
            command.CommandText = await File.ReadAllTextAsync("Sql/CreateTableAndSeed.sql", cancellationToken);

            await command
                .ExecuteNonQueryAsync(cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("Table dbo.LabParameters ensured and seeded if empty.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
