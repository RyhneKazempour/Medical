namespace Medical.Application.Hospitals.Commands.RestoreHospital;

using MediatR;
using Shared.Domain.Common;

public sealed record RestoreHospitalCommand(Guid Id) : IRequest<Result>;