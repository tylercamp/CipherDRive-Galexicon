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
    public class Settlement : FreeBody
    {
        public Settlement()
        {
            Radius = new Distance { Value = 45.0, Unit = Distance.DistanceUnit.Kilometer };
            SettlementKind = SettlementType.Orbital;

            PresentFactions = new ObservableCollection<Faction>();
            Species = new ObservableCollection<MajorSpecies>();
            Economy = new Economy();
            CommonCulture = new Culture();
            CommonGovernment = new Government();
            Civilization = new CivilizationClass();

        }

        public Settlement(Settlement source) : base(source)
        {
            Radius = new Distance(source.Radius);
            Species = new ObservableCollection<MajorSpecies>(source.Species);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            Economy = new Economy(source.Economy);
            CommonCulture = new Culture(source.CommonCulture);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
        }

        [Category("Inhabitants")]
        public ObservableCollection<MajorSpecies> Species { get { return GetProperty<ObservableCollection<MajorSpecies>>(); } set { SetProperty(value); } }

        public Economy Economy { get { return GetProperty<Economy>(); } set { SetProperty(value); } }
        

        public Culture CommonCulture { get { return GetProperty<Culture>(); } set { SetProperty(value); } }

        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }


        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }

        public enum SettlementType
        {
            TerrestrialBase,

            Orbital,
            Dock,
            Bay,
            SpaceElevator
        }

        [Category("Properties")]
        public bool Grounded
        {
            get
            {
                return SettlementKind == SettlementType.TerrestrialBase;
            }
        }

        [Category("Properties")]
        public SettlementType SettlementKind { get { return GetProperty<SettlementType>(); } set { SetProperty(value); } }
    }
}
