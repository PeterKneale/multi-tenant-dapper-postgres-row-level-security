using FluentMigrator;

namespace Demo.Infrastructure.Database;

[Migration(1)]
public class Migration1 : Migration
{
    public override void Up()
    {
        Create.Table("cars")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("registration").AsString().Nullable().Unique()
            .WithColumn("data").AsCustom("jsonb").NotNullable();

        Create.Table("persons")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("data").AsCustom("jsonb").NotNullable();
        
        // Index Person.HasLicence
        Execute.Sql("create index idx_person_has_licence on persons (((data ->> 'HasLicence')::bool));");
        
        // Index Car.Owner.Id
        Execute.Sql("create index idx_car_owner_id on cars (((data->'Owner'->>'Id')::uuid));");
    }

    public override void Down()
    {
        Delete.Table("cars");
        Delete.Table("persons");
    }
}