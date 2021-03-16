using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Game
    {
        public int id;
        public string name;
        public DateTime date;
        public int genreId;
        public string genreName;
        public int developerId;
        public string developerName;
        public int publisherId;
        public string publisherName;

        public Game(int id, string name, DateTime date, int genreId, string genreName, int developerId, string developerName, int publisherId, string publisherName)
        {
            this.id = id;
            this.name = name;
            this.date = date;
            this.genreId = genreId;
            this.genreName = genreName;
            this.developerId = developerId;
            this.developerName = developerName;
            this.publisherId = publisherId;
            this.publisherName = publisherName;
        }
    }
}
