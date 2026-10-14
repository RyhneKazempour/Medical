namespace Medical.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.OpenApi;
using MediatR;
using Medical.Application.Hospitals.Commands.UpdateHospital;
using Medical.Application.Hospitals.Queries.GetHospital;
using Medical.Application.Hospitals.Commands.SoftDeleteHospital;
using Medical.Application.Hospitals.Commands.RestoreHospital;
using Medical.Application.Hospitals.Commands.NewFolder;

public static class HospitalEndpoints
{
    public static IEndpointRouteBuilder MapHospitalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/medical/hospitals")
            .WithTags("Hospitals");

        group.MapPost("/", CreateHospital)
            .WithName("CreateHospital")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(HospitalResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{hospitalId:guid}", GetHospital)
            .WithName("GetHospital")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(HospitalResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/", ListHospitals)
            .WithName("ListHospitals")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(IReadOnlyList<HospitalResponse>))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{hospitalId:guid}", UpdateHospital)
            .WithName("UpdateHospital")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(HospitalResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{hospitalId:guid}", SoftDeleteHospital)
            .WithName("SoftDeleteHospital")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{hospitalId:guid}/restore", RestoreHospital)
            .WithName("RestoreHospital")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateHospital(
        CreateHospitalCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> GetHospital(
        Guid hospitalId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetHospitalQuery(hospitalId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Hospital.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> ListHospitals(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListHospitalsQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> UpdateHospital(
        Guid hospitalId,
        UpdateHospitalCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var updatedCommand = command with { Id = hospitalId };
        var result = await sender.Send(updatedCommand, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Hospital.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> SoftDeleteHospital(
        Guid hospitalId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteHospitalCommand(hospitalId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Hospital.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> RestoreHospital(
        Guid hospitalId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RestoreHospitalCommand(hospitalId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Hospital.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }
}
