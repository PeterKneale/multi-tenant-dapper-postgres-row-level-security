using FluentMigrator;

namespace Demo.Infrastructure.Database;

[Migration(1)]
public class Migration1 : Migration
{
    public override void Up()
    {
        Create.Table("cars")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("tenant").AsString().NotNullable()
            .WithColumn("registration").AsString().Nullable().Unique()
            .WithColumn("data").AsCustom("jsonb").NotNullable();
    }

    public override void Down()
    {
        Delete.Table("cars");
    }
}