using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone_Number",
                table: "Users",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Last_Name",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Hash_Password",
                table: "Users",
                newName: "HashPassword");

            migrationBuilder.RenameColumn(
                name: "First_Name",
                table: "Users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Date_Of_Birthday",
                table: "Users",
                newName: "DateOfBirthday");

            migrationBuilder.RenameColumn(
                name: "Date_Time_End",
                table: "Sessions",
                newName: "DateTimeEnd");

            migrationBuilder.RenameColumn(
                name: "Date_Time_Beg",
                table: "Sessions",
                newName: "DateTimeBeg");

            migrationBuilder.RenameColumn(
                name: "Number_Of_Seats",
                table: "Halls",
                newName: "NumberOfSeats");

            migrationBuilder.RenameColumn(
                name: "Release_Date",
                table: "Films",
                newName: "ReleaseRate");

            migrationBuilder.RenameColumn(
                name: "Film_Name",
                table: "Films",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Duration_Of_The_Movie",
                table: "Films",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "Age_Rating",
                table: "Films",
                newName: "AgeRating");

            migrationBuilder.RenameColumn(
                name: "Date_Time",
                table: "Bookings",
                newName: "DateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Users",
                newName: "Phone_Number");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "Last_Name");

            migrationBuilder.RenameColumn(
                name: "HashPassword",
                table: "Users",
                newName: "Hash_Password");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Users",
                newName: "First_Name");

            migrationBuilder.RenameColumn(
                name: "DateOfBirthday",
                table: "Users",
                newName: "Date_Of_Birthday");

            migrationBuilder.RenameColumn(
                name: "DateTimeEnd",
                table: "Sessions",
                newName: "Date_Time_End");

            migrationBuilder.RenameColumn(
                name: "DateTimeBeg",
                table: "Sessions",
                newName: "Date_Time_Beg");

            migrationBuilder.RenameColumn(
                name: "NumberOfSeats",
                table: "Halls",
                newName: "Number_Of_Seats");

            migrationBuilder.RenameColumn(
                name: "ReleaseRate",
                table: "Films",
                newName: "Release_Date");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Films",
                newName: "Film_Name");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Films",
                newName: "Duration_Of_The_Movie");

            migrationBuilder.RenameColumn(
                name: "AgeRating",
                table: "Films",
                newName: "Age_Rating");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Bookings",
                newName: "Date_Time");
        }
    }
}
