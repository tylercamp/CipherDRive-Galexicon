using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Galexicon
{
    /// <summary>
    /// Represents an orbiting body with some distance to the given frame
    /// </summary>
    [Serializable]
    [ExpandableObject]
    public class CelestialBody : AutoObject
    {
        public CelestialBody()
        {
            OrbitalAxis = new Coordinate();
            CenterDistance = new Distance { Unit = Distance.DistanceUnit.AstronomicalUnit, Value = 0.5 };
            Regularity = new Percent();
        }

        public CelestialBody(CelestialBody source)
        {
            OrbitalAxis = new Coordinate(source.OrbitalAxis);
            CenterDistance = new Distance(source.CenterDistance);
            Regularity = new Percent(source.Regularity);

            if (source.Frame != null)
                SetFrame(source.Frame);
            else
                SetFrame(source.FrameId);

            if (source.Body != null)
                SetBody(source.Body);
            else
                SetBody(source.BodyId);
        }

        public override string ToString()
        {
            //  Why does this not reflect in PropertyGrid?
            return String.Format("'{0}' Orbiting '{1}'", Body, Frame);
        }

        [XmlIgnore]
        public String Name
        {
            get { return this.ToString(); }
        }

        public void SetFrame(ulong frameId)
        {
            FrameId = frameId;
            Frame = null;
        }

        public void SetFrame(FreeBody frame)
        {
            FrameId = frame.Id;
            Frame = frame;
        }

        public void SetBody(ulong bodyId)
        {
            BodyId = bodyId;
            Body = null;
        }

        public void SetBody(FreeBody body)
        {
            BodyId = body.Id;
            Body = body;
        }

        public ulong FrameId;
        public ulong BodyId;
        
        [XmlIgnore]
        public FreeBody Body { get; private set; }

        //  Frames used to represent a list of objects; now for multi-body frames, make the frame objects orbit each other - cyclic celestials enable implicitly merged frames, if
        //      you orbit one you also orbit the other
        [XmlIgnore]
        public FreeBody Frame { get; private set; }

        public Coordinate OrbitalAxis { get { return GetProperty<Coordinate>(); } set { SetProperty(value); } }
        public Distance CenterDistance { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        public Percent Regularity { get { return GetProperty<Percent>(); } set { SetProperty(value); } } // Ellipticality
    }
}
