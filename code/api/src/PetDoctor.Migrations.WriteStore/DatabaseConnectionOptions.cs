namespace PetDoctor.Migrations.WriteStore
{
    public class DatabaseConnectionOptions
    {
        public string WriteStore { get; set; } = string.Empty;
        public string Master { get; set; } = string.Empty;
        public string ApplicationDatabaseName { get; set; } = string.Empty;
    }
}
