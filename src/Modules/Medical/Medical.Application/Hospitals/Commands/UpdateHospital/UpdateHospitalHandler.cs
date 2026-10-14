namespace Medical.Application.Hospitals.Commands.UpdateHospital;

using MediatR;
using Medical.Application.Abstractions;
using Medical.Application.Hospitals.Queries.GetHospital;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

public sealed class UpdateHospitalHandler
    : IRequestHandler<UpdateHospitalCommand, Result<HospitalResponse>>
{
    private readonly IHospitalRepository _hospitalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHospitalHandler(
        IHospitalRepository hospitalRepository,
        IUnitOfWork unitOfWork)
    {
        _hospitalRepository = hospitalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HospitalResponse>> Handle(UpdateHospitalCommand request, CancellationToken cancellationToken)
    {
        var hospital = await _hospitalRepository.GetByIdAsync(request.Id, cancellationToken);
        if (hospital is null || hospital.IsDeleted)
        {
            return Result<HospitalResponse>.Failure(new Error("Hospital.NotFound", "Hospital not found."));
        }

        var updateResult = hospital.Update(request.Name, request.Address, request.Phone, request.Email, request.IsActive);
        if (updateResult.IsFailure)
        {
            return Result<HospitalResponse>.Failure(updateResult.Error);
        }

        await _hospitalRepository.UpdateAsync(hospital, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HospitalResponse>.Success(new HospitalResponse(
            hospital.Id,
            hospital.Name,
            hospital.Address,
            hospital.Phone,
            hospital.Email));
    }
}
