using System;
using System.Configuration;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core.Logging;

namespace UmbracoLatch.Core.PackageActions
{
    public class EditOwinStartup : IPackageAction
    {

        private const string Key = "owin:appStartup";
        private const string DefaultValue = "UmbracoDefaultOwinStartup";

        public string Alias()
        {
            return string.Concat(LatchConstants.PluginName, "_EditOwinStartup");
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                LogHelper.Info<EditOwinStartup>(string.Format("Umbraco Latch Package Acction - Changing the {0} key on the web.config.", Key));
                EditAppSettingsKey(Key, "UmbracoLatchOwinStartup");
                return true;
            }
            catch (Exception ex)
            {
                var message = string.Format("UmbracoLatch Package Action - Error at install {0} package action: {1}", Alias(), ex);
                LogHelper.Error(typeof(EditOwinStartup), message, ex);
            }

            return false;
        }

        public XmlNode SampleXml()
        {
            var xml = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", Alias(), "\" />");
            return helper.parseStringToXmlNode(xml);
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                LogHelper.Info<EditOwinStartup>(string.Format("UmbracoLatch Package Action - Restoring the default value of the {0} key on the web.config.", Key));
                EditAppSettingsKey(Key, DefaultValue);
                return true;
            }
            catch(Exception ex)
            {
                var message = string.Format("UmbracoLatch Package Action - Error at undo {0} package action: {1}", Alias(), ex);
                LogHelper.Error(typeof(EditOwinStartup), message, ex);
            }

            return false;
        }

        private static void EditAppSettingsKey(string key, string value)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = (AppSettingsSection)config.GetSection("appSettings");

            appSettings.Settings.Remove(key);
            appSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
        }

    }
}
