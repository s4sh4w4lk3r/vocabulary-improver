using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console
{
    internal class ReadDictionary
    {
        public static Dictionary<string, string> GetDictFromDB(string connString, string tableName)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            MySqlConnection connection = new MySqlConnection(connString);

            connection.Open();
            string sql = $"SELECT * FROM `{tableName}`;";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                dict.Add(reader[0].ToString()!, reader[1].ToString()!);
            }
            reader.Close();
            connection.Close();
            return dict;
        }
    }
}
