namespace MyApp.Insurance.Domain.Entities;

using MyApp.Shared.Domain;
using System.ComponentModel.DataAnnotations.Schema;

public sealed class PatientInsurance : AuditableEntity
{
    public Guid PatientId { get; private set; }
    public Guid InsuranceId { get; private set; }
    public string PolicyNumber { get; private set; } = null!;
    public string? GroupNumber { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateOnly ValidFrom { get; private set; }
    public DateOnly? ValidUntil { get; private set; }

    [ForeignKey(nameof(InsuranceId))]
    public Insurance? Insurance { get; private set; }

    private PatientInsurance() { }

    private PatientInsurance(Guid id, Guid patientId, Guid insuranceId, string policyNumber, string? groupNumber,
        bool isPrimary, DateOnly validFrom, DateOnly? validUntil)
        : base(id)
    {
        PatientId = patientId;
        InsuranceId = insuranceId;
        PolicyNumber = policyNumber;
        GroupNumber = groupNumber;
        IsPrimary = isPrimary;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        IsDeleted = false;
    }

    public static Result<PatientInsurance> Create(Guid patientId, Guid insuranceId, string policyNumber,
        string? groupNumber, bool isPrimary, DateOnly validFrom, DateOnly? validUntil)
    {
        if (patientId == Guid.Empty)
            return Result<PatientInsurance>.Failure(new Error("PatientInsurance.PatientIdRequired", "Patient ID is required."));

        if (insuranceId == Guid.Empty)
            return Result<PatientInsurance>.Failure(new Error("PatientInsurance.InsuranceIdRequired", "Insurance ID is required."));

        if (string.IsNullOrWhiteSpace(policyNumber))
            return Result<PatientInsurance>.Failure(new Error("PatientInsurance.PolicyNumberRequired", "Policy number is required."));

        if (validUntil.HasValue && validUntil.Value < validFrom)
            return Result<PatientInsurance>.Failure(new Error("PatientInsurance.InvalidValidityPeriod", "Valid until date cannot be before valid from date."));

        var patientInsurance = new PatientInsurance(Guid.NewGuid(), patientId, insuranceId, policyNumber.Trim(), groupNumber?.Trim(),
            isPrimary, validFrom, validUntil);
        return Result<PatientInsurance>.Success(patientInsurance);
    }

    public Result Update(string? policyNumber, string? groupNumber, bool? isPrimary, DateOnly? validFrom, DateOnly? validUntil)
    {
        if (policyNumber is not null)
        {
            if (string.IsNullOrWhiteSpace(policyNumber))
                return Result.Failure(new Error("PatientInsurance.PolicyNumberRequired", "Policy number is required."));
            PolicyNumber = policyNumber.Trim();
        }

        if (groupNumber is not null) GroupNumber = groupNumber.Trim();
        if (isPrimary.HasValue) IsPrimary = isPrimary.Value;
        if (validFrom.HasValue) ValidFrom = validFrom.Value;
        if (validUntil.HasValue) ValidUntil = validUntil.Value;

        if (ValidUntil.HasValue && ValidUntil.Value < ValidFrom)
            return Result.Failure(new Error("PatientInsurance.InvalidValidityPeriod", "Valid until date cannot be before valid from date."));

        return Result.Success();
    }

    public void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;
}
