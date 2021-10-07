using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MosaicMason.DataTypes
{
    public class XIRImage
    {
        public int id;
        public float aspectRatio;
        public string address;
        public string modified;
        public Dimensions dimensions;
        public List<XIRPixelationTechnique> pixTechs;

        public XIRImage(XmlElement element)
        {
            id = int.Parse(element.GetAttribute("image_id"));
            aspectRatio = float.Parse(element.GetAttribute("aspectRatio"));
            address = element.GetElementsByTagName("address")[0].InnerText;
            modified = element.GetElementsByTagName("modified")[0].InnerText;

            dimensions = new Dimensions();
            XmlElement dimElem = (XmlElement)element.GetElementsByTagName("dimensions")[0];
            dimensions.width = int.Parse(dimElem.GetAttribute("width"));
            dimensions.height = int.Parse(dimElem.GetAttribute("height"));

            pixTechs = new List<XIRPixelationTechnique>();
            XmlNodeList pixElems = element.GetElementsByTagName("pixelationTechnique");
            for (int i = 0; i < pixElems.Count; i++)
            {
                pixTechs.Add(new XIRPixelationTechnique((XmlElement)pixElems[i]));
            }
        }

        public XIRImage(int id, float aspectRatio, string address, string modified, Dimensions dimensions)
        {
            this.id = id;
            this.address = address;
            this.aspectRatio = aspectRatio;
            this.modified = modified;
            this.dimensions = dimensions;
            pixTechs = new List<XIRPixelationTechnique>();
        }

        public XmlElement ToXmlElement()
        {
            XmlDocument xir = XIR.Instance.XML;
            XmlElement imageElement = xir.CreateElement("image");
            imageElement.SetAttribute("image_id", id.ToString().PadLeft(7, '0'));
            imageElement.SetAttribute("aspectRatio", aspectRatio.ToString());
            XmlElement addressElement = xir.CreateElement("address");
            addressElement.InnerText = address;
            imageElement.AppendChild(addressElement);
            XmlElement modifiedElement = xir.CreateElement("modified");
            modifiedElement.InnerText = modified;
            imageElement.AppendChild(modifiedElement);
            XmlElement dims = xir.CreateElement("dimensions");
            dims.SetAttribute("width", dimensions.width.ToString());
            dims.SetAttribute("height", dimensions.height.ToString());
            imageElement.AppendChild(dims);
            pixTechs.RemoveAll(new Predicate<XIRPixelationTechnique>(elem => elem == null));
            foreach (XIRPixelationTechnique pixTech in pixTechs)
            {
                imageElement.AppendChild(pixTech.toXmlElement(xir));
            }
            return imageElement;
        }

        public XIRPixelationTechnique GetMatchingPixelationTechnique(float aspectRatio, float subdivisionLevel)
        {
            foreach (XIRPixelationTechnique pixTech in pixTechs)
            {
                if (pixTech != null && Math.Abs(pixTech.aspectRatio - aspectRatio) < 0.005f && pixTech.subdivisionLevel == subdivisionLevel)
                {
                    return pixTech;
                }
            }
            return null;
        }

        public bool PixelationTechniqueExists(float aspectRatio, float cellWidth)
        {
            return GetMatchingPixelationTechnique(aspectRatio, cellWidth) != null;
        }
    }
}
