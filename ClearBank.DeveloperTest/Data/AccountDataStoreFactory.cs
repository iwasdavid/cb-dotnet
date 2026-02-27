using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClearBank.DeveloperTest.Data;

public class AccountDataStoreFactory(
    IOptions<DataStoreOptions> dataStoreOptions,
    ILogger<AccountDataStoreFactory> logger
) : IAccountDataStoreFactory
{
    private const string BackupDataStoreType = "Backup";

    public IAccountDataStore Create()
    {
        var dataStoreType = dataStoreOptions.Value.DataStoreType;

        if (string.Equals(dataStoreType, BackupDataStoreType, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Using backup account data store.");
            return new BackupAccountDataStore();
        }

        if (string.IsNullOrWhiteSpace(dataStoreType))
        {
            logger.LogInformation("DataStoreType config is missing. Defaulting to primary account data store.");
        }
        else
        {
            logger.LogInformation("Unknown DataStoreType config value '{DataStoreType}'. Defaulting to primary account data store.",  dataStoreType);
        }

        return new AccountDataStore();
    }
}