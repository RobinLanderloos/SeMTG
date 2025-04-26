using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeMTG.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScryfallCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Object = table.Column<string>(type: "text", nullable: true),
                    OracleId = table.Column<string>(type: "text", nullable: true),
                    MultiverseIds = table.Column<List<int?>>(type: "integer[]", nullable: true),
                    MtgoId = table.Column<int>(type: "integer", nullable: true),
                    ArenaId = table.Column<int>(type: "integer", nullable: true),
                    TcgplayerId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Lang = table.Column<string>(type: "text", nullable: true),
                    ReleasedAt = table.Column<string>(type: "text", nullable: true),
                    Uri = table.Column<string>(type: "text", nullable: true),
                    ScryfallUri = table.Column<string>(type: "text", nullable: true),
                    Layout = table.Column<string>(type: "text", nullable: true),
                    HighresImage = table.Column<bool>(type: "boolean", nullable: true),
                    ImageStatus = table.Column<string>(type: "text", nullable: true),
                    ManaCost = table.Column<string>(type: "text", nullable: true),
                    Cmc = table.Column<double>(type: "double precision", nullable: true),
                    TypeLine = table.Column<string>(type: "text", nullable: true),
                    OracleText = table.Column<string>(type: "text", nullable: true),
                    Colors = table.Column<List<string>>(type: "text[]", nullable: true),
                    ColorIdentity = table.Column<List<string>>(type: "text[]", nullable: true),
                    Keywords = table.Column<List<string>>(type: "text[]", nullable: true),
                    ProducedMana = table.Column<List<string>>(type: "text[]", nullable: true),
                    Games = table.Column<List<string>>(type: "text[]", nullable: true),
                    Reserved = table.Column<bool>(type: "boolean", nullable: true),
                    GameChanger = table.Column<bool>(type: "boolean", nullable: true),
                    Foil = table.Column<bool>(type: "boolean", nullable: true),
                    Nonfoil = table.Column<bool>(type: "boolean", nullable: true),
                    Finishes = table.Column<List<string>>(type: "text[]", nullable: true),
                    Oversized = table.Column<bool>(type: "boolean", nullable: true),
                    Promo = table.Column<bool>(type: "boolean", nullable: true),
                    Reprint = table.Column<bool>(type: "boolean", nullable: true),
                    Variation = table.Column<bool>(type: "boolean", nullable: true),
                    SetId = table.Column<string>(type: "text", nullable: true),
                    Set = table.Column<string>(type: "text", nullable: true),
                    SetName = table.Column<string>(type: "text", nullable: true),
                    SetType = table.Column<string>(type: "text", nullable: true),
                    SetUri = table.Column<string>(type: "text", nullable: true),
                    SetSearchUri = table.Column<string>(type: "text", nullable: true),
                    ScryfallSetUri = table.Column<string>(type: "text", nullable: true),
                    RulingsUri = table.Column<string>(type: "text", nullable: true),
                    PrintsSearchUri = table.Column<string>(type: "text", nullable: true),
                    CollectorNumber = table.Column<string>(type: "text", nullable: true),
                    Digital = table.Column<bool>(type: "boolean", nullable: true),
                    Rarity = table.Column<string>(type: "text", nullable: true),
                    CardBackId = table.Column<string>(type: "text", nullable: true),
                    Artist = table.Column<string>(type: "text", nullable: true),
                    ArtistIds = table.Column<List<string>>(type: "text[]", nullable: true),
                    IllustrationId = table.Column<string>(type: "text", nullable: true),
                    BorderColor = table.Column<string>(type: "text", nullable: true),
                    Frame = table.Column<string>(type: "text", nullable: true),
                    FullArt = table.Column<bool>(type: "boolean", nullable: true),
                    Textless = table.Column<bool>(type: "boolean", nullable: true),
                    Booster = table.Column<bool>(type: "boolean", nullable: true),
                    StorySpotlight = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScryfallCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageUris",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScryfallCardObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Small = table.Column<string>(type: "text", nullable: false),
                    Normal = table.Column<string>(type: "text", nullable: false),
                    Large = table.Column<string>(type: "text", nullable: false),
                    Png = table.Column<string>(type: "text", nullable: false),
                    ArtCrop = table.Column<string>(type: "text", nullable: false),
                    BorderCrop = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageUris_ScryfallCards_ScryfallCardObjectId",
                        column: x => x.ScryfallCardObjectId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Legalities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScryfallCardObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Standard = table.Column<string>(type: "text", nullable: false),
                    Future = table.Column<string>(type: "text", nullable: false),
                    Historic = table.Column<string>(type: "text", nullable: false),
                    Timeless = table.Column<string>(type: "text", nullable: false),
                    Gladiator = table.Column<string>(type: "text", nullable: false),
                    Pioneer = table.Column<string>(type: "text", nullable: false),
                    Explorer = table.Column<string>(type: "text", nullable: false),
                    Modern = table.Column<string>(type: "text", nullable: false),
                    Legacy = table.Column<string>(type: "text", nullable: false),
                    Pauper = table.Column<string>(type: "text", nullable: false),
                    Vintage = table.Column<string>(type: "text", nullable: false),
                    Penny = table.Column<string>(type: "text", nullable: false),
                    Commander = table.Column<string>(type: "text", nullable: false),
                    Oathbreaker = table.Column<string>(type: "text", nullable: false),
                    Standardbrawl = table.Column<string>(type: "text", nullable: false),
                    Brawl = table.Column<string>(type: "text", nullable: false),
                    Alchemy = table.Column<string>(type: "text", nullable: false),
                    Paupercommander = table.Column<string>(type: "text", nullable: false),
                    Duel = table.Column<string>(type: "text", nullable: false),
                    Oldschool = table.Column<string>(type: "text", nullable: false),
                    Premodern = table.Column<string>(type: "text", nullable: false),
                    Predh = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Legalities_ScryfallCards_ScryfallCardObjectId",
                        column: x => x.ScryfallCardObjectId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScryfallCardObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Usd = table.Column<string>(type: "text", nullable: false),
                    UsdFoil = table.Column<string>(type: "text", nullable: false),
                    UsdEtched = table.Column<string>(type: "text", nullable: false),
                    Eur = table.Column<string>(type: "text", nullable: false),
                    EurFoil = table.Column<string>(type: "text", nullable: false),
                    Tix = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_ScryfallCards_ScryfallCardObjectId",
                        column: x => x.ScryfallCardObjectId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseUris",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScryfallCardObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tcgplayer = table.Column<string>(type: "text", nullable: false),
                    Cardmarket = table.Column<string>(type: "text", nullable: false),
                    Cardhoarder = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseUris_ScryfallCards_ScryfallCardObjectId",
                        column: x => x.ScryfallCardObjectId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedUris",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScryfallCardObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gatherer = table.Column<string>(type: "text", nullable: false),
                    TcgplayerInfiniteArticles = table.Column<string>(type: "text", nullable: false),
                    TcgplayerInfiniteDecks = table.Column<string>(type: "text", nullable: false),
                    Edhrec = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedUris_ScryfallCards_ScryfallCardObjectId",
                        column: x => x.ScryfallCardObjectId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardObjectId",
                table: "ImageUris",
                column: "ScryfallCardObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Legalities_ScryfallCardObjectId",
                table: "Legalities",
                column: "ScryfallCardObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardObjectId",
                table: "Prices",
                column: "ScryfallCardObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseUris_ScryfallCardObjectId",
                table: "PurchaseUris",
                column: "ScryfallCardObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedUris_ScryfallCardObjectId",
                table: "RelatedUris",
                column: "ScryfallCardObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScryfallCards_Name",
                table: "ScryfallCards",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ScryfallCards_OracleText",
                table: "ScryfallCards",
                column: "OracleText");

            migrationBuilder.CreateIndex(
                name: "IX_ScryfallCards_TypeLine",
                table: "ScryfallCards",
                column: "TypeLine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageUris");

            migrationBuilder.DropTable(
                name: "Legalities");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "PurchaseUris");

            migrationBuilder.DropTable(
                name: "RelatedUris");

            migrationBuilder.DropTable(
                name: "ScryfallCards");
        }
    }
}
