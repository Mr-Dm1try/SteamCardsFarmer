namespace SteamCardsFarmer.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>Конфигурирование базы данных</summary>
    internal sealed class Configuration : DbMigrationsConfiguration<Model.SteamGamesContext>
    {
        /// <summary>Конструктор класса конфигурирования</summary>
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Model.SteamGamesContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
