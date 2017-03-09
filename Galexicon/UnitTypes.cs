using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Galexicon
{
    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Distance : AutoObject, ICloneable
    {
        public Distance()
        {

        }

        public Distance(Distance source)
        {
            Value = source.Value;
            Unit = source.Unit;
        }

        public override string ToString()
        {
            String label = null;
            switch (Unit)
            {
                case DistanceUnit.AstronomicalUnit:
                    label = "AU";
                    break;

                case DistanceUnit.Kilometer:
                    label = "km";
                    break;

                case DistanceUnit.LightSecond:
                    label = "Ls";
                    break;

                case DistanceUnit.LightYear:
                    label = "Ly";
                    break;

                case DistanceUnit.SolarRadius:
                    label = "SR";
                    break;
            }

            return String.Format("{0:0.##}{1}", Value, label);
        }

        public double AsMeters
        {
            get
            {
                switch (Unit)
                {
                    case DistanceUnit.Kilometer: return Value * 1000;
                    case DistanceUnit.LightSecond: return Value * 2.998e8;
                    case DistanceUnit.LightYear: return Value * 9.4607e15;
                    case DistanceUnit.AstronomicalUnit: return Value * 149597870700;
                    case DistanceUnit.SolarRadius: return Value * 695500 * 1000;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public object Clone()
        {
            return new Distance(this);
        }

        public enum DistanceUnit
        {
            Kilometer,
            AstronomicalUnit,
            LightYear,
            LightSecond,
            SolarRadius
        }

        public double Value { get { return GetProperty<double>(); } set { SetProperty(value); } }
        public DistanceUnit Unit { get { return GetProperty<DistanceUnit>(); } set { SetProperty(value); } }
    }

    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Temperature : AutoObject, ICloneable
    {
        public Temperature()
        {

        }

        public Temperature(Temperature source)
        {
            Value = source.Value;
        }

        public override string ToString()
        {
            return String.Format("{0:0.#}K", Value);
        }

        public object Clone()
        {
            return new Temperature(this);
        }

        public double Value { get { return GetProperty<double>(); } set { SetProperty(value); } }
    }

    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Mass : AutoObject, ICloneable
    {
        public Mass()
        {

        }

        public Mass(Mass source)
        {
            Value = source.Value;
            Unit = source.Unit;
        }

        public enum MassUnit
        {
            Kilograms,
            SolarMasses
        }

        public override string ToString()
        {
            String label = null;
            switch (Unit)
            {
                case MassUnit.Kilograms:
                    label = "kg";
                    break;

                case MassUnit.SolarMasses:
                    label = "SM";
                    break;
            }

            return String.Format("{0:0.##}{1}", Value, label);
        }

        public object Clone()
        {
            return new Mass(this);
        }

        public double Value { get { return GetProperty<double>(); } set { SetProperty(value); } }
        public MassUnit Unit { get { return GetProperty<MassUnit>(); } set { SetProperty(value); } }
    }

    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Percent : AutoObject, ICloneable
    {
        public Percent()
        {

        }

        public Percent(Percent source)
        {
            Value = source.Value;
        }

        public override string ToString()
        {
            return String.Format("{0:0.#}%", Value);
        }

        public object Clone()
        {
            return new Percent(this);
        }

        public double Value { get { return GetProperty<double>(); } set { SetProperty(value); } }
    }

    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Coordinate : AutoObject, ICloneable
    {
        public Coordinate()
        {

        }

        public Coordinate(Coordinate source)
        {
            X = source.X;
            Y = source.Y;
            Z = source.Z;
        }

        public double X { get { return GetProperty<double>(); } set { SetProperty(value); } }
        public double Y { get { return GetProperty<double>(); } set { SetProperty(value); } }
        public double Z { get { return GetProperty<double>(); } set { SetProperty(value); } }

        public object Clone()
        {
            return new Coordinate(this);
        }

        public override string ToString()
        {
            return String.Format("{0:0.#}, {1:0.#}, {2:0.#}", X, Y, Z);
        }
    }

    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Time : AutoObject, ICloneable
    {
        public Time()
        {

        }

        public Time(Time source)
        {
            Value = source.Value;
            Unit = source.Unit;
        }

        public override string ToString()
        {
            String label = null;
            switch (Unit)
            {
                case UnitType.Years: label = "Y"; break;
                case UnitType.MillionYears: label = "MY"; break;
                case UnitType.BillionYears: label = "BY"; break;
            }
            return String.Format("{0:0.##}{1}", Value, label);
        }

        public object Clone()
        {
            return new Time(this);
        }

        public enum UnitType
        {
            Years,
            MillionYears,
            BillionYears
        }

        public double Value { get { return GetProperty<double>(); } set { SetProperty(value); } }
        public UnitType Unit { get { return GetProperty<UnitType>(); } set { SetProperty(value); } }
    }

    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class SurfaceComposition : AutoObject
    {
        public SurfaceComposition()
        {
            Presence = new Percent();
            Acidity = new Percent();
            WaterPresence = new Percent();
            Volcanic = new Percent();
            Organic = new Percent();
            Hospitable = new Percent();
            SignificantAlbedae = new List<Color>();
        }

        public SurfaceComposition(SurfaceComposition source)
        {
            Presence = new Percent(source.Presence);
            Acidity = new Percent(source.Acidity);
            WaterPresence = new Percent(source.WaterPresence);
            Volcanic = new Percent(source.Volcanic);
            Organic = new Percent(source.Organic);
            Hospitable = new Percent(source.Hospitable);

            SignificantAlbedae = source.SignificantAlbedae.ToList();
        }

        public override string ToString()
        {
            return String.Format("{0} present Surface, {1} volcanic, {2} water presence", Presence, Volcanic, WaterPresence);
        }

        public Percent Presence { get { return GetProperty<Percent>(); } set { SetProperty(value); } }

        public Percent Acidity { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent WaterPresence { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Volcanic { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Organic { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Hospitable { get { return GetProperty<Percent>(); } set { SetProperty(value); } }



        public List<Color> SignificantAlbedae { get { return GetProperty<List<Color>>(); } set { SetProperty(value); } }
    }

    [Category("Properties")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class AtmosphericComposition : AutoObject
    {
        public AtmosphericComposition()
        {
            Presence = new Percent();
            CommonThickness = new Distance();
            CommonTemperature = new Temperature();
            Ammonia = new Percent();
            Nitrogen = new Percent();
            Hydrogen = new Percent();

            SignificantAlbedae = new List<Color>();
        }

        public AtmosphericComposition(AtmosphericComposition source)
        {
            Presence = new Percent(source.Presence);
            CommonThickness = new Distance(source.CommonThickness);
            CommonPressure = source.CommonPressure;
            CommonTemperature = new Temperature(source.CommonTemperature);
            CommonDensity = source.CommonDensity;
            Ammonia = new Percent(source.Ammonia);
            Nitrogen = new Percent(source.Nitrogen);
            Hydrogen = new Percent(source.Hydrogen);

            SignificantAlbedae = source.SignificantAlbedae.ToList();
        }

        public override string ToString()
        {
            return String.Format("{0} present Atmosphere, {1} thick at {2}", Presence, CommonThickness, CommonTemperature);
        }

        public Percent Presence { get { return GetProperty<Percent>(); } set { SetProperty(value); } }

        public Distance CommonThickness { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        public double CommonPressure { get { return GetProperty<double>(); } set { SetProperty(value); } }
        public Temperature CommonTemperature { get { return GetProperty<Temperature>(); } set { SetProperty(value); } }
        public double CommonDensity { get { return GetProperty<double>(); } set { SetProperty(value); } }

        public Percent Ammonia { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Nitrogen { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Hydrogen { get { return GetProperty<Percent>(); } set { SetProperty(value); } }

        

        public List<Color> SignificantAlbedae { get { return GetProperty<List<Color>>(); } set { SetProperty(value); } }
    }

    [Category("Inhabitants")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class CivilizationClass : AutoObject
    {
        public CivilizationClass()
        {

        }

        public CivilizationClass(CivilizationClass source)
        {
            Class = source.Class;
            KindOfLife = source.KindOfLife;
        }

        public enum ClassUnit
        {
            SubType0,
            Type0,
            SubType1,
            Type1,
            SubType2,
            Type2,
            SubType3,
            Type3
        }

        public enum LifeUnit
        {
            CarbonBased,
            SiliconBased
        }

        public ClassUnit Class { get { return GetProperty<ClassUnit>(); } set { SetProperty(value); } }
        public LifeUnit KindOfLife { get { return GetProperty<LifeUnit>(); } set { SetProperty(value); } }

        public override string ToString()
        {
            return String.Format("{0} {1} Civilization", Class, KindOfLife);
        }
    }

    [Category("Inhabitants")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Culture : AutoObject
    {
        public Culture()
        {
            Passive = new Percent();
            Peaceful = new Percent();
            Authorative = new Percent();
            Hostile = new Percent();
        }

        public Culture(Culture source)
        {
            Passive = new Percent(source.Passive);
            Peaceful = new Percent(source.Peaceful);
            Authorative = new Percent(source.Authorative);
            Hostile = new Percent(source.Hostile);
        }

        public override string ToString()
        {
            return String.Format("Culture - {0} Hostile, {1} Peaceful, {2} Authoritative, {3} Passive", Hostile, Peaceful, Authorative, Passive);
        }

        public Percent Passive { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Peaceful { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Authorative { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Hostile { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
    }

    [Category("Inhabitants")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Government : AutoObject
    {
        public Government()
        {
            Democratic = new Percent();
            Dictatorship = new Percent();
            Independent = new Percent();
            Socialistic = new Percent();
            Anarchistic = new Percent();
        }

        public Government(Government source)
        {
            Democratic = new Percent(source.Democratic);
            Dictatorship = new Percent(source.Dictatorship);
            Independent = new Percent(source.Independent);
            Socialistic = new Percent(source.Socialistic);
            Anarchistic = new Percent(source.Anarchistic);
        }

        public override string ToString()
        {
            return String.Format("Government - {0} Dem., {1} Dict., {2} Ind., {3} Soc.", Democratic, Dictatorship, Independent, Socialistic);
        }

        public Percent Democratic { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Dictatorship { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Independent { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Socialistic { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Anarchistic { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
    }

    [Category("Inhabitants")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class MajorSpecies : AutoObject
    {
        public MajorSpecies()
        {

        }

        public MajorSpecies(MajorSpecies source)
        {
            Name = source.Name;
            HomePlanet = source.HomePlanet;
            Population = source.Population;
        }

        public override string ToString()
        {
            return String.Format("{0} of {1}, Population {2}", Name, HomePlanet, Population);
        }

        public String Name { get { return GetProperty<String>(); } set { SetProperty(value); } }
        public String HomePlanet { get { return GetProperty<String>(); } set { SetProperty(value); } }
        public ulong Population { get { return GetProperty<ulong>(); } set { SetProperty(value); } }
    }

    [Category("Inhabitants")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Faction : AutoObject
    {
        public Faction()
        {
            Presence = new Percent();
        }

        public Faction(Faction source)
        {
            Organization = source.Organization;
            Presence = new Percent(source.Presence);
        }

        public enum Factions
        {
            Elven_Magistrate,
            Legion_For_Humanity,
            Republic_of_United_Living_Entities,
            Denglorian_Order,
            Galbuan_Core,
            Zekkan_Railguard,

            //  Gangs
            The_Board,
            The_Technosis,
            The_Xecsar_Claw,
            The_Noglioni_Family,
            The_Big_Red_Racers,
            The_Space_Pirates
        }

        public override string ToString()
        {
            return String.Format("{0}, {1} present", Organization, Presence);
        }

        public Percent Presence { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Factions Organization { get { return GetProperty<Factions>(); } set { SetProperty(value); } }
    }

    public enum CommodityType
    {
        Miscellaneous,

        Textiles,
        Technology,
        Machinery,
        Weapons,

        Medicine,
        Foods,
        Drugs,

        IndustrialMaterials,
        Chemicals,
        Metals,
        Minerals,

        Waste
    }

    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class MajorExport : AutoObject
    {
        public MajorExport()
        {

        }

        public MajorExport(MajorExport source)
        {
            Name = source.Name;
            Type = source.Type;
            VolumePerYear = source.VolumePerYear;
        }

        public String Name { get { return GetProperty<String>(); } set { SetProperty(value); } }
        public CommodityType Type { get { return GetProperty<CommodityType>(); } set { SetProperty(value); } }

        public double VolumePerYear { get { return GetProperty<double>(); } set { SetProperty(value); } }

        public override string ToString()
        {
            return String.Format("{0} - {1}. {2}/yr", Type, Name, VolumePerYear);
        }
    }

    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class MajorImport : AutoObject
    {
        public MajorImport()
        {

        }

        public MajorImport(MajorImport source)
        {
            Name = source.Name;
            Type = source.Type;
            VolumePerYear = source.VolumePerYear;
        }

        public String Name { get { return GetProperty<String>(); } set { SetProperty(value); } }
        public CommodityType Type { get { return GetProperty<CommodityType>(); } set { SetProperty(value); } }
        public double VolumePerYear { get { return GetProperty<double>(); } set { SetProperty(value); } }

        public override string ToString()
        {
            return String.Format("{0} - {1}. {2}/yr", Type, Name, VolumePerYear);
        }
    }

    /*
    //  Simple list type to enable editing of lists in Xeceed PropertyGrid
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct SimpleList<T> where T : struct
    {
        uint _Count;
        [NotifyParentProperty(true)]
        public uint Count
        {
            get { return _Count; }
            set
            {
                if (value > 9)
                    return;

                _Count = value;

                if (_Count >= 1 && !Value_1.HasValue) Value_1 = new T();
                if (_Count >= 2 && !Value_2.HasValue) Value_2 = new T();
                if (_Count >= 3 && !Value_3.HasValue) Value_3 = new T();
                if (_Count >= 4 && !Value_4.HasValue) Value_4 = new T();
                if (_Count >= 5 && !Value_5.HasValue) Value_5 = new T();
                if (_Count >= 6 && !Value_6.HasValue) Value_6 = new T();
                if (_Count >= 7 && !Value_7.HasValue) Value_7 = new T();
                if (_Count >= 8 && !Value_8.HasValue) Value_8 = new T();
                if (_Count >= 9 && !Value_9.HasValue) Value_9 = new T();

                if (_Count < 1) Value_1 = null;
                if (_Count < 2) Value_2 = null;
                if (_Count < 3) Value_3 = null;
                if (_Count < 4) Value_4 = null;
                if (_Count < 5) Value_5 = null;
                if (_Count < 6) Value_6 = null;
                if (_Count < 7) Value_7 = null;
                if (_Count < 8) Value_8 = null;
                if (_Count < 9) Value_9 = null;
            }
        }

        [NotifyParentProperty(true)]
        public T? Value_1 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        [NotifyParentProperty(true)]
        public T? Value_2 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        [NotifyParentProperty(true)]
        public T? Value_3 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        [NotifyParentProperty(true)]
        public T? Value_4 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        public T? Value_5 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        public T? Value_6 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        public T? Value_7 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        public T? Value_8 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        public T? Value_9 { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
    }
    */

    [Category("Inhabitants")]
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    [ExpandableObject]
    public class Economy : AutoObject
    {
        //  Mostly X,Y,Z, exports A,B,C (top 3 of each)
        public Economy()
        {
            Agricultural = new Percent();
            Industrial = new Percent();
            Technological = new Percent();
            Weaponry = new Percent();
            Medical = new Percent();
            Energy = new Percent();

            Exports = new List<MajorExport>();
            Imports = new List<MajorImport>();
        }

        public Economy(Economy source)
        {
            Agricultural = new Percent(source.Agricultural);
            Industrial = new Percent(source.Industrial);
            Technological = new Percent(source.Technological);
            Weaponry = new Percent(source.Weaponry);
            Medical = new Percent(source.Medical);
            Energy = new Percent(source.Energy);

            Exports = source.Exports.Select(e => new MajorExport(e)).ToList();
            Imports = source.Imports.Select(e => new MajorImport(e)).ToList();
        }

        
        public Percent Agricultural { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Industrial { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Technological { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Weaponry { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Medical { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Energy { get { return GetProperty<Percent>(); } set { SetProperty(value); } }

        public List<MajorExport> Exports { get { return GetProperty<List<MajorExport>>(); } set { SetProperty(value); } }
        public List<MajorImport> Imports { get { return GetProperty<List<MajorImport>>(); } set { SetProperty(value); } }
    }
}
