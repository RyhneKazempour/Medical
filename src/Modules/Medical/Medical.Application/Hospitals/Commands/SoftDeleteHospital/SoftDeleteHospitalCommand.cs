namespace Medical.Application.Hospitals.Commands.SoftDeleteHospital;

using MediatR;
using Shared.Domain.Common;

public sealed record SoftDeleteHospitalCommand(Guid Id) : IRequest<Result>;