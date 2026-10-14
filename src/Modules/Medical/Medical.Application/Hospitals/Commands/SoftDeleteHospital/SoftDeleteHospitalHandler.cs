namespace Medical.Application.Hospitals.Commands.SoftDeleteHospital;

using MediatR;
using Medical.Application.Abstractions;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

public sealed class SoftDeleteHospitalHandler
    : IRequestHandler<SoftDeleteHospitalCommand, Result>
{
    private readonly IHospitalRepository _hospitalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteHospitalHandler(
        IHospitalRepository hospitalRepository,
        IUnitOfWork unitOfWork)
    {
        _hospitalRepository = hospitalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SoftDeleteHospitalCommand request, CancellationToken cancellationToken)
    {
        var hospital = await _hospitalRepository.GetByIdAsync(request.Id, cancellationToken);
        if (hospital is null || hospital.IsDeleted)
        {
            return Result.Failure(new Error("Hospital.NotFound", "Hospital not found."));
        }

        hospital.SoftDelete();
        await _hospitalRepository.UpdateAsync(hospital, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
