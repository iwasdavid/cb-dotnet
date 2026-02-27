using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Logging;

namespace ClearBank.DeveloperTest.Services;

public class PaymentService(
    IAccountDataStoreFactory accountDataStoreFactory,
    ILogger<PaymentService> logger
) : IPaymentService
{
    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        logger.LogInformation("Processing payment request. Scheme: {PaymentScheme}, Amount: {Amount}", request.PaymentScheme, request.Amount);

        var accountDataStore = accountDataStoreFactory.Create();

        var account = accountDataStore.GetAccount(request.DebtorAccountNumber);

        if (!IsPaymentAllowed(request, account))
        {
            logger.LogWarning("Payment rejected. Scheme: {PaymentScheme}, Amount: {Amount}", request.PaymentScheme, request.Amount);
            return new MakePaymentResult(false);
        }

        account.Balance -= request.Amount;
        accountDataStore.UpdateAccount(account);

        logger.LogInformation("Payment accepted. Scheme: {PaymentScheme}, Amount: {Amount}, NewBalance: {NewBalance}", request.PaymentScheme, request.Amount, account.Balance);

        return new MakePaymentResult(true);
    }

    private static bool IsPaymentAllowed(MakePaymentRequest request, Account account)
    {
        if (account == null)
        {
            return false;
        }

        return request.PaymentScheme switch
        {
            PaymentScheme.Bacs => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs),
            PaymentScheme.FasterPayments =>
                account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
                && account.Balance >= request.Amount,
            PaymentScheme.Chaps =>
                account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps)
                && account.Status == AccountStatus.Live,
            _ => false
        };
    }
}
