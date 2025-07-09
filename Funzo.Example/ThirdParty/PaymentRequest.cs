namespace Funzo.Example.ThirdParty;

public record PaymentRequest(string DestinationAccountId, decimal Amount)
{
    public Option<DateTimeOffset> EffectiveDate { get; set; }
}

[Union<InvalidAccountError, InsufficientFundsError, InvalidEffectiveDateError>]
public partial class PaymentError;

public record InvalidAccountError(string AccountId);
public record InsufficientFundsError(decimal CurrentFunds);
public record InvalidEffectiveDateError(DateTimeOffset SuppliedDate);