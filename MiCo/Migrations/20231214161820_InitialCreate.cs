using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiCo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tag = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    pfp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_banned_user = table.Column<int>(type: "int", nullable: false),
                    id_moderator = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ban_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ban_until = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bans", x => x.id);
                    table.ForeignKey(
                        name: "FK_bans_users_id_banned_user",
                        column: x => x.id_banned_user,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_bans_users_id_moderator",
                        column: x => x.id_moderator,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_reported_user = table.Column<int>(type: "int", nullable: false),
                    id_reporting_user = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    report_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_reports_users_id_reported_user",
                        column: x => x.id_reported_user,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_reports_users_id_reporting_user",
                        column: x => x.id_reporting_user,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "threads",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_author = table.Column<int>(type: "int", nullable: false),
                    id_reply = table.Column<int>(type: "int", nullable: true),
                    id_OG_thread = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_threads", x => x.id);
                    table.ForeignKey(
                        name: "FK_threads_threads_id_OG_thread",
                        column: x => x.id_OG_thread,
                        principalTable: "threads",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_threads_threads_id_reply",
                        column: x => x.id_reply,
                        principalTable: "threads",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_threads_users_id_author",
                        column: x => x.id_author,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_user = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expiration_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    expired = table.Column<bool>(type: "bit", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_users_id_user",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "likes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_user = table.Column<int>(type: "int", nullable: false),
                    id_thread = table.Column<int>(type: "int", nullable: false),
                    like_or_dislike = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_likes_threads_id_thread",
                        column: x => x.id_thread,
                        principalTable: "threads",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_likes_users_id_user",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "thread_images",
                columns: table => new
                {
                    id_thread = table.Column<int>(type: "int", nullable: false),
                    id_image = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thread_images", x => new { x.id_thread, x.id_image });
                    table.ForeignKey(
                        name: "FK_thread_images_images_id_image",
                        column: x => x.id_image,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_thread_images_threads_id_thread",
                        column: x => x.id_thread,
                        principalTable: "threads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "thread_tags",
                columns: table => new
                {
                    id_thread = table.Column<int>(type: "int", nullable: false),
                    id_tag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thread_tags", x => new { x.id_thread, x.id_tag });
                    table.ForeignKey(
                        name: "FK_thread_tags_tags_id_tag",
                        column: x => x.id_tag,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_thread_tags_threads_id_thread",
                        column: x => x.id_thread,
                        principalTable: "threads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bans_id_banned_user",
                table: "bans",
                column: "id_banned_user");

            migrationBuilder.CreateIndex(
                name: "IX_bans_id_moderator",
                table: "bans",
                column: "id_moderator");

            migrationBuilder.CreateIndex(
                name: "IX_likes_id_thread",
                table: "likes",
                column: "id_thread");

            migrationBuilder.CreateIndex(
                name: "IX_likes_id_user",
                table: "likes",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_reports_id_reported_user",
                table: "reports",
                column: "id_reported_user");

            migrationBuilder.CreateIndex(
                name: "IX_reports_id_reporting_user",
                table: "reports",
                column: "id_reporting_user");

            migrationBuilder.CreateIndex(
                name: "IX_thread_images_id_image",
                table: "thread_images",
                column: "id_image");

            migrationBuilder.CreateIndex(
                name: "IX_thread_tags_id_tag",
                table: "thread_tags",
                column: "id_tag");

            migrationBuilder.CreateIndex(
                name: "IX_threads_id_author",
                table: "threads",
                column: "id_author");

            migrationBuilder.CreateIndex(
                name: "IX_threads_id_OG_thread",
                table: "threads",
                column: "id_OG_thread");

            migrationBuilder.CreateIndex(
                name: "IX_threads_id_reply",
                table: "threads",
                column: "id_reply");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_id_user",
                table: "tokens",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bans");

            migrationBuilder.DropTable(
                name: "likes");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "thread_images");

            migrationBuilder.DropTable(
                name: "thread_tags");

            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "threads");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
