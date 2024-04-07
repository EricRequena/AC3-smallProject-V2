using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    [XmlRoot("Comarques")]
    public class ComarquesXml
    {
        [XmlElement("Comarca")]
        public List<ComarcaXml> Comarques { get; set; }
    }
}
