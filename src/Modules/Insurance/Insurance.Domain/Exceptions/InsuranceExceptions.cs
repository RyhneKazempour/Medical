namespace MyApp.Insurance.Domain.Exceptions;

using MyApp.Shared.Domain.Exceptions;

public sealed class InsuranceNotFoundException : DomainException
{
    public InsuranceNotFoundException(int insuranceId)
        : base($"Insurance with ID '{insuranceId}' was not found.")
    {
        InsuranceId = insuranceId;
    }

    public int InsuranceId { get; }
}

public sealed class PatientInsuranceNotFoundException : DomainException
{
    public PatientInsuranceNotFoundException(int patientInsuranceId)
        : base($"Patient insurance with ID '{patientInsuranceId}' was not found.")
    {
        PatientInsuranceId = patientInsuranceId;
    }

    public int PatientInsuranceId { get; }
}

public sealed class PrimaryInsuranceAlreadyExistsException : DomainException
{
    public PrimaryInsuranceAlreadyExistsException(int patientId)
        : base($"Patient '{patientId}' already has a primary insurance.")
    {
        PatientId = patientId;
    }

    public int PatientId { get; }
}
