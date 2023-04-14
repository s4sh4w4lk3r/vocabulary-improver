using MySql.Data.MySqlClient;

namespace console;

class DBProcessing
{
    #region Database initializing
    string hostname;
    string port;
    string username;
    string password;
    string db_name;
    string tableName;
    MySqlConnection connection;
    public DBProcessing(string hostname, string port, string username, string password, string db_name, string tableName)
    {
        this.hostname = hostname;
        this.port = port;
        this.username = username;
        this.password = password;
        this.db_name = db_name;
        this.tableName = tableName;
        this.connection = GetDBConnection();
    }
    MySqlConnection GetDBConnection() // Возвращает MySqlConnection объект, если подключение усешно, или исключение, если неуспешно для ctor
    {
        string connectionString = $"server={hostname};port={port};username={username};password={password};database={db_name}";
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();
            connection.Close();
        }
        catch (MySql.Data.MySqlClient.MySqlException)
        {
            throw new Exception("An error occurred while connecting to the database.");
        }
        return connection;
    }
    #endregion
    #region Working with database
    public List<Word> GetDict() // Получить список объектов Word в БД
    {
        List<Word> dict = new List<Word>();

        string select = $"SELECT * FROM `{tableName}` ORDER BY RAND();";

        connection.Open();
        MySqlCommand command = new MySqlCommand(select, connection);
        MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            string key = reader[0].ToString()!;
            string value = reader[1].ToString()!;
            byte.TryParse(reader[1].ToString(), out byte rating);
            dict.Add(new Word(key, value, rating));
        }
        reader.Close();

        connection.Close();
        return dict;
    }
    public void CreateDict() // Создать таблицу в БД
    {
        string create = $"CREATE TABLE if not exists `{tableName}` (`word1` VARCHAR(255) NOT NULL , `word2` VARCHAR(255) NOT NULL , " +
        "`rating` TINYINT UNSIGNED NOT NULL , PRIMARY KEY (`word1`)) ENGINE = InnoDB CHARSET=utf8mb4 COLLATE utf8mb4_unicode_ci;";
        connection.Open();
        MySqlCommand command = new MySqlCommand(create, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public void Add(string key, string value) // Добавить слово с ретинглом 0 в БД
    {
        string insert = $"INSERT INTO `{tableName}` (`word1`, `word2`, `rating`) VALUES ('{key}', '{value}', 0)";
        connection.Open();
        MySqlCommand command = new MySqlCommand(insert, connection);
        
        try {command.ExecuteNonQuery();} 
        catch (MySql.Data.MySqlClient.MySqlException ex) 
        {throw new Exception($"Insertion error, probably duplication in database table.\n{ex.Message.ToString()}");}

        connection.Close();
    }
    public void Add(List<Word> dict) // Добавить список объектов Word с ретинглом 0 в БД
    {
        string megaInsert = $"INSERT INTO `{tableName}` (`word1`, `word2`, `rating`) VALUES ";

        foreach (var item in dict)
        {
            megaInsert += $"('{item.Key}', '{item.Value}', 0), ";
        }

        megaInsert = megaInsert.Remove(megaInsert.Length - 2) + ";";

        connection.Open();
        MySqlCommand command = new MySqlCommand(megaInsert, connection);

        try {command.ExecuteNonQuery();} 
        catch (MySql.Data.MySqlClient.MySqlException ex) 
        {throw new Exception($"Insertion error, probably duplication in database table.\n{ex.Message.ToString()}");}
        connection.Close();
    }
    public void Remove(string key) // Удалить слово по ключу в БД
    {
        string delete = $"DELETE FROM `{tableName}` WHERE `word1`='{key}';";

        connection.Open();
        MySqlCommand command = new MySqlCommand(delete, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public void DeleteDict() // Удалить таблицу в БД
    {
        string drop = $"DROP TABLE `{tableName}`;";

        connection.Open();
        MySqlCommand command = new MySqlCommand(drop, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public void ClearDict() // Очистить таблицу в БД
    {
        string delete = $"DELETE FROM `{tableName}`;";

        connection.Open();
        MySqlCommand command = new MySqlCommand(delete, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public void ReduceRatingDB(string key) // Увелечение рейтинга в БД
    {
        connection.Open();
        string update = $"UPDATE `{tableName}` SET `rating`=rating - 1 WHERE word1 = '{key}';";
        MySqlCommand command = new MySqlCommand(update, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    public void IncreaseRatingDB(string key) // Уменьшение рейтинга в БД
    {
        connection.Open();
        string update = $"UPDATE `{tableName}` SET `rating`=rating + 1 WHERE word1 = '{key}';";
        MySqlCommand command = new MySqlCommand(update, connection);
        command.ExecuteNonQuery();
        connection.Close();
    }
    #endregion
}