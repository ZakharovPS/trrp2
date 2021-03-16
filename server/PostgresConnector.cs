using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Npgsql;

namespace Server
{
	static class PostgresConnector
	{
		static public NpgsqlConnection postresConnection = new NpgsqlConnection(ConfigurationManager.AppSettings["connectionString"]);
		static public bool GameCheck(int gameId)
		{
			using (var sCommand = new NpgsqlCommand("SELECT * FROM \"Игры\" where \"Код игры\" = @value", postresConnection))
			{
				sCommand.Parameters.AddWithValue("@value", gameId);
				using (NpgsqlDataReader reader = sCommand.ExecuteReader())
					return reader.Read();
			}
		}

		static public bool GanreCheck(int ganreId)
		{
			using (var sCommand = new NpgsqlCommand("SELECT * FROM \"Жанры\" where \"Код жанра\" = @value", postresConnection))
			{
				sCommand.Parameters.AddWithValue("@value", ganreId);
				using (NpgsqlDataReader reader = sCommand.ExecuteReader())
					return reader.Read();
			}
		}

		static public bool GanreAndGameCheck(int gameId, int ganreId)
		{
			using (var sCommand = new NpgsqlCommand("SELECT * FROM \"Жанры и игры\" where \"Код игры\" = @GameId AND \"Код жанра\" = @GanreId", postresConnection))
			{
				sCommand.Parameters.AddWithValue("@GameId", gameId);
				sCommand.Parameters.AddWithValue("@GanreId", ganreId);
				using (NpgsqlDataReader reader = sCommand.ExecuteReader())
					return reader.Read();
			}
		}

		static public bool DeveloperCheck(int developerId)
		{
			using (var sCommand = new NpgsqlCommand("SELECT * FROM \"Разработчики\" where \"Код разработчика\" = @value", postresConnection))
			{
				sCommand.Parameters.AddWithValue("@value", developerId);
				using (NpgsqlDataReader reader = sCommand.ExecuteReader())
					return reader.Read();
			}
		}

		static public bool PublisherCheck(int publisherId)
		{
			using (var sCommand = new NpgsqlCommand("SELECT * FROM \"Издатели\" where \"Код издателя\" = @value", postresConnection))
			{
				sCommand.Parameters.AddWithValue("@value", publisherId);
				using (NpgsqlDataReader reader = sCommand.ExecuteReader())
					return reader.Read();
			}
		}

		static public void GanreInsert(Game game)
		{
			using (NpgsqlCommand sCommand = new NpgsqlCommand("INSERT INTO \"Жанры\" (\"Код жанра\", \"Название жанра\") VALUES (@GanreId, @GanreName)", postresConnection))
			{
				sCommand.Parameters.AddWithValue("GanreId", game.genreId);
				sCommand.Parameters.AddWithValue("GanreName", game.genreName);
				sCommand.ExecuteNonQuery();
			}
		}

		static public void DeveloperInsert(Game game)
		{
			using (NpgsqlCommand sCommand = new NpgsqlCommand("INSERT INTO \"Разработчики\" (\"Код разработчика\", \"Название разработчика\") VALUES (@DeveloperId, @DeveloperName)", postresConnection))
			{
				sCommand.Parameters.AddWithValue("DeveloperId", game.developerId);
				sCommand.Parameters.AddWithValue("DeveloperName", game.developerName);
				sCommand.ExecuteNonQuery();
			}
		}

		static public void PublisherInsert(Game game)
		{
			using (NpgsqlCommand sCommand = new NpgsqlCommand("INSERT INTO \"Издатели\" (\"Код издателя\", \"Название издателя\") VALUES (@PublisherId, @PublisherName)", postresConnection))
			{
				sCommand.Parameters.AddWithValue("PublisherId", game.publisherId);
				sCommand.Parameters.AddWithValue("PublisherName", game.publisherName);
				sCommand.ExecuteNonQuery();
			}
		}

		static public void GameInsert(Game game)
		{
			using (NpgsqlCommand sCommand = new NpgsqlCommand("INSERT INTO \"Игры\" (\"Код игры\", \"Название игры\", \"Дата выпуска\", \"Код разработчика\", \"Код издателя\") VALUES (@GameId, @GameName, @GameDate, @DeveloperId, @PublisherId)", postresConnection))
			{
				sCommand.Parameters.AddWithValue("GameId", game.id);
				sCommand.Parameters.AddWithValue("GameName", game.name);
				sCommand.Parameters.AddWithValue("GameDate", game.date);
				sCommand.Parameters.AddWithValue("DeveloperId", game.developerId);
				sCommand.Parameters.AddWithValue("PublisherId", game.publisherId);
				sCommand.ExecuteNonQuery();
			}
		}

		static public void GanreAndGameInsert(Game game)
		{
			using (NpgsqlCommand sCommand = new NpgsqlCommand("INSERT INTO \"Жанры и игры\" (\"Код игры\", \"Код жанра\") VALUES (@GameId, @GanreId)", postresConnection))
			{
				sCommand.Parameters.AddWithValue("GameId", game.id);
				sCommand.Parameters.AddWithValue("GanreId", game.genreId);
				sCommand.ExecuteNonQuery();
			}
		}
	}
}
