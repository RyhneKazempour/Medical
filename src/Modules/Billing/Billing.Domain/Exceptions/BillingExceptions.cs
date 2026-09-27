namespace MyApp.Billing.Domain.Exceptions;

using MyApp.Billing.Domain.Entities;
using MyApp.Shared.Domain.Exceptions;

public sealed class PaymentNotFoundException : DomainException
{
    public PaymentNotFoundException(int paymentId)
        : base($"Payment with ID '{paymentId}' was not found.")
    {
        PaymentId = paymentId;
    }

    public int PaymentId { get; }
}

public sealed class PaymentTransactionNumberExistsException : DomainException
{
    public PaymentTransactionNumberExistsException(string transactionNumber)
        : base($"Payment with transaction number '{transactionNumber}' already exists.")
    {
        TransactionNumber = transactionNumber;
    }

    public string TransactionNumber { get; }
}

public sealed class PaymentStateTransitionException : DomainException
{
    public PaymentStateTransitionException(int paymentId, PaymentStatus currentStatus, PaymentStatus targetStatus)
        : base($"Cannot transition payment '{paymentId}' from '{currentStatus}' to '{targetStatus}'.")
    {
        PaymentId = paymentId;
        CurrentStatus = currentStatus;
        TargetStatus = targetStatus;
    }

    public int PaymentId { get; }
    public PaymentStatus CurrentStatus { get; }
    public PaymentStatus TargetStatus { get; }
}
