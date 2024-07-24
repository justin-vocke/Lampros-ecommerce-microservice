var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else if (url.includes("readyforpickup")) {
        loadDataTable("readyforpickup");
    }
    else if (url.includes("cancelled")) {
        loadDataTable("cancelled");
    }
    else {
        loadDataTable("all")
    }
    
});

function loadDataTable(status) {
    $('#tblData').DataTable({
        order: [[0, 'desc']],
        "ajax": { url: "/order/getall?orderStatus=" + status },
        "columns": [
            { data: 'orderHeaderId', "width": "5%" },
            { data: 'email', "width": "25%" },
            { data: 'firstName', "width": "10%" },
            { data: 'lastName', "width": "10%" },
            { data: 'phone', "width": "10%" },
            { data: 'status', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'orderHeaderId',
                "render": function (data) {
                    return `<div class="a-75 btn-group" role="group">
                    <a href="/order/orderDetail?orderId=${data}" class="btn btn-primary mx-2">
                        <i class="bi bi-pencil-square"></i>
                    </a>
                    </div>`
                },
                "width":"10%"
            }
        ]
    });
}