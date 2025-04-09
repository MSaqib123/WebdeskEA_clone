import { HttpClient } from '../Common/common.js';

export class LocationDropdownManager {
    /**
     * @param {string} baseURL - Base URL for HttpClient
     * @param {string} countrySelectId - ID of the country select element
     * @param {string} stateSelectId - ID of the state select element
     * @param {string} citySelectId - ID of the city select element
     * @param {string} [dropdownType='tomSelect'] - "tomSelect" to use TomSelect API, "simple" for plain HTML select
     */
    constructor(baseURL, countrySelectId, stateSelectId, citySelectId, dropdownType = 'tomSelect') {
        this.httpClient = new HttpClient(baseURL);
        this.dropdownType = dropdownType;
        this.countrySelect = document.getElementById(countrySelectId);
        this.stateSelect = document.getElementById(stateSelectId);
        this.citySelect = document.getElementById(citySelectId);

        // Load countries and then set default values
        this.initializeDropdowns();
    }

    async initializeDropdowns() {
        await this.loadCountries();
        this.setupEventListeners();
    }

    async loadCountries() {
        try {
            const countries = await this.httpClient.get('GetCountries');
            this.populateDropdown(this.countrySelect, countries);
        } catch (error) {
            console.error('Error loading countries:', error);
        }
    }

    setupEventListeners() {
        if (this.countrySelect) {
            this.countrySelect.addEventListener('change', () => this.onCountryChange());
        }
        if (this.stateSelect) {
            this.stateSelect.addEventListener('change', () => this.onStateChange());
        }
    }

    async onCountryChange() {
        const countryId = this.countrySelect.value;
        if (countryId) {
            try {
                this.clearAndDisable(this.stateSelect);
                this.clearAndDisable(this.citySelect);
                const states = await this.httpClient.get('GetStatesByCountry', { countryId });
                this.populateDropdown(this.stateSelect, states);
            } catch (error) {
                console.error('Error loading states:', error);
            }
        } else {
            this.clearAndDisable(this.stateSelect);
            this.clearAndDisable(this.citySelect);
        }
    }

    async onStateChange() {
        const stateId = this.stateSelect.value;
        if (stateId) {
            try {
                this.clearAndDisable(this.citySelect);
                const cities = await this.httpClient.get('GetCitiesByState', { stateId });
                this.populateDropdown(this.citySelect, cities);
            } catch (error) {
                console.error('Error loading cities:', error);
            }
        } else {
            this.clearAndDisable(this.citySelect);
        }
    }

    /**
     * Populates a dropdown with items.
     * If using TomSelect, uses its API; otherwise, uses standard DOM manipulation.
     * @param {HTMLElement} dropdown 
     * @param {Array} items - Array of items with properties { id, name }
     */
    populateDropdown(dropdown, items) {
        if (this.dropdownType === 'tomSelect') {
            // Ensure a TomSelect instance exists
            const tomSelectInstance = dropdown.tomselect;
            if (tomSelectInstance) {
                tomSelectInstance.clearOptions();
                items.forEach(item => {
                    tomSelectInstance.addOption({ value: item.id, text: item.name });
                });
                tomSelectInstance.enable();
            }
        } else {
            // Simple dropdown: clear existing options (except first default) and add new ones
            while (dropdown.options.length > 1) {
                dropdown.remove(1);
            }
            items.forEach(item => {
                const option = document.createElement('option');
                option.value = item.id;
                option.text = item.name;
                dropdown.add(option);
            });
            dropdown.disabled = false;
        }
    }

    /**
     * Clears and disables a dropdown.
     * If using TomSelect, uses its API; otherwise, works on the native select.
     * @param {HTMLElement} dropdown 
     */
    clearAndDisable(dropdown) {
        if (this.dropdownType === 'tomSelect') {
            const tomSelectInstance = dropdown.tomselect;
            if (tomSelectInstance) {
                tomSelectInstance.clear();
                tomSelectInstance.clearOptions();
                tomSelectInstance.disable();
            }
        } else {
            // Simple dropdown: remove all options except first default option
            while (dropdown.options.length > 1) {
                dropdown.remove(1);
            }
            dropdown.disabled = true;
        }
    }

    /**
     * Sets selected values for country, state, and city.
     * Works with TomSelect or simple dropdown based on dropdownType.
     * @param {Object} selectedValues - Object with keys: countryId, stateId, cityId.
     */
    async setSelectedValues(selectedValues) {
        if (selectedValues && selectedValues.countryId) {
            if (this.dropdownType === 'tomSelect') {
                this.countrySelect.tomselect.setValue(selectedValues.countryId);
            } else {
                this.countrySelect.value = selectedValues.countryId;
            }
            await this.onCountryChange();
            if (selectedValues.stateId) {
                if (this.dropdownType === 'tomSelect') {
                    this.stateSelect.tomselect.setValue(selectedValues.stateId);
                } else {
                    this.stateSelect.value = selectedValues.stateId;
                }
                await this.onStateChange();
                if (selectedValues.cityId) {
                    if (this.dropdownType === 'tomSelect') {
                        this.citySelect.tomselect.setValue(selectedValues.cityId);
                    } else {
                        this.citySelect.value = selectedValues.cityId;
                    }
                }
            }
        }
    }
}



//import { HttpClient } from '../Common/common.js';
//export class LocationDropdownManager {
//    constructor(baseURL, countrySelectId, stateSelectId, citySelectId) {
//        this.httpClient = new HttpClient(baseURL);
//        this.countrySelect = document.getElementById(countrySelectId);
//        this.stateSelect = document.getElementById(stateSelectId);
//        this.citySelect = document.getElementById(citySelectId);

//        // Load countries and then set default values
//        this.initializeDropdowns();
//    }

//    async initializeDropdowns() {
//        await this.loadCountries();
//        this.setupEventListeners();
//    }

//    async loadCountries() {
//        try {
//            const countries = await this.httpClient.get('GetCountries');
//            this.populateDropdown(this.countrySelect, countries);
//        } catch (error) {
//            console.error('Error loading countries:', error);
//        }
//    }

//    setupEventListeners() {
//        this.countrySelect.addEventListener('change', () => this.onCountryChange());
//        this.stateSelect.addEventListener('change', () => this.onStateChange());
//    }

//    async onCountryChange() {
//        const countryId = this.countrySelect.value;

//        if (countryId) {
//            try {
//                this.clearAndDisable(this.stateSelect);
//                this.clearAndDisable(this.citySelect);

//                const states = await this.httpClient.get('GetStatesByCountry', { countryId });
//                this.populateDropdown(this.stateSelect, states);
//            } catch (error) {
//                console.error('Error loading states:', error);
//            }
//        } else {
//            this.clearAndDisable(this.stateSelect);
//            this.clearAndDisable(this.citySelect);
//        }
//    }

//    async onStateChange() {
//        const stateId = this.stateSelect.value;
//        if (stateId) {
//            try {
//                this.clearAndDisable(this.citySelect);
//                const cities = await this.httpClient.get('GetCitiesByState', { stateId });
//                this.populateDropdown(this.citySelect, cities);
//            } catch (error) {
//                console.error('Error loading cities:', error);
//            }
//        } else {
//            this.clearAndDisable(this.citySelect);
//        }
//    }

//    populateDropdown(dropdown, items) {
//        const tomSelectInstance = dropdown.tomselect;
//        tomSelectInstance.clearOptions();
//        items.forEach(item => {
//            tomSelectInstance.addOption({ value: item.id, text: item.name });
//        });
//        tomSelectInstance.enable();
//    }

//    clearAndDisable(dropdown) {
//        const tomSelectInstance = dropdown.tomselect;
//        tomSelectInstance.clear();
//        tomSelectInstance.clearOptions();
//        tomSelectInstance.disable();
//    }

//    async setSelectedValues(selectedValues) {
//        if (selectedValues && selectedValues.countryId) {
//            // Set the selected value for the country dropdown
//            this.countrySelect.tomselect.setValue(selectedValues.countryId);
//            await this.onCountryChange();

//            if (selectedValues.stateId) {
//                // Set the selected value for the state dropdown
//                this.stateSelect.tomselect.setValue(selectedValues.stateId);
//                await this.onStateChange();

//                if (selectedValues.cityId) {
//                    // Set the selected value for the city dropdown
//                    this.citySelect.tomselect.setValue(selectedValues.cityId);
//                }
//            }
//        }
//    }

//}
