namespace MyApp.Billing.Domain.Entities;

using MyApp.Shared.Domain;

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    PartiallyRefunded = 5
}

public enum PaymentMethod
{
    Cash = 1,
    Card = 2,
    Insurance = 3,
    BankTransfer = 4,
    Other = 5
}

public sealed class Payment : AuditableEntity
{
    public Guid AppointmentId { get; private set; }
    public long AmountCents { get; private set; }
    public string Currency { get; private set; } = null!;
    public string TransactionNumber { get; private set; } = null!;
    public PaymentStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public Guid? InsuranceId { get; private set; }

    private Payment() { }

    private Payment(Guid id, Guid appointmentId, long amountCents, string currency, string transactionNumber,
        PaymentMethod paymentMethod, Guid? insuranceId)
        : base(id)
    {
        AppointmentId = appointmentId;
        AmountCents = amountCents;
        Currency = currency;
        TransactionNumber = transactionNumber;
        PaymentMethod = paymentMethod;
        InsuranceId = insuranceId;
        Status = PaymentStatus.Pending;
        IsDeleted = false;
    }

    public static Result<Payment> Create(Guid appointmentId, long amountCents, string currency, string transactionNumber,
        PaymentMethod paymentMethod, Guid? insuranceId)
    {
        if (appointmentId == Guid.Empty)
            return Result<Payment>.Failure(new Error("Payment.AppointmentIdRequired", "Appointment ID is required."));

        if (amountCents < 0)
            return Result<Payment>.Failure(new Error("Payment.InvalidAmount", "Amount cannot be negative."));

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Payment>.Failure(new Error("Payment.CurrencyRequired", "Currency is required."));

        if (string.IsNullOrWhiteSpace(transactionNumber))
            return Result<Payment>.Failure(new Error("Payment.TransactionNumberRequired", "Transaction number is required."));

        var payment = new Payment(Guid.NewGuid(), appointmentId, amountCents, currency.Trim().ToUpperInvariant(),
            transactionNumber.Trim(), paymentMethod, insuranceId);
        return Result<Payment>.Success(payment);
    }

    public Result Complete()
    {
        if (Status == PaymentStatus.Completed)
            return Result.Failure(new Error("Payment.AlreadyCompleted", "Payment is already completed."));

        if (Status == PaymentStatus.Refunded)
            return Result.Failure(new Error("Payment.AlreadyRefunded", "Cannot complete a refunded payment."));

        Status = PaymentStatus.Completed;
        PaidAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Fail()
    {
        if (Status == PaymentStatus.Completed)
            return Result.Failure(new Error("Payment.AlreadyCompleted", "Cannot fail a completed payment."));

        Status = PaymentStatus.Failed;
        return Result.Success();
    }

    public Result Refund()
    {
        if (Status != PaymentStatus.Completed)
            return Result.Failure(new Error("Payment.NotCompleted", "Only completed payments can be refunded."));

        Status = PaymentStatus.Refunded;
        return Result.Success();
    }

    public Result PartialRefund(long refundAmountCents)
    {
        if (Status != PaymentStatus.Completed && Status != PaymentStatus.PartiallyRefunded)
            return Result.Failure(new Error("Payment.InvalidStateForPartialRefund", "Only completed or partially refunded payments can be partially refunded."));

        if (refundAmountCents <= 0 || refundAmountCents > AmountCents)
            return Result.Failure(new Error("Payment.InvalidRefundAmount", "Refund amount must be positive and not exceed original amount."));

        Status = PaymentStatus.PartiallyRefunded;
        return Result.Success();
    }
}
