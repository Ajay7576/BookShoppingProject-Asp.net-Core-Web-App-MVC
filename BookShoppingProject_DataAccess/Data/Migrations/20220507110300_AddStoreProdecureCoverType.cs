using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShoppingProject_DataAccess.Migrations
{
    public partial class AddStoreProdecureCoverType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_GetCoverTypes
                As
                  select* from CoverTypes");
            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_GetCoverType
                 @Id int
                 As                 
                   select* from CoverTypes where Id=@Id");
            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_CreateCoverType
                @Name varchar(50)
                 As 
                      insert CoverTypes Values (@Name)");
            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_UpdateCoverType
               @Id int,
                @Name Varchar(50)
                 As
                       update CoverTypes set name=@Name where Id=@Id");
            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_DeleteCoverType
               @Id int
                   AS 
                 delete from CoverTypes where Id=@Id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
