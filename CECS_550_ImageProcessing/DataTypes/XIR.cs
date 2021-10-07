using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;

namespace MosaicMason.DataTypes
{
    class XIR : Dictionary<int, XIRImage>
    {

        private static XIR instance;

        private XIR() { }

        public static XIR Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XIR();
                }
                return instance;
            }
        }

        public int nextID = 0;

        public int[] AverageGrayDiffArray;

        public string URL{get; set;}
        public XmlDocument XML { get; private set; }

        public void SaveAs( string url ){
            XmlElement images = ((XmlElement)XML.GetElementsByTagName("images")[0]);
            XmlElement currentNode;

            if (!File.Exists(url))
            {
                BuildXIRFileShell(url);
            }
            foreach (XIRImage image in this.Values)
            {
                currentNode = XML.GetElementById(image.id.ToString().PadLeft(7, '0'));
                if (currentNode != null && currentNode.ParentNode != null)
                {
                    images.ReplaceChild(image.ToXmlElement(), currentNode);
                }
                else
                {
                    images.AppendChild(image.ToXmlElement());
                }
            }
            try
            {
                XML.Save(url);
            }
            catch (IOException)
            {
                MessageBox.Show("Error: Repository could not be saved.");
            }
        }

        public void Save()
        {
            string filename = (URL != null) ? URL : Path.GetDirectoryName(Application.ExecutablePath) + "\\Repository.xir";
            SaveAs(filename);
        }

        /// <summary>
        /// This method is used to create the shell of an XIR file where one did not exist before.
        /// </summary>
        /// <param name="URL">The location where the file is to be created.</param>
        private void BuildXIRFileShell(string URL)
        {
            StreamWriter sw = new StreamWriter(File.Create(URL));
            string minText = "<!DOCTYPE images[\n" +
                    "<!ELEMENT image ANY> \n" +
                    "<!ATTLIST image image_id ID #REQUIRED>]>\n" +
                    "<images>\n</images>";
            sw.Write(minText);
            sw.Close();
            sw.Dispose();
        }

        public void Load(string URL)
        {
            this.URL = URL;
            XML = new XmlDocument();
            try
            {
                if (!File.Exists(URL))
                {
                    BuildXIRFileShell(URL);
                }
                XML.Load(URL);

                //Finding the next empty image ID
                while (XML.GetElementById(nextID.ToString().PadLeft(7, '0')) != null)
                {
                    nextID++;
                }

                XmlNodeList imgElems = XML.GetElementsByTagName("image");
                XIRImage xirImage;
                foreach (XmlNode node in imgElems)
                {
                    xirImage = new XIRImage((XmlElement)node);
                    this.Add(xirImage.id, xirImage);
                }
                CleanRepository();
            }
            catch (XmlException)
            {
                MessageBox.Show("Error: Load failed!  Are you sure " + URL + " is a valid XIR file?");
            }
        }

        /// <summary>
        /// CleanRepository removes all information on images that no longer exist.
        /// </summary>
        /// <returns>the total number of images removed during the cleaning.</returns>
        public int CleanRepository()
        {
            List<int> pendingRemovals = new List<int>();
            int elementsRemoved = 0;
            foreach (XIRImage xirImage in this.Values)
            {
                if (!File.Exists(xirImage.address))
                {
                    try
                    {
                        pendingRemovals.Add(xirImage.id);
                       
                        elementsRemoved++;
                    }
                    catch (Exception)
                    {

                    }
                }
               
            }
            foreach (int i in pendingRemovals)
            {
                this.Remove(i);
                XML.GetElementsByTagName("images")[0].RemoveChild(XML.GetElementById(i.ToString().PadLeft(7, '0')));
            }
            return elementsRemoved;
        }

        public Bitmap GetBitmapFromImageId(int id)
        {
            XIRImage img = this[id];
            return (File.Exists(img.address)) ? new Bitmap(img.address, true) : null;
        }

        /// <summary>
        /// This gets the image element for a corresponding image address.
        /// </summary>
        /// <param name="address">The fully qualified URL of the image to be retrieved.</param>
        /// <returns>the XML image element if the image already exists or null if not.</returns>
        public XIRImage GetXIRImageByAddress(string address)
        {
            XIRImage img;
            try
            {
                foreach (int key in this.Keys)
                {
                    img = this[key];
                    if (address.ToLower().CompareTo(img.address.ToLower()) == 0)
                    {
                        return img;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return GetXIRImageByAddress(address);
            }
            return null;
        }
    }
}
