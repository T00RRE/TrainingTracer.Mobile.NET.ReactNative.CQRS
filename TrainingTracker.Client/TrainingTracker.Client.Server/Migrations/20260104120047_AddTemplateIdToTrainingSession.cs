using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingTracker.Client.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateIdToTrainingSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "TrainingSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "TrainingSessions");
        }
    }
}
