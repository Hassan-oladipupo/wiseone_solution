'use strict';

app.controller('RoleDetailCtrl', function($sessionStorage, $scope, $window, configFileService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    var $functions = $('#functions');

    $functions
        .bind('loaded.jstree', function(e, data) {
            // invoked after jstree has loaded  
            $functions.jstree("check_node", $scope.role.Functions);
        })

    $scope.role = $sessionStorage.transferData.data;
    $scope.edit = $sessionStorage.transferData.edit;

    $scope.init = function() {

        configFileService
            .get(configFileService.apiHandler.retrieveFunctions)
            .success(function(data, status, headers) {
                $functions.jstree({
                    "checkbox": {
                        "keep_selected_style": false,
                    },
                    "types": {
                        "default": {
                            "icon": "glyphicon glyphicon-file"
                        }
                    },
                    "plugins": ["checkbox", "types"],
                    "core": {
                        "data": functionsToTreeViewList(data.data),
                    }
                });
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Role Management', data);
            });

    }

    $scope.save = function() {

        $scope.loading = true;
        $scope.role.Functions = [];

        $($functions.jstree("get_checked", true)).each(function() {
            var node = this;
            if (!_.isEqual(node.parent, "#")) {
                $scope.role.Functions.push(node.id);
            }
        });

        configFileService
            .put(configFileService.apiHandler.updateRole, $scope.role)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Role Management', data);
                $scope.loading = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Role Management', data);
                $scope.Functions = [];
                $scope.loading = false;
            });

    };

    $scope.goBack = function() {
        $window.history.back();
    }

    var functionsToTreeViewList = function(functionList) {

        var modules = [];
        var treeViewList = [];
        $.each(functionList, function(key, func) {

            if (!_.includes(modules, func.Module)) {
                modules.push(func.Module);
                //parent node
                treeViewList.push({
                    "id": func.Module.toLowerCase(),
                    "parent": "#",
                    "text": func.Module,
                    "state": {
                        "opened": false
                    },
                });

                //first child node
                treeViewList.push({
                    "id": func.ID,
                    "parent": func.Module.toLowerCase(),
                    "text": func.Name,
                    "class": "jstree-checked"
                });
            } else {
                //other child nodes
                treeViewList.push({
                    "id": func.ID,
                    "parent": func.Module.toLowerCase(),
                    "text": func.Name,
                    "class": "jstree-checked"
                });
            }
        });
        return treeViewList;

    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });
});