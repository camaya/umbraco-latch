(function() {
    'use strict';

    angular
        .module('umbraco')
        .controller('Latch.PropertyEditors.UsersPickerController', UsersPickerController);

    UsersPickerController.$inject = ['$scope', 'dialogService'];

    function UsersPickerController($scope, dialogService) {
        var vm = this;

        $scope.model.value = $scope.model.value || [];

        vm.openDialog = openDialog;
        vm.remove = remove;

        $scope.$watch(function() {
            return $scope.model.value.length;
        }, function (newValue) {
            if ($scope.model.config && $scope.model.config.minNumber) {
                var isValid = newValue >= $scope.model.config.minNumber;
                $scope.usersPickerForm.minCount.$setValidity('minCount', isValid);
            }
        });

        function openDialog() {
            dialogService.open({
                template: '/App_Plugins/Latch/backoffice/propertyeditors/dialogs/users.html',
                callback: function(users) {
                    var existingIds = $scope.model.value.map(function(user) {
                        return user.id;
                    });

                    users.forEach(function(user) {
                        if (existingIds.indexOf(user.id) === -1) {
                            $scope.model.value.push(user);
                        }
                    });
                }
            });
        }

        function remove(index) {
            $scope.model.value.splice(index, 1);
        }

    }

})();
