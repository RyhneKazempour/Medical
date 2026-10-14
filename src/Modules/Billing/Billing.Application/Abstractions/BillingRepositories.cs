using Billing.Domain.Entities;
using Shared.Application.Abstractions;

namespace Billing.Application.Abstractions;
public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
}
