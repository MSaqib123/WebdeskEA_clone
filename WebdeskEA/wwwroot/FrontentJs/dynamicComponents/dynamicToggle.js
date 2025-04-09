
// ToggleComponent class (in dynamicToggle.js)
export class ToggleComponent {
    constructor(togglerId, toggleElementIds) {
        this.toggler = document.getElementById(togglerId);
        this.toggleElements = toggleElementIds.map(id => document.getElementById(id));

        // Check if all elements exist
        if (this.elementsExist()) {
            this.init();
        } else {
            console.error('ToggleComponent: One or more elements are missing in the DOM.');
        }
    }

    elementsExist() {
        return this.toggler && this.toggleElements.every(elem => elem !== null);
    }

    init() {
        // Bind the event listener
        this.toggler.addEventListener('change', () => this.toggleElementsVisibility());

        // Set the initial state
        this.toggleElementsVisibility();
    }

    toggleElementsVisibility() {
        if (this.toggler.checked) {
            this.toggleElements.forEach(elem => {
                elem.classList.remove('hiddenField');
            });
        } else {
            this.toggleElements.forEach(elem => {
                elem.classList.add('hiddenField');
            });
        }
    }
}
