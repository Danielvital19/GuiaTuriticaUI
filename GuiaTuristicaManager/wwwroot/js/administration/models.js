var image, datagird, zones = [], store;

var DemoApp = angular.module('DemoApp', ['dx']);

DemoApp.controller('DemoController', function DemoController($scope) {

    var ZoneModel = {
        "Name": 0,
        "Image": ""
    };

    $scope.dataGridOptions = {
        dataSource: [],
        keyExpr: "name",
        showBorders: true,
        onInitialized: function (e) {
            datagird = e.component;
        },
        editing: {
            useIcons: true,
            mode: "popup",
            allowUpdating: true,
            allowAdding: true,
            allowDeleting: true,
            popup: {
                title: "Nueva Zona",
                showTitle: true,
                width: 700,
                height: 525,
                position: {
                    my: "top",
                    at: "top",
                    of: window
                }
            },
            form: {
                formData: ZoneModel,
                items: [
                    {
                        template: "<img id='picture' height='200px' width='200px'  border='solid 1px' src=''>"
                    },
                    {

                        dataField: "name",
                        label: { text: "Nombre" },
                        editorOptions: {
                            maxLength: '20'
                        }
                    }, {
                        dataField: "Image",
                        label: { text: "Imagen" },
                        editorType: "dxFileUploader",
                        editorOptions:
                        {
                            selectButtonText: "Seleccionar",
                            multiple: false,
                            labelText: "",
                            uploadUrl: "",
                            accept: "image/*",
                            uploadMode: "useForm",
                            onValueChanged: function (e) {
                                var files = e.value
                                if (files.length > 0) {
                                    var file = files[0]
                                    fileCach = file;
                                    // Poner la imagen seleccionada en el tag IMG
                                    var reader = new FileReader();
                                    reader.addEventListener("load", function () {
                                        var img = document.getElementById('picture');
                                        img.src = event.target.result;
                                    }, false),
                                        function () { console.log = console.log.bind(console); }();

                                    reader.readAsDataURL(file);
                                    image = e.value;
                                }
                            }
                        }
                    }
                ]
            }
        },
        columns: [
            { caption: "ID", dataField: "zoneId", alignment: 'center', width: 100 },
            { caption: "Nombre de la zona", dataField: "name", alignment: 'center' },
            {
                caption: "Ver", dataField: "edit", alignment: 'center', width: 100,
                cellTemplate: function (container, e) {

                    $("<div disabled='true' style='display:inline;margin-right:4px'>")
                        .append('<button class="btn btn-link btn-sm" onclick="openSite(' + e.data.zoneId+')"> <i class="fas fa-edit"></i> </button>')
                        .appendTo(container);
                }
            }

        ]
    };

    $scope.loadData = function () {
        $.ajax({
            url: "Administration/GetAllZones",
            cache: false,
            contentType: 'application/html ; charset:utf-8',
            type: "GET"
        }).done(function (response) {

            $.each(response, function (i, zone) {
                zones[zone.zoneId] = zone;
            });

            store = new DevExpress.data.CustomStore({
                key: "zoneId",
                data: response,
                load: function () {
                    return response;
                },
                insert: function (values) {
                    var formData = new FormData();

                    for (var i = 0; i != image.length; i++) {
                        formData.append("file", image[i]);
                    }

                    formData.append("zonename", values.name);

                    $.ajax({
                        url: "Administration/PostZone",
                        type: "POST",
                        processData: false,
                        contentType: false,
                        data: formData
                    }).done(function (response) {
                        reloadStore();
                    });
                },
                update: function (key, values) {
                    return sendRequest(URL + "/UpdateOrder", "PUT", {
                        key: key,
                        values: JSON.stringify(values)
                    });
                },
                remove: function (key) {
                    $.ajax({
                        url: "Administration/DeleteZone",
                        type: "POST",
                        data: { id: parseInt(key) }
                    }).done(function (response) {
                        store.push([{ type: "remove", key: key }]);
                    });
                }
            });

            datagird.option("dataSource", store);
        });
    };

    $scope.loadData();
});

function reloadStore() {
    $.ajax({
        url: "Administration/GetAllZones",
        cache: false,
        contentType: 'application/html ; charset:utf-8',
        type: "GET"
    }).done(function (response) {

        $.each(response, function (i, zone) {
            zones[zone.zoneId] = zone;
        });

        store = new DevExpress.data.CustomStore({
            key: "zoneId",
            data: response,
            load: function () {
                return response;
            },
            insert: function (values) {
                var formData = new FormData();

                for (var i = 0; i != image.length; i++) {
                    formData.append("file", image[i]);
                }

                formData.append("zonename", values.name);

                $.ajax({
                    url: "Administration/PostZone",
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: formData
                }).done(function (response) {
                    reloadStore();
                });
            },
            update: function (key, values) {
                return sendRequest(URL + "/UpdateOrder", "PUT", {
                    key: key,
                    values: JSON.stringify(values)
                });
            },
            remove: function (key) {
                $.ajax({
                    url: "Administration/DeleteZone",
                    type: "POST",
                    data: { id: parseInt(key) }
                }).done(function (response) {
                    store.push([{ type: "remove", key: key }]);
                });
            }
        });

        datagird.option("dataSource", store);
    });
}