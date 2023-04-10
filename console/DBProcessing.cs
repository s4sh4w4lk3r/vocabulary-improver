using MySql.Data.MySqlClient;

namespace console;

partial class Input
{
    static private string connString = null!;
    static public string ConnString
    {
        get { return connString; }
        set
        {
            MySqlConnection connection = new MySqlConnection(value);
            try
            {
                connection.Open();
                connection.Close();
                connString = value;
            }
            catch (Exception) { }
        }
    }

    public static Dictionary<string, string> GetDict(string connString, string tableName)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        MySqlConnection connection = new MySqlConnection(connString);
        connection.Open();

        string select = $"SELECT * FROM `{tableName}` ORDER BY RAND();";

        MySqlCommand command = new MySqlCommand(select, connection);
        MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            dict.Add(reader[0].ToString()!, reader[1].ToString()!);
        }
        reader.Close();

        connection.Close();
        return dict;
    }
    public static void CreateDict(string connString, string tableName)
    {
        MySqlConnection connection = new MySqlConnection(connString);
        connection.Open();

        string create = $"CREATE TABLE `{tableName}` (`key` VARCHAR(255) NOT NULL , `value` VARCHAR(255) NULL , " +
        "PRIMARY KEY (`key`)) ENGINE = InnoDB CHARSET=utf8mb4 COLLATE utf8mb4_unicode_ci;";

        MySqlCommand command = new MySqlCommand(create, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public static void Add(string connString, string tableName, string key, string value)
    {
        MySqlConnection connection = new MySqlConnection(connString);
        connection.Open();

        string insert = $"INSERT INTO `{tableName}` (`key`, `value`) VALUES ('{key}', '{value}')";

        MySqlCommand command = new MySqlCommand(insert, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public static void Add(string connString, string tableName, Dictionary<string, string> dict)
    {
        MySqlConnection connection = new MySqlConnection(connString);
        string megaInsert = $"INSERT INTO `{tableName}` (`key`, `value`) VALUES ";
        foreach (var item in dict)
        {
            megaInsert += $"('{item.Key}', '{item.Value}'), ";
        }
        megaInsert = megaInsert.Remove(megaInsert.Length - 2) + ";";
        connection.Open();
        MySqlCommand command = new MySqlCommand(megaInsert, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public static void Remove(string connString, string tableName, string key)
    {
        MySqlConnection connection = new MySqlConnection(connString);
        connection.Open();

        string delete = $"DELETE FROM `{tableName}` WHERE `key`='{key}';";

        MySqlCommand command = new MySqlCommand(delete, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public static void DeleteDict(string connString, string tableName)
    {
        MySqlConnection connection = new MySqlConnection(connString);
        connection.Open();

        string drop = $"DROP TABLE `{tableName}`;";

        MySqlCommand command = new MySqlCommand(drop, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public static void ClearDict(string connString, string tableName)
    {
        MySqlConnection connection = new MySqlConnection(connString);
        connection.Open();

        string delete = $"DELETE FROM {tableName};";

        MySqlCommand command = new MySqlCommand(delete, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
}