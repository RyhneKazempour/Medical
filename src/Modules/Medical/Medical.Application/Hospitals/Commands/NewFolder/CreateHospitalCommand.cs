using MediatR;
using Medical.Application.Hospitals.Queries.GetHospital;
using Shared.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Application.Hospitals.Commands.NewFolder
{
    public sealed record CreateHospitalCommand(
    string? Name,
    string? Address,
    string? Phone,
    string? Email,
    bool? IsActive)
    : IRequest<Result<HospitalResponse>>;
}
