using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;

namespace UmbracoLatch.Core.Data
{
    public class LatchRepository
    {

        private readonly UmbracoDatabase db;

        public LatchRepository(UmbracoDatabase db)
        {
            this.db = db;
        }

        public void Insert<T>(T record)
        {
            db.Insert(record);
        }

        public void AddApplication(LatchApplication latchApplication)
        {
            using (var scope = db.GetTransaction())
            {
                var deleteQuery = Sql.Builder
                    .Append("DELETE FROM LatchApplication");

                db.Execute(deleteQuery);
                Insert(latchApplication);

                scope.Complete();
            }
        }

        public LatchApplication GetApplication()
        {
            var query = Sql.Builder
                .Select("TOP 1 *")
                .From("LatchApplication");
            var application = db.FirstOrDefault<LatchApplication>(query);
            return application;
        }

        public void AddPairedAccount(LatchPairedAccount pairedAccount)
        {
            using(var scope = db.GetTransaction())
            {
                DeletePairedAccount();
                Insert(pairedAccount);
                scope.Complete();
            }
        }

        public LatchPairedAccount GetPairedAccount()
        {
            var query = Sql.Builder
                .Select("TOP 1 *")
                .From("LatchPairedAccount");
            var account = db.FirstOrDefault<LatchPairedAccount>(query);
            return account;
        }

        public void DeletePairedAccount()
        {
            var deleteQuery = Sql.Builder
                .Append("DELETE FROM LatchPairedAccount");
            db.Execute(deleteQuery);
        }

        public void InsertOperation(LatchOperation operation, IEnumerable<int> userIds, IEnumerable<int> nodeIds)
        {
            using (var scope = db.GetTransaction())
            {
                Insert(operation);

                if (userIds.Any())
                {
                    InsertLatchOperationUsers(userIds.ToList(), operation.Id);
                }

                if (nodeIds.Any())
                {
                    InsertLatchOperationNodes(nodeIds.ToList(), operation.Id);
                }

                scope.Complete();
            }
        }

        public void EditOperation(LatchOperation operation, IEnumerable<int> userIdsToAdd, IEnumerable<int> userIdsToRemove, IEnumerable<int> nodeIdsToAdd, IEnumerable<int> nodeIdsToRemove)
        {
            using (var scope = db.GetTransaction())
            {
                db.Update(operation);

                if (userIdsToAdd.Any())
                {
                    InsertLatchOperationUsers(userIdsToAdd.ToList(), operation.Id);
                }

                if (userIdsToRemove.Any())
                {
                    DeleteLatchOperationUsers(userIdsToRemove, operation.Id);
                }

                if (nodeIdsToAdd.Any())
                {
                    InsertLatchOperationNodes(nodeIdsToAdd.ToList(), operation.Id);
                }

                if (nodeIdsToRemove.Any())
                {
                    DeleteLatchOperationNodes(nodeIdsToRemove, operation.Id);
                }

                scope.Complete();
            }
        }

        public void DeleteOperation(LatchOperation operation)
        {
            using (var scope = db.GetTransaction())
            {
                if (!operation.ApplyToAllUsers)
                {
                    var deleteUsersQuery = Sql.Builder
                        .Append("DELETE FROM LatchOperationUser")
                        .Where("LatchOperationId = @0", operation.Id);
                    db.Execute(deleteUsersQuery);
                }

                if (!operation.ApplyToAllNodes)
                {
                    var deleteNodesQuery = Sql.Builder
                        .Append("DELETE FROM LatchOperationNode")
                        .Where("LatchOperationId = @0", operation.Id);
                    db.Execute(deleteNodesQuery);
                }

                db.Delete(operation);
                scope.Complete();
            }
        }

        public bool OperationExists(int operationId)
        {
            var exists = db.Exists<LatchOperation>(operationId);
            return exists;
        }

        public IEnumerable<LatchOperation> GetAllOperations()
        {
            var query = Sql.Builder
                .Select("*")
                .From("LatchOperation");
            var operations = db.Fetch<LatchOperation>(query);
            return operations;
        }

        public LatchOperation GetOperationById(int operationId)
        {
            var query = Sql.Builder
                .Select("*")
                .From("LatchOperation")
                .Where("Id = @0", operationId);
            var operation = db.FirstOrDefault<LatchOperation>(query);
            return operation;
        }

        public LatchOperation GetOperationByIdIncludingRelationships(int operationId)
        {
            var operation = GetOperationById(operationId);

            if (operation != null)
            {
                if (!operation.ApplyToAllUsers)
                {
                    operation.UserIds = GetOperationUserIds(operation.Id);
                }

                if (!operation.ApplyToAllNodes)
                {
                    operation.NodeIds = GetOperationNodesIds(operation.Id);
                }
            }

            return operation;
        }

        public IEnumerable<LatchOperation> GetOperationsByType(string type)
        {
            var query = Sql.Builder
                .Select("*")
                .From("LatchOperation")
                .Where("Type = @0", type);
            var operations = GetOperations(query);
            return operations;
        }

        public IEnumerable<LatchOperation> GetOperationsByTypeAndAction(string type, string action)
        {
            var query = Sql.Builder
                .Select("*")
                .From("LatchOperation")
                .Where("Type = @0", type)
                .Append("AND Action = @0", action);
            var operations = GetOperations(query);
            return operations;
        }

        private IEnumerable<LatchOperation> GetOperations(Sql query)
        {
            var operations = db.Fetch<LatchOperation>(query);

            foreach (var operation in operations)
            {
                if (!operation.ApplyToAllUsers)
                {
                    operation.UserIds = GetOperationUserIds(operation.Id);
                }

                if (!operation.ApplyToAllNodes)
                {
                    operation.NodeIds = GetOperationNodesIds(operation.Id);
                }
            }

            return operations;
        }

        private IEnumerable<int> GetOperationUserIds(int operationId)
        {
            var query = Sql.Builder
                .Select("UserId")
                .From("LatchOperationUser")
                .Where("LatchOperationId = @0", operationId);
            var userIds = db.Fetch<int>(query);
            return userIds;
        }

        private void InsertLatchOperationUsers(IList<int> userIds, int latchOperationId)
        {
            if (IsSqlServerCe())
            {
                foreach (var userId in userIds)
                {
                    Insert(new LatchOperationUser { UserId = userId, LatchOperationId = latchOperationId });
                }
            }
            else
            {
                var query = Sql.Builder
                    .Append("INSERT INTO LatchOperationUser (UserId, LatchOperationId) VALUES");

                for (var i = 0; i < userIds.Count; ++i)
                {
                    query.Append("(@0, @1)", userIds[i], latchOperationId);
                    if (i != (userIds.Count - 1))
                    {
                        query.Append(",");
                    }
                }

                db.Execute(query);
            }
        }

        private void DeleteLatchOperationUsers(IEnumerable<int> userIds, int latchOperationId)
        {
            var query = Sql.Builder
                .Append("DELETE FROM LatchOperationUser")
                .Where("LatchOperationId = @0", latchOperationId)
                .Append("AND UserId IN (@0)", userIds);
            db.Execute(query);
        }

        private IEnumerable<int> GetOperationNodesIds(int operationId)
        {
            var query = Sql.Builder
                .Select("NodeId")
                .From("LatchOperationNode")
                .Where("LatchOperationId = @0", operationId);
            var nodeIds = db.Fetch<int>(query);
            return nodeIds;
        }

        private void InsertLatchOperationNodes(IList<int> nodeIds, int latchOperationId)
        {
            if (IsSqlServerCe())
            {
                foreach (var nodeId in nodeIds)
                {
                    Insert(new LatchOperationNode { NodeId = nodeId, LatchOperationId = latchOperationId });
                }
            }
            else
            {
                var query = Sql.Builder
                    .Append("INSERT INTO LatchOperationNode (NodeId, LatchOperationId) VALUES");

                for (var i = 0; i < nodeIds.Count; ++i)
                {
                    query.Append("(@0, @1)", nodeIds[i], latchOperationId);
                    if (i != (nodeIds.Count - 1))
                    {
                        query.Append(",");
                    }
                }

                db.Execute(query);
            }
        }

        private void DeleteLatchOperationNodes(IEnumerable<int> nodeIds, int latchOperationId)
        {
            var query = Sql.Builder
                .Append("DELETE FROM LatchOperationNode")
                .Where("LatchOperationId = @0", latchOperationId)
                .Append("AND NodeId IN (@0)", nodeIds);
            db.Execute(query);
        }

        private bool IsSqlServerCe()
        {
            return db.Connection.Database.Contains(".sdf");
        }

    }
}