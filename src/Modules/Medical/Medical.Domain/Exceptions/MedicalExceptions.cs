namespace MyApp.Medical.Domain.Exceptions;

using MyApp.Shared.Domain.Exceptions;

public sealed class HospitalNotFoundException : DomainException
{
    public HospitalNotFoundException(int hospitalId)
        : base($"Hospital with ID '{hospitalId}' was not found.")
    {
        HospitalId = hospitalId;
    }

    public int HospitalId { get; }
}

public sealed class ClinicNotFoundException : DomainException
{
    public ClinicNotFoundException(int clinicId)
        : base($"Clinic with ID '{clinicId}' was not found.")
    {
        ClinicId = clinicId;
    }

    public int ClinicId { get; }
}

public sealed class DoctorNotFoundException : DomainException
{
    public DoctorNotFoundException(int doctorId)
        : base($"Doctor with ID '{doctorId}' was not found.")
    {
        DoctorId = doctorId;
    }

    public DoctorNotFoundException(string licenseNumber)
        : base($"Doctor with license number '{licenseNumber}' was not found.")
    {
        LicenseNumber = licenseNumber;
    }

    public int? DoctorId { get; }
    public string? LicenseNumber { get; }
}

public sealed class PatientNotFoundException : DomainException
{
    public PatientNotFoundException(int patientId)
        : base($"Patient with ID '{patientId}' was not found.")
    {
        PatientId = patientId;
    }

    public int PatientId { get; }
}

public sealed class SpecializationNotFoundException : DomainException
{
    public SpecializationNotFoundException(int specializationId)
        : base($"Specialization with ID '{specializationId}' was not found.")
    {
        SpecializationId = specializationId;
    }

    public int SpecializationId { get; }
}

public sealed class DoctorSpecializationNotFoundException : DomainException
{
    public DoctorSpecializationNotFoundException(int doctorSpecializationId)
        : base($"Doctor specialization with ID '{doctorSpecializationId}' was not found.")
    {
        DoctorSpecializationId = doctorSpecializationId;
    }

    public int DoctorSpecializationId { get; }
}

public sealed class DoctorHospitalNotFoundException : DomainException
{
    public DoctorHospitalNotFoundException(int doctorHospitalId)
        : base($"Doctor hospital assignment with ID '{doctorHospitalId}' was not found.")
    {
        DoctorHospitalId = doctorHospitalId;
    }

    public int DoctorHospitalId { get; }
}

public sealed class DoctorLicenseNumberExistsException : DomainException
{
    public DoctorLicenseNumberExistsException(string licenseNumber)
        : base($"Doctor with license number '{licenseNumber}' already exists.")
    {
        LicenseNumber = licenseNumber;
    }

    public string LicenseNumber { get; }
}

public sealed class DoctorAlreadyHasSpecializationException : DomainException
{
    public DoctorAlreadyHasSpecializationException(int doctorId, int specializationId)
        : base($"Doctor '{doctorId}' already has specialization '{specializationId}'.")
    {
        DoctorId = doctorId;
        SpecializationId = specializationId;
    }

    public int DoctorId { get; }
    public int SpecializationId { get; }
}
