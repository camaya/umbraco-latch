(function() {
    'use strict';

    angular
        .module('umbraco')
        .controller('Latch.Settings.ApplicationController', ApplicationController);

    ApplicationController.$inject = ['$scope', 'formHelper', 'notificationsService', 'Latch.Constants', 'Latch.Resources.Application'];

    function ApplicationController($scope, formHelper, notificationsService, latchConstants, latchApplicationResources) {
        var vm = this;

        vm.data = {};
        vm.data.applicationId = '';
        vm.data.secret = '';

        vm.page = {};
        vm.page.loaded = false;
        vm.page.saveButtonState = latchConstants.BUTTON_STATE.INIT;

        vm.save = save;

        activate();

        function activate() {
            latchApplicationResources.getApplication()
                .then(function(response) {
                    vm.data = angular.isObject(response.data) ? response.data : {};
                    vm.page.loaded = true;
                });
        }

        function save(applicationForm) {
            if (formHelper.submitForm({ scope: $scope })) {
                setSaveButtonState(latchConstants.BUTTON_STATE.BUSY);
                latchApplicationResources.addApplication(vm.data)
                    .then(function(response) {
                        if (response.data.success) {
                            notificationsService.success(response.data.message);
                            setSaveButtonState(latchConstants.BUTTON_STATE.SUCCESS);
                            applicationForm.$setPristine();
                        } else {
                            setSaveButtonState(latchConstants.BUTTON_STATE.ERROR);
                        }
                    });
            }
        }

        function setSaveButtonState(state) {
            vm.page.saveButtonState = state;
        }

    }

})();

