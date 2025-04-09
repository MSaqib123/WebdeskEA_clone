//=================================================================================
//================================== V5 ===========================================
//=================================================================================
export class AdvancedDynamicTable {
    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
        this.selectors = {
            productTableBody: document.getElementById(tableBodyId),
            addRowBtn: document.getElementById(addButtonId),

            subtotalId: options.subtotalId || null,
            totalId: options.totalId || null,
            discountId: options.discountId || null,

            productSelectClass: ".product-select",
            currentStockClass: ".current-stock",
            unitPriceClass: ".unit-price",
            unitPriceInputClass: ".unit-price-input",
            quantityInputClass: ".product-qty",
            productTotalClass: ".product-total",
            productTotalInputClass: ".product-total-input",

            grandTotalId: "#grandTotal",
            totalQuantityId: "#totalQuantity",
            totalPerPiecePriceId: "#totalPerPiecePrice",
            totalUnitPriceId: "#totalUnitPrice",

            // Hidden for Subtotal/Total
            subtotalHiddenClass: ".subtotal-hidden",
            subtotalVisibleClass: ".subtotal-visible",
            totalHiddenClass: ".total-hidden",
            totalVisibleClass: ".total-visible",

            // PriceAfterVAT
            productAfterVatClass: ".product-after-vat",
            productAfterVatInputClass: ".product-after-vat-input",

            // Tax sub-rows
            taxRowClass: ".tax-row",
            taxMasterSelectClass: ".tax-master-select",
            taxAmountClass: ".tax-amount",
            taxAmountInputClass: ".tax-amount-input",
            taxSODtlIdClass: ".tax-sodtlid-input",  // rename if needed (just a reference)
            taxSOIdClass: ".tax-soid-input",        // rename if needed
            taxAfterTaxAmtClass: ".tax-aftertaxamount-input",

            // Hidden ID fields
            sodtlIdInputClass: ".sodtl-id-input",
            taxIdInputClass: ".tax-id-input"
        };

        // --- Options and config ---
        // This is key: allow user to pass dynamic "Tax Detail" and "VAT Breakdown" collection names:
        this.taxDetailCollectionName = options.taxDetailCollectionName || "DTLTaxDtos";
        this.vatBreakdownCollectionName = options.vatBreakdownCollectionName || "VATBreakdownDtos";

        // NEW: for line-level foreign keys
        this.detailIdFieldName = options.detailIdFieldName || "SODtlId";  // e.g. "SIDtlId"
        this.docIdFieldName = options.docIdFieldName || "SOId";          // e.g. "SIId"

        this.nameAttributeOptionObject = nameAttributeOptionObject;

        // Field name possibilities
        this.unitPriceFields = options.unitPriceFields || ["unitprice", "productprice", "price"];
        this.stockFields = options.stockFields || ["stock", "currentstock", "quantityinstock"];
        this.quantityFields = options.quantityFields || ["quantity", "qty"];
        this.totalFields = options.totalFields || ["total", "totalprice"];

        // DOM references
        this.productTableBody = this.selectors.productTableBody;
        this.addRowBtn = this.selectors.addRowBtn;

        // Data
        this.productsData = options.productsData || [];
        this.prefilledRows = options.prefilledRows || [];
        this.isEditMode = options.isEditMode || false;



        // Subtotal/Total
        this.subtotalId = this.selectors.subtotalId;
        this.totalId = this.selectors.totalId;
        this.discountId = this.selectors.discountId;
        this.isDiscountAllowed = options.isDiscountAllowed || false;

        // Distinguish which doc type mode: e.g. create/edit/delete/detail
        this.mode = options.mode || "create";
        this.editTemplateType = options.editTemplateType || "Default";

        // For multi-VAT
        this.taxMasterData = options.taxMasterData || [];
        this.prefilledTaxMaster = options.prefilledTaxMaster || [];

        this.selectedProductIds = new Set();
        this.currentMaxSODtlId = 0; // track largest detail id

        // Bind methods
        this.handleAddRow = this.handleAddRow.bind(this);
        this.handleTableInput = this.handleTableInput.bind(this);
        this.handleTableClick = this.handleTableClick.bind(this);
        this.handleDiscountInput = this.handleDiscountInput.bind(this);

        this.initialize();
    }

    initialize() {
        this.findCurrentMaxSODtlId();
        this.renderInitialRows();
        this.addEventListeners();
        this.calculateTotals();

        // Hide the main "Add" button if in delete/detail mode
        if (this.mode === "delete" || this.mode === "detail") {
            if (this.addRowBtn) {
                this.addRowBtn.style.display = "none";
            }
        }
    }

    findCurrentMaxSODtlId() {
        let maxId = 0;
        this.prefilledRows.forEach(row => {
            if (row.id && row.id > maxId) {
                maxId = row.id;
            }
        });
        this.currentMaxSODtlId = maxId;
    }

    // If you re-fetch data or want to reload
    updateData(newOptions, newNameAttributeOptionObject) {
        this.removeEventListeners();

        this.productsData = newOptions.productsData || this.productsData;
        this.prefilledRows = newOptions.prefilledRows || [];
        this.isEditMode = newOptions.isEditMode || this.isEditMode;

        this.subtotalId = newOptions.subtotalId || this.subtotalId;
        this.totalId = newOptions.totalId || this.totalId;
        this.discountId = newOptions.discountId || this.discountId;
        this.isDiscountAllowed = newOptions.isDiscountAllowed !== undefined ? newOptions.isDiscountAllowed : this.isDiscountAllowed;

        this.mode = newOptions.mode || this.mode;
        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

        // Re-config dynamic naming
        this.taxDetailCollectionName = newOptions.taxDetailCollectionName || this.taxDetailCollectionName;
        this.vatBreakdownCollectionName = newOptions.vatBreakdownCollectionName || this.vatBreakdownCollectionName;

        // New tax data or existing
        this.taxMasterData = newOptions.taxMasterData || this.taxMasterData;
        this.prefilledTaxMaster = newOptions.prefilledTaxMaster || this.prefilledTaxMaster;

        if (newNameAttributeOptionObject) {
            this.nameAttributeOptionObject = newNameAttributeOptionObject;
        }

        // Clear table
        this.productTableBody.innerHTML = "";
        this.selectedProductIds.clear();

        this.findCurrentMaxSODtlId();
        this.initialize();
    }

    removeEventListeners() {
        this.productTableBody.removeEventListener("input", this.handleTableInput);
        this.productTableBody.removeEventListener("click", this.handleTableClick);

        if (this.addRowBtn) {
            this.addRowBtn.removeEventListener("click", this.handleAddRow);
        }

        if (this.isDiscountAllowed && this.discountId) {
            const discountInput = document.getElementById(this.discountId);
            if (discountInput) {
                discountInput.removeEventListener("input", this.handleDiscountInput);
            }
        }
    }

    // Utility functions
    findFieldName(dataObj, possibleFields) {
        for (let field of possibleFields) {
            if (field.toLowerCase() in dataObj) {
                return field.toLowerCase();
            }
        }
        return null;
    }
    normalizeKeys(obj) {
        const normalized = {};
        for (const key in obj) {
            normalized[key.toLowerCase()] = obj[key];
        }
        return normalized;
    }

    // ------------------------------------------------------------------------
    // Render the prefilled or initial rows
    // ------------------------------------------------------------------------
    renderInitialRows() {
        this.productTableBody.innerHTML = "";
        if (this.prefilledRows.length > 0) {
            this.prefilledRows.forEach((row) => {
                const productRow = this.renderRow(row);
                // If we have associated tax lines
                if (row.Id) {
                    const matchingTaxes = this.prefilledTaxMaster.filter(t => Number(t.SODtlId) === Number(row.Id) ||
                        Number(t.SIDtlId) === Number(row.Id) ||
                        Number(t.PODtlId) === Number(row.Id) ||
                        Number(t.PRDtlId) === Number(row.Id) ||
                        Number(t.SRDtlId) === Number(row.Id) ||
                        Number(t.PIDtlId) === Number(row.Id));
                    if (matchingTaxes.length > 0) {
                        matchingTaxes.forEach(taxDto => {
                            this.renderTaxRow(productRow, taxDto);
                        });
                    }
                }
            });
        } else if (this.mode === "create") {
            this.renderRow(null);
        }

        this.updateRowIndices();
    }

    // ------------------------------------------------------------------------
    // Render a single product row
    // ------------------------------------------------------------------------
    renderRow(data = null) {
        const normalizedData = data ? this.normalizeKeys(data) : {};
        let sodtlIdValue = data && typeof data.id !== "undefined" ? data.id : ++this.currentMaxSODtlId;

        let pQuantity = normalizedData["quantity"] ?? 0;
        const productOptions = this.productsData
            .map(p =>
                `<option value="${p.value}" ${normalizedData["productid"] == p.value ? "selected" : ""}>
                    ${p.text}
                 </option>`
            ).join("");

        const rowHTML = `
<tr>
    <!-- Hidden ID for detail row -->
    <td style="display:none;">
        <input type="hidden" class="sodtl-id-input" value="${sodtlIdValue}">
    </td>
    <td>
        <div class="tom-select-custom">
            <select class="js-select form-select product-select">
                <option value="">--- Select Product ---</option>
                ${productOptions}
            </select>
        </div>
    </td>
    <td class="current-stock">${normalizedData["stock"] ?? "--"}</td>
    <td class="unit-price">
        <input type="input" class="form-control unit-price-input" value="${(normalizedData["unitprice"] ?? 0).toFixed(2)}">
    </td>
    <td>
        <input type="number" class="form-control product-qty" min="0" value="${pQuantity}">
    </td>
    <td class="product-total">
        ${(normalizedData["total"] ?? 0).toFixed(2)}
        <input type="hidden" class="product-total-input" value="${(normalizedData["total"] ?? 0).toFixed(2)}">
    </td>
    <td class="product-after-vat">
        ${(normalizedData["afterVAT"] ?? 0).toFixed(2)}
        <input type="hidden" class="product-after-vat-input"
               value="${(normalizedData["afterVAT"] ?? 0).toFixed(2)}">
    </td>
    <td>
        ${this.getActionButtonHTML(data)}
        <button type="button" class="btn btn-soft-info btn-sm add-vat-btn">
            <i class="bi-plus-circle"></i> VAT
        </button>
    </td>
</tr>
`;
        this.productTableBody.insertAdjacentHTML("beforeend", rowHTML);
        const row = this.productTableBody.lastElementChild;
        this.updateRowIndices();

        // Initialize TomSelect
        const productSelect = row.querySelector(".product-select");
        new TomSelect(productSelect, {
            placeholder: "--- Select Product ---",
            maxItems: 1,
            onChange: (value) => this.handleProductSelection(row, value, data)
        });

        if (data) {
            productSelect.tomselect.setValue(normalizedData["productid"]);
        } else {
            // brand-new => disable quantity until product chosen
            row.querySelector(".product-qty").disabled = true;
        }

        // If delete/detail mode => disable
        if (this.mode === "delete" || this.mode === "detail") {
            this.disableRow(row);
            // Hide "Add VAT" button
            const addVatBtn = row.querySelector(".add-vat-btn");
            if (addVatBtn) addVatBtn.style.display = "none";
        } else if (data && this.mode === "edit") {
            this.adjustRowForEditTemplate(row);
        }

        return row;
    }

    // ------------------------------------------------------------------------
    // Render a single tax sub-row
    // ------------------------------------------------------------------------
    renderTaxRow(productRow, taxData = null) {
        console.log(productRow, 'pr');
        console.log(taxData, 'taxData');
        // Insert after product row
        let nextRow = productRow.nextElementSibling;
        while (nextRow && nextRow.classList.contains("tax-row")) {
            nextRow = nextRow.nextElementSibling;
        }

        const row = document.createElement("tr");
        row.classList.add("tax-row");
        row.style.backgroundColor = "#f1f8e9";
        row.style.borderLeft = "4px solid #8bc34a";
        row.style.fontSize = "0.95rem";

        const parentRowId = productRow.querySelector(".sodtl-id-input").value || 0;

        row.innerHTML = `
<td style="display:none;">
    <input type="hidden" class="tax-id-input" value="${taxData?.Id || 0}">
</td>
<td colspan="4">
    <div class="tom-select-custom d-flex justify-content-end">
        <select class="form-select tax-master-select w-50">
            <option value="">-- Select Tax --</option>
            ${this.taxMasterData.map(t => `
                <option value="${t.Id}">${t.TaxName} (${t.TaxValue}${t.IsPercentage ? "%" : ""})</option>
            `).join("")}
        </select>
        <input type="hidden" class="tax-sodtlid-input" value="${taxData?.SODtlId || taxData?.SIDtlId || parentRowId}">
        <input type="hidden" class="tax-soid-input" value="${taxData?.SOId || taxData?.SIId || 0}">
        <input type="hidden" class="tax-aftertaxamount-input" value="0.00">
    </div>
</td>
<td>
    <span class="tax-amount">0.00</span>
    <input type="hidden" class="tax-amount-input" value="0.00">
</td>
<td>
    <button type="button" class="btn btn-danger btn-sm delete-row">
        <i class="bi-trash"></i>
    </button>
</td>
`;

        productRow.parentNode.insertBefore(row, nextRow);

        // TomSelect for the tax dropdown
        const taxSelect = row.querySelector(".tax-master-select");
        new TomSelect(taxSelect, {
            placeholder: "-- Select Tax --",
            maxItems: 1,
            onChange: (taxId) => this.handleTaxSelection(row, productRow, taxId)
        });

        // Fill existing data
        if (taxData) {
            if (taxData.TaxId) {
                taxSelect.tomselect.setValue(taxData.TaxId.toString());
            }
            row.querySelector(".tax-amount").textContent = (taxData.TaxAmount || 0).toFixed(2);
            row.querySelector(".tax-amount-input").value = (taxData.TaxAmount || 0).toFixed(2);

            if (typeof taxData.aftertaxamount !== "undefined") {
                row.querySelector(".tax-aftertaxamount-input").value = taxData.aftertaxamount.toFixed(2);
            }
        }

        // If in delete or detail => hide the delete button + disable
        if (this.mode === "delete" || this.mode === "detail") {
            const delBtn = row.querySelector(".delete-row");
            if (delBtn) delBtn.style.display = "none";
            this.disableRow(row);
        }

        return row;
    }

    // ------------------------------------------------------------------------
    // Returns the HTML for the "Actions" cell
    // ------------------------------------------------------------------------
    getActionButtonHTML(data) {
        if (data) {
            if (this.mode === "edit") {
                if (this.editTemplateType === "Template_1") {
                    return `
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-white btn-sm edit-row">
                                <i class="bi-pencil-fill me-1"></i> Edit
                            </button>
                            <div class="btn-group">
                                <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty"
                                    data-bs-toggle="dropdown" aria-expanded="false"></button>
                                <div class="dropdown-menu dropdown-menu-end mt-1">
                                    <a class="dropdown-item delete-row" href="#">
                                        <i class="bi-trash dropdown-item-icon"></i> Delete
                                    </a>
                                </div>
                            </div>
                        </div>
                    `;
                } else if (this.editTemplateType === "Template_2") {
                    return `
                        <button class="btn btn-primary btn-sm edit-row">
                            <i class="bi-pencil"></i>
                        </button>
                        <button class="btn btn-danger btn-sm delete-row">
                            <i class="bi-trash"></i>
                        </button>
                    `;
                } else if (this.editTemplateType === "Template_3") {
                    return `
                        <button class="btn btn-success btn-sm edit-row" type="button">
                            <i class="bi-pencil"></i>
                        </button>
                        <button class="btn btn-danger btn-sm delete-row">
                            <i class="bi-trash"></i>
                        </button>
                    `;
                } else {
                    return `<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>`;
                }
            } else if (this.mode === "delete" || this.mode === "detail") {
                return "";
            }
        }
        // brand-new row
        return `<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>`;
    }

    // If an "edit template" is chosen, we disable or etc.
    adjustRowForEditTemplate(row) {
        if (this.editTemplateType === "Template_1") {
            this.disableRow(row);
        } else if (this.editTemplateType === "Template_2") {
            this.disableRowForTemplate2(row);
        } else if (this.editTemplateType === "Template_3") {
            this.disableRow(row);
        }
    }

    // ------------------------------------------------------------------------
    // Handle product selection
    // ------------------------------------------------------------------------
    handleProductSelection(row, productId, existingData = null) {
        const prevId = row.dataset.previousProductId;
        if (prevId) {
            this.selectedProductIds.delete(parseInt(prevId));
        }

        const quantityInput = row.querySelector(".product-qty");
        if (!productId) {
            quantityInput.disabled = true;
            quantityInput.value = "";
            this.updateRowValues(row, null, existingData);
            row.dataset.previousProductId = null;
            this.calculateTotals();
            return;
        }
        if (this.selectedProductIds.has(parseInt(productId))) {
            alert("This product is already selected. Please choose another.");
            row.querySelector(".product-select").tomselect.clear();
            return;
        }

        this.selectedProductIds.add(parseInt(productId));
        row.dataset.previousProductId = productId;
        quantityInput.disabled = false;

        const product = this.productsData.find(p => p.value == productId) || {};
        const productNormalized = this.normalizeKeys(product);

        this.updateRowValues(row, productNormalized, existingData);
        this.calculateTotals();

        if (quantityInput) {
            quantityInput.focus();
        }
    }

    updateRowValues(row, data, existingData) {
        const normalizedData = data || {};
        const existingNormalizedData = existingData ? this.normalizeKeys(existingData) : {};
        const unitPriceCell = row.querySelector(".unit-price");
        const unitPriceInput = row.querySelector(".unit-price-input");
        const productTotalCell = row.querySelector(".product-total");
        const productTotalInput = row.querySelector(".product-total-input");
        const quantityInput = row.querySelector(".product-qty");
        const currentStockCell = row.querySelector(".current-stock");

        if (!data) {
            // Clear everything
            //unitPriceCell.firstChild.textContent = "0.00";
            unitPriceInput.value = "0.00";
            productTotalCell.firstChild.textContent = "0.00";
            productTotalInput.value = "0.00";
            quantityInput.value = 0;
            currentStockCell.textContent = "--";
            return;
        }

        // find matching fields
        const unitPriceField = this.findFieldName(normalizedData, this.unitPriceFields);
        const stockField = this.findFieldName(normalizedData, this.stockFields);
        const unitPrice = parseFloat(normalizedData[unitPriceField]) || 0;
        const stockVal = normalizedData[stockField] !== undefined ? normalizedData[stockField] : "--";

        //unitPriceCell.firstChild.textContent = unitPrice.toFixed(2);
        unitPriceInput.value = unitPrice.toFixed(2);
        currentStockCell.textContent = stockVal;

        let existingQty = parseFloat(existingNormalizedData[this.nameAttributeOptionObject.quantity?.toLowerCase()]) || 0;
        console.log(unitPrice * existingQty)
        if (!existingQty) {
            // default 0
            quantityInput.value = 0;
            productTotalCell.firstChild.textContent = "0.00";
            productTotalInput.value = "0.00";
        } else {
            quantityInput.value = existingQty;
            const calcTotal = unitPrice * existingQty;
            productTotalCell.firstChild.textContent = calcTotal.toFixed(2);
            productTotalInput.value = calcTotal.toFixed(2);
        }
    }

    // ------------------------------------------------------------------------
    // Handle selection of a tax row
    // ------------------------------------------------------------------------
    handleTaxSelection(taxRow, productRow, taxId) {
        if (!taxId) {
            taxRow.querySelector(".tax-amount").textContent = "0.00";
            taxRow.querySelector(".tax-amount-input").value = "0.00";
            this.calculateTotals();
            return;
        }

        const chosenTax = this.taxMasterData.find(t => t.Id == taxId);
        if (!chosenTax) return;

        // prevent duplicates
        let nextRow = productRow.nextElementSibling;
        while (nextRow && nextRow.classList.contains("tax-row")) {
            const siblingTaxSelect = nextRow.querySelector(".tax-master-select");
            if (siblingTaxSelect && siblingTaxSelect.value == taxId && nextRow !== taxRow) {
                alert("This Tax is already applied. Choose another.");
                taxRow.querySelector(".tax-master-select").tomselect.clear();
                return;
            }
            nextRow = nextRow.nextElementSibling;
        }

        const productTotal = parseFloat(productRow.querySelector(".product-total-input").value) || 0;
        let computedTax = 0;
        let afterTax = 0;
        const taxValue = parseFloat(chosenTax.TaxValue) || 0;

        // isPercentage, isExclusive, isInclusive
        if (chosenTax.IsPercentage) {
            if (chosenTax.IsExclusive) {
                computedTax = (productTotal * taxValue) / 100;
                afterTax = productTotal + computedTax;
            } else if (chosenTax.IsInclusive) {
                const net = productTotal / (1 + taxValue / 100);
                computedTax = productTotal - net;
                afterTax = productTotal;
            } else {
                // default
                computedTax = (productTotal * taxValue) / 100;
                afterTax = productTotal + computedTax;
            }
        } else {
            // fixed
            if (chosenTax.IsExclusive) {
                computedTax = taxValue;
                afterTax = productTotal + computedTax;
            } else if (chosenTax.IsInclusive) {
                if (productTotal >= taxValue) {
                    computedTax = taxValue;
                    afterTax = productTotal;
                } else {
                    computedTax = productTotal;
                    afterTax = productTotal;
                }
            } else {
                computedTax = taxValue;
                afterTax = productTotal + computedTax;
            }
        }

        taxRow.querySelector(".tax-amount").textContent = computedTax.toFixed(2);
        taxRow.querySelector(".tax-amount-input").value = computedTax.toFixed(2);

        const afterTaxInput = taxRow.querySelector(".tax-aftertaxamount-input");
        if (afterTaxInput) {
            afterTaxInput.value = afterTax.toFixed(2);
        }

        this.calculateTotals();
    }

    // ------------------------------------------------------------------------
    // updateRowIndices: dynamic for product lines and tax lines
    // ------------------------------------------------------------------------
    updateRowIndices() {
        const nameAttrs = this.nameAttributeOptionObject;
        this.globalTaxCounter = 0;

        let productIndex = 0;
        const allRows = this.productTableBody.querySelectorAll("tr");
        allRows.forEach(row => {
            if (!row.classList.contains("tax-row")) {
                // It's a product row
                row.setAttribute("data-product-index", productIndex);

                // main detail ID
                const sodtlIdInput = row.querySelector(".sodtl-id-input");
                if (sodtlIdInput) {
                    sodtlIdInput.name = `${nameAttrs.base}[${productIndex}].Id`;
                }
                // product select
                const productSelect = row.querySelector(".product-select");
                productSelect.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.productId}`;

                // quantity
                const qtyInput = row.querySelector(".product-qty");
                qtyInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.quantity}`;

                // unit price
                const upInput = row.querySelector(".unit-price-input");
                upInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.unitPrice}`;

                // total
                const totalInput = row.querySelector(".product-total-input");
                totalInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.total}`;

                // after VAT
                const afterVat = row.querySelector(".product-after-vat-input");
                afterVat.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.afterVAT}`;
                // or pass a custom "afterVatPropertyName" if you want

                productIndex++;
            } else {
                // It's a tax row => we use the dynamic “taxDetailCollectionName”
                const taxIdInput = row.querySelector(this.selectors.taxIdInputClass);
                const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
                const taxAmtInput = row.querySelector(this.selectors.taxAmountInputClass);
                const sodtlIdInput = row.querySelector(this.selectors.taxSODtlIdClass);
                const soIdInput = row.querySelector(this.selectors.taxSOIdClass);
                const afterTaxAmtInput = row.querySelector(this.selectors.taxAfterTaxAmtClass);

                taxIdInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].Id`;
                taxSelect.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].TaxId`;
                taxAmtInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].TaxAmount`;
                sodtlIdInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].${this.detailIdFieldName}`;
                soIdInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].${this.docIdFieldName}`;
                afterTaxAmtInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].AfterTaxAmount`;

                // If this is for SI => you might rename e.g. SIDtlId, SIId instead
                // You can pass that logic in your code or rename these input classes

                this.globalTaxCounter++;
            }
        });
    }

    // ------------------------------------------------------------------------
    // Add event listeners
    // ------------------------------------------------------------------------
    addEventListeners() {
        this.productTableBody.addEventListener("input", this.handleTableInput);
        this.productTableBody.addEventListener("click", this.handleTableClick);

        if (this.isDiscountAllowed && this.discountId) {
            const discountInput = document.getElementById(this.discountId);
            if (discountInput) {
                discountInput.addEventListener("input", this.handleDiscountInput);
            }
        }

        if (this.addRowBtn) {
            this.addRowBtn.addEventListener("click", this.handleAddRow);
        }
    }

    handleAddRow(e) {
        e.preventDefault();
        if (this.mode === "delete" || this.mode === "detail") return;
        this.renderRow();
        this.updateRowIndices();
    }

    // If user changes quantity
    handleTableInput(e) {
        if (e.target.classList.contains("product-qty")) {
            const row = e.target.closest("tr");
            const qty = parseFloat(e.target.value) || 0;
            //const unitPrice = parseFloat(row.querySelector(".unit-price").textContent) || 0;
            const unitPrice = parseFloat(row.querySelector(".unit-price-input").value) || 0;
            const stockText = row.querySelector(".current-stock").textContent;
            const stock = parseFloat(stockText) || 0;

            // SAQIB
            //if (stockText !== "--" && qty > stock) {
            //    alert("Quantity cannot exceed the available stock!");
            //    e.target.value = 0;
            //    this.updateRowValues(row, null, null);
            //    this.calculateTotals();
            //    return;
            //}

            const totalPrice = qty * unitPrice;
            row.querySelector(".product-total").firstChild.textContent = totalPrice.toFixed(2);
            row.querySelector(".product-total-input").value = totalPrice.toFixed(2);
            this.calculateTotals();
        }
    }

    handleTableClick(e) {
        const btn = e.target.closest("button, a");
        if (!btn) return;

        if (btn.classList.contains("delete-row")) {
            e.preventDefault();
            const row = btn.closest("tr");
            if (row.classList.contains("tax-row")) {
                // tax line remove
                row.remove();
                this.updateRowIndices();
                this.calculateTotals();
            } else {
                // product row => remove product + sub rows
                const productId = row.querySelector(".product-select").value;
                this.selectedProductIds.delete(parseInt(productId));

                let next = row.nextElementSibling;
                while (next && next.classList.contains("tax-row")) {
                    next.remove();
                    next = row.nextElementSibling;
                }
                row.remove();
                this.updateRowIndices();
                this.calculateTotals();
            }
        } else if (btn.classList.contains("edit-row")) {
            e.preventDefault();
            const row = btn.closest("tr");
            if (this.editTemplateType === "Template_1") {
                this.enterEditModeTemplate1(row);
            } else if (this.editTemplateType === "Template_3") {
                this.enterEditModeTemplate3(row);
            } else if (this.editTemplateType === "Template_2") {
                this.enableRow(row);
            } else {
                this.enableRow(row);
            }
        } else if (btn.classList.contains("save-row")) {
            e.preventDefault();
            const row = btn.closest("tr");
            if (this.editTemplateType === "Template_1") {
                this.saveRowTemplate1(row);
            }
        } else if (btn.classList.contains("check-row")) {
            e.preventDefault();
            const row = btn.closest("tr");
            if (this.editTemplateType === "Template_3") {
                this.saveRowTemplate3(row);
            }
        } else if (btn.classList.contains("reset-row")) {
            e.preventDefault();
            const row = btn.closest("tr");
            if (this.editTemplateType === "Template_1") {
                this.resetRowTemplate1(row);
            } else if (this.editTemplateType === "Template_3") {
                this.resetRowTemplate3(row);
            }
        } else if (btn.classList.contains("disable-row")) {
            e.preventDefault();
            const row = btn.closest("tr");
            this.disableRow(row);
        } else if (btn.classList.contains("add-vat-btn")) {
            e.preventDefault();
            const productRow = btn.closest("tr");
            this.renderTaxRow(productRow, null);
            this.updateRowIndices();
        }
    }

    handleDiscountInput(e) {
        const discountInput = e.target;
        const subtotalEl = document.getElementById(this.subtotalId);
        if (subtotalEl) {
            const subtotal = parseFloat(subtotalEl.value) || 0;
            const discountVal = parseFloat(discountInput.value) || 0;
            if (discountVal < 0 || discountVal > subtotal) {
                alert("Invalid discount value!");
                discountInput.value = "0.00";
                this.calculateTotals();
                return;
            }
        }
        this.calculateTotals();
    }

    // ------------------------------------------------------------------------
    // Disabling / enabling rows
    // ------------------------------------------------------------------------
    disableRow(row) {
        const inputs = row.querySelectorAll("input, select");
        inputs.forEach(input => {
            if (input.name) {
                const hiddenInput = document.createElement("input");
                hiddenInput.type = "hidden";
                hiddenInput.name = input.name;
                hiddenInput.value = input.value;
                hiddenInput.classList.add("hidden-disabled-input");
                row.appendChild(hiddenInput);
            }
            input.disabled = true;
            if (input.tomselect) {
                input.tomselect.disable();
            }
        });
    }

    disableRowForTemplate2(row) {
        this.disableRow(row);
    }

    enableRow(row) {
        const inputs = row.querySelectorAll("input, select");
        inputs.forEach(input => {
            input.disabled = false;
            if (input.tomselect) {
                input.tomselect.enable();
            }
        });
        const hiddenInputs = row.querySelectorAll(".hidden-disabled-input");
        hiddenInputs.forEach(hi => hi.remove());
    }

    // ------------------------------------------------------------------------
    // Template 1 editing approach
    // ------------------------------------------------------------------------
    enterEditModeTemplate1(row) {
        this.enableRow(row);
        if (!row.dataset.originalData) {
            row.dataset.originalData = JSON.stringify({
                productId: row.querySelector(".product-select").value,
                quantity: row.querySelector(".product-qty").value,
                unitPrice: row.querySelector(".unit-price-input").value,
                total: row.querySelector(".product-total-input").value
            });
        }
        const editBtn = row.querySelector(".edit-row");
        if (editBtn) {
            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
            editBtn.classList.remove("edit-row");
            editBtn.classList.add("save-row");
            editBtn.classList.add("btn-success");
            editBtn.classList.remove("btn-white");
        }
        const resetOption = row.querySelector(".dropdown-item.reset-row");
        if (resetOption) {
            resetOption.remove();
        }
    }

    saveRowTemplate1(row) {
        this.disableRow(row);
        const saveBtn = row.querySelector(".save-row");
        if (saveBtn) {
            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
            saveBtn.classList.remove("save-row");
            saveBtn.classList.add("edit-row");
            saveBtn.classList.remove("btn-success");
            saveBtn.classList.add("btn-white");
        }
        const dropdownMenu = row.querySelector(".dropdown-menu");
        if (dropdownMenu && !row.querySelector(".dropdown-item.reset-row")) {
            const resetOptionHTML = `
                <a class="dropdown-item reset-row" href="#">
                    <i class="bi-arrow-clockwise dropdown-item-icon"></i> Reset
                </a>
            `;
            dropdownMenu.insertAdjacentHTML("beforeend", resetOptionHTML);
        }
        this.calculateTotals();
    }

    resetRowTemplate1(row) {
        const originalData = JSON.parse(row.dataset.originalData || "{}");
        row.querySelector(".product-select").tomselect.setValue(originalData.productId);
        row.querySelector(".product-qty").value = originalData.quantity;
        row.querySelector(".unit-price-input").value = originalData.unitPrice;
        row.querySelector(".product-total-input").value = originalData.total;

        row.querySelector(".unit-price").firstChild.textContent = parseFloat(originalData.unitPrice).toFixed(2);
        row.querySelector(".product-total").firstChild.textContent = parseFloat(originalData.total).toFixed(2);

        this.disableRow(row);
        delete row.dataset.originalData;
        const resetOption = row.querySelector(".dropdown-item.reset-row");
        if (resetOption) {
            resetOption.remove();
        }
        this.calculateTotals();
    }

    // ------------------------------------------------------------------------
    // Template 3 editing approach
    // ------------------------------------------------------------------------
    enterEditModeTemplate3(row) {
        this.enableRow(row);
        if (!row.dataset.originalData) {
            row.dataset.originalData = JSON.stringify({
                productId: row.querySelector(".product-select").value,
                quantity: row.querySelector(".product-qty").value,
                unitPrice: row.querySelector(".unit-price-input").value,
                total: row.querySelector(".product-total-input").value
            });
        }
        const editBtn = row.querySelector(".edit-row");
        if (editBtn) {
            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
            editBtn.classList.remove("edit-row");
            editBtn.classList.add("check-row");
            editBtn.classList.remove("btn-success");
            editBtn.classList.add("btn-primary");
        }
        const resetBtn = row.querySelector(".reset-row");
        if (resetBtn) {
            resetBtn.remove();
        }
    }

    saveRowTemplate3(row) {
        this.disableRow(row);
        const checkBtn = row.querySelector(".check-row");
        if (checkBtn) {
            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
            checkBtn.classList.remove("check-row");
            checkBtn.classList.add("edit-row");
            checkBtn.classList.remove("btn-primary");
            checkBtn.classList.add("btn-success");
        }
        if (!row.querySelector(".reset-row")) {
            const resetBtn = document.createElement("button");
            resetBtn.className = "btn btn-warning btn-sm reset-row ms-1";
            resetBtn.innerHTML = '<i class="bi-arrow-clockwise"></i>';
            const deleteBtn = row.querySelector(".delete-row");
            deleteBtn.insertAdjacentElement("afterend", resetBtn);
        }
        this.calculateTotals();
    }

    resetRowTemplate3(row) {
        const originalData = JSON.parse(row.dataset.originalData || "{}");
        row.querySelector(".product-select").tomselect.setValue(originalData.productId);
        row.querySelector(".product-qty").value = originalData.quantity;
        row.querySelector(".unit-price-input").value = originalData.unitPrice;
        row.querySelector(".product-total-input").value = originalData.total;

        row.querySelector(".unit-price").firstChild.textContent = parseFloat(originalData.unitPrice).toFixed(2);
        row.querySelector(".product-total").firstChild.textContent = parseFloat(originalData.total).toFixed(2);

        this.disableRow(row);

        const resetBtn = row.querySelector(".reset-row");
        if (resetBtn) {
            resetBtn.remove();
        }
        const editBtn = row.querySelector(".edit-row");
        const checkBtn = row.querySelector(".check-row");
        if (checkBtn) {
            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
            checkBtn.classList.remove("check-row");
            checkBtn.classList.add("edit-row");
            checkBtn.classList.remove("btn-primary");
            checkBtn.classList.add("btn-success");
        } else if (!editBtn) {
            const btnContainer = row.querySelector("td:last-child");
            const newEditBtn = document.createElement("button");
            newEditBtn.className = "btn btn-success btn-sm edit-row";
            newEditBtn.innerHTML = '<i class="bi-pencil"></i>';
            btnContainer.insertAdjacentElement("afterbegin", newEditBtn);
        }
        delete row.dataset.originalData;
        this.calculateTotals();
    }

    // ------------------------------------------------------------------------
    // CALCULATE TOTALS: dynamic naming for the aggregated VAT breakdown
    // ------------------------------------------------------------------------
    calculateTotals() {
        // 1) For each product row, sum taxes => afterVAT
        const productRows = this.productTableBody.querySelectorAll("tr:not(.tax-row)");
        productRows.forEach(pr => {
            const totalPrice = parseFloat(pr.querySelector(".product-total-input").value) || 0;
            let sumTax = 0;
            let nextRow = pr.nextElementSibling;
            while (nextRow && nextRow.classList.contains("tax-row")) {
                const tAmt = parseFloat(nextRow.querySelector(".tax-amount-input").value) || 0;
                sumTax += tAmt;
                nextRow = nextRow.nextElementSibling;
            }
            const afterVat = totalPrice + sumTax;
            pr.querySelector(".product-after-vat").firstChild.textContent = afterVat.toFixed(2);
            pr.querySelector(".product-after-vat-input").value = afterVat.toFixed(2);
        });

        // 2) Subtotal + discount => total
        let grandTotal = 0, totalQty = 0, totalUnitPrice = 0;
        productRows.forEach(row => {
            const qty = parseFloat(row.querySelector(".product-qty").value) || 0;
            //const unitPrice = parseFloat(row.querySelector(".unit-price").textContent) || 0;
            const unitPrice = parseFloat(row.querySelector(".unit-price-input").value) || 0;
            const totalLinePrice = qty * unitPrice;
            grandTotal += totalLinePrice;
            totalQty += qty;
            totalUnitPrice += unitPrice;
        });

        const subtotal = grandTotal;
        if (this.subtotalId) {
            const subEl = document.getElementById(this.subtotalId);
            if (subEl) subEl.value = subtotal.toFixed(2);

            document.querySelectorAll(".subtotal-hidden").forEach(el => el.value = subtotal.toFixed(2));
            document.querySelectorAll(".subtotal-visible").forEach(el => el.value = subtotal.toFixed(2));
        }

        let discount = 0;
        if (this.isDiscountAllowed && this.discountId) {
            discount = parseFloat(document.getElementById(this.discountId).value) || 0;
        }
        const docTotal = subtotal - discount;
        if (this.totalId) {
            const totalEl = document.getElementById(this.totalId);
            if (totalEl) totalEl.value = docTotal.toFixed(2);
        }

        // Replace this (which causes syntax errors in older environments):
        // document.querySelector("#grandTotal")?.textContent = grandTotal.toFixed(2);

        // With checks like this:
        const grandTotalEl = document.querySelector("#grandTotal");
        if (grandTotalEl) {
            grandTotalEl.textContent = grandTotal.toFixed(2);
        }

        // For totalQuantity
        const totalQuantityEl = document.querySelector("#totalQuantity");
        if (totalQuantityEl) {
            totalQuantityEl.textContent = totalQty.toString();
        }

        // For totalPerPiecePrice
        const totalPerPiecePriceEl = document.querySelector("#totalPerPiecePrice");
        if (totalPerPiecePriceEl) {
            totalPerPiecePriceEl.textContent = totalQty > 0
                ? (grandTotal / totalQty).toFixed(2)
                : "0.00";
        }
        const totalUnitPriceEl = document.querySelector("#totalUnitPrice");
        if (totalUnitPriceEl) {
            totalUnitPriceEl.textContent = totalUnitPrice.toFixed(2);
        }
        document.querySelectorAll(".total-hidden").forEach(el => {
            el.value = docTotal.toFixed(2);
        });
        document.querySelectorAll(".total-visible").forEach(el => {
            el.value = docTotal.toFixed(2);
        });


        // 3) grand total after VAT
        let grandTotalAfterVat = 0;
        productRows.forEach(r => {
            const rowAfterVat = parseFloat(r.querySelector(".product-after-vat-input").value) || 0;
            grandTotalAfterVat += rowAfterVat;
        });
        // discount logic on afterVAT if needed

        const grandVatVisible = document.querySelector(".totalVat-Visible");
        const grandVatHidden = document.querySelector(".totalVat-hidden");
        if (grandVatVisible && grandVatHidden) {
            grandVatVisible.value = grandTotalAfterVat.toFixed(2);
            grandVatHidden.value = grandTotalAfterVat.toFixed(2);
        }

        // 4) Aggregated VAT breakdown with dynamic "this.vatBreakdownCollectionName"
        let vatBreakdown = {};  // { taxId : sumOfTax }

        const allTaxRows = this.productTableBody.querySelectorAll("tr.tax-row");
        allTaxRows.forEach(tRow => {
            const taxSelect = tRow.querySelector(".tax-master-select");
            const taxId = taxSelect ? taxSelect.value : "";
            if (!taxId) return;

            const taxAmt = parseFloat(tRow.querySelector(".tax-amount-input").value) || 0;
            if (vatBreakdown[taxId]) {
                vatBreakdown[taxId] += taxAmt;
            } else {
                vatBreakdown[taxId] = taxAmt;
            }
        });

        let vatBreakdownHTML = "";
        let vatBreakdownHiddenHTML = "";
        let index = 0;
        for (let taxId in vatBreakdown) {
            const taxObj = this.taxMasterData.find(t => t.Id == taxId);
            const taxName = taxObj ? taxObj.TaxName : "UnknownTax";
            const safeTaxName = taxName.replace(/\s+/g, "");

            vatBreakdownHTML += `
                <div class="col-lg-3 col-md-4 col-sm-4 text-end mb-2">
                    <span class="badge bg-soft-primary text-primary fs-5">${taxName}:</span>
                    <span id="vatBreakdown_${safeTaxName}" class="text-dark fs-5 fw-bold">
                        ${vatBreakdown[taxId].toFixed(2)}
                    </span>
                </div>
            `;
            vatBreakdownHiddenHTML += `
                <input type="hidden" name="${this.vatBreakdownCollectionName}[${index}].TaxId" value="${taxId}" />
                <input type="hidden" name="${this.vatBreakdownCollectionName}[${index}].TaxName" value="${taxName}" />
                <input type="hidden" name="${this.vatBreakdownCollectionName}[${index}].TaxAmount" value="${vatBreakdown[taxId].toFixed(2)}" />
            `;
            index++;
        }

        const vatBreakdownRow = document.getElementById("vatBreakdownRow");
        if (vatBreakdownRow) {
            vatBreakdownRow.innerHTML = vatBreakdownHTML;
        }
        const vatBreakdownHiddenContainer = document.getElementById("vatBreakdownHiddenContainer");
        if (vatBreakdownHiddenContainer) {
            vatBreakdownHiddenContainer.innerHTML = vatBreakdownHiddenHTML;
        }
    }
}


//=================================================================================
//================================== V4 ===========================================
//=================================================================================
//export class AdvancedDynamicTable {
//    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
//        this.selectors = {
//            productTableBody: document.getElementById(tableBodyId),
//            addRowBtn: document.getElementById(addButtonId),

//            subtotalId: options.subtotalId || null,
//            totalId: options.totalId || null,
//            discountId: options.discountId || null,

//            productSelectClass: ".product-select",
//            currentStockClass: ".current-stock",
//            unitPriceClass: ".unit-price",
//            unitPriceInputClass: ".unit-price-input",
//            quantityInputClass: ".product-qty",
//            productTotalClass: ".product-total",
//            productTotalInputClass: ".product-total-input",

//            grandTotalId: "#grandTotal",
//            totalQuantityId: "#totalQuantity",
//            totalPerPiecePriceId: "#totalPerPiecePrice",
//            totalUnitPriceId: "#totalUnitPrice",

//            // Hidden for Subtotal/Total
//            subtotalHiddenClass: ".subtotal-hidden",
//            subtotalVisibleClass: ".subtotal-visible",
//            totalHiddenClass: ".total-hidden",
//            totalVisibleClass: ".total-visible",

//            // PriceAfterVAT
//            productAfterVatClass: ".product-after-vat",
//            productAfterVatInputClass: ".product-after-vat-input",

//            // Tax sub-rows
//            taxRowClass: ".tax-row",
//            taxMasterSelectClass: ".tax-master-select",
//            taxAmountClass: ".tax-amount",
//            taxAmountInputClass: ".tax-amount-input",
//            taxSODtlIdClass: ".tax-sodtlid-input",  // rename if needed (just a reference)
//            taxSOIdClass: ".tax-soid-input",        // rename if needed
//            taxAfterTaxAmtClass: ".tax-aftertaxamount-input",

//            // Hidden ID fields
//            sodtlIdInputClass: ".sodtl-id-input",
//            taxIdInputClass: ".tax-id-input"
//        };

//        // --- Options and config ---
//        // This is key: allow user to pass dynamic "Tax Detail" and "VAT Breakdown" collection names:
//        this.taxDetailCollectionName = options.taxDetailCollectionName || "DTLTaxDtos";
//        this.vatBreakdownCollectionName = options.vatBreakdownCollectionName || "VATBreakdownDtos";

//        // NEW: for line-level foreign keys
//        this.detailIdFieldName = options.detailIdFieldName || "SODtlId";  // e.g. "SIDtlId"
//        this.docIdFieldName = options.docIdFieldName || "SOId";          // e.g. "SIId"

//        this.nameAttributeOptionObject = nameAttributeOptionObject;

//        // Field name possibilities
//        this.unitPriceFields = options.unitPriceFields || ["unitprice", "productprice", "price"];
//        this.stockFields = options.stockFields || ["stock", "currentstock", "quantityinstock"];
//        this.quantityFields = options.quantityFields || ["quantity", "qty"];
//        this.totalFields = options.totalFields || ["total", "totalprice"];

//        // DOM references
//        this.productTableBody = this.selectors.productTableBody;
//        this.addRowBtn = this.selectors.addRowBtn;

//        // Data
//        this.productsData = options.productsData || [];
//        this.prefilledRows = options.prefilledRows || [];
//        this.isEditMode = options.isEditMode || false;

        

//        // Subtotal/Total
//        this.subtotalId = this.selectors.subtotalId;
//        this.totalId = this.selectors.totalId;
//        this.discountId = this.selectors.discountId;
//        this.isDiscountAllowed = options.isDiscountAllowed || false;

//        // Distinguish which doc type mode: e.g. create/edit/delete/detail
//        this.mode = options.mode || "create";
//        this.editTemplateType = options.editTemplateType || "Default";

//        // For multi-VAT
//        this.taxMasterData = options.taxMasterData || [];
//        this.prefilledTaxMaster = options.prefilledTaxMaster || [];

//        this.selectedProductIds = new Set();
//        this.currentMaxSODtlId = 0; // track largest detail id

//        // Bind methods
//        this.handleAddRow = this.handleAddRow.bind(this);
//        this.handleTableInput = this.handleTableInput.bind(this);
//        this.handleTableClick = this.handleTableClick.bind(this);
//        this.handleDiscountInput = this.handleDiscountInput.bind(this);

//        this.initialize();
//    }

//    initialize() {
//        this.findCurrentMaxSODtlId();
//        this.renderInitialRows();
//        this.addEventListeners();
//        this.calculateTotals();

//        // Hide the main "Add" button if in delete/detail mode
//        if (this.mode === "delete" || this.mode === "detail") {
//            if (this.addRowBtn) {
//                this.addRowBtn.style.display = "none";
//            }
//        }
//    }

//    findCurrentMaxSODtlId() {
//        let maxId = 0;
//        this.prefilledRows.forEach(row => {
//            if (row.id && row.id > maxId) {
//                maxId = row.id;
//            }
//        });
//        this.currentMaxSODtlId = maxId;
//    }

//    // If you re-fetch data or want to reload
//    updateData(newOptions, newNameAttributeOptionObject) {
//        this.removeEventListeners();

//        this.productsData = newOptions.productsData || this.productsData;
//        this.prefilledRows = newOptions.prefilledRows || [];
//        this.isEditMode = newOptions.isEditMode || this.isEditMode;

//        this.subtotalId = newOptions.subtotalId || this.subtotalId;
//        this.totalId = newOptions.totalId || this.totalId;
//        this.discountId = newOptions.discountId || this.discountId;
//        this.isDiscountAllowed = newOptions.isDiscountAllowed !== undefined ? newOptions.isDiscountAllowed : this.isDiscountAllowed;

//        this.mode = newOptions.mode || this.mode;
//        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

//        // Re-config dynamic naming
//        this.taxDetailCollectionName = newOptions.taxDetailCollectionName || this.taxDetailCollectionName;
//        this.vatBreakdownCollectionName = newOptions.vatBreakdownCollectionName || this.vatBreakdownCollectionName;

//        // New tax data or existing
//        this.taxMasterData = newOptions.taxMasterData || this.taxMasterData;
//        this.prefilledTaxMaster = newOptions.prefilledTaxMaster || this.prefilledTaxMaster;

//        if (newNameAttributeOptionObject) {
//            this.nameAttributeOptionObject = newNameAttributeOptionObject;
//        }

//        // Clear table
//        this.productTableBody.innerHTML = "";
//        this.selectedProductIds.clear();

//        this.findCurrentMaxSODtlId();
//        this.initialize();
//    }

//    removeEventListeners() {
//        this.productTableBody.removeEventListener("input", this.handleTableInput);
//        this.productTableBody.removeEventListener("click", this.handleTableClick);

//        if (this.addRowBtn) {
//            this.addRowBtn.removeEventListener("click", this.handleAddRow);
//        }

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.removeEventListener("input", this.handleDiscountInput);
//            }
//        }
//    }

//    // Utility functions
//    findFieldName(dataObj, possibleFields) {
//        for (let field of possibleFields) {
//            if (field.toLowerCase() in dataObj) {
//                return field.toLowerCase();
//            }
//        }
//        return null;
//    }
//    normalizeKeys(obj) {
//        const normalized = {};
//        for (const key in obj) {
//            normalized[key.toLowerCase()] = obj[key];
//        }
//        return normalized;
//    }

//    // ------------------------------------------------------------------------
//    // Render the prefilled or initial rows
//    // ------------------------------------------------------------------------
//    renderInitialRows() {
//        this.productTableBody.innerHTML = "";
//        if (this.prefilledRows.length > 0) {
//            this.prefilledRows.forEach((row) => {
//                const productRow = this.renderRow(row);
//                // If we have associated tax lines
//                if (row.Id) {
//                    const matchingTaxes = this.prefilledTaxMaster.filter(t => Number(t.SODtlId) === Number(row.Id) ||
//                        Number(t.SIDtlId) === Number(row.Id) ||
//                        Number(t.PODtlId) === Number(row.Id) ||
//                        Number(t.PRDtlId) === Number(row.Id) ||
//                        Number(t.SRDtlId) === Number(row.Id) ||
//                        Number(t.PIDtlId) === Number(row.Id));
//                    if (matchingTaxes.length > 0) {
//                        matchingTaxes.forEach(taxDto => {
//                            this.renderTaxRow(productRow, taxDto);
//                        });
//                    }
//                }
//            });
//        } else if (this.mode === "create") {
//            this.renderRow(null);
//        }

//        this.updateRowIndices();
//    }

//    // ------------------------------------------------------------------------
//    // Render a single product row
//    // ------------------------------------------------------------------------
//    renderRow(data = null) {
//        const normalizedData = data ? this.normalizeKeys(data) : {};
//        let sodtlIdValue = data && typeof data.id !== "undefined" ? data.id : ++this.currentMaxSODtlId;

//        let pQuantity = normalizedData["quantity"] ?? 0;
//        const productOptions = this.productsData
//            .map(p =>
//                `<option value="${p.value}" ${normalizedData["productid"] == p.value ? "selected" : ""}>
//                    ${p.text}
//                 </option>`
//            ).join("");

//        const rowHTML = `
//<tr>
//    <!-- Hidden ID for detail row -->
//    <td style="display:none;">
//        <input type="hidden" class="sodtl-id-input" value="${sodtlIdValue}">
//    </td>
//    <td>
//        <div class="tom-select-custom">
//            <select class="js-select form-select product-select">
//                <option value="">--- Select Product ---</option>
//                ${productOptions}
//            </select>
//        </div>
//    </td>
//    <td class="current-stock">${normalizedData["stock"] ?? "--"}</td>
//    <td class="unit-price">
//        <input type="input" class="form-control unit-price-input" value="${(normalizedData["unitprice"] ?? 0).toFixed(2)}">
//    </td>
//    <td>
//        <input type="number" class="form-control product-qty" min="0" value="${pQuantity}">
//    </td>
//    <td class="product-total">
//        ${(normalizedData["total"] ?? 0).toFixed(2)}
//        <input type="hidden" class="product-total-input" value="${(normalizedData["total"] ?? 0).toFixed(2)}">
//    </td>
//    <td class="product-after-vat">
//        ${(normalizedData["afterVAT"] ?? 0).toFixed(2)}
//        <input type="hidden" class="product-after-vat-input"
//               value="${(normalizedData["afterVAT"] ?? 0).toFixed(2)}">
//    </td>
//    <td>
//        ${this.getActionButtonHTML(data)}
//        <button type="button" class="btn btn-soft-info btn-sm add-vat-btn">
//            <i class="bi-plus-circle"></i> VAT
//        </button>
//    </td>
//</tr>
//`;
//        this.productTableBody.insertAdjacentHTML("beforeend", rowHTML);
//        const row = this.productTableBody.lastElementChild;
//        this.updateRowIndices();

//        // Initialize TomSelect
//        const productSelect = row.querySelector(".product-select");
//        new TomSelect(productSelect, {
//            placeholder: "--- Select Product ---",
//            maxItems: 1,
//            onChange: (value) => this.handleProductSelection(row, value, data)
//        });

//        if (data) {
//            productSelect.tomselect.setValue(normalizedData["productid"]);
//        } else {
//            // brand-new => disable quantity until product chosen
//            row.querySelector(".product-qty").disabled = true;
//        }

//        // If delete/detail mode => disable
//        if (this.mode === "delete" || this.mode === "detail") {
//            this.disableRow(row);
//            // Hide "Add VAT" button
//            const addVatBtn = row.querySelector(".add-vat-btn");
//            if (addVatBtn) addVatBtn.style.display = "none";
//        } else if (data && this.mode === "edit") {
//            this.adjustRowForEditTemplate(row);
//        }

//        return row;
//    }

//    // ------------------------------------------------------------------------
//    // Render a single tax sub-row
//    // ------------------------------------------------------------------------
//    renderTaxRow(productRow, taxData = null) {
//        console.log(productRow,'pr');
//        console.log(taxData,'taxData');
//        // Insert after product row
//        let nextRow = productRow.nextElementSibling;
//        while (nextRow && nextRow.classList.contains("tax-row")) {
//            nextRow = nextRow.nextElementSibling;
//        }

//        const row = document.createElement("tr");
//        row.classList.add("tax-row");
//        row.style.backgroundColor = "#f1f8e9";
//        row.style.borderLeft = "4px solid #8bc34a";
//        row.style.fontSize = "0.95rem";

//        const parentRowId = productRow.querySelector(".sodtl-id-input").value || 0;

//        row.innerHTML = `
//<td style="display:none;">
//    <input type="hidden" class="tax-id-input" value="${taxData?.Id || 0}">
//</td>
//<td colspan="4">
//    <div class="tom-select-custom d-flex justify-content-end">
//        <select class="form-select tax-master-select w-50">
//            <option value="">-- Select Tax --</option>
//            ${this.taxMasterData.map(t => `
//                <option value="${t.Id}">${t.TaxName} (${t.TaxValue}${t.IsPercentage ? "%" : ""})</option>
//            `).join("")}
//        </select>
//        <input type="hidden" class="tax-sodtlid-input" value="${taxData?.SODtlId || taxData?.SIDtlId || parentRowId}">
//        <input type="hidden" class="tax-soid-input" value="${taxData?.SOId || taxData?.SIId || 0}">
//        <input type="hidden" class="tax-aftertaxamount-input" value="0.00">
//    </div>
//</td>
//<td>
//    <span class="tax-amount">0.00</span>
//    <input type="hidden" class="tax-amount-input" value="0.00">
//</td>
//<td>
//    <button type="button" class="btn btn-danger btn-sm delete-row">
//        <i class="bi-trash"></i>
//    </button>
//</td>
//`;

//        productRow.parentNode.insertBefore(row, nextRow);

//        // TomSelect for the tax dropdown
//        const taxSelect = row.querySelector(".tax-master-select");
//        new TomSelect(taxSelect, {
//            placeholder: "-- Select Tax --",
//            maxItems: 1,
//            onChange: (taxId) => this.handleTaxSelection(row, productRow, taxId)
//        });

//        // Fill existing data
//        if (taxData) {
//            if (taxData.TaxId) {
//                taxSelect.tomselect.setValue(taxData.TaxId.toString());
//            }
//            row.querySelector(".tax-amount").textContent = (taxData.TaxAmount || 0).toFixed(2);
//            row.querySelector(".tax-amount-input").value = (taxData.TaxAmount || 0).toFixed(2);

//            if (typeof taxData.aftertaxamount !== "undefined") {
//                row.querySelector(".tax-aftertaxamount-input").value = taxData.aftertaxamount.toFixed(2);
//            }
//        }

//        // If in delete or detail => hide the delete button + disable
//        if (this.mode === "delete" || this.mode === "detail") {
//            const delBtn = row.querySelector(".delete-row");
//            if (delBtn) delBtn.style.display = "none";
//            this.disableRow(row);
//        }

//        return row;
//    }

//    // ------------------------------------------------------------------------
//    // Returns the HTML for the "Actions" cell
//    // ------------------------------------------------------------------------
//    getActionButtonHTML(data) {
//        if (data) {
//            if (this.mode === "edit") {
//                if (this.editTemplateType === "Template_1") {
//                    return `
//                        <div class="btn-group" role="group">
//                            <button type="button" class="btn btn-white btn-sm edit-row">
//                                <i class="bi-pencil-fill me-1"></i> Edit
//                            </button>
//                            <div class="btn-group">
//                                <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty"
//                                    data-bs-toggle="dropdown" aria-expanded="false"></button>
//                                <div class="dropdown-menu dropdown-menu-end mt-1">
//                                    <a class="dropdown-item delete-row" href="#">
//                                        <i class="bi-trash dropdown-item-icon"></i> Delete
//                                    </a>
//                                </div>
//                            </div>
//                        </div>
//                    `;
//                } else if (this.editTemplateType === "Template_2") {
//                    return `
//                        <button class="btn btn-primary btn-sm edit-row">
//                            <i class="bi-pencil"></i>
//                        </button>
//                        <button class="btn btn-danger btn-sm delete-row">
//                            <i class="bi-trash"></i>
//                        </button>
//                    `;
//                } else if (this.editTemplateType === "Template_3") {
//                    return `
//                        <button class="btn btn-success btn-sm edit-row" type="button">
//                            <i class="bi-pencil"></i>
//                        </button>
//                        <button class="btn btn-danger btn-sm delete-row">
//                            <i class="bi-trash"></i>
//                        </button>
//                    `;
//                } else {
//                    return `<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>`;
//                }
//            } else if (this.mode === "delete" || this.mode === "detail") {
//                return "";
//            }
//        }
//        // brand-new row
//        return `<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>`;
//    }

//    // If an "edit template" is chosen, we disable or etc.
//    adjustRowForEditTemplate(row) {
//        if (this.editTemplateType === "Template_1") {
//            this.disableRow(row);
//        } else if (this.editTemplateType === "Template_2") {
//            this.disableRowForTemplate2(row);
//        } else if (this.editTemplateType === "Template_3") {
//            this.disableRow(row);
//        }
//    }

//    // ------------------------------------------------------------------------
//    // Handle product selection
//    // ------------------------------------------------------------------------
//    handleProductSelection(row, productId, existingData = null) {
//        const prevId = row.dataset.previousProductId;
//        if (prevId) {
//            this.selectedProductIds.delete(parseInt(prevId));
//        }

//        const quantityInput = row.querySelector(".product-qty");
//        if (!productId) {
//            quantityInput.disabled = true;
//            quantityInput.value = "";
//            this.updateRowValues(row, null, existingData);
//            row.dataset.previousProductId = null;
//            this.calculateTotals();
//            return;
//        }
//        if (this.selectedProductIds.has(parseInt(productId))) {
//            alert("This product is already selected. Please choose another.");
//            row.querySelector(".product-select").tomselect.clear();
//            return;
//        }

//        this.selectedProductIds.add(parseInt(productId));
//        row.dataset.previousProductId = productId;
//        quantityInput.disabled = false;

//        const product = this.productsData.find(p => p.value == productId) || {};
//        const productNormalized = this.normalizeKeys(product);

//        this.updateRowValues(row, productNormalized, existingData);
//        this.calculateTotals();

//        if (quantityInput) {
//            quantityInput.focus();
//        }
//    }

//    updateRowValues(row, data, existingData) {
//        const normalizedData = data || {};
//        const existingNormalizedData = existingData ? this.normalizeKeys(existingData) : {};
//        const unitPriceCell = row.querySelector(".unit-price");
//        const unitPriceInput = row.querySelector(".unit-price-input");
//        const productTotalCell = row.querySelector(".product-total");
//        const productTotalInput = row.querySelector(".product-total-input");
//        const quantityInput = row.querySelector(".product-qty");
//        const currentStockCell = row.querySelector(".current-stock");

//        if (!data) {
//            // Clear everything
//            //unitPriceCell.firstChild.textContent = "0.00";
//            unitPriceInput.value = "0.00";
//            productTotalCell.firstChild.textContent = "0.00";
//            productTotalInput.value = "0.00";
//            quantityInput.value = 0;
//            currentStockCell.textContent = "--";
//            return;
//        }

//        // find matching fields
//        const unitPriceField = this.findFieldName(normalizedData, this.unitPriceFields);
//        const stockField = this.findFieldName(normalizedData, this.stockFields);
//        const unitPrice = parseFloat(normalizedData[unitPriceField]) || 0;
//        const stockVal = normalizedData[stockField] !== undefined ? normalizedData[stockField] : "--";

//        //unitPriceCell.firstChild.textContent = unitPrice.toFixed(2);
//        unitPriceInput.value = unitPrice.toFixed(2);
//        currentStockCell.textContent = stockVal;

//        let existingQty = parseFloat(existingNormalizedData[this.nameAttributeOptionObject.quantity?.toLowerCase()]) || 0;
//        console.log(unitPrice * existingQty)
//        if (!existingQty) {
//            // default 0
//            quantityInput.value = 0;
//            productTotalCell.firstChild.textContent = "0.00";
//            productTotalInput.value = "0.00";
//        } else {
//            quantityInput.value = existingQty;
//            const calcTotal = unitPrice * existingQty;
//            productTotalCell.firstChild.textContent = calcTotal.toFixed(2);
//            productTotalInput.value = calcTotal.toFixed(2);
//        }
//    }

//    // ------------------------------------------------------------------------
//    // Handle selection of a tax row
//    // ------------------------------------------------------------------------
//    handleTaxSelection(taxRow, productRow, taxId) {
//        if (!taxId) {
//            taxRow.querySelector(".tax-amount").textContent = "0.00";
//            taxRow.querySelector(".tax-amount-input").value = "0.00";
//            this.calculateTotals();
//            return;
//        }

//        const chosenTax = this.taxMasterData.find(t => t.Id == taxId);
//        if (!chosenTax) return;

//        // prevent duplicates
//        let nextRow = productRow.nextElementSibling;
//        while (nextRow && nextRow.classList.contains("tax-row")) {
//            const siblingTaxSelect = nextRow.querySelector(".tax-master-select");
//            if (siblingTaxSelect && siblingTaxSelect.value == taxId && nextRow !== taxRow) {
//                alert("This Tax is already applied. Choose another.");
//                taxRow.querySelector(".tax-master-select").tomselect.clear();
//                return;
//            }
//            nextRow = nextRow.nextElementSibling;
//        }

//        const productTotal = parseFloat(productRow.querySelector(".product-total-input").value) || 0;
//        let computedTax = 0;
//        let afterTax = 0;
//        const taxValue = parseFloat(chosenTax.TaxValue) || 0;

//        // isPercentage, isExclusive, isInclusive
//        if (chosenTax.IsPercentage) {
//            if (chosenTax.IsExclusive) {
//                computedTax = (productTotal * taxValue) / 100;
//                afterTax = productTotal + computedTax;
//            } else if (chosenTax.IsInclusive) {
//                const net = productTotal / (1 + taxValue / 100);
//                computedTax = productTotal - net;
//                afterTax = productTotal;
//            } else {
//                // default
//                computedTax = (productTotal * taxValue) / 100;
//                afterTax = productTotal + computedTax;
//            }
//        } else {
//            // fixed
//            if (chosenTax.IsExclusive) {
//                computedTax = taxValue;
//                afterTax = productTotal + computedTax;
//            } else if (chosenTax.IsInclusive) {
//                if (productTotal >= taxValue) {
//                    computedTax = taxValue;
//                    afterTax = productTotal;
//                } else {
//                    computedTax = productTotal;
//                    afterTax = productTotal;
//                }
//            } else {
//                computedTax = taxValue;
//                afterTax = productTotal + computedTax;
//            }
//        }

//        taxRow.querySelector(".tax-amount").textContent = computedTax.toFixed(2);
//        taxRow.querySelector(".tax-amount-input").value = computedTax.toFixed(2);

//        const afterTaxInput = taxRow.querySelector(".tax-aftertaxamount-input");
//        if (afterTaxInput) {
//            afterTaxInput.value = afterTax.toFixed(2);
//        }

//        this.calculateTotals();
//    }

//    // ------------------------------------------------------------------------
//    // updateRowIndices: dynamic for product lines and tax lines
//    // ------------------------------------------------------------------------
//    updateRowIndices() {
//        const nameAttrs = this.nameAttributeOptionObject;
//        this.globalTaxCounter = 0;

//        let productIndex = 0;
//        const allRows = this.productTableBody.querySelectorAll("tr");
//        allRows.forEach(row => {
//            if (!row.classList.contains("tax-row")) {
//                // It's a product row
//                row.setAttribute("data-product-index", productIndex);

//                // main detail ID
//                const sodtlIdInput = row.querySelector(".sodtl-id-input");
//                if (sodtlIdInput) {
//                    sodtlIdInput.name = `${nameAttrs.base}[${productIndex}].Id`;
//                }
//                // product select
//                const productSelect = row.querySelector(".product-select");
//                productSelect.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.productId}`;

//                // quantity
//                const qtyInput = row.querySelector(".product-qty");
//                qtyInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.quantity}`;

//                // unit price
//                const upInput = row.querySelector(".unit-price-input");
//                upInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.unitPrice}`;

//                // total
//                const totalInput = row.querySelector(".product-total-input");
//                totalInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.total}`;

//                // after VAT
//                const afterVat = row.querySelector(".product-after-vat-input");
//                afterVat.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.afterVAT}`;
//                // or pass a custom "afterVatPropertyName" if you want

//                productIndex++;
//            } else {
//                // It's a tax row => we use the dynamic “taxDetailCollectionName”
//                const taxIdInput = row.querySelector(this.selectors.taxIdInputClass);
//                const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
//                const taxAmtInput = row.querySelector(this.selectors.taxAmountInputClass);
//                const sodtlIdInput = row.querySelector(this.selectors.taxSODtlIdClass);
//                const soIdInput = row.querySelector(this.selectors.taxSOIdClass);
//                const afterTaxAmtInput = row.querySelector(this.selectors.taxAfterTaxAmtClass);

//                taxIdInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].Id`;
//                taxSelect.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].TaxId`;
//                taxAmtInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].TaxAmount`;
//                sodtlIdInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].${this.detailIdFieldName}`;
//                soIdInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].${this.docIdFieldName}`;
//                afterTaxAmtInput.name = `${this.taxDetailCollectionName}[${this.globalTaxCounter}].AfterTaxAmount`;

//                // If this is for SI => you might rename e.g. SIDtlId, SIId instead
//                // You can pass that logic in your code or rename these input classes

//                this.globalTaxCounter++;
//            }
//        });
//    }

//    // ------------------------------------------------------------------------
//    // Add event listeners
//    // ------------------------------------------------------------------------
//    addEventListeners() {
//        this.productTableBody.addEventListener("input", this.handleTableInput);
//        this.productTableBody.addEventListener("click", this.handleTableClick);

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.addEventListener("input", this.handleDiscountInput);
//            }
//        }

//        if (this.addRowBtn) {
//            this.addRowBtn.addEventListener("click", this.handleAddRow);
//        }
//    }

//    handleAddRow(e) {
//        e.preventDefault();
//        if (this.mode === "delete" || this.mode === "detail") return;
//        this.renderRow();
//        this.updateRowIndices();
//    }

//    // If user changes quantity
//    handleTableInput(e) {
//        if (e.target.classList.contains("product-qty")) {
//            const row = e.target.closest("tr");
//            const qty = parseFloat(e.target.value) || 0;
//            //const unitPrice = parseFloat(row.querySelector(".unit-price").textContent) || 0;
//            const unitPrice = parseFloat(row.querySelector(".unit-price-input").value) || 0;
//            const stockText = row.querySelector(".current-stock").textContent;
//            const stock = parseFloat(stockText) || 0;

//            if (stockText !== "--" && qty > stock) {
//                alert("Quantity cannot exceed the available stock!");
//                e.target.value = 0;
//                this.updateRowValues(row, null, null);
//                this.calculateTotals();
//                return;
//            }

//            const totalPrice = qty * unitPrice;
//            row.querySelector(".product-total").firstChild.textContent = totalPrice.toFixed(2);
//            row.querySelector(".product-total-input").value = totalPrice.toFixed(2);
//            this.calculateTotals();
//        }
//    }

//    handleTableClick(e) {
//        const btn = e.target.closest("button, a");
//        if (!btn) return;

//        if (btn.classList.contains("delete-row")) {
//            e.preventDefault();
//            const row = btn.closest("tr");
//            if (row.classList.contains("tax-row")) {
//                // tax line remove
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            } else {
//                // product row => remove product + sub rows
//                const productId = row.querySelector(".product-select").value;
//                this.selectedProductIds.delete(parseInt(productId));

//                let next = row.nextElementSibling;
//                while (next && next.classList.contains("tax-row")) {
//                    next.remove();
//                    next = row.nextElementSibling;
//                }
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            }
//        } else if (btn.classList.contains("edit-row")) {
//            e.preventDefault();
//            const row = btn.closest("tr");
//            if (this.editTemplateType === "Template_1") {
//                this.enterEditModeTemplate1(row);
//            } else if (this.editTemplateType === "Template_3") {
//                this.enterEditModeTemplate3(row);
//            } else if (this.editTemplateType === "Template_2") {
//                this.enableRow(row);
//            } else {
//                this.enableRow(row);
//            }
//        } else if (btn.classList.contains("save-row")) {
//            e.preventDefault();
//            const row = btn.closest("tr");
//            if (this.editTemplateType === "Template_1") {
//                this.saveRowTemplate1(row);
//            }
//        } else if (btn.classList.contains("check-row")) {
//            e.preventDefault();
//            const row = btn.closest("tr");
//            if (this.editTemplateType === "Template_3") {
//                this.saveRowTemplate3(row);
//            }
//        } else if (btn.classList.contains("reset-row")) {
//            e.preventDefault();
//            const row = btn.closest("tr");
//            if (this.editTemplateType === "Template_1") {
//                this.resetRowTemplate1(row);
//            } else if (this.editTemplateType === "Template_3") {
//                this.resetRowTemplate3(row);
//            }
//        } else if (btn.classList.contains("disable-row")) {
//            e.preventDefault();
//            const row = btn.closest("tr");
//            this.disableRow(row);
//        } else if (btn.classList.contains("add-vat-btn")) {
//            e.preventDefault();
//            const productRow = btn.closest("tr");
//            this.renderTaxRow(productRow, null);
//            this.updateRowIndices();
//        }
//    }

//    handleDiscountInput(e) {
//        const discountInput = e.target;
//        const subtotalEl = document.getElementById(this.subtotalId);
//        if (subtotalEl) {
//            const subtotal = parseFloat(subtotalEl.value) || 0;
//            const discountVal = parseFloat(discountInput.value) || 0;
//            if (discountVal < 0 || discountVal > subtotal) {
//                alert("Invalid discount value!");
//                discountInput.value = "0.00";
//                this.calculateTotals();
//                return;
//            }
//        }
//        this.calculateTotals();
//    }

//    // ------------------------------------------------------------------------
//    // Disabling / enabling rows
//    // ------------------------------------------------------------------------
//    disableRow(row) {
//        const inputs = row.querySelectorAll("input, select");
//        inputs.forEach(input => {
//            if (input.name) {
//                const hiddenInput = document.createElement("input");
//                hiddenInput.type = "hidden";
//                hiddenInput.name = input.name;
//                hiddenInput.value = input.value;
//                hiddenInput.classList.add("hidden-disabled-input");
//                row.appendChild(hiddenInput);
//            }
//            input.disabled = true;
//            if (input.tomselect) {
//                input.tomselect.disable();
//            }
//        });
//    }

//    disableRowForTemplate2(row) {
//        this.disableRow(row);
//    }

//    enableRow(row) {
//        const inputs = row.querySelectorAll("input, select");
//        inputs.forEach(input => {
//            input.disabled = false;
//            if (input.tomselect) {
//                input.tomselect.enable();
//            }
//        });
//        const hiddenInputs = row.querySelectorAll(".hidden-disabled-input");
//        hiddenInputs.forEach(hi => hi.remove());
//    }

//    // ------------------------------------------------------------------------
//    // Template 1 editing approach
//    // ------------------------------------------------------------------------
//    enterEditModeTemplate1(row) {
//        this.enableRow(row);
//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(".product-select").value,
//                quantity: row.querySelector(".product-qty").value,
//                unitPrice: row.querySelector(".unit-price-input").value,
//                total: row.querySelector(".product-total-input").value
//            });
//        }
//        const editBtn = row.querySelector(".edit-row");
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
//            editBtn.classList.remove("edit-row");
//            editBtn.classList.add("save-row");
//            editBtn.classList.add("btn-success");
//            editBtn.classList.remove("btn-white");
//        }
//        const resetOption = row.querySelector(".dropdown-item.reset-row");
//        if (resetOption) {
//            resetOption.remove();
//        }
//    }

//    saveRowTemplate1(row) {
//        this.disableRow(row);
//        const saveBtn = row.querySelector(".save-row");
//        if (saveBtn) {
//            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
//            saveBtn.classList.remove("save-row");
//            saveBtn.classList.add("edit-row");
//            saveBtn.classList.remove("btn-success");
//            saveBtn.classList.add("btn-white");
//        }
//        const dropdownMenu = row.querySelector(".dropdown-menu");
//        if (dropdownMenu && !row.querySelector(".dropdown-item.reset-row")) {
//            const resetOptionHTML = `
//                <a class="dropdown-item reset-row" href="#">
//                    <i class="bi-arrow-clockwise dropdown-item-icon"></i> Reset
//                </a>
//            `;
//            dropdownMenu.insertAdjacentHTML("beforeend", resetOptionHTML);
//        }
//        this.calculateTotals();
//    }

//    resetRowTemplate1(row) {
//        const originalData = JSON.parse(row.dataset.originalData || "{}");
//        row.querySelector(".product-select").tomselect.setValue(originalData.productId);
//        row.querySelector(".product-qty").value = originalData.quantity;
//        row.querySelector(".unit-price-input").value = originalData.unitPrice;
//        row.querySelector(".product-total-input").value = originalData.total;

//        row.querySelector(".unit-price").firstChild.textContent = parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(".product-total").firstChild.textContent = parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);
//        delete row.dataset.originalData;
//        const resetOption = row.querySelector(".dropdown-item.reset-row");
//        if (resetOption) {
//            resetOption.remove();
//        }
//        this.calculateTotals();
//    }

//    // ------------------------------------------------------------------------
//    // Template 3 editing approach
//    // ------------------------------------------------------------------------
//    enterEditModeTemplate3(row) {
//        this.enableRow(row);
//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(".product-select").value,
//                quantity: row.querySelector(".product-qty").value,
//                unitPrice: row.querySelector(".unit-price-input").value,
//                total: row.querySelector(".product-total-input").value
//            });
//        }
//        const editBtn = row.querySelector(".edit-row");
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
//            editBtn.classList.remove("edit-row");
//            editBtn.classList.add("check-row");
//            editBtn.classList.remove("btn-success");
//            editBtn.classList.add("btn-primary");
//        }
//        const resetBtn = row.querySelector(".reset-row");
//        if (resetBtn) {
//            resetBtn.remove();
//        }
//    }

//    saveRowTemplate3(row) {
//        this.disableRow(row);
//        const checkBtn = row.querySelector(".check-row");
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove("check-row");
//            checkBtn.classList.add("edit-row");
//            checkBtn.classList.remove("btn-primary");
//            checkBtn.classList.add("btn-success");
//        }
//        if (!row.querySelector(".reset-row")) {
//            const resetBtn = document.createElement("button");
//            resetBtn.className = "btn btn-warning btn-sm reset-row ms-1";
//            resetBtn.innerHTML = '<i class="bi-arrow-clockwise"></i>';
//            const deleteBtn = row.querySelector(".delete-row");
//            deleteBtn.insertAdjacentElement("afterend", resetBtn);
//        }
//        this.calculateTotals();
//    }

//    resetRowTemplate3(row) {
//        const originalData = JSON.parse(row.dataset.originalData || "{}");
//        row.querySelector(".product-select").tomselect.setValue(originalData.productId);
//        row.querySelector(".product-qty").value = originalData.quantity;
//        row.querySelector(".unit-price-input").value = originalData.unitPrice;
//        row.querySelector(".product-total-input").value = originalData.total;

//        row.querySelector(".unit-price").firstChild.textContent = parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(".product-total").firstChild.textContent = parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        const resetBtn = row.querySelector(".reset-row");
//        if (resetBtn) {
//            resetBtn.remove();
//        }
//        const editBtn = row.querySelector(".edit-row");
//        const checkBtn = row.querySelector(".check-row");
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove("check-row");
//            checkBtn.classList.add("edit-row");
//            checkBtn.classList.remove("btn-primary");
//            checkBtn.classList.add("btn-success");
//        } else if (!editBtn) {
//            const btnContainer = row.querySelector("td:last-child");
//            const newEditBtn = document.createElement("button");
//            newEditBtn.className = "btn btn-success btn-sm edit-row";
//            newEditBtn.innerHTML = '<i class="bi-pencil"></i>';
//            btnContainer.insertAdjacentElement("afterbegin", newEditBtn);
//        }
//        delete row.dataset.originalData;
//        this.calculateTotals();
//    }

//    // ------------------------------------------------------------------------
//    // CALCULATE TOTALS: dynamic naming for the aggregated VAT breakdown
//    // ------------------------------------------------------------------------
//    calculateTotals() {
//        // 1) For each product row, sum taxes => afterVAT
//        const productRows = this.productTableBody.querySelectorAll("tr:not(.tax-row)");
//        productRows.forEach(pr => {
//            const totalPrice = parseFloat(pr.querySelector(".product-total-input").value) || 0;
//            let sumTax = 0;
//            let nextRow = pr.nextElementSibling;
//            while (nextRow && nextRow.classList.contains("tax-row")) {
//                const tAmt = parseFloat(nextRow.querySelector(".tax-amount-input").value) || 0;
//                sumTax += tAmt;
//                nextRow = nextRow.nextElementSibling;
//            }
//            const afterVat = totalPrice + sumTax;
//            pr.querySelector(".product-after-vat").firstChild.textContent = afterVat.toFixed(2);
//            pr.querySelector(".product-after-vat-input").value = afterVat.toFixed(2);
//        });

//        // 2) Subtotal + discount => total
//        let grandTotal = 0, totalQty = 0, totalUnitPrice = 0;
//        productRows.forEach(row => {
//            const qty = parseFloat(row.querySelector(".product-qty").value) || 0;
//            //const unitPrice = parseFloat(row.querySelector(".unit-price").textContent) || 0;
//            const unitPrice = parseFloat(row.querySelector(".unit-price-input").value) || 0;
//            const totalLinePrice = qty * unitPrice;
//            grandTotal += totalLinePrice;
//            totalQty += qty;
//            totalUnitPrice += unitPrice;
//        });

//        const subtotal = grandTotal;
//        if (this.subtotalId) {
//            const subEl = document.getElementById(this.subtotalId);
//            if (subEl) subEl.value = subtotal.toFixed(2);

//            document.querySelectorAll(".subtotal-hidden").forEach(el => el.value = subtotal.toFixed(2));
//            document.querySelectorAll(".subtotal-visible").forEach(el => el.value = subtotal.toFixed(2));
//        }

//        let discount = 0;
//        if (this.isDiscountAllowed && this.discountId) {
//            discount = parseFloat(document.getElementById(this.discountId).value) || 0;
//        }
//        const docTotal = subtotal - discount;
//        if (this.totalId) {
//            const totalEl = document.getElementById(this.totalId);
//            if (totalEl) totalEl.value = docTotal.toFixed(2);
//        }

//        // Replace this (which causes syntax errors in older environments):
//        // document.querySelector("#grandTotal")?.textContent = grandTotal.toFixed(2);

//        // With checks like this:
//        const grandTotalEl = document.querySelector("#grandTotal");
//        if (grandTotalEl) {
//            grandTotalEl.textContent = grandTotal.toFixed(2);
//        }

//        // For totalQuantity
//        const totalQuantityEl = document.querySelector("#totalQuantity");
//        if (totalQuantityEl) {
//            totalQuantityEl.textContent = totalQty.toString();
//        }

//        // For totalPerPiecePrice
//        const totalPerPiecePriceEl = document.querySelector("#totalPerPiecePrice");
//        if (totalPerPiecePriceEl) {
//            totalPerPiecePriceEl.textContent = totalQty > 0
//                ? (grandTotal / totalQty).toFixed(2)
//                : "0.00";
//        }
//        const totalUnitPriceEl = document.querySelector("#totalUnitPrice");
//        if (totalUnitPriceEl) {
//            totalUnitPriceEl.textContent = totalUnitPrice.toFixed(2);
//        }
//        document.querySelectorAll(".total-hidden").forEach(el => {
//            el.value = docTotal.toFixed(2);
//        });
//        document.querySelectorAll(".total-visible").forEach(el => {
//            el.value = docTotal.toFixed(2);
//        });


//        // 3) grand total after VAT
//        let grandTotalAfterVat = 0;
//        productRows.forEach(r => {
//            const rowAfterVat = parseFloat(r.querySelector(".product-after-vat-input").value) || 0;
//            grandTotalAfterVat += rowAfterVat;
//        });
//        // discount logic on afterVAT if needed

//        const grandVatVisible = document.querySelector(".totalVat-Visible");
//        const grandVatHidden = document.querySelector(".totalVat-hidden");
//        if (grandVatVisible && grandVatHidden) {
//            grandVatVisible.value = grandTotalAfterVat.toFixed(2);
//            grandVatHidden.value = grandTotalAfterVat.toFixed(2);
//        }

//        // 4) Aggregated VAT breakdown with dynamic "this.vatBreakdownCollectionName"
//        let vatBreakdown = {};  // { taxId : sumOfTax }

//        const allTaxRows = this.productTableBody.querySelectorAll("tr.tax-row");
//        allTaxRows.forEach(tRow => {
//            const taxSelect = tRow.querySelector(".tax-master-select");
//            const taxId = taxSelect ? taxSelect.value : "";
//            if (!taxId) return;

//            const taxAmt = parseFloat(tRow.querySelector(".tax-amount-input").value) || 0;
//            if (vatBreakdown[taxId]) {
//                vatBreakdown[taxId] += taxAmt;
//            } else {
//                vatBreakdown[taxId] = taxAmt;
//            }
//        });

//        let vatBreakdownHTML = "";
//        let vatBreakdownHiddenHTML = "";
//        let index = 0;
//        for (let taxId in vatBreakdown) {
//            const taxObj = this.taxMasterData.find(t => t.Id == taxId);
//            const taxName = taxObj ? taxObj.TaxName : "UnknownTax";
//            const safeTaxName = taxName.replace(/\s+/g, "");

//            vatBreakdownHTML += `
//                <div class="col-lg-3 col-md-4 col-sm-4 text-end mb-2">
//                    <span class="badge bg-soft-primary text-primary fs-5">${taxName}:</span>
//                    <span id="vatBreakdown_${safeTaxName}" class="text-dark fs-5 fw-bold">
//                        ${vatBreakdown[taxId].toFixed(2)}
//                    </span>
//                </div>
//            `;
//            vatBreakdownHiddenHTML += `
//                <input type="hidden" name="${this.vatBreakdownCollectionName}[${index}].TaxId" value="${taxId}" />
//                <input type="hidden" name="${this.vatBreakdownCollectionName}[${index}].TaxName" value="${taxName}" />
//                <input type="hidden" name="${this.vatBreakdownCollectionName}[${index}].TaxAmount" value="${vatBreakdown[taxId].toFixed(2)}" />
//            `;
//            index++;
//        }

//        const vatBreakdownRow = document.getElementById("vatBreakdownRow");
//        if (vatBreakdownRow) {
//            vatBreakdownRow.innerHTML = vatBreakdownHTML;
//        }
//        const vatBreakdownHiddenContainer = document.getElementById("vatBreakdownHiddenContainer");
//        if (vatBreakdownHiddenContainer) {
//            vatBreakdownHiddenContainer.innerHTML = vatBreakdownHiddenHTML;
//        }
//    }
//}



//=================================================================================
//================================== V3 ===========================================
//=================================================================================
//export class AdvancedDynamicTable {
//    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
//        this.selectors = {
//            productTableBody: document.getElementById(tableBodyId),
//            addRowBtn: document.getElementById(addButtonId),

//            subtotalId: options.subtotalId || null,
//            totalId: options.totalId || null,
//            discountId: options.discountId || null,

//            productSelectClass: ".product-select",
//            currentStockClass: ".current-stock",
//            unitPriceClass: ".unit-price",
//            unitPriceInputClass: ".unit-price-input",
//            quantityInputClass: ".product-qty",
//            productTotalClass: ".product-total",
//            productTotalInputClass: ".product-total-input",

//            grandTotalId: "#grandTotal",
//            totalQuantityId: "#totalQuantity",
//            totalPerPiecePriceId: "#totalPerPiecePrice",
//            totalUnitPriceId: "#totalUnitPrice",

//            // Hidden for Subtotal/Total
//            subtotalHiddenClass: ".subtotal-hidden",
//            subtotalVisibleClass: ".subtotal-visible",
//            totalHiddenClass: ".total-hidden",
//            totalVisibleClass: ".total-visible",

//            // PriceAfterVAT
//            productAfterVatClass: ".product-after-vat",
//            productAfterVatInputClass: ".product-after-vat-input",

//            // Tax sub-rows
//            taxRowClass: ".tax-row",
//            taxMasterSelectClass: ".tax-master-select",
//            taxAmountClass: ".tax-amount",
//            taxAmountInputClass: ".tax-amount-input",
//            taxSODtlIdClass: ".tax-sodtlid-input",
//            taxSOIdClass: ".tax-soid-input",
//            taxAfterTaxAmtClass: ".tax-aftertaxamount-input",

//            // (NEW) Hidden ID fields
//            sodtlIdInputClass: ".sodtl-id-input",
//            taxIdInputClass: ".tax-id-input",
//        };

//        this.nameAttributeOptionObject = nameAttributeOptionObject;

//        this.unitPriceFields = options.unitPriceFields || ["unitprice", "productprice", "price"];
//        this.stockFields = options.stockFields || ["stock", "currentstock", "quantityinstock"];
//        this.quantityFields = options.quantityFields || ["quantity", "qty"];
//        this.totalFields = options.totalFields || ["total", "totalprice"];

//        this.productTableBody = this.selectors.productTableBody;
//        this.addRowBtn = this.selectors.addRowBtn;

//        this.productsData = options.productsData || [];
//        this.prefilledRows = options.prefilledRows || [];
//        this.isEditMode = options.isEditMode || false;

//        this.subtotalId = this.selectors.subtotalId;
//        this.totalId = this.selectors.totalId;
//        this.discountId = this.selectors.discountId;
//        this.isDiscountAllowed = options.isDiscountAllowed || false;

//        this.selectedProductIds = new Set();

//        this.mode = options.mode || "create";
//        this.editTemplateType = options.editTemplateType || "Default";

//        // For multi-VAT
//        this.taxMasterData = options.taxMasterData || [];
//        this.prefilledTaxMaster = options.prefilledTaxMaster || [];

//        // (NEW) We'll keep track of the largest SODtlId from prefilledRows
//        this.currentMaxSODtlId = 0;

//        // Bind methods
//        this.handleAddRow = this.handleAddRow.bind(this);
//        this.handleTableInput = this.handleTableInput.bind(this);
//        this.handleTableClick = this.handleTableClick.bind(this);
//        this.handleDiscountInput = this.handleDiscountInput.bind(this);

//        this.initialize();
//    }

//    initialize() {
//        this.findCurrentMaxSODtlId();
//        this.renderInitialRows();
//        this.addEventListeners();
//        this.calculateTotals();

//        if (this.mode === "delete" || this.mode === "detail") {
//            if (this.addRowBtn) {
//                this.addRowBtn.style.display = "none";
//            }
//        }
//    }

//    findCurrentMaxSODtlId() {
//        // Among prefilled SODtl rows, find the max "Id" field
//        let maxId = 0;
//        this.prefilledRows.forEach((row) => {
//            if (row.id && row.id > maxId) {
//                maxId = row.id;
//            }
//        });
//        this.currentMaxSODtlId = maxId;
//    }

//    updateData(newOptions, newNameAttributeOptionObject) {
//        this.removeEventListeners();

//        this.productsData = newOptions.productsData || this.productsData;
//        this.prefilledRows = newOptions.prefilledRows || [];
//        this.isEditMode = newOptions.isEditMode || this.isEditMode;

//        this.subtotalId = newOptions.subtotalId || this.subtotalId;
//        this.totalId = newOptions.totalId || this.totalId;
//        this.discountId = newOptions.discountId || this.discountId;
//        this.isDiscountAllowed =
//            newOptions.isDiscountAllowed !== undefined ? newOptions.isDiscountAllowed : this.isDiscountAllowed;

//        this.mode = newOptions.mode || this.mode;
//        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

//        this.taxMasterData = newOptions.taxMasterData || this.taxMasterData;
//        this.prefilledTaxMaster = newOptions.prefilledTaxMaster || this.prefilledTaxMaster;

//        if (newNameAttributeOptionObject) {
//            this.nameAttributeOptionObject = newNameAttributeOptionObject;
//        }

//        this.productTableBody.innerHTML = "";
//        this.selectedProductIds.clear();

//        // Re-initialize
//        this.findCurrentMaxSODtlId();
//        this.initialize();
//    }

//    removeEventListeners() {
//        this.productTableBody.removeEventListener("input", this.handleTableInput);
//        this.productTableBody.removeEventListener("click", this.handleTableClick);

//        if (this.addRowBtn) {
//            this.addRowBtn.removeEventListener("click", this.handleAddRow);
//        }

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.removeEventListener("input", this.handleDiscountInput);
//            }
//        }
//    }

//    findFieldName(dataObj, possibleFields) {
//        for (let field of possibleFields) {
//            if (field.toLowerCase() in dataObj) {
//                return field.toLowerCase();
//            }
//        }
//        return null;
//    }

//    normalizeKeys(obj) {
//        const normalized = {};
//        for (const key in obj) {
//            normalized[key.toLowerCase()] = obj[key];
//        }
//        return normalized;
//    }

//    renderInitialRows() {
//        this.productTableBody.innerHTML = "";

//        if (this.prefilledRows.length > 0) {
//            this.prefilledRows.forEach((row) => {
//                const productRow = this.renderRow(row);
//                // Fill in sub-rows for taxes
//                if (row.Id) {
//                    const matchingTaxes = this.prefilledTaxMaster.filter(t => Number(t.SODtlId) === Number(row.Id));
//                    if (matchingTaxes.length > 0) {
//                        matchingTaxes.forEach(taxDto => {
//                            this.renderTaxRow(productRow, taxDto);
//                        });
//                    }
//                }
//            });
//        } else if (this.mode === "create") {
//            // If creating from scratch, we can add one initial row
//            this.renderRow();
//        }

//        this.updateRowIndices();
//    }

//    /*********************************************************************************************
//     * RENDER PRODUCT ROW
//     *********************************************************************************************/
//    renderRow(data = null) {
//        const normalizedData = data ? this.normalizeKeys(data) : {};

//        // If data is existing => use that Id
//        // If data is null => brand new => generate a new ID
//        let sodtlIdValue = 0;
//        if (data && typeof data.id !== "undefined") {
//            sodtlIdValue = data.id;
//        } else {
//            // brand new row => increment
//            sodtlIdValue = ++this.currentMaxSODtlId;
//        }

//        let pQuantity = normalizedData["quantity"] ?? 0;
//        const productOptions = this.productsData
//            .map(
//                (product) =>
//                    `<option value="${product.value}"
//    ${normalizedData["productid"] == product.value ? "selected" : ""}>
//    ${product.text}
//</option>`
//            )
//            .join("");

//        const rowHTML = `
//<tr>
//    <!-- Hidden input for SODtlId -->
//    <td style="display:none;">
//        <input type="hidden" class="sodtl-id-input" value="${sodtlIdValue}">
//    </td>

//    <td>
//        <div class="tom-select-custom">
//            <select class="js-select form-select product-select">
//                <option value="">--- Select Product ---</option>
//                ${productOptions}
//            </select>
//        </div>
//    </td>
//    <td class="current-stock">${normalizedData["stock"] ?? "--"}</td>
//    <td class="unit-price">
//        ${(normalizedData["unitprice"] ?? 0).toFixed(2)}
//        <input type="hidden" class="unit-price-input"
//            value="${(normalizedData["unitprice"] ?? 0).toFixed(2)}">
//    </td>
//    <td>
//        <input type="number" class="form-control product-qty" min="0"
//            value="${pQuantity}">
//    </td>
//    <td class="product-total">
//        ${(normalizedData["total"] ?? 0).toFixed(2)}
//        <input type="hidden" class="product-total-input"
//            value="${(normalizedData["total"] ?? 0).toFixed(2)}">
//    </td>
//    <td class="product-after-vat">
//        ${(normalizedData["sodtltotalaftervat"] ?? 0).toFixed(2)}
//        <input type="hidden" class="product-after-vat-input"
//            value="${(normalizedData["sodtltotalaftervat"] ?? 0).toFixed(2)}">
//    </td>
//    <td>
//        ${this.getActionButtonHTML(data)}
//        <button type="button" class="btn btn-soft-info btn-sm add-vat-btn">
//            <i class="bi-plus-circle"></i> VAT
//        </button>
//    </td>
//</tr>
//`;

//        this.productTableBody.insertAdjacentHTML("beforeend", rowHTML);
//        const row = this.productTableBody.lastElementChild;
//        this.updateRowIndices();

//        // TomSelect
//        const productSelect = row.querySelector(this.selectors.productSelectClass);
//        new TomSelect(productSelect, {
//            placeholder: "--- Select Product ---",
//            maxItems: 1,
//            onChange: (value) => this.handleProductSelection(row, value, data),
//        });

//        if (data) {
//            productSelect.tomselect.setValue(normalizedData['productid']);
//        } else {
//            // brand new => quantity disabled until product is selected
//            row.querySelector(this.selectors.quantityInputClass).disabled = true;
//        }

//        // If row is from existing data, possibly disable if in certain modes
//        if (data) {
//            if (this.mode === "edit") {
//                this.adjustRowForEditTemplate(row);
//            } else if (this.mode === "delete" || this.mode === "detail") {
//                this.disableRow(row);
//            }
//        } else {
//            if (this.mode === "delete" || this.mode === "detail") {
//                this.disableRow(row);
//            }
//        }

//        return row;
//    }

//    /*********************************************************************************************
//     * RENDER TAX ROW
//     *********************************************************************************************/
//    renderTaxRow(productRow, taxData = null) {
//        // Insert the new row below the product row
//        let nextRow = productRow.nextElementSibling;
//        while (nextRow && nextRow.classList.contains("tax-row")) {
//            nextRow = nextRow.nextElementSibling;
//        }

//        const row = document.createElement("tr");
//        row.classList.add("tax-row");
//        row.style.backgroundColor = "#f1f8e9";
//        row.style.borderLeft = "4px solid #8bc34a";
//        row.style.fontSize = "0.95rem";

//        // Link to same SODtlId
//        const sodtlIdFromParent = productRow.querySelector(this.selectors.sodtlIdInputClass).value || 0;

//        row.innerHTML = `
//<td style="display:none;">
//    <input type="hidden" class="tax-id-input" value="${taxData?.Id || 0}">
//</td>
//<td colspan="4">
//    <div class="tom-select-custom d-flex justify-content-end">
//        <select class="form-select tax-master-select w-50">
//            <option value="">-- Select Tax --</option>
//            ${this.taxMasterData.map(t =>
//            `<option value="${t.Id}">${t.TaxName} (${t.TaxValue}${t.IsPercentage ? '%' : ''})</option>`
//        ).join("")
//            }
//        </select>
//        <input type="hidden" class="tax-sodtlid-input" value="${taxData?.SODtlId || sodtlIdFromParent}">
//        <input type="hidden" class="tax-soid-input" value="${taxData?.SOId || 0}">
//        <input type="hidden" class="tax-aftertaxamount-input" value="0.00">
//    </div>
//</td>
//<td>
//    <span class="tax-amount">0.00</span>
//    <input type="hidden" class="tax-amount-input" value="0.00">
//</td>
//<td>
//    <button type="button" class="btn btn-danger btn-sm delete-row">
//        <i class="bi-trash"></i>
//    </button>
//</td>
//`;

//        productRow.parentNode.insertBefore(row, nextRow);

//        // TomSelect for tax
//        const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
//        new TomSelect(taxSelect, {
//            placeholder: "-- Select Tax --",
//            maxItems: 1,
//            onChange: (taxId) => this.handleTaxSelection(row, productRow, taxId),
//        });

//        // Pre-populate if we have existing taxData
//        if (taxData) {
//            if (taxData.TaxId) {
//                taxSelect.tomselect.setValue(taxData.TaxId.toString());
//            }
//            row.querySelector(this.selectors.taxAmountClass).textContent = (taxData.TaxAmount || 0).toFixed(2);
//            row.querySelector(this.selectors.taxAmountInputClass).value = (taxData.TaxAmount || 0).toFixed(2);

//            if (typeof taxData.aftertaxamount !== "undefined") {
//                row.querySelector(this.selectors.taxAfterTaxAmtClass).value = taxData.aftertaxamount.toFixed(2);
//            }
//        }

//        return row;
//    }

//    getActionButtonHTML(data) {
//        // Same logic from your code
//        if (data) {
//            if (this.mode === 'edit') {
//                if (this.editTemplateType === 'Template_1') {
//                    return `
//                        <div class="btn-group" role="group">
//                            <button type="button" class="btn btn-white btn-sm edit-row">
//                                <i class="bi-pencil-fill me-1"></i> Edit
//                            </button>
//                            <div class="btn-group">
//                                <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty"
//                                    data-bs-toggle="dropdown" aria-expanded="false"></button>
//                                <div class="dropdown-menu dropdown-menu-end mt-1">
//                                    <a class="dropdown-item delete-row" href="#">
//                                        <i class="bi-trash dropdown-item-icon"></i> Delete
//                                    </a>
//                                </div>
//                            </div>
//                        </div>
//                    `;
//                }
//                else if (this.editTemplateType === 'Template_2') {
//                    return `
//                        <button class="btn btn-primary btn-sm edit-row">
//                            <i class="bi-pencil"></i>
//                        </button>
//                        <button class="btn btn-danger btn-sm delete-row">
//                            <i class="bi-trash"></i>
//                        </button>
//                    `;
//                }
//                else if (this.editTemplateType === 'Template_3') {
//                    return `
//                        <button class="btn btn-success btn-sm edit-row" type="button">
//                            <i class="bi-pencil"></i>
//                        </button>
//                        <button class="btn btn-danger btn-sm delete-row">
//                            <i class="bi-trash"></i>
//                        </button>
//                    `;
//                }
//                else {
//                    return `<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>`;
//                }
//            } else if (this.mode === 'delete' || this.mode === 'detail') {
//                return '';
//            }
//        }
//        return '<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
//    }

//    adjustRowForEditTemplate(row) {
//        if (this.editTemplateType === 'Template_1') {
//            this.disableRow(row);
//        } else if (this.editTemplateType === 'Template_2') {
//            this.disableRowForTemplate2(row);
//        } else if (this.editTemplateType === 'Template_3') {
//            this.disableRow(row);
//        }
//    }

//    handleProductSelection(row, productId, existingData = null) {
//        const previousProductId = row.dataset.previousProductId;
//        if (previousProductId) {
//            this.selectedProductIds.delete(parseInt(previousProductId));
//        }

//        const quantityInput = row.querySelector(this.selectors.quantityInputClass);
//        if (!productId || productId === "0") {
//            quantityInput.disabled = true;
//            quantityInput.value = "";
//            this.updateRowValues(row, null, existingData);
//            row.dataset.previousProductId = null;
//            this.calculateTotals();
//            return;
//        }

//        if (this.selectedProductIds.has(parseInt(productId))) {
//            alert("This product is already selected. Please choose another.");
//            row.querySelector(this.selectors.productSelectClass).tomselect.clear();
//            return;
//        }

//        this.selectedProductIds.add(parseInt(productId));
//        row.dataset.previousProductId = productId;
//        quantityInput.disabled = false;

//        const product = this.productsData.find((p) => p.value == productId) || {};
//        const productNormalized = this.normalizeKeys(product);

//        this.updateRowValues(row, productNormalized, existingData);
//        this.calculateTotals();
//    }

//    updateRowValues(row, data, existingData = null) {
//        const normalizedData = data || {};
//        const existingNormalizedData = existingData ? this.normalizeKeys(existingData) : {};
//        const nameAttributes = this.nameAttributeOptionObject;

//        const unitPriceCell = row.querySelector(this.selectors.unitPriceClass);
//        const unitPriceInput = unitPriceCell.querySelector(this.selectors.unitPriceInputClass);
//        const productTotalCell = row.querySelector(this.selectors.productTotalClass);
//        const productTotalInput = productTotalCell.querySelector(this.selectors.productTotalInputClass);
//        const quantityInput = row.querySelector(this.selectors.quantityInputClass);

//        if (data) {
//            const unitPriceField = this.findFieldName(normalizedData, this.unitPriceFields);
//            const stockField = this.findFieldName(normalizedData, this.stockFields);

//            const unitPrice = parseFloat(normalizedData[unitPriceField]) || 0;
//            const stock = normalizedData[stockField] !== undefined ? normalizedData[stockField] : "--";

//            unitPriceCell.firstChild.textContent = unitPrice.toFixed(2);
//            unitPriceInput.value = unitPrice.toFixed(2);

//            let quantity = parseFloat(existingNormalizedData[nameAttributes.quantity?.toLowerCase()]) || 0;
//            if (quantity === 0) {
//                quantityInput.value = 0;
//                productTotalCell.firstChild.textContent = "0.00";
//                productTotalInput.value = "0.00";
//            } else {
//                quantityInput.value = quantity;
//                const calculatedTotal = unitPrice * quantity;
//                productTotalCell.firstChild.textContent = calculatedTotal.toFixed(2);
//                productTotalInput.value = calculatedTotal.toFixed(2);
//            }
//            row.querySelector(this.selectors.currentStockClass).textContent = stock;
//        } else {
//            unitPriceCell.firstChild.textContent = "0.00";
//            unitPriceInput.value = "0.00";
//            productTotalCell.firstChild.textContent = "0.00";
//            productTotalInput.value = "0.00";
//            quantityInput.value = 0;
//            row.querySelector(this.selectors.currentStockClass).textContent = "--";
//        }
//    }

//    /************************************************************************************************
//     * IMPORTANT: handleTaxSelection - MODIFIED to reflect new logic for:
//     *   1) Checking if same tax is already applied to the same product row
//     *   2) Handling IsPercentage, IsExclusive, IsInclusive
//     ************************************************************************************************/
//    handleTaxSelection(taxRow, productRow, taxId) {
//        if (!taxId || taxId === "0") {
//            taxRow.querySelector(this.selectors.taxAmountClass).textContent = "0.00";
//            taxRow.querySelector(this.selectors.taxAmountInputClass).value = "0.00";
//            this.calculateTotals();
//            return;
//        }

//        const chosenTax = this.taxMasterData.find(t => t.Id == taxId);
//        if (!chosenTax) return;

//        // 2A. Validate if this exact tax is already chosen for this productRow
//        //     If so, alert and clear the selection.
//        const rowSiblings = [];
//        let nextSib = productRow.nextElementSibling;
//        while (nextSib && nextSib.classList.contains("tax-row")) {
//            rowSiblings.push(nextSib);
//            nextSib = nextSib.nextElementSibling;
//        }
//        for (let sibling of rowSiblings) {
//            if (sibling !== taxRow) {
//                const siblingTaxSelect = sibling.querySelector(this.selectors.taxMasterSelectClass);
//                if (siblingTaxSelect && siblingTaxSelect.value == taxId) {
//                    alert("This Tax is already applied to the same product. Please choose another tax.");
//                    // Clear the TomSelect
//                    taxRow.querySelector(this.selectors.taxMasterSelectClass).tomselect.clear();
//                    return;
//                }
//            }
//        }

//        // 2B. Compute tax based on IsPercentage / IsInclusive / IsExclusive
//        const productTotal = parseFloat(
//            productRow.querySelector(this.selectors.productTotalInputClass).value
//        ) || 0;

//        let computedTax = 0;
//        let afterTax = 0;
//        const taxValue = parseFloat(chosenTax.TaxValue) || 0;

//        // If it's a percentage
//        if (chosenTax.IsPercentage) {
//            if (chosenTax.IsExclusive) {
//                // tax = productTotal * (taxValue/100)
//                computedTax = (productTotal * taxValue) / 100;
//                afterTax = productTotal + computedTax;
//            } else if (chosenTax.IsInclusive) {
//                // productTotal is "already" inclusive of the tax, so we back-calculate:
//                // net = productTotal / (1 + (taxValue/100))
//                // tax = productTotal - net
//                const net = productTotal / (1 + (taxValue / 100));
//                computedTax = productTotal - net; // portion that is tax
//                afterTax = productTotal; // remains the same visually
//            } else {
//                // If neither is set, fallback to exclusive logic
//                computedTax = (productTotal * taxValue) / 100;
//                afterTax = productTotal + computedTax;
//            }
//        }
//        else {
//            // It's a fixed tax
//            if (chosenTax.IsExclusive) {
//                // we simply add that amount
//                computedTax = taxValue;
//                afterTax = productTotal + computedTax;
//            } else if (chosenTax.IsInclusive) {
//                // productTotal includes a fixed portion
//                // if productTotal >= taxValue, fine. Otherwise, cap
//                if (productTotal >= taxValue) {
//                    computedTax = taxValue;
//                    afterTax = productTotal; // no visible change
//                } else {
//                    // Edge case: if productTotal < fixed tax
//                    // We'll just say the entire productTotal is tax
//                    computedTax = productTotal;
//                    afterTax = productTotal;
//                }
//            } else {
//                // Fallback => treat as exclusive
//                computedTax = taxValue;
//                afterTax = productTotal + computedTax;
//            }
//        }

//        // Update DOM with computedTax
//        taxRow.querySelector(this.selectors.taxAmountClass).textContent = computedTax.toFixed(2);
//        taxRow.querySelector(this.selectors.taxAmountInputClass).value = computedTax.toFixed(2);

//        // This hidden field tracks the productTotal + tax, or productTotal alone if inclusive
//        const afterTaxInput = taxRow.querySelector(this.selectors.taxAfterTaxAmtClass);
//        if (afterTaxInput) {
//            afterTaxInput.value = afterTax.toFixed(2);
//        }

//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//     *  updateRowIndices:
//     *  Sets each product row's name: "SODtlDtos[i].XYZ"
//     *  Sets each tax row’s name:     "SODtlTaxDtos[j].XYZ".
//     *********************************************************************************************/
//    updateRowIndices() {
//        const nameAttrs = this.nameAttributeOptionObject;
//        this.globalTaxCounter = 0;

//        let productIndex = 0;
//        const rows = this.productTableBody.querySelectorAll("tr");
//        rows.forEach(row => {
//            if (!row.classList.contains("tax-row")) {
//                // It's a product row
//                row.setAttribute("data-product-index", productIndex);

//                // hidden ID
//                const sodtlIdInput = row.querySelector(this.selectors.sodtlIdInputClass);
//                if (sodtlIdInput) {
//                    sodtlIdInput.name = `${nameAttrs.base}[${productIndex}].Id`;
//                }

//                // product select
//                const productSelect = row.querySelector(this.selectors.productSelectClass);
//                productSelect.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.productId}`;

//                // quantity
//                const qtyInput = row.querySelector(this.selectors.quantityInputClass);
//                qtyInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.quantity}`;

//                // unit price
//                const upInput = row.querySelector(this.selectors.unitPriceInputClass);
//                upInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.unitPrice}`;

//                // total
//                const totalInput = row.querySelector(this.selectors.productTotalInputClass);
//                totalInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.total}`;

//                // afterVAT
//                const afterVat = row.querySelector(this.selectors.productAfterVatInputClass);
//                afterVat.name = `${nameAttrs.base}[${productIndex}].SODtlTotalAfterVAT`;

//                productIndex++;
//            }
//            else {
//                // It's a tax-row => SODtlTaxDtos
//                const taxIdInput = row.querySelector(this.selectors.taxIdInputClass);
//                const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
//                const taxAmtInput = row.querySelector(this.selectors.taxAmountInputClass);
//                const sodtlIdInput = row.querySelector(this.selectors.taxSODtlIdClass);
//                const soIdInput = row.querySelector(this.selectors.taxSOIdClass);
//                const afterTaxAmtInput = row.querySelector(this.selectors.taxAfterTaxAmtClass);

//                taxIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].Id`;
//                taxSelect.name = `SODtlTaxDtos[${this.globalTaxCounter}].TaxId`;
//                taxAmtInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].TaxAmount`;
//                sodtlIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].SODtlId`;
//                soIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].SOId`;
//                afterTaxAmtInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].AfterTaxAmount`;

//                this.globalTaxCounter++;
//            }
//        });
//    }

//    addEventListeners() {
//        this.productTableBody.addEventListener("input", this.handleTableInput);
//        this.productTableBody.addEventListener("click", this.handleTableClick);

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.addEventListener("input", this.handleDiscountInput);
//            }
//        }

//        if (this.addRowBtn) {
//            this.addRowBtn.addEventListener("click", this.handleAddRow);
//        }
//    }

//    handleAddRow(e) {
//        e.preventDefault();
//        if (this.mode === "delete" || this.mode === "detail") return;
//        this.renderRow();
//        this.updateRowIndices();
//    }

//    handleTableInput(e) {
//        if (e.target.classList.contains(this.selectors.quantityInputClass.substring(1))) {
//            const row = e.target.closest("tr");
//            const qty = parseFloat(e.target.value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const stockText = row.querySelector(this.selectors.currentStockClass).textContent;
//            const stock = parseFloat(stockText) || 0;

//            if (stockText !== "--" && qty > stock) {
//                alert("Quantity cannot exceed the available stock!");
//                e.target.value = 0;
//                this.updateRowValues(row, null, null);
//                this.calculateTotals();
//                return;
//            }

//            const totalPrice = qty * unitPrice;
//            row.querySelector(this.selectors.productTotalClass).firstChild.textContent = totalPrice.toFixed(2);
//            row.querySelector(this.selectors.productTotalInputClass).value = totalPrice.toFixed(2);

//            this.calculateTotals();
//        }
//    }

//    handleTableClick(e) {
//        const targetBtn = e.target.closest('button, a');
//        if (!targetBtn) return;

//        if (targetBtn.classList.contains('delete-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");

//            if (row.classList.contains("tax-row")) {
//                // Remove just the tax row
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            } else {
//                // Product row => remove product + its tax sub-rows
//                const productId = row.querySelector(this.selectors.productSelectClass).value;
//                this.selectedProductIds.delete(parseInt(productId));

//                let next = row.nextElementSibling;
//                while (next && next.classList.contains("tax-row")) {
//                    next.remove();
//                    next = row.nextElementSibling;
//                }
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            }
//        }
//        else if (targetBtn.classList.contains('edit-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.enterEditModeTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.enterEditModeTemplate3(row);
//            } else if (this.editTemplateType === 'Template_2') {
//                this.enableRow(row);
//            } else {
//                this.enableRow(row);
//            }
//        }
//        else if (targetBtn.classList.contains('save-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.saveRowTemplate1(row);
//            }
//        }
//        else if (targetBtn.classList.contains('check-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_3') {
//                this.saveRowTemplate3(row);
//            }
//        }
//        else if (targetBtn.classList.contains('reset-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.resetRowTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.resetRowTemplate3(row);
//            }
//        }
//        else if (targetBtn.classList.contains('disable-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            this.disableRow(row);
//        }
//        else if (targetBtn.classList.contains("add-vat-btn")) {
//            e.preventDefault();
//            const productRow = targetBtn.closest("tr");
//            this.renderTaxRow(productRow, null);
//            this.updateRowIndices();
//        }
//    }

//    handleDiscountInput(e) {
//        const discountInput = e.target;
//        const subtotalInput = document.getElementById(this.subtotalId);
//        if (subtotalInput) {
//            const subtotal = parseFloat(subtotalInput.value) || 0;
//            const discount = parseFloat(discountInput.value) || 0;

//            if (discount < 0 || discount > subtotal) {
//                alert("Invalid discount value!");
//                discountInput.value = "0.00";
//                this.calculateTotals();
//                return;
//            }
//        }
//        this.calculateTotals();
//    }

//    disableRow(row) {
//        const inputs = row.querySelectorAll('input, select');
//        inputs.forEach(input => {
//            if (input.name) {
//                const hiddenInput = document.createElement('input');
//                hiddenInput.type = 'hidden';
//                hiddenInput.name = input.name;
//                hiddenInput.value = input.value;
//                hiddenInput.classList.add('hidden-disabled-input');
//                hiddenInput.setAttribute('data-original-name', input.name.split('.').pop());
//                row.appendChild(hiddenInput);
//            }
//            input.disabled = true;

//            if (input.tomselect) {
//                input.tomselect.disable();
//            }
//        });
//    }

//    disableRowForTemplate2(row) {
//        this.disableRow(row);
//    }

//    enableRow(row) {
//        const inputs = row.querySelectorAll('input, select');
//        inputs.forEach(input => {
//            input.disabled = false;
//            if (input.tomselect) {
//                input.tomselect.enable();
//            }
//        });
//        const hiddenInputs = row.querySelectorAll('.hidden-disabled-input');
//        hiddenInputs.forEach(hiddenInput => {
//            hiddenInput.remove();
//        });
//    }

//    /*********************************************************************************************
//     *  Template_1
//     *********************************************************************************************/
//    enterEditModeTemplate1(row) {
//        this.enableRow(row);

//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('save-row');
//            editBtn.classList.add('btn-success');
//            editBtn.classList.remove('btn-white');
//        }

//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }
//    }

//    saveRowTemplate1(row) {
//        this.disableRow(row);

//        const saveBtn = row.querySelector('.save-row');
//        if (saveBtn) {
//            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
//            saveBtn.classList.remove('save-row');
//            saveBtn.classList.add('edit-row');
//            saveBtn.classList.remove('btn-success');
//            saveBtn.classList.add('btn-white');
//        }

//        const dropdownMenu = row.querySelector('.dropdown-menu');
//        if (dropdownMenu && !row.querySelector('.dropdown-item.reset-row')) {
//            const resetOptionHTML = `
//                <a class="dropdown-item reset-row" href="#">
//                    <i class="bi-arrow-clockwise dropdown-item-icon"></i> Reset
//                </a>
//            `;
//            dropdownMenu.insertAdjacentHTML('beforeend', resetOptionHTML);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate1(row) {
//        const originalData = JSON.parse(row.dataset.originalData || '{}');
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent =
//            parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent =
//            parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        delete row.dataset.originalData;
//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }

//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//     *  Template_3
//     *********************************************************************************************/
//    enterEditModeTemplate3(row) {
//        this.enableRow(row);

//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('check-row');
//            editBtn.classList.remove('btn-success');
//            editBtn.classList.add('btn-primary');
//        }

//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }
//    }

//    saveRowTemplate3(row) {
//        this.disableRow(row);

//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        }

//        if (!row.querySelector('.reset-row')) {
//            const resetBtn = document.createElement('button');
//            resetBtn.className = 'btn btn-warning btn-sm reset-row ms-1';
//            resetBtn.innerHTML = '<i class="bi-arrow-clockwise"></i>';
//            const deleteBtn = row.querySelector('.delete-row');
//            deleteBtn.insertAdjacentElement('afterend', resetBtn);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate3(row) {
//        const originalData = JSON.parse(row.dataset.originalData || '{}');
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent =
//            parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent =
//            parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }

//        const editBtn = row.querySelector('.edit-row');
//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        } else if (!editBtn) {
//            const btnContainer = row.querySelector('td:last-child');
//            const newEditBtn = document.createElement('button');
//            newEditBtn.className = 'btn btn-success btn-sm edit-row';
//            newEditBtn.innerHTML = '<i class="bi-pencil"></i>';
//            btnContainer.insertAdjacentElement('afterbegin', newEditBtn);
//        }

//        delete row.dataset.originalData;
//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//     *  CALCULATE TOTALS
//     *  Now includes a second grand total that accounts for VAT (i.e. sum of each row’s After-VAT).
//     *********************************************************************************************/
//    calculateTotals() {
//        // 1) For each product row, sum up taxes => PriceAfterVAT
//        const productRows = this.productTableBody.querySelectorAll("tr:not(.tax-row)");
//        productRows.forEach((pRow) => {
//            const totalPrice = parseFloat(
//                pRow.querySelector(this.selectors.productTotalInputClass).value
//            ) || 0;

//            let sumTax = 0;
//            let nextRow = pRow.nextElementSibling;
//            while (nextRow && nextRow.classList.contains("tax-row")) {
//                const tAmt = parseFloat(nextRow.querySelector(this.selectors.taxAmountInputClass).value) || 0;
//                sumTax += tAmt;
//                nextRow = nextRow.nextElementSibling;
//            }

//            const afterVat = totalPrice + sumTax;  // For exclusive taxes, we add them
//            pRow.querySelector(this.selectors.productAfterVatClass).firstChild.textContent =
//                afterVat.toFixed(2);
//            pRow.querySelector(this.selectors.productAfterVatInputClass).value =
//                afterVat.toFixed(2);
//        });

//        // 2) Subtotal / discount / final total (WITHOUT TAX)
//        let grandTotal = 0; // sum of product totals (ignore tax)
//        let totalQuantity = 0;
//        let totalUnitPrice = 0;

//        productRows.forEach((row) => {
//            const qty = parseFloat(row.querySelector(this.selectors.quantityInputClass).value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const totalPrice = qty * unitPrice;

//            grandTotal += totalPrice;
//            totalQuantity += qty;
//            totalUnitPrice += unitPrice;
//        });

//        const subtotal = grandTotal;
//        if (this.subtotalId) {
//            const subtotalEl = document.getElementById(this.subtotalId);
//            if (subtotalEl) {
//                subtotalEl.value = subtotal.toFixed(2);
//            }
//            document.querySelectorAll(this.selectors.subtotalHiddenClass).forEach((el) => {
//                el.value = subtotal.toFixed(2);
//            });
//            document.querySelectorAll(this.selectors.subtotalVisibleClass).forEach((el) => {
//                el.value = subtotal.toFixed(2);
//            });
//        }

//        let discount = 0;
//        if (this.isDiscountAllowed && this.discountId) {
//            discount = parseFloat(document.getElementById(this.discountId).value) || 0;
//        }

//        const total = subtotal - discount;
//        if (this.totalId) {
//            const totalEl = document.getElementById(this.totalId);
//            if (totalEl) {
//                totalEl.value = total.toFixed(2);
//            }
//        }

//        // Update existing summary fields
//        document.querySelector(this.selectors.grandTotalId).textContent = grandTotal.toFixed(2);
//        document.querySelector(this.selectors.totalQuantityId).textContent = totalQuantity;
//        document.querySelector(this.selectors.totalPerPiecePriceId).textContent =
//            totalQuantity > 0 ? (grandTotal / totalQuantity).toFixed(2) : "0.00";
//        document.querySelector(this.selectors.totalUnitPriceId).textContent = totalUnitPrice.toFixed(2);

//        document.querySelectorAll(this.selectors.totalHiddenClass).forEach((el) => {
//            el.value = total.toFixed(2);
//        });
//        document.querySelectorAll(this.selectors.totalVisibleClass).forEach((el) => {
//            el.value = total.toFixed(2);
//        });

//        // 3) NEW: Calculate the Grand Total AFTER VAT
//        let grandTotalAfterVat = 0;
//        productRows.forEach((pRow) => {
//            const rowAfterVat = parseFloat(
//                pRow.querySelector(this.selectors.productAfterVatInputClass).value
//            ) || 0;
//            grandTotalAfterVat += rowAfterVat;
//        });

//        // If you have a discount logic that also applies to after-VAT amounts, you can decide how:
//        // For simplicity, we’ll assume discount only applies to the base subtotal, not the total after VAT.
//        // But if you need discount from the afterVat total, adjust here.

//        // Display it in your new field #grandTotalAfterVat (make sure it exists in your HTML).
//        const afterVatEl = document.getElementById("grandTotalAfterVat");
//        if (afterVatEl) {
//            // If you want discount subtracted from afterVat as well, do so here
//            afterVatEl.value = grandTotalAfterVat.toFixed(2);
//            // Or if it's a <span>, do:
//            // afterVatEl.textContent = grandTotalAfterVat.toFixed(2);
//        }
//    }
//}



//=================================================================================
//================================== V2 ===========================================
//=================================================================================
//export class AdvancedDynamicTable {
//    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
//        this.selectors = {
//            productTableBody: document.getElementById(tableBodyId),
//            addRowBtn: document.getElementById(addButtonId),

//            subtotalId: options.subtotalId || null,
//            totalId: options.totalId || null,
//            discountId: options.discountId || null,

//            productSelectClass: ".product-select",
//            currentStockClass: ".current-stock",
//            unitPriceClass: ".unit-price",
//            unitPriceInputClass: ".unit-price-input",
//            quantityInputClass: ".product-qty",
//            productTotalClass: ".product-total",
//            productTotalInputClass: ".product-total-input",

//            grandTotalId: "#grandTotal",
//            totalQuantityId: "#totalQuantity",
//            totalPerPiecePriceId: "#totalPerPiecePrice",
//            totalUnitPriceId: "#totalUnitPrice",

//            // Hidden for Subtotal/Total
//            subtotalHiddenClass: ".subtotal-hidden",
//            subtotalVisibleClass: ".subtotal-visible",
//            totalHiddenClass: ".total-hidden",
//            totalVisibleClass: ".total-visible",

//            // PriceAfterVAT
//            productAfterVatClass: ".product-after-vat",
//            productAfterVatInputClass: ".product-after-vat-input",

//            // Tax sub-rows
//            taxRowClass: ".tax-row",
//            taxMasterSelectClass: ".tax-master-select",
//            taxAmountClass: ".tax-amount",
//            taxAmountInputClass: ".tax-amount-input",
//            taxSODtlIdClass: ".tax-sodtlid-input",
//            taxSOIdClass: ".tax-soid-input",
//            taxAfterTaxAmtClass: ".tax-aftertaxamount-input",

//            // (NEW) Hidden ID fields
//            sodtlIdInputClass: ".sodtl-id-input",
//            taxIdInputClass: ".tax-id-input",
//        };

//        this.nameAttributeOptionObject = nameAttributeOptionObject;

//        this.unitPriceFields = options.unitPriceFields || ["unitprice", "productprice", "price"];
//        this.stockFields = options.stockFields || ["stock", "currentstock", "quantityinstock"];
//        this.quantityFields = options.quantityFields || ["quantity", "qty"];
//        this.totalFields = options.totalFields || ["total", "totalprice"];

//        this.productTableBody = this.selectors.productTableBody;
//        this.addRowBtn = this.selectors.addRowBtn;

//        this.productsData = options.productsData || [];
//        this.prefilledRows = options.prefilledRows || [];
//        this.isEditMode = options.isEditMode || false;

//        this.subtotalId = this.selectors.subtotalId;
//        this.totalId = this.selectors.totalId;
//        this.discountId = this.selectors.discountId;
//        this.isDiscountAllowed = options.isDiscountAllowed || false;

//        this.selectedProductIds = new Set();

//        this.mode = options.mode || "create";
//        this.editTemplateType = options.editTemplateType || "Default";

//        // For multi-VAT
//        this.taxMasterData = options.taxMasterData || [];
//        this.prefilledTaxMaster = options.prefilledTaxMaster || [];
//        console.log(this.prefilledTaxMaster)

//        // (NEW) We'll keep track of the largest SODtlId we have from prefilledRows
//        this.currentMaxSODtlId = 0;

//        // Bind methods
//        this.handleAddRow = this.handleAddRow.bind(this);
//        this.handleTableInput = this.handleTableInput.bind(this);
//        this.handleTableClick = this.handleTableClick.bind(this);
//        this.handleDiscountInput = this.handleDiscountInput.bind(this);

//        this.initialize();
//    }

//    initialize() {
//        this.findCurrentMaxSODtlId();
//        this.renderInitialRows();
//        this.addEventListeners();
//        this.calculateTotals();

//        if (this.mode === "delete" || this.mode === "detail") {
//            if (this.addRowBtn) {
//                this.addRowBtn.style.display = "none";
//            }
//        }
//    }

//    findCurrentMaxSODtlId() {
//        // Among prefilled SODtl rows, find the max "Id" field
//        // so we can start incrementing from there for new items
//        let maxId = 0;
//        this.prefilledRows.forEach((row) => {
//            if (row.id && row.id > maxId) {
//                maxId = row.id;
//            }
//        });
//        this.currentMaxSODtlId = maxId;
//    }

//    updateData(newOptions, newNameAttributeOptionObject) {
//        this.removeEventListeners();

//        this.productsData = newOptions.productsData || this.productsData;
//        this.prefilledRows = newOptions.prefilledRows || [];
//        this.isEditMode = newOptions.isEditMode || this.isEditMode;

//        this.subtotalId = newOptions.subtotalId || this.subtotalId;
//        this.totalId = newOptions.totalId || this.totalId;
//        this.discountId = newOptions.discountId || this.discountId;
//        this.isDiscountAllowed =
//            newOptions.isDiscountAllowed !== undefined ? newOptions.isDiscountAllowed : this.isDiscountAllowed;

//        this.mode = newOptions.mode || this.mode;
//        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

//        this.taxMasterData = newOptions.taxMasterData || this.taxMasterData;
//        this.prefilledTaxMaster = newOptions.prefilledTaxMaster || this.prefilledTaxMaster;

//        if (newNameAttributeOptionObject) {
//            this.nameAttributeOptionObject = newNameAttributeOptionObject;
//        }

//        this.productTableBody.innerHTML = "";
//        this.selectedProductIds.clear();

//        // Re-initialize
//        this.findCurrentMaxSODtlId();
//        this.initialize();
//    }

//    removeEventListeners() {
//        this.productTableBody.removeEventListener("input", this.handleTableInput);
//        this.productTableBody.removeEventListener("click", this.handleTableClick);

//        if (this.addRowBtn) {
//            this.addRowBtn.removeEventListener("click", this.handleAddRow);
//        }

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.removeEventListener("input", this.handleDiscountInput);
//            }
//        }
//    }

//    findFieldName(dataObj, possibleFields) {
//        for (let field of possibleFields) {
//            if (field.toLowerCase() in dataObj) {
//                return field.toLowerCase();
//            }
//        }
//        return null;
//    }

//    normalizeKeys(obj) {
//        const normalized = {};
//        for (const key in obj) {
//            normalized[key.toLowerCase()] = obj[key];
//        }
//        return normalized;
//    }

//    renderInitialRows() {
//        this.productTableBody.innerHTML = "";

//        if (this.prefilledRows.length > 0) {
//            this.prefilledRows.forEach((row) => {
//                const productRow = this.renderRow(row);
//                // Fill in sub-rows for taxes
//                if (row.Id) {
//                    /*const matchingTaxes = this.prefilledTaxMaster.filter(t => t.sodtlid === row.id);*/
//                    const matchingTaxes = this.prefilledTaxMaster.filter(t => Number(t.SODtlId) == Number(row.Id));
//                    if (matchingTaxes.length > 0) {
//                        matchingTaxes.forEach(taxDto => {
//                            this.renderTaxRow(productRow, taxDto);
//                        });
//                    }
//                }
//            });
//        } else if (this.mode === "create") {
//            // If creating from scratch, we can add one initial row
//            this.renderRow();
//        }

//        this.updateRowIndices();
//    }

//    /*********************************************************************************************
//        * RENDER PRODUCT ROW
//        *********************************************************************************************/
//    renderRow(data = null) {
//        const normalizedData = data ? this.normalizeKeys(data) : {};

//        // If data is existing => use that Id
//        // If data is null => brand new => generate a new ID
//        let sodtlIdValue = 0;
//        if (data && typeof data.id !== "undefined") {
//            sodtlIdValue = data.id;
//        } else {
//            // brand new row => increment the currentMaxSODtlId
//            sodtlIdValue = ++this.currentMaxSODtlId;
//        }

//        let pQuantity = normalizedData["quantity"] ?? 0;
//        const productOptions = this.productsData
//            .map(
//                (product) =>
//                    `<option value="${product.value}"
//    ${normalizedData["productid"] == product.value ? "selected" : ""}>
//    ${product.text}
//</option>`
//            )
//            .join("");

//        const rowHTML = `
//<tr>
//    <!-- Hidden input for SODtlId -->
//    <td style="display:none;">
//        <input type="hidden" class="sodtl-id-input" value="${sodtlIdValue}">
//    </td>

//    <td>
//        <div class="tom-select-custom">
//            <select class="js-select form-select product-select">
//                <option value="">--- Select Product ---</option>
//                ${productOptions}
//            </select>
//        </div>
//    </td>
//    <td class="current-stock">${normalizedData["stock"] ?? "--"}</td>
//    <td class="unit-price">
//        ${(normalizedData["unitprice"] ?? 0).toFixed(2)}
//        <input type="hidden" class="unit-price-input"
//            value="${(normalizedData[" unitprice"] ?? 0).toFixed(2)}">
//    </td>
//    <td>
//        <input type="number" class="form-control product-qty" min="0"
//            value="${pQuantity}">
//    </td>
//    <td class="product-total">
//        ${(normalizedData["total"] ?? 0).toFixed(2)}
//        <input type="hidden" class="product-total-input"
//            value="${(normalizedData[" total"] ?? 0).toFixed(2)}">
//    </td>
//    <td class="product-after-vat">
//        ${(normalizedData["sodtltotalaftervat"] ?? 0).toFixed(2)}
//        <input type="hidden" class="product-after-vat-input"
//            value="${(normalizedData[" sodtltotalaftervat"] ?? 0).toFixed(2)}">
//    </td>
//    <td>
//        ${this.getActionButtonHTML(data)}
//        <button type="button" class="btn btn-soft-info btn-sm add-vat-btn">
//            <i class="bi-plus-circle"></i> VAT
//        </button>
//    </td>
//</tr>
//`;

//        this.productTableBody.insertAdjacentHTML("beforeend", rowHTML);
//        const row = this.productTableBody.lastElementChild;
//        this.updateRowIndices();

//        // TomSelect
//        const productSelect = row.querySelector(this.selectors.productSelectClass);
//        new TomSelect(productSelect, {
//            placeholder: "--- Select Product ---",
//            maxItems: 1,
//            onChange: (value) => this.handleProductSelection(row, value, data),
//        });

//        if (data) {
//            productSelect.tomselect.setValue(normalizedData['productid']);
//        } else {
//            // brand new => quantity disabled
//            row.querySelector(this.selectors.quantityInputClass).disabled = true;
//        }

//        if (data) {
//            if (this.mode === "edit") {
//                this.adjustRowForEditTemplate(row);
//            } else if (this.mode === "delete" || this.mode === "detail") {
//                this.disableRow(row);
//            }
//        } else {
//            if (this.mode === "delete" || this.mode === "detail") {
//                this.disableRow(row);
//            }
//        }

//        return row;
//    }

//    /*********************************************************************************************
//        *  RENDER TAX ROW
//        *********************************************************************************************/
//    renderTaxRow(productRow, taxData = null) {
//        // Insert the new row below the product row
//        let nextRow = productRow.nextElementSibling;
//        while (nextRow && nextRow.classList.contains("tax-row")) {
//            nextRow = nextRow.nextElementSibling;
//        }

//        const row = document.createElement("tr");
//        row.classList.add("tax-row");
//        row.style.backgroundColor = "#f1f8e9";
//        row.style.borderLeft = "4px solid #8bc34a";
//        row.style.fontSize = "0.95rem";

//        // We want the tax row to link to the same SODtlId as its parent:
//        const sodtlIdFromParent = productRow.querySelector(this.selectors.sodtlIdInputClass).value || 0;

//        row.innerHTML = `
//<td style="display:none;">
//    <input type="hidden" class="tax-id-input" value="${taxData?.Id || 0}">
//</td>
//<td colspan="4">
//    <div class="tom-select-custom d-flex justify-content-end">
//        <select class="form-select tax-master-select w-50">
//            <option value="">-- Select Tax --</option>
//            ${this.taxMasterData.map(t =>
//            `<option value="${t.Id}">${t.TaxName} (${t.TaxValue}%)</option>`
//        ).join("")
//            }
//        </select>
//        <input type="hidden" class="tax-sodtlid-input" value="${taxData?.SODtlId || sodtlIdFromParent}">
//            <input type="hidden" class="tax-soid-input" value="${taxData?.SOId || 0}">
//                <input type="hidden" class="tax-aftertaxamount-input" value="0.00">
//                </div>
//            </td>
//            <td>
//                <span class="tax-amount">0.00</span>
//                <input type="hidden" class="tax-amount-input" value="0.00">
//            </td>
//            <td>
//                <button type="button" class="btn btn-danger btn-sm delete-row">
//                    <i class="bi-trash"></i>
//                </button>
//            </td>
//            `;

//        productRow.parentNode.insertBefore(row, nextRow);

//        // TomSelect for tax
//        const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
//        new TomSelect(taxSelect, {
//            placeholder: "-- Select Tax --",
//            maxItems: 1,
//            onChange: (taxId) => this.handleTaxSelection(row, productRow, taxId),
//        });

//        if (taxData) {
//            if (taxData.TaxId) {
//                taxSelect.tomselect.setValue(taxData.TaxId.toString());
//            }
//            row.querySelector(this.selectors.taxAmountClass).textContent = (taxData.TaxAmount || 0).toFixed(2);
//            row.querySelector(this.selectors.taxAmountInputClass).value = (taxData.TaxAmount || 0).toFixed(2);

//            if (typeof taxData.aftertaxamount !== "undefined") {
//                row.querySelector(this.selectors.taxAfterTaxAmtClass).value = taxData.aftertaxamount.toFixed(2);
//            }
//        }

//        return row;
//    }

//    getActionButtonHTML(data) {
//        // ... same logic from your code ...
//        if (data) {
//            if (this.mode === 'edit') {
//                if (this.editTemplateType === 'Template_1') {
//                    return `
//            <div class="btn-group" role="group">
//                <button type="button" class="btn btn-white btn-sm edit-row">
//                    <i class="bi-pencil-fill me-1"></i> Edit
//                </button>
//                <div class="btn-group">
//                    <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty"
//                        data-bs-toggle="dropdown" aria-expanded="false"></button>
//                    <div class="dropdown-menu dropdown-menu-end mt-1">
//                        <a class="dropdown-item delete-row" href="#">
//                            <i class="bi-trash dropdown-item-icon"></i> Delete
//                        </a>
//                    </div>
//                </div>
//            </div>
//            `;
//                }
//                else if (this.editTemplateType === 'Template_2') {
//                    return `
//            <button class="btn btn-primary btn-sm edit-row">
//                <i class="bi-pencil"></i>
//            </button>
//            <button class="btn btn-danger btn-sm delete-row">
//                <i class="bi-trash"></i>
//            </button>
//            `;
//                }
//                else if (this.editTemplateType === 'Template_3') {
//                    return `
//            <button class="btn btn-success btn-sm edit-row" type="button">
//                <i class="bi-pencil"></i>
//            </button>
//            <button class="btn btn-danger btn-sm delete-row">
//                <i class="bi-trash"></i>
//            </button>
//            `;
//                }
//                else {
//                    return `<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>`;
//                }
//            } else if (this.mode === 'delete' || this.mode === 'detail') {
//                return '';
//            }
//        }
//        return '<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
//    }

//    adjustRowForEditTemplate(row) {
//        // same logic from your code
//        if (this.editTemplateType === 'Template_1') {
//            this.disableRow(row);
//        } else if (this.editTemplateType === 'Template_2') {
//            this.disableRowForTemplate2(row);
//        } else if (this.editTemplateType === 'Template_3') {
//            this.disableRow(row);
//        }
//    }

//    handleProductSelection(row, productId, existingData = null) {
//        const previousProductId = row.dataset.previousProductId;
//        if (previousProductId) {
//            this.selectedProductIds.delete(parseInt(previousProductId));
//        }

//        const quantityInput = row.querySelector(this.selectors.quantityInputClass);
//        if (!productId || productId === "0") {
//            quantityInput.disabled = true;
//            quantityInput.value = "";
//            this.updateRowValues(row, null, existingData);
//            row.dataset.previousProductId = null;
//            this.calculateTotals();
//            return;
//        }

//        if (this.selectedProductIds.has(parseInt(productId))) {
//            alert("This product is already selected. Please choose another.");
//            row.querySelector(this.selectors.productSelectClass).tomselect.clear();
//            return;
//        }

//        this.selectedProductIds.add(parseInt(productId));
//        row.dataset.previousProductId = productId;
//        quantityInput.disabled = false;

//        const product = this.productsData.find((p) => p.value == productId) || {};
//        const productNormalized = this.normalizeKeys(product);

//        this.updateRowValues(row, productNormalized, existingData);
//        this.calculateTotals();
//    }

//    updateRowValues(row, data, existingData = null) {
//        const normalizedData = data || {};
//        const existingNormalizedData = existingData ? this.normalizeKeys(existingData) : {};
//        const nameAttributes = this.nameAttributeOptionObject;

//        const unitPriceCell = row.querySelector(this.selectors.unitPriceClass);
//        const unitPriceInput = unitPriceCell.querySelector(this.selectors.unitPriceInputClass);
//        const productTotalCell = row.querySelector(this.selectors.productTotalClass);
//        const productTotalInput = productTotalCell.querySelector(this.selectors.productTotalInputClass);
//        const quantityInput = row.querySelector(this.selectors.quantityInputClass);

//        if (data) {
//            const unitPriceField = this.findFieldName(normalizedData, this.unitPriceFields);
//            const stockField = this.findFieldName(normalizedData, this.stockFields);

//            const unitPrice = parseFloat(normalizedData[unitPriceField]) || 0;
//            const stock = normalizedData[stockField] !== undefined ? normalizedData[stockField] : "--";

//            unitPriceCell.firstChild.textContent = unitPrice.toFixed(2);
//            unitPriceInput.value = unitPrice.toFixed(2);

//            let quantity = parseFloat(existingNormalizedData[nameAttributes.quantity?.toLowerCase()]) || 0;
//            if (quantity === 0) {
//                quantityInput.value = 0;
//                productTotalCell.firstChild.textContent = "0.00";
//                productTotalInput.value = "0.00";
//            } else {
//                quantityInput.value = quantity;
//                const calculatedTotal = unitPrice * quantity;
//                productTotalCell.firstChild.textContent = calculatedTotal.toFixed(2);
//                productTotalInput.value = calculatedTotal.toFixed(2);
//            }
//            row.querySelector(this.selectors.currentStockClass).textContent = stock;
//        } else {
//            unitPriceCell.firstChild.textContent = "0.00";
//            unitPriceInput.value = "0.00";
//            productTotalCell.firstChild.textContent = "0.00";
//            productTotalInput.value = "0.00";
//            quantityInput.value = 0;
//            row.querySelector(this.selectors.currentStockClass).textContent = "--";
//        }
//    }

//    handleTaxSelection(taxRow, productRow, taxId) {
//        if (!taxId || taxId === "0") {
//            taxRow.querySelector(this.selectors.taxAmountClass).textContent = "0.00";
//            taxRow.querySelector(this.selectors.taxAmountInputClass).value = "0.00";
//            this.calculateTotals();
//            return;
//        }
//        const chosenTax = this.taxMasterData.find(t => t.Id == taxId);
//        if (!chosenTax) return;

//        const taxValue = parseFloat(chosenTax.TaxValue) || 0;
//        const productTotal = parseFloat(productRow.querySelector(this.selectors.productTotalInputClass).value) || 0;
//        const computedTax = (productTotal * taxValue) / 100.0;

//        taxRow.querySelector(this.selectors.taxAmountClass).textContent = computedTax.toFixed(2);
//        taxRow.querySelector(this.selectors.taxAmountInputClass).value = computedTax.toFixed(2);

//        const afterTaxInput = taxRow.querySelector(this.selectors.taxAfterTaxAmtClass);
//        if (afterTaxInput) {
//            const afterTax = productTotal + computedTax;
//            afterTaxInput.value = afterTax.toFixed(2);
//        }

//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//        *  updateRowIndices:
//        *  This sets each row’s `name="SODtlDtos[i].XYZ"` and each tax row’s `name="SODtlTaxDtos[j].XYZ"`.
//        *********************************************************************************************/
//    updateRowIndices() {
//        const nameAttrs = this.nameAttributeOptionObject;
//        this.globalTaxCounter = 0;

//        let productIndex = 0;
//        const rows = this.productTableBody.querySelectorAll("tr");
//        rows.forEach(row => {
//            if (!row.classList.contains("tax-row")) {
//                // It's a product row
//                row.setAttribute("data-product-index", productIndex);

//                // hidden ID
//                const sodtlIdInput = row.querySelector(this.selectors.sodtlIdInputClass);
//                if (sodtlIdInput) {
//                    sodtlIdInput.name = `${nameAttrs.base}[${productIndex}].Id`;
//                }

//                // product select
//                const productSelect = row.querySelector(this.selectors.productSelectClass);
//                productSelect.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.productId}`;

//                // quantity
//                const qtyInput = row.querySelector(this.selectors.quantityInputClass);
//                qtyInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.quantity}`;

//                // unit price
//                const upInput = row.querySelector(this.selectors.unitPriceInputClass);
//                upInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.unitPrice}`;

//                // total
//                const totalInput = row.querySelector(this.selectors.productTotalInputClass);
//                totalInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.total}`;

//                // afterVAT
//                const afterVat = row.querySelector(this.selectors.productAfterVatInputClass);
//                afterVat.name = `${nameAttrs.base}[${productIndex}].SODtlTotalAfterVAT`;

//                productIndex++;
//            }
//            else {
//                // It's a tax-row => SODtlTaxDtos
//                const taxIdInput = row.querySelector(this.selectors.taxIdInputClass);
//                const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
//                const taxAmtInput = row.querySelector(this.selectors.taxAmountInputClass);
//                const sodtlIdInput = row.querySelector(this.selectors.taxSODtlIdClass);
//                const soIdInput = row.querySelector(this.selectors.taxSOIdClass);
//                const afterTaxAmtInput = row.querySelector(this.selectors.taxAfterTaxAmtClass);

//                taxIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].Id`;
//                taxSelect.name = `SODtlTaxDtos[${this.globalTaxCounter}].TaxId`;
//                taxAmtInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].TaxAmount`;
//                sodtlIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].SODtlId`;
//                soIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].SOId`;
//                afterTaxAmtInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].AfterTaxAmount`;

//                this.globalTaxCounter++;
//            }
//        });
//    }

//    addEventListeners() {
//        this.productTableBody.addEventListener("input", this.handleTableInput);
//        this.productTableBody.addEventListener("click", this.handleTableClick);

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.addEventListener("input", this.handleDiscountInput);
//            }
//        }

//        if (this.addRowBtn) {
//            this.addRowBtn.addEventListener("click", this.handleAddRow);
//        }
//    }

//    handleAddRow(e) {
//        e.preventDefault();
//        if (this.mode === "delete" || this.mode === "detail") return;
//        this.renderRow();
//        this.updateRowIndices();
//    }

//    handleTableInput(e) {
//        if (e.target.classList.contains(this.selectors.quantityInputClass.substring(1))) {
//            const row = e.target.closest("tr");
//            const qty = parseFloat(e.target.value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const stockText = row.querySelector(this.selectors.currentStockClass).textContent;
//            const stock = parseFloat(stockText) || 0;

//            if (stockText !== "--" && qty > stock) {
//                alert("Quantity cannot exceed the available stock!");
//                e.target.value = 0;
//                this.updateRowValues(row, null, null);
//                this.calculateTotals();
//                return;
//            }

//            const totalPrice = qty * unitPrice;
//            row.querySelector(this.selectors.productTotalClass).firstChild.textContent = totalPrice.toFixed(2);
//            row.querySelector(this.selectors.productTotalInputClass).value = totalPrice.toFixed(2);

//            this.calculateTotals();
//        }
//    }

//    handleTableClick(e) {
//        const targetBtn = e.target.closest('button, a');
//        if (!targetBtn) return;

//        if (targetBtn.classList.contains('delete-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");

//            if (row.classList.contains("tax-row")) {
//                // Remove just the tax row
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            } else {
//                // Product row => remove product + its tax sub-rows
//                const productId = row.querySelector(this.selectors.productSelectClass).value;
//                this.selectedProductIds.delete(parseInt(productId));

//                let next = row.nextElementSibling;
//                while (next && next.classList.contains("tax-row")) {
//                    next.remove();
//                    next = row.nextElementSibling;
//                }
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            }
//        }
//        else if (targetBtn.classList.contains('edit-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.enterEditModeTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.enterEditModeTemplate3(row);
//            } else if (this.editTemplateType === 'Template_2') {
//                this.enableRow(row);
//            } else {
//                this.enableRow(row);
//            }
//        }
//        else if (targetBtn.classList.contains('save-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.saveRowTemplate1(row);
//            }
//        }
//        else if (targetBtn.classList.contains('check-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_3') {
//                this.saveRowTemplate3(row);
//            }
//        }
//        else if (targetBtn.classList.contains('reset-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.resetRowTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.resetRowTemplate3(row);
//            }
//        }
//        else if (targetBtn.classList.contains('disable-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            this.disableRow(row);
//        }
//        else if (targetBtn.classList.contains("add-vat-btn")) {
//            e.preventDefault();
//            const productRow = targetBtn.closest("tr");
//            this.renderTaxRow(productRow, null);
//            this.updateRowIndices();
//        }
//    }

//    handleDiscountInput(e) {
//        const discountInput = e.target;
//        const subtotalInput = document.getElementById(this.subtotalId);
//        if (subtotalInput) {
//            const subtotal = parseFloat(subtotalInput.value) || 0;
//            const discount = parseFloat(discountInput.value) || 0;

//            if (discount < 0 || discount > subtotal) {
//                alert("Invalid discount value!");
//                discountInput.value = "0.00";
//                this.calculateTotals();
//                return;
//            }
//        }
//        this.calculateTotals();
//    }

//    disableRow(row) {
//        const inputs = row.querySelectorAll('input, select');
//        inputs.forEach(input => {
//            if (input.name) {
//                const hiddenInput = document.createElement('input');
//                hiddenInput.type = 'hidden';
//                hiddenInput.name = input.name;
//                hiddenInput.value = input.value;
//                hiddenInput.classList.add('hidden-disabled-input');
//                hiddenInput.setAttribute('data-original-name', input.name.split('.').pop());
//                row.appendChild(hiddenInput);
//            }
//            input.disabled = true;

//            if (input.tomselect) {
//                input.tomselect.disable();
//            }
//        });
//    }

//    disableRowForTemplate2(row) {
//        this.disableRow(row);
//    }

//    enableRow(row) {
//        const inputs = row.querySelectorAll('input, select');
//        inputs.forEach(input => {
//            input.disabled = false;
//            if (input.tomselect) {
//                input.tomselect.enable();
//            }
//        });
//        const hiddenInputs = row.querySelectorAll('.hidden-disabled-input');
//        hiddenInputs.forEach(hiddenInput => {
//            hiddenInput.remove();
//        });
//    }

//    /*********************************************************************************************
//        *  Template_1
//        *********************************************************************************************/
//    enterEditModeTemplate1(row) {
//        this.enableRow(row);

//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('save-row');
//            editBtn.classList.add('btn-success');
//            editBtn.classList.remove('btn-white');
//        }

//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }
//    }

//    saveRowTemplate1(row) {
//        this.disableRow(row);

//        const saveBtn = row.querySelector('.save-row');
//        if (saveBtn) {
//            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
//            saveBtn.classList.remove('save-row');
//            saveBtn.classList.add('edit-row');
//            saveBtn.classList.remove('btn-success');
//            saveBtn.classList.add('btn-white');
//        }

//        const dropdownMenu = row.querySelector('.dropdown-menu');
//        if (dropdownMenu && !row.querySelector('.dropdown-item.reset-row')) {
//            const resetOptionHTML = `
//            <a class="dropdown-item reset-row" href="#">
//                <i class="bi-arrow-clockwise dropdown-item-icon"></i> Reset
//            </a>
//        `;
//            dropdownMenu.insertAdjacentHTML('beforeend', resetOptionHTML);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate1(row) {
//        const originalData = JSON.parse(row.dataset.originalData || '{}');
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent =
//            parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent =
//            parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        delete row.dataset.originalData;
//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }

//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//        *  Template_3
//        *********************************************************************************************/
//    enterEditModeTemplate3(row) {
//        this.enableRow(row);

//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('check-row');
//            editBtn.classList.remove('btn-success');
//            editBtn.classList.add('btn-primary');
//        }

//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }
//    }

//    saveRowTemplate3(row) {
//        this.disableRow(row);

//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        }

//        if (!row.querySelector('.reset-row')) {
//            const resetBtn = document.createElement('button');
//            resetBtn.className = 'btn btn-warning btn-sm reset-row ms-1';
//            resetBtn.innerHTML = '<i class="bi-arrow-clockwise"></i>';
//            const deleteBtn = row.querySelector('.delete-row');
//            deleteBtn.insertAdjacentElement('afterend', resetBtn);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate3(row) {
//        const originalData = JSON.parse(row.dataset.originalData || '{}');
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent =
//            parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent =
//            parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }

//        const editBtn = row.querySelector('.edit-row');
//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        } else if (!editBtn) {
//            const btnContainer = row.querySelector('td:last-child');
//            const newEditBtn = document.createElement('button');
//            newEditBtn.className = 'btn btn-success btn-sm edit-row';
//            newEditBtn.innerHTML = '<i class="bi-pencil"></i>';
//            btnContainer.insertAdjacentElement('afterbegin', newEditBtn);
//        }

//        delete row.dataset.originalData;
//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//        *  CALCULATE TOTALS
//        *********************************************************************************************/
//    calculateTotals() {
//        // 1) For each product row, sum up taxes => PriceAfterVAT
//        const productRows = this.productTableBody.querySelectorAll("tr:not(.tax-row)");
//        productRows.forEach((pRow) => {
//            const totalPrice = parseFloat(
//                pRow.querySelector(this.selectors.productTotalInputClass).value
//            ) || 0;

//            let sumTax = 0;
//            let nextRow = pRow.nextElementSibling;
//            while (nextRow && nextRow.classList.contains("tax-row")) {
//                const tAmt = parseFloat(nextRow.querySelector(this.selectors.taxAmountInputClass).value) || 0;
//                sumTax += tAmt;
//                nextRow = nextRow.nextElementSibling;
//            }

//            const afterVat = totalPrice + sumTax;
//            pRow.querySelector(this.selectors.productAfterVatClass).firstChild.textContent =
//                afterVat.toFixed(2);
//            pRow.querySelector(this.selectors.productAfterVatInputClass).value =
//                afterVat.toFixed(2);
//        });

//        // 2) Subtotal / discount / final total
//        let grandTotal = 0;
//        let totalQuantity = 0;
//        let totalUnitPrice = 0;

//        productRows.forEach((row) => {
//            const qty = parseFloat(row.querySelector(this.selectors.quantityInputClass).value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const totalPrice = qty * unitPrice;

//            grandTotal += totalPrice;
//            totalQuantity += qty;
//            totalUnitPrice += unitPrice;
//        });

//        const subtotal = grandTotal;
//        if (this.subtotalId) {
//            const subtotalEl = document.getElementById(this.subtotalId);
//            if (subtotalEl) {
//                subtotalEl.value = subtotal.toFixed(2);
//            }
//            document.querySelectorAll(this.selectors.subtotalHiddenClass).forEach((el) => (el.value = subtotal.toFixed(2)));
//            document.querySelectorAll(this.selectors.subtotalVisibleClass).forEach((el) => (el.value = subtotal.toFixed(2)));
//        }

//        let discount = 0;
//        if (this.isDiscountAllowed && this.discountId) {
//            discount = parseFloat(document.getElementById(this.discountId).value) || 0;
//        }

//        const total = subtotal - discount;
//        if (this.totalId) {
//            const totalEl = document.getElementById(this.totalId);
//            if (totalEl) {
//                totalEl.value = total.toFixed(2);
//            }
//        }

//        document.querySelector(this.selectors.grandTotalId).textContent = grandTotal.toFixed(2);
//        document.querySelector(this.selectors.totalQuantityId).textContent = totalQuantity;
//        document.querySelector(this.selectors.totalPerPiecePriceId).textContent =
//            totalQuantity > 0 ? (grandTotal / totalQuantity).toFixed(2) : "0.00";
//        document.querySelector(this.selectors.totalUnitPriceId).textContent = totalUnitPrice.toFixed(2);

//        document.querySelectorAll(this.selectors.totalHiddenClass).forEach((el) => (el.value = total.toFixed(2)));
//        document.querySelectorAll(this.selectors.totalVisibleClass).forEach((el) => (el.value = total.toFixed(2)));
//    }
//}





//=================================================================================
//================================== V1 ===========================================
//=================================================================================

//export class AdvancedDynamicTable {
//    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
//        this.selectors = {
//            productTableBody: document.getElementById(tableBodyId),
//            addRowBtn: document.getElementById(addButtonId),

//            subtotalId: options.subtotalId || null,
//            totalId: options.totalId || null,
//            discountId: options.discountId || null,

//            // Classes for product rows
//            productSelectClass: ".product-select",
//            currentStockClass: ".current-stock",
//            unitPriceClass: ".unit-price",
//            unitPriceInputClass: ".unit-price-input",
//            quantityInputClass: ".product-qty",
//            productTotalClass: ".product-total",
//            productTotalInputClass: ".product-total-input",

//            // Summary
//            grandTotalId: "#grandTotal",
//            totalQuantityId: "#totalQuantity",
//            totalPerPiecePriceId: "#totalPerPiecePrice",
//            totalUnitPriceId: "#totalUnitPrice",

//            // Hidden for Subtotal/Total
//            subtotalHiddenClass: ".subtotal-hidden",
//            subtotalVisibleClass: ".subtotal-visible",
//            totalHiddenClass: ".total-hidden",
//            totalVisibleClass: ".total-visible",

//            // PriceAfterVAT
//            productAfterVatClass: ".product-after-vat",
//            productAfterVatInputClass: ".product-after-vat-input",

//            // Tax sub-rows
//            taxRowClass: ".tax-row",
//            taxMasterSelectClass: ".tax-master-select",
//            taxAmountClass: ".tax-amount",
//            taxAmountInputClass: ".tax-amount-input",
//            taxSODtlIdClass: ".tax-sodtlid-input",
//            taxSOIdClass: ".tax-soid-input",
//            taxAfterTaxAmtClass: ".tax-aftertaxamount-input",

//            // (NEW) Hidden ID fields in product/tax rows
//            sodtlIdInputClass: ".sodtl-id-input",
//            taxIdInputClass: ".tax-id-input",
//        };

//        this.nameAttributeOptionObject = nameAttributeOptionObject;

//        // Possible field names
//        this.unitPriceFields = options.unitPriceFields || ["unitprice", "productprice", "price"];
//        this.stockFields = options.stockFields || ["stock", "currentstock", "quantityinstock"];
//        this.quantityFields = options.quantityFields || ["quantity", "qty"];
//        this.totalFields = options.totalFields || ["total", "totalprice"];

//        this.productTableBody = this.selectors.productTableBody;
//        this.addRowBtn = this.selectors.addRowBtn;

//        this.productsData = options.productsData || [];
//        this.prefilledRows = options.prefilledRows || [];
//        this.isEditMode = options.isEditMode || false;

//        this.subtotalId = this.selectors.subtotalId;
//        this.totalId = this.selectors.totalId;
//        this.discountId = this.selectors.discountId;
//        this.isDiscountAllowed = options.isDiscountAllowed || false;

//        this.selectedProductIds = new Set();

//        this.mode = options.mode || "create";
//        this.editTemplateType = options.editTemplateType || "Default";

//        // For multi-VAT
//        this.taxMasterData = options.taxMasterData || [];
//        this.prefilledTaxMaster = options.prefilledTaxMaster || [];

//        // Bind methods
//        this.handleAddRow = this.handleAddRow.bind(this);
//        this.handleTableInput = this.handleTableInput.bind(this);
//        this.handleTableClick = this.handleTableClick.bind(this);
//        this.handleDiscountInput = this.handleDiscountInput.bind(this);

//        this.initialize();
//    }

//    initialize() {
//        this.renderInitialRows();
//        this.addEventListeners();
//        this.calculateTotals();

//        if (this.mode === "delete" || this.mode === "detail") {
//            if (this.addRowBtn) {
//                this.addRowBtn.style.display = "none";
//            }
//        }
//    }

//    updateData(newOptions, newNameAttributeOptionObject) {
//        this.removeEventListeners();

//        this.productsData = newOptions.productsData || this.productsData;
//        this.prefilledRows = newOptions.prefilledRows || [];
//        this.isEditMode = newOptions.isEditMode || this.isEditMode;

//        this.subtotalId = newOptions.subtotalId || this.subtotalId;
//        this.totalId = newOptions.totalId || this.totalId;
//        this.discountId = newOptions.discountId || this.discountId;
//        this.isDiscountAllowed =
//            newOptions.isDiscountAllowed !== undefined ? newOptions.isDiscountAllowed : this.isDiscountAllowed;

//        this.mode = newOptions.mode || this.mode;
//        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

//        this.taxMasterData = newOptions.taxMasterData || this.taxMasterData;
//        this.prefilledTaxMaster = newOptions.prefilledTaxMaster || this.prefilledTaxMaster;

//        if (newNameAttributeOptionObject) {
//            this.nameAttributeOptionObject = newNameAttributeOptionObject;
//        }

//        this.productTableBody.innerHTML = "";
//        this.selectedProductIds.clear();
//        this.initialize();
//    }

//    removeEventListeners() {
//        this.productTableBody.removeEventListener("input", this.handleTableInput);
//        this.productTableBody.removeEventListener("click", this.handleTableClick);

//        if (this.addRowBtn) {
//            this.addRowBtn.removeEventListener("click", this.handleAddRow);
//        }

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.removeEventListener("input", this.handleDiscountInput);
//            }
//        }
//    }

//    findFieldName(dataObj, possibleFields) {
//        for (let field of possibleFields) {
//            if (field.toLowerCase() in dataObj) {
//                return field.toLowerCase();
//            }
//        }
//        return null;
//    }

//    normalizeKeys(obj) {
//        const normalized = {};
//        for (const key in obj) {
//            normalized[key.toLowerCase()] = obj[key];
//        }
//        return normalized;
//    }

//    renderInitialRows() {
//        this.productTableBody.innerHTML = "";

//        if (this.prefilledRows.length > 0) {
//            this.prefilledRows.forEach((row) => {
//                const productRow = this.renderRow(row);

//                if (row.id) {
//                    // Fill in sub-rows
//                    const matchingTaxes = this.prefilledTaxMaster.filter(t => t.sodtlid === row.id);
//                    if (matchingTaxes.length > 0) {
//                        matchingTaxes.forEach(taxDto => {
//                            this.renderTaxRow(productRow, taxDto);
//                        });
//                    }
//                }
//            });
//        } else if (this.mode === "create") {
//            this.renderRow();
//        }

//        this.updateRowIndices();
//    }

//    /*********************************************************************************************
//     *  RENDER PRODUCT ROW
//     *********************************************************************************************/
//    renderRow(data = null) {
//        const normalizedData = data ? this.normalizeKeys(data) : {};
//        let pQuantity = normalizedData["quantity"] ?? 0;

//        // Build product select options
//        const productOptions = this.productsData
//            .map(
//                (product) =>
//                    `<option value="${product.value}" 
//                        ${normalizedData["productid"] == product.value ? "selected" : ""}>
//                        ${product.text}
//                    </option>`
//            )
//            .join("");

//        // Insert row
//        const rowHTML = `
//        <tr>
//            <!-- (NEW) Hidden input for SODtlDto.Id -->
//            <td style="display:none;">
//              <input type="hidden" class="sodtl-id-input" 
//                     value="${normalizedData["id"] ?? 0}">
//            </td>

//            <td>
//                <div class="tom-select-custom">
//                    <select class="js-select form-select product-select">
//                        <option value="">--- Select Product ---</option>
//                        ${productOptions}
//                    </select>
//                </div>
//            </td>
//            <td class="current-stock">${normalizedData["stock"] ?? "--"}</td>
//            <td class="unit-price">
//                ${(normalizedData["unitprice"] ?? 0).toFixed(2)}
//                <input type="hidden" class="unit-price-input" 
//                       value="${(normalizedData["unitprice"] ?? 0).toFixed(2)}">
//            </td>
//            <td>
//                <input type="number" class="form-control product-qty" min="0" 
//                       value="${pQuantity}">
//            </td>
//            <td class="product-total">
//                ${(normalizedData["total"] ?? 0).toFixed(2)}
//                <input type="hidden" class="product-total-input" 
//                       value="${(normalizedData["total"] ?? 0).toFixed(2)}">
//            </td>
//            <td class="product-after-vat">
//                ${(normalizedData["sodtltotalaftervat"] ?? 0).toFixed(2)}
//                <input type="hidden" class="product-after-vat-input" 
//                       value="${(normalizedData["sodtltotalaftervat"] ?? 0).toFixed(2)}">
//            </td>
//            <td>
//                ${this.getActionButtonHTML(data)}
//                <button type="button" class="btn btn-soft-info btn-sm add-vat-btn">
//                    <i class="bi-plus-circle"></i> VAT
//                </button>
//            </td>
//        </tr>
//        `;

//        this.productTableBody.insertAdjacentHTML("beforeend", rowHTML);
//        const row = this.productTableBody.lastElementChild;
//        this.updateRowIndices();

//        // TomSelect
//        const productSelect = row.querySelector(this.selectors.productSelectClass);
//        new TomSelect(productSelect, {
//            placeholder: "--- Select Product ---",
//            maxItems: 1,
//            onChange: (value) => this.handleProductSelection(row, value, data),
//        });

//        if (data) {
//            productSelect.tomselect.setValue(normalizedData['productid']);
//        } else {
//            // brand new => quantity disabled
//            row.querySelector(this.selectors.quantityInputClass).disabled = true;
//        }

//        if (data) {
//            if (this.mode === "edit") {
//                this.adjustRowForEditTemplate(row);
//            } else if (this.mode === "delete" || this.mode === "detail") {
//                this.disableRow(row);
//            }
//        } else {
//            if (this.mode === "delete" || this.mode === "detail") {
//                this.disableRow(row);
//            }
//        }

//        return row;
//    }

//    /*********************************************************************************************
//     *  RENDER TAX ROW
//     *********************************************************************************************/
//    renderTaxRow(productRow, taxData = null) {
//        let nextRow = productRow.nextElementSibling;
//        while (nextRow && nextRow.classList.contains("tax-row")) {
//            nextRow = nextRow.nextElementSibling;
//        }

//        const row = document.createElement("tr");
//        row.classList.add("tax-row");

//        // Some minor styling
//        row.style.backgroundColor = "#f1f8e9";
//        row.style.borderLeft = "4px solid #8bc34a";
//        row.style.fontSize = "0.95rem";

//        row.innerHTML = `
//          <!-- (NEW) Hidden input for SODtlTaxDto.Id -->
//          <td style="display:none;">
//            <input type="hidden" class="tax-id-input" value="${taxData?.id || 0}">
//          </td>

//          <td colspan="4">
//            <div class="tom-select-custom d-flex justify-content-end ">
//              <select class="form-select tax-master-select w-75">
//                <option value="">-- Select Tax --</option>
//                ${this.taxMasterData.map(t =>
//            `<option value="${t.Id}">${t.TaxName} (${t.TaxValue}%)</option>`
//        ).join("")
//            }
//              </select>
//              <!-- Hidden inputs for SODtlTaxDto binding -->
//              <input type="hidden" class="tax-sodtlid-input" value="${taxData?.sodtlid || 0}">
//              <input type="hidden" class="tax-soid-input" value="${taxData?.soid || 0}">
//              <input type="hidden" class="tax-aftertaxamount-input" value="0.00">
//            </div>
//          </td>
//          <td>
//            <span class="tax-amount">0.00</span>
//            <input type="hidden" class="tax-amount-input" value="0.00">
//          </td>
//          <td>
//            <button type="button" class="btn btn-danger btn-sm delete-row">
//              <i class="bi-trash"></i>
//            </button>
//          </td>
//        `;

//        productRow.parentNode.insertBefore(row, nextRow);

//        // TomSelect for tax
//        const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
//        new TomSelect(taxSelect, {
//            placeholder: "-- Select Tax --",
//            maxItems: 1,
//            onChange: (taxId) => this.handleTaxSelection(row, productRow, taxId),
//        });

//        if (taxData) {
//            if (taxData.taxid) {
//                taxSelect.tomselect.setValue(taxData.taxid.toString());
//            }
//            row.querySelector(this.selectors.taxAmountClass).textContent = (taxData.taxamount || 0).toFixed(2);
//            row.querySelector(this.selectors.taxAmountInputClass).value = (taxData.taxamount || 0).toFixed(2);

//            if (typeof taxData.aftertaxamount !== "undefined") {
//                row.querySelector(this.selectors.taxAfterTaxAmtClass).value = taxData.aftertaxamount.toFixed(2);
//            }
//        }

//        return row;
//    }

//    getActionButtonHTML(data) {
//        if (data) {
//            if (this.mode === 'edit') {
//                if (this.editTemplateType === 'Template_1') {
//                    return `
//                    <div class="btn-group" role="group">
//                        <button type="button" class="btn btn-white btn-sm edit-row">
//                            <i class="bi-pencil-fill me-1"></i> Edit
//                        </button>
//                        <div class="btn-group">
//                            <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty"
//                                    data-bs-toggle="dropdown" aria-expanded="false"></button>
//                            <div class="dropdown-menu dropdown-menu-end mt-1">
//                                <a class="dropdown-item delete-row" href="#">
//                                    <i class="bi-trash dropdown-item-icon"></i> Delete
//                                </a>
//                            </div>
//                        </div>
//                    </div>
//                    `;
//                } else if (this.editTemplateType === 'Template_2') {
//                    return `
//                      <button class="btn btn-primary btn-sm edit-row">
//                        <i class="bi-pencil"></i>
//                      </button>
//                      <button class="btn btn-danger btn-sm delete-row">
//                        <i class="bi-trash"></i>
//                      </button>
//                    `;
//                } else if (this.editTemplateType === 'Template_3') {
//                    return `
//                      <button class="btn btn-success btn-sm edit-row" type="button">
//                        <i class="bi-pencil"></i>
//                      </button>
//                      <button class="btn btn-danger btn-sm delete-row">
//                        <i class="bi-trash"></i>
//                      </button>
//                    `;
//                } else {
//                    return `<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>`;
//                }
//            } else if (this.mode === 'delete' || this.mode === 'detail') {
//                return '';
//            }
//        }
//        return '<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
//    }

//    adjustRowForEditTemplate(row) {
//        if (this.editTemplateType === 'Template_1') {
//            this.disableRow(row);
//        } else if (this.editTemplateType === 'Template_2') {
//            this.disableRowForTemplate2(row);
//        } else if (this.editTemplateType === 'Template_3') {
//            this.disableRow(row);
//        }
//    }

//    handleProductSelection(row, productId, existingData = null) {
//        const previousProductId = row.dataset.previousProductId;
//        if (previousProductId) {
//            this.selectedProductIds.delete(parseInt(previousProductId));
//        }

//        const quantityInput = row.querySelector(this.selectors.quantityInputClass);
//        if (!productId || productId === "0") {
//            quantityInput.disabled = true;
//            quantityInput.value = "";
//            this.updateRowValues(row, null, existingData);
//            row.dataset.previousProductId = null;
//            this.calculateTotals();
//            return;
//        }

//        if (this.selectedProductIds.has(parseInt(productId))) {
//            alert("This product is already selected. Please choose another.");
//            row.querySelector(this.selectors.productSelectClass).tomselect.clear();
//            return;
//        }

//        this.selectedProductIds.add(parseInt(productId));
//        row.dataset.previousProductId = productId;
//        quantityInput.disabled = false;

//        const product = this.productsData.find((p) => p.value == productId) || {};
//        const productNormalized = this.normalizeKeys(product);

//        this.updateRowValues(row, productNormalized, existingData);
//        this.calculateTotals();
//    }

//    updateRowValues(row, data, existingData = null) {
//        const normalizedData = data || {};
//        const existingNormalizedData = existingData ? this.normalizeKeys(existingData) : {};
//        const nameAttributes = this.nameAttributeOptionObject;

//        const unitPriceCell = row.querySelector(this.selectors.unitPriceClass);
//        const unitPriceInput = unitPriceCell.querySelector(this.selectors.unitPriceInputClass);
//        const productTotalCell = row.querySelector(this.selectors.productTotalClass);
//        const productTotalInput = productTotalCell.querySelector(this.selectors.productTotalInputClass);
//        const quantityInput = row.querySelector(this.selectors.quantityInputClass);

//        if (data) {
//            const unitPriceField = this.findFieldName(normalizedData, this.unitPriceFields);
//            const stockField = this.findFieldName(normalizedData, this.stockFields);

//            const unitPrice = parseFloat(normalizedData[unitPriceField]) || 0;
//            const stock = normalizedData[stockField] !== undefined ? normalizedData[stockField] : "--";

//            unitPriceCell.firstChild.textContent = unitPrice.toFixed(2);
//            unitPriceInput.value = unitPrice.toFixed(2);

//            const qField = nameAttributes.quantity.toLowerCase();
//            let quantity = parseFloat(existingNormalizedData[qField]) || 0;

//            if (quantity === 0) {
//                quantityInput.value = 0;
//                productTotalCell.firstChild.textContent = "0.00";
//                productTotalInput.value = "0.00";
//            } else {
//                quantityInput.value = quantity;
//                const calculatedTotal = unitPrice * quantity;
//                productTotalCell.firstChild.textContent = calculatedTotal.toFixed(2);
//                productTotalInput.value = calculatedTotal.toFixed(2);
//            }
//            row.querySelector(this.selectors.currentStockClass).textContent = stock;
//        } else {
//            unitPriceCell.firstChild.textContent = "0.00";
//            unitPriceInput.value = "0.00";
//            productTotalCell.firstChild.textContent = "0.00";
//            productTotalInput.value = "0.00";
//            quantityInput.value = 0;
//            row.querySelector(this.selectors.currentStockClass).textContent = "--";
//        }
//    }

//    handleTaxSelection(taxRow, productRow, taxId) {
//        if (!taxId || taxId === "0") {
//            taxRow.querySelector(this.selectors.taxAmountClass).textContent = "0.00";
//            taxRow.querySelector(this.selectors.taxAmountInputClass).value = "0.00";
//            this.calculateTotals();
//            return;
//        }
//        const chosenTax = this.taxMasterData.find(t => t.Id == taxId);
//        if (!chosenTax) return;

//        const taxValue = parseFloat(chosenTax.TaxValue) || 0;
//        const productTotal = parseFloat(productRow.querySelector(this.selectors.productTotalInputClass).value) || 0;
//        const computedTax = (productTotal * taxValue) / 100.0;

//        taxRow.querySelector(this.selectors.taxAmountClass).textContent = computedTax.toFixed(2);
//        taxRow.querySelector(this.selectors.taxAmountInputClass).value = computedTax.toFixed(2);

//        const afterTaxInput = taxRow.querySelector(this.selectors.taxAfterTaxAmtClass);
//        if (afterTaxInput) {
//            const afterTax = productTotal + computedTax;
//            afterTaxInput.value = afterTax.toFixed(2);
//        }

//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//     *  updateRowIndices: CRITICAL for correct name="SODtlDtos[i]..."
//     *********************************************************************************************/
//    /*********************************************************************************************
//    * updateRowIndices - Flattened tax approach
//    *********************************************************************************************/
//    updateRowIndices() {
//        const nameAttrs = this.nameAttributeOptionObject;
//        // Example: { base: "SODtlDtos", productId: "ProductId", quantity: "SODtlQty", ... }

//        // We'll keep track of a global "taxCounter" across all product rows
//        // so each new tax row is SODtlTaxDtos[0], [1], [2], etc.
//        // We'll reset it here before we walk the rows:
//        this.globalTaxCounter = 0;

//        // Now let's index each product row
//        let productIndex = 0;

//        this.productTableBody.querySelectorAll("tr").forEach(row => {
//            if (!row.classList.contains("tax-row")) {
//                // It's a product row => SODtlDtos[productIndex]
//                row.setAttribute("data-product-index", productIndex);

//                // hidden ID
//                const sodtlIdInput = row.querySelector(this.selectors.sodtlIdInputClass);
//                if (sodtlIdInput) {
//                    sodtlIdInput.name = `${nameAttrs.base}[${productIndex}].Id`;
//                }

//                // product select
//                const productSelect = row.querySelector(this.selectors.productSelectClass);
//                productSelect.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.productId}`;

//                // quantity
//                const qtyInput = row.querySelector(this.selectors.quantityInputClass);
//                qtyInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.quantity}`;

//                // unit price
//                const upInput = row.querySelector(this.selectors.unitPriceInputClass);
//                upInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.unitPrice}`;

//                // total
//                const totalInput = row.querySelector(this.selectors.productTotalInputClass);
//                totalInput.name = `${nameAttrs.base}[${productIndex}].${nameAttrs.total}`;

//                // afterVAT
//                const afterVat = row.querySelector(this.selectors.productAfterVatInputClass);
//                afterVat.name = `${nameAttrs.base}[${productIndex}].SODtlTotalAfterVAT`;

//                // disabled hidden?
//                const hiddenDisabled = row.querySelectorAll(".hidden-disabled-input");
//                hiddenDisabled.forEach(h => {
//                    const orig = h.getAttribute("data-original-name");
//                    if (orig) {
//                        h.name = `${nameAttrs.base}[${productIndex}].${orig}`;
//                    }
//                });

//                productIndex++;
//            }
//            else {
//                // It's a tax-row => Flatten as SODtlTaxDtos[this.globalTaxCounter]
//                const taxIdInput = row.querySelector(this.selectors.taxIdInputClass);
//                const taxSelect = row.querySelector(this.selectors.taxMasterSelectClass);
//                const taxAmtInput = row.querySelector(this.selectors.taxAmountInputClass);
//                const sodtlIdInput = row.querySelector(this.selectors.taxSODtlIdClass);
//                const soIdInput = row.querySelector(this.selectors.taxSOIdClass);
//                const afterTaxAmtInput = row.querySelector(this.selectors.taxAfterTaxAmtClass);

//                taxIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].Id`;
//                taxSelect.name = `SODtlTaxDtos[${this.globalTaxCounter}].TaxId`;
//                taxAmtInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].TaxAmount`;
//                sodtlIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].SODtlId`;
//                soIdInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].SOId`;
//                afterTaxAmtInput.name = `SODtlTaxDtos[${this.globalTaxCounter}].AfterTaxAmount`;

//                this.globalTaxCounter++;
//            }
//        });
//    }

//    addEventListeners() {
//        this.productTableBody.addEventListener("input", this.handleTableInput);
//        this.productTableBody.addEventListener("click", this.handleTableClick);

//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.addEventListener("input", this.handleDiscountInput);
//            }
//        }

//        if (this.addRowBtn) {
//            this.addRowBtn.addEventListener("click", this.handleAddRow);
//        }
//    }

//    handleAddRow(e) {
//        e.preventDefault();
//        if (this.mode === "delete" || this.mode === "detail") return;
//        this.renderRow();
//        this.updateRowIndices();
//    }

//    handleTableInput(e) {
//        if (e.target.classList.contains(this.selectors.quantityInputClass.substring(1))) {
//            const row = e.target.closest("tr");
//            const qty = parseFloat(e.target.value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const stockText = row.querySelector(this.selectors.currentStockClass).textContent;
//            const stock = parseFloat(stockText) || 0;

//            if (stockText !== "--" && qty > stock) {
//                alert("Quantity cannot exceed the available stock!");
//                e.target.value = 0;
//                this.updateRowValues(row, null, null);
//                this.calculateTotals();
//                return;
//            }

//            const totalPrice = qty * unitPrice;
//            row.querySelector(this.selectors.productTotalClass).firstChild.textContent = totalPrice.toFixed(2);
//            row.querySelector(this.selectors.productTotalInputClass).value = totalPrice.toFixed(2);

//            this.calculateTotals();
//        }
//    }

//    handleTableClick(e) {
//        const targetBtn = e.target.closest('button, a');
//        if (!targetBtn) return;

//        if (targetBtn.classList.contains('delete-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");

//            if (row.classList.contains("tax-row")) {
//                // Remove just the tax row
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            } else {
//                // Product row => remove product + sub-rows
//                const productId = row.querySelector(this.selectors.productSelectClass).value;
//                this.selectedProductIds.delete(parseInt(productId));

//                let next = row.nextElementSibling;
//                while (next && next.classList.contains("tax-row")) {
//                    next.remove();
//                    next = row.nextElementSibling;
//                }
//                row.remove();
//                this.updateRowIndices();
//                this.calculateTotals();
//            }
//        }
//        else if (targetBtn.classList.contains('edit-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.enterEditModeTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.enterEditModeTemplate3(row);
//            } else if (this.editTemplateType === 'Template_2') {
//                this.enableRow(row);
//            } else {
//                this.enableRow(row);
//            }
//        }
//        else if (targetBtn.classList.contains('save-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.saveRowTemplate1(row);
//            }
//        }
//        else if (targetBtn.classList.contains('check-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_3') {
//                this.saveRowTemplate3(row);
//            }
//        }
//        else if (targetBtn.classList.contains('reset-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.resetRowTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.resetRowTemplate3(row);
//            }
//        }
//        else if (targetBtn.classList.contains('disable-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            this.disableRow(row);
//        }
//        else if (targetBtn.classList.contains("add-vat-btn")) {
//            e.preventDefault();
//            const productRow = targetBtn.closest("tr");
//            this.renderTaxRow(productRow, null);
//            this.updateRowIndices();
//        }
//    }

//    handleDiscountInput(e) {
//        const discountInput = e.target;
//        const subtotalInput = document.getElementById(this.subtotalId);
//        if (subtotalInput) {
//            const subtotal = parseFloat(subtotalInput.value) || 0;
//            const discount = parseFloat(discountInput.value) || 0;

//            if (discount < 0 || discount > subtotal) {
//                alert("Invalid discount value!");
//                discountInput.value = "0.00";
//                this.calculateTotals();
//                return;
//            }
//        }
//        this.calculateTotals();
//    }

//    disableRow(row) {
//        const inputs = row.querySelectorAll('input, select');
//        inputs.forEach(input => {
//            if (input.name) {
//                const hiddenInput = document.createElement('input');
//                hiddenInput.type = 'hidden';
//                hiddenInput.name = input.name;
//                hiddenInput.value = input.value;
//                hiddenInput.classList.add('hidden-disabled-input');
//                hiddenInput.setAttribute('data-original-name', input.name.split('.').pop());
//                row.appendChild(hiddenInput);
//            }
//            input.disabled = true;

//            if (input.classList.contains(this.selectors.productSelectClass.substring(1)) ||
//                input.classList.contains(this.selectors.taxMasterSelectClass?.substring(1))) {
//                if (input.tomselect) {
//                    input.tomselect.disable();
//                }
//            }
//        });
//    }

//    disableRowForTemplate2(row) {
//        this.disableRow(row);
//    }

//    enableRow(row) {
//        const inputs = row.querySelectorAll('input, select');
//        inputs.forEach(input => {
//            input.disabled = false;
//            if (input.classList.contains(this.selectors.productSelectClass.substring(1)) ||
//                input.classList.contains(this.selectors.taxMasterSelectClass?.substring(1))) {
//                if (input.tomselect) {
//                    input.tomselect.enable();
//                }
//            }
//        });
//        const hiddenInputs = row.querySelectorAll('.hidden-disabled-input');
//        hiddenInputs.forEach(hiddenInput => {
//            hiddenInput.remove();
//        });
//    }

//    /*********************************************************************************************
//     *  Template_1
//     *********************************************************************************************/
//    enterEditModeTemplate1(row) {
//        this.enableRow(row);

//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('save-row');
//            editBtn.classList.add('btn-success');
//            editBtn.classList.remove('btn-white');
//        }

//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }
//    }

//    saveRowTemplate1(row) {
//        this.disableRow(row);

//        const saveBtn = row.querySelector('.save-row');
//        if (saveBtn) {
//            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
//            saveBtn.classList.remove('save-row');
//            saveBtn.classList.add('edit-row');
//            saveBtn.classList.remove('btn-success');
//            saveBtn.classList.add('btn-white');
//        }

//        const dropdownMenu = row.querySelector('.dropdown-menu');
//        if (dropdownMenu && !row.querySelector('.dropdown-item.reset-row')) {
//            const resetOptionHTML = `
//                <a class="dropdown-item reset-row" href="#">
//                    <i class="bi-arrow-clockwise dropdown-item-icon"></i> Reset
//                </a>
//            `;
//            dropdownMenu.insertAdjacentHTML('beforeend', resetOptionHTML);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate1(row) {
//        const originalData = JSON.parse(row.dataset.originalData || '{}');
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent =
//            parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent =
//            parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        delete row.dataset.originalData;
//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }

//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//     *  Template_3
//     *********************************************************************************************/
//    enterEditModeTemplate3(row) {
//        this.enableRow(row);

//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('check-row');
//            editBtn.classList.remove('btn-success');
//            editBtn.classList.add('btn-primary');
//        }

//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }
//    }

//    saveRowTemplate3(row) {
//        this.disableRow(row);

//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        }

//        if (!row.querySelector('.reset-row')) {
//            const resetBtn = document.createElement('button');
//            resetBtn.className = 'btn btn-warning btn-sm reset-row ms-1';
//            resetBtn.innerHTML = '<i class="bi-arrow-clockwise"></i>';
//            const deleteBtn = row.querySelector('.delete-row');
//            deleteBtn.insertAdjacentElement('afterend', resetBtn);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate3(row) {
//        const originalData = JSON.parse(row.dataset.originalData || '{}');
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent =
//            parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent =
//            parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }

//        const editBtn = row.querySelector('.edit-row');
//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        } else if (!editBtn) {
//            const btnContainer = row.querySelector('td:last-child');
//            const newEditBtn = document.createElement('button');
//            newEditBtn.className = 'btn btn-success btn-sm edit-row';
//            newEditBtn.innerHTML = '<i class="bi-pencil"></i>';
//            btnContainer.insertAdjacentElement('afterbegin', newEditBtn);
//        }

//        delete row.dataset.originalData;
//        this.calculateTotals();
//    }

//    /*********************************************************************************************
//     *  CALCULATE TOTALS
//     *********************************************************************************************/
//    calculateTotals() {
//        // 1) For each product row, sum up taxes => PriceAfterVAT
//        const productRows = this.productTableBody.querySelectorAll("tr:not(.tax-row)");
//        productRows.forEach((pRow) => {
//            const totalPrice = parseFloat(
//                pRow.querySelector(this.selectors.productTotalInputClass).value
//            ) || 0;

//            let sumTax = 0;
//            let nextRow = pRow.nextElementSibling;
//            while (nextRow && nextRow.classList.contains("tax-row")) {
//                const tAmt = parseFloat(nextRow.querySelector(this.selectors.taxAmountInputClass).value) || 0;
//                sumTax += tAmt;
//                nextRow = nextRow.nextElementSibling;
//            }

//            const afterVat = totalPrice + sumTax;
//            pRow.querySelector(this.selectors.productAfterVatClass).firstChild.textContent =
//                afterVat.toFixed(2);
//            pRow.querySelector(this.selectors.productAfterVatInputClass).value =
//                afterVat.toFixed(2);
//        });

//        // 2) Subtotal / discount / final total
//        let grandTotal = 0;
//        let totalQuantity = 0;
//        let totalUnitPrice = 0;

//        productRows.forEach((row) => {
//            const qty = parseFloat(row.querySelector(this.selectors.quantityInputClass).value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const totalPrice = qty * unitPrice;

//            grandTotal += totalPrice;
//            totalQuantity += qty;
//            totalUnitPrice += unitPrice;
//        });

//        const subtotal = grandTotal;
//        if (this.subtotalId) {
//            const subtotalEl = document.getElementById(this.subtotalId);
//            if (subtotalEl) {
//                subtotalEl.value = subtotal.toFixed(2);
//            }
//            document.querySelectorAll(this.selectors.subtotalHiddenClass).forEach((el) => (el.value = subtotal.toFixed(2)));
//            document.querySelectorAll(this.selectors.subtotalVisibleClass).forEach((el) => (el.value = subtotal.toFixed(2)));
//        }

//        let discount = 0;
//        if (this.isDiscountAllowed && this.discountId) {
//            discount = parseFloat(document.getElementById(this.discountId).value) || 0;
//        }

//        const total = subtotal - discount;
//        if (this.totalId) {
//            const totalEl = document.getElementById(this.totalId);
//            if (totalEl) {
//                totalEl.value = total.toFixed(2);
//            }
//        }

//        document.querySelector(this.selectors.grandTotalId).textContent = grandTotal.toFixed(2);
//        document.querySelector(this.selectors.totalQuantityId).textContent = totalQuantity;
//        document.querySelector(this.selectors.totalPerPiecePriceId).textContent =
//            totalQuantity > 0 ? (grandTotal / totalQuantity).toFixed(2) : "0.00";
//        document.querySelector(this.selectors.totalUnitPriceId).textContent = totalUnitPrice.toFixed(2);

//        document.querySelectorAll(this.selectors.totalHiddenClass).forEach((el) => (el.value = total.toFixed(2)));
//        document.querySelectorAll(this.selectors.totalVisibleClass).forEach((el) => (el.value = total.toFixed(2)));
//    }
//}








//================================ Version : 1 ===========================================
//export class AdvancedDynamicTable {
//    constructor(tableBodyId, addButtonId, options, nameAttributeOptionObject) {
//        // --- Selectors and Elements ---
//        this.selectors = {
//            productTableBody: document.getElementById(tableBodyId),
//            addRowBtn: document.getElementById(addButtonId),
//            subtotalId: options.subtotalId || null,
//            totalId: options.totalId || null,
//            discountId: options.discountId || null,

//            productSelectClass: ".product-select",
//            currentStockClass: ".current-stock",
//            unitPriceClass: ".unit-price",
//            unitPriceInputClass: ".unit-price-input",
//            quantityInputClass: ".product-qty",
//            productTotalClass: ".product-total",
//            productTotalInputClass: ".product-total-input",

//            grandTotalId: "#grandTotal",
//            totalQuantityId: "#totalQuantity",
//            totalPerPiecePriceId: "#totalPerPiecePrice",
//            totalUnitPriceId: "#totalUnitPrice",

//            subtotalHiddenClass: ".subtotal-hidden",
//            subtotalVisibleClass: ".subtotal-visible",
//            totalHiddenClass: ".total-hidden",
//            totalVisibleClass: ".total-visible",
//        };

//        // --- Dynamic Name Attributes ---
//        this.nameAttributeOptionObject = nameAttributeOptionObject;

//        // --- Possible Field Names ---
//        this.unitPriceFields = options.unitPriceFields || ['unitprice', 'productprice', 'price'];
//        this.stockFields = options.stockFields || ['stock', 'currentstock', 'quantityinstock'];
//        this.quantityFields = options.quantityFields || ['quantity', 'qty'];
//        this.totalFields = options.totalFields || ['total', 'totalprice'];

//        // --- Data and Mode ---
//        this.productTableBody = this.selectors.productTableBody;
//        this.addRowBtn = this.selectors.addRowBtn;

//        this.productsData = options.productsData || [];
//        this.prefilledRows = options.prefilledRows || [];
//        this.isEditMode = options.isEditMode || false;

//        this.subtotalId = this.selectors.subtotalId;
//        this.totalId = this.selectors.totalId;
//        this.discountId = this.selectors.discountId;
//        this.isDiscountAllowed = options.isDiscountAllowed || false;

//        this.selectedProductIds = new Set();

//        // Mode handling
//        this.mode = options.mode || 'create'; // Modes: create, edit, delete, detail

//        // Edit Template Type
//        this.editTemplateType = options.editTemplateType || 'Default'; // Templates: Template_1, Template_2, Template_3, Default

//        // Bind methods to ensure correct 'this' context
//        this.handleAddRow = this.handleAddRow.bind(this);
//        this.handleTableInput = this.handleTableInput.bind(this);
//        this.handleTableClick = this.handleTableClick.bind(this);
//        this.handleDiscountInput = this.handleDiscountInput.bind(this);

//        // Initialize the table
//        this.initialize();
//    }

//    initialize() {
//        this.renderInitialRows();
//        this.addEventListeners();
//        this.calculateTotals();

//        // Adjust UI based on mode
//        if (this.mode === 'delete' || this.mode === 'detail') {
//            if (this.addRowBtn) {
//                this.addRowBtn.style.display = 'none';
//            }
//        }
//    }

//    // --- Update the table with new data ---
//    updateData(newOptions, newNameAttributeOptionObject) {
//        // Remove existing event listeners to prevent duplication
//        this.removeEventListeners();

//        // Update options and name attributes
//        this.productsData = newOptions.productsData || this.productsData;
//        this.prefilledRows = newOptions.prefilledRows || [];
//        this.isEditMode = newOptions.isEditMode || this.isEditMode;

//        this.subtotalId = newOptions.subtotalId || this.subtotalId;
//        this.totalId = newOptions.totalId || this.totalId;
//        this.discountId = newOptions.discountId || this.discountId;
//        this.isDiscountAllowed = newOptions.isDiscountAllowed !== undefined ? newOptions.isDiscountAllowed : this.isDiscountAllowed;

//        this.mode = newOptions.mode || this.mode;
//        this.editTemplateType = newOptions.editTemplateType || this.editTemplateType;

//        // Update name attributes if provided
//        if (newNameAttributeOptionObject) {
//            this.nameAttributeOptionObject = newNameAttributeOptionObject;
//        }

//        // Reset the table with new prefilled data
//        this.productTableBody.innerHTML = "";
//        this.selectedProductIds.clear();

//        // Re-initialize the table
//        this.initialize();
//    }

//    // --- Remove event listeners ---
//    removeEventListeners() {
//        // Remove input event listener
//        this.productTableBody.removeEventListener("input", this.handleTableInput);

//        // Remove click event listener
//        this.productTableBody.removeEventListener("click", this.handleTableClick);

//        // Remove Add Row button event listener
//        if (this.addRowBtn) {
//            this.addRowBtn.removeEventListener("click", this.handleAddRow);
//        }

//        // Remove discount input event listener
//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.removeEventListener("input", this.handleDiscountInput);
//            }
//        }
//    }

//    // --- Find Field Name in Data ---
//    findFieldName(dataObj, possibleFields) {
//        for (let field of possibleFields) {
//            if (field.toLowerCase() in dataObj) {
//                return field.toLowerCase();
//            }
//        }
//        return null;
//    }

//    // --- Normalize keys for dynamic property access ---
//    normalizeKeys(obj) {
//        const normalized = {};
//        for (const key in obj) {
//            normalized[key.toLowerCase()] = obj[key];
//        }
//        return normalized;
//    }

//    renderInitialRows() {
//        this.productTableBody.innerHTML = ""; // Clear existing rows

//        if (this.prefilledRows.length > 0) {
//            this.prefilledRows.forEach((row) => {
//                this.renderRow(row);
//                const normalizedRow = this.normalizeKeys(row);
//                this.selectedProductIds.add(normalizedRow['productid']);
//            });
//        } else if (this.mode === 'create') {
//            this.renderRow(); // Render a blank row for Create Mode
//        }

//        // Update indices after rendering all rows
//        this.updateRowIndices();
//    }

//    renderRow(data = null) {
//        debugger
//        const normalizedData = data ? this.normalizeKeys(data) : {};
//        let pQuantity = normalizedData['quantity'] ?? 0;
//        console.log(pQuantity)
//        const productOptions = this.productsData
//            .map(
//                (product) =>
//                    `<option value="${product.value}" ${normalizedData['productid'] == product.value
//                        ? "selected"
//                        : ""
//                    }>${product.text}</option>`
//            )
//            .join("");

//        const rowHTML = `
//        <tr>
//            <td>
//                <div class="tom-select-custom">
//                    <select class="js-select form-select product-select">
//                        <option value="">--- Select Product ---</option>
//                        ${productOptions}
//                    </select>
//                </div>
//            </td>
//            <td class="current-stock">${normalizedData['stock'] ?? "--"}</td>
//            <td class="unit-price">
//                ${(normalizedData['unitprice'] ?? 0).toFixed(2)}
//                <input type="hidden" class="unit-price-input" value="${(normalizedData['unitprice'] ?? 0).toFixed(2)}">
//            </td>
//            <td>
//                <input type="number" class="form-control product-qty" min="0" value="${pQuantity}">
//            </td>
//            <td class="product-total">
//                ${(normalizedData['total'] ?? 0).toFixed(2)}
//                <input type="hidden" class="product-total-input" value="${(normalizedData['total'] ?? 0).toFixed(2)}">
//            </td>
//            <td>
//                ${this.getActionButtonHTML(data)}
//            </td>
//        </tr>
//        `;
//        debugger
//        this.productTableBody.insertAdjacentHTML("beforeend", rowHTML);

//        const row = this.productTableBody.lastElementChild;

//        // Update row indices immediately to set name attributes before disabling
//        this.updateRowIndices();

//        debugger;
//        const productSelect = row.querySelector(this.selectors.productSelectClass);
//        new TomSelect(productSelect, {
//            placeholder: "--- Select Product ---",
//            maxItems: 1,
//            onChange: (value) => this.handleProductSelection(row, value, data),
//        });

//        if (data) {
//            productSelect.tomselect.setValue(normalizedData['productid']); // Pre-select value
//        } else {
//            // Disable quantity input initially
//            const quantityInput = row.querySelector(this.selectors.quantityInputClass);
//            quantityInput.disabled = true;
//        }

//        // Adjust row based on mode and data
//        if (data) {
//            // Existing row
//            if (this.mode === 'edit') {
//                this.adjustRowForEditTemplate(row);
//            } else if (this.mode === 'delete' || this.mode === 'detail') {
//                this.disableRow(row);
//            }
//        } else {
//            // New row
//            if (this.mode === 'delete' || this.mode === 'detail') {
//                this.disableRow(row);
//            }
//        }
//    }

//    // --- Get action button HTML based on mode and template ---
//    getActionButtonHTML(data) {
//        if (data) {
//            if (this.mode === 'edit') {
//                if (this.editTemplateType === 'Template_1') {
//                    // Use menu-based action buttons for Template_1
//                    return `
//                    <div class="btn-group" role="group">
//                        <button type="button" class="btn btn-white btn-sm edit-row">
//                            <i class="bi-pencil-fill me-1"></i> Edit
//                        </button>
//                        <!-- Button Group -->
//                        <div class="btn-group">
//                            <button type="button" class="btn btn-white btn-icon btn-sm dropdown-toggle dropdown-toggle-empty" data-bs-toggle="dropdown" aria-expanded="false"></button>
//                            <div class="dropdown-menu dropdown-menu-end mt-1">
//                                <a class="dropdown-item delete-row" href="#">
//                                    <i class="bi-trash dropdown-item-icon"></i> Delete
//                                </a>
//                                <!-- Reset option will appear after editing -->
//                                <!-- Additional menu items can be added here -->
//                            </div>
//                        </div>
//                        <!-- End Button Group -->
//                    </div>
//                    `;
//                } else if (this.editTemplateType === 'Template_2') {
//                    return `
//                        <button class="btn btn-primary btn-sm edit-row"><i class="bi-pencil"></i></button>
//                        <button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>
//                    `;
//                } else if (this.editTemplateType === 'Template_3') {
//                    return `
//                        <button class="btn btn-success btn-sm edit-row" type="button"><i class="bi-pencil"></i></button>
//                        <button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>
//                    `;
//                } else {
//                    // Default
//                    return '<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
//                }
//            } else if (this.mode === 'delete' || this.mode === 'detail') {
//                return ''; // No button
//            }
//        }
//        // Default is 'Delete' button
//        return '<button class="btn btn-danger btn-sm delete-row"><i class="bi-trash"></i></button>';
//    }

//    // --- Adjust row based on edit template ---
//    adjustRowForEditTemplate(row) {
//        if (this.editTemplateType === 'Template_1') {
//            this.disableRow(row);
//        } else if (this.editTemplateType === 'Template_2') {
//            this.disableRowForTemplate2(row);
//        } else if (this.editTemplateType === 'Template_3') {
//            this.disableRow(row);
//        } else {
//            // Default behavior: row is enabled
//            // No action needed
//        }
//    }

//    // --- Handle product selection ---
//    handleProductSelection(row, productId, existingData = null) {
//        const previousProductId = row.dataset.previousProductId;

//        if (previousProductId) {
//            this.selectedProductIds.delete(parseInt(previousProductId));
//        }

//        const quantityInput = row.querySelector(this.selectors.quantityInputClass);

//        if (!productId || productId === "0") {
//            // Disable quantity input when no product is selected
//            quantityInput.disabled = true;
//            quantityInput.value = "";
//            this.updateRowValues(row, null, existingData); // Clear row values
//            row.dataset.previousProductId = null;
//            this.calculateTotals();
//            return;
//        }

//        if (this.selectedProductIds.has(parseInt(productId))) {
//            alert("This product is already selected. Please choose another.");
//            row.querySelector(this.selectors.productSelectClass).tomselect.clear();
//            return;
//        }

//        this.selectedProductIds.add(parseInt(productId));
//        row.dataset.previousProductId = productId;

//        // Enable quantity input when a product is selected
//        quantityInput.disabled = false;

//        const product = this.productsData.find((p) => p.value == productId) || {};

//        // Normalize product data
//        const productNormalized = this.normalizeKeys(product);

//        this.updateRowValues(row, productNormalized, existingData);
//        this.calculateTotals();
//    }

//    // --- Update row values ---
//    updateRowValues(row, data, existingData = null) {
//        const normalizedData = data ? data : {};
//        const existingNormalizedData = existingData ? this.normalizeKeys(existingData) : {};
//        const nameAttributes = this.nameAttributeOptionObject;

//        const unitPriceCell = row.querySelector(this.selectors.unitPriceClass);
//        const unitPriceInput = unitPriceCell.querySelector(this.selectors.unitPriceInputClass);
//        const productTotalCell = row.querySelector(this.selectors.productTotalClass);
//        const productTotalInput = productTotalCell.querySelector(this.selectors.productTotalInputClass);
//        const quantityInput = row.querySelector(this.selectors.quantityInputClass); // Get quantity input

//        if (data) {
//            const unitPriceField = this.findFieldName(normalizedData, this.unitPriceFields);
//            const stockField = this.findFieldName(normalizedData, this.stockFields);

//            const unitPrice = parseFloat(normalizedData[unitPriceField]) || 0;
//            const stock = normalizedData[stockField] !== undefined ? normalizedData[stockField] : "--";

//            unitPriceCell.firstChild.textContent = unitPrice.toFixed(2);
//            unitPriceInput.value = unitPrice.toFixed(2);

//            // Set quantity and total based on existing data or reset if none
//            const quantityField = nameAttributes.quantity.toLowerCase();
//            const totalField = nameAttributes.total.toLowerCase();

//            let quantity = parseFloat(existingNormalizedData[quantityField]) || 0;

//            if (quantity === 0) {
//                // New selection without existing data
//                quantityInput.value = 0;
//                productTotalCell.firstChild.textContent = "0.00";
//                productTotalInput.value = "0.00";
//            } else {
//                // Existing data present
//                quantityInput.value = quantity;
//                const calculatedTotal = unitPrice * quantity;
//                productTotalCell.firstChild.textContent = calculatedTotal.toFixed(2);
//                productTotalInput.value = calculatedTotal.toFixed(2);
//            }

//            row.querySelector(this.selectors.currentStockClass).textContent = stock;
//        } else {
//            // Clear values
//            unitPriceCell.firstChild.textContent = "0.00";
//            unitPriceInput.value = "0.00";

//            productTotalCell.firstChild.textContent = "0.00";
//            productTotalInput.value = "0.00";

//            quantityInput.value = 0; // Reset the quantity value

//            row.querySelector(this.selectors.currentStockClass).textContent = "--";
//        }
//    }

//    updateRowIndices() {
//        const nameAttributes = this.nameAttributeOptionObject; // Get the mapping object

//        this.productTableBody.querySelectorAll("tr").forEach((row, index) => {
//            row.setAttribute("data-index", index);

//            row.querySelector(this.selectors.productSelectClass)
//                .setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.productId}`);

//            row.querySelector(this.selectors.quantityInputClass)
//                .setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.quantity}`);

//            const unitPriceInput = row.querySelector(this.selectors.unitPriceInputClass);
//            if (unitPriceInput) {
//                unitPriceInput.setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.unitPrice}`);
//            }

//            const productTotalInput = row.querySelector(this.selectors.productTotalInputClass);
//            if (productTotalInput) {
//                productTotalInput.setAttribute("name", `${nameAttributes.base}[${index}].${nameAttributes.total}`);
//            }

//            // Update names for hidden inputs if any
//            const hiddenInputs = row.querySelectorAll('.hidden-disabled-input');
//            hiddenInputs.forEach(hiddenInput => {
//                const originalName = hiddenInput.getAttribute('data-original-name');
//                if (originalName) {
//                    hiddenInput.name = `${nameAttributes.base}[${index}].${originalName}`;
//                }
//            });
//        });
//    }

//    addEventListeners() {
//        // Handle input events within the table
//        this.productTableBody.addEventListener("input", this.handleTableInput);

//        // Handle click events within the table
//        this.productTableBody.addEventListener("click", this.handleTableClick);

//        // Handle discount input
//        if (this.isDiscountAllowed && this.discountId) {
//            const discountInput = document.getElementById(this.discountId);
//            if (discountInput) {
//                discountInput.addEventListener("input", this.handleDiscountInput);
//            }
//        }

//        // Handle Add Row button
//        if (this.addRowBtn) {
//            this.addRowBtn.addEventListener("click", this.handleAddRow);
//        }
//    }

//    // --- Handle Add Row Button Click ---
//    handleAddRow(e) {
//        e.preventDefault();
//        if (this.mode === 'delete' || this.mode === 'detail') {
//            return;
//        }
//        this.renderRow();
//        this.updateRowIndices();
//    }

//    // --- Handle Table Input Events ---
//    handleTableInput(e) {
//        if (e.target.classList.contains(this.selectors.quantityInputClass.substring(1))) {
//            const row = e.target.closest("tr");
//            const qty = parseFloat(e.target.value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const stockText = row.querySelector(this.selectors.currentStockClass).textContent;
//            const stock = parseFloat(stockText) || 0;

//            if (stockText !== "--" && qty > stock) {
//                alert("Quantity cannot exceed the available stock!");
//                e.target.value = 0;
//                this.updateRowValues(row, null, null);
//                this.calculateTotals();
//                return;
//            }

//            const totalPrice = qty * unitPrice;

//            const productTotalCell = row.querySelector(this.selectors.productTotalClass);
//            productTotalCell.firstChild.textContent = totalPrice.toFixed(2);

//            const productTotalInput = productTotalCell.querySelector(this.selectors.productTotalInputClass);
//            productTotalInput.value = totalPrice.toFixed(2);

//            this.calculateTotals();
//        }
//    }

//    // --- Handle Table Click Events ---
//    handleTableClick(e) {
//        const targetBtn = e.target.closest('button, a');
//        if (!targetBtn) return;

//        if (targetBtn.classList.contains('delete-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            const productId = row.querySelector(this.selectors.productSelectClass).value;
//            this.selectedProductIds.delete(parseInt(productId));
//            row.remove();
//            this.updateRowIndices();
//            this.calculateTotals();
//        } else if (targetBtn.classList.contains('edit-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.enterEditModeTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.enterEditModeTemplate3(row);
//            } else if (this.editTemplateType === 'Template_2') {
//                this.enableRow(row);
//            } else {
//                this.enableRow(row);
//            }
//        } else if (targetBtn.classList.contains('save-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.saveRowTemplate1(row);
//            }
//        } else if (targetBtn.classList.contains('check-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_3') {
//                this.saveRowTemplate3(row);
//            }
//        } else if (targetBtn.classList.contains('reset-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            if (this.editTemplateType === 'Template_1') {
//                this.resetRowTemplate1(row);
//            } else if (this.editTemplateType === 'Template_3') {
//                this.resetRowTemplate3(row);
//            }
//        } else if (targetBtn.classList.contains('disable-row')) {
//            e.preventDefault();
//            const row = targetBtn.closest("tr");
//            this.disableRow(row);
//        }
//    }

//    // --- Handle Discount Input ---
//    handleDiscountInput(e) {
//        const discountInput = e.target;
//        const subtotalInput = document.getElementById(this.subtotalId);
//        if (subtotalInput) {
//            const subtotal = parseFloat(subtotalInput.value) || 0;
//            const discount = parseFloat(discountInput.value) || 0;

//            if (discount < 0 || discount > subtotal) {
//                alert("Invalid discount value!");
//                discountInput.value = "0.00";
//                this.calculateTotals();
//                return;
//            }
//        }
//        this.calculateTotals();
//    }

//    disableRow(row) {
//        const inputs = row.querySelectorAll('input, select');

//        inputs.forEach(input => {
//            if (input.name) {
//                // Create a hidden input with the same name and value
//                const hiddenInput = document.createElement('input');
//                hiddenInput.type = 'hidden';
//                hiddenInput.name = input.name;
//                hiddenInput.value = input.value;
//                hiddenInput.classList.add('hidden-disabled-input');
//                // Store the original name for index updates
//                hiddenInput.setAttribute('data-original-name', input.name.split('.').pop());
//                // Append the hidden input to the row
//                row.appendChild(hiddenInput);
//            }

//            input.disabled = true;

//            if (input.classList.contains(this.selectors.productSelectClass.substring(1))) {
//                if (input.tomselect) {
//                    input.tomselect.disable();
//                }
//            }
//        });
//    }

//    disableRowForTemplate2(row) {
//        this.disableRow(row);
//    }

//    enableRow(row) {
//        const inputs = row.querySelectorAll('input, select');

//        inputs.forEach(input => {
//            input.disabled = false;

//            if (input.classList.contains(this.selectors.productSelectClass.substring(1))) {
//                if (input.tomselect) {
//                    input.tomselect.enable();
//                }
//            }
//        });

//        // Remove hidden inputs to prevent duplication
//        const hiddenInputs = row.querySelectorAll('.hidden-disabled-input');
//        hiddenInputs.forEach(hiddenInput => {
//            hiddenInput.remove();
//        });
//    }

//    // --- Template_1 Methods ---
//    enterEditModeTemplate1(row) {
//        this.enableRow(row);

//        // Store original data if not already stored
//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        // Change Edit button to Save button
//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg me-1"></i> Save';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('save-row');
//            editBtn.classList.add('btn-success');
//            editBtn.classList.remove('btn-white');
//        }

//        // Remove existing Reset option from menu if any
//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }
//    }

//    saveRowTemplate1(row) {
//        this.disableRow(row);

//        // Change Save button back to Edit button
//        const saveBtn = row.querySelector('.save-row');
//        if (saveBtn) {
//            saveBtn.innerHTML = '<i class="bi-pencil-fill me-1"></i> Edit';
//            saveBtn.classList.remove('save-row');
//            saveBtn.classList.add('edit-row');
//            saveBtn.classList.remove('btn-success');
//            saveBtn.classList.add('btn-white');
//        }

//        // Add Reset option to the dropdown menu
//        const dropdownMenu = row.querySelector('.dropdown-menu');
//        if (dropdownMenu && !row.querySelector('.dropdown-item.reset-row')) {
//            const resetOptionHTML = `
//                <a class="dropdown-item reset-row" href="#">
//                    <i class="bi-arrow-clockwise dropdown-item-icon"></i> Reset
//                </a>
//            `;
//            dropdownMenu.insertAdjacentHTML('beforeend', resetOptionHTML);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate1(row) {
//        // Retrieve original data
//        const originalData = JSON.parse(row.dataset.originalData || '{}');

//        // Reset values
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        // Update displayed values
//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent = parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent = parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        // Remove stored original data
//        delete row.dataset.originalData;

//        // Remove Reset option from menu
//        const resetOption = row.querySelector('.dropdown-item.reset-row');
//        if (resetOption) {
//            resetOption.remove();
//        }

//        this.calculateTotals();
//    }

//    // --- Template_3 Methods ---
//    enterEditModeTemplate3(row) {
//        this.enableRow(row);

//        // Store original data if not already stored
//        if (!row.dataset.originalData) {
//            row.dataset.originalData = JSON.stringify({
//                productId: row.querySelector(this.selectors.productSelectClass).value,
//                quantity: row.querySelector(this.selectors.quantityInputClass).value,
//                unitPrice: row.querySelector(this.selectors.unitPriceInputClass).value,
//                total: row.querySelector(this.selectors.productTotalInputClass).value,
//            });
//        }

//        // Change Edit button to Check button
//        const editBtn = row.querySelector('.edit-row');
//        if (editBtn) {
//            editBtn.innerHTML = '<i class="bi-check-lg"></i>';
//            editBtn.classList.remove('edit-row');
//            editBtn.classList.add('check-row');
//            editBtn.classList.remove('btn-success');
//            editBtn.classList.add('btn-primary');
//        }

//        // Remove Reset button if it exists
//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }
//    }

//    saveRowTemplate3(row) {
//        this.disableRow(row);

//        // Change Check button back to Edit button
//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        }

//        // Show Reset button
//        if (!row.querySelector('.reset-row')) {
//            const resetBtn = document.createElement('button');
//            resetBtn.className = 'btn btn-warning btn-sm reset-row ms-1';
//            resetBtn.innerHTML = '<i class="bi-arrow-clockwise"></i>';
//            const deleteBtn = row.querySelector('.delete-row');
//            deleteBtn.insertAdjacentElement('afterend', resetBtn);
//        }

//        this.calculateTotals();
//    }

//    resetRowTemplate3(row) {
//        // Retrieve original data
//        const originalData = JSON.parse(row.dataset.originalData || '{}');

//        // Reset values
//        row.querySelector(this.selectors.productSelectClass).tomselect.setValue(originalData.productId);
//        row.querySelector(this.selectors.quantityInputClass).value = originalData.quantity;
//        row.querySelector(this.selectors.unitPriceInputClass).value = originalData.unitPrice;
//        row.querySelector(this.selectors.productTotalInputClass).value = originalData.total;

//        // Update displayed values
//        row.querySelector(this.selectors.unitPriceClass).firstChild.textContent = parseFloat(originalData.unitPrice).toFixed(2);
//        row.querySelector(this.selectors.productTotalClass).firstChild.textContent = parseFloat(originalData.total).toFixed(2);

//        this.disableRow(row);

//        // Remove Reset button
//        const resetBtn = row.querySelector('.reset-row');
//        if (resetBtn) {
//            resetBtn.remove();
//        }

//        // Ensure the Edit button is in place
//        const editBtn = row.querySelector('.edit-row');
//        const checkBtn = row.querySelector('.check-row');
//        if (checkBtn) {
//            checkBtn.innerHTML = '<i class="bi-pencil"></i>';
//            checkBtn.classList.remove('check-row');
//            checkBtn.classList.add('edit-row');
//            checkBtn.classList.remove('btn-primary');
//            checkBtn.classList.add('btn-success');
//        } else if (!editBtn) {
//            // In case the buttons are in an unexpected state
//            const btnContainer = row.querySelector('td:last-child');
//            const newEditBtn = document.createElement('button');
//            newEditBtn.className = 'btn btn-success btn-sm edit-row';
//            newEditBtn.innerHTML = '<i class="bi-pencil"></i>';
//            btnContainer.insertAdjacentElement('afterbegin', newEditBtn);
//        }

//        // Remove stored original data
//        delete row.dataset.originalData;

//        this.calculateTotals();
//    }

//    calculateTotals() {
//        let grandTotal = 0;
//        let totalQuantity = 0;
//        let totalUnitPrice = 0;

//        this.productTableBody.querySelectorAll("tr").forEach((row) => {
//            const qty = parseFloat(row.querySelector(this.selectors.quantityInputClass).value) || 0;
//            const unitPrice = parseFloat(row.querySelector(this.selectors.unitPriceClass).textContent) || 0;
//            const totalPrice = qty * unitPrice;

//            grandTotal += totalPrice;
//            totalQuantity += qty;
//            totalUnitPrice += unitPrice;
//        });

//        const subtotal = grandTotal;

//        if (this.subtotalId) {
//            document.getElementById(this.subtotalId).value = subtotal.toFixed(2);

//            // Update SubTotal fields
//            document.querySelectorAll(this.selectors.subtotalHiddenClass).forEach((el) => (el.value = subtotal.toFixed(2)));
//            document.querySelectorAll(this.selectors.subtotalVisibleClass).forEach((el) => (el.value = subtotal.toFixed(2)));
//        }

//        let discount = 0;
//        if (this.isDiscountAllowed && this.discountId) {
//            discount = parseFloat(document.getElementById(this.discountId).value) || 0;
//        }

//        const total = subtotal - discount;

//        if (this.totalId) {
//            document.getElementById(this.totalId).value = total.toFixed(2);
//        }

//        // Update Summary Fields
//        document.querySelector(this.selectors.grandTotalId).textContent = grandTotal.toFixed(2);
//        document.querySelector(this.selectors.totalQuantityId).textContent = totalQuantity;
//        document.querySelector(this.selectors.totalPerPiecePriceId).textContent =
//            totalQuantity > 0 ? (grandTotal / totalQuantity).toFixed(2) : "0.00";
//        document.querySelector(this.selectors.totalUnitPriceId).textContent = totalUnitPrice.toFixed(2);

//        // Update Total fields
//        document.querySelectorAll(this.selectors.totalHiddenClass).forEach((el) => (el.value = total.toFixed(2)));
//        document.querySelectorAll(this.selectors.totalVisibleClass).forEach((el) => (el.value = total.toFixed(2)));
//    }
//}

