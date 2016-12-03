using System;
using System.Linq;
using System.Web;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core.Logging;

namespace UmbracoLatch.Core.PackageActions
{
    public class AddUmbracoLatchSection : IPackageAction
    {

        public string Alias()
        {
            return string.Concat(LatchConstants.PluginName, "_AddLatchSection");
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                LogHelper.Info<EditOwinStartup>("Umbraco Latch Package Acction - Adding the Latch section.");

                var result = false;
                if (xmlData.HasChildNodes)
                {
                    var applicationsConfigPath = GetApplicationsConfigPath();

                    // Open applications.config file
                    var applicationsConfigFile = Umbraco.Core.XmlHelper.OpenAsXmlDocument(applicationsConfigPath);

                    // Select applications node in the config file
                    var applicationsNode = applicationsConfigFile.SelectSingleNode("//applications");

                    // Get existing sections max order
                    var maxOrder = applicationsConfigFile.SelectNodes("//applications/add")
                        .Cast<XmlElement>()
                        .Max(x => int.Parse(x.Attributes["sortOrder"].Value));

                    // Select the section from the supplied xmlData
                    var latchSectionNode = xmlData.SelectSingleNode("./add");

                    // Set new section order
                    var orderAttribute = xmlData.OwnerDocument.CreateAttribute("sortOrder");
                    orderAttribute.Value = (maxOrder + 1).ToString();
                    latchSectionNode.Attributes.Append(orderAttribute);

                    // Add the new section
                    var newSectionNode = applicationsNode.OwnerDocument.ImportNode(latchSectionNode, true);
                    applicationsNode.AppendChild(newSectionNode);

                    // Save the config file
                    applicationsConfigFile.Save(HttpContext.Current.Server.MapPath(applicationsConfigPath));
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                var message = string.Format("UmbracoLatch Package Action - Error at install {0} package action: {1}", Alias(), ex);
                LogHelper.Error(typeof(AddUmbracoLatchSection), message, ex);

            }
            return false;
        }

        public XmlNode SampleXml()
        {
            var xml = string.Format("<Action runat=\"install\" undo=\"true\" alias=\"{0}\">", Alias());
            xml += string.Format("<add alias=\"{0}\" name=\"{1}\" icon=\"{2}\" />",
                LatchConstants.SectionAlias, LatchConstants.SectionName, LatchConstants.SectionIcon);
            xml += "</Action>";
            return helper.parseStringToXmlNode(xml);
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                LogHelper.Info<EditOwinStartup>("Umbraco Latch Package Acction - Removing the Latch section.");

                var result = false;
                if (xmlData.HasChildNodes)
                {
                    var applicationsConfigPath = GetApplicationsConfigPath();

                    // Open applications.config file
                    var applicationsConfigFile = Umbraco.Core.XmlHelper.OpenAsXmlDocument(applicationsConfigPath);

                    // Select applications node in the config file
                    var applicationsNode = applicationsConfigFile.SelectSingleNode("//applications");

                    // Select the section from the supplied xmlData
                    var latchSectionNode = xmlData.SelectSingleNode("./add");


                    // Get the section alias
                    var sectionAlias = latchSectionNode.Attributes["alias"].Value;

                    var existingSectionNode = applicationsNode.SelectSingleNode("//add[@alias = '" + sectionAlias + "']");
                    if (existingSectionNode != null)
                    {
                        // Section is found, remove it from the xml document
                        applicationsNode.RemoveChild(existingSectionNode);

                        // Save the modified configuration file
                        applicationsConfigFile.Save(HttpContext.Current.Server.MapPath(applicationsConfigPath));
                    }
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                var message = string.Format("UmbracoLatch Package Action - Error at undo {0} package action: {1}", Alias(), ex);
                LogHelper.Error(typeof(AddUmbracoLatchSection), message, ex);
            }
            return false;
        }

        private string GetApplicationsConfigPath()
        {
            return VirtualPathUtility.ToAbsolute("~/config/applications.config");
        }

    }
}
