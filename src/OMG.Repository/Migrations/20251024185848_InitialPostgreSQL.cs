using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OMG.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aromas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aromas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Embalagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embalagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventChangeStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<int>(type: "integer", nullable: false),
                    OldStatus = table.Column<int>(type: "integer", maxLength: 250, nullable: false),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    UsuarioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UsuarioEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChangeStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Formatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: false, defaultValue: 0m),
                    Desconto = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: false, defaultValue: 0m),
                    Entrada = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: false, defaultValue: 0m),
                    IsPermuta = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataEntrega = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    FormatoId = table.Column<int>(type: "integer", nullable: false),
                    CorId = table.Column<int>(type: "integer", nullable: false),
                    AromaId = table.Column<int>(type: "integer", nullable: false),
                    EmbalagemId = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Aromas_AromaId",
                        column: x => x.AromaId,
                        principalTable: "Aromas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Cores_CorId",
                        column: x => x.CorId,
                        principalTable: "Cores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Embalagens_EmbalagemId",
                        column: x => x.EmbalagemId,
                        principalTable: "Embalagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Formatos_FormatoId",
                        column: x => x.FormatoId,
                        principalTable: "Formatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aromas_IsDeleted",
                table: "Aromas",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Aromas_Nome",
                table: "Aromas",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IsDeleted",
                table: "Clientes",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Nome",
                table: "Clientes",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Cores_IsDeleted",
                table: "Cores",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Cores_Nome",
                table: "Cores",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Embalagens_Descricao",
                table: "Embalagens",
                column: "Descricao");

            migrationBuilder.CreateIndex(
                name: "IX_Embalagens_IsDeleted",
                table: "Embalagens",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Formatos_Descricao",
                table: "Formatos",
                column: "Descricao");

            migrationBuilder.CreateIndex(
                name: "IX_Formatos_IsDeleted",
                table: "Formatos",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_AromaId",
                table: "PedidoItens",
                column: "AromaId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_CorId",
                table: "PedidoItens",
                column: "CorId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_EmbalagemId",
                table: "PedidoItens",
                column: "EmbalagemId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_FormatoId",
                table: "PedidoItens",
                column: "FormatoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_IsDeleted",
                table: "PedidoItens",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_PedidoId",
                table: "PedidoItens",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_ProdutoId",
                table: "PedidoItens",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteId",
                table: "Pedidos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IsDeleted",
                table: "Pedidos",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Descricao",
                table: "Produtos",
                column: "Descricao");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IsDeleted",
                table: "Produtos",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventChangeStatus");

            migrationBuilder.DropTable(
                name: "PedidoItens");

            migrationBuilder.DropTable(
                name: "Aromas");

            migrationBuilder.DropTable(
                name: "Cores");

            migrationBuilder.DropTable(
                name: "Embalagens");

            migrationBuilder.DropTable(
                name: "Formatos");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
