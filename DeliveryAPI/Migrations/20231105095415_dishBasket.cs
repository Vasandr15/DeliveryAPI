using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryAPI.Migrations
{
    /// <inheritdoc />
    public partial class dishBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DishId",
                table: "DishBaskets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishBaskets_DishId",
                table: "DishBaskets",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishBaskets_Dishes_DishId",
                table: "DishBaskets",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishBaskets_Dishes_DishId",
                table: "DishBaskets");

            migrationBuilder.DropIndex(
                name: "IX_DishBaskets_DishId",
                table: "DishBaskets");

            migrationBuilder.AlterColumn<Guid>(
                name: "DishId",
                table: "DishBaskets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
