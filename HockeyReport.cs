using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniza.CSharp.HockeyPlayers.Interfaces;

namespace Uniza.Csharp.HockeyPlayers.App
{
    class HockeyReport : IHockeyReport<Club, Player>
    {
        public IEnumerable<int> GetBiggestClubPlayerAges()
        {
            // najskor si najdeme kde hra najviacHracov
            var najvacsiKlub = 
        }

        public IEnumerable<Club> GetClubs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGrouping<int, Player>> GetGroupedPlayersByYearOfBirth()
        {
            throw new NotImplementedException();
        }

        public Player GetOldestPlayer()
        {
            throw new NotImplementedException();
        }

        public double GetPlayerAverageAge()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Player> GetPlayers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Player> GetPlayersByAge(int minAge, int maxAge)
        {
            throw new NotImplementedException();
        }

        public IDictionary<AgeCategory, ReportResult> GetReportByAgeCategory()
        {
            throw new NotImplementedException();
        }

        public ReportResult GetReportByClub(string clubName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Club> GetSortedClubs(int maxResultCount)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Player> GetSortedPlayers(int maxResultCount)
        {
            throw new NotImplementedException();
        }

        public Player GetYoungestPlayer()
        {
            throw new NotImplementedException();
        }

        public void LoadFromCsv(string clubsCsvFile, string playersCsvFile)
        {
            throw new NotImplementedException();
        }

        public void LoadFromXml(string fileName)
        {
            throw new NotImplementedException();
        }

        public void SaveClubs(string clubsCsvFile)
        {
            throw new NotImplementedException();
        }

        public void SavePlayers(string playersCsvFile)
        {
            throw new NotImplementedException();
        }

        public void SaveToXml(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
