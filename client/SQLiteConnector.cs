using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Configuration;

namespace Client
{
    static class SQLiteConnector
    {
        static SQLiteConnection sqliteConnection = new SQLiteConnection(ConfigurationManager.AppSettings["connectionString"]);
        static public List<Game> SelectAll()
        {
            var games = new List<Game>();
            sqliteConnection.Open();
            using (SQLiteCommand selectGameCommand = new SQLiteCommand("SELECT * FROM \"Игры\"", sqliteConnection))
            {
                using (SQLiteDataReader reader = selectGameCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        games.Add(new Game(reader.GetInt32(0), reader.GetString(1), reader.GetDateTime(2), reader.GetInt32(3),
                            reader.GetString(4), reader.GetInt32(5), reader.GetString(6), reader.GetInt32(7), reader.GetString(8)));
                    }
                }
            }
            sqliteConnection.Close();
            return games;
        }
    }
}
