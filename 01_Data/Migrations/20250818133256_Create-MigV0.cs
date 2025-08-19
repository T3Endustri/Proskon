using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _01_Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateMigV0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T3FormResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<short>(type: "smallint", nullable: false),
                    IsSystemDefined = table.Column<bool>(type: "bit", nullable: false),
                    AllowMultipleSelection = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3FormResource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3IdentityRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    StartPage = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsTeam = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDepartment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3IdentityRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3IdentityUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    StartPage = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3IdentityUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    OperationNo = table.Column<int>(type: "int", nullable: false),
                    IsStation = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3Module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, defaultValue: "", collation: "Turkish_CI_AS"),
                    PageText = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    IconMultiple = table.Column<int>(type: "int", nullable: false),
                    IconSingle = table.Column<int>(type: "int", nullable: false),
                    IsCanPage = table.Column<bool>(type: "bit", nullable: false),
                    IsCanShift = table.Column<bool>(type: "bit", nullable: false),
                    IsCanFilter = table.Column<bool>(type: "bit", nullable: false),
                    IsCanTemplate = table.Column<bool>(type: "bit", nullable: false),
                    IsCanBarcode = table.Column<bool>(type: "bit", nullable: false),
                    IsCanModuleType = table.Column<bool>(type: "bit", nullable: false),
                    IsCanTarget = table.Column<bool>(type: "bit", nullable: false),
                    IsCanSerial = table.Column<bool>(type: "bit", nullable: false),
                    ColorBack = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    ColorFore = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Module", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3ProcessType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    IconMultiple = table.Column<int>(type: "int", nullable: false),
                    IconSingle = table.Column<int>(type: "int", nullable: false),
                    ColorBack = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    ColorFore = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Target = table.Column<long>(type: "bigint", nullable: false),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ProcessType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3ShiftType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ShiftType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3ShiftTypeCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ShiftTypeCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T3FormResourceItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3FormResourceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3FormResourceItem_T3FormResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "T3FormResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3Property",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    FormResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    DisplayText = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    ExtField = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    FieldType = table.Column<short>(type: "smallint", nullable: false),
                    Range = table.Column<bool>(type: "bit", nullable: false),
                    RMax = table.Column<double>(type: "float", nullable: false),
                    RMin = table.Column<double>(type: "float", nullable: false),
                    RStep = table.Column<double>(type: "float", nullable: false),
                    Require = table.Column<bool>(type: "bit", nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    MaxLength = table.Column<int>(type: "int", nullable: false),
                    FileTypes = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    FileMultiple = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3Property_T3FormResource_FormResourceId",
                        column: x => x.FormResourceId,
                        principalTable: "T3FormResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3IdentityRoleHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3IdentityRoleHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3IdentityRoleHierarchy_T3IdentityRole_ChildId",
                        column: x => x.ChildId,
                        principalTable: "T3IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3IdentityRoleHierarchy_T3IdentityRole_ParentId",
                        column: x => x.ParentId,
                        principalTable: "T3IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T3IdentityClaim",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PermissionType = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3IdentityClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3IdentityClaim_T3IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "T3IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3IdentityClaim_T3IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "T3IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3IdentityUserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3IdentityUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_T3IdentityUserRole_T3IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "T3IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3IdentityUserRole_T3IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "T3IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3LocationHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3LocationHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3LocationHierarchy_T3Location_ChildId",
                        column: x => x.ChildId,
                        principalTable: "T3Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3LocationHierarchy_T3Location_ParentId",
                        column: x => x.ParentId,
                        principalTable: "T3Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3Shift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Finish = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Target = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Shift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3Shift_T3Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "T3Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModuleTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    ExternalFilter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3Item_T3Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "T3Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3Item_T3Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "T3Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3Item_T3Module_ModuleTypeId",
                        column: x => x.ModuleTypeId,
                        principalTable: "T3Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T3ModuleHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ModuleHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3ModuleHierarchy_T3Module_ChildId",
                        column: x => x.ChildId,
                        principalTable: "T3Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3ModuleHierarchy_T3Module_ParentId",
                        column: x => x.ParentId,
                        principalTable: "T3Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3ProcessTypeModule",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ProcessTypeModule", x => new { x.ModuleId, x.TypeId });
                    table.ForeignKey(
                        name: "FK_T3ProcessTypeModule_T3Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "T3Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3ProcessTypeModule_T3ProcessType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "T3ProcessType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3Protocol",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ProcessTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<long>(type: "bigint", nullable: false),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Protocol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3Protocol_T3ProcessType_ProcessTypeId",
                        column: x => x.ProcessTypeId,
                        principalTable: "T3ProcessType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3ShiftTypeDay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ShiftTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    StartTime = table.Column<long>(type: "bigint", nullable: false),
                    EndTime = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ShiftTypeDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftTypeDay_ShiftType",
                        column: x => x.ShiftTypeId,
                        principalTable: "T3ShiftType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3ShiftTypeLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ShiftTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ShiftTypeLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3ShiftTypeLocation_T3Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "T3Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3ShiftTypeLocation_T3ShiftType_ShiftTypeId",
                        column: x => x.ShiftTypeId,
                        principalTable: "T3ShiftType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3ShiftBreak",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ShiftBreak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3ShiftBreak_T3Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "T3Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3ItemHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ItemHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3ItemHierarchy_T3Item_ChildId",
                        column: x => x.ChildId,
                        principalTable: "T3Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3ItemHierarchy_T3Item_ParentId",
                        column: x => x.ParentId,
                        principalTable: "T3Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T3LocationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3LocationItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3LocationItem_T3Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "T3Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3LocationItem_T3Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "T3Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T3ProcessTypeItem",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ProcessTypeItem", x => new { x.ItemId, x.TypeId });
                    table.ForeignKey(
                        name: "FK_T3ProcessTypeItem_T3Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "T3Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3ProcessTypeItem_T3ProcessType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "T3ProcessType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3Template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ColumnCount = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Template", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3Template_T3Item_Id",
                        column: x => x.Id,
                        principalTable: "T3Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3Template_T3Module_Id",
                        column: x => x.Id,
                        principalTable: "T3Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T3ProtocolItem",
                columns: table => new
                {
                    ProtocolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Target = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ProtocolItem", x => new { x.ProtocolId, x.ItemId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_T3ProtocolItem_T3Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "T3Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3ProtocolItem_T3Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "T3Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3ProtocolItem_T3Protocol_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "T3Protocol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3ShiftTypeBreak",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ShiftTypeDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    StartTime = table.Column<long>(type: "bigint", nullable: false),
                    EndTime = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3ShiftTypeBreak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3ShiftTypeBreak_T3ShiftTypeDay_ShiftTypeDayId",
                        column: x => x.ShiftTypeDayId,
                        principalTable: "T3ShiftTypeDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3Form",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsApprove = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3Form", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3Form_T3IdentityUser_ApprovedUserId",
                        column: x => x.ApprovedUserId,
                        principalTable: "T3IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3Form_T3IdentityUser_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "T3IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3Form_T3Item_Id",
                        column: x => x.Id,
                        principalTable: "T3Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3Form_T3Module_Id",
                        column: x => x.Id,
                        principalTable: "T3Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T3Form_T3Template_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "T3Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T3PropertyTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    PropertyFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Column = table.Column<int>(type: "int", nullable: false),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3PropertyTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3PropertyTemplate_T3Property_PropertyFieldId",
                        column: x => x.PropertyFieldId,
                        principalTable: "T3Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3PropertyTemplate_T3Template_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "T3Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3TemplateApprover",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3TemplateApprover", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3TemplateApprover_T3IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "T3IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3TemplateApprover_T3IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "T3IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3TemplateApprover_T3Template_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "T3Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3TemplatePanel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3TemplatePanel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3TemplatePanel_T3Template_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "T3Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3FormField",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3FormField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3FormField_T3Form_FormId",
                        column: x => x.FormId,
                        principalTable: "T3Form",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3FormField_T3Property_PropertyFieldId",
                        column: x => x.PropertyFieldId,
                        principalTable: "T3Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3PropertyPanel",
                columns: table => new
                {
                    PropertyFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PanelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Column = table.Column<int>(type: "int", nullable: false),
                    SortBy = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3PropertyPanel", x => new { x.PropertyFieldId, x.PanelId });
                    table.ForeignKey(
                        name: "FK_T3PropertyPanel_T3Property_PropertyFieldId",
                        column: x => x.PropertyFieldId,
                        principalTable: "T3Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3PropertyPanel_T3TemplatePanel_PanelId",
                        column: x => x.PanelId,
                        principalTable: "T3TemplatePanel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T3FormFieldValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    FormFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T3FormFieldValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T3FormFieldValue_T3FormField_FormFieldId",
                        column: x => x.FormFieldId,
                        principalTable: "T3FormField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T3FormFieldValue_T3IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "T3IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T3Form_ApprovedUserId",
                table: "T3Form",
                column: "ApprovedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Form_CreateUserId",
                table: "T3Form",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Form_TemplateId",
                table: "T3Form",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_T3FormField_FormId",
                table: "T3FormField",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_T3FormField_PropertyFieldId",
                table: "T3FormField",
                column: "PropertyFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_T3FormFieldValue_FormFieldId",
                table: "T3FormFieldValue",
                column: "FormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_T3FormFieldValue_UserId",
                table: "T3FormFieldValue",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_T3FormResourceItem_ResourceId",
                table: "T3FormResourceItem",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_T3IdentityClaim_RoleId",
                table: "T3IdentityClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_T3IdentityClaim_UserId",
                table: "T3IdentityClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_T3IdentityRoleHierarchy_ChildId",
                table: "T3IdentityRoleHierarchy",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_T3IdentityRoleHierarchy_ParentId_ChildId",
                table: "T3IdentityRoleHierarchy",
                columns: new[] { "ParentId", "ChildId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T3IdentityUserRole_RoleId",
                table: "T3IdentityUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_T3IdentityUserRole_UserId",
                table: "T3IdentityUserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Item_LocationId",
                table: "T3Item",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Item_ModuleId",
                table: "T3Item",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Item_ModuleTypeId",
                table: "T3Item",
                column: "ModuleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ItemHierarchy_ChildId",
                table: "T3ItemHierarchy",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ItemHierarchy_ParentId_ChildId",
                table: "T3ItemHierarchy",
                columns: new[] { "ParentId", "ChildId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T3LocationHierarchy_ChildId",
                table: "T3LocationHierarchy",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_T3LocationHierarchy_ParentId",
                table: "T3LocationHierarchy",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_T3LocationItem_ItemId",
                table: "T3LocationItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_T3LocationItem_LocationId",
                table: "T3LocationItem",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "UX_T3Module_Name",
                table: "T3Module",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T3ModuleHierarchy_ChildId",
                table: "T3ModuleHierarchy",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ModuleHierarchy_ParentId",
                table: "T3ModuleHierarchy",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ProcessTypeItem_ItemId",
                table: "T3ProcessTypeItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ProcessTypeItem_TypeId",
                table: "T3ProcessTypeItem",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ProcessTypeModule_ModuleId",
                table: "T3ProcessTypeModule",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ProcessTypeModule_TypeId",
                table: "T3ProcessTypeModule",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Property_FormResourceId",
                table: "T3Property",
                column: "FormResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_T3PropertyPanel_PanelId",
                table: "T3PropertyPanel",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_T3PropertyTemplate_PropertyFieldId",
                table: "T3PropertyTemplate",
                column: "PropertyFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_T3PropertyTemplate_TemplateId",
                table: "T3PropertyTemplate",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Protocol_ProcessTypeId",
                table: "T3Protocol",
                column: "ProcessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ProtocolItem_ItemId",
                table: "T3ProtocolItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ProtocolItem_LocationId",
                table: "T3ProtocolItem",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_T3Shift_LocationId",
                table: "T3Shift",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ShiftBreak_ShiftId",
                table: "T3ShiftBreak",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ShiftTypeBreak_ShiftTypeDayId",
                table: "T3ShiftTypeBreak",
                column: "ShiftTypeDayId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ShiftTypeDay_ShiftTypeId",
                table: "T3ShiftTypeDay",
                column: "ShiftTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ShiftTypeLocation_LocationId",
                table: "T3ShiftTypeLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_T3ShiftTypeLocation_ShiftTypeId",
                table: "T3ShiftTypeLocation",
                column: "ShiftTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_T3TemplateApprover_RoleId",
                table: "T3TemplateApprover",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_T3TemplateApprover_TemplateId",
                table: "T3TemplateApprover",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_T3TemplateApprover_UserId",
                table: "T3TemplateApprover",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_T3TemplatePanel_TemplateId",
                table: "T3TemplatePanel",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T3FormFieldValue");

            migrationBuilder.DropTable(
                name: "T3FormResourceItem");

            migrationBuilder.DropTable(
                name: "T3IdentityClaim");

            migrationBuilder.DropTable(
                name: "T3IdentityRoleHierarchy");

            migrationBuilder.DropTable(
                name: "T3IdentityUserRole");

            migrationBuilder.DropTable(
                name: "T3ItemHierarchy");

            migrationBuilder.DropTable(
                name: "T3LocationHierarchy");

            migrationBuilder.DropTable(
                name: "T3LocationItem");

            migrationBuilder.DropTable(
                name: "T3ModuleHierarchy");

            migrationBuilder.DropTable(
                name: "T3ProcessTypeItem");

            migrationBuilder.DropTable(
                name: "T3ProcessTypeModule");

            migrationBuilder.DropTable(
                name: "T3PropertyPanel");

            migrationBuilder.DropTable(
                name: "T3PropertyTemplate");

            migrationBuilder.DropTable(
                name: "T3ProtocolItem");

            migrationBuilder.DropTable(
                name: "T3ShiftBreak");

            migrationBuilder.DropTable(
                name: "T3ShiftTypeBreak");

            migrationBuilder.DropTable(
                name: "T3ShiftTypeCategory");

            migrationBuilder.DropTable(
                name: "T3ShiftTypeLocation");

            migrationBuilder.DropTable(
                name: "T3TemplateApprover");

            migrationBuilder.DropTable(
                name: "T3FormField");

            migrationBuilder.DropTable(
                name: "T3TemplatePanel");

            migrationBuilder.DropTable(
                name: "T3Protocol");

            migrationBuilder.DropTable(
                name: "T3Shift");

            migrationBuilder.DropTable(
                name: "T3ShiftTypeDay");

            migrationBuilder.DropTable(
                name: "T3IdentityRole");

            migrationBuilder.DropTable(
                name: "T3Form");

            migrationBuilder.DropTable(
                name: "T3Property");

            migrationBuilder.DropTable(
                name: "T3ProcessType");

            migrationBuilder.DropTable(
                name: "T3ShiftType");

            migrationBuilder.DropTable(
                name: "T3IdentityUser");

            migrationBuilder.DropTable(
                name: "T3Template");

            migrationBuilder.DropTable(
                name: "T3FormResource");

            migrationBuilder.DropTable(
                name: "T3Item");

            migrationBuilder.DropTable(
                name: "T3Location");

            migrationBuilder.DropTable(
                name: "T3Module");
        }
    }
}
