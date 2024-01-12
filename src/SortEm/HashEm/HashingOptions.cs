namespace HashEm;

public class HashingOptions
{
    public const string SearchRootDefault = @".\";
    public const string MetaDataBasePathDefault = @$"{SearchRootDefault}metadata";
    public const string ConnectionStringDefault = @$"Data Source={MetaDataBasePathDefault}\hashing.db";

    public string SearchRoot { get; set; } = SearchRootDefault;
    public string MetaDataBasePath { get; set; } = MetaDataBasePathDefault;
    public string ConnectionString { get; set; } = ConnectionStringDefault;
}
