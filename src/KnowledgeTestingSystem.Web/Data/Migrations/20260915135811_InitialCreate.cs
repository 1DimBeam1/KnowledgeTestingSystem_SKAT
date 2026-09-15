using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KnowledgeTestingSystem.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "algorithms",
                columns: table => new
                {
                    algo_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    src_path = table.Column<string>(type: "text", nullable: false),
                    pic_path = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    algo_name = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_algorithms", x => x.algo_id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    difficult = table.Column<float>(type: "real", nullable: false),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_finish = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    session_type = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.session_id);
                });

            migrationBuilder.CreateTable(
                name: "testinputdata",
                columns: table => new
                {
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    var_name = table.Column<string>(type: "text", nullable: false),
                    var_value = table.Column<string>(type: "text", nullable: false),
                    var_type = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testinputdata", x => new { x.test_id, x.var_name });
                });

            migrationBuilder.CreateTable(
                name: "teststepresponses",
                columns: table => new
                {
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    algo_id = table.Column<int>(type: "integer", nullable: false),
                    algo_step = table.Column<int>(type: "integer", nullable: false),
                    correct_count = table.Column<int>(type: "integer", nullable: false),
                    incorrect_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teststepresponses", x => new { x.test_id, x.algo_step, x.algo_id });
                });

            migrationBuilder.CreateTable(
                name: "usertestabilities",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    ability = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usertestabilities", x => new { x.user_id, x.test_id });
                });

            migrationBuilder.CreateTable(
                name: "variables_solutions_by_programs",
                columns: table => new
                {
                    program_step = table.Column<int>(type: "integer", nullable: false),
                    program_line_number = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    var_name = table.Column<string>(type: "text", nullable: false),
                    var_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variables_solutions_by_programs", x => new { x.program_step, x.program_line_number, x.order_number, x.test_id, x.var_name });
                });

            migrationBuilder.CreateTable(
                name: "variables_solutions_by_users",
                columns: table => new
                {
                    user_step = table.Column<int>(type: "integer", nullable: false),
                    user_line_number = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    var_name = table.Column<string>(type: "text", nullable: false),
                    var_value = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variables_solutions_by_users", x => new { x.user_step, x.user_line_number, x.order_number, x.test_id, x.var_name });
                });

            migrationBuilder.CreateTable(
                name: "algorithmsteps",
                columns: table => new
                {
                    algo_id = table.Column<int>(type: "integer", nullable: false),
                    algo_step = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    difficult = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_algorithmsteps", x => new { x.algo_step, x.algo_id });
                    table.UniqueConstraint("AK_algorithmsteps_algo_id_algo_step", x => new { x.algo_id, x.algo_step });
                    table.ForeignKey(
                        name: "FK_algorithmsteps_algorithms_algo_id",
                        column: x => x.algo_id,
                        principalTable: "algorithms",
                        principalColumn: "algo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    test_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    algo_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    test_name = table.Column<string>(type: "text", nullable: false),
                    difficult = table.Column<float>(type: "real", nullable: false),
                    solved_count = table.Column<int>(type: "integer", nullable: false),
                    unsolved_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests", x => x.test_id);
                    table.ForeignKey(
                        name: "FK_tests_algorithms_algo_id",
                        column: x => x.algo_id,
                        principalTable: "algorithms",
                        principalColumn: "algo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    icon_path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "groupsessions",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groupsessions", x => new { x.session_id, x.group_id });
                    table.ForeignKey(
                        name: "FK_groupsessions_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groupsessions_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessiontests",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessiontests", x => new { x.session_id, x.test_id });
                    table.ForeignKey(
                        name: "FK_sessiontests_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solutionsbyprograms",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    program_step = table.Column<int>(type: "integer", nullable: false),
                    program_line_number = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    step_difficult = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solutionsbyprograms", x => new { x.session_id, x.test_id, x.program_step, x.program_line_number, x.order_number });
                    table.ForeignKey(
                        name: "FK_solutionsbyprograms_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trackedvariables",
                columns: table => new
                {
                    sequence = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    var_type = table.Column<string>(type: "text", nullable: false),
                    var_name = table.Column<string>(type: "text", nullable: false),
                    algo_step = table.Column<int>(type: "integer", nullable: false),
                    algo_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trackedvariables", x => x.sequence);
                    table.ForeignKey(
                        name: "FK_trackedvariables_algorithmsteps_algo_id_algo_step",
                        columns: x => new { x.algo_id, x.algo_step },
                        principalTable: "algorithmsteps",
                        principalColumns: new[] { "algo_id", "algo_step" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grades",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    mark = table.Column<float>(type: "real", nullable: false),
                    datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grades", x => new { x.user_id, x.session_id });
                    table.ForeignKey(
                        name: "FK_grades_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grades_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solutionsbyusers",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    user_step = table.Column<int>(type: "integer", nullable: false),
                    user_line_number = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    test_id = table.Column<int>(type: "integer", nullable: false),
                    step_difficult = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solutionsbyusers", x => new { x.session_id, x.user_id, x.user_step, x.user_line_number, x.order_number, x.test_id });
                    table.ForeignKey(
                        name: "FK_solutionsbyusers_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_solutionsbyusers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_session_id",
                table: "grades",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSessions_group_id",
                table: "groupsessions",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionsByUsers_UserId",
                table: "solutionsbyusers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tests_algo_id",
                table: "tests",
                column: "algo_id");

            migrationBuilder.CreateIndex(
                name: "IX_trackedvariables_algo_id_algo_step",
                table: "trackedvariables",
                columns: new[] { "algo_id", "algo_step" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_group_id",
                table: "users",
                column: "group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grades");

            migrationBuilder.DropTable(
                name: "groupsessions");

            migrationBuilder.DropTable(
                name: "sessiontests");

            migrationBuilder.DropTable(
                name: "solutionsbyprograms");

            migrationBuilder.DropTable(
                name: "solutionsbyusers");

            migrationBuilder.DropTable(
                name: "testinputdata");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "teststepresponses");

            migrationBuilder.DropTable(
                name: "trackedvariables");

            migrationBuilder.DropTable(
                name: "usertestabilities");

            migrationBuilder.DropTable(
                name: "variables_solutions_by_programs");

            migrationBuilder.DropTable(
                name: "variables_solutions_by_users");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "algorithmsteps");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "algorithms");
        }
    }
}
