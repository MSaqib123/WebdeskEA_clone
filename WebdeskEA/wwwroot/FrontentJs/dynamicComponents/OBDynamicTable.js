// File: wwwroot/js/components/OBDynamicTable.js

export class OBDynamicTable {
    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
        this.selectors = {
            tableBody: document.getElementById(tableBodyId),
            addRowBtn: document.getElementById(addButtonId),
            coaSelectClass: ".coa-select",
            tranTypeSelectClass: ".tran-type-select",
            openBalanceInputClass: ".open-balance-input",
            hiddenIdClass: ".ob-dtl-id",
            hiddenOBIdClass: ".ob-dtl-obid"
        };

        this.hiddenDisabledInputClass = "hidden-disabled-input";
        this.nameAttributeOptionObject = nameAttributeOptionObject;

        this.coasData = options.coasData || [];
        this.transactionTypesData = options.transactionTypesData || [];
        this.prefilledRows = options.prefilledRows || [];
        this.isEditMode = options.isEditMode || false;
        this.mode = options.mode || 'create';
        this.editTemplateType = options.editTemplateType || 'Default';

        this.selectedCOAIds = new Set();

        // Bind methods
        this.handleAddRow = this.handleAddRow.bind(this);
        this.handleTableInput = this.handleTableInput.bind(this);
        this.handleTableClick = this.handleTableClick.bind(this);

        this.initialize();
    }

    initialize() {
        if (!this.selectors.tableBody) {
            console.error("Table body element not found.");
            return;
        }

        this.renderInitialRows();
        this.addEventListeners();

        // Hide Add button if in delete/detail mode
        if (this.mode === 'delete' || this.mode === 'detail') {
            if (this.selectors.addRowBtn) {
                this.selectors.addRowBtn.style.display = 'none';
            }
        }
    }

    removeEventListeners() {
        if (this.selectors.tableBody) {
            this.selectors.tableBody.removeEventListener("input", this.handleTableInput);
            this.selectors.tableBody.removeEventListener("click", this.handleTableClick);
        }
        if (this.selectors.addRowBtn) {
            this.selectors.addRowBtn.removeEventListener("click", this.handleAddRow);
        }
    }

    updateData(newOptions, newNameAttributeOptionObject) {
        this.removeEventListeners();

        this.coasData = newOptions.coasData || this.coasData;
        this.transactionTypesData = newOptions.transactionTypesData || this.transactionTypesData;
        this.prefilledRows = newOptions.prefilledRows || [];
        this.isEditMode = newOptions.isEditMode || this.isEditMode;
        this.mode = newOptions.mode || this.mode;
        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

        if (newNameAttributeOptionObject) {
            this.nameAttributeOptionObject = newNameAttributeOptionObject;
        }

        this.selectors.tableBody.innerHTML = "";
        this.selectedCOAIds.clear();

        this.initialize();
    }

    normalizeKeys(obj) {
        const normalized = {};
        for (const key in obj) {
            normalized[key.toLowerCase()] = obj[key];
        }
        return normalized;
    }

    renderInitialRows() {
        this.selectors.tableBody.innerHTML = "";

        // Render all rows first
        if (this.prefilledRows && this.prefilledRows.length > 0) {
            this.prefilledRows.forEach((rowData) => {
                this.renderRow(rowData);
                const normalizedData = this.normalizeKeys(rowData);
                if (normalizedData['coaid']) {
                    this.selectedCOAIds.add(parseInt(normalizedData['coaid']));
                }
            });
        } else if (this.mode === 'create') {
            this.renderRow(null);
        }

        // After all rows are rendered, update indices
        this.updateRowIndices();

        // If in edit mode, we now disable rows after indexing and initialization
        if (this.mode === 'edit') {
            // Disable rows according to template
            const rows = this.selectors.tableBody.querySelectorAll("tr");
            rows.forEach((row) => {
                if (['Template_1', 'Template_2', 'Template_3'].includes(this.editTemplateType)) {
                    this.disableRow(row);
                } else {
                    // If no specific template, you might choose to leave them enabled
                    // or implement custom logic.
                }
            });
        }
    }

    renderRow(data = null) {
        const normalizedData = data ? this.normalizeKeys(data) : {};
        const coaId = normalizedData['coaid'] || "";
        const tranType = normalizedData['obdtltrantype'] || "";
        const openBalance = normalizedData['obdtlopenblnc'] || "";
        const rowId = normalizedData['id'] || "";
        const obId = normalizedData['obid'] || "";

        const coaOptions = this.coasData.map(coa =>
            `<option value="${coa.Id}" ${coaId == coa.Id ? "selected" : ""}>${coa.AccountName}</option>`
        ).join("");

        const tranTypeOptions = this.transactionTypesData.map(tran =>
            `<option value="${tran.Value}" ${tranType === tran.Value ? "selected" : ""}>${tran.Text}</option>`
        ).join("");

        const rowHTML = `
            <tr>
                <td>
                    <div class="tom-select-custom">
                        <select class="js-select form-select coa-select">
                            <option value="">--- Select COA ---</option>
                            ${coaOptions}
                        </select>
                    </div>
                </td>
                <td>
                    <div class="tom-select-custom">
                        <select class="js-select form-select tran-type-select">
                            <option value="">--- Select Transaction Type ---</option>
                            ${tranTypeOptions}
                        </select>
                    </div>
                </td>
                <td>
                    <input type="number" class="form-control open-balance-input" min="0" value="${openBalance}" required data-msg="Open Balance is required." />
                    <span class="invalid-feedback">Open Balance is required.</span>
                </td>
                <td>
                    ${this.getActionButtonHTML(data)}
                </td>
            </tr>
        `;

        this.selectors.tableBody.insertAdjacentHTML("beforeend", rowHTML);
        const row = this.selectors.tableBody.lastElementChild;

        // If data exists, add hidden inputs for Id and OBId
        if (data) {
            if (rowId) {
                const hiddenId = document.createElement('input');
                hiddenId.type = 'hidden';
                hiddenId.value = rowId;
                hiddenId.classList.add('ob-dtl-id');
                row.appendChild(hiddenId);
            }

            if (obId) {
                const hiddenOBId = document.createElement('input');
                hiddenOBId.type = 'hidden';
                hiddenOBId.value = obId;
                hiddenOBId.classList.add('ob-dtl-obid');
                row.appendChild(hiddenOBId);
            }
        }

        // Initialize TomSelect
        const coaSelect = row.querySelector(this.selectors.coaSelectClass);
        new TomSelect(coaSelect, {
            placeholder: "--- Select COA ---",
            maxItems: 1,
            onChange: (value) => this.handleCOASelection(row, value, data),
        });

        const tranTypeSelect = row.querySelector(this.selectors.tranTypeSelectClass);
        new TomSelect(tranTypeSelect, {
            placeholder: "--- Select Transaction Type ---",
            maxItems: 1,
            onChange: (value) => this.handleTransactionTypeSelection(row, value, data),
        });
    }

    getActionButtonHTML(data) {
        if (data && this.mode === 'edit') {
            if (this.editTemplateType === 'Template_1') {
                return `
                <div class="btn-group" role="group">
                    <button type="button" class="btn btn-white btn-sm edit-row">
                        <i class="bi-pencil-fill me-1"></i> Edit
                    </button>
                    <div class="btn-group">
                        <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty" data-bs-toggle="dropdown" aria-expanded="false"></button>
                        <div class="dropdown-menu dropdown-menu-end mt-1">
                            <a class="dropdown-item delete-row" href="#">
                                <i class="bi-trash dropdown-item-icon"></i> Delete
                            </a>
                        </div>
                    </div>
                </div>
                `;
            } else {
                return '<button type="button" class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
            }
        } else if (!data && (this.mode === 'create' || this.mode === 'edit')) {
            return '<button type="button" class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
        }
        return '';
    }

    handleCOASelection(row, coaId, existingData = null) {
        const previousCOAId = row.dataset.previousCoaId;
        if (previousCOAId) {
            this.selectedCOAIds.delete(parseInt(previousCOAId));
        }

        const openBalanceInput = row.querySelector(this.selectors.openBalanceInputClass);
        if (!coaId || coaId === "0") {
            openBalanceInput.disabled = true;
            openBalanceInput.value = "";
            row.dataset.previousCoaId = null;
            return;
        }

        if (this.selectedCOAIds.has(parseInt(coaId))) {
            alert("This COA is already selected. Choose another.");
            row.querySelector(this.selectors.coaSelectClass).tomselect.clear();
            return;
        }

        this.selectedCOAIds.add(parseInt(coaId));
        row.dataset.previousCoaId = coaId;
        openBalanceInput.disabled = false;
    }

    handleTransactionTypeSelection(row, tranType, existingData = null) {
        // Implement logic if needed
    }

    updateRowIndices() {
        const nameAttributes = this.nameAttributeOptionObject;

        this.selectors.tableBody.querySelectorAll("tr").forEach((row, index) => {
            row.setAttribute("data-index", index);

            const coaSelect = row.querySelector(this.selectors.coaSelectClass);
            if (coaSelect) {
                coaSelect.setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.coaId}`);
            }

            const tranTypeSelect = row.querySelector(this.selectors.tranTypeSelectClass);
            if (tranTypeSelect) {
                tranTypeSelect.setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.obDtlTranType}`);
            }

            const openBalanceInput = row.querySelector(this.selectors.openBalanceInputClass);
            if (openBalanceInput) {
                openBalanceInput.setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.obDtlOpenBlnc}`);
            }

            const hiddenIdInput = row.querySelector(this.selectors.hiddenIdClass);
            if (hiddenIdInput) {
                hiddenIdInput.setAttribute("name", `${nameAttributes.base}[${index}].Id`);
            }

            const hiddenOBIdInput = row.querySelector(this.selectors.hiddenOBIdClass);
            if (hiddenOBIdInput) {
                hiddenOBIdInput.setAttribute("name", `${nameAttributes.base}[${index}].OBId`);
            }
        });
    }

    addEventListeners() {
        if (this.selectors.tableBody) {
            this.selectors.tableBody.addEventListener("input", this.handleTableInput);
            this.selectors.tableBody.addEventListener("click", this.handleTableClick);
        }
        if (this.selectors.addRowBtn) {
            this.selectors.addRowBtn.addEventListener("click", this.handleAddRow);
        }
    }

    handleAddRow(e) {
        e.preventDefault();
        if (this.mode === 'delete' || this.mode === 'detail') return;
        this.renderRow();
        this.updateRowIndices();
    }

    handleTableInput(e) {
        // Add validation logic if needed
    }

    handleTableClick(e) {
        const targetBtn = e.target.closest('button, a');
        if (!targetBtn) return;

        if (targetBtn.classList.contains('delete-row')) {
            e.preventDefault();
            const row = targetBtn.closest("tr");
            const coaId = row.querySelector(this.selectors.coaSelectClass)?.value;
            if (coaId) {
                this.selectedCOAIds.delete(parseInt(coaId));
            }
            row.remove();
            this.updateRowIndices();
        } else if (targetBtn.classList.contains('edit-row')) {
            e.preventDefault();
            const row = targetBtn.closest("tr");
            if (this.editTemplateType === 'Template_1') {
                this.enterEditModeTemplate1(row);
            } else if (this.editTemplateType === 'Template_3') {
                this.enterEditModeTemplate3(row);
            } else {
                this.enableRow(row);
            }
        } else if (targetBtn.classList.contains('save-row')) {
            e.preventDefault();
            const row = targetBtn.closest("tr");
            if (this.editTemplateType === 'Template_1') {
                this.saveRowTemplate1(row);
            } else if (this.editTemplateType === 'Template_3') {
                this.saveRowTemplate3(row);
            }
        } else if (targetBtn.classList.contains('check-row')) {
            e.preventDefault();
            const row = targetBtn.closest("tr");
            if (this.editTemplateType === 'Template_3') {
                this.saveRowTemplate3(row);
            }
        } else if (targetBtn.classList.contains('reset-row')) {
            e.preventDefault();
            const row = targetBtn.closest("tr");
            if (this.editTemplateType === 'Template_1') {
                this.resetRowTemplate1(row);
            } else if (this.editTemplateType === 'Template_3') {
                this.resetRowTemplate3(row);
            }
        }
    }

    disableRow(row) {
        // Remove old hidden inputs if any
        const oldHiddenInputs = row.querySelectorAll(`.${this.hiddenDisabledInputClass}`);
        oldHiddenInputs.forEach(h => h.remove());

        const inputs = row.querySelectorAll('input, select');
        inputs.forEach(input => {
            let val = input.value;
            if (input.tomselect) {
                val = input.tomselect.getValue();
            }

            // If field has a value, create a hidden input
            if (input.name && val !== "") {
                const hiddenInput = document.createElement('input');
                hiddenInput.type = 'hidden';
                hiddenInput.name = input.name;
                hiddenInput.value = val;
                hiddenInput.classList.add(this.hiddenDisabledInputClass);
                const td = input.closest('td') || row;
                td.appendChild(hiddenInput);
            }

            if (input.tomselect) {
                input.tomselect.disable();
            }
            input.disabled = true;
        });
    }

    enableRow(row) {
        const hiddenInputs = row.querySelectorAll(`.${this.hiddenDisabledInputClass}`);
        hiddenInputs.forEach(hi => hi.remove());

        const inputs = row.querySelectorAll('input, select');
        inputs.forEach(input => {
            input.disabled = false;
            if (input.tomselect) {
                input.tomselect.enable();
            }
        });
    }

    // Template_1 methods
    enterEditModeTemplate1(row) {
        this.enableRow(row);
        if (!row.dataset.originalData) {
            row.dataset.originalData = JSON.stringify({
                coaId: row.querySelector(this.selectors.coaSelectClass).tomselect?.getValue() || "",
                tranType: row.querySelector(this.selectors.tranTypeSelectClass).tomselect?.getValue() || "",
                openBalance: row.querySelector(this.selectors.openBalanceInputClass).value
            });
        }
        const editBtn = row.querySelector('.edit-row');
        if (editBtn) {
            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
            editBtn.classList.remove('edit-row');
            editBtn.classList.add('save-row', 'btn-success');
            editBtn.classList.remove('btn-white');
        }

        const resetOption = row.querySelector('.dropdown-item.reset-row');
        if (resetOption) resetOption.remove();
    }

    saveRowTemplate1(row) {
        this.disableRow(row);
        const saveBtn = row.querySelector('.save-row');
        if (saveBtn) {
            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
            saveBtn.classList.remove('save-row', 'btn-success');
            saveBtn.classList.add('edit-row', 'btn-white');
        }

        const dropdownMenu = row.querySelector('.dropdown-menu');
        if (dropdownMenu && !row.querySelector('.dropdown-item.reset-row')) {
            const resetOptionHTML = `
                <a class="dropdown-item reset-row" href="#">
                    <i class="bi-arrow-clockwise dropdown-item-icon"></i> Reset
                </a>
            `;
            dropdownMenu.insertAdjacentHTML('beforeend', resetOptionHTML);
        }
    }

    resetRowTemplate1(row) {
        const originalData = JSON.parse(row.dataset.originalData || '{}');
        row.querySelector(this.selectors.coaSelectClass).tomselect.setValue(originalData.coaId);
        row.querySelector(this.selectors.tranTypeSelectClass).tomselect.setValue(originalData.tranType);
        row.querySelector(this.selectors.openBalanceInputClass).value = originalData.openBalance;
        this.disableRow(row);
        delete row.dataset.originalData;

        const resetOption = row.querySelector('.dropdown-item.reset-row');
        if (resetOption) resetOption.remove();
    }

    // Template_3 methods
    enterEditModeTemplate3(row) {
        this.enableRow(row);
        if (!row.dataset.originalData) {
            row.dataset.originalData = JSON.stringify({
                coaId: row.querySelector(this.selectors.coaSelectClass).tomselect?.getValue() || "",
                tranType: row.querySelector(this.selectors.tranTypeSelectClass).tomselect?.getValue() || "",
                openBalance: row.querySelector(this.selectors.openBalanceInputClass).value
            });
        }
        const editBtn = row.querySelector('.edit-row');
        if (editBtn) {
            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
            editBtn.classList.remove('edit-row', 'btn-primary');
            editBtn.classList.add('check-row', 'btn-success');
        }

        const resetBtn = row.querySelector('.reset-row');
        if (resetBtn) resetBtn.remove();
    }

    saveRowTemplate3(row) {
        this.disableRow(row);
        const checkBtn = row.querySelector('.check-row');
        if (checkBtn) {
            checkBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
            checkBtn.classList.remove('check-row', 'btn-success');
            checkBtn.classList.add('edit-row', 'btn-primary');
        }

        if (!row.querySelector('.reset-row')) {
            const resetBtnHTML = `
                <button class="btn btn-warning btn-sm reset-row ms-1" type="button">
                    <i class="bi-arrow-clockwise"></i>
                </button>
            `;
            const deleteBtn = row.querySelector('.delete-row');
            if (deleteBtn) {
                deleteBtn.insertAdjacentHTML('afterend', resetBtnHTML);
            }
        }
    }

    resetRowTemplate3(row) {
        const originalData = JSON.parse(row.dataset.originalData || '{}');
        row.querySelector(this.selectors.coaSelectClass).tomselect.setValue(originalData.coaId);
        row.querySelector(this.selectors.tranTypeSelectClass).tomselect.setValue(originalData.tranType);
        row.querySelector(this.selectors.openBalanceInputClass).value = originalData.openBalance;
        this.disableRow(row);

        const resetBtn = row.querySelector('.reset-row');
        if (resetBtn) resetBtn.remove();

        const editBtn = row.querySelector('.edit-row');
        if (editBtn && editBtn.classList.contains('btn-success')) {
            editBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
            editBtn.classList.remove('btn-success');
            editBtn.classList.add('btn-primary');
        }

        delete row.dataset.originalData;
    }

    calculateTotals() {
        // Not needed for OB
    }
}
