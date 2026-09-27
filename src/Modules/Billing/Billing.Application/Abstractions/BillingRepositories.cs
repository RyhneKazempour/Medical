namespace MyApp.Billing.Application.Abstractions;

using MyApp.Billing.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
}
