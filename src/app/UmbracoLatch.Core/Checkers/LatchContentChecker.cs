using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Extensions;
using UmbracoLatch.Core.Models;
using UmbracoLatch.Core.Services;

namespace UmbracoLatch.Core.Checkers
{
    public class LatchContentChecker : LatchChecker
    {

        public LatchContentChecker(LatchOperationService latchSvc, ILocalizedTextService textService) 
            : base(latchSvc, textService) { }

        public void LatchContentPublishing(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            if (!LatchesShouldBeChecked) return;

            var user = HttpContext.Current.GetCurrentBackofficeUser();
            var latches = latchOperationSvc.GetLatches(LatchOperationType.Content, LatchOperationAction.Publish);
            foreach(var content in e.PublishedEntities)
            {
                var latchesToApply = GetLatchesToApply(latches, content.Id, user.Id);
                var nodeIsLocked = AnyLatchIsClosed(latches);
                if (nodeIsLocked)
                {
                    var errorMessage = GetErrorMessage("contentPublish");
                    var eventMessage = new EventMessage(LatchConstants.SectionName , errorMessage, EventMessageType.Error);
                    e.CancelOperation(eventMessage);
                    break;
                }
            }
        }

        public void LatchContentUnPublishing(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            if (!LatchesShouldBeChecked) return;

            var latches = latchOperationSvc.GetLatches(LatchOperationType.Content, LatchOperationAction.Unpublish);
            var contentItems = e.PublishedEntities;
            var user = HttpContext.Current.GetCurrentBackofficeUser();

            var parentItem = contentItems.First();
            var lockedNode = GetLockedNodeRecursively(latches, contentItems, user.Id);
            if (lockedNode != null)
            {
                var errorMessage = lockedNode.Id.Equals(parentItem.Id)
                    ? GetErrorMessage("contentUnpublish")
                    : GetErrorMessage("contentUnpublishChildren");
                var eventMessage = new EventMessage(LatchConstants.SectionName, errorMessage, EventMessageType.Error);
                e.CancelOperation(eventMessage);
            }
        }

        public void LatchContentTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            if (!LatchesShouldBeChecked) return;

            var latches = latchOperationSvc.GetLatches(LatchOperationType.Content, LatchOperationAction.Delete);
            var contentItems = e.MoveInfoCollection.Select(x => x.Entity);
            var user = HttpContext.Current.GetCurrentBackofficeUser();

            var parentItem = contentItems.First();
            var lockedNode = GetLockedNodeRecursively(latches, contentItems, user.Id);
            if (lockedNode != null)
            {
                var errorMessage = lockedNode.Id.Equals(parentItem.Id)
                    ? GetErrorMessage("contentDelete")
                    : GetErrorMessage("contentDeleteChildren");
                var eventMessage = new EventMessage(LatchConstants.SectionName, errorMessage, EventMessageType.Error);
                e.CancelOperation(eventMessage);
            }
        }

        private IContent GetLockedNodeRecursively(IEnumerable<LatchOperationDto> latches, IEnumerable<IContent> nodes, int userId)
        {
            foreach(var node in nodes)
            {
                var latchesToApply = GetLatchesToApply(latches, node.Id, userId);
                var nodeIsLocked = AnyLatchIsClosed(latches);
                if (nodeIsLocked)
                {
                    return node;
                }

                if (node.Children().Any())
                {
                    var lockedNode = GetLockedNodeRecursively(latches, node.Children(), userId);
                    return lockedNode;
                }
            }

            return null;
        }

        private IEnumerable<LatchOperationDto> GetLatchesToApply(IEnumerable<LatchOperationDto> latches, int nodeId, int userId)
        {
            var latchesToApply = GetNodeLatches(nodeId, latches);

            if (latchesToApply.Any())
            {
                latchesToApply = GetLatchesApplyingToUser(latches, userId);
            }

            return latchesToApply;
        }

        private IEnumerable<LatchOperationDto> GetNodeLatches(int nodeId, IEnumerable<LatchOperationDto> latches)
        {
            if (!latches.Any())
            {
                return Enumerable.Empty<LatchOperationDto>();
            }

            var latchesToApply = new List<LatchOperationDto>();
            latchesToApply.AddRange(latches.Where(x => x.ApplyToAllNodes));
            latchesToApply.AddRange(latches.Where(x => !x.ApplyToAllNodes && x.Nodes.Any(n => n.Equals(nodeId))));

            return latchesToApply;
        }

    }
}