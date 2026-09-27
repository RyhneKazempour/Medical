namespace MyApp.Insurance.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Insurance.Application.Abstractions;
using MyApp.Insurance.Domain.Entities;
using MyApp.Insurance.Infrastructure.Persistence.DbContext;

internal sealed class PatientInsuranceRepository : IPatientInsuranceRepository
{
    private readonly InsuranceDbContext _dbContext;

    public PatientInsuranceRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientInsurance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientInsurances.AsNoTracking().FirstOrDefaultAsync(pi => pi.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PatientInsurance>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientInsurances.AsNoTracking().OrderBy(pi => pi.PatientId).ThenBy(pi => pi.InsuranceId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PatientInsurance>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientInsurances.AsNoTracking().Where(pi => pi.PatientId == patientId).OrderBy(pi => pi.InsuranceId).ToListAsync(cancellationToken);
    }

    public async Task<PatientInsurance?> GetPrimaryByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientInsurances.AsNoTracking().FirstOrDefaultAsync(pi => pi.PatientId == patientId && pi.IsPrimary, cancellationToken);
    }

    public async Task<PatientInsurance?> GetByPatientAndInsuranceAsync(Guid patientId, Guid insuranceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientInsurances.AsNoTracking().FirstOrDefaultAsync(pi => pi.PatientId == patientId && pi.InsuranceId == insuranceId, cancellationToken);
    }

    public async Task AddAsync(PatientInsurance entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.PatientInsurances.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(PatientInsurance entity, CancellationToken cancellationToken = default)
    {
        _dbContext.PatientInsurances.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(PatientInsurance entity, CancellationToken cancellationToken = default)
    {
        _dbContext.PatientInsurances.Remove(entity);
        await Task.CompletedTask;
    }
}
