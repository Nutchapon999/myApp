function handleButtonClick(button) {
    var modalRadio = button.parentNode.querySelector('input[name^="modalRadio"]:checked');
    if (modalRadio) {
        button.textContent = modalRadio.value;
        calculateGap(button);

        var row = button.closest('tr');
        var selectedActual = parseFloat(modalRadio.value);
        var actualInput = row.querySelector('input[name^="forms"][name$=".Actual"]');
        actualInput.value = selectedActual;
    }
}

var actualButtons = document.querySelectorAll('button[id^="actualButton"]');
actualButtons.forEach(function (button) {
    button.addEventListener('click', function () {
        handleButtonClick(this);
    });
});

var radioButtons = document.querySelectorAll('input[name^="modalRadio"]');
radioButtons.forEach(function (radioButton) {
    radioButton.addEventListener('change', function () {
        var selectedValue = this.value;
        var actualButton = this.closest('.modal').previousElementSibling;
        if (actualButton) {
            actualButton.textContent = selectedValue;
            calculateGap(actualButton);
            updateHiddenInput(actualButton);
        }
    });
});

function calculateGap(button) {
    var row = button.closest('tr');
    var negativeGapCell = row.querySelector("td:nth-child(6) span");
    var positiveGapCell = row.querySelector("td:nth-child(7) span");
    var priorityCell = row.querySelector("td:nth-child(8) span");
    var planCell = row.querySelector("td:nth-child(9) span");
    var planDescCell = row.querySelector("td:nth-child(10) span");
    var Q1Cell = row.querySelector("td:nth-child(11) span");
    var Q2Cell = row.querySelector("td:nth-child(12) span");
    var Q3Cell = row.querySelector("td:nth-child(13) span");
    var Q4Cell = row.querySelector("td:nth-child(14) span");
    var requirement = parseFloat(row.querySelector("td:nth-child(4) input").value);
    var actual = parseFloat(button.textContent);
    var gap = actual - requirement;

    negativeGapCell.textContent = (gap < 0) ? gap : '';
    positiveGapCell.textContent = (gap > 0) ? gap : (gap === 0) ? '0' : '';

    if (gap < 0) {
        priorityCell.innerHTML = '<select class="form-select" name="' + button.name.replace('Actual', 'Priority') + '">' +
            '<option value="A" ' + (button.name.includes('A') ? 'selected' : '') + '>A</option>' +
            '<option value="B" ' + (button.name.includes('B') ? 'selected' : '') + '>B</option>' +
            '<option value="C" ' + (button.name.includes('C') ? 'selected' : '') + '>C</option>' +
            '</select>';

        var prioritySelect = priorityCell.querySelector('select');
        prioritySelect.addEventListener('change', function () {
            var selectedPriority = this.value;
            var priorityInput = row.querySelector('input[name^="forms"][name$=".Priority"]');
            priorityInput.value = selectedPriority;
        });

        // Set the initial priority value to the first option
        var priorityInput = row.querySelector('input[name^="forms"][name$=".Priority"]');
        priorityInput.value = prioritySelect.value;

        planCell.innerHTML = '<select class="form-select" name="' + button.name.replace('Actual', 'Plan') + '">' +
            '<option value="Co" ' + (button.name.includes('Co') ? 'selected' : '') + '>ผู้บังคับบัญชา (Coaching)</option>' +
            '<option value="INT" ' + (button.name.includes('INT') ? 'selected' : '') + '>อบรมภายใน (Inhous Training)</option>' +
            '<option value="JA" ' + (button.name.includes('JA') ? 'selected' : '') + '>การมอบหมายงาน (Job Assignment)</option>' +
            '<option value="EXT" ' + (button.name.includes('EXT') ? 'selected' : '') + '>อบรมภายนอก (External Training)</option>' +
            '<option value="PJ" ' + (button.name.includes('PJ') ? 'selected' : '') + '>การมอบหมายโครงการ (Project Assignment)</option>' +
            '<option value="SS" ' + (button.name.includes('SS') ? 'selected' : '') + '>การให้ศึกษาเอง (Self- Study)</option>' +
            '<option value="OJT" ' + (button.name.includes('OJT') ? 'selected' : '') + '>การสอนงานหน้างาน (On The Job Training)</option>' +
            '</select>';

        var planSelect = planCell.querySelector('select');
        planSelect.addEventListener('change', function () {
            var selectedPlan = this.value;
            var planInput = row.querySelector('input[name^="forms"][name$=".Plan"]');
            planInput.value = selectedPlan;
        });

        var planInput = row.querySelector('input[name^="forms"][name$=".Plan"]');
        planInput.value = planSelect.value;

        planDescCell.innerHTML = '<textarea class="form-control" name="' + button.name.replace('Actual', 'PlanDesc') + '"></textarea>';

        var planDescTextarea = planDescCell.querySelector('textarea[name="' + button.name.replace('Actual', 'PlanDesc') + '"]');
        if (planDescTextarea) {
            var planDescInput = row.querySelector('input[name^="forms"][name$=".PlanDesc"]');
            if (planDescInput) {
                planDescTextarea.addEventListener('input', function () {
                    planDescInput.value = this.value;
                });
                planDescInput.value = planDescTextarea.value;
            }
        }

        Q1Cell.innerHTML = '<input class="form-check-input" type="checkbox" name="' + button.name.replace('Actual', 'Q1') + '" value="1" ' + (button.name.includes('Q1') ? 'checked' : '') + '>';
        Q2Cell.innerHTML = '<input class="form-check-input" type="checkbox" name="' + button.name.replace('Actual', 'Q2') + '" value="1" ' + (button.name.includes('Q2') ? 'checked' : '') + '>';
        Q3Cell.innerHTML = '<input class="form-check-input" type="checkbox" name="' + button.name.replace('Actual', 'Q3') + '" value="1" ' + (button.name.includes('Q3') ? 'checked' : '') + '>';
        Q4Cell.innerHTML = '<input class="form-check-input" type="checkbox" name="' + button.name.replace('Actual', 'Q4') + '" value="1" ' + (button.name.includes('Q4') ? 'checked' : '') + '>';

        var Q1check = Q1Cell.querySelector('input');
        Q1check.addEventListener('change', function () {
            var checkQ1 = this.checked ? '1' : '';
            var Q1Input = row.querySelector('input[name^="forms"][name$=".Q1"]');
            Q1Input.value = checkQ1;
        });

        var Q2check = Q2Cell.querySelector('input');
        Q2check.addEventListener('change', function () {
            var checkQ2 = this.checked ? '1' : '';
            var Q2Input = row.querySelector('input[name^="forms"][name$=".Q2"]');
            Q2Input.value = checkQ2;
        });

        var Q3check = Q3Cell.querySelector('input');
        Q3check.addEventListener('change', function () {
            var checkQ3 = this.checked ? '1' : '';
            var Q3Input = row.querySelector('input[name^="forms"][name$=".Q3"]');
            Q3Input.value = checkQ3;
        });

        var Q4check = Q4Cell.querySelector('input');
        Q4check.addEventListener('change', function () {
            var checkQ4 = this.checked ? '1' : '';
            var Q4Input = row.querySelector('input[name^="forms"][name$=".Q4"]');
            Q4Input.value = checkQ4;
        });



    } else {
        priorityCell.innerHTML = '';
        planCell.innerHTML = '';
        planDescCell.innerHTML = '';
        Q1Cell.innerHTML = '';
        Q2Cell.innerHTML = '';
        Q3Cell.innerHTML = '';
        Q4Cell.innerHTML = '';

        var priorityInput = row.querySelector('input[name^="forms"][name$=".Priority"]');
        priorityInput.value = '';
        var planInput = row.querySelector('input[name^="forms"][name$=".Plan"]');
        planInput.value = '';
        var planDescInput = row.querySelector('textarea[name^="forms"][name$=".PlanDesc"]');
        if (planDescInput) {
            planDescInput.value = '';
        }
        var Q1Input = row.querySelector('input[name^="forms"][name$=".Q1"]');
        if (Q1Input) {
            Q1Input.checked = false;
            Q1Input.value = "";
        }

        // ค่า checkbox Q2
        var Q2Input = row.querySelector('input[name^="forms"][name$=".Q2"]');
        if (Q2Input) {
            Q2Input.checked = false;
            Q2Input.value = "";
        }

        // ค่า checkbox Q3
        var Q3Input = row.querySelector('input[name^="forms"][name$=".Q3"]');
        if (Q3Input) {
            Q3Input.checked = false;
            Q3Input.value = "";
        }

        // ค่า checkbox Q4
        var Q4Input = row.querySelector('input[name^="forms"][name$=".Q4"]');
        if (Q4Input) {
            Q4Input.checked = false;
            Q4Input.value = "";
        }
    }

    var actualInput = row.querySelector("input[name^='forms'][name$='.Actual']");
    actualInput.value = actual;
}



function updateHiddenInput(button) {
    var row = button.closest('tr');
    var prioritySelect = row.querySelector('select[name^="forms"][name$=".Priority"]');
    var priorityInput = row.querySelector('input[name^="forms"][name$=".Priority"]');
    if (prioritySelect) {
        var selectedPriority = prioritySelect.value;
        priorityInput.value = selectedPriority;
    }
    var planSelect = row.querySelector('select[name^="forms"][name$=".Plan"]');
    var planInput = row.querySelector('input[name^="forms"][name$=".Plan"]');
    if (planSelect) {
        var selectedPlan = planSelect.value;
        planInput.value = selectedPlan;
    }
    var planDescTextarea = row.querySelector('textarea[name^="forms"][name$=".PlanDesc"]');
    var planDescInput = row.querySelector('input[name^="forms"][name$=".PlanDesc"]');
    if (planDescTextarea) {
        var selectedPlanDesc = planDescTextarea.value;
        planDescInput.value = selectedPlanDesc;
    }
    var Q1Check = row.querySelector('input[type="checkbox"][name^="forms"][name$=".Q1"]');
    var Q1Input = row.querySelector('input[name^="forms"][name$=".Q1"]');
    if (Q1Check) {
        var checkedQ1 = Q1Check.checked ? '1' : '';
        Q1Input.value = checkedQ1;
    }
    var Q2Check = row.querySelector('input[type="checkbox"][name^="forms"][name$=".Q2"]');
    var Q2Input = row.querySelector('input[name^="forms"][name$=".Q2"]');
    if (Q2Check) {
        var checkedQ2 = Q2Check.checked ? '1' : '';
        Q2Input.value = checkedQ2;
    }
    var Q3Check = row.querySelector('input[type="checkbox"][name^="forms"][name$=".Q3"]');
    var Q3Input = row.querySelector('input[name^="forms"][name$=".Q3"]');
    if (Q3Check) {
        var checkedQ3 = Q3Check.checked ? '1' : '';
        Q3Input.value = checkedQ3;
    }
    var Q4Check = row.querySelector('input[type="checkbox"][name^="forms"][name$=".Q4"]');
    var Q4Input = row.querySelector('input[name^="forms"][name$=".Q4"]');
    if (Q4Check) {
        var checkedQ4 = Q4Check.checked ? '1' : '';
        Q4Input.value = checkedQ4;
    }
}



var forms = document.querySelectorAll('form[id^="form-"]');
forms.forEach(function (form) {
    form.addEventListener('submit', function (event) {
        var actualButtons = this.querySelectorAll('button[id^="actualButton"]');
        actualButtons.forEach(function (button) {
            updateHiddenInput(button);
        });
    });
});