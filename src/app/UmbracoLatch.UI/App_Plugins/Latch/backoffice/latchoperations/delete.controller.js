(function() {
    'use strict';

    angular
        .module('umbraco')
        .controller('Latch.Operations.DeleteController', LatchOperationDeleteController);


    LatchOperationDeleteController.$inject = ['$scope', 'navigationService',
        'treeService', 'Latch.Resources.Operation'];

    function LatchOperationDeleteController($scope, navigationService, treeService, latchOperationResources) {
        var vm = this;

        vm.remove = remove;
        vm.cancel = cancel;

        function remove(id) {
            latchOperationResources.remove(id)
                .then(function(response) {
                    treeService.removeNode($scope.currentNode);
                    navigationService.hideNavigation();
                });
        }

        function cancel() {
            navigationService.hideNavigation();
        }

    }
})();
