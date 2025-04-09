import { HttpClient } from '../Common/common.js';
export class COAbyCustomerSelectManager {
    constructor(baseURL, CustomerSelectId = null, CoatypeSelectId = null, CoaSelectId) {
        this.httpClient = new HttpClient(baseURL);
        this.CustomerSelect = document.getElementById(CustomerSelectId);
        this.CoatypeSelect = document.getElementById(CoatypeSelectId);
        this.CoaSelect = document.getElementById(CoaSelectId);
        this.initializeDropdowns();
    }

    async initializeDropdowns() {
        this.setupEventListeners();
        this.loadCOA()   
    }
    setupEventListeners() {
        this.CustomerSelect.addEventListener('change', (e) => this.onCustomerChange(e));
        this.CoatypeSelect.addEventListener('change', (e) => this.onCoaTypeChange(e));
    }

    async loadCOA() {
        try {
            const countries = await this.httpClient.get('GetAllCOA');
            this.populateDropdown(this.CoaSelect.tomselect, countries);
        } catch (error) {
            console.error('Error loading countries:', error);
        }
    }

    async onCustomerChange(e) {
        const customerId = this.CustomerSelect.value;
        const tomSelectInstance = this.CoaSelect.tomselect;
        if (customerId === "0" && e.target.value ==="0") {
            await this.loadCOA();
            return;
        }
        try {
            this.clearAndDisable(tomSelectInstance);
            const result = await this.httpClient.get('GetCoaByCustomer', { customerId });
            this.populateDropdown(tomSelectInstance, result,true);
        } catch (error) {
            console.error('Error loading states:', error);
        }
    }
    

    async onCoaTypeChange(e) {
        const coaTypeId = this.CoatypeSelect.value;
        const tomSelectInstance = this.CoaSelect.tomselect;
        if (coaTypeId === "0" && e.target.value === "0") {
            await this.loadCOA();
            return;
        }
        try {
            this.clearAndDisable(tomSelectInstance);
            const result = await this.httpClient.get('GetCoaByCoaTypeId', { coaTypeId });
            this.populateDropdown(tomSelectInstance, result);
        } catch (error) {
            console.error('Error loading chart of account:', error);
        }
    }

    populateDropdown(tomSelectInstance, items, selectallow = false) {
        tomSelectInstance.clearOptions();
        tomSelectInstance.addOption({ value: "0", text: "-- Select --" });
        items.forEach(item => {
            tomSelectInstance.addOption({ value: item.id, text: item.name });
        });
        tomSelectInstance.enable();

        if (selectallow === true) {
            if (items.length >= 1) {
                tomSelectInstance.setValue(items[0].id);
            }
        }

    }

    clearAndDisable(tomSelectInstance) {
        tomSelectInstance.clear();
        tomSelectInstance.clearOptions();
    }
}


//async setSelectedValues(selectedValues) {
//    if (selectedValues && selectedValues.countryId) {
//        // Set the selected value for the country dropdown
//        this.countrySelect.tomselect.setValue(selectedValues.countryId);
//        await this.onCountryChange();

//        if (selectedValues.stateId) {
//            // Set the selected value for the state dropdown
//            this.stateSelect.tomselect.setValue(selectedValues.stateId);
//            await this.onStateChange();

//            if (selectedValues.cityId) {
//                // Set the selected value for the city dropdown
//                this.citySelect.tomselect.setValue(selectedValues.cityId);
//            }
//        }
//    }
//}