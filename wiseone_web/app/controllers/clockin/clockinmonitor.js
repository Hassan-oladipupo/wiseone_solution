app.controller('ClockInMonitorCtrl', function($scope, $interval, configFileService, $timeout) {

    $scope.LatestFeeds = [];
    $scope.Locations = [];
    $scope.FeedLocation = "";
    $scope.promise = null;
    $scope.FeedExists = false;
    $scope.message = "Processing...";
    $scope.Processed = false;

    $scope.init = function() {

        $scope.disabled = true;
        configFileService
            .get(configFileService.apiHandler.retrieveActiveLocations)
            .success(function(data, status, headers) {
                $scope.Locations = data.data;
                $scope.disabled = !$scope.disabled;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Clock In Monitor', data);
            });
    };

    $scope.getFeeds = function() {

        if ($scope.promise) {
            $interval.cancel($scope.promise);
        }

        feeds();
        $scope.promise = $interval(feedsUpdate, 60000);

    };

    var feeds = function() {

        $scope.Processed = true;
        $scope.message = "Processing...";
        $scope.FeedExists = false;

        configFileService
            .get(configFileService.apiHandler.retrieveShiftLatestFeeds + '?locationID=' + $scope.FeedLocation.ID)
            .success(function(data, status, headers) {
                $scope.LatestFeeds = data.data;
                if (_.size($scope.LatestFeeds) > 0) {
                    $scope.FeedExists = true;
                } else {
                    $scope.FeedExists = false;
                    $scope.message = "There are no shift feeds for the selected location.";
                }
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Clock In Monitor', data);
            });
    };

    var feedsUpdate = function() {

        configFileService
            .get(configFileService.apiHandler.retrieveShiftLatestFeeds + '?locationID=' + $scope.FeedLocation.ID)
            .success(function(data, status, headers) {
                $scope.LatestFeeds = data.data;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Clock In Monitor', data);
            });
    };

    $scope.$on('$destroy', function() {
        $interval.cancel($scope.promise);
    });

});