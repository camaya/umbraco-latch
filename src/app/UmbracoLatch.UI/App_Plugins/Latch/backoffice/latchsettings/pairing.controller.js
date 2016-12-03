(function() {
    'use strict';

    angular
        .module('umbraco')
        .controller('Latch.Settings.PairingController', PairingController);

    PairingController.$inject = ['$scope', 'formHelper', 'notificationsService', 'navigationService', 'Latch.Constants', 'Latch.Resources.Application'];

    function PairingController($scope, formHelper, notificationsService, navigationService, latchConstants, latchApplicationResources) {
        var vm = this;

        vm.data = {};
        vm.data.token = '';
        vm.data.accountId = '';
        vm.data.isPaired = false;

        vm.page = {};
        vm.page.loaded = false;
        vm.page.pairButtonState = latchConstants.BUTTON_STATE.INIT;
        vm.page.unpairButtonState = latchConstants.BUTTON_STATE.INIT;

        vm.loaded = false;
        vm.isPaired = false;

        vm.pair = pair;
        vm.unpair = unpair;

        activate();

        function activate() {
            getPairedAccount().then(function() {
                vm.page.loaded = true;
            });
        }

        function pair(pairForm) {
            if (formHelper.submitForm({ scope: $scope, formCtrl: pairForm })) {
                setPairButtonState(latchConstants.BUTTON_STATE.BUSY);
                latchApplicationResources.pair(vm.data.token).then(onPairedAccount, onPairedError);
                pairForm.$setPristine();
            }
        }

        function unpair(unpairForm) {
            if (formHelper.submitForm({ scope: $scope, formCtrl: unpairForm })) {
                setUnpairButtonState(latchConstants.BUTTON_STATE.BUSY);
                latchApplicationResources.unpair().then(onUnpairedAccount);
            }
        }

        function onPairedAccount(response) {
            if (response.data.success) {
                setPairButtonState(latchConstants.BUTTON_STATE.SUCCESS);
                notificationsService.success(response.data.message);
                getPairedAccount();
                navigationService.syncTree({tree: 'latchOperations', path: [-1, -1], forceReload: true, activate: true});
            } else {
                setPairButtonState(latchConstants.BUTTON_STATE.ERROR);
                notificationsService.error(response.data.message);
            }
        }

        function onPairedError(error) {
            setPairButtonState(latchConstants.BUTTON_STATE.ERROR);
        }

        function onUnpairedAccount(response) {
            if (response.data.success) {
                setUnpairButtonState(latchConstants.BUTTON_STATE.SUCCESS);
                notificationsService.success(response.data.message);
                vm.data.accountId = '';
                vm.data.isPaired = false;

                // Necessary to reset the pair form, otherwise it would
                // appear with errors.
                $scope.$broadcast('formSubmitted');
            } else {
                setUnpairButtonState(latchConstants.BUTTON_STATE.ERROR);
                notificationsService.error(response.data.message);
            }
        }

        function getPairedAccount() {
            return latchApplicationResources.getPairedAccount()
                .then(function(response) {
                    if (response.data.accountId) {
                        vm.data.accountId = response.data.accountId;
                        vm.data.isPaired = true;
                    }
                    return response;
                });
        }

        function setPairButtonState(state) {
            vm.page.pairButtonState = state;
        }

        function setUnpairButtonState(state) {
            vm.page.unpairButtonState = state;
        }

    }

})();

