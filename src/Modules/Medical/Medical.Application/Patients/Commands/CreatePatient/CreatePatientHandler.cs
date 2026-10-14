namespace Medical.Application.Patients.Commands.CreatePatient;

using MediatR;
using Medical.Domain.Entities;
using Shared.Domain.Common;
using Shared.Application.Abstractions;
using Identity.Contracts.Abstractions;
using Medical.Application.Abstractions;

public sealed class CreatePatientHandler
    : IRequestHandler<CreatePatientCommand, Result<Guid>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUserExistsChecker _userExistsChecker;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientHandler(
        IPatientRepository patientRepository,
        IUserExistsChecker userExistsChecker,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _userExistsChecker = userExistsChecker;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userExistsChecker.ExistsByIdAsync(request.UserId, cancellationToken);
        if (!userExists)
        {
            return Result<Guid>.Failure(new Error("Patient.UserNotFound", "User does not exist."));
        }

        var patientExists = await _patientRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (patientExists is not null)
        {
            return Result<Guid>.Failure(new Error("Patient.AlreadyExists", "User is already registered as a patient."));
        }

        var patientResult = Patient.Create(
            request.UserId,
            request.DateOfBirth,
            request.Gender,
            request.BloodType,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.Address);
        if (patientResult.IsFailure)
        {
            return Result<Guid>.Failure(patientResult.Error);
        }

        var patient = patientResult.Value;
        await _patientRepository.AddAsync(patient, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (IsUniqueConstraintViolation(ex))
        {
            return Result<Guid>.Failure(new Error("Patient.AlreadyExists", "User is already registered as a patient."));
        }

        return Result<Guid>.Success(patient.Id);
    }

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        return ex.Message.Contains("uq_patients_user_id", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || (ex.InnerException?.Message.Contains("uq_patients_user_id", StringComparison.OrdinalIgnoreCase) ?? false)
            || (ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ?? false);
    }
}
