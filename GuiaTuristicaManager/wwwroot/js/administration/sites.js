var sitesPopUp = document.getElementById('sites-container');
var sitesZoneName = document.getElementById('sites-zoneName');//sites-zoneName
var datagirdSites, imageSite, formInstanceSite;
var placeId;
var multimedia, formInstanceMulti, datagridMedia;
var store2;

var medias = [];


function openSite(id) {
    openSitePopUp();

    var zoneName = zones[id].name;
    sitesZoneName.innerHTML = zoneName;

    placeId = id;

    angular.element($('#MasterController')).scope().changeSite(placeId);
}

DemoApp.controller('MasterController', function DemoController($scope) {


    var siteModel = {
        "name": "",
        "Image": ""
    };

    $scope.gridOptions = {
        dataSource: [],
        onInitialized: function (e) {
            datagirdSites = e.component;
        },
        keyExpr: "ID",
        noDataText: "Aún noo hay sitios en esta Zona, agrega uno.",
        searchPanel: {
            placeholder: "Busca algún sitio",
            visible: true
        },
        scrolling: {
            mode: 'infinite'
        },
        summary: {
            totalItems: [{
                column: "name",
                summaryType: "count",
                customizeText: function (data) {
                    return "Total: " + data.value;
                }
            }]
        },
        showBorders: true,
        editing: {
            useIcons: true,
            mode: "popup",
            allowUpdating: true,
            allowAdding: true,
            allowDeleting: true,
            popup: {
                title: "Nuevo sitio",
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
                onInitialized: function (e) {
                    formInstanceSite = e.component;
                },
                formData: siteModel,
                items: [
                    {
                        template: "<img id='picture' height='200px' width='200px'  border='solid 1px' src=''>"
                    },
                    {
                        dataField: "siteName",
                        label: { text: "Nombre" }
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
                                var files = e.value;
                                if (files.length > 0) {
                                    var file = files[0];
                                    fileCach = file;
                                    // Poner la imagen seleccionada en el tag IMG
                                    var reader = new FileReader();
                                    reader.addEventListener("load", function () {
                                        var img = document.getElementById('picture');
                                        img.src = event.target.result;
                                    }, false),
                                        function () { console.log = console.log.bind(console); }();

                                    reader.readAsDataURL(file);
                                    imageSite = e.value;
                                }
                            }
                        }
                    }
                ]
            }
        },
        columns: [{
            dataField: "placeId",
            caption: "Id",
            width: 70,
            visible: false
        },{
            dataField: "Ima",
            caption: "Imagen",
            width: 100,
                cellTemplate: function (container, e) {
                    var src = "data:image/jpeg;base64,";
                    src += e.data.base64Image;
                    var newImage = document.createElement('img');
                    newImage.src = src;
                    newImage.width = newImage.height = "80";

                    container[0].innerHTML = newImage.outerHTML;
                }
        },{
                dataField: "name",
                caption: "Nombre del sitio"
            }, {
                dataField: "Model",
                caption: "Modelo 3D",
                cellTemplate: function (container, e) {
                    if (e.data.Model !== undefined) { 
                        $("<div id=model" + e.data.Model.modelId + ">" + e.data.Model.name + "<i onclick= delteModel(" + e.data.Model.modelId + "," + e.data.placeId + ") style='cursor: pointer; margin-left:9px' class='fa fa-trash'></i></div>").appendTo(container);
                    }
                    else {
                        $("<input accept='.fbx' type='file' class='custom-file-input' id='customFile" + e.data.placeId + "' name='filename' title='" + e.data.placeId + "'> <label class='custom-file-label' for= 'customFile" + e.data.placeId + "' style='width: 22%; left: 454px'> Choose an fbx file</label >")
                            .appendTo(container);
                    }
                }
            }
        ],
        masterDetail: {
            enabled: true,
            template: "detail"
        }
    };

    $scope.getDetailGridSettings = function (key) {
        return {
            noDataText: "Aún no hay multimedia en esta sitio, agrega uno.",
            dataSource: store2 = new DevExpress.data.CustomStore({
                key: "placeId",
                data: medias,
                load: function () {
                    return medias.filter(e => e.placeId === key);
                },
                insert: function (values) {
                    var formData = new FormData();
                    var newName = formInstanceMulti.getEditor('siteName').option('value');

                    for (var i = 0; i !== multimedia.length; i++) {
                        formData.append("file", multimedia[i]);
                    }

                    var formato = multimedia[0].name.split(".")[1];

                    if (formato === 'mp4' || formato === 'MP4' || formato === 'AVI' || formato === 'avi')
                        type = 2;
                    else if (formato === 'jpg' || formato === 'JPG' || formato === 'gif' || formato === 'GIF' || formato === 'JEPG' || formato === 'jepg' || formato === 'png' || formato === 'PNG')
                        type = 1;
                    else
                        type = 0;


                    formData.append("name", newName);
                    formData.append("text", newName);
                    formData.append("type", type);
                    formData.append("modelId", 1);

                    $.ajax({
                        url: "Administration/PostMedia",
                        type: "POST",
                        processData: false,
                        contentType: false,
                        data: formData
                    }).done(function (response) {
                        datagirdSites.option("dataSource", createStore(idZone));
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
                        url: "Administration/DeleteMedia",
                        type: "POST",
                        data: { id: parseInt(key) }
                    }).done(function (response) {
                        store2.push([{ type: "remove", key: key }]);
                    });
                }
            }),
            editing: {
                useIcons: true,
                mode: "popup",
                allowUpdating: true,
                allowAdding: true,
                allowDeleting: true,
                popup: {
                    title: "Nuevo archivo multimedia",
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
                    onInitialized: function (e) {
                        formInstanceMulti = e.component;
                    },
                    formData: siteModel,
                    items: [
                        {
                            dataField: "siteName",
                            label: { text: "Nombre" }
                        }, {
                            dataField: "Image",
                            label: { text: "Archivo" },
                            editorType: "dxFileUploader",
                            editorOptions:
                            {
                                selectButtonText: "Seleccionar",
                                multiple: false,
                                labelText: "",
                                uploadUrl: "",
                                accept: ".jpg, .mp4, .txt",
                                uploadMode: "useForm",
                                onValueChanged: function (e) {
                                    var files = e.value
                                    if (files.length > 0) {
                                        var file = files[0]
                                        fileCach = file;
                                        // Poner la imagen seleccionada en el tag IMG
                                        var reader = new FileReader();
                                        reader.addEventListener("load", function () {
                                            //var img = document.getElementById('picture');
                                            //img.src = event.target.result;
                                        }, false),
                                            function () { console.log = console.log.bind(console); }();

                                        reader.readAsDataURL(file);
                                        multimedia = e.value;
                                    }
                                }
                            }
                        }
                    ]
                }
            },
            columnAutoWidth: true,
            showBorders: true,
            onInitialized: function (e) {
                datagridMedia = e.component;
            },
            columns: [{
                dataField: "name",
                caption: "Nombre",
            }, {
                    dataField: "type",
                    caption: "Tipo",
            }]
        };
    };

    $scope.changeSite = function (idZone) {
        medias = [];
        $.ajax({
            url: "Administration/GetAllPlace",
            data: { id: idZone },
            type: "GET"
        }).done(function (response) {

            $.each(response, function (index, place) {
                $.ajax({
                    url: "Administration/GetModel",
                    data: { id: place.placeId },
                    type: "GET"
                }).done(function (response) {

                    place.Model = response;

                    $.each(response.media, function (index, media) {
                        media.placeId = place.placeId;
                        media.modelId = response.modelId;
                        medias.push(media);
                    });
                });
            });


            setTimeout(function () {
                var store2 = new DevExpress.data.CustomStore({
                    key: "placeId",
                    data: response,
                    load: function () {
                        return response;
                    },
                    insert: function (values) {
                        var formData = new FormData();
                        var newName = formInstanceSite.getEditor('siteName').option('value');

                        for (var i = 0; i !== imageSite.length; i++) {
                            formData.append("file", imageSite[i]);
                        }

                        formData.append("placename", newName);
                        formData.append("zoneId", idZone);

                        $.ajax({
                            url: "Administration/PostPlace",
                            type: "POST",
                            processData: false,
                            contentType: false,
                            data: formData
                        }).done(function (response) {
                            datagirdSites.option("dataSource", createStore(idZone));
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
                            url: "Administration/DeletePlace",
                            type: "POST",
                            data: { id: parseInt(key) }
                        }).done(function (response) {
                            store2.push([{ type: "remove", key: key }]);
                        });
                    }
                });

                datagirdSites.option("dataSource", store2);



            }, 3000);
        });
    };


});

function openSitePopUp() {
    sitesPopUp.style.display = "block";
    sitesPopUp.classList.remove('animated', 'fadeOut');
    sitesPopUp.classList.add('animated', 'fadeIn');
}

function closeSitePopUp() {
    sitesPopUp.classList.remove('animated', 'fadeIn');
    sitesPopUp.classList.add('animated', 'fadeOut');

    datagirdSites.option("dataSource", []);


    setTimeout(function () {
        sitesPopUp.style.display = "none";
    }, 1000);
}

setInterval(function () {
    datagirdSites.updateDimensions();
}, 100);

setInterval(function () {
    $(".custom-file-input").on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        var extension = fileName.split(".")[1];

        var placeid = this.title;

        if (extension === 'fbx') {
            $(this).siblings(".custom-file-label").addClass("selected").html(fileName);

            var formData = new FormData();
            for (var i = 0; i !== this.files.length; i++) {
                formData.append("file", this.files[i]);
            }

            formData.append("name", fileName);
            formData.append("placeid", placeid);

            $.ajax({
                url: "Administration/PostModel",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                error: function (response) {
                    console.log("Error en modelo " + response);
                }
            }).done(function (response) {
            });
        }
    });
}, 3000);


function delteModel(modelId, placeId) {
    var r = DevExpress.ui.dialog.confirm("¿Quieres Eliminar el modelo?", "Confirmar");
    r.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: "Administration/DeleteModel",
                type: "POST",
                data: { id: parseInt(modelId) }
            }).done(function (response) {
                $('#model' + modelId).html(
                    '     <input type="file" class="custom-file-input" id="customFile"' + placeId + ' title= "' + placeId + '"name="filename"> ' +
                    '     <label class= "custom-file-label" for= "customFile" style="width: 22%; left: 454px" > Choose an fbx file</label >'
                );
            });
        }
    });
}


function createStore(idZone) {
    $.ajax({
        url: "Administration/GetAllPlace",
        data: { id: idZone },
        type: "GET"
    }).done(function (response) {

        $.each(response, function (index, place) {
            $.ajax({
                url: "Administration/GetModel",
                data: { id: place.placeId },
                type: "GET"
            }).done(function (response) {
                place.Model = response;
            });
        });


        setTimeout(function () {
            var store2 = new DevExpress.data.CustomStore({
                key: "placeId",
                data: response,
                load: function () {
                    return response;
                },
                insert: function (values) {
                    var formData = new FormData();
                    var newName = formInstanceSite.getEditor('siteName').option('value');

                    for (var i = 0; i !== imageSite.length; i++) {
                        formData.append("file", imageSite[i]);
                    }

                    formData.append("placename", newName);
                    formData.append("zoneId", idZone);

                    $.ajax({
                        url: "Administration/PostPlace",
                        type: "POST",
                        processData: false,
                        contentType: false,
                        data: formData
                    }).done(function (response) {
                        datagirdSites.option("dataSource", createStore(idZone));
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
                        url: "Administration/DeletePlace",
                        type: "POST",
                        data: { id: parseInt(key) }
                    }).done(function (response) {
                        store2.push([{ type: "remove", key: key }]);
                    });
                }
            });

            datagirdSites.option("dataSource", store2);
        }, 2000);
    });
}


function createStore2(){
    return store2 = new DevExpress.data.CustomStore({
        key: "placeId",
        data: function () {

        },
        load: function () {
            var model;
            $.ajax({
                url: "Administration/GetModel",
                data: { id: 1 },
                type: "GET"
            }).done(function (response) {
                model = response.media;
            });
            setTimeout(function () {
                return model;
            }, 2000);
        },
        insert: function (values) {
            var formData = new FormData();
            var newName = formInstanceMulti.getEditor('siteName').option('value');

            for (var i = 0; i !== multimedia.length; i++) {
                formData.append("file", multimedia[i]);
            }

            var formato = multimedia[0].name.split(".")[1];

            if (formato === 'mp4' || formato === 'MP4' || formato === 'AVI' || formato === 'avi')
                type = 2;
            else if (formato === 'jpg' || formato === 'JPG' || formato === 'gif' || formato === 'GIF' || formato === 'JEPG' || formato === 'jepg' || formato === 'png' || formato === 'PNG')
                type = 1;
            else
                type = 0;


            formData.append("name", newName);
            formData.append("text", newName);
            formData.append("type", type);
            formData.append("modelId", 1);

            $.ajax({
                url: "Administration/PostMedia",
                type: "POST",
                processData: false,
                contentType: false,
                data: formData
            }).done(function (response) {
                datagirdSites.option("dataSource", createStore(idZone));
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
                url: "Administration/DeletePlace",
                type: "POST",
                data: { id: parseInt(key) }
            }).done(function (response) {
                store2.push([{ type: "remove", key: key }]);
            });
        }
    });
}
