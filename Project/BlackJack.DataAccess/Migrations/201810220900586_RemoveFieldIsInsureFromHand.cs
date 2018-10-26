namespace BlackJack.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFieldIsInsureFromHand : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Hands", "IsInsure");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Hands", "IsInsure", c => c.Boolean(nullable: false));
        }
    }
}
