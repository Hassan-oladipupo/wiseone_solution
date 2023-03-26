'use strict';

app.controller('ViewClassRoomCtrl', function($sessionStorage, $scope, $state, configFileService) {

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
            ajax: configFileService.apiHandler.retrieveClassRooms,
            columns: [
                { "data": "Name" },
                { "data": "Location.Name" },
                { "data": "CreatedOn" },
                { "data": "LastModifiedOn" },
                { "data": "Status" }
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $classRoomList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.refresh = function() {
        var table = $classRoomList.DataTable();
        table.ajax.reload();
    };
});