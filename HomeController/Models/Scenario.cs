using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HomeController
{
    public class Scenario
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlArray]
        public List<LightSwitch> SwitchStates { get; set; }

        [XmlAttribute]
        public bool PlayRadio { get; set; }

        [XmlElement]
        public PandoraRadioStation StationToPlay { get; set; }
    }
}
