namespace Medical.Api;

using Medical.Api.Endpoints;
using Microsoft.AspNetCore.Routing;

public static class MedicalApiExtensions
{
    public static IEndpointRouteBuilder MapMedicalApi(this IEndpointRouteBuilder app)
    {
        app.MapDoctorEndpoints();
        app.MapPatientEndpoints();
        app.MapHospitalEndpoints();

        return app;
    }
}
