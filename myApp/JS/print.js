
function printResult(...tableNames) {
    var printContents = tableNames.map(tableName => {
        var table = document.getElementById(tableName);
        if (table) {
            return table.outerHTML;
        }
        return '';
    }).join('');

    var originalContents = document.body.innerHTML;

    var printWindow = window.open('', '_blank');
    var printDocument = printWindow.document;

    printDocument.write('<!DOCTYPE html>');
    printDocument.write('<html>');
    printDocument.write('<head>');
    printDocument.write('<meta charset="utf-8">');
    printDocument.write('<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">');
    printDocument.write('<title>Print</title>');
    printDocument.write('<meta name="filename" content="Print">');
    printDocument.write('<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">');
    //printDocument.write('<link href="https://fonts.googleapis.com/css2?family=Noto+Serif+Thai&display=swap" rel="stylesheet">');
    printDocument.write('<style type="text/css">');
    printDocument.write('body { font-family: "Noto Serif Thai", serif; font-size: 12px; }');
    printDocument.write('@page { size: landscape; }');
    printDocument.write('</style>');

    printDocument.write('</head>');
    printDocument.write('<body>');

    printDocument.write(printContents);

    printDocument.write('</body>');
    printDocument.write('</html>');
    printDocument.close();

    setTimeout(function () {
        printWindow.print();
        printWindow.close();
        document.body.innerHTML = originalContents;
    }, 500);
}
