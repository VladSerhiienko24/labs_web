namespace BlackJack.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializeDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Suit = c.Int(nullable: false),
                        Face = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HandCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HandId = c.Int(nullable: false),
                        CardId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cards", t => t.CardId, cascadeDelete: true)
                .ForeignKey("dbo.Hands", t => t.HandId, cascadeDelete: true)
                .Index(t => t.HandId)
                .Index(t => t.CardId);
            
            CreateTable(
                "dbo.Hands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Summary = c.Int(nullable: false),
                        VictoryType = c.Int(nullable: false),
                        Situation = c.Int(nullable: false),
                        Deal = c.Int(nullable: false),
                        IsInsure = c.Boolean(nullable: false),
                        InsuranceCoins = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        RoundId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .ForeignKey("dbo.Rounds", t => t.RoundId, cascadeDelete: true)
                .Index(t => t.PlayerId)
                .Index(t => t.RoundId);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NickName = c.String(nullable: false, maxLength: 20),
                        PlayerRole = c.Int(nullable: false),
                        Coins = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlayerGames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayerId = c.Int(nullable: false),
                        GameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.PlayerId)
                .Index(t => t.GameId);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaxCountRounds = c.Int(nullable: false),
                        CoinsAtStart = c.Int(nullable: false),
                        CountBots = c.Int(nullable: false),
                        Reward = c.Int(nullable: false),
                        GameStart = c.DateTime(nullable: false),
                        IsFinished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rounds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfRound = c.Int(nullable: false),
                        GameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .Index(t => t.GameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HandCards", "HandId", "dbo.Hands");
            DropForeignKey("dbo.PlayerGames", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.PlayerGames", "GameId", "dbo.Games");
            DropForeignKey("dbo.Hands", "RoundId", "dbo.Rounds");
            DropForeignKey("dbo.Rounds", "GameId", "dbo.Games");
            DropForeignKey("dbo.Hands", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.HandCards", "CardId", "dbo.Cards");
            DropIndex("dbo.Rounds", new[] { "GameId" });
            DropIndex("dbo.PlayerGames", new[] { "GameId" });
            DropIndex("dbo.PlayerGames", new[] { "PlayerId" });
            DropIndex("dbo.Hands", new[] { "RoundId" });
            DropIndex("dbo.Hands", new[] { "PlayerId" });
            DropIndex("dbo.HandCards", new[] { "CardId" });
            DropIndex("dbo.HandCards", new[] { "HandId" });
            DropTable("dbo.Rounds");
            DropTable("dbo.Games");
            DropTable("dbo.PlayerGames");
            DropTable("dbo.Players");
            DropTable("dbo.Hands");
            DropTable("dbo.HandCards");
            DropTable("dbo.Cards");
        }
    }
}
