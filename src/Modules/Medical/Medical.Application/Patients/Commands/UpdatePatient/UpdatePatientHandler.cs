namespace Medical.Application.Patients.Commands.UpdatePatient;

using MediatR;
using Medical.Application.Abstractions;
using Medical.Application.Patients.Queries.GetPatient;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

public sealed class UpdatePatientHandler
    : IRequestHandler<UpdatePatientCommand, Result<PatientResponse>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePatientHandler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PatientResponse>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (patient is null || patient.IsDeleted)
        {
            return Result<PatientResponse>.Failure(new Error("Patient.NotFound", "Patient not found."));
        }

        var updateResult = patient.Update(
            request.DateOfBirth,
            request.Gender,
            request.BloodType,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.Address);
        if (updateResult.IsFailure)
        {
            return Result<PatientResponse>.Failure(updateResult.Error);
        }

        await _patientRepository.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PatientResponse>.Success(new PatientResponse(
            patient.Id,
            patient.UserId,
            patient.DateOfBirth,
            patient.Gender,
            patient.BloodType,
            patient.EmergencyContactName,
            patient.EmergencyContactPhone,
            patient.Address));
    }
}
