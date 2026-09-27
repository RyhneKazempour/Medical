namespace MyApp.Billing.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Billing.Application.Abstractions;
using MyApp.Billing.Domain.Entities;
using MyApp.Billing.Infrastructure.Persistence.DbContext;

internal sealed class PaymentRepository : IPaymentRepository
{
    private readonly BillingDbContext _dbContext;

    public PaymentRepository(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.AsNoTracking().OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<Payment?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.TransactionNumber == transactionNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.AsNoTracking().Where(p => p.Status == status).OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Payments.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Payments.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Payments.Remove(entity);
        await Task.CompletedTask;
    }
}
