using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniza.Csharp.HockeyPlayers.App;
using Uniza.CSharp.HockeyPlayers.Interfaces;

namespace Uniza.CSharp.HockeyPlayers.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IHockeyReport<Club, Player> report = new HockeyReport();
            report.LoadFromCsv("Zoznam-klubov.csv", "Zoznam-hracov.csv");
            PrintAllToConsole(report);
            
            const string exportXmlFile = "export.xml";
            //report.SaveToXml(exportXmlFile);

            IHockeyReport<Club, Player> report2 = new HockeyReport();
            report2.LoadFromXml(exportXmlFile);
            Console.WriteLine("report == report2: " + HockeyReportEquals(report, report2));

            const string exportedClubCsvFile = "exported-clubs.csv";
            const string exportedPlayersCsvFile = "exported-players.csv";
            report2.SaveClubs(exportedClubCsvFile);
            report2.SavePlayers(exportedPlayersCsvFile);

            IHockeyReport<Club, Player> report3 = new HockeyReport();
            report3.LoadFromCsv(exportedClubCsvFile, exportedPlayersCsvFile);
            Console.WriteLine("report == report3: " + HockeyReportEquals(report, report3));
        }

        /// <summary>
        /// Vypise vystupy z reportu.
        /// </summary>
        /// <param name="report">Hokejovy report.</param>
        private static void PrintAllToConsole(IHockeyReport<Club, Player> report)
        {
            PrintToConsole("GetClubs():", report.GetClubs());

            PrintToConsole("GetPlayers():", report.GetPlayers());

            PrintToConsole("GetSortedClubs(2):", report.GetSortedClubs(2));
            PrintToConsole("GetSortedClubs(10000000):", report.GetSortedClubs(10000000));

            PrintToConsole("GetSortedPlayers(2):", report.GetSortedPlayers(2));
            PrintToConsole("GetSortedPlayers(11):", report.GetSortedPlayers(11));

            PrintToConsole("GetPlayerAverageAge():", report.GetPlayerAverageAge());

            PrintToConsole("GetYoungestPlayer():", report.GetYoungestPlayer());

            PrintToConsole("GetOldestPlayer():", report.GetOldestPlayer());

            PrintToConsole("GetBiggestClubPlayerAges():", report.GetBiggestClubPlayerAges());

            PrintToConsole("GetGroupedPlayersByYearOfBirth():", report.GetGroupedPlayersByYearOfBirth());

            PrintToConsole("GetPlayersByAge(1, 18):", report.GetPlayersByAge(1, 18));
            PrintToConsole("GetPlayersByAge(18, 18):", report.GetPlayersByAge(18, 18));
            PrintToConsole("GetPlayersByAge(30, 31):", report.GetPlayersByAge(30, 31));
            PrintToConsole("GetPlayersByAge(int.MinValue, int.MaxValue):",
                report.GetPlayersByAge(int.MinValue, int.MaxValue));

            PrintToConsole("GetReportByClub(\"MsHK Zilina, a.s.\"):", report.GetReportByClub("MsHK Žilina, a.s."));
            PrintToConsole("GetReportByClub(\"HC SLOVAN Bratislava, a.s.\"):",
                report.GetReportByClub("HC SLOVAN Bratislava, a.s."));
            PrintToConsole("GetReportByClub(\"MHC Martin, a.s.\"):", report.GetReportByClub("MHC Martin, a.s."));

            PrintToConsole("GetReportByAgeCategory():", report.GetReportByAgeCategory());
        }

        /// <summary>
        /// Porovna dva hokejove reporty.
        /// </summary>
        /// <param name="report1">Prvy hokejovy report.</param>
        /// <param name="report2">Druhy hokejovy report.</param>
        /// <returns>True, ak obsahuju obidva reporty rovnake udaje. Inak vrati False.</returns>
        private static bool HockeyReportEquals(IHockeyReport<Club, Player> report1, IHockeyReport<Club, Player> report2)
        {
            bool result =
                report1.GetClubs().OrderBy(c => c).SequenceEqual(report2.GetClubs().OrderBy(c => c)) &&
                report1.GetPlayers().OrderBy(p => p).SequenceEqual(report2.GetPlayers().OrderBy(p => p));

            if (result)
            {
                foreach (var club in report1.GetClubs())
                {
                    result = Comparer<ReportResult>.Default.Compare(report1.GetReportByClub(club.Name), report2.GetReportByClub(club.Name)) == 0;
                    if (!result)
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Vlastna pomocna metoda na vypisanie na konzolu.
        /// </summary>
        /// <typeparam name="T">Typ zdroja, ktory sa vypisuje na obrazovku.</typeparam>
        /// <param name="description">Popis zobrazeny pred vypisanim zdroja.</param>
        /// <param name="source">Zdroj - enumerovatelna kolekcia alebo lubovolny objekt.</param>
        /// <param name="newLine">Urcuje, ci sa ma pridat prazdny riadok.</param>
        private static void PrintToConsole<T>(string description, T source, bool newLine = true)
        {
            // Vypiseme popis toho, co ideme dalej vypisovat
            Console.Write(description);

            // Ak je zdrojom enumerovatelna kolekcia (napr. pole, zoznam atd.), potom ju mozeme foreachom vypisat
            if (source is IEnumerable enumerable)
            {
                Console.WriteLine($" [count: {enumerable.Cast<object>().Count()}]");

                // Ak sa jedna o IEnumerable<IGrouping<,>>
                if (enumerable.GetType()
                    .GetInterfaces()
                    .Any(interfaceType => interfaceType.IsGenericType &&
                                          interfaceType.GenericTypeArguments.Any(t => t.IsGenericType &&
                                                                                      t.GetGenericTypeDefinition() == typeof(IGrouping<,>))))
                {
                    dynamic enumerableGrouping = enumerable;
                   
                    foreach (var grouping in enumerableGrouping)
                    {
                        // Vypise sa hodnota kluca a vsetky enumerovatelne polozky tohto kluca
                        Console.WriteLine($"  {grouping.Key}: [count: {Enumerable.Count(grouping)}]");
                        foreach (var item in grouping)
                        {
                            Console.WriteLine($"    {item}");
                        }
                    }
                }
                else
                {
                    // Vypise sa enumerovatelna kolekcia
                    foreach (var item in enumerable)
                    {
                        Console.WriteLine($"  {item}");
                    }
                }
            }
            else
            {
                Console.WriteLine();
                
                // V pripade neenumerovatelnych kolekcii (napr. cislo, retazec), sa vypise objekt pomocou metody ToString()
                Console.WriteLine($"  {source}");
            }

            // Prida novy prazdny riadok (pre prehladne oddelenie od ostatnych vypisov)
            if (newLine)
                Console.WriteLine();
        }
    }
}
