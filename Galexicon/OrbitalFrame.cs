using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Galexicon
{
    /// <summary>
    /// Frame of a significant source of gravity creating orbits
    /// </summary>
    public class OrbitalFrame
    {
        public OrbitalFrame()
        {
            Reference = new List<FreeBody>();
            CelestialBodies = new List<CelestialBody>();
        }

        public OrbitalFrame(OrbitalFrame source)
        {
            Reference = new List<FreeBody>(source.Reference);
            CelestialBodies = new List<CelestialBody>(source.CelestialBodies);
        }

        public enum KnownFrameKind
        {
            StarSystem,
            BinaryStarSystem,
            TertiaryStarSystem,
            PlanetarySystem,
            LunarySystem,
            MegaStructureSystem,
            BlackHoleSystem,
            Unknown
        }

        public override string ToString()
        {
            if (IsStarSystem)
                return "Star System";
            if (IsBinaryStarSystem)
                return "Binary Star System";
            if (IsTertiaryStarSystem)
                return "Tertiary Star System";
            if (IsPlanetarySystem)
                return "Planetary System";
            if (IsLunarySystem)
                return "Lunary System";
            if (IsMegaStructureSystem)
                return "Megastructural System";
            if (IsBlackHoleSystem)
                return "Black Hole System";

            return String.Format("{0]-body system", Reference.Count);
        }

        private bool IsStarSystem
        {
            get { return Reference.Count == 1 && Reference.All(r => r.Type == FreeBody.BodyType.Star); }
        }

        private bool IsBinaryStarSystem
        {
            get { return Reference.Count == 2 && Reference.All(r => r.Type == FreeBody.BodyType.Star); }
        }

        private bool IsTertiaryStarSystem
        {
            get { return Reference.Count == 3 && Reference.All(r => r.Type == FreeBody.BodyType.Star); }
        }

        private bool IsPlanetarySystem
        {
            get { return Reference.Count == 1 && Reference.All(r => r.Type == FreeBody.BodyType.Planet); }
        }

        private bool IsLunarySystem
        {
            get { return Reference.Count == 1 && Reference.All(r => r.Type == FreeBody.BodyType.Moon); }
        }

        private bool IsMegaStructureSystem
        {
            get { return Reference.Count == 1 && Reference.All(r => r.Type == FreeBody.BodyType.MegaStructure); }
        }

        private bool IsBlackHoleSystem
        {
            get { return Reference.Count == 1 && Reference.All(r => r.Type == FreeBody.BodyType.BlackHole); }
        }

        public KnownFrameKind FrameKind
        {
            get
            {
                if (IsStarSystem) return KnownFrameKind.StarSystem;
                if (IsBinaryStarSystem) return KnownFrameKind.BinaryStarSystem;
                if (IsPlanetarySystem) return KnownFrameKind.PlanetarySystem;
                if (IsLunarySystem) return KnownFrameKind.LunarySystem;
                if (IsMegaStructureSystem) return KnownFrameKind.MegaStructureSystem;
                if (IsBlackHoleSystem) return KnownFrameKind.BlackHoleSystem;

                return KnownFrameKind.Unknown;
            }
        }
        
        /// <summary>
        /// The bodies generating the gravitational/orbital frame
        /// </summary>
        public List<FreeBody> Reference;

        /// <summary>
        /// The bodies orbiting this frame
        /// </summary>
        public List<CelestialBody> CelestialBodies; // Celestial with respect to this orbital frame
    }
}
