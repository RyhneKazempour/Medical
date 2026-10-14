namespace Medical.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.OpenApi;
using MediatR;
using Medical.Application.Doctors.Queries.GetDoctor;
using Medical.Application.Doctors.Commands.CreateDoctor;
using Medical.Application.Doctors.Commands.UpdateDoctor;
using Medical.Application.Doctors.Commands.SoftDeleteDoctor;
using Medical.Application.Doctors.Commands.RestoreDoctor;

public static class DoctorEndpoints
{
    public static IEndpointRouteBuilder MapDoctorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/medical/doctors")
            .WithTags("Doctors");

        group.MapPost("/", CreateDoctor)
            .WithName("CreateDoctor")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(DoctorResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{doctorId:guid}", GetDoctor)
            .WithName("GetDoctor")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(DoctorResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/", ListDoctors)
            .WithName("ListDoctors")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(IReadOnlyList<DoctorResponse>))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{doctorId:guid}", UpdateDoctor)
            .WithName("UpdateDoctor")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(DoctorResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{doctorId:guid}", SoftDeleteDoctor)
            .WithName("SoftDeleteDoctor")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{doctorId:guid}/restore", RestoreDoctor)
            .WithName("RestoreDoctor")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/by-specialization/{specializationId:guid}", ListDoctorsBySpecialization)
            .WithName("ListDoctorsBySpecialization")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(IReadOnlyList<DoctorResponse>))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/by-hospital/{hospitalId:guid}", ListDoctorsByHospital)
            .WithName("ListDoctorsByHospital")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(IReadOnlyList<DoctorResponse>))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        //group.MapPost("/associate-specialization", AssociateDoctorWithSpecialization)
        //    .WithName("AssociateDoctorWithSpecialization")
        //    .WithOpenApi()
        //    .Produces(StatusCodes.Status200OK)
        //    .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreateDoctor(
        CreateDoctorCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> GetDoctor(
        Guid doctorId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetDoctorQuery(doctorId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Doctor.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> ListDoctors(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListDoctorsQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> UpdateDoctor(
        Guid doctorId,
        UpdateDoctorCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var updatedCommand = command with { Id = doctorId };
        var result = await sender.Send(updatedCommand, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Doctor.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> SoftDeleteDoctor(
        Guid doctorId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteDoctorCommand(doctorId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Doctor.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> RestoreDoctor(
        Guid doctorId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RestoreDoctorCommand(doctorId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Doctor.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> ListDoctorsBySpecialization(
        Guid specializationId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListDoctorsBySpecializationQuery(specializationId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Specialization.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> ListDoctorsByHospital(
        Guid hospitalId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListDoctorsByHospitalQuery(hospitalId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Hospital.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    //private static async Task<IResult> AssociateDoctorWithSpecialization(
    //    AssociateDoctorWithSpecializationCommand command,
    //    ISender sender,
    //    CancellationToken cancellationToken)
    //{
    //    var result = await sender.Send(command, cancellationToken);

    //    return result.IsSuccess
    //        ? Results.Ok()
    //        : result.Error.Code switch
    //        {
    //            "Doctor.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
    //            "Doctor.Inactive" => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description }),
    //            "Specialization.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
    //            "Specialization.Inactive" => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description }),
    //            "Doctor.AlreadyHasSpecialization" => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description }),
    //            _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
    //        };
    //}
}
