using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galexicon
{
    [Serializable]
    [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor")]
    public class DomainObject : AutoObject
    {
        public DomainObject()
        {
            Id = (uint)(new Random().NextDouble() * uint.MaxValue);
            IsDefault = false;
        }

        [ReadOnly(true)]
        public uint Id { get; set; }

        [ReadOnly(true)]
        public bool IsDefault { get; set; }
    }
}
