using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Galexicon.BodyTypes
{
    [Serializable]
    [XmlInclude(typeof(FreeBody))]
    public class Asteroid : FreeBody
    {
        public Asteroid()
        {
            Radius = new Distance { Value = 2.0, Unit = Distance.DistanceUnit.Kilometer };
            AsteroidKind = AsteroidType.Rocky;

            Surface = new SurfaceComposition();

            Species = new ObservableCollection<MajorSpecies>();
            Economy = new Economy();

            CommonCulture = new Culture();
            CommonGovernment = new Government();
            Civilization = new CivilizationClass();
            PresentFactions = new ObservableCollection<Faction>();
        }

        public Asteroid(Asteroid source) : base(source)
        {
            Radius = new Distance(source.Radius);
            AsteroidKind = source.AsteroidKind;
            Surface = new SurfaceComposition(source.Surface);
            Species = new ObservableCollection<MajorSpecies>(source.Species);
            Economy = new Economy(source.Economy);
            CommonCulture = new Culture(source.CommonCulture);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            ClassKind = source.ClassKind;
        }

        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }


        [Category("Inhabitants")]
        public ObservableCollection<MajorSpecies> Species { get { return GetProperty<ObservableCollection<MajorSpecies>>(); } set { SetProperty(value); } }

        public Economy Economy { get { return GetProperty<Economy>(); } set { SetProperty(value); } }



        public Culture CommonCulture { get { return GetProperty<Culture>(); } set { SetProperty(value); } }

        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }



        public SurfaceComposition Surface { get { return GetProperty<SurfaceComposition>(); } set { SetProperty(value); } }

        public enum AsteroidType
        {
            Rocky,
            Icy
        }

        public enum AsteroidClass
        {
            A, B, C, D, E
        }

        [Category("Properties")]
        public AsteroidType AsteroidKind { get { return GetProperty<AsteroidType>(); } set { SetProperty(value); } }

        public AsteroidClass ClassKind { get { return GetProperty<AsteroidClass>(); } set { SetProperty(value); } }
    }
}
