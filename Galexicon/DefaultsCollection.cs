using Galexicon.BodyTypes;
using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Galexicon
{
    [Serializable]
    [ExpandableObject]
    public class DefaultsCollection : AutoObject
    {
        public static void SaveTo(String file, DefaultsCollection c)
        {
            using (var stream = new FileStream(file, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                XmlSerializer s = new XmlSerializer(typeof(DefaultsCollection));
                s.Serialize(writer, c);
            }
        }

        public static DefaultsCollection LoadFrom(String file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                XmlSerializer s = new XmlSerializer(typeof(DefaultsCollection));
                return (DefaultsCollection)s.Deserialize(reader);
            }
        }

        public IEnumerable<T> Defaults<T>()
        {
            var type = typeof(T);
            if (type == typeof(CelestialBody)) return DefaultCelestials.Cast<T>();
            if (type == typeof(Asteroid)) return DefaultAsteroids.Cast<T>();
            if (type == typeof(Belt)) return DefaultBelts.Cast<T>();
            if (type == typeof(BlackHole)) return DefaultBlackHoles.Cast<T>();
            if (type == typeof(Comet)) return DefaultComets.Cast<T>();
            if (type == typeof(MegaStructure)) return DefaultMegaStructures.Cast<T>();
            if (type == typeof(Moon)) return DefaultMoons.Cast<T>();
            if (type == typeof(Planet)) return DefaultPlanets.Cast<T>();
            if (type == typeof(Settlement)) return DefaultSettlements.Cast<T>();
            if (type == typeof(Star)) return DefaultStars.Cast<T>();
            if (type == typeof(WarpGate)) return DefaultWarpGates.Cast<T>();

            throw new InvalidOperationException();
        }

        public DefaultsCollection()
        {
            this.DefaultCelestials = new ObservableCollection<CelestialBody>();

            this.DefaultAsteroids = new ObservableCollection<Asteroid> { new Asteroid() };
            this.DefaultBelts = new ObservableCollection<Belt>() { new Belt() };
            this.DefaultBlackHoles = new ObservableCollection<BlackHole>() { new BlackHole() };
            this.DefaultComets = new ObservableCollection<Comet>() { new Comet() };
            this.DefaultMegaStructures = new ObservableCollection<MegaStructure>() { new MegaStructure() };
            this.DefaultMoons = new ObservableCollection<Moon>() { new Moon() };
            this.DefaultPlanets = new ObservableCollection<Planet>() { new Planet() };
            this.DefaultStars = new ObservableCollection<Star>() { new Star() };
            this.DefaultSettlements = new ObservableCollection<Settlement>() { new Settlement() };
            this.DefaultWarpGates = new ObservableCollection<WarpGate>() { new WarpGate() };
        }

        public DefaultsCollection(DefaultsCollection source)
        {
            this.DefaultAsteroids = new ObservableCollection<Asteroid>(source.DefaultAsteroids);
            this.DefaultBelts = new ObservableCollection<Belt>(source.DefaultBelts);
            this.DefaultBlackHoles = new ObservableCollection<BlackHole> (source.DefaultBlackHoles);
            this.DefaultComets = new ObservableCollection<Comet>(source.DefaultComets);
            this.DefaultMegaStructures = new ObservableCollection<MegaStructure>(source.DefaultMegaStructures);
            this.DefaultMoons = new ObservableCollection<Moon>(source.DefaultMoons);
            this.DefaultPlanets = new ObservableCollection<Planet>(source.DefaultPlanets);
            this.DefaultStars = new ObservableCollection<Star>(source.DefaultStars);
            this.DefaultSettlements = new ObservableCollection<Settlement>(source.DefaultSettlements);
            this.DefaultWarpGates = new ObservableCollection<WarpGate>(source.DefaultWarpGates);
        }


        //  Could have defaults for each category of type

        //  Celestials can have defaults made based on the conditions of the celestial (ie between 2-star ref and planet vs planet and moon)
        public ObservableCollection<CelestialBody> DefaultCelestials { get { return GetProperty<ObservableCollection<CelestialBody>>(); } set { SetProperty(value); } }

        //  Start with defaults provided by library
        public ObservableCollection<Asteroid> DefaultAsteroids { get { return GetProperty<ObservableCollection<Asteroid>>(); } set { SetProperty(value); } }
        public ObservableCollection<Belt> DefaultBelts { get { return GetProperty<ObservableCollection<Belt>>(); } set { SetProperty(value); } }
        public ObservableCollection<BlackHole> DefaultBlackHoles { get { return GetProperty<ObservableCollection<BlackHole>>(); } set { SetProperty(value); } }
        public ObservableCollection<Comet> DefaultComets { get { return GetProperty<ObservableCollection<Comet>>(); } set { SetProperty(value); } }
        public ObservableCollection<MegaStructure> DefaultMegaStructures { get { return GetProperty<ObservableCollection<MegaStructure>>(); } set { SetProperty(value); } }
        public ObservableCollection<Moon> DefaultMoons { get { return GetProperty<ObservableCollection<Moon>>(); } set { SetProperty(value); } }
        public ObservableCollection<Planet> DefaultPlanets { get { return GetProperty<ObservableCollection<Planet>>(); } set { SetProperty(value); } }
        public ObservableCollection<Star> DefaultStars { get { return GetProperty<ObservableCollection<Star>>(); } set { SetProperty(value); } }
        public ObservableCollection<Settlement> DefaultSettlements { get { return GetProperty<ObservableCollection<Settlement>>(); } set { SetProperty(value); } }
        public ObservableCollection<WarpGate> DefaultWarpGates { get { return GetProperty<ObservableCollection<WarpGate>>(); } set { SetProperty(value); } }
    }
}
