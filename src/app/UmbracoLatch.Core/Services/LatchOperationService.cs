using LatchSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Data;
using UmbracoLatch.Core.Data.Extensions;
using UmbracoLatch.Core.Models;

namespace UmbracoLatch.Core.Services
{
    public class LatchOperationService : LatchService
    {

        private readonly IUserService userService;

        public LatchOperationService(UmbracoDatabase database, ILocalizedTextService textService, IUserService userService) 
            : base(database, textService)
        {
            this.userService = userService;
        }

        public void CreateDefaultOperation()
        {
            var operations = GetAllOperations();
            if (!operations.Any())
            {
                var defaultOperationName = textService.Localize("latch_operation/defaultName");
                var operationRequest = new LatchOperationRequestModel
                {
                    Name = defaultOperationName,
                    Type = LatchConstants.OperationTypes.Login,
                    ApplyToAllUsers = true
                };
                CreateOperation(operationRequest);
            }
        }

        public UmbracoLatchResponse CreateOperation(LatchOperationRequestModel operation)
        {
            if (!AccountIsPaired)
            {
                var errorMessage = GetResponseMessage("accountUnpaired");
                return new UmbracoLatchResponse(false, errorMessage);
            }

            var application = GetApplication();
            var latch = new Latch(application.ApplicationId, application.Secret);
            var response = latch.CreateOperation(application.ApplicationId, operation.Name);

            if (response.Error != null)
            {
                var errorMessage = GetResponseMessage("error" + response.Error.Code);
                return new UmbracoLatchResponse(false, errorMessage);
            }

            if (response.Data == null || !response.Data.ContainsKey("operationId"))
            {
                var errorMessage = GetResponseMessage("createError");
                return new UmbracoLatchResponse(false, errorMessage);
            }

            var latchOperation = new LatchOperation
            {
                Name = operation.Name,
                OperationId = response.Data["operationId"] as string,
                Type = operation.Type,
                Action = operation.Action,
                ApplyToAllUsers = operation.ApplyToAllUsers,
                ApplyToAllNodes = operation.ApplyToAllNodes
            };
            latchRepo.InsertOperation(latchOperation, operation.Users, operation.Nodes);

            var successMessage = GetResponseMessage("createSuccess");
            return new UmbracoLatchResponse(true, successMessage);
        }

        public IEnumerable<LatchOperation> GetAllOperations()
        {
            var operations = latchRepo.GetAllOperations();
            return operations;
        }

        public bool OperationExists(int operationId)
        {
            var exists = latchRepo.OperationExists(operationId);
            return exists;
        }

        public LatchOperationDto GetOperationById(int operationId)
        {
            var operation = latchRepo.GetOperationByIdIncludingRelationships(operationId);
            var dto = operation.ToDto();

            if (dto.Users.Any())
            {
                foreach(var user in dto.Users)
                {
                    user.Name = userService.GetUserById(user.Id).Name;
                }
            }

            return dto;
        }

        public UmbracoLatchResponse EditOperation(int operationId, LatchOperationRequestModel operation)
        {
            if (!OperationExists(operationId))
            {
                var errorMessage = GetResponseMessage("notFound");
                return new UmbracoLatchResponse(false, errorMessage);
            }

            var currentOperation = latchRepo.GetOperationByIdIncludingRelationships(operationId);

            var operationNameHasChanged = !currentOperation.Name.Equals(operation.Name, StringComparison.InvariantCultureIgnoreCase);
            if (operationNameHasChanged)
            {
                var application = GetApplication();
                var latch = new Latch(application.ApplicationId, application.Secret);
                var response = latch.UpdateOperation(currentOperation.OperationId, operation.Name);

                if (response.Error != null)
                {
                    var errorMessage = GetResponseMessage("error" + response.Error.Code);
                    return new UmbracoLatchResponse(false, errorMessage);
                }
            }

            var userIdsToAdd = new List<int>();
            var userIdsToRemove = new List<int>();
            if (!currentOperation.ApplyToAllUsers && operation.ApplyToAllUsers)
            {
                userIdsToRemove.AddRange(currentOperation.UserIds);
            }
            else if (operation.Users.Any())
            {
                if (currentOperation.UserIds == null || !currentOperation.UserIds.Any())
                {
                    userIdsToAdd.AddRange(operation.Users);
                }
                else if (!Enumerable.SequenceEqual(currentOperation.UserIds, operation.Users))
                {
                    userIdsToAdd.AddRange(operation.Users.Where(x => !currentOperation.UserIds.Contains(x)));
                    userIdsToRemove.AddRange(currentOperation.UserIds.Where(x => !operation.Users.Contains(x)));
                }
            }

            var nodeIdsToAdd = new List<int>();
            var nodeIdsToRemove = new List<int>();
            if (!currentOperation.ApplyToAllNodes && operation.ApplyToAllNodes)
            {
                nodeIdsToRemove.AddRange(currentOperation.NodeIds);
            }
            else if (operation.Nodes.Any())
            {
                if (currentOperation.NodeIds == null || !currentOperation.NodeIds.Any())
                {
                    nodeIdsToAdd.AddRange(operation.Nodes);
                }
                else if (!Enumerable.SequenceEqual(currentOperation.NodeIds, operation.Nodes))
                {
                    nodeIdsToAdd.AddRange(operation.Nodes.Where(x => !currentOperation.NodeIds.Contains(x)));
                    nodeIdsToRemove.AddRange(currentOperation.NodeIds.Where(x => !operation.Nodes.Contains(x)));
                }
            }

            currentOperation.Name = operation.Name;
            currentOperation.Type = operation.Type;
            currentOperation.Action = operation.Action;
            currentOperation.ApplyToAllUsers = operation.ApplyToAllUsers;
            currentOperation.ApplyToAllNodes = operation.ApplyToAllNodes;

            latchRepo.EditOperation(currentOperation, userIdsToAdd, userIdsToRemove, nodeIdsToAdd, nodeIdsToRemove);

            var successMessage = GetResponseMessage("editSuccess");
            return new UmbracoLatchResponse(true, successMessage);
        }

        public UmbracoLatchResponse DeleteOperation(int operationId)
        {
            if (!OperationExists(operationId))
            {
                var errorMessage = GetResponseMessage("notFound");
                return new UmbracoLatchResponse(false, errorMessage);
            }

            var currentOperation = latchRepo.GetOperationById(operationId);
            var application = GetApplication();
            var latch = new Latch(application.ApplicationId, application.Secret);
            var response = latch.RemoveOperation(currentOperation.OperationId);

            // The 301 error code means that the operation wasn't found on Latch
            // so it's probably that the user deleted the operation from the Latch app
            // and we only need to delete it from the local database.
            if (response.Error != null && response.Error.Code != 301)
            {
                var errorMessage = GetResponseMessage("error" + response.Error.Code);
                return new UmbracoLatchResponse(false, errorMessage);
            }

            latchRepo.DeleteOperation(currentOperation);

            var successMessage = GetResponseMessage("editSuccess");
            return new UmbracoLatchResponse(true, successMessage);
        }

        public IEnumerable<LatchOperationDto> GetLatches(LatchOperationType type)
        {
            var operations = GetLatches(type, LatchOperationAction.None);
            return operations;
        }

        public IEnumerable<LatchOperationDto> GetLatches(LatchOperationType type, LatchOperationAction action)
        {
            IEnumerable<LatchOperation> operations;
            if (action == LatchOperationAction.None)
            {
                operations = latchRepo.GetOperationsByType(type.ToString().ToLowerInvariant());
            }
            else
            {
                operations = latchRepo.GetOperationsByTypeAndAction(type.ToString().ToLowerInvariant(), action.ToString().ToLowerInvariant());
            }

            var operationDtos = operations.Select(x => x.ToDto());
            return operationDtos;
        }

        public bool LatchIsOpen(string operationId)
        {
            var application = GetApplication();
            var account = GetPairedAccount();
            var latch = new Latch(application.ApplicationId, application.Secret);
            var response = latch.OperationStatus(account.AccountId, operationId);

            if (response.Error != null)
            {
                return true;
            }

            var isOpen = true;
            if (response.Data.ContainsKey("operations"))
            {
                var operations = response.Data["operations"] as Dictionary<string, object>;
                if (operations.ContainsKey(operationId))
                {
                    var currentOperation = operations[operationId] as Dictionary<string, object>;
                    if (currentOperation.ContainsKey("status"))
                    {
                        var currentStatus = currentOperation["status"] as string;
                        var latchIsOpen = currentStatus.Equals("on", StringComparison.InvariantCultureIgnoreCase);
                        isOpen = latchIsOpen;
                    }
                }
            }

            return isOpen;
        }

        private string GetResponseMessage(string key)
        {
            var message = textService.Localize("latch_operation/" + key);
            return message;
        }

    }
}