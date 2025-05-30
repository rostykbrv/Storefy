using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Services.Data;

/// <summary>
/// MS SQL Database context for the application.
/// </summary>
public class StorefyDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorefyDbContext"/> class.
    /// </summary>
    /// <param name="options">DbContextOptions to be used.</param>
    public StorefyDbContext(DbContextOptions<StorefyDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the games represented as a set within the context.
    /// </summary>
    public DbSet<Game> Games { get; set; }

    /// <summary>
    /// Gets or sets the genres represented as a set within the context.
    /// </summary>
    public DbSet<Genre> Genres { get; set; }

    /// <summary>
    /// Gets or sets the platforms represented as a set within the context.
    /// </summary>
    public DbSet<Platform> Platforms { get; set; }

    /// <summary>
    /// Gets or sets the publishers represented as a set within the context.
    /// </summary>
    public DbSet<Publisher> Publishers { get; set; }

    /// <summary>
    /// Gets or sets the payment methods represented as a set within the context.
    /// </summary>
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    /// <summary>
    /// Gets or sets the orders represented as a set within the context.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Gets or sets the comments represented as a set within the context.
    /// </summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>
    /// Gets or sets the user represented as a set within the context.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the role represented as a set within the context.
    /// </summary>
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Gets or sets the role permissions represented as a set within the context.
    /// </summary>
    public DbSet<Permissions> Permissions { get; set; }

    /// <summary>
    /// Gets or sets the role-permissions relationship represented as a set within the context.
    /// </summary>
    public DbSet<RolePermissions> RolePermissions { get; set; }

    /// <summary>
    /// Gets or sets the ban history represented as a set within the context.
    /// </summary>
    public DbSet<BanHistory> BanHistories { get; set; }

    /// <summary>
    /// Gets or sets the languages represented as a set within the context.
    /// </summary>
    public DbSet<Language> Languages { get; set; }

    /// <summary>
    /// Gets or sets the game translations represented as a set within the context.
    /// </summary>
    public DbSet<GameTranslation> GameTranslations { get; set; }

    /// <summary>
    /// Gets or sets the genre translations represented as a set within the context.
    /// </summary>
    public DbSet<GenreTranslation> GenreTranslations { get; set; }

    /// <summary>
    /// Gets or sets the platform translations represented as a set within the context.
    /// </summary>
    public DbSet<PlatformTranslation> PlatformTranslations { get; set; }

    /// <summary>
    /// Gets or sets the notifications types represented as a set within the context.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasIndex(g => g.Price)
            .HasDatabaseName("IX_Games_PRICE");

        modelBuilder.Entity<Game>()
            .HasIndex(g => g.DateAdded)
            .HasDatabaseName("IX_Game_DateAdded");

        modelBuilder.Entity<Game>()
            .HasMany(g => g.GameTranslations)
            .WithOne(gt => gt.Game)
            .HasForeignKey(gt => gt.GameId);

        modelBuilder.Entity<Genre>()
            .HasMany(g => g.GenreTranslations)
            .WithOne(gt => gt.Genre)
            .HasForeignKey(gt => gt.GenreId);

        modelBuilder.Entity<Platform>()
            .HasMany(p => p.PlatformTranslations)
            .WithOne(pt => pt.Platform)
            .HasForeignKey(pt => pt.PlatformId);

        modelBuilder.Entity<Language>()
            .HasMany(l => l.GameTranslations)
            .WithOne(gt => gt.Language)
            .HasForeignKey(gt => gt.LanguageId);

        modelBuilder.Entity<Language>()
            .HasMany(l => l.GenreTranslations)
            .WithOne(gt => gt.Language)
            .HasForeignKey(gt => gt.LanguageId);

        modelBuilder.Entity<Language>()
            .HasMany(l => l.PlatformTranslations)
            .WithOne(pt => pt.Language)
            .HasForeignKey(pt => pt.LanguageId);

        modelBuilder.Entity<GameTranslation>()
            .HasOne(gt => gt.Game)
            .WithMany(g => g.GameTranslations)
            .HasForeignKey(gt => gt.GameId);

        modelBuilder.Entity<PlatformTranslation>()
            .HasOne(pt => pt.Platform)
            .WithMany(p => p.PlatformTranslations)
            .HasForeignKey(pt => pt.PlatformId);

        modelBuilder.Entity<GameTranslation>()
            .HasOne(gt => gt.Language)
            .WithMany(l => l.GameTranslations)
            .HasForeignKey(gt => gt.LanguageId);

        modelBuilder.Entity<PlatformTranslation>()
            .HasOne(pt => pt.Language)
            .WithMany(l => l.PlatformTranslations)
            .HasForeignKey(pt => pt.LanguageId);

        modelBuilder.Entity<GenreTranslation>()
            .HasOne(gt => gt.Genre)
            .WithMany(g => g.GenreTranslations)
            .HasForeignKey(gt => gt.GenreId);

        modelBuilder.Entity<GameTranslation>()
            .HasOne(gt => gt.Language)
            .WithMany(l => l.GameTranslations)
            .HasForeignKey(gt => gt.LanguageId);

        modelBuilder.Entity<GenreTranslation>()
            .HasOne(gt => gt.Language)
            .WithMany(l => l.GenreTranslations)
            .HasForeignKey(gt => gt.LanguageId);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Genres)
            .WithMany(g => g.Games);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Platforms)
            .WithMany(p => p.Games);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Publisher)
            .WithMany(p => p.Games)
            .HasForeignKey(g => g.PublisherId);

        modelBuilder.Entity<OrderDetails>()
            .HasOne(od => od.Game)
            .WithMany(g => g.OrderDetails)
            .HasForeignKey(od => od.ProductId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users);

        modelBuilder.Entity<User>()
            .HasOne(u => u.NotificationType)
            .WithMany(n => n.UserNotificationType)
            .HasForeignKey(u => u.NotificationId);

        modelBuilder.Entity<RolePermissions>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermissions>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermissions>()
            .HasOne(rp => rp.Permissions)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        modelBuilder.Entity<Notification>().HasData(
            new Notification { Id = Guid.NewGuid().ToString(), Type = "Sms" },
            new Notification { Id = Guid.NewGuid().ToString(), Type = "Email" },
            new Notification { Id = Guid.NewGuid().ToString(), Type = "Push" });

        modelBuilder.Entity<Genre>().HasData(
        new Genre { Id = "cd536d56-74ac-11ee-b962-0242ac120002", Name = "Strategy" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "RTS", ParentGenreId = "cd536d56-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "TBS", ParentGenreId = "cd536d56-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = "cd5370e4-74ac-11ee-b962-0242ac120002", Name = "RPG" },
        new Genre { Id = "cd537260-74ac-11ee-b962-0242ac120002", Name = "Sports" },
        new Genre { Id = "cd5375a8-74ac-11ee-b962-0242ac120002", Name = "Races" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "Rally", ParentGenreId = "cd5375a8-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "Arcade", ParentGenreId = "cd5375a8-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "Formula", ParentGenreId = "cd5375a8-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "Off-road", ParentGenreId = "cd5375a8-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = "cd5376d4-74ac-11ee-b962-0242ac120002", Name = "Action" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "FPS", ParentGenreId = "cd5376d4-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "TPS", ParentGenreId = "cd5376d4-74ac-11ee-b962-0242ac120002" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "Adventure" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "Puzzle & Skill" },
        new Genre { Id = Guid.NewGuid().ToString(), Name = "Misc" });

        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = Guid.NewGuid().ToString(), Type = "Mobile" },
            new Platform { Id = Guid.NewGuid().ToString(), Type = "Browser" },
            new Platform { Id = Guid.NewGuid().ToString(), Type = "Dekstop" },
            new Platform { Id = Guid.NewGuid().ToString(), Type = "Console" });

        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Bank",
                Description = "Payment through bank. Generating invoice file",
                ImageUrl = "https://nuvei.com/wp-content/uploads/2021/06/regular-bank-transfer.png",
            },
            new PaymentMethod
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Visa",
                Description = "Payment through Visa system.",
                ImageUrl = "https://d2csxpduxe849s.cloudfront.net/media/F44207E3-1DDE-4798-B0FCC94F6227FCB7/FD889B2B-B4FE-445C-97A356E3955CC1CC/webimage-ED81074F-347A-430E-AC7CC0A3429D9570.jpg",
            },
            new PaymentMethod
            {
                Id = Guid.NewGuid().ToString(),
                Title = "IBox terminal",
                Description = "Payment through IBox terminal.",
                ImageUrl = "https://campus.rv.ua/sites/default/files/news-images/ibox_terminal.png",
            });

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = "069363ee-64a8-4156-859a-87c89ce31e3e",
                Name = "Administrator",
            },
            new Role
            {
                Id = "09d91cb4-634d-43ee-ba3b-ea94937297fb",
                Name = "Manager",
            },
            new Role
            {
                Id = "70f9d4de-8960-48ea-8f5b-f1e7fd982b1f",
                Name = "Moderator",
            },
            new Role
            {
                Id = "158b3e70-558f-4119-bfe6-a50344f55f9e",
                Name = "User",
            });

        modelBuilder.Entity<Permissions>().HasData(
            new Permissions { Id = "72988a76-aaed-451e-85d4-069aeb1cf379", Name = "Comments" },
            new Permissions { Id = "72988a76-aaed-451e-85d4-069aeb1cf378", Name = "Buy" },
            new Permissions { Id = "72988a76-aaed-451e-85d4-069aeb1cf374", Name = "Game" },
            new Permissions { Id = "62988a76-aaed-451e-85d4-069aeb1cf374", Name = "UpdateGame" },
            new Permissions { Id = "52988a76-aaed-451e-85d4-069aeb1cf374", Name = "DeleteGame" },
            new Permissions { Id = "42988a76-aaed-451e-85d4-069aeb1cf374", Name = "AddGame" },
            new Permissions { Id = "32988a76-aaed-451e-85d4-069aeb1cf374", Name = "Games" },
            new Permissions { Id = "33988a76-aaed-451e-85d4-069aeb1cf374", Name = "Genres" },
            new Permissions { Id = "34988a76-aaed-451e-85d4-069aeb1cf374", Name = "Genre" },
            new Permissions { Id = "35988a76-aaed-451e-85d4-069aeb1cf374", Name = "AddGenre" },
            new Permissions { Id = "36988a76-aaed-451e-85d4-069aeb1cf374", Name = "DeleteGenre" },
            new Permissions { Id = "37988a76-aaed-451e-85d4-069aeb1cf375", Name = "UpdateGenre" },
            new Permissions { Id = "38988a76-aaed-451e-85d4-069aeb1cf374", Name = "UpdatePlatform" },
            new Permissions { Id = "39988a76-aaed-451e-85d4-069aeb1cf374", Name = "DeletePlatform" },
            new Permissions { Id = "37888a76-aaed-451e-85d4-069aeb1cf374", Name = "Platform" },
            new Permissions { Id = "37788a76-aaed-451e-85d4-069aeb1cf374", Name = "AddPlatform" },
            new Permissions { Id = "37900a76-aaed-451e-85d4-069aeb1cf374", Name = "Platforms" },
            new Permissions { Id = "37911a76-aaed-451e-85d4-069aeb1cf374", Name = "Publishers" },
            new Permissions { Id = "37922a76-aaed-451e-85d4-069aeb1cf374", Name = "Publisher" },
            new Permissions { Id = "37933a76-aaed-451e-85d4-069aeb1cf374", Name = "UpdatePublisher" },
            new Permissions { Id = "37944a76-aaed-451e-85d4-069aeb1cf374", Name = "DeletePublisher" },
            new Permissions { Id = "37955a76-aaed-451e-85d4-069aeb1cf374", Name = "AddPublisher" },
            new Permissions { Id = "37966a76-aaed-451e-85d4-069aeb1cf374", Name = "Order" },
            new Permissions { Id = "37977a76-aaed-451e-85d4-069aeb1cf374", Name = "UpdateOrder" },
            new Permissions { Id = "37988a76-aaed-451e-85d4-069aeb1cf374", Name = "History" },
            new Permissions { Id = "79900a76-aaed-451e-85d4-069aeb1cf374", Name = "Orders" },
            new Permissions { Id = "79911a76-aaed-451e-85d4-069aeb1cf374", Name = "Basket" },
            new Permissions { Id = "79922a76-aaed-451e-85d4-069aeb1cf374", Name = "MakeOrder" },
            new Permissions { Id = "79933a76-aaed-451e-85d4-069aeb1cf374", Name = "Users" },
            new Permissions { Id = "79944a76-aaed-451e-85d4-069aeb1cf374", Name = "User" },
            new Permissions { Id = "79955a76-aaed-451e-85d4-069aeb1cf374", Name = "UpdateUser" },
            new Permissions { Id = "79966a76-aaed-451e-85d4-069aeb1cf374", Name = "DeleteUser" },
            new Permissions { Id = "79977a76-aaed-451e-85d4-069aeb1cf374", Name = "AddUser" },
            new Permissions { Id = "79988a76-aaed-451e-85d4-069aeb1cf374", Name = "Login" },
            new Permissions { Id = "79999a76-aaed-451e-85d4-069aeb1cf374", Name = "Roles" },
            new Permissions { Id = "799aa876-aaed-451e-85d4-069aeb1cf374", Name = "Role" },
            new Permissions { Id = "799bb976-aaed-451e-85d4-069aeb1cf374", Name = "UpdateRole" },
            new Permissions { Id = "799cca76-aaed-451e-85d4-069aeb1cf374", Name = "DeleteRole" },
            new Permissions { Id = "799dda76-aaed-451e-85d4-069aeb1cf374", Name = "AddRole" },
            new Permissions { Id = "799cca76-aaed-451e-82d4-069aeb1cf374", Name = "ShipOrder" },
            new Permissions { Id = "799cca76-aaed-451e-81d4-069aeb1cf374", Name = "ReplyComment" },
            new Permissions { Id = "799cca76-aaed-451e-83d4-069aeb1cf374", Name = "QuoteComment" },
            new Permissions { Id = "799cca76-aaed-451e-86d4-069aeb1cf374", Name = "DeleteComment" },
            new Permissions { Id = "799cca76-aaed-451e-87d4-069aeb1cf374", Name = "BanComment" },
            new Permissions { Id = "799cca76-aaed-451e-88d4-069aeb1cf374", Name = "AddComment" });

        var administratorPermissions = new string[]
        {
            "799cca76-aaed-451e-82d4-069aeb1cf374",
            "799cca76-aaed-451e-81d4-069aeb1cf374",
            "799cca76-aaed-451e-83d4-069aeb1cf374",
            "799cca76-aaed-451e-86d4-069aeb1cf374",
            "799cca76-aaed-451e-87d4-069aeb1cf374",
            "799cca76-aaed-451e-88d4-069aeb1cf374",
            "72988a76-aaed-451e-85d4-069aeb1cf379",
            "72988a76-aaed-451e-85d4-069aeb1cf378",
            "62988a76-aaed-451e-85d4-069aeb1cf374",
            "72988a76-aaed-451e-85d4-069aeb1cf374",
            "52988a76-aaed-451e-85d4-069aeb1cf374",
            "42988a76-aaed-451e-85d4-069aeb1cf374",
            "32988a76-aaed-451e-85d4-069aeb1cf374",
            "33988a76-aaed-451e-85d4-069aeb1cf374",
            "34988a76-aaed-451e-85d4-069aeb1cf374",
            "35988a76-aaed-451e-85d4-069aeb1cf374",
            "36988a76-aaed-451e-85d4-069aeb1cf374",
            "37988a76-aaed-451e-85d4-069aeb1cf375",
            "38988a76-aaed-451e-85d4-069aeb1cf374",
            "39988a76-aaed-451e-85d4-069aeb1cf374",
            "37888a76-aaed-451e-85d4-069aeb1cf374",
            "37788a76-aaed-451e-85d4-069aeb1cf374",
            "37900a76-aaed-451e-85d4-069aeb1cf374",
            "37911a76-aaed-451e-85d4-069aeb1cf374",
            "37922a76-aaed-451e-85d4-069aeb1cf374",
            "37933a76-aaed-451e-85d4-069aeb1cf374",
            "37944a76-aaed-451e-85d4-069aeb1cf374",
            "37955a76-aaed-451e-85d4-069aeb1cf374",
            "37966a76-aaed-451e-85d4-069aeb1cf374",
            "37977a76-aaed-451e-85d4-069aeb1cf374",
            "37988a76-aaed-451e-85d4-069aeb1cf374",
            "79900a76-aaed-451e-85d4-069aeb1cf374",
            "79911a76-aaed-451e-85d4-069aeb1cf374",
            "79922a76-aaed-451e-85d4-069aeb1cf374",
            "79933a76-aaed-451e-85d4-069aeb1cf374",
            "79944a76-aaed-451e-85d4-069aeb1cf374",
            "79955a76-aaed-451e-85d4-069aeb1cf374",
            "79966a76-aaed-451e-85d4-069aeb1cf374",
            "79977a76-aaed-451e-85d4-069aeb1cf374",
            "79988a76-aaed-451e-85d4-069aeb1cf374",
            "79999a76-aaed-451e-85d4-069aeb1cf374",
            "799aa876-aaed-451e-85d4-069aeb1cf374",
            "799bb976-aaed-451e-85d4-069aeb1cf374",
            "799cca76-aaed-451e-85d4-069aeb1cf374",
            "799dda76-aaed-451e-85d4-069aeb1cf374",
        };

        var managerPermissions = new string[]
        {
            "799cca76-aaed-451e-82d4-069aeb1cf374",
            "799cca76-aaed-451e-81d4-069aeb1cf374",
            "799cca76-aaed-451e-83d4-069aeb1cf374",
            "799cca76-aaed-451e-86d4-069aeb1cf374",
            "799cca76-aaed-451e-87d4-069aeb1cf374",
            "799cca76-aaed-451e-88d4-069aeb1cf374",
            "72988a76-aaed-451e-85d4-069aeb1cf379",
            "72988a76-aaed-451e-85d4-069aeb1cf378",
            "62988a76-aaed-451e-85d4-069aeb1cf374",
            "72988a76-aaed-451e-85d4-069aeb1cf374",
            "52988a76-aaed-451e-85d4-069aeb1cf374",
            "42988a76-aaed-451e-85d4-069aeb1cf374",
            "32988a76-aaed-451e-85d4-069aeb1cf374",
            "33988a76-aaed-451e-85d4-069aeb1cf374",
            "34988a76-aaed-451e-85d4-069aeb1cf374",
            "35988a76-aaed-451e-85d4-069aeb1cf374",
            "36988a76-aaed-451e-85d4-069aeb1cf374",
            "37988a76-aaed-451e-85d4-069aeb1cf375",
            "38988a76-aaed-451e-85d4-069aeb1cf374",
            "39988a76-aaed-451e-85d4-069aeb1cf374",
            "37888a76-aaed-451e-85d4-069aeb1cf374",
            "37788a76-aaed-451e-85d4-069aeb1cf374",
            "37900a76-aaed-451e-85d4-069aeb1cf374",
            "37911a76-aaed-451e-85d4-069aeb1cf374",
            "37922a76-aaed-451e-85d4-069aeb1cf374",
            "37933a76-aaed-451e-85d4-069aeb1cf374",
            "37944a76-aaed-451e-85d4-069aeb1cf374",
            "37955a76-aaed-451e-85d4-069aeb1cf374",
            "37966a76-aaed-451e-85d4-069aeb1cf374",
            "37977a76-aaed-451e-85d4-069aeb1cf374",
            "37988a76-aaed-451e-85d4-069aeb1cf374",
            "79900a76-aaed-451e-85d4-069aeb1cf374",
            "79911a76-aaed-451e-85d4-069aeb1cf374",
            "79922a76-aaed-451e-85d4-069aeb1cf374",
            "79933a76-aaed-451e-85d4-069aeb1cf374",
            "79944a76-aaed-451e-85d4-069aeb1cf374",
        };

        var moderatorPermissions = new string[]
        {
            "72988a76-aaed-451e-85d4-069aeb1cf379",
            "72988a76-aaed-451e-85d4-069aeb1cf374",
            "32988a76-aaed-451e-85d4-069aeb1cf374",
            "33988a76-aaed-451e-85d4-069aeb1cf374",
            "34988a76-aaed-451e-85d4-069aeb1cf374",
            "37888a76-aaed-451e-85d4-069aeb1cf374",
            "37900a76-aaed-451e-85d4-069aeb1cf374",
            "37911a76-aaed-451e-85d4-069aeb1cf374",
            "37922a76-aaed-451e-85d4-069aeb1cf374",
            "799cca76-aaed-451e-88d4-069aeb1cf374",
            "799cca76-aaed-451e-81d4-069aeb1cf374",
            "799cca76-aaed-451e-83d4-069aeb1cf374",
            "799cca76-aaed-451e-86d4-069aeb1cf374",
            "799cca76-aaed-451e-87d4-069aeb1cf374",
        };

        var userPermissions = new string[]
        {
            "72988a76-aaed-451e-85d4-069aeb1cf379",
            "72988a76-aaed-451e-85d4-069aeb1cf374",
            "32988a76-aaed-451e-85d4-069aeb1cf374",
            "33988a76-aaed-451e-85d4-069aeb1cf374",
            "34988a76-aaed-451e-85d4-069aeb1cf374",
            "37888a76-aaed-451e-85d4-069aeb1cf374",
            "37900a76-aaed-451e-85d4-069aeb1cf374",
            "37911a76-aaed-451e-85d4-069aeb1cf374",
            "37922a76-aaed-451e-85d4-069aeb1cf374",
            "799cca76-aaed-451e-88d4-069aeb1cf374",
            "799cca76-aaed-451e-81d4-069aeb1cf374",
            "799cca76-aaed-451e-83d4-069aeb1cf374",
        };

        foreach (var permissionId in administratorPermissions)
        {
            modelBuilder.Entity<RolePermissions>().HasData(
                new RolePermissions { RoleId = "069363ee-64a8-4156-859a-87c89ce31e3e", PermissionId = permissionId });
        }

        foreach (var permissionId in managerPermissions)
        {
            modelBuilder.Entity<RolePermissions>().HasData(
                new RolePermissions { RoleId = "09d91cb4-634d-43ee-ba3b-ea94937297fb", PermissionId = permissionId });
        }

        foreach (var permissionId in moderatorPermissions)
        {
            modelBuilder.Entity<RolePermissions>().HasData(
                new RolePermissions { RoleId = "70f9d4de-8960-48ea-8f5b-f1e7fd982b1f", PermissionId = permissionId });
        }

        foreach (var permissionId in userPermissions)
        {
            modelBuilder.Entity<RolePermissions>().HasData(
                new RolePermissions { RoleId = "158b3e70-558f-4119-bfe6-a50344f55f9e", PermissionId = permissionId });
        }
    }
}
