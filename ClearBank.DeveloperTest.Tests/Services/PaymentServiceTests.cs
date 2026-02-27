using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Services;

[TestFixture]
public class PaymentServiceTests
{
    private ILogger<PaymentService> _logger = null!;
    private IAccountDataStore _accountDataStore = null!;
    private IAccountDataStoreFactory _accountDataStoreFactory = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = A.Fake<ILogger<PaymentService>>();
        _accountDataStore = A.Fake<IAccountDataStore>();
        _accountDataStoreFactory = A.Fake<IAccountDataStoreFactory>();
    }

    [Test]
    public void MakePayment_ShouldReturnFalse_WhenAccountIsNull()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(null!);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Bacs
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MakePayment_ShouldReturnFalse_WhenPaymentSchemeIsUnknown()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000,
            Status = AccountStatus.Live
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = (PaymentScheme)999
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MakePayment_ShouldReturnFalse_WhenBacsIsNotAllowed()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var accountDataStore = A.Fake<IAccountDataStore>();

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
        };

        A.CallTo(() => accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Bacs
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MakePayment_ShouldReturnTrue_WhenBacsIsAllowed()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Bacs
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void MakePayment_ShouldReturnFalse_WhenFasterPaymentsIsNotAllowed()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.FasterPayments
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MakePayment_ShouldReturnFalse_WhenFasterPaymentsBalanceIsInsufficient()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 50
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.FasterPayments
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MakePayment_ShouldReturnTrue_WhenFasterPaymentsIsAllowedAndBalanceIsSufficient()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 150
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.FasterPayments
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void MakePayment_ShouldReturnFalse_WhenChapsIsNotAllowed()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live,
            Balance = 1000
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Chaps
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MakePayment_ShouldReturnFalse_WhenChapsAccountIsNotLive()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Disabled,
            Balance = 1000
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Chaps
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MakePayment_ShouldReturnTrue_WhenChapsIsAllowedAndAccountIsLive()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live,
            Balance = 1000
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Chaps
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void MakePayment_ShouldUpdateAccountBalance_WhenPaymentIsAllowed()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Bacs
        };

        // Act
        paymentService.MakePayment(request);

        // Assert
        Assert.That(account.Balance, Is.EqualTo(900));
        A.CallTo(() => _accountDataStore.UpdateAccount(account)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void MakePayment_ShouldUpdateAccountInDataStore_WhenPaymentIsAllowed()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Bacs
        };

        // Act
        var _ = paymentService.MakePayment(request);

        // Assert
        A.CallTo(() => _accountDataStore.UpdateAccount(account)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void MakePayment_ShouldNotUpdateAccount_WhenPaymentIsRejected()
    {
        // Arrange
        const string DebtorAccountNumber = "12345678";

        var account = new Account
        {
            AccountNumber = DebtorAccountNumber,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000
        };

        A.CallTo(() => _accountDataStore.GetAccount(DebtorAccountNumber)).Returns(account);
        A.CallTo(() => _accountDataStoreFactory.Create()).Returns(_accountDataStore);

        var paymentService = new PaymentService(_accountDataStoreFactory, _logger);
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = DebtorAccountNumber,
            CreditorAccountNumber = "87654321",
            Amount = 100,
            PaymentScheme = PaymentScheme.Chaps
        };

        // Act
        var result = paymentService.MakePayment(request);

        // Assert
        Assert.That(result.Success, Is.False);
        A.CallTo(() => _accountDataStore.UpdateAccount(A<Account>._)).MustNotHaveHappened();
    }
}