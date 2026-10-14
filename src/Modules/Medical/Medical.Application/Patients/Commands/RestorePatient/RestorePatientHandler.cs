namespace Medical.Application.Patients.Commands.RestorePatient;

using MediatR;
using Medical.Application.Abstractions;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

public sealed class RestorePatientHandler
    : IRequestHandler<RestorePatientCommand, Result>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RestorePatientHandler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RestorePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (patient is null || !patient.IsDeleted)
        {
            return Result.Failure(new Error("Patient.NotFound", "Patient not found."));
        }

        patient.Restore();
        await _patientRepository.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
