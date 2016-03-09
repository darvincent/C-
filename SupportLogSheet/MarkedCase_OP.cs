// case收藏 操作类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace SupportLogSheet
{
    public class MarkedCase_OP
    {
        private XElement config;
        private string path;

        public MarkedCase_OP(string path)
        {
            this.path = path;
            if (!File.Exists(path))
            {
                createXML();
            }
            config = XElement.Load(@path);
        }
        
        private void createXML()
        {
            FileStream Fs = new FileStream(path, FileMode.Create);
            StreamWriter Sw = new StreamWriter(Fs);
            Sw.Write(Config.MarkedCaseXMLContent);
            Sw.Close();
            Fs.Close();
            MessageBox.Show("No MarkedCase config file: MyMarkedCase.XML!\r\nCreated a new one.");
        }

        public void addCase(message msg)
        {
            if (!msg.getValueFromPairs("1").Equals(""))
            {
                XElement cate = addCategory(msg);
                if (cate != null)
                {
                    cate.Add(new XElement("case", msg.getValueFromPairs("1")));
                    config.Save(path);
                }
            }        
        }

        public void removeCase(message msg)
        {
            IEnumerable<XElement> categories = from cate in config.Elements("category")
                                               where cate.Attribute("name").Value == msg.getValueFromPairs("210")
                                               select cate;
            if (categories.Count() > 0)
            {
                IEnumerable<XElement> cateCase = from aCase in categories.First().Elements("case")
                                                 where aCase.Value == msg.getValueFromPairs("1")
                                                 select aCase;
                if (cateCase.Count() > 0)
                {
                    cateCase.First().Remove();
                }
                config.Save(path);
                string filePath = msg.getValueFromPairs("212");
                if (File.Exists(filePath))
                {
                    File.Decrypt(filePath);
                }
            }
        }

        public XElement addCategory(message msg)
        {
            if (!msg.getValueFromPairs("210").Equals(""))
            {
                IEnumerable<XElement> categories = from cate in config.Elements("category")
                                                   where cate.Attribute("name").Value == msg.getValueFromPairs("210")
                                                   select cate;
                if (categories.Count() != 0)
                {
                    return categories.First();
                }
                else
                {
                    XElement record =
                    new XElement("category",
                    new XAttribute("name", msg.getValueFromPairs("210")),
                    new XAttribute("description", "")
                    );
                    config.Add(record);
                    config.Save(path);
                    return record;
                }
            }
            return null;
        }

        public void removeCategory(message msg)
        {
            IEnumerable<XElement> categories = from cate in config.Elements("category")
                                               where cate.Attribute("name").Value == msg.getValueFromPairs("210")
                                               select cate;
            if (categories.Count() != 0)
            {
                categories.First().Remove();
                config.Save(path);
            }         
        }

        public void editCategory(object obj)
        {
            message msg = (message)obj;
            IEnumerable<XElement> categories = from cate in config.Elements("category")
                                               where cate.Attribute("name").Value == msg.getValueFromPairs("210")
                                               select cate;
            if (categories.Count() != 0)
            {
                categories.First().SetAttributeValue("name",msg.getValueFromPairs("211"));
                config.Save(path);
                if (System.IO.Directory.Exists("./" + msg.getValueFromPairs("210")))
                {
                    Directory.Move("./" + msg.getValueFromPairs("210"), "./" + msg.getValueFromPairs("211"));
                }
            }
        }

        public void moveCaseBetweenCates(message msg)
        {
            string caseID = msg.getValueFromPairs("1");
            string from_Cate = msg.getValueFromPairs("210");
            string to_Cate = msg.getValueFromPairs("211");
            IEnumerable<XElement> XE_fromCat = from cate in config.Elements("category")
                                               where cate.Attribute("name").Value == from_Cate
                                               select cate;
            IEnumerable<XElement> cateCase = from aCase in XE_fromCat.First().Elements("case")
                                             where aCase.Value == caseID
                                             select aCase;
            cateCase.First().Remove();

            IEnumerable<XElement> XE_ToCat = from cate in config.Elements("category")
                                               where cate.Attribute("name").Value == to_Cate
                                               select cate;
            IEnumerable<XElement> cateCase1 = from aCase in XE_ToCat.First().Elements("case")
                                             where aCase.Value == caseID
                                             select aCase;
            if (cateCase1.Count() == 0)
            {
                XE_ToCat.First().Add(new XElement("case", caseID));
            }
            config.Save(path);

            creatCategoryDirectory(to_Cate);
            if (System.IO.Directory.Exists("./" + from_Cate))
            {
                string fileName = caseID + "_CaseRemark.doc";
                if (System.IO.File.Exists(@"./" + from_Cate + "/" + fileName))
                {
                    FileInfo fi = new FileInfo(@"./" + from_Cate + "/" + fileName);
                    fi.CopyTo(@"./" + to_Cate + "/" + fileName);
                    fi.Delete();
                }
            }
        }

        public void editCateDescription(message msg)
        {
            IEnumerable<XElement> categories = from cate in config.Elements("category")
                                    where cate.Attribute("name").Value == msg.getValueFromPairs("210")
                                    select cate;
            if (categories.Count() != 0)
            {
                categories.First().SetAttributeValue("description",msg.getValueFromPairs("212"));
            }
            config.Save(path);
        }

        public string getCateDescription(string category)
        {
            IEnumerable<XElement> categories = from cate in config.Elements("category")
                                               where cate.Attribute("name").Value == category
                                               select cate;
            if (categories.Count() != 0)
            {
                return categories.First().Attribute("description").Value;
            }
            return "";
        }

        public Dictionary<string,List<string>> getCateCases()
        {
            Dictionary<string, List<string>> cateCases = new Dictionary<string, List<string>>();
            IEnumerable<XElement> categories = from cate in config.Elements("category")
                                               select cate;
            foreach (var cate in categories)
            {
                List<string> cases =  new List<string>();
                if (cate.HasElements)
                {
                    IEnumerable<XElement> temp = from caseTemp in cate.Elements("case")
                                                 select caseTemp;

                    foreach (var aCase in temp)
                    {
                        cases.Add(aCase.Value);
                    }
                }
                cateCases.Add(cate.Attribute("name").Value, cases);
            }
            return cateCases;
        }

        public void creatCategoryDirectory(string category)
        {
            try
            {
                if (!Directory.Exists("./" + category))
                {
                    Directory.CreateDirectory("./" + category);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                MessageBox.Show("create folder failed.");
            }
        }

    }
}
