using ClearBank.DeveloperTest.Data;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Data;

[TestFixture]
public class AccountDataStoreFactoryTests
{
    private ILogger<AccountDataStoreFactory> logger = null!;

    [SetUp]
    public void SetUp()
    {
        logger = A.Fake<ILogger<AccountDataStoreFactory>>();
    }

    [Test]
    public void Create_ShouldReturnBackupAccountDataStore_WhenDataStoreTypeIsBackup()
    {
        var options = Options.Create(new DataStoreOptions { DataStoreType = "Backup" });
        var accountDataStoreFactory = new AccountDataStoreFactory(options, logger);

        var result = accountDataStoreFactory.Create();

        Assert.That(result, Is.TypeOf<BackupAccountDataStore>());
    }

    [Test]
    public void Create_ShouldReturnBackupAccountDataStore_WhenDataStoreTypeIsBackupIgnoringCase()
    {
        var options = Options.Create(new DataStoreOptions { DataStoreType = "backup" });
        var accountDataStoreFactory = new AccountDataStoreFactory(options, logger);

        var result = accountDataStoreFactory.Create();

        Assert.That(result, Is.TypeOf<BackupAccountDataStore>());
    }

    [Test]
    public void Create_ShouldReturnAccountDataStore_WhenDataStoreTypeIsMissing()
    {
        var options = Options.Create(new DataStoreOptions());
        var accountDataStoreFactory = new AccountDataStoreFactory(options, logger);

        var result = accountDataStoreFactory.Create();

        Assert.That(result, Is.TypeOf<AccountDataStore>());
    }

    [Test]
    public void Create_ShouldReturnAccountDataStore_WhenDataStoreTypeIsUnknown()
    {
        var options = Options.Create(new DataStoreOptions { DataStoreType = "Live" });
        var accountDataStoreFactory = new AccountDataStoreFactory(options, logger);

        var result = accountDataStoreFactory.Create();

        Assert.That(result, Is.TypeOf<AccountDataStore>());
    }
}
