using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;

namespace MosaicMason.DataTypes
{
    public class XIRPixelationTechnique
    {
        public int subdivisionLevel;
        public float aspectRatio;
        public string colorArray;

        public XIRPixelationTechnique(XmlElement element)
        {
            subdivisionLevel = int.Parse(element.GetAttribute("subdivLevel"));
            aspectRatio = float.Parse(element.GetAttribute("aspectRatio"));
            colorArray = element.GetElementsByTagName("colorArray")[0].InnerText;
        }

        public XIRPixelationTechnique(int subdivisionLevel, float aspectRatio, string colorArray)
        {
            this.subdivisionLevel = subdivisionLevel;
            this.aspectRatio = aspectRatio;
            this.colorArray = colorArray;
        }

        public XmlElement toXmlElement(XmlDocument xir)
        {
            XmlElement pixelationElement = xir.CreateElement("pixelationTechnique");
            pixelationElement.SetAttribute("subdivLevel", subdivisionLevel.ToString());
            pixelationElement.SetAttribute("aspectRatio", aspectRatio.ToString());
            XmlElement colorArrayElement = xir.CreateElement("colorArray");
            colorArrayElement.InnerText = colorArray.ToString();
            pixelationElement.AppendChild(colorArrayElement);
            return pixelationElement;
        }
    }
}
