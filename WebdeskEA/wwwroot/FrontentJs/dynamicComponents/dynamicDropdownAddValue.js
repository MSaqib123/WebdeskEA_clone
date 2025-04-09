export class TomSelectHandler {
    constructor(selectId, serviceName, dtoName, valueColumn, textColumn, createUrl='') {
        this.selectId = selectId;
        this.serviceName = serviceName;
        this.dtoName = dtoName;
        this.valueColumn = valueColumn;
        this.textColumn = textColumn;
        this.createUrl = createUrl;
        this.init();
    }

    async init() {
        let select = document.querySelector(`#${this.selectId}`);

        if (!select) {
            console.error(`Element with ID ${this.selectId} not found.`);
            return;
        }

        // Wait for Tom Select to initialize
        await this.waitForTomSelect(select);

        console.log(`Tom Select is ready for #${this.selectId}:`, select.tomselect);

        // Attach the create event
        this.attachCreateEvent(select.tomselect);
    }

    waitForTomSelect(select) {
        return new Promise(resolve => {
            let observer = new MutationObserver(() => {
                if (select.tomselect) {
                    observer.disconnect(); // Stop observing
                    resolve();
                }
            });
            observer.observe(select, { attributes: true });
        });
    }

    attachCreateEvent(tomSelectInstance) {
        tomSelectInstance.on('option_add', async (input) => {
            console.log('New value created:', input);
            tomSelectInstance.removeOption(input);

            try {
                //let response = await fetch(this.createUrl, {
                let response = await fetch('/Common/DropDown/OpenDropdwnEntry', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        value: input,
                        serviceName: this.serviceName,
                        dtoName: this.dtoName,
                        valueColumn: this.valueColumn,
                        textColumn: this.textColumn
                    }),
                });

                let data = await response.json();
                if (data.success) {
                    console.log('Value saved successfully:', data);
                    tomSelectInstance.addOption({ value: data.id, text: data.name });
                    tomSelectInstance.setValue(data.id);
                } else {
                    console.error('Error saving value:', data.message);
                }
            } catch (error) {
                console.error('Error:', error);
            }
        });
    }
}






//export class TomSelectHandler {
//    constructor(selectId, serviceName, dtoName, valueColumn, textColumn, createUrl = '') {
//        this.selectId = selectId;
//        this.serviceName = serviceName;
//        this.dtoName = dtoName;
//        this.valueColumn = valueColumn;
//        this.textColumn = textColumn;
//        this.createUrl = createUrl;
//        this.init();
//    }

//    async init() {
//        let select = document.querySelector(`#${this.selectId}`);

//        if (!select) {
//            console.error(`Element with ID ${this.selectId} not found.`);
//            return;
//        }

//        // Initialize Tom Select with a custom `create` function
//        select.tomselect = new TomSelect(select, {
//            create: (input, callback) => this.handleCreate(input, callback),
//            placeholder: "Create or select a value...",
//        });

//        console.log(`Tom Select initialized for #${this.selectId}`);
//    }

//    async handleCreate(input, callback) {
//        console.log('Custom create triggered:', input);

//        try {
//            let response = await fetch('/Common/DropDown/OpenDropdwnEntry', {
//                method: 'POST',
//                headers: {
//                    'Content-Type': 'application/json',
//                },
//                body: JSON.stringify({
//                    value: input,
//                    serviceName: this.serviceName,
//                    dtoName: this.dtoName,
//                    valueColumn: this.valueColumn,
//                    textColumn: this.textColumn,
//                }),
//            });

//            let data = await response.json();
//            if (data.success) {
//                console.log('Value saved successfully:', data);

//                // Add the new option via callback
//                callback({ value: data.id, text: data.name });

//                // Set the new value in Tom Select
//                let tomSelectInstance = select.tomselect;
//                if (tomSelectInstance) {
//                    tomSelectInstance.setValue(data.id);
//                }
//            } else {
//                console.error('Error saving value:', data.message);
//                callback(null); // Indicate failure
//            }
//        } catch (error) {
//            console.error('Error:', error);
//            callback(null); // Indicate failure
//        }
//    }
//}
