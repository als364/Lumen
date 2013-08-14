using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MyDataTypes
{
    //[XmlElement(ElementName="XnaContent")]
    public class LevelObjects
    {
        [XmlElement(ElementName = "objectInstance")]
        public ObjectInstance[] levelObjects;
    }
}
