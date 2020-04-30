using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Uniza.CSharp.HockeyPlayers.Interfaces;

namespace Uniza.Csharp.HockeyPlayers.App
{
    class Club : IClub, IEquatable<Club>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }

        public int CompareTo([AllowNull] IClub other)
        {
            if(other == null) 
                return 1;
            
            return Name.CompareTo(other.Name);

        }

        public bool Equals([AllowNull] Club other) => this.Equals(other) ? true : false;

        public override bool Equals(object obj)
        {
            return this.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format($"{Name} ({Address}), Web: {Url}");
        }
    }
}
