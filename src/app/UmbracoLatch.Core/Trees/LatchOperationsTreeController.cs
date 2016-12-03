using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Core;
using umbraco.BusinessLogic.Actions;
using System.Globalization;
using UmbracoLatch.Core.Services;

namespace UmbracoLatch.Core.Trees
{
    [Tree(LatchConstants.SectionAlias, LatchConstants.OperationsTreeAlias, null)]
    [PluginController(LatchConstants.SectionName)]
    public class LatchOperationsTreeController : TreeController
    {

        private readonly LatchOperationService latchOperationSvc;

        public LatchOperationsTreeController()
        {
            latchOperationSvc = new LatchOperationService(ApplicationContext.Current.DatabaseContext.Database, Services.TextService, Services.UserService);
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Constants.System.Root.ToString())
            {
                var operations = latchOperationSvc.GetAllOperations();
                foreach(var operation in operations)
                {
                    nodes.Add(CreateTreeNode(operation.Id.ToString(), id, queryStrings, operation.Name, "icon-lock", false));
                }
            }

            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var text = Services.TextService;
            var menu = new MenuItemCollection();

            if (id == Constants.System.Root.ToString())
            {
                menu.Items.Add<CreateChildEntity, ActionNew>(text.Localize("actions/" + ActionNew.Instance.Alias, CultureInfo.CurrentCulture));
                menu.Items.Add<RefreshNode, ActionRefresh>(text.Localize("actions/" + ActionRefresh.Instance.Alias, CultureInfo.CurrentCulture), true);
            }
            else
            {
                menu.Items.Add<ActionDelete>(text.Localize("actions/" + ActionDelete.Instance.Alias, CultureInfo.CurrentCulture));
            }

            return menu;
        }

    }
}