using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralLabSolutions.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditAgainInfra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 150, nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllDay = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "varchar(100)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaProduto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(200)", nullable: false, comment: "Família do produto")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaProduto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Instituicao = table.Column<string>(type: "varchar(200)", nullable: false),
                    Documento = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoDeConta = table.Column<string>(type: "varchar(40)", nullable: false),
                    EstaPaga = table.Column<bool>(type: "bit", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacao = table.Column<string>(type: "varchar(500)", nullable: true),
                    Inativa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KanbanTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "varchar(80)", nullable: false),
                    Description = table.Column<string>(type: "varchar(150)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Column = table.Column<string>(type: "varchar(40)", nullable: false),
                    Priority = table.Column<string>(type: "varchar(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Company = table.Column<string>(type: "varchar(100)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "varchar(100)", maxLength: 50, nullable: true),
                    Position = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Source = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Responsible = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    LastContact = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Temperature = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioInclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensagensChat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EhRespostaIA = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagensChat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participante",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participante", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pessoa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusDoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusDoItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(25)", nullable: false),
                    Percentual = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ValorDesconto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    TipoDescontoVoucher = table.Column<string>(type: "varchar(40)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUtilizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Utilizado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioInclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CrmTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<DateTime>(type: "date", nullable: true),
                    Priority = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Assignee = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    LeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioInclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmTasks_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioInclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadNotes_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KanbanTaskParticipante",
                columns: table => new
                {
                    ParticipantesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TasksId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanTaskParticipante", x => new { x.ParticipantesId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_KanbanTaskParticipante_KanbanTask_TasksId",
                        column: x => x.TasksId,
                        principalTable: "KanbanTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KanbanTaskParticipante_Participante_ParticipantesId",
                        column: x => x.ParticipantesId,
                        principalTable: "Participante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Documento = table.Column<string>(type: "varchar(14)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    InscricaoEstadual = table.Column<string>(type: "varchar(20)", nullable: true),
                    Observacao = table.Column<string>(type: "varchar(4000)", nullable: true),
                    TelefonePrincipal = table.Column<string>(type: "varchar(15)", nullable: false),
                    ContatoRepresentante = table.Column<string>(type: "varchar(50)", nullable: true),
                    TipoDePessoa = table.Column<string>(type: "varchar(40)", nullable: false),
                    StatusDoCliente = table.Column<string>(type: "varchar(40)", nullable: false),
                    TipoDeCliente = table.Column<string>(type: "varchar(40)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(120)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(120)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cliente_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contato",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    Telefone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    EmailAlternativo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    TelefoneAlternativo = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    Observacao = table.Column<string>(type: "varchar(500)", nullable: false),
                    TipoDeContato = table.Column<string>(type: "varchar(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contato_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadosBancarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Banco = table.Column<string>(type: "varchar(100)", nullable: false),
                    Agencia = table.Column<string>(type: "varchar(20)", nullable: false),
                    Conta = table.Column<string>(type: "varchar(20)", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDeContaBancaria = table.Column<string>(type: "varchar(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosBancarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DadosBancarios_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaisCodigoIso = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    LinhaEndereco1 = table.Column<string>(type: "varchar(100)", maxLength: 200, nullable: false),
                    LinhaEndereco2 = table.Column<string>(type: "varchar(100)", maxLength: 200, nullable: false),
                    Cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    EstadoOuProvincia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CodigoPostal = table.Column<string>(type: "varchar(100)", maxLength: 20, nullable: false),
                    InformacoesAdicionais = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: false),
                    TipoDeEndereco = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Endereco_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Documento = table.Column<string>(type: "varchar(14)", nullable: false),
                    TipoDePessoa = table.Column<string>(type: "varchar(40)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    StatusDoFornecedor = table.Column<string>(type: "varchar(40)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(120)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(120)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fornecedor_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Telefone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DDD = table.Column<string>(type: "varchar(3)", nullable: false),
                    Numero = table.Column<string>(type: "varchar(15)", nullable: false),
                    TipoDeTelefone = table.Column<string>(type: "varchar(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telefone_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vendedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Documento = table.Column<string>(type: "varchar(14)", nullable: false),
                    TipoDePessoa = table.Column<string>(type: "varchar(40)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    StatusDoVendedor = table.Column<string>(type: "varchar(40)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(120)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(120)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendedor_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusDoItemIncompativel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusDoItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusDoItemIncompativelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusDoItemIncompativel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusDoItemIncompativel_StatusDoItem_StatusDoItemId",
                        column: x => x.StatusDoItemId,
                        principalTable: "StatusDoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatusDoItemIncompativel_StatusDoItem_StatusDoItemIncompativelId",
                        column: x => x.StatusDoItemIncompativelId,
                        principalTable: "StatusDoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CrmTaskAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    UploadedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CrmTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioInclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmTaskAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmTaskAttachments_CrmTasks_CrmTaskId",
                        column: x => x.CrmTaskId,
                        principalTable: "CrmTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CrmTaskComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CrmTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioInclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataUltimaModificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioUltimaModificacao = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmTaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmTaskComments_CrmTasks_CrmTaskId",
                        column: x => x.CrmTaskId,
                        principalTable: "CrmTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(20)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: false, comment: "Descrição do produto"),
                    NCM = table.Column<string>(type: "varchar(15)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StatusDoProduto = table.Column<string>(type: "varchar(40)", nullable: false, defaultValue: "Dropshipping"),
                    DataDeValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Imagem = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produto_CategoriaProduto_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriaProduto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produto_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime", nullable: false),
                    StatusDoPedido = table.Column<string>(type: "varchar(40)", nullable: false, defaultValue: "Orcamento"),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Vendedor_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Vendedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistoricoPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEvento = table.Column<string>(type: "varchar(100)", nullable: false),
                    StatusAnterior = table.Column<string>(type: "varchar(50)", nullable: true),
                    StatusNovo = table.Column<string>(type: "varchar(50)", nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: true),
                    DadosExtras = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoPedido_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NomeDoProduto = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: false, comment: "Descrição do produto")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemPedido_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemPedido_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstadoDoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemPedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusDoItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DadosExtras = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoDoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadoDoItem_ItemPedido_ItemPedidoId",
                        column: x => x.ItemPedidoId,
                        principalTable: "ItemPedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstadoDoItem_StatusDoItem_StatusDoItemId",
                        column: x => x.StatusDoItemId,
                        principalTable: "StatusDoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemPedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEvento = table.Column<string>(type: "varchar(100)", nullable: false),
                    StatusAnterior = table.Column<string>(type: "varchar(50)", nullable: true),
                    StatusNovo = table.Column<string>(type: "varchar(50)", nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: true),
                    DadosExtras = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoItem_ItemPedido_ItemPedidoId",
                        column: x => x.ItemPedidoId,
                        principalTable: "ItemPedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_LeadId",
                table: "Activities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Descricao",
                table: "CategoriaProduto",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Documento",
                table: "Cliente",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Email",
                table: "Cliente",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Nome",
                table: "Cliente",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_PessoaId",
                table: "Cliente",
                column: "PessoaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conta_DataVencimento",
                table: "Contas",
                column: "DataVencimento");

            migrationBuilder.CreateIndex(
                name: "IX_Conta_Documento",
                table: "Contas",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contato_Email",
                table: "Contato",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Contato_Nome",
                table: "Contato",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Contato_PessoaId",
                table: "Contato",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmTaskAttachments_CrmTaskId",
                table: "CrmTaskAttachments",
                column: "CrmTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmTaskComments_CrmTaskId",
                table: "CrmTaskComments",
                column: "CrmTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmTasks_LeadId",
                table: "CrmTasks",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_DadosBancarios_Agencia",
                table: "DadosBancarios",
                column: "Agencia");

            migrationBuilder.CreateIndex(
                name: "IX_DadosBancarios_Banco",
                table: "DadosBancarios",
                column: "Banco");

            migrationBuilder.CreateIndex(
                name: "IX_DadosBancarios_Conta",
                table: "DadosBancarios",
                column: "Conta");

            migrationBuilder.CreateIndex(
                name: "IX_DadosBancarios_PessoaId",
                table: "DadosBancarios",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_DadosBancarios_TipoDeContaBancaria",
                table: "DadosBancarios",
                column: "TipoDeContaBancaria");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_CodigoPostal",
                table: "Endereco",
                column: "CodigoPostal");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_PaisCodigoIso",
                table: "Endereco",
                column: "PaisCodigoIso");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_PessoaId",
                table: "Endereco",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoDoItem_ItemPedidoId",
                table: "EstadoDoItem",
                column: "ItemPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoDoItem_StatusDoItemId",
                table: "EstadoDoItem",
                column: "StatusDoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedor_Documento",
                table: "Fornecedor",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedor_Email",
                table: "Fornecedor",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedor_Nome",
                table: "Fornecedor",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedor_PessoaId",
                table: "Fornecedor",
                column: "PessoaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoItem_ItemPedidoId",
                table: "HistoricoItem",
                column: "ItemPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPedido_PedidoId",
                table: "HistoricoPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_NomeDoProduto",
                table: "ItemPedido",
                column: "NomeDoProduto");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_PedidoId",
                table: "ItemPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_ProdutoId",
                table: "ItemPedido",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanTask_Description",
                table: "KanbanTask",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanTask_Title",
                table: "KanbanTask",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanTaskParticipante_TasksId",
                table: "KanbanTaskParticipante",
                column: "TasksId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadNotes_LeadId",
                table: "LeadNotes",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Email",
                table: "Leads",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_LeadId",
                table: "Logs",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Participante_Email",
                table: "Participante",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participante_Name",
                table: "Participante",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_ClienteId",
                table: "Pedido",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_VendedorId",
                table: "Pedido",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_VoucherId",
                table: "Pedido",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaId",
                table: "Produto",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Codigo",
                table: "Produto",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Descricao",
                table: "Produto",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_FornecedorId",
                table: "Produto",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Ncm",
                table: "Produto",
                column: "NCM",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusDoItemIncompativel_StatusDoItemId",
                table: "StatusDoItemIncompativel",
                column: "StatusDoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusDoItemIncompativel_StatusDoItemIncompativelId",
                table: "StatusDoItemIncompativel",
                column: "StatusDoItemIncompativelId");

            migrationBuilder.CreateIndex(
                name: "IX_Telefone_DDD",
                table: "Telefone",
                column: "DDD");

            migrationBuilder.CreateIndex(
                name: "IX_Telefone_Numero",
                table: "Telefone",
                column: "Numero");

            migrationBuilder.CreateIndex(
                name: "IX_Telefone_PessoaId",
                table: "Telefone",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedor_Documento",
                table: "Vendedor",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendedor_Email",
                table: "Vendedor",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendedor_Nome",
                table: "Vendedor",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedor_PessoaId",
                table: "Vendedor",
                column: "PessoaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Codigo",
                table: "Voucher",
                column: "Codigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "Contas");

            migrationBuilder.DropTable(
                name: "Contato");

            migrationBuilder.DropTable(
                name: "CrmTaskAttachments");

            migrationBuilder.DropTable(
                name: "CrmTaskComments");

            migrationBuilder.DropTable(
                name: "DadosBancarios");

            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.DropTable(
                name: "EstadoDoItem");

            migrationBuilder.DropTable(
                name: "HistoricoItem");

            migrationBuilder.DropTable(
                name: "HistoricoPedido");

            migrationBuilder.DropTable(
                name: "KanbanTaskParticipante");

            migrationBuilder.DropTable(
                name: "LeadNotes");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MensagensChat");

            migrationBuilder.DropTable(
                name: "StatusDoItemIncompativel");

            migrationBuilder.DropTable(
                name: "Telefone");

            migrationBuilder.DropTable(
                name: "CrmTasks");

            migrationBuilder.DropTable(
                name: "ItemPedido");

            migrationBuilder.DropTable(
                name: "KanbanTask");

            migrationBuilder.DropTable(
                name: "Participante");

            migrationBuilder.DropTable(
                name: "StatusDoItem");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Vendedor");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "CategoriaProduto");

            migrationBuilder.DropTable(
                name: "Fornecedor");

            migrationBuilder.DropTable(
                name: "Pessoa");
        }
    }
}
