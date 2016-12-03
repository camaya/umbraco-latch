(function() {
    'use strict';

    angular
        .module('umbraco.resources')
        .factory('Latch.Resources.Application', LatchApplicationResources);

    LatchApplicationResources.$inject = ['$http'];

    function LatchApplicationResources($http) {
        var API_ROOT = '/umbraco/backoffice/latch/application';

        var service = {
            addApplication: addApplication,
            getApplication: getApplication,
            pair: pair,
            getPairedAccount: getPairedAccount,
            unpair: unpair
        };
        return service;

        function addApplication(appData) {
            return $http.post(API_ROOT + '/addapplication', appData);
        }

        function getApplication() {
            return $http.get(API_ROOT + '/getapplication');
        }

        function pair(token) {
            return $http({
                url: API_ROOT + '/pair',
                method: 'POST',
                data: '=' + token,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            });
        }

        function getPairedAccount() {
            return $http.get(API_ROOT + '/getpairedaccount');
        }

        function unpair() {
            return $http.post(API_ROOT + '/unpair');
        }

    }

})();

