namespace ClearBank.DeveloperTest.Types;

public class Account
{
    public required string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public AccountStatus Status { get; set; }
    public AllowedPaymentSchemes AllowedPaymentSchemes { get; set; }
}
