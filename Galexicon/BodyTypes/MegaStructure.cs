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
    public class MegaStructure : FreeBody
    {
        public MegaStructure()
        {
            Species = new ObservableCollection<MajorSpecies>();
            PresentFactions = new ObservableCollection<Faction>();
            Radius = new Distance();
            Economy = new Economy();
            CommonCulture = new Culture();
            CommonGovernment = new Government();
            Civilization = new CivilizationClass();
        }

        public MegaStructure(MegaStructure source) : base(source)
        {
            Species = new ObservableCollection<MajorSpecies>(source.Species);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            Radius = new Distance(source.Radius);
            Economy = new Economy(source.Economy);
            CommonCulture = new Culture(source.CommonCulture);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
        }

        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<MajorSpecies> Species { get { return GetProperty<ObservableCollection<MajorSpecies>>(); } set { SetProperty(value); } }

        public Economy Economy { get { return GetProperty<Economy>(); } set { SetProperty(value); } }

        public enum MegaStructureType
        {
            ArtificialDwarfPlanet,
            SuperWeapon,
            DysonSphere,
            PowerPlant,
            Factory
        }

        [Category("Properties")]
        public MegaStructureType StructureKind;

        public Culture CommonCulture { get { return GetProperty<Culture>(); } set { SetProperty(value); } }

        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }

    }
}
