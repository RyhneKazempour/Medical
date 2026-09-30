using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "doctor_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    doctor_hospital_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    slot_duration_minutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_schedules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "appointment_slots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    doctor_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_slots_doctor_schedules_doctor_schedule_id",
                        column: x => x.doctor_schedule_id,
                        principalTable: "doctor_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "doctor_schedule_exceptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    doctor_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exception_date = table.Column<DateOnly>(type: "date", nullable: false),
                    exception_type = table.Column<int>(type: "integer", nullable: false),
                    new_start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    new_end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_schedule_exceptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_doctor_schedule_exceptions_doctor_schedules_doctor_schedule~",
                        column: x => x.doctor_schedule_id,
                        principalTable: "doctor_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_appointment_slots_date",
                table: "appointment_slots",
                column: "date",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_slots_date_status",
                table: "appointment_slots",
                columns: new[] { "date", "status" },
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_slots_doctor_schedule_id",
                table: "appointment_slots",
                column: "doctor_schedule_id",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_slots_status",
                table: "appointment_slots",
                column: "status",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "uq_appointment_slots_schedule_date_time",
                table: "appointment_slots",
                columns: new[] { "doctor_schedule_id", "date", "start_time" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_doctor_schedule_exceptions_date",
                table: "doctor_schedule_exceptions",
                column: "exception_date",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_doctor_schedule_exceptions_doctor_schedule_id",
                table: "doctor_schedule_exceptions",
                column: "doctor_schedule_id",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "uq_doctor_schedule_exceptions_schedule_date",
                table: "doctor_schedule_exceptions",
                columns: new[] { "doctor_schedule_id", "exception_date" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_doctor_schedules_doctor_hospital_id",
                table: "doctor_schedules",
                column: "doctor_hospital_id",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_doctor_schedules_is_active",
                table: "doctor_schedules",
                column: "is_active",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "uq_doctor_schedules_doctor_hospital_day",
                table: "doctor_schedules",
                columns: new[] { "doctor_hospital_id", "day_of_week" },
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment_slots");

            migrationBuilder.DropTable(
                name: "doctor_schedule_exceptions");

            migrationBuilder.DropTable(
                name: "doctor_schedules");
        }
    }
}
