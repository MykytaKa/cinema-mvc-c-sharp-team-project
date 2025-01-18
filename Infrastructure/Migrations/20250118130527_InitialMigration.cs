using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Genre_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Genre_ID);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Region_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Region_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Region_ID);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Status_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Status_ID);
                });

            migrationBuilder.CreateTable(
                name: "User_Types",
                columns: table => new
                {
                    ID_Type = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Types", x => x.ID_Type);
                });

            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    Film_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Film_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Poster = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Trailer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actors = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Release_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration_Of_The_Movie = table.Column<TimeSpan>(type: "time", nullable: false),
                    Age_Rating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre_ID = table.Column<int>(type: "int", nullable: false),
                    Genre_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.Film_ID);
                    table.ForeignKey(
                        name: "FK_Films_Genres_Genre_ID1",
                        column: x => x.Genre_ID1,
                        principalTable: "Genres",
                        principalColumn: "Genre_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    Cinema_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cinema_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region_ID = table.Column<int>(type: "int", nullable: false),
                    Region_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.Cinema_ID);
                    table.ForeignKey(
                        name: "FK_Cinemas_Regions_Region_ID1",
                        column: x => x.Region_ID1,
                        principalTable: "Regions",
                        principalColumn: "Region_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    First_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone_Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hash_Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date_Of_Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_Type = table.Column<int>(type: "int", nullable: false),
                    User_TypeID_Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_Users_User_Types_User_TypeID_Type",
                        column: x => x.User_TypeID_Type,
                        principalTable: "User_Types",
                        principalColumn: "ID_Type",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    Hall_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hall_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number_Of_Seats = table.Column<int>(type: "int", nullable: false),
                    Cinema_ID = table.Column<int>(type: "int", nullable: false),
                    Cinema_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.Hall_ID);
                    table.ForeignKey(
                        name: "FK_Halls_Cinemas_Cinema_ID1",
                        column: x => x.Cinema_ID1,
                        principalTable: "Cinemas",
                        principalColumn: "Cinema_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    Seat_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Column = table.Column<int>(type: "int", nullable: false),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Hall_ID = table.Column<int>(type: "int", nullable: false),
                    Hall_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Seat_ID);
                    table.ForeignKey(
                        name: "FK_Seats_Halls_Hall_ID1",
                        column: x => x.Hall_ID1,
                        principalTable: "Halls",
                        principalColumn: "Hall_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Session_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date_Time_Beg = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Time_End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Film_ID = table.Column<int>(type: "int", nullable: false),
                    Film_ID1 = table.Column<int>(type: "int", nullable: false),
                    Hall_ID = table.Column<int>(type: "int", nullable: false),
                    Hall_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Session_ID);
                    table.ForeignKey(
                        name: "FK_Sessions_Films_Film_ID1",
                        column: x => x.Film_ID1,
                        principalTable: "Films",
                        principalColumn: "Film_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Halls_Hall_ID1",
                        column: x => x.Hall_ID1,
                        principalTable: "Halls",
                        principalColumn: "Hall_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Booking_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    User_ID1 = table.Column<int>(type: "int", nullable: false),
                    Session_ID = table.Column<int>(type: "int", nullable: false),
                    Session_ID1 = table.Column<int>(type: "int", nullable: false),
                    Status_ID = table.Column<int>(type: "int", nullable: false),
                    Status_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Booking_ID);
                    table.ForeignKey(
                        name: "FK_Bookings_Sessions_Session_ID1",
                        column: x => x.Session_ID1,
                        principalTable: "Sessions",
                        principalColumn: "Session_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Status_Status_ID1",
                        column: x => x.Status_ID1,
                        principalTable: "Status",
                        principalColumn: "Status_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_User_ID1",
                        column: x => x.User_ID1,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Ticket_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Booking_ID = table.Column<int>(type: "int", nullable: false),
                    Seat_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Ticket_ID);
                    table.ForeignKey(
                        name: "FK_Tickets_Bookings_Booking_ID",
                        column: x => x.Booking_ID,
                        principalTable: "Bookings",
                        principalColumn: "Booking_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Seats_Seat_ID",
                        column: x => x.Seat_ID,
                        principalTable: "Seats",
                        principalColumn: "Seat_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Session_ID1",
                table: "Bookings",
                column: "Session_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status_ID1",
                table: "Bookings",
                column: "Status_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_User_ID1",
                table: "Bookings",
                column: "User_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Cinemas_Region_ID1",
                table: "Cinemas",
                column: "Region_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Films_Genre_ID1",
                table: "Films",
                column: "Genre_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Halls_Cinema_ID1",
                table: "Halls",
                column: "Cinema_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Hall_ID1",
                table: "Seats",
                column: "Hall_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Film_ID1",
                table: "Sessions",
                column: "Film_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Hall_ID1",
                table: "Sessions",
                column: "Hall_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Booking_ID",
                table: "Tickets",
                column: "Booking_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Seat_ID",
                table: "Tickets",
                column: "Seat_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_User_TypeID_Type",
                table: "Users",
                column: "User_TypeID_Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropTable(
                name: "Halls");

            migrationBuilder.DropTable(
                name: "User_Types");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Cinemas");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
