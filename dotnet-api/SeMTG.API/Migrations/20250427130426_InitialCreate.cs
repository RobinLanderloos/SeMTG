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
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Vector = table.Column<float[]>(type: "real[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardEditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Object = table.Column<string>(type: "text", nullable: true),
                    OracleId = table.Column<string>(type: "text", nullable: true),
                    MultiverseIds = table.Column<List<int?>>(type: "integer[]", nullable: true),
                    MtgoId = table.Column<int>(type: "integer", nullable: true),
                    ArenaId = table.Column<int>(type: "integer", nullable: true),
                    TcgplayerId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Lang = table.Column<string>(type: "text", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_CardEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardEditions_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageUris",
                columns: table => new
                {
                    CardEditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Small = table.Column<string>(type: "text", nullable: false),
                    Normal = table.Column<string>(type: "text", nullable: false),
                    Large = table.Column<string>(type: "text", nullable: false),
                    Png = table.Column<string>(type: "text", nullable: false),
                    ArtCrop = table.Column<string>(type: "text", nullable: false),
                    BorderCrop = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageUris", x => x.CardEditionId);
                    table.ForeignKey(
                        name: "FK_ImageUris_CardEditions_CardEditionId",
                        column: x => x.CardEditionId,
                        principalTable: "CardEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Legalities",
                columns: table => new
                {
                    CardEditionId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Legalities", x => x.CardEditionId);
                    table.ForeignKey(
                        name: "FK_Legalities_CardEditions_CardEditionId",
                        column: x => x.CardEditionId,
                        principalTable: "CardEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    CardEditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Usd = table.Column<string>(type: "text", nullable: true),
                    UsdFoil = table.Column<string>(type: "text", nullable: true),
                    UsdEtched = table.Column<string>(type: "text", nullable: true),
                    Eur = table.Column<string>(type: "text", nullable: true),
                    EurFoil = table.Column<string>(type: "text", nullable: true),
                    Tix = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.CardEditionId);
                    table.ForeignKey(
                        name: "FK_Prices_CardEditions_CardEditionId",
                        column: x => x.CardEditionId,
                        principalTable: "CardEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseUris",
                columns: table => new
                {
                    CardEditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tcgplayer = table.Column<string>(type: "text", nullable: false),
                    Cardmarket = table.Column<string>(type: "text", nullable: false),
                    Cardhoarder = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseUris", x => x.CardEditionId);
                    table.ForeignKey(
                        name: "FK_PurchaseUris_CardEditions_CardEditionId",
                        column: x => x.CardEditionId,
                        principalTable: "CardEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedUris",
                columns: table => new
                {
                    CardEditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gatherer = table.Column<string>(type: "text", nullable: true),
                    TcgplayerInfiniteArticles = table.Column<string>(type: "text", nullable: true),
                    TcgplayerInfiniteDecks = table.Column<string>(type: "text", nullable: true),
                    Edhrec = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedUris", x => x.CardEditionId);
                    table.ForeignKey(
                        name: "FK_RelatedUris_CardEditions_CardEditionId",
                        column: x => x.CardEditionId,
                        principalTable: "CardEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardEditions_CardId",
                table: "CardEditions",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardEditions_Name",
                table: "CardEditions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CardEditions_OracleText",
                table: "CardEditions",
                column: "OracleText");

            migrationBuilder.CreateIndex(
                name: "IX_CardEditions_TypeLine",
                table: "CardEditions",
                column: "TypeLine");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Name",
                table: "Cards",
                column: "Name");
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
                name: "CardEditions");

            migrationBuilder.DropTable(
                name: "Cards");
        }
    }
}
