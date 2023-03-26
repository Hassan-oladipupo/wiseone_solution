'use strict';

app.controller('UpdateClassRoomCtrl', function($sessionStorage, $scope, $state, configFileService, confirmService) {

    var $classRoomList = $('#classRoomList');

    $scope.init = function() {

        $classRoomList.find('tfoot').find('th').each(function() {
            var title = $classRoomList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $classRoomList.DataTable({
            stateSave: false,
            processing: true,
            ajax: configFileService.apiHandler.retrieveActiveClassRooms,
            columns: [
                { "data": "Name" },
                { "data": "Location.Name" },
                { "data": "CreatedOn" },
                { "data": "LastModifiedOn" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-edit themeprimary bordered-themeprimary dt-icon"></i>'
                }
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $classRoomList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $classRoomList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    }

    $scope.refresh = function() {
        var table = $classRoomList.DataTable();
        table.ajax.reload();
    }

    $scope.confirmEdit = function(data) {

        var modalOptions = {
            headerText: 'Modify Room: ' + data.Name + '?',
            bodyText: 'Are you sure you want to modify this Room?'
        };

        confirmService.showModal({}, modalOptions).then(function() {
            $sessionStorage.transferData = {
                edit: true,
                data: data
            };

            $state.go('app.classroomdetail');
        });
    }
});