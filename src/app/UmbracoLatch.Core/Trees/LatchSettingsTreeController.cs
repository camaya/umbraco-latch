using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;
using Umbraco.Core;
using Umbraco.Web.Mvc;
using Umbraco.Web;
using System.Collections.Generic;
using System.Globalization;

namespace UmbracoLatch.Core.Trees
{
    [Tree(LatchConstants.SectionAlias, LatchConstants.SettingsTreeAlias, null)]
    [PluginController(LatchConstants.SectionName)]
    public class LatchSettingsTreeController : TreeController
    {

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var rootNode = base.CreateRootNode(queryStrings);
            rootNode.Icon = "icon-settings";
            return rootNode;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var text = Services.TextService;
            var settingsNodes = new List<dynamic>();
            settingsNodes.Add(new { Id = "PairSettings", TitleKey = "pairing", Icon = "icon-link", RouteAction = "pairing" });
            settingsNodes.Add(new { Id = "AppSettings", TitleKey = "application", Icon = "icon-hearts", RouteAction = "application" });

            var nodes = new TreeNodeCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
                foreach(var node in settingsNodes)
                {
                    var routePath = string.Format("{0}/{1}/{2}/all", FormDataCollectionExtensions.GetValue<string>(queryStrings, "application"), TreeAlias, node.RouteAction);
                    var treeTitle = text.Localize(string.Format("latch_settingsTree/{0}", node.TitleKey), CultureInfo.CurrentCulture);
                    nodes.Add(CreateTreeNode(node.Id, id, queryStrings, treeTitle, node.Icon, false, routePath));
                }
            }

            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

    }
}