namespace Medical.Application.Doctors.Commands.SoftDeleteDoctor;

using MediatR;
using Medical.Application.Abstractions;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

public sealed class SoftDeleteDoctorHandler
    : IRequestHandler<SoftDeleteDoctorCommand, Result>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteDoctorHandler(
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SoftDeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor is null || doctor.IsDeleted)
        {
            return Result.Failure(new Error("Doctor.NotFound", "Doctor not found."));
        }

        doctor.SoftDelete();
        await _doctorRepository.UpdateAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
