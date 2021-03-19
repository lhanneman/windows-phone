using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorySquared.Helpers
{
    public class LeaderboardHelper
    {
        /// <summary>
        /// Returns True if the passed in score is greater than any
        /// of the scores on the current leaderboard
        /// </summary>
        public static bool CanAddToLeaderboard(GameData data, int nScore)
        {

            // WHICH LIST WILL THIS SCORE BE ADDED TO?
            var leaders = (data.GameMode == Enumerations.GameMode.Easy ? data.oLeaderboard.Leaders_Easy : data.oLeaderboard.Leaders_Hard);

            // IF THERE ARE LESS THAN 10 RECORDS ON THE LEADERBOARD RIGHT NOW, WE CAN ADD THIS ONE:
            if (leaders.Count < 30) return true;

            // THERE ARE AT LEAST 30 RECORDS ALREADY, SO GET THE TOP TEN:
            List<GameData.Leaderboard.Leader> TopLeaders = leaders.OrderByDescending(l => l.Score).Take(30).ToList();

            // IF THIS SCORE IS GREATER THAN THE 30th HIGHEST SCORE, WE CAN ADD IT TO THE LIST:
            if (TopLeaders != null && nScore > TopLeaders[TopLeaders.Count() - 1].Score) return true;
            
            // DIDN'T PASS OUR CHECKS, DON'T ADD THE RECORD:
            return false;
        }

        public static int AddToLeaderboard(GameData data, string name, int score)
        {

            // WHICH LIST WILL THIS SCORE BE ADDED TO?
            var leaders = (data.GameMode == Enumerations.GameMode.Easy ? data.oLeaderboard.Leaders_Easy : data.oLeaderboard.Leaders_Hard);

            // CREATE THE NEW LEADER RECORD:
            GameData.Leaderboard.Leader leader = new GameData.Leaderboard.Leader();

            string timeInMilliseconds = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString();

            leader.Name = name;
            leader.Score = score;
            leader.TimeInMilliseconds = timeInMilliseconds;

            leaders.Add(leader);

            // AFTER WE ORDER THE LIST DESCENDING BY SCORE, THE NEW RANK WILL BE 
            // THE INDEX OF THIS LEADER RECORD + 1
            int rank = leaders.OrderByDescending(l => l.Score).ToList().IndexOf(leader) + 1;

            // OVER TIME, OUR LIST WILL GROW TO OVER 30, SO WE'LL NEED TO REMOVE SOME ITEMS:
            if (leaders.Count() > 31)
            {
                while (leaders.Count() > 31)
                {
                    GameData.Leaderboard.Leader last = leaders.OrderBy(l => l.Score).FirstOrDefault();
                    if (last != null) leaders.Remove(last);
                }
            }

            data.SaveGameData();
            return rank;
        }
    }
}
