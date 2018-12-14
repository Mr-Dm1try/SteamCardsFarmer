namespace SteamCardsFarmer.Model
{
    using System.Data.Entity;
    using SteamCardsFarmer.Model.Types;

    public class SteamGamesContext : DbContext {
        // Контекст настроен для использования строки подключения "SteamGamesContext" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "Data.SteamGamesContext" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "SteamGamesContext" 
        // в файле конфигурации приложения.
        public SteamGamesContext()
            : base("name=SteamGamesContext") {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SteamGamesContext, SteamCardsFarmer.Migrations.Configuration>());
        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<SteamGame> SteamGames { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}