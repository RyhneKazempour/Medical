# Healthcare/Clinic Management System - Database Schema Analysis & Architecture Proposal

## 1. DATABASE SCHEMA ANALYSIS

### 1.1 Original Conceptual Tables

```
users
roles
permissions
role_permissions
user_roles

hospitals
clinic (should be clinics)

doctors
specializations
doctor_specializations
doctor_hospitals

patients

doctor_schedules
doctor_schedule_exceptions
appointment_slots

appointments

payments

insurances
patient_insurances
```

---

## 2. IDENTIFIED ISSUES

### 2.1 Critical Relationship Inconsistencies

#### Issue 1: `doctor_hospitals.doctor_specializations_id`
- **Current**: References `doctors.id` (based on column name pattern)
- **Problem**: Column name suggests it should reference `doctor_specializations.id`
- **Domain Analysis**: A doctor-hospital assignment should be linked to a specific doctor-specialization combination (a doctor can work as a Cardiologist at Hospital A and as an Internist at Hospital B)
- **Correction**: Rename to `doctor_specialization_id` and reference `doctor_specializations.id`

#### Issue 2: `appointment_slots.doctor_schedules`
- **Current**: Appears to reference `doctor_hospitals.id` (based on naming inconsistency)
- **Problem**: Column name suggests it should reference `doctor_schedules.id`
- **Domain Analysis**: Appointment slots are generated from doctor schedules, not directly from doctor-hospital assignments
- **Correction**: Rename to `doctor_schedule_id` and reference `doctor_schedules.id`

### 2.2 Naming Inconsistencies

| Current | Corrected | Reason |
|---------|-----------|--------|
| `clinic` | `clinics` | PostgreSQL convention: plural table names |
| `doctor_hospitals.doctor_specializations_id` | `doctor_hospitals.doctor_specialization_id` | Singular FK column convention |
| `appointment_slots.doctor_schedules` | `appointment_slots.doctor_schedule_id` | Singular FK column convention |
| `role_permissions` | `role_permissions` | ✓ Already correct |
| `user_roles` | `user_roles` | ✓ Already correct |
| `doctor_specializations` | `doctor_specializations` | ✓ Already correct |
| `doctor_schedule_exceptions` | `doctor_schedule_exceptions` | ✓ Already correct |
| `appointment_slots` | `appointment_slots` | ✓ Already correct |
| `patient_insurances` | `patient_insurances` | ✓ Already correct |

### 2.3 Missing Constraints

| Table | Missing Constraint | Business Reason |
|-------|-------------------|-----------------|
| `user_roles` | Unique(`user_id`, `role_id`) | Prevent duplicate role assignments |
| `role_permissions` | Unique(`role_id`, `permission_id`) | Prevent duplicate permission assignments |
| `doctor_specializations` | Unique(`doctor_id`, `specialization_id`) | Prevent duplicate specialization assignments |
| `doctor_hospitals` | Unique(`doctor_specialization_id`, `clinic_id`) | A doctor-specialization can only be assigned once per clinic |
| `patient_insurances` | Partial Unique Index on `patient_id` where `is_primary = true` | Only one primary insurance per patient |
| `appointments` | Unique(`slot_id`) | Prevent double-booking (critical) |
| `doctor_schedules` | Unique(`doctor_hospital_id`, `day_of_week`) | One schedule per day per doctor-hospital |
| `appointment_slots` | Unique(`doctor_schedule_id`, `date`, `start_time`) | Prevent duplicate slots |

### 2.4 Normalization Issues

1. **`clinic` table** - Singular name violates PostgreSQL conventions
2. **`doctor_hospitals`** - Contains `room_number`, `start_contract_date`, `end_contract_date`, `is_active` - these belong to the assignment entity, which is correct
3. **`roles.context_type` / `roles.context_id`** vs **`user_roles.scope_type` / `user_roles.scope_id`** - These appear to model different concepts:
   - `roles.context_*`: Defines WHERE a role applies (e.g., Hospital=1, Clinic=5)
   - `user_roles.scope_*`: Defines WHERE a user has that role (e.g., Hospital=1, Clinic=5)
   - These are NOT redundant - they serve different purposes in a hierarchical RBAC system

### 2.5 Audit Field Inconsistencies

Tables with audit fields:
- `users`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `roles`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `permissions`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `hospitals`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `clinics`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `doctors`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `patients`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `specializations`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `doctor_specializations`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `doctor_hospitals`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `doctor_schedules`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `doctor_schedule_exceptions`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `appointment_slots`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `appointments`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `payments`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `insurances`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`
- `patient_insurances`: `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted`

Tables with `is_active` (business state):
- `users`
- `roles`
- `hospitals`
- `clinics`
- `doctors`
- `specializations`
- `doctor_hospitals`
- `doctor_schedules`
- `appointment_slots` (has `status` instead)
- `insurances`

**Key Distinction**: `is_active` = business availability; `is_deleted` = soft delete. These are orthogonal.

---

## 3. PROPOSED MODULE BOUNDARIES

```
src/
├── MyApp.Api/
│
├── Modules/
│   ├── Identity/
│   │   ├── Identity.Domain/
│   │   ├── Identity.Application/
│   │   ├── Identity.Infrastructure/
│   │   └── Identity.Api/
│   │
│   ├── Medical/
│   │   ├── Medical.Domain/
│   │   ├── Medical.Application/
│   │   ├── Medical.Infrastructure/
│   │   └── Medical.Api/
│   │
│   ├── Scheduling/
│   │   ├── Scheduling.Domain/
│   │   ├── Scheduling.Application/
│   │   ├── Scheduling.Infrastructure/
│   │   └── Scheduling.Api/
│   │
│   ├── Appointments/
│   │   ├── Appointments.Domain/
│   │   ├── Appointments.Application/
│   │   ├── Appointments.Infrastructure/
│   │   └── Appointments.Api/
│   │
│   ├── Insurance/
│   │   ├── Insurance.Domain/
│   │   ├── Insurance.Application/
│   │   ├── Insurance.Infrastructure/
│   │   └── Insurance.Api/
│   │
│   └── Billing/
│       ├── Billing.Domain/
│       ├── Billing.Application/
│       ├── Billing.Infrastructure/
│       └── Billing.Api/
│
└── Shared/
    ├── Shared.Domain/
    ├── Shared.Application/
    └── Shared.Infrastructure/
```

### Module Ownership Rationale

| Module | Owns Tables | Rationale |
|--------|-------------|-----------|
| **Identity** | `users`, `roles`, `permissions`, `role_permissions`, `user_roles` | Core authentication/authorization |
| **Medical** | `hospitals`, `clinics`, `doctors`, `specializations`, `doctor_specializations`, `doctor_hospitals`, `patients` | Healthcare provider/patient master data |
| **Scheduling** | `doctor_schedules`, `doctor_schedule_exceptions`, `appointment_slots` | Time-based availability management |
| **Appointments** | `appointments` | Patient booking workflow |
| **Insurance** | `insurances`, `patient_insurances` | Insurance coverage management |
| **Billing** | `payments` | Payment processing |

---

## 4. CORRECTED TABLE DEFINITIONS

### 4.1 Identity Module

```sql
-- users (unchanged structure, plural already)
CREATE TABLE users (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    email varchar(255) NOT NULL UNIQUE,
    password_hash varchar(255) NOT NULL,
    first_name varchar(100) NOT NULL,
    last_name varchar(100) NOT NULL,
    phone varchar(20),
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);

-- roles (unchanged structure)
CREATE TABLE roles (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(100) NOT NULL UNIQUE,
    description text,
    context_type varchar(50),  -- 'hospital', 'clinic', 'global'
    context_id int,            -- specific hospital/clinic ID
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);

-- permissions
CREATE TABLE permissions (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    resource varchar(100) NOT NULL,
    action varchar(50) NOT NULL,
    description text,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int,
    UNIQUE(resource, action)
);

-- role_permissions
CREATE TABLE role_permissions (
    role_id int NOT NULL REFERENCES roles(id),
    permission_id int NOT NULL REFERENCES permissions(id),
    PRIMARY KEY (role_id, permission_id)
);

-- user_roles
CREATE TABLE user_roles (
    user_id int NOT NULL REFERENCES users(id),
    role_id int NOT NULL REFERENCES roles(id),
    scope_type varchar(50),    -- 'hospital', 'clinic', 'global'
    scope_id int,              -- specific hospital/clinic ID
    PRIMARY KEY (user_id, role_id)
);
```

### 4.2 Medical Module

```sql
-- hospitals
CREATE TABLE hospitals (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(200) NOT NULL,
    address text,
    phone varchar(20),
    email varchar(255),
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);

-- clinics (CORRECTED: plural)
CREATE TABLE clinics (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    hospital_id int NOT NULL REFERENCES hospitals(id),
    name varchar(200) NOT NULL,
    description text,
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);

-- doctors
CREATE TABLE doctors (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id int NOT NULL REFERENCES users(id) UNIQUE,
    license_number varchar(50) NOT NULL UNIQUE,
    bio text,
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);

-- specializations
CREATE TABLE specializations (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(100) NOT NULL UNIQUE,
    description text,
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);

-- doctor_specializations
CREATE TABLE doctor_specializations (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    doctor_id int NOT NULL REFERENCES doctors(id),
    specialization_id int NOT NULL REFERENCES specializations(id),
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int,
    UNIQUE (doctor_id, specialization_id)
);

-- doctor_hospitals (CORRECTED: doctor_specialization_id)
CREATE TABLE doctor_hospitals (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    doctor_specialization_id int NOT NULL REFERENCES doctor_specializations(id),
    clinic_id int NOT NULL REFERENCES clinics(id),
    room_number varchar(20),
    start_contract_date date NOT NULL,
    end_contract_date date,
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int,
    UNIQUE (doctor_specialization_id, clinic_id)
);

-- patients
CREATE TABLE patients (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id int NOT NULL REFERENCES users(id) UNIQUE,
    date_of_birth date,
    gender varchar(20),
    blood_type varchar(5),
    emergency_contact_name varchar(200),
    emergency_contact_phone varchar(20),
    address text,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);
```

### 4.3 Scheduling Module

```sql
-- doctor_schedules
CREATE TABLE doctor_schedules (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    doctor_hospital_id int NOT NULL REFERENCES doctor_hospitals(id),
    day_of_week smallint NOT NULL CHECK (day_of_week BETWEEN 0 AND 6),  -- 0=Sunday
    start_time time NOT NULL,
    end_time time NOT NULL,
    slot_duration_minutes int NOT NULL DEFAULT 30 CHECK (slot_duration_minutes > 0),
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int,
    UNIQUE (doctor_hospital_id, day_of_week)
);

-- doctor_schedule_exceptions
CREATE TABLE doctor_schedule_exceptions (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    doctor_schedule_id int NOT NULL REFERENCES doctor_schedules(id),
    exception_date date NOT NULL,
    exception_type varchar(20) NOT NULL CHECK (exception_type IN ('cancelled', 'rescheduled', 'extended')),
    new_start_time time,
    new_end_time time,
    reason text,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int,
    UNIQUE (doctor_schedule_id, exception_date)
);

-- appointment_slots (CORRECTED: doctor_schedule_id)
CREATE TABLE appointment_slots (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    doctor_schedule_id int NOT NULL REFERENCES doctor_schedules(id),
    date date NOT NULL,
    start_time time NOT NULL,
    end_time time NOT NULL,
    status varchar(20) NOT NULL DEFAULT 'available' CHECK (status IN ('available', 'reserved', 'booked', 'blocked')),
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int,
    UNIQUE (doctor_schedule_id, date, start_time)
);
```

### 4.4 Appointments Module

```sql
-- appointments
CREATE TABLE appointments (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    patient_id int NOT NULL REFERENCES patients(id),
    slot_id int NOT NULL REFERENCES appointment_slots(id) UNIQUE,  -- CRITICAL: Prevents double booking
    status varchar(20) NOT NULL DEFAULT 'reserved' CHECK (status IN ('reserved', 'confirmed', 'cancelled', 'completed', 'no_show')),
    reserved_at timestamptz NOT NULL DEFAULT now(),
    confirmed_at timestamptz,
    cancelled_at timestamptz,
    cancel_reason text,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);
```

### 4.5 Insurance Module

```sql
-- insurances
CREATE TABLE insurances (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(200) NOT NULL,
    code varchar(50) NOT NULL UNIQUE,
    contact_phone varchar(20),
    contact_email varchar(255),
    address text,
    is_active boolean NOT NULL DEFAULT true,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);

-- patient_insurances
CREATE TABLE patient_insurances (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    patient_id int NOT NULL REFERENCES patients(id),
    insurance_id int NOT NULL REFERENCES insurances(id),
    policy_number varchar(100) NOT NULL,
    group_number varchar(100),
    is_primary boolean NOT NULL DEFAULT false,
    valid_from date NOT NULL,
    valid_until date,
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int,
    UNIQUE (patient_id, insurance_id)
);

-- Partial unique index for primary insurance (PostgreSQL)
CREATE UNIQUE INDEX uq_patient_insurances_primary 
    ON patient_insurances (patient_id) 
    WHERE is_primary = true AND is_deleted = false;
```

### 4.6 Billing Module

```sql
-- payments
CREATE TABLE payments (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    appointment_id int NOT NULL REFERENCES appointments(id),
    amount_cents bigint NOT NULL CHECK (amount_cents >= 0),  -- Money in smallest unit
    currency varchar(3) NOT NULL DEFAULT 'USD',
    transaction_number varchar(100) NOT NULL UNIQUE,
    status varchar(20) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'completed', 'failed', 'refunded', 'partially_refunded')),
    payment_method varchar(50),
    paid_at timestamptz,
    insurance_id int REFERENCES insurances(id),
    is_deleted boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_user_id int,
    updated_at timestamptz,
    updated_user_id int
);
```

---

## 5. INDEXING STRATEGY

```sql
-- Identity
CREATE INDEX ix_users_email ON users(email) WHERE is_deleted = false;
CREATE INDEX ix_users_is_active ON users(is_active) WHERE is_deleted = false;
CREATE INDEX ix_user_roles_user_id ON user_roles(user_id);
CREATE INDEX ix_user_roles_role_id ON user_roles(role_id);
CREATE INDEX ix_user_roles_scope ON user_roles(scope_type, scope_id);
CREATE INDEX ix_roles_context ON roles(context_type, context_id);

-- Medical
CREATE INDEX ix_clinics_hospital_id ON clinics(hospital_id) WHERE is_deleted = false;
CREATE INDEX ix_doctors_user_id ON doctors(user_id);
CREATE INDEX ix_doctors_license ON doctors(license_number);
CREATE INDEX ix_doctor_specializations_doctor_id ON doctor_specializations(doctor_id);
CREATE INDEX ix_doctor_specializations_spec_id ON doctor_specializations(specialization_id);
CREATE INDEX ix_doctor_hospitals_doctor_spec_id ON doctor_hospitals(doctor_specialization_id);
CREATE INDEX ix_doctor_hospitals_clinic_id ON doctor_hospitals(clinic_id);
CREATE INDEX ix_patients_user_id ON patients(user_id);

-- Scheduling
CREATE INDEX ix_doctor_schedules_doctor_hospital_id ON doctor_schedules(doctor_hospital_id) WHERE is_deleted = false;
CREATE INDEX ix_doctor_schedule_exceptions_schedule_id ON doctor_schedule_exceptions(doctor_schedule_id);
CREATE INDEX ix_doctor_schedule_exceptions_date ON doctor_schedule_exceptions(exception_date);
CREATE INDEX ix_appointment_slots_schedule_id ON appointment_slots(doctor_schedule_id) WHERE is_deleted = false;
CREATE INDEX ix_appointment_slots_date ON appointment_slots(date) WHERE is_deleted = false;
CREATE INDEX ix_appointment_slots_status ON appointment_slots(status) WHERE is_deleted = false;
CREATE INDEX ix_appointment_slots_date_status ON appointment_slots(date, status) WHERE is_deleted = false;

-- Appointments
CREATE INDEX ix_appointments_patient_id ON appointments(patient_id) WHERE is_deleted = false;
CREATE INDEX ix_appointments_slot_id ON appointments(slot_id);
CREATE INDEX ix_appointments_status ON appointments(status) WHERE is_deleted = false;
CREATE INDEX ix_appointments_reserved_at ON appointments(reserved_at);

-- Insurance
CREATE INDEX ix_patient_insurances_patient_id ON patient_insurances(patient_id) WHERE is_deleted = false;
CREATE INDEX ix_patient_insurances_insurance_id ON patient_insurances(insurance_id);

-- Billing
CREATE INDEX ix_payments_appointment_id ON payments(appointment_id) WHERE is_deleted = false;
CREATE INDEX ix_payments_transaction_number ON payments(transaction_number);
CREATE INDEX ix_payments_status ON payments(status) WHERE is_deleted = false;
```

---

## 6. APPOINTMENT CONCURRENCY STRATEGY

### Problem
Two patients simultaneously attempting to book the same appointment slot.

### Solution: Database-Level Concurrency Control

**Primary Strategy: Unique Constraint on `appointments.slot_id`**
```sql
UNIQUE (slot_id)  -- In appointments table
```
This guarantees at database level that only one appointment can reference a slot.

**Booking Flow:**
1. Transaction starts
2. `SELECT * FROM appointment_slots WHERE id = @slotId AND status = 'available' FOR UPDATE`
3. If found: `UPDATE appointment_slots SET status = 'reserved' WHERE id = @slotId`
4. `INSERT INTO appointments (patient_id, slot_id, status) VALUES (@patientId, @slotId, 'reserved')`
5. Commit transaction

**Why this works:**
- `FOR UPDATE` locks the slot row
- Unique constraint on `appointments.slot_id` is the ultimate guard
- If race occurs, one transaction fails with unique violation → retry with 409 Conflict

**Alternative for high contention:** Optimistic concurrency with `xmin` or version column on `appointment_slots`.

---

## 7. MODULE DEPENDENCY GRAPH

```
MyApp.Api
    ↓
Identity.Api → Identity.Application → Identity.Domain
    ↓                    ↓
Medical.Api → Medical.Application → Medical.Domain
    ↓                    ↓
Scheduling.Api → Scheduling.Application → Scheduling.Domain
    ↓                    ↓
Appointments.Api → Appointments.Application → Appointments.Domain
    ↓                    ↓
Insurance.Api → Insurance.Application → Insurance.Domain
    ↓                    ↓
Billing.Api → Billing.Application → Billing.Domain

All Infrastructure projects depend on their Application + Domain
All Application projects depend on their Domain + Shared.Application
All Domain projects depend on Shared.Domain (or nothing)
```

### Cross-Module Communication Rules

| From Module | To Module | Mechanism |
|-------------|-----------|-----------|
| Appointments | Medical (Patients) | `IPatientRepository` abstraction in Appointments.Application |
| Appointments | Scheduling (Slots) | `IAppointmentSlotRepository` abstraction |
| Appointments | Insurance | `IPatientInsuranceRepository` abstraction |
| Billing | Appointments | `IAppointmentRepository` abstraction |
| Scheduling | Medical (DoctorHospital) | `IDoctorHospitalRepository` abstraction |
| Medical (Patients) | Identity (Users) | `IUserRepository` abstraction |

**No module directly references another module's Infrastructure/DbContext.**

---

## 8. UNCHANGED DATABASE CONCEPTS

| Concept | Status | Reason |
|---------|--------|--------|
| `int` primary keys with identity | ✅ Keep | Simple, performant, sufficient for this scale |
| `created_at`, `created_user_id`, `updated_at`, `updated_user_id`, `is_deleted` | ✅ Keep | Standard audit pattern |
| `is_active` vs `is_deleted` distinction | ✅ Keep | Business state vs. soft delete are different |
| `roles.context_type/context_id` | ✅ Keep | Hierarchical RBAC context |
| `user_roles.scope_type/scope_id` | ✅ Keep | User-role assignment scope |
| `doctor_specializations` junction table | ✅ Keep | Many-to-many with audit fields |
| `appointment_slots.status` enum | ✅ Keep | Explicit slot state machine |
| `payments.amount_cents` (bigint) | ✅ Keep | Correct money representation |
| `patient_insurances.is_primary` | ✅ Keep | Primary insurance designation |

---

## 9. NECESSARY CHANGES SUMMARY

| Change | Type | Impact |
|--------|------|--------|
| `clinic` → `clinics` | Rename | PostgreSQL convention |
| `doctor_hospitals.doctor_specializations_id` → `doctor_specialization_id` | Fix FK | Correct domain relationship |
| `appointment_slots.doctor_schedules` → `doctor_schedule_id` | Fix FK | Correct hierarchy |
| Add missing unique constraints | Add constraints | Data integrity |
| Add partial unique index for primary insurance | Add index | Business rule enforcement |
| Add unique constraint on `appointments.slot_id` | Add constraint | Concurrency safety |
| Add CHECK constraints on enums | Add constraints | Data validity |
| Add indexes for query patterns | Add indexes | Performance |

---

## 10. IMPLEMENTATION PHASES

### Phase 1: Foundation (Current State + Corrections)
- Update existing boilerplate with corrected naming
- Add `clinics` table (rename from `clinic`)
- Add all missing constraints/indexes
- Create Medical, Scheduling, Appointments, Insurance, Billing module skeletons

### Phase 2: Core Domain Implementation
- Medical: Doctors, Patients, Hospitals, Clinics, Specializations
- Scheduling: DoctorSchedules, Exceptions, Slot Generation
- Appointments: Booking, Confirmation, Cancellation with concurrency

### Phase 3: Insurance & Billing
- Insurance management
- Payment processing

### Phase 4: Testing & Validation
- Unit tests for domain rules
- Integration tests for concurrency, booking flows
- Load testing for appointment booking

---

## 11. APPROVAL REQUESTED

Before proceeding with implementation, please confirm:

1. **Module boundaries** as proposed above
2. **Corrected relationships** for `doctor_hospitals` and `appointment_slots`
3. **Table naming** changes (especially `clinic` → `clinics`)
4. **Concurrency strategy** using unique constraint + row locking
5. **Partial unique index** for primary insurance
6. **Int money representation** (`amount_cents` bigint)

Once approved, I'll proceed with Phase 1 implementation.