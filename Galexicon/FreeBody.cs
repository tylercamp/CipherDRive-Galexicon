
using Galexicon.BodyTypes;
using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Galexicon
{
    [Serializable]
    [XmlInclude(typeof(Asteroid))]
    [XmlInclude(typeof(Belt))]
    [XmlInclude(typeof(BlackHole))]
    [XmlInclude(typeof(Comet))]
    [XmlInclude(typeof(MegaStructure))]
    [XmlInclude(typeof(Moon))]
    [XmlInclude(typeof(Planet))]
    [XmlInclude(typeof(Settlement))]
    [XmlInclude(typeof(Star))]
    [XmlInclude(typeof(WarpGate))]
    [ExpandableObject]
    public abstract class FreeBody : DomainObject, IComparable
    {
        public enum BodyType
        {
            Planet,
            Star,
            Asteroid,
            Belt, // arbitrarily dense, arbitrary composition (?)
            Settlement,
            Comet,
            Moon,
            BlackHole,
            MegaStructure,
            WarpGate
        }

        public FreeBody()
        {
            GalacticCoordinate = new Coordinate();
            Radiation = new Percent();
            Axis = new Coordinate();
            Notes = "";
            CelestialContexts = new List<CelestialBody>();

            Name = "Unnamed";
        }

        public FreeBody(FreeBody source)
        {
            Name = source.Name;
            Notes = source.Notes;
            GalacticCoordinate = new Coordinate(source.GalacticCoordinate);
            Radiation = new Percent(source.Radiation);
            RotationsPerYear = source.RotationsPerYear;
            Axis = new Coordinate(source.Axis);
            CelestialContexts = new List<CelestialBody>();
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Type, Name);
        }

        [Category("Properties")]
        public String Name { get { return GetProperty<String>(); } set { SetProperty(value); } }

        public String Notes { get { return GetProperty<String>(); } set { SetProperty(value); } }

        public Coordinate GalacticCoordinate { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }


        public Percent Radiation { get { return GetProperty<Percent>(); } set { SetProperty(value); } }

        //  Compared to Earth years
        [Category("Properties")]
        [Description("Rotations per Earth-year")]
        public double RotationsPerYear { get { return GetProperty<double>(); } set { SetProperty(value); } }

        /// <summary>
        /// The axis about which the body rotates (also points north)
        /// </summary>
        public Coordinate Axis { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }

        [ReadOnly(true)]
        public List<CelestialBody> CelestialContexts { get; set; }


        //  Must be >= 0
        public double ArrivalsPerYear { get { return GetProperty<double>(); } set { SetProperty(value); } }
        public double ExitsPerYear { get { return GetProperty<double>(); } set { SetProperty(value); } }

        public FreeBody Default { get; set; }


        /// <summary>
        /// A mapping of BodyType values to their C# class Types
        /// </summary>
        public static Dictionary<BodyType, Type> TypeIndex = new Dictionary<BodyType, System.Type>
        {
            { BodyType.Asteroid, typeof( BodyTypes.Asteroid ) },
            { BodyType.Belt, typeof( BodyTypes.Belt ) },
            { BodyType.Comet, typeof( BodyTypes.Comet ) },
            { BodyType.Moon, typeof( BodyTypes.Moon ) },
            { BodyType.Planet,  typeof( BodyTypes.Planet ) },
            { BodyType.Star, typeof( BodyTypes.Star ) },
            { BodyType.Settlement, typeof( BodyTypes.Settlement ) },
            { BodyType.MegaStructure, typeof( BodyTypes.MegaStructure) },
            { BodyType.BlackHole, typeof( BodyTypes.BlackHole ) },
            { BodyType.WarpGate, typeof( BodyTypes.WarpGate ) },
        };

        public BodyType Type
        {
            get
            {
                var map = TypeIndex.ToDictionary(k => k.Value, k => k.Key);
                return map[this.GetType()];
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is DomainObject))
                return 1;

            var fb = obj as DomainObject;
            return (int)(fb.Id - this.Id);
        }
    }
}
