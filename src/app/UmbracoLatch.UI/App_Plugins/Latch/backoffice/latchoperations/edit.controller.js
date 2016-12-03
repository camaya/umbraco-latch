(function() {
    'use strict';

    angular
        .module('umbraco')
        .controller('Latch.Operations.EditController', LatchOperationsController);

    LatchOperationsController.$inject = [
        '$scope', '$routeParams', '$q', 'formHelper', 'localizationService',
        'notificationsService', 'navigationService', 'Latch.Constants',
        'Latch.Resources.Operation'
    ];

    function LatchOperationsController($scope, $routeParams, $q, formHelper, localizationService, notificationsService, navigationService, latchConstants, latchOperationResources) {
        var vm = this;

        vm.data = {};

        vm.data.id = undefined;
        vm.data.name = '';

        vm.data.types = [];
        vm.data.typeSelected = false;
        vm.data.selectedType = {};
        vm.data.selectedAction = {};

        vm.data.showActions = false;
        vm.data.actions = [];
        vm.data.selectedAction = {};

        vm.data.applyToAllUsers = true;
        vm.data.invalidUsers = false;

        vm.data.showNodesOptions = false;
        vm.data.applyToAllNodes = true;
        vm.data.invalidNodes = false;

        vm.page = {};
        vm.page.loaded = false;
        vm.page.saveButtonState = latchConstants.BUTTON_STATE.INIT;

        vm.configureActions = configureActions;
        vm.save = save;

        var usersPicker = {
            label: localizationService.localize('latch_operation_usersLabel'),
            description: localizationService.localize('latch_operation_usersDescription'),
            view: '/App_Plugins/Latch/backoffice/propertyeditors/userspicker.html',
            config: {
                minNumber: 1
            },
            value: []
        };
        vm.usersProperties = [usersPicker];

        var contentPicker = {
            label: localizationService.localize('latch_operation_nodesLabel'),
            description: localizationService.localize('latch_operation_nodesDescription'),
            view: 'contentpicker',
            config: {
                multiPicker: '1',
                entityType: 'Document',
                minNumber: 1,
                maxNumber: 0
            },
            value: ''
        };
        vm.contentProperties = [contentPicker];

        activate();

        function activate() {
            loadOperationTypes();

            if (!$routeParams.create) {
                latchOperationResources.getOperation($routeParams.id).then(loadOperationData);
            } else {
                vm.page.loaded = true;
            }

        }

        function loadOperationData(response) {
            var operation = response.data;
            vm.data.id = operation.id;
            vm.data.name = operation.name;
            vm.data.selectedType = vm.data.types.filter(function(type) {
                return type.id === operation.type;
            })[0];

            if (operation.action) {
                vm.data.selectedAction = vm.data.selectedType.actions.filter(function(action) {
                    return action.id === operation.action;
                })[0];
            }

            vm.data.applyToAllUsers = operation.applyToAllUsers;
            vm.data.applyToAllNodes = operation.applyToAllNodes;

            if (!operation.applyToAllUsers) {
                usersPicker.value = operation.users;
            }

            if (!operation.applyToAllNodes) {
                contentPicker.value = operation.nodes ? operation.nodes.join(',') : '';
            }

            configureActions();
            vm.page.loaded = true;
        }

        function configureActions() {
            if (vm.data.selectedType) {
                vm.data.actions = vm.data.selectedType.actions;
                vm.data.typeSelected = true;
                vm.data.showActions = vm.data.selectedType.actions.length > 0;
                vm.data.showNodesOptions = vm.data.selectedType.id === latchConstants.OPERATION_TYPES.CONTENT.id;
            } else {
                vm.data.actions = [];
                vm.data.typeSelected = false;
                vm.data.showActions = false;
                vm.data.showNodesOptions = false;
            }
        }

        function save(form) {
            validateForm(form);

            if (formHelper.submitForm({ scope: $scope, formCtrl: form })) {
                var userIds = usersPicker.value.map(function(user) {
                    return parseInt(user.id, 10);
                });

                var operation = {
                    name: vm.data.name,
                    type: vm.data.selectedType.id,
                    action: vm.data.selectedAction.id,
                    applyToAllUsers: vm.data.applyToAllUsers,
                    users: userIds,
                    applyToAllNodes: vm.data.applyToAllNodes,
                    nodes: contentPicker.value.length ? contentPicker.value.split(',') : []
                };

                setSaveButtonState(latchConstants.BUTTON_STATE.BUSY);

                if (vm.data.id) {
                    latchOperationResources.edit(vm.data.id, operation).then(onOperationSaved);
                } else {
                    latchOperationResources.create(operation).then(onOperationSaved);
                }
            }
        }

        function onOperationSaved(response) {
            if (response.data.success) {
                setSaveButtonState(latchConstants.BUTTON_STATE.SUCCESS);
                notificationsService.success(response.data.message);
                navigationService.syncTree({tree: 'latchOperations', path: [-1, -1], forceReload: true, activate: true});
            } else {
                setSaveButtonState(latchConstants.BUTTON_STATE.ERROR);
                notificationsService.error(response.data.message);
            }
        }

        function validateForm(form) {
            var anyErrors = false;

            var selectedTypeIsValid = angular.isObject(vm.data.selectedType) &&
                !angular.isUndefined(vm.data.selectedType.id);
            form.operationType.$setValidity('required', selectedTypeIsValid);

            var isContentOperation = vm.data.selectedType.id === latchConstants.OPERATION_TYPES.CONTENT.id;
            if (isContentOperation) {
                var selectedActionisValid = angular.isObject(vm.data.selectedAction) &&
                    !angular.isUndefined(vm.data.selectedAction.id);
                form.operationAction.$setValidity('required', selectedActionisValid);
            }

            if (!vm.data.applyToAllUsers && !usersPicker.value.length) {
                vm.data.invalidUsers = true;
            }

            if (!vm.data.applyToAllNodes && !contentPicker.value.length) {
                vm.data.invalidNodes = true;
            }
        }

        function loadOperationTypes() {
            var promises = getOperationTypesLocalizationPromises();
            $q.all(promises).then(function(values) {
                for (var key in latchConstants.OPERATION_TYPES) {
                    var currentOperation = latchConstants.OPERATION_TYPES[key];
                    var operationType = {};
                    operationType.id = currentOperation.id;
                    operationType.name = values[currentOperation.id];
                    operationType.actions = currentOperation.actions.map(function(actionId) {
                        return { id: actionId, name: values['action_' + actionId] };
                    });
                    vm.data.types.push(operationType);
                }
            });
        }

        function getOperationTypesLocalizationPromises() {
            var promises = {};
            var operation = undefined;

            for (var key in latchConstants.OPERATION_TYPES) {
                operation = latchConstants.OPERATION_TYPES[key];
                promises[operation.id] = localizationService.localize('latch_operation_' + operation.id + 'Type');
                operation.actions.forEach(function(actionId) {
                    var actionKey = 'action_' + actionId;
                    if (!promises.hasOwnProperty(actionKey)) {
                        promises[actionKey] = localizationService.localize('latch_operation_' + actionId + 'Action');
                    }
                });
            }

            return promises;
        }

        function setSaveButtonState(state) {
            vm.page.saveButtonState = state;
        }

    }

})();
