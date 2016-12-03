(function() {
    'use strict';

    angular
        .module('umbraco.resources')
        .factory('Latch.Resources.Operation', LatchOperationResources);

    LatchOperationResources.$inject = ['$http'];

    function LatchOperationResources($http) {
        var API_ROOT = '/umbraco/backoffice/latch/operations';

        var service = {
            create: create,
            edit: edit,
            remove: remove,
            getOperation: getOperation
        };
        return service;

        function create(operation) {
            return $http.post(API_ROOT + '/create', operation);
        }

        function edit(id, operation) {
            return $http.post(API_ROOT + '/edit?operationId=' + id, operation);
        }

        function remove(id) {
            return $http.delete(API_ROOT + '/delete?operationId=' + id);
        }

        function getOperation(id) {
            return $http.get(API_ROOT + '/getoperation', {
                params: { operationId: id }
            });
        }
    }

})();
