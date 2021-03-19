using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MemorySquared
{
    public class GameData
    {
      
        public Color UserColor;
        public Color GameColor;
        public bool? YourTurnHelper;
        public Enumerations.Theme Theme;
        public Enumerations.GameMode GameMode;
        public Leaderboard oLeaderboard;

        public GameData()
        {
            oLeaderboard = new Leaderboard();
        }

        public int HighScoreEasy
        {
            get
            {
                if (oLeaderboard.Leaders_Easy.Count() > 0)
                {
                    // look up high score:
                    Leaderboard.Leader leader = oLeaderboard.Leaders_Easy.OrderByDescending(l => l.Score).FirstOrDefault();
                    if (leader != null) return leader.Score;
                }
                return 0;
            }
        }

        public int HighScoreHard
        {
            get
            {
                if (oLeaderboard.Leaders_Hard.Count() > 0)
                {
                    // look up high score:
                    Leaderboard.Leader leader = oLeaderboard.Leaders_Hard.OrderByDescending(l => l.Score).FirstOrDefault();
                    if (leader != null) return leader.Score;
                }
                return 0;
            }
        }

               
        public void SaveGameData()
        {

            string sFile = @"MemorySquared\MemorySquaredGameData.xml";

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {

                // create the MemorySquared directory if it doesn't already exist:
                if (isf.DirectoryExists("MemorySquared") == false) isf.CreateDirectory("MemorySquared");

                // Write to the Isolated Storage
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;

                using (IsolatedStorageFileStream stream = isf.OpenFile(sFile, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, this);
                        Globals.Data = this; // update our global variable to reflect the changes
                    }
                }

            }
                        
        }

        public static GameData GetGameData()
        {

            string sFile = @"MemorySquared\MemorySquaredGameData.xml";
            
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (isf.FileExists(sFile) == false) return CreateDefaultGameData();
                                                      
                GameData data = new GameData();
                   
                using (IsolatedStorageFileStream stream = isf.OpenFile(sFile, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                    data = (GameData)serializer.Deserialize(stream);
                    stream.Close();
                }
                   
                return data;
                   
            }

        }

        private static GameData CreateDefaultGameData()
        {
            GameData data = new GameData();
            
            data.GameColor = Colors.Red;
            data.UserColor = Colors.Blue;
            data.YourTurnHelper = true;
            data.Theme = Enumerations.Theme.Dark;
            data.GameMode = Enumerations.GameMode.Unknown;
            data.oLeaderboard = new Leaderboard();
         
            data.SaveGameData();

            return data;
                       
        }

        public class Leaderboard
        {
            public Leaderboard()
            {
                Leaders_Easy = new List<Leader>();
                Leaders_Hard = new List<Leader>();
            }

            public List<Leader> Leaders_Easy = new List<Leader>();
            public List<Leader> Leaders_Hard = new List<Leader>();

            public class Leader
            {
                public string Name = "";
                public int Score = 0;
                public string TimeInMilliseconds = "";
            }
        }

    }

}
