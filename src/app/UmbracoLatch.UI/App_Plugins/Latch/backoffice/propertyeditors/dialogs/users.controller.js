(function() {
    'use strict';

    angular
        .module('umbraco')
        .controller('Latch.Dialogs.UsersController', LatchUserPickerController);

    LatchUserPickerController.$inject = ['$scope'];

    function LatchUserPickerController($scope) {
        var vm = this;

        vm.tree = {};
        vm.tree.section = 'users';
        vm.tree.alias = 'users';
        vm.tree.hideHeader = true;
        vm.tree.customParams = '';
        vm.tree.multipicker = true;
        vm.tree.handler = $({});

        vm.select = select;

        var tree = undefined;

        function treeLoadedHandler(ev, args) {
            tree = args.tree;
        }

        function nodeSelectHandler(ev, args) {
            $scope.select(args.node.id);
            args.node.selected = !args.node.selected;
        }

        function select() {
            var users = [];
            tree.root.children.forEach(function(item) {
                if ($scope.dialogData.selection.indexOf(item.id) !== -1) {
                    users.push({
                        id: item.id,
                        name: item.name
                    });
                }
            });

            $scope.submit(users);
        }

        vm.tree.handler.bind('treeLoaded', treeLoadedHandler);
        vm.tree.handler.bind('treeNodeSelect', nodeSelectHandler);

        $scope.$on('$destroy', function() {
            vm.tree.handler.unbind('treeLoaded', treeLoadedHandler);
            vm.tree.handler.unbind('treeNodeSelect', nodeSelectHandler);
        });
    }

})();
