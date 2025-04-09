// File: /Frontentjs/dynamicComponents/simpleDynamicTable.js

export class OSDynamicTable {
    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
        // --- Selectors and Elements ---
        this.selectors = {
            productTableBody: document.getElementById(tableBodyId),
            addRowBtn: document.getElementById(addButtonId),

            productSelectClass: ".product-select",
            quantityInputClass: ".product-qty",
        };

        // --- Dynamic Name Attributes ---
        this.nameAttributeOptionObject = nameAttributeOptionObject;

        // --- Possible Field Names ---
        this.stockFields = options.stockFields || ['stock', 'currentstock', 'quantityinstock'];
        this.quantityFields = options.quantityFields || ['quantity', 'qty'];

        // --- Data and Mode ---
        this.productTableBody = this.selectors.productTableBody;
        this.addRowBtn = this.selectors.addRowBtn;

        this.productsData = options.productsData || [];
        this.prefilledRows = options.prefilledRows || [];
        this.isEditMode = options.isEditMode || false;

        this.isDiscountAllowed = false; // OS and OB do not have discounts

        this.selectedProductIds = new Set();

        // Mode handling
        this.mode = options.mode || 'create'; // Modes: create, edit, delete, detail

        // Edit Template Type
        this.editTemplateType = options.editTemplateType || 'Default'; // Templates: Template_1, Template_2, Template_3, Default

        // Bind methods to ensure correct 'this' context
        this.handleAddRow = this.handleAddRow.bind(this);
        this.handleTableInput = this.handleTableInput.bind(this);
        this.handleTableClick = this.handleTableClick.bind(this);

        // Initialize the table
        this.initialize();
    }

    initialize() {
        this.renderInitialRows();
        this.addEventListeners();

        // Adjust UI based on mode
        if (this.mode === 'delete' || this.mode === 'detail') {
            if (this.addRowBtn) {
                this.addRowBtn.style.display = 'none';
            }
        }
    }

    // --- Update the table with new data ---
    updateData(newOptions, newNameAttributeOptionObject) {
        // Remove existing event listeners to prevent duplication
        this.removeEventListeners();

        // Update options and name attributes
        this.productsData = newOptions.productsData || this.productsData;
        this.prefilledRows = newOptions.prefilledRows || [];
        this.isEditMode = newOptions.isEditMode || this.isEditMode;

        this.mode = newOptions.mode || this.mode;
        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

        // Update name attributes if provided
        if (newNameAttributeOptionObject) {
            this.nameAttributeOptionObject = newNameAttributeOptionObject;
        }

        // Reset the table with new prefilled data
        this.productTableBody.innerHTML = "";
        this.selectedProductIds.clear();

        // Re-initialize the table
        this.initialize();
    }

    // --- Remove event listeners ---
    removeEventListeners() {
        // Remove input event listener
        this.productTableBody.removeEventListener("input", this.handleTableInput);

        // Remove click event listener
        this.productTableBody.removeEventListener("click", this.handleTableClick);

        // Remove Add Row button event listener
        if (this.addRowBtn) {
            this.addRowBtn.removeEventListener("click", this.handleAddRow);
        }
    }

    // --- Normalize keys for dynamic property access ---
    normalizeKeys(obj) {
        debugger;
        const normalized = {};
        for (const key in obj) {
            normalized[key.toLowerCase()] = obj[key];
        }
        return normalized;
    }

    renderInitialRows() {
        console.log(this.productTableBody)
        this.productTableBody.innerHTML = ""; // Clear existing rows

        if (this.prefilledRows.length > 0) {
            this.prefilledRows.forEach((row) => {
                this.renderRow(row);
                const normalizedRow = this.normalizeKeys(row);
                this.selectedProductIds.add(normalizedRow['productid']);
            });
        } else if (this.mode === 'create') {
            this.renderRow(); // Render a blank row for Create Mode
        }

        // Update indices after rendering all rows
        this.updateRowIndices();
    }

    renderRow(data = null) {
        debugger;
        const normalizedData = data ? this.normalizeKeys(data) : {};
        let pQuantity = normalizedData['osdtlqty'] ?? 0;
        const productOptions = this.productsData
            .map(
                (product) =>
                    `<option value="${product.value}" ${normalizedData['productId'] == product.value
                        ? "selected"
                        : ""
                    }>${product.text}</option>`
            )
            .join("");

        const rowHTML = `
        <tr>
            <td>
                <div class="tom-select-custom">
                    <select class="js-select form-select product-select">
                        <option value="">--- Select Product ---</option>
                        ${productOptions}
                    </select>
                </div>
            </td>
            <td>
                <input type="number" class="form-control product-qty" min="0" value="${pQuantity}">
            </td>
            <td>
                ${this.getActionButtonHTML(data)}
            </td>
        </tr>
        `;
        this.productTableBody.insertAdjacentHTML("beforeend", rowHTML);

        const row = this.productTableBody.lastElementChild;

        // Update row indices immediately to set name attributes before disabling
        this.updateRowIndices();

        const productSelect = row.querySelector(this.selectors.productSelectClass);
        new TomSelect(productSelect, {
            placeholder: "--- Select Product ---",
            maxItems: 1,
            onChange: (value) => this.handleProductSelection(row, value, data),
        });

        if (data) {
            productSelect.tomselect.setValue(normalizedData['productid']); // Pre-select value
        } else {
            // Disable quantity input initially
            const quantityInput = row.querySelector(this.selectors.quantityInputClass);
            quantityInput.disabled = true;
        }

        // Adjust row based on mode and data
        if (data) {
            // Existing row
            if (this.mode === 'edit') {
                this.adjustRowForEditTemplate(row);
            } else if (this.mode === 'delete' || this.mode === 'detail') {
                this.disableRow(row);
            }
        } else {
            // New row
            if (this.mode === 'delete' || this.mode === 'detail') {
                this.disableRow(row);
            }
        }
    }

    // --- Get action button HTML based on mode and template ---
    getActionButtonHTML(data) {
        if (data) {
            if (this.mode === 'edit') {
                if (this.editTemplateType === 'Template_1') {
                    // Use menu-based action buttons for Template_1
                    return `
                    <div class="btn-group" role="group">
                        <button type="button" class="btn btn-white btn-sm edit-row">
                            <i class="bi-pencil-fill me-1"></i> Edit
                        </button>
                        <!-- Button Group -->
                        <div class="btn-group">
                            <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty" data-bs-toggle="dropdown" aria-expanded="false"></button>
                            <div class="dropdown-menu dropdown-menu-end mt-1">
                                <a class="dropdown-item delete-row" href="#">
                                    <i class="bi-trash dropdown-item-icon"></i> Delete
                                </a>
                                <!-- Reset option will appear after editing -->
                                <!-- Additional menu items can be added here -->
                            </div>
                        </div>
                        <!-- End Button Group -->
                    </div>
                    `;
                } else if (this.editTemplateType === 'Template_2') {
                    return `
                        <button class="btn btn-primary btn-sm edit-row"><i class="bi-pencil"></i></button>
                        <button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>
                    `;
                } else if (this.editTemplateType === 'Template_3') {
                    return `
                        <button class="btn btn-success btn-sm edit-row" type="button"><i class="bi-pencil"></i></button>
                        <button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>
                    `;
                } else {
                    // Default
                    return '<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
                }
            } else if (this.mode === 'delete' || this.mode === 'detail') {
                return ''; // No button
            }
        }
        // Default is 'Delete' button
        return '<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
    }

    // --- Adjust row based on edit template ---
    adjustRowForEditTemplate(row) {
        if (this.editTemplateType === 'Template_1') {
            this.disableRow(row);
        } else if (this.editTemplateType === 'Template_2') {
            this.disableRow(row);
        } else if (this.editTemplateType === 'Template_3') {
            this.disableRow(row);
        } else {
            // Default behavior: row is enabled
            // No action needed
        }
    }

    // --- Handle product selection ---
    handleProductSelection(row, productId, existingData = null) {
        const previousProductId = row.dataset.previousProductId;
        
        if (previousProductId) {
            this.selectedProductIds.delete(parseInt(previousProductId));
        }

        const quantityInput = row.querySelector(this.selectors.quantityInputClass);

        if (!productId || productId === "0") {
            // Disable quantity input when no product is selected
            quantityInput.disabled = true;
            quantityInput.value = "";
            row.dataset.previousProductId = null;
            return;
        }

        if (this.selectedProductIds.has(parseInt(productId))) {
            alert("This product is already selected. Please choose another.");
            row.querySelector(this.selectors.productSelectClass).tomselect.clear();
            return;
        }

        this.selectedProductIds.add(parseInt(productId));
        row.dataset.previousProductId = productId;

        // Enable quantity input when a product is selected
        quantityInput.disabled = false;



        //const product = this.productsData.find((p) => p.value == productId) || {};
        //const productNormalized = this.normalizeKeys(product);

        //this.updateRowValues(row, productNormalized, existingData);
    }

    updateRowIndices() {
        const nameAttributes = this.nameAttributeOptionObject;
        this.productTableBody.querySelectorAll("tr").forEach((row, index) => {
            row.setAttribute("data-index", index);
            console.log(row)

            row.querySelector(this.selectors.productSelectClass)
                .setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.productId}`);

            row.querySelector(this.selectors.quantityInputClass)
                .setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.quantity}`);
        });
    }

    addEventListeners() {
        // Handle input events within the table
        this.productTableBody.addEventListener("input", this.handleTableInput);

        // Handle click events within the table
        this.productTableBody.addEventListener("click", this.handleTableClick);

        // Handle Add Row button
        if (this.addRowBtn) {
            this.addRowBtn.addEventListener("click", this.handleAddRow);
        }
    }

    // --- Handle Add Row Button Click ---
    handleAddRow(e) {
        e.preventDefault();
        if (this.mode === 'delete' || this.mode === 'detail') {
            return;
        }
        this.renderRow();
        this.updateRowIndices();
    }

    // --- Handle Table Input Events ---
    handleTableInput(e) {
        if (e.target.classList.contains(this.selectors.quantityInputClass.substring(1))) {
            const row = e.target.closest("tr");
            const qty = parseFloat(e.target.value) || 0;

            // You can add validation here if needed

            // No totals to calculate in OS and OB
        }
    }

    // --- Handle Table Click Events ---
    handleTableClick(e) {
        const targetBtn = e.target.closest('button, a');
        if (!targetBtn) return;

        if (targetBtn.classList.contains('delete-row')) {
            e.preventDefault();
            const row = targetBtn.closest("tr");
            const productId = row.querySelector(this.selectors.productSelectClass).value;
            this.selectedProductIds.delete(parseInt(productId));
            row.remove();
            this.updateRowIndices();
        } else if (targetBtn.classList.contains('edit-row')) {
            e.preventDefault();
            const row = targetBtn.closest("tr");
            if (this.editTemplateType === 'Template_1') {
                this.enterEditModeTemplate1(row);
            } else if (this.editTemplateType === 'Template_3') {
                this.enterEditModeTemplate3(row);
            } else if (this.editTemplateType === 'Template_2') {
                this.enableRow(row);
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
        const inputs = row.querySelectorAll('input, select');

        inputs.forEach(input => {
            if (input.name) {
                // Create a hidden input with the same name and value
                const hiddenInput = document.createElement('input');
                hiddenInput.type = 'hidden';
                hiddenInput.name = input.name;
                hiddenInput.value = input.value;
                hiddenInput.classList.add('hidden-disabled-input');
                // Store the original name for index updates
                hiddenInput.setAttribute('data-original-name', input.name.split('.').pop());
                // Append the hidden input to the row
                row.appendChild(hiddenInput);
            }

            input.disabled = true;

            if (input.classList.contains(this.selectors.productSelectClass.substring(1))) {
                if (input.tomselect) {
                    input.tomselect.disable();
                }
            }
        });
    }

    enableRow(row) {
        const inputs = row.querySelectorAll('input, select');

        inputs.forEach(input => {
            input.disabled = false;

            if (input.classList.contains(this.selectors.productSelectClass.substring(1))) {
                if (input.tomselect) {
                    input.tomselect.enable();
                }
            }
        });

        // Remove hidden inputs to prevent duplication
        const hiddenInputs = row.querySelectorAll('.hidden-disabled-input');
        hiddenInputs.forEach(hiddenInput => {
            hiddenInput.remove();
        });
    }

    // --- Template_1 Methods ---
    enterEditModeTemplate1(row) {
        this.enableRow(row);

        // Store original data if not already stored
        if (!row.dataset.originalData) {
            row.dataset.originalData = JSON.stringify({
                productId: row.querySelector(this.selectors.productSelectClass).value,
                quantity: row.querySelector(this.selectors.quantityInputClass).value,
            });
        }

        // Change Edit button to Save button
        const editBtn = row.querySelector('.edit-row');
        if (editBtn) {
            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
            editBtn.classList.remove('edit-row');
            editBtn.classList.add('save-row');
            editBtn.classList.add('btn-success');
            editBtn.classList.remove('btn-white');
        }

        // Remove existing Reset option from menu if any
        const resetOption = row.querySelector('.dropdown-item.reset-row');
        if (resetOption) {
            resetOption.remove();
        }
    }

    saveRowTemplate1(row) {
        this.disableRow(row);

        // Change Save button back to Edit button
        const saveBtn = row.querySelector('.save-row');
        if (saveBtn) {
            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
            saveBtn.classList.remove('save-row');
            saveBtn.classList.add('edit-row');
            saveBtn.classList.remove('btn-success');
            saveBtn.classList.add('btn-white');
        }

        // Add Reset option to the dropdown menu
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
        // Retrieve original data
        const originalData = JSON.parse(row.dataset.originalData || '{}');

        // Reset values
        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;

        this.disableRow(row);

        // Remove stored original data
        delete row.dataset.originalData;

        // Remove Reset option from menu
        const resetOption = row.querySelector('.dropdown-item.reset-row');
        if (resetOption) {
            resetOption.remove();
        }
    }

    // --- Template_3 Methods ---
    enterEditModeTemplate3(row) {
        this.enableRow(row);

        // Store original data if not already stored
        if (!row.dataset.originalData) {
            row.dataset.originalData = JSON.stringify({
                productId: row.querySelector(this.selectors.productSelectClass).value,
                quantity: row.querySelector(this.selectors.quantityInputClass).value,
            });
        }

        // Change Edit button to Check button
        const editBtn = row.querySelector('.edit-row');
        if (editBtn) {
            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
            editBtn.classList.remove('edit-row');
            editBtn.classList.add('check-row');
            editBtn.classList.remove('btn-primary');
            editBtn.classList.add('btn-success');
        }

        // Remove Reset button if it exists
        const resetBtn = row.querySelector('.reset-row');
        if (resetBtn) {
            resetBtn.remove();
        }
    }

    saveRowTemplate3(row) {
        this.disableRow(row);

        // Change Check button back to Edit button
        const checkBtn = row.querySelector('.check-row');
        if (checkBtn) {
            checkBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
            checkBtn.classList.remove('check-row');
            checkBtn.classList.add('edit-row');
            checkBtn.classList.remove('btn-success');
            checkBtn.classList.add('btn-primary');
        }

        // Add Reset button
        if (!row.querySelector('.reset-row')) {
            const resetBtnHTML = `
                <button class="btn btn-warning btn-sm reset-row ms-1">
                    <i class="bi-arrow-clockwise"></i>
                </button>
            `;
            const deleteBtn = row.querySelector('.delete-row');
            deleteBtn.insertAdjacentHTML('afterend', resetBtnHTML);
        }
    }

    resetRowTemplate3(row) {
        // Retrieve original data
        const originalData = JSON.parse(row.dataset.originalData || '{}');

        // Reset values
        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;

        this.disableRow(row);

        // Remove Reset button
        const resetBtn = row.querySelector('.reset-row');
        if (resetBtn) {
            resetBtn.remove();
        }

        // Change Edit button back if it was altered
        const editBtn = row.querySelector('.edit-row');
        if (editBtn && editBtn.classList.contains('btn-success')) {
            editBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
            editBtn.classList.remove('btn-success');
            editBtn.classList.add('btn-primary');
        }

        // Remove stored original data
        delete row.dataset.originalData;
    }

    // --- Template_2 Methods ---
    // If needed, you can add specific methods for Template_2 similar to Template_1 and Template_3

    // --- General Methods ---

    calculateTotals() {
        // Not required for OS and OB as they don't have totals
    }
}
