namespace MyApp.Medical.Application.Commands;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;

public sealed class CreateDoctorHandler
    : IRequestHandler<CreateDoctorCommand, Result<Guid>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDoctorHandler(
        IDoctorRepository doctorRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsByIdAsync(request.UserId, cancellationToken);
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