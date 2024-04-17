(function () {
    'use strict';

    angular
        .module('app')
        .controller('Test', Test);

    Test.$inject = ['$location'];

    function Test($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'Test';

        activate();

        function activate() { }
    }
})();
