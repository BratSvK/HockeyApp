using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Uniza.CSharp.HockeyPlayers.Interfaces;


namespace Uniza.Csharp.HockeyPlayers.App
{

    class HockeyReport : IHockeyReport<Club, Player>
    {
        public List<Club> Clubs = new List<Club>();
        public List<Player> Players = new List<Player>();
        

        public IEnumerable<int> GetBiggestClubPlayerAges()
        {
            // najskor si najdeme kde hra najviacHracov
            var najviacHracovClub = Clubs.Max();
            var vekHracov = from hrac in Players
                            where hrac.Club.Name.Equals(najviacHracovClub.Name)
                            select (DateTime.Now.Year - hrac.YearOfBirth);
            return vekHracov;

        }

        public IEnumerable<Club> GetClubs()
        {
            return Clubs.ToList();
        }

        public IEnumerable<IGrouping<int, Player>> GetGroupedPlayersByYearOfBirth()
        {
            var hraciPodlaRokuNarodenia = Players.OrderBy(_ => _.YearOfBirth).ToList().GroupBy(_ => _.YearOfBirth);
            return hraciPodlaRokuNarodenia;
        }

        public Player GetOldestPlayer()
        {
            var oldestPlayer = Players.Min(_ => _.YearOfBirth);
            var zoznamHracovRovnakyRok = Players.Where(_ => _.YearOfBirth.Equals(oldestPlayer)).ToList();

            var najvacsieKrdId = zoznamHracovRovnakyRok.Max(_ => _.KrpId);

            Player player = null;


            if (zoznamHracovRovnakyRok.Count() == 1)
            {
                return player = zoznamHracovRovnakyRok.First();
            }
            else
            {
                return player = zoznamHracovRovnakyRok.Where(_ => _.KrpId.Equals(najvacsieKrdId)).First();
            }

            

        }

        public double GetPlayerAverageAge()
        {
            var zoznamRokov = Players.Select(_ => -(_.YearOfBirth - DateTime.Now.Year)).ToArray();
            return zoznamRokov.Average();
        }


        public IEnumerable<Player> GetPlayers()
        {
            return Players.ToList();
        }

        public IEnumerable<Player> GetPlayersByAge(int minAge, int maxAge) => Players.Where(_ => Math.Abs((_.YearOfBirth - DateTime.Now.Year)) >= minAge &&
                                                                                                 Math.Abs((_.YearOfBirth - DateTime.Now.Year)) <= maxAge);

        public IDictionary<AgeCategory, ReportResult> GetReportByAgeCategory()
        {

            IDictionary<AgeCategory, ReportResult> dict = new Dictionary<AgeCategory, ReportResult>();

            for (int i = 0; i < 4; i++)
            {
                var hracipodlaKategorie = HraciPodlaKategorie(i);
                var youngest = YoungestPlayerByKategorie(i, out hracipodlaKategorie);
                var oldest = OldestPlayerByKategorie(i, out hracipodlaKategorie);
                var report = Result(hracipodlaKategorie, youngest, oldest);

                switch (i)
                {
                    case 0:
                        dict.Add(AgeCategory.Cadet,report);
                        break;
                    case 1:
                        dict.Add(AgeCategory.Junior, report);
                        break;
                    case 2:
                        dict.Add(AgeCategory.Midgest, report);
                        break;
                    case 3:
                        dict.Add(AgeCategory.Senior, report);
                        break;
                    default:
                        throw new System.ArgumentException("Nepoznam takuto kategoriu");
                }
            }
            dict.OrderByDescending(_ => _.Key);

            return dict;

        }
        private Player OldestPlayerByKategorie(int kategoria, out List<Player> hraciTohoKlubu)
        {
            Player player = null;
            hraciTohoKlubu = HraciPodlaKategorie(kategoria);
            if (hraciTohoKlubu.Count > 0)
            {
                var oldest = hraciTohoKlubu.Min(_ => _.YearOfBirth);
                var zoznamHracovRovnakyRok = hraciTohoKlubu.Where(_ => _.YearOfBirth.Equals(oldest)).ToList();
                var najvacsieKrdId = zoznamHracovRovnakyRok.Max(_ => _.KrpId);
                player = vypisNajvacasieKRID(zoznamHracovRovnakyRok, out player, najvacsieKrdId);
            }
            return player;

        }

        private Player YoungestPlayerByKategorie(int kategoria, out List<Player> hraciTohoKlubu)
        {
            Player player = null;
            hraciTohoKlubu = HraciPodlaKategorie(kategoria);
            if (hraciTohoKlubu.Count > 0)
            {
                var youngest = hraciTohoKlubu.Max(_ => _.YearOfBirth);
                var zoznamHracovRovnakyRok = hraciTohoKlubu.Where(_ => _.YearOfBirth.Equals(youngest)).ToList();

                var najvacsieKrdId = zoznamHracovRovnakyRok.Max(_ => _.KrpId);
                player = vypisNajvacasieKRID(zoznamHracovRovnakyRok, out player, najvacsieKrdId);
            }

            return player;


        }

        private List<Player> HraciPodlaKategorie(int kategoria)
        {
            AgeCategory kategoriaEnum = AgeCategory.Cadet;
            switch (kategoria)
            {
                case 0:
                    kategoriaEnum = AgeCategory.Cadet;
                    break;
                case 1:
                    kategoriaEnum = AgeCategory.Junior;
                    break;
                case 2:
                    kategoriaEnum = AgeCategory.Midgest;
                    break;
                case 3:
                    kategoriaEnum = AgeCategory.Senior;
                    break;
                default:
                    break;
            }
            return (GetPlayers().Where(_ => _.AgeCategory.Equals(kategoriaEnum)).ToList());
        }

        public ReportResult GetReportByClub(string clubName)
        {
            var hraciTohoKlubu = Players.Where(_ => _.Club.Name.Equals(clubName)).ToList();
            var youngestPlayer = YoungestByClub(clubName, out hraciTohoKlubu);
            var oldestPlayer = OldestPlayerByClub(clubName, out hraciTohoKlubu);

            if (hraciTohoKlubu.Count > 0)
            {
                ReportResult result = Result(hraciTohoKlubu, youngestPlayer, oldestPlayer);
                return result;
            }
            else {

                ReportResult result = new ReportResult(hraciTohoKlubu.Count, 0, "not exists", "not exists", 0, 0);
                return result;
            }
                
        }

        private static ReportResult Result(List<Player> hraciTohoKlubu, Player youngestPlayer, Player oldestPlayer)
        {
            return new ReportResult(hraciTohoKlubu.Count, hraciTohoKlubu.Average(_ => -(_.YearOfBirth - DateTime.Now.Year)), youngestPlayer.FullName, oldestPlayer.FullName, (DateTime.Now.Year - youngestPlayer.YearOfBirth), (DateTime.Now.Year - oldestPlayer.YearOfBirth));
        }

        private Player YoungestByClub(string clubName, out List<Player> hraciTohoKlubu)
        {
            Player player = null;

            hraciTohoKlubu = (GetPlayers().Where(_ => _.Club.Name.Equals(clubName)).ToList());
            if (hraciTohoKlubu.Count > 0)
            {
                var youngesttPlayer = hraciTohoKlubu.Max(_ => _.YearOfBirth);
                var zoznamHracovRovnakyRok = hraciTohoKlubu.Where(_ => _.YearOfBirth.Equals(youngesttPlayer)).ToList();
                var najvacsieKrdId = zoznamHracovRovnakyRok.Max(_ => _.KrpId);
                player = vypisNajvacasieKRID(hraciTohoKlubu, out player, najvacsieKrdId);
            }
            return player;
        }

        private Player OldestPlayerByClub(string clubName, out List<Player> hraciTohoKlubu)
        {
            Player player = null;
            
            
            hraciTohoKlubu = (GetPlayers().Where(_ => _.Club.Name.Equals(clubName)).ToList());
            if (hraciTohoKlubu.Count > 0)
            {
                var oldest = hraciTohoKlubu.Min(_ => _.YearOfBirth);
                var zoznamHracovRovnakyRok = hraciTohoKlubu.Where(_ => _.YearOfBirth.Equals(oldest)).ToList();

                var najvacsieKrdId = zoznamHracovRovnakyRok.Max(_ => _.KrpId);
                player = vypisNajvacasieKRID(zoznamHracovRovnakyRok, out player, najvacsieKrdId);
            }

            return player;


        }

        private static Player vypisNajvacasieKRID(List<Player> hraciTohoKlubu, out Player player, int najvacsieKrdId)
        {
            if (hraciTohoKlubu.Count() == 1)
            {
                return player = hraciTohoKlubu.First();
            }
            else
            {
                return player = hraciTohoKlubu.Where(_ => _.KrpId.Equals(najvacsieKrdId)).First();
            }
        }

        public IEnumerable<Club> GetSortedClubs(int maxResultCount) => Clubs.OrderByDescending(GetPocetHracov).ToList().Take(maxResultCount);
        private int GetPocetHracov(Club club) => GetReportByClub(club.Name).TotalPlayerCount;

        public IEnumerable<Player> GetSortedPlayers(int maxResultCount)
        {
            var sortedPlayers = Players.OrderBy(_ => _.LastName).ThenByDescending(_ => _.FirstName).ToList().Take(maxResultCount);
            return sortedPlayers;
        }

        public Player GetYoungestPlayer()
        {

            var youngesttPlayer = Players.Max(_ => _.YearOfBirth);
            var zoznamHracovRovnakyRok = Players.Where(_ => _.YearOfBirth.Equals(youngesttPlayer)).ToList();

            var najvacsieKrdId = zoznamHracovRovnakyRok.Max(_ => _.KrpId);

            Player player = null;


            if (zoznamHracovRovnakyRok.Count() == 1)
            {
                return player = zoznamHracovRovnakyRok.First();
            }
            else
            {
                return player = zoznamHracovRovnakyRok.Where(_ => _.KrpId.Equals(najvacsieKrdId)).First();
            }



        }

        public void LoadFromCsv(string clubsCsvFile, string playersCsvFile)
        {
            StreamReader readerClub = new StreamReader(File.OpenRead(clubsCsvFile));
            StreamReader readerPlayers = new StreamReader(File.OpenRead(playersCsvFile));

            var line = readerClub.ReadLine();
            while (!readerClub.EndOfStream)
            {
                 line = readerClub.ReadLine();
                var values = line.Split(';');

                var club = new Club();
                club.Name = values[0];
                club.Address = values[1];
                club.Url = values[2];
                Clubs.Add(club);
            }

            var linePlayer = readerPlayers.ReadLine();
            while (!readerPlayers.EndOfStream)
            {
                linePlayer = readerPlayers.ReadLine();
                var valuesPlayer = linePlayer.Split(';');

                var player = new Player();
                player.LastName = valuesPlayer[0];
                player.FirstName = valuesPlayer[1];
                player.TitleBefore = valuesPlayer[2];
                player.YearOfBirth = int.Parse(valuesPlayer[3]);
                player.KrpId = int.Parse(valuesPlayer[4]);
                var club = Clubs.Where(_ => _.Name.Equals(valuesPlayer[5])).First();
                player.Club = Clubs.Where(_ => _.Name.Equals(valuesPlayer[5])).FirstOrDefault();
                VyberKategorie(valuesPlayer, player);
                Players.Add(player);

            }



        }

        private static void VyberKategorie(string[] values, Player player)
        {
            var kategoria = values[6];
            switch (kategoria)
            {
                case "Kadeti":
                    player.AgeCategory = AgeCategory.Cadet;
                    break;
                case "Juniori":
                    player.AgeCategory = AgeCategory.Junior;
                    break;
                case "Dorastenci":
                    player.AgeCategory = AgeCategory.Midgest;
                    break;
                case "Seniori":
                    player.AgeCategory = AgeCategory.Senior;
                    break;
                default:
                    throw new System.ArgumentException("Nepoznam takuto kategoriu");

            }
        }

        public void LoadFromXml(string fileName) => _ = XElement.Load(fileName);

        public void SaveClubs(string clubsCsvFile)
        {
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Nazov,Adresa,URL");
            foreach (var club in GetClubs())
            {
                csvContent.AppendLine($"{club.Name};{club.Address};{club.Url}");
            }
            File.AppendAllText(clubsCsvFile, csvContent.ToString());
            
        }

        public void SavePlayers(string playersCsvFile)
        {
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Priezvisko,Meno,TitulPred,RokNarodenia,KRP,MaterskyKlub,VekovaKategoria");
            foreach (var player in Players)
            {
                csvContent.AppendLine($"{player.LastName};{player.FirstName};{player.TitleBefore};{player.YearOfBirth};{player.KrpId};{player.Club.Name};{player.AgeCategory}");
            }
            File.AppendAllText(playersCsvFile, csvContent.ToString());
        }

        public void SaveToXml(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement name = doc.CreateElement("name");
            XmlElement Adrres = doc.CreateElement("Adress");
            XmlElement Url = doc.CreateElement("Url");

            foreach (var club in GetClubs())
            {
                name.SetAttribute(name.ToString(),club.Name);
                Adrres.SetAttribute(Adrres.ToString(),club.Address);
                Url.SetAttribute(Url.ToString(),club.Url);
                
                
            }
            
            //Save the document to a file.
            //doc.Save(fileName);
        }
    }
}
