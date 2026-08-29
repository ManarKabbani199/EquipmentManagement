using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_IdentityUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_IdentityUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_BorrowingRecords_EmployeeId",
                table: "BorrowingRecords");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingRecords_BorrowDate",
                table: "BorrowingRecords",
                column: "BorrowDate");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingRecords_EmployeeId_BorrowDate",
                table: "BorrowingRecords",
                columns: new[] { "EmployeeId", "BorrowDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingRecords_IsReturned_ExpectedReturnDate",
                table: "BorrowingRecords",
                columns: new[] { "IsReturned", "ExpectedReturnDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BorrowingRecords_BorrowDate",
                table: "BorrowingRecords");

            migrationBuilder.DropIndex(
                name: "IX_BorrowingRecords_EmployeeId_BorrowDate",
                table: "BorrowingRecords");

            migrationBuilder.DropIndex(
                name: "IX_BorrowingRecords_IsReturned_ExpectedReturnDate",
                table: "BorrowingRecords");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IdentityUserId",
                table: "Employees",
                column: "IdentityUserId",
                unique: true,
                filter: "[IdentityUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingRecords_EmployeeId",
                table: "BorrowingRecords",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_IdentityUserId",
                table: "Employees",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
