using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeMTG.API.Migrations
{
    /// <inheritdoc />
    public partial class AddQdrantInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QdrantInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScryfallCardObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vector = table.Column<float[]>(type: "real[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QdrantInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QdrantInfo");
        }
    }
}
