namespace Medical.Application.Doctors.Commands.UpdateDoctor;

using MediatR;
using Medical.Application.Abstractions;
using Medical.Application.Doctors.Queries.GetDoctor;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

public sealed class UpdateDoctorHandler
    : IRequestHandler<UpdateDoctorCommand, Result<DoctorResponse>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDoctorHandler(
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DoctorResponse>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor is null || doctor.IsDeleted)
        {
            return Result<DoctorResponse>.Failure(new Error("Doctor.NotFound", "Doctor not found."));
        }

        var updateResult = doctor.Update(request.LicenseNumber, request.Bio, request.IsActive);
        if (updateResult.IsFailure)
        {
            return Result<DoctorResponse>.Failure(updateResult.Error);
        }

        await _doctorRepository.UpdateAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DoctorResponse>.Success(new DoctorResponse(
            doctor.Id,
            doctor.UserId,
            doctor.LicenseNumber,
            doctor.Bio));
    }
}
