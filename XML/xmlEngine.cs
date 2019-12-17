using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QCOH.FIREHOSE;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;


namespace QCOH
{
    public sealed  class xmlEngine
    {

        public static byte[] convertFHpeek(string raw) // Converts hex strings to byte array
        {
          
         
            string xml = "<firehose-response>" + raw + "</firehose-response>";
            var r = new List<byte[]>();
            //remove the xml declarations
            xml = xml.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", "");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            string xpath = "firehose-response";
            var nodes = xmlDoc.SelectSingleNode(xpath).ChildNodes;
          
            foreach (XmlNode dataNode in nodes)
            {
                //each childNode is a <data> respone from FH

                if (dataNode.FirstChild.Name == "log")
                {
                    var value = dataNode.FirstChild.Attributes["value"].InnerText.ToString();
                    var vb = dataNode.FirstChild.Attributes["value"].InnerText.ToString();


                    if(value.Contains("0x00") || value.Contains("0x60") || value.Contains("0x61") || value.Contains("0x01")) // probaly a more elegant way to do this
                    {
                        var x = vb.Split(' ').ToArray();
                        var _x = util.stringHexArrayToByteArray(x, x.Count());
                        r.Add(_x);
                   

                    }
                    else
                    {
                        continue;
                    }

                }
              
            }
            return r.SelectMany(a => a).ToArray();
        }
    }
}
