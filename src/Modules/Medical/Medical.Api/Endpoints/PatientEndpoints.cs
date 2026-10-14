namespace Medical.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.OpenApi;
using MediatR;
using Medical.Application.Patients.Commands.CreatePatient;
using Medical.Application.Patients.Queries.GetPatient;
using MyApp.M.Application.Patients.Queries.GetPatient;
using Medical.Application.Patients.Commands.UpdatePatient;
using Medical.Application.Patients.Commands.SoftDeletePatient;
using Medical.Application.Patients.Commands.RestorePatient;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/medical/patients")
            .WithTags("Patients");

        group.MapPost("/", CreatePatient)
            .WithName("CreatePatient")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(PatientResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{patientId:guid}", GetPatient)
            .WithName("GetPatient")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(PatientResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/", ListPatients)
            .WithName("ListPatients")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(IReadOnlyList<PatientResponse>))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{patientId:guid}", UpdatePatient)
            .WithName("UpdatePatient")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(PatientResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{patientId:guid}", SoftDeletePatient)
            .WithName("SoftDeletePatient")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{patientId:guid}/restore", RestorePatient)
            .WithName("RestorePatient")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreatePatient(
        CreatePatientCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> GetPatient(
        Guid patientId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetPatientQuery(patientId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Patient.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> ListPatients(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListPatientsQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> UpdatePatient(
        Guid patientId,
        UpdatePatientCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var updatedCommand = command with { Id = patientId };
        var result = await sender.Send(updatedCommand, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Patient.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> SoftDeletePatient(
        Guid patientId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeletePatientCommand(patientId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Patient.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> RestorePatient(
        Guid patientId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RestorePatientCommand(patientId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Patient.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }
}
