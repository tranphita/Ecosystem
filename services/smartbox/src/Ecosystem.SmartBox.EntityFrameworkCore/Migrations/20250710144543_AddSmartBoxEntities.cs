using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecosystem.SmartBox.Migrations
{
    /// <inheritdoc />
    public partial class AddSmartBoxEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmartBoxCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaxCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Logo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartBoxCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmartBoxRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartBoxRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmartBoxUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Avatar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Salary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartBoxUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmartBoxUsers_SmartBoxCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "SmartBoxCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SmartBoxUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartBoxUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_SmartBoxUserRoles_SmartBoxRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SmartBoxRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmartBoxUserRoles_SmartBoxUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "SmartBoxUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxCompanies_IsActive",
                table: "SmartBoxCompanies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxCompanies_Name",
                table: "SmartBoxCompanies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxCompanies_TaxCode",
                table: "SmartBoxCompanies",
                column: "TaxCode");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxRoles_DisplayName",
                table: "SmartBoxRoles",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxRoles_DisplayOrder",
                table: "SmartBoxRoles",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxRoles_IsActive",
                table: "SmartBoxRoles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxRoles_Name",
                table: "SmartBoxRoles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUserRoles_AssignedDate",
                table: "SmartBoxUserRoles",
                column: "AssignedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUserRoles_RoleId",
                table: "SmartBoxUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUserRoles_UserId",
                table: "SmartBoxUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUsers_AuthUserId",
                table: "SmartBoxUsers",
                column: "AuthUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUsers_CompanyId",
                table: "SmartBoxUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUsers_Email",
                table: "SmartBoxUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUsers_EmployeeCode",
                table: "SmartBoxUsers",
                column: "EmployeeCode");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUsers_IsActive",
                table: "SmartBoxUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SmartBoxUsers_UserName",
                table: "SmartBoxUsers",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmartBoxUserRoles");

            migrationBuilder.DropTable(
                name: "SmartBoxRoles");

            migrationBuilder.DropTable(
                name: "SmartBoxUsers");

            migrationBuilder.DropTable(
                name: "SmartBoxCompanies");
        }
    }
}
