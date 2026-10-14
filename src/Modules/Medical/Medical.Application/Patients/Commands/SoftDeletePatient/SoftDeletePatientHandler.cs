namespace Medical.Application.Patients.Commands.SoftDeletePatient;

using MediatR;
using Medical.Application.Abstractions;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

public sealed class SoftDeletePatientHandler
    : IRequestHandler<SoftDeletePatientCommand, Result>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeletePatientHandler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SoftDeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (patient is null || patient.IsDeleted)
        {
            return Result.Failure(new Error("Patient.NotFound", "Patient not found."));
        }

        patient.SoftDelete();
        await _patientRepository.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
