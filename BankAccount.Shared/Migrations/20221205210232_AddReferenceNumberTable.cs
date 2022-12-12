using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankAccount.Shared.Migrations
{
    public partial class AddReferenceNumberTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TOD",
                table: "PotentialMembers",
                newName: "InitializationTime");

            migrationBuilder.CreateTable(
                name: "ReferenceNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dimension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextVal = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceNumbers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferenceNumbers");

            migrationBuilder.RenameColumn(
                name: "InitializationTime",
                table: "PotentialMembers",
                newName: "TOD");
        }
    }
}
