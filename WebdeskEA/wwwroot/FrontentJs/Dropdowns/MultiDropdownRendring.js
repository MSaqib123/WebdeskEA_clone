export class MultiDropdownRendering {
    /**
     * @constructor
     * Initializes a new instance of the MultiDropdownRendering class.
     * @param {string} containerSelector - The selector for the container element.
     * @param {string} buttonSelector - The selector for the "Add Field" button.
     * @param {string} fieldName - The name attribute for the dropdowns.
     * @param {Array} options - The options for the dropdown (array of {value, text} objects).
     * @param {string} actionType - The action type ('Create' or 'Edit').
     * @param {Array} selectedOptions - Pre-selected options for edit mode.
     */
    constructor(containerSelector, buttonSelector, fieldName, options, actionType = 'Create', selectedOptions = []) {
        this.container = document.querySelector(containerSelector);
        this.addButton = document.querySelector(buttonSelector);
        this.fieldName = fieldName;
        this.options = options;
        this.actionType = actionType;
        this.selectedOptions = selectedOptions;
        this.validateConstructorArguments();
        this.initialize();
    }

    /**
     * Validates constructor arguments and logs errors if invalid.
     */
    validateConstructorArguments() {
        if (!this.container) {
            throw new Error(`Container not found: ${this.container}`);
        }
        if (!this.addButton) {
            throw new Error(`Add button not found: ${this.addButton}`);
        }
        if (!this.fieldName || typeof this.fieldName !== 'string') {
            throw new Error(`Invalid field name: ${this.fieldName}`);
        }
        if (!Array.isArray(this.options)) {
            throw new Error(`Options must be an array: ${this.options}`);
        }
    }

    /**
     * Initializes the component by setting up event listeners and rendering defaults.
     */
    initialize() {
        this.setupAddFieldButton();
        this.setupContainerEvents();
        this.validateDropdowns();
        if (this.actionType === 'Edit') {
            this.renderDefaultDropdowns();
        }
    }

    /**
     * Renders the fixed dropdown and additional selected dropdowns for edit mode.
     */
    renderDefaultDropdowns() {
        let fixedDropdown = document.querySelector(`#${this.fieldName}`);
        if (fixedDropdown && this.selectedOptions.length > 0) {
            this.setDropdownValue(fixedDropdown, this.selectedOptions[0]);
        }

        for (let i = 1; i < this.selectedOptions.length; i++) {
            this.addDropdown(this.selectedOptions[i]);
        }
    }

    /**
     * Sets up the "Add Field" button event listener.
     */
    setupAddFieldButton() {
        this.addButton.addEventListener('click', () => this.addDropdown());
    }

    /**
     * Sets up Event Delegation for the container to handle dynamic dropdown interactions.
     */
    setupContainerEvents() {
        this.container.addEventListener('click', (event) => {
            let deleteButton = event.target.closest('.js-delete-field');
            if (deleteButton) {
                deleteButton.closest('.input-group-add-field').remove();
                this.validateDropdowns();
            }
        });
    }


    /**
     * Dynamically adds a new dropdown to the container.
     * @param {string|number} selectedValue - (Optional) Pre-selected value for the dropdown.
     */
    addDropdown(selectedValue = null) {
        let dropdownTemplate = `
            <div class="input-group-add-field">
                <div class="tom-select-custom">
                    <select class="form-select" name="${this.fieldName}" autocomplete="off">
                        <option value="0">--- Select ---</option>
                        ${this.options
                .map(({ value, text }) => `<option value="${value}">${text}</option>`)
                .join('')}
                    </select>
                </div>
                <a class="js-delete-field input-group-add-field-delete" href="javascript:;">
                    <i class="bi-x-lg"></i>
                </a>
            </div>
        `;

        let fragment = document.createRange().createContextualFragment(dropdownTemplate);
        let newDropdown = fragment.querySelector(`[name="${this.fieldName}"]`);

        this.container.appendChild(fragment);

        // Initialize TomSelect and set pre-selected value
        this.initializeTomSelect(newDropdown, selectedValue);
    }

    /**
     * Initializes a TomSelect instance for a given dropdown element.
     * @param {HTMLElement} dropdown - The dropdown element.
     * @param {string|number} selectedValue - (Optional) Pre-selected value for the dropdown.
     */
    initializeTomSelect(dropdown, selectedValue = null) {
        if (!dropdown.tomselect) {
            let tomSelectInstance = new TomSelect(dropdown, { placeholder: "--- Select ---" });

            if (selectedValue && this.isValidOption(dropdown, selectedValue)) {
                setTimeout(() => {
                    this.setDropdownValue(dropdown, selectedValue);
                }, 0);
            }

            tomSelectInstance.on('change', () => this.validateDropdowns());
        }
    }

    isValidOption(dropdown, value) {
        return Array.from(dropdown.options).some((option) => option.value === String(value));
    }


    /**
     * Sets a value for the given dropdown element.
     * @param {HTMLElement} dropdown - The dropdown element.
     * @param {string|number} value - The value to be set.
     */
    async setDropdownValue(dropdown, value) {
        let tomSelectInstance = dropdown.tomselect;
        if (tomSelectInstance) {
            tomSelectInstance.setValue(value);
        } else {
            dropdown.value = value;
        }
    }

    /**
     * Validates dropdowns to ensure unique selections and handles errors.
     */
    validateDropdowns() {
        let selectedValues = new Set();
        let dropdowns = this.getAllDropdownElements();

        let hasValidSelection = false;

        dropdowns.forEach((dropdown) => {
            let tomSelectInstance = dropdown.tomselect;
            let value = tomSelectInstance ? tomSelectInstance.getValue() : dropdown.value;

            if (value && value !== "0") {
                if (selectedValues.has(value)) {
                    notyf.error(`This Account is already selected`);
                    this.resetDropdown(tomSelectInstance);
                } else {
                    selectedValues.add(value);
                    hasValidSelection = true;
                }
            }
        });

        if (!hasValidSelection) {
            console.warn("At least one dropdown must have a valid selection.");
        }
    }

    /**
     * Resets a dropdown to its default value.
     * @param {TomSelect} tomSelectInstance - The TomSelect instance.
     */
    resetDropdown(tomSelectInstance) {
        tomSelectInstance.clear();
        tomSelectInstance.setValue("0");
    }

    /**
     * Retrieves all dropdown elements in the container.
     * @returns {Array} List of dropdown elements.
     */
    getAllDropdownElements() {
        return Array.from(document.querySelectorAll(`[name="${this.fieldName}"]`));
    }
}

