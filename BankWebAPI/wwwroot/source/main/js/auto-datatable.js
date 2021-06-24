// Automatically initialises DataTables with the auto-datatable class

$(document).ready(function() {
    $('.auto-datatable').DataTable({
        "paging": true,
        "searching": false,
        "ordering": true,
        "info": false,
        responsive: true
    });
});