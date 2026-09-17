using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testWPF.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroOrdre = table.Column<int>(type: "INTEGER", nullable: true),
                    DateEnregistrement = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NomPrenom = table.Column<string>(type: "TEXT", nullable: false),
                    Sexe = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: true),
                    AdresseComplete = table.Column<string>(type: "TEXT", nullable: false),
                    DateDebutTraitement = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LieuDebutTraitement = table.Column<string>(type: "TEXT", nullable: false),
                    RegimeTraitement = table.Column<string>(type: "TEXT", nullable: false),
                    TpOuTep = table.Column<string>(type: "TEXT", nullable: false),
                    TypeN = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeR = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeE = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeREP = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeT = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeA = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreTraitFrottis = table.Column<string>(type: "TEXT", nullable: false),
                    PreTraitCult = table.Column<string>(type: "TEXT", nullable: false),
                    Fin2eMoisFrottis = table.Column<string>(type: "TEXT", nullable: false),
                    Fin2eMoisCult = table.Column<string>(type: "TEXT", nullable: false),
                    Fin4eMoisFrottis = table.Column<string>(type: "TEXT", nullable: false),
                    Fin4eMoisCult = table.Column<string>(type: "TEXT", nullable: false),
                    Mois6ou8Frottis = table.Column<string>(type: "TEXT", nullable: false),
                    Mois6ou8Cult = table.Column<string>(type: "TEXT", nullable: false),
                    AuDelaFrottis = table.Column<string>(type: "TEXT", nullable: false),
                    GuerisTraitTermine = table.Column<string>(type: "TEXT", nullable: false),
                    PasExamenBacteriol = table.Column<string>(type: "TEXT", nullable: false),
                    Decede = table.Column<string>(type: "TEXT", nullable: false),
                    Echec = table.Column<string>(type: "TEXT", nullable: false),
                    PerduDeVue = table.Column<string>(type: "TEXT", nullable: false),
                    Transfere = table.Column<string>(type: "TEXT", nullable: false),
                    Observation = table.Column<string>(type: "TEXT", nullable: false),
                    LungDrawing = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cases");
        }
    }
}
