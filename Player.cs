using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Uniza.CSharp.HockeyPlayers.Interfaces;

namespace Uniza.Csharp.HockeyPlayers.App
{
    class Player : IPlayer, IEquatable<Player>
    {
        public string FirstName { get; set ; }
        public string LastName { get; set; }

        public string FullName { get; set; }

        public string TitleBefore { get; set; }
        public int YearOfBirth { get; set; }
        public int KrpId { get; set; }
        public AgeCategory? AgeCategory { get; set; }
        public IClub Club { get; set; }

        public int CompareTo([AllowNull] IPlayer other)
        {
            if (other == null)
                return 1;

            return KrpId.CompareTo(other.KrpId);
        }

        public bool Equals([AllowNull] Player other) => this.Equals(other) ? true : false;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var titulPredmenom = (TitleBefore != null) ? TitleBefore : "";
            var plneMeno = FirstName + " " + LastName;
            return string.Format($"{KrpId} {titulPredmenom} {plneMeno} ({YearOfBirth}), {AgeCategory}, {Club.Name}");
        }
    }
}
