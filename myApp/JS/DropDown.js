var js = jQuery.noConflict(true);
js(document).ready(function () {
    js('#myTable').DataTable({
        initComplete: function () {
            var api = this.api();
            api.columns([0, 1, 2, 3, 4, 5]).every(function () {
                var Destino = '#prueba';
                var column = this;
                var select = $('<select class="Filtros select2 col-md-1" style="width:15%"><option value="">Seleccionar Todas</option></select>&nbsp; &nbsp; &nbsp; <script>$(".Filtros").select2({allowClear: true,theme: "classic",dropddownAutoWidth: false,placeholder: "Selecciona"});</script>')
                    .appendTo(Destino)
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );

                        column
                            .search(val ? '^' + val + '$' : '', true, false)
                            .draw();
                    });

                column.data().unique().sort().each(function (d, j) {
                    select.append('<option value="' + d + '">' + d + '</option>');
                });
            });
            api.on('draw', function () {
                console.log('here')
                api.columns([0, 1, 2, 3, 4, 5]).every(function (idx) {

                    var Destino = $("#prueba");
                    var column = this;
                    var idx = this.index();

                    console.log(idx)
                    var select = $(table.column(idx)).find('select');
                    console.log(select)

                    if (select.val() === '') {
                        select
                            .empty()
                            .append('<option value="" />');

                        api.column(idx, {
                            search: 'applied'
                        }).data().unique().sort().each(function (d, j) {
                            console.log(d)
                            select.append('<option value="' + d + '">' + d + '</option>');
                        });
                    }
                });
            });

        },
    });
});