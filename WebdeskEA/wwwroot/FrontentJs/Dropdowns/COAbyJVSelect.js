import { HttpClient } from '../Common/common.js';
export class COAbyJVSelectManager {
    constructor(baseURL, CoatypeSelectId = null, CoaSelectId) {
        this.httpClient = new HttpClient(baseURL);
        this.CoatypeSelect = document.getElementById(CoatypeSelectId);
        this.CoaSelect = document.getElementById(CoaSelectId);
        this.initializeDropdowns();
    }

    async initializeDropdowns() {
        this.setupEventListeners();
        this.loadCOA()   
    }
    setupEventListeners() {
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
            console.log(result,"")
            this.populateDropdown(tomSelectInstance, result);
        } catch (error) {
            console.error('Error loading chart of account:', error);
        }
    }

    populateDropdown(tomSelectInstance, items,selectallow=false) {
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
        //tomSelectInstance.disable();
    }

}
