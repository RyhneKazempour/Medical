namespace Medical.Application.Doctors.Commands.CreateDoctor;

using MediatR;
using Medical.Application.Abstractions;
using Medical.Domain.Entities;
using Shared.Domain.Common;
using Shared.Application.Abstractions;
using Identity.Contracts.Abstractions;

public sealed class CreateDoctorHandler
    : IRequestHandler<CreateDoctorCommand, Result<Guid>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUserExistsChecker _userExistsChecker;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDoctorHandler(
        IDoctorRepository doctorRepository,
        IUserExistsChecker userExistsChecker,
        IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _userExistsChecker = userExistsChecker;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userExistsChecker.ExistsByIdAsync(request.UserId, cancellationToken);
        if (!userExists)
        {
            return Result<Guid>.Failure(new Error("Doctor.UserNotFound", "User does not exist."));
        }

        var doctorExists = await _doctorRepository.ExistsForUserAsync(request.UserId, cancellationToken);
        if (doctorExists)
        {
            return Result<Guid>.Failure(new Error("Doctor.AlreadyExists", "User is already registered as a doctor."));
        }

        var doctorResult = Doctor.Create(request.UserId, request.LicenseNumber, request.Bio);
        if (doctorResult.IsFailure)
        {
            return Result<Guid>.Failure(doctorResult.Error);
        }

        var doctor = doctorResult.Value;
        await _doctorRepository.AddAsync(doctor, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (IsUniqueConstraintViolation(ex))
        {
            return Result<Guid>.Failure(new Error("Doctor.AlreadyExists", "User is already registered as a doctor."));
        }

        return Result<Guid>.Success(doctor.Id);
    }

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        return ex.Message.Contains("uq_doctors_user_id", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || (ex.InnerException?.Message.Contains("uq_doctors_user_id", StringComparison.OrdinalIgnoreCase) ?? false)
            || (ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ?? false);
    }
}