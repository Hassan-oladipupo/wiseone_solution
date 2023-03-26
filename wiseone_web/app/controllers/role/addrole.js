app.controller('AddRoleCtrl', function($scope, configFileService) {

    var $functions = $('#functions');

    $scope.role = new roledto();

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

        $($functions.jstree("get_checked", true)).each(function() {
            var node = this;
            if (!_.isEqual(node.parent, "#")) {
                $scope.role.Functions.push(node.id);
            }
        });

        configFileService
            .post(configFileService.apiHandler.saveRole, $scope.role)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Role Management', data);
                $scope.loading = false;
                $scope.reset();
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Role Management', data);
                $scope.Functions = [];
                $scope.loading = false;
            });

    };

    $scope.reset = function() {
        $scope.role = new roledto();
        $functions.jstree("deselect_all");
    };

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
                    "text": func.Name
                });
            } else {
                //other child nodes
                treeViewList.push({
                    "id": func.ID,
                    "parent": func.Module.toLowerCase(),
                    "text": func.Name
                });
            }
        });

        return treeViewList;
    }
});