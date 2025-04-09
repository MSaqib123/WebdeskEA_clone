
/*============================== V-11 stableVersion ================================*/
//export class ContextMenuTable
export class ContextMenuTable {
    constructor(options) {
        this.tables = document.querySelectorAll(options.tableSelector);
        this.entity = options.entity;
        this.dataIdAttribute = options.dataIdAttribute;
        this.checkExistsUrl = options.checkExistsUrl;
        this.menuItems = options.menuItems;
        this.contextMenu = null;
        this.selectedRow = null;
        this.selectedData = null;
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.init();
    }

    init() {
        this.injectStyles();
        this.createContextMenu();
        this.attachEvents();
    }

    injectStyles() {
        const style = document.createElement('style');
        style.innerHTML = `
            .context-menu {
                display: none;
                position: fixed;
                z-index: 1050;
                width: 220px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                cursor: default;
                padding: 0;
                margin: 0;
                opacity: 0;
                transition: opacity 0.3s ease, transform 0.3s ease;
                transform: scale(0.95);
            }

            .context-menu.visible {
                display: block;
                opacity: 1;
                transform: scale(1);
                animation: fadeInUp 0.3s forwards;
            }

            .context-menu.hidden {
                opacity: 0;
                transform: scale(0.95);
                animation: fadeOutDown 0.3s forwards;
            }

            .context-menu ul {
                list-style: none;
                padding: 0;
                margin: 0;
            }

            .context-menu ul li {
                cursor: pointer;
            }

            .context-menu ul li:focus {
                outline: none; 
            }

            .highlight-row {
                background-color: #d4edda !important;
                transition: background-color 0.3s ease;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
                to {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
            }

            @keyframes fadeOutDown {
                from {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
                to {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
            }

            .loader {
                display: flex;
                align-items: center;
                justify-content: center;
                padding: 10px 0;
            }

            .loader .dot {
                width: 8px;
                height: 8px;
                margin: 0 2px;
                background-color: #007bff; 
                border-radius: 50%;
                animation: loader 1s infinite;
            }

            .loader .dot:nth-child(2) {
                animation-delay: 0.2s;
            }

            .loader .dot:nth-child(3) {
                animation-delay: 0.4s;
            }

            @keyframes loader {
                0%, 80%, 100% {
                    transform: scale(0);
                }
                40% {
                    transform: scale(1);
                }
            }
        `;
        document.head.appendChild(style);
    }

    createContextMenu() {
        this.contextMenu = document.createElement('div');
        this.contextMenu.classList.add('context-menu', 'hidden');
        this.contextMenu.setAttribute('role', 'menu');
        this.contextMenu.setAttribute('aria-hidden', 'true');

        this.contextMenu.innerHTML = `
            <ul>
                <div class="dropdown-menu dropdown-menu-end mt-1 show">
                    ${this.menuItems.map(item => `
                        <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}" ${item.isHidden ? 'style="display: none; visibility: hidden;"' : ''}>
                            ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
                        </li>
                    `).join('')}
                </div>
            </ul>
        `;
        document.body.appendChild(this.contextMenu);

        this.menuItems.forEach(item => {
            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
            if (menuItem) {
                menuItem.addEventListener('click', (e) => {
                    e.stopPropagation();
                    if (!item.isHidden) {
                        this.handleMenuItemClick(item);
                        this.hideContextMenu();
                    }
                });
                menuItem.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        if (!item.isHidden) {
                            this.handleMenuItemClick(item);
                            this.hideContextMenu();
                        }
                    }
                });
            }
        });

        const debounce = (func, wait) => {
            let timeout;
            return function (...args) {
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(this, args), wait);
            };
        };

        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
    }

    attachEvents() {
        this.tables.forEach(table => {
            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
        });

        document.addEventListener('click', () => this.hideContextMenu());

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.hideContextMenu();
            }
            if (this.contextMenu.classList.contains('visible')) {
                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
                const focusedElement = document.activeElement;
                const currentIndex = menuItems.indexOf(focusedElement);

                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = (currentIndex + 1) % menuItems.length;
                    menuItems[nextIndex].focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
                    menuItems[prevIndex].focus();
                }
            }
        });

        document.addEventListener('click', (e) => {
            const generateInvoiceLink = e.target.closest('.generate-invoice');
            const gotoInvoiceLink = e.target.closest('.goto-invoice');
            const deleteSO = e.target.closest('.deleteSO');
            const deleteSI = e.target.closest('.deleteSI');

            if (generateInvoiceLink || gotoInvoiceLink || deleteSO || deleteSI) {
                e.preventDefault();
                const row = e.target.closest('tr');
                if (!row) return;

                this.selectedRow = row;
                this.selectedData = this.getRowData(row);

                let actionName = '';
                if (generateInvoiceLink) actionName = 'create';
                if (gotoInvoiceLink) actionName = 'open';
                if (deleteSO) actionName = 'deleteSO';
                if (deleteSI) actionName = 'deleteSI';

                const menuItem = this.getMenuItemByAction(actionName);
                if (menuItem) {
                    if (menuItem.isConfirmationAllow) {
                        this.confirmAndHandleAction(menuItem);
                    } else {
                        this.directlyPerformAction(menuItem);
                    }
                }
            }
        });
    }

    async handleContextMenu(e, table) {
        e.preventDefault();
        const target = e.target.closest('tr');
        if (target && target.rowIndex > 0) {
            this.selectedRow = target;
            this.selectedData = this.getRowData(target);
            this.lastMouseX = e.clientX;
            this.lastMouseY = e.clientY;

            // Show static items first
            this.menuItems.forEach(item => {
                if (!item.isJsonFetchRequest && !item.isHidden) {
                    this.showMenuItem(item.id);
                }
            });

            if (this.checkExistsUrl) {
                await this.checkEntityExists();
            }

            this.showContextMenu(this.lastMouseX, this.lastMouseY);
        } else {
            this.hideContextMenu();
        }
    }

    async checkEntityExists() {
        const idKey = this.getIdKey();
        if (!this.selectedData || !this.selectedData[idKey]) {
            console.error(`${this.entity} ID is missing in the selected row data.`);
            notyf.error('Selected row data is incomplete.');

            this.menuItems.forEach(item => {
                if (item.isJsonFetchRequest) {
                    this.hideMenuItem(item.id);
                }
            });
            return;
        }

        try {
            const idValue = encodeURIComponent(this.selectedData[idKey]);
            const response = await fetch(`${this.checkExistsUrl}?${this.getQueryParam()}=${idValue}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const result = await response.json();
            if (result.success) {
                this.selectedData.entityExists = result.entityExists;

                this.menuItems.forEach(item => {
                    if (!item.isJsonFetchRequest) {
                        return;
                    }

                    if (this.selectedData.entityExists && item.showIfExists && !item.isHidden) {
                        this.showMenuItem(item.id);
                    } else if (!this.selectedData.entityExists && item.showIfNotExists && !item.isHidden) {
                        this.showMenuItem(item.id);
                    } else {
                        this.hideMenuItem(item.id);
                    }
                });
            } else {
                console.warn(`Failed to check ${this.entity} existence:`, result.message);
                //notyf.error(result.message || `Failed to check ${this.entity} existence.`);

                this.menuItems.forEach(item => {
                    if (item.isJsonFetchRequest) {
                        this.hideMenuItem(item.id);
                    }
                });
            }
        } catch (error) {
            console.warn(`Error checking ${this.entity} existence:`, error);
            //notyf.error(`An error occurred while checking ${this.entity} existence.`);

            this.menuItems.forEach(item => {
                if (item.isJsonFetchRequest) {
                    this.hideMenuItem(item.id);
                }
            });
        }
    }

    handleMenuItemClick(menuItem) {
        if (menuItem.isConfirmationAllow) {
            this.confirmAndHandleAction(menuItem);
        } else {
            this.directlyPerformAction(menuItem);
        }
    }

    directlyPerformAction(menuItem) {
        const idValue = this.getItemKeyValue(menuItem); // Use dynamic key resolution
        if (menuItem.isJsonFetchRequest) {
            this.performAction(menuItem, { [this.getAppropriateKeyName(menuItem)]: idValue });
        } else {
            this.navigateToStaticLink(menuItem, idValue);
        }
    }

    confirmAndHandleAction(menuItem) {
        Swal.fire({
            title: 'Are you sure?',
            text: `Do you want to ${menuItem.label.toLowerCase()}?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, do it!'
        }).then((result) => {
            if (result.isConfirmed) {
                this.directlyPerformAction(menuItem);
            }
        });
    }

    navigateToStaticLink(menuItem, idValue) {
        let url = menuItem.actionUrl;
        if (!url.endsWith('/')) {
            url += '/';
        }
        url += encodeURIComponent(idValue);
        window.location.href = url;
    }

    showLoader(row) {
        const sigeneratedCell = row.querySelector('td:nth-child(7)');
        if (!sigeneratedCell) {
            console.error('SI Generated cell not found.');
            return;
        }
        sigeneratedCell.innerHTML = `
            <div class="loader">
                <div class="dot"></div>
                <div class="dot"></div>
                <div class="dot"></div>
            </div>
        `;
    }

    hideLoader(row) {
        // Loader removed once updateSIGeneratedCell is called
    }

    async performAction(menuItem, data = {}) {
        let currentAction = menuItem.action;
        let create = 'create' + this.entity;
        let edit = 'edit' + this.entity;
        let open = 'open' + this.entity;
        let deleted = 'delete' + this.entity;

        try {
            if ([create, deleted].includes(currentAction)) {
                this.showLoader(this.selectedRow);
            }

            const fetchOptions = {
                method: menuItem.method,
                headers: {
                    'Content-Type': 'application/json'
                }
            };

            if (menuItem.method.toUpperCase() !== 'GET') {
                fetchOptions.body = JSON.stringify(data);
                const token = this.getAntiForgeryToken();
                if (token) {
                    fetchOptions.headers['RequestVerificationToken'] = token;
                }
            }
            const url = (currentAction === open || currentAction === edit)
                ? `${menuItem.actionUrl}?${this.getAppropriateKeyName(menuItem)}=${encodeURIComponent(data[this.getAppropriateKeyName(menuItem)])}`
                : menuItem.actionUrl;

            const response = await fetch(url, fetchOptions);
            const result = await response.json();
            if (result.success) {
                switch (currentAction) {
                    case create:
                        notyf.success(result.message);
                        this.selectedRow.classList.add('bg-soft-primary');
                        this.updateEntityGeneratedCell(this.selectedRow, true);
                        this.updateGroupButton(this.selectedRow, true);
                        break;
                    case open:
                        if (result.entityUrl) {
                            window.open(result.entityUrl, '_blank');
                            notyf.success('Invoice opened successfully!');
                        } else {
                            notyf.error(`URL for ${this.entity} not found.`);
                        }
                        break;
                    case edit:
                        if (result.entityUrl) {
                            window.open(result.entityUrl, '_blank');
                            notyf.success(`Invoice is ready for editing.`);
                        } else {
                            notyf.error(`Edit URL for Invoice not found.`);
                        }
                        break;
                    case deleted:
                        notyf.success(`Invoice deleted successfully!`);
                        this.selectedRow.classList.remove('bg-soft-primary');
                        this.updateEntityGeneratedCell(this.selectedRow, false);
                        this.updateGroupButton(this.selectedRow, false);
                        break;
                    default:
                        notyf.success(`Action '${menuItem.label}' completed successfully!`);
                }
            } else {
                notyf.error(result.message || `Failed to ${menuItem.label}.`);
                if ([create, deleted].includes(currentAction)) {
                    this.hideLoader(this.selectedRow);
                }
            }
        } catch (error) {
            console.error(`Error performing action '${menuItem.action}' on ${this.entity}:`, error);
            notyf.error(`An error occurred while performing '${menuItem.label}'.`);
            if ([create, deleted].includes(currentAction)) {
                this.hideLoader(this.selectedRow);
            }
        } finally {
            if ([create, deleted].includes(currentAction)) {
                this.hideLoader(this.selectedRow);
            }
        }
    }

    updateEntityGeneratedCell(row, isEntityExist) {
        const sigeneratedCell = row.querySelector('td:nth-child(7)');
        if (!sigeneratedCell) {
            console.error('SI Generated cell not found.');
            return;
        }

        if (isEntityExist) {
            sigeneratedCell.innerHTML = `
                <span class="badge bg-soft-success text-success">
                    <span class="legend-indicator bg-success"></span> Yes
                </span>
            `;
        } else {
            sigeneratedCell.innerHTML = `
                <span class="badge bg-soft-danger text-danger">
                    <span class="legend-indicator bg-danger"></span> No
                </span>
            `;
        }
    }

    updateGroupButton(row, isEntityExist) {
        const actionsCell = row.querySelector('td:nth-child(9)');
        if (!actionsCell) {
            console.error('Actions cell not found.');
            return;
        }

        const dropdownMenu = actionsCell.querySelector('.dropdown-menu');
        if (!dropdownMenu) {
            console.error('Dropdown menu not found in actions cell.');
            return;
        }

        const divider = dropdownMenu.querySelector('.dropdown-divider');
        if (!divider) {
            console.error('Dropdown divider not found. Cannot update group button.');
            return;
        }

        let followingLinks = divider.nextElementSibling;
        while (followingLinks) {
            const next = followingLinks.nextElementSibling;
            dropdownMenu.removeChild(followingLinks);
            followingLinks = next;
        }

        const idValue = this.selectedData[this.getIdKey()];
        if (!isEntityExist) {
            const generateLinkHtml = `
                <a href="#" class="dropdown-item generate-invoice" data-so-id="${idValue}" title="Generate Invoice">
                    <i class="bi-plus-circle dropdown-item-icon"></i> Generate Invoice
                </a>
            `;
            dropdownMenu.insertAdjacentHTML('beforeend', generateLinkHtml);
        } else {
            const gotoLinkHtml = `
                <a href="#" class="dropdown-item goto-invoice" data-so-id="${idValue}" title="Goto Invoice">
                    <i class="bi-box-arrow-up-left dropdown-item-icon"></i> Goto Invoice
                </a>
            `;
            dropdownMenu.insertAdjacentHTML('beforeend', gotoLinkHtml);
        }
    }

    getIdKey() {
        return this.dataIdAttribute.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
    }

    getQueryParam() {
        return this.getIdKey();
    }

    showMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'flex';
            menuItem.style.visibility = 'visible';
        }
    }

    hideMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'none';
            menuItem.style.visibility = 'hidden';
        }
    }

    hideAllMenuItems() {
        this.menuItems.forEach(item => {
            this.hideMenuItem(item.id);
        });
    }

    showContextMenu(x, y) {
        this.contextMenu.setAttribute('aria-hidden', 'false');
        this.contextMenu.style.display = 'block';
        this.contextMenu.style.opacity = '0';
        this.contextMenu.style.transform = 'scale(0.95)';

        const menuWidth = this.contextMenu.offsetWidth;
        const menuHeight = this.contextMenu.offsetHeight;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let finalX = x;
        let finalY = y;

        if (x + menuWidth > viewportWidth) {
            finalX = viewportWidth - menuWidth - 10;
        }

        if (y + menuHeight > viewportHeight) {
            finalY = viewportHeight - menuHeight - 10;
        }

        this.contextMenu.style.top = `${finalY}px`;
        this.contextMenu.style.left = `${finalX}px`;
        this.contextMenu.classList.remove('hidden');
        this.contextMenu.classList.add('visible');

        this.contextMenu.style.display = '';

        const menuItems = this.contextMenu.querySelectorAll('li');
        menuItems.forEach(item => {
            item.setAttribute('tabindex', '-1');
        });
    }

    hideContextMenu() {
        if (this.contextMenu) {
            this.contextMenu.setAttribute('aria-hidden', 'true');
            this.contextMenu.classList.remove('visible');
            this.contextMenu.classList.add('hidden');

            const focusedElement = document.activeElement;
            if (this.contextMenu.contains(focusedElement)) {
                focusedElement.blur();
            }
        }
    }

    getRowData(row) {
        const data = {};
        Array.from(row.attributes).forEach(attr => {
            if (attr.name.startsWith('data-')) {
                const key = attr.name.replace('data-', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
                data[key] = attr.value;
            }
        });
        console.log(`Row Data for ${this.entity}:`, data);
        return data;
    }

    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }

    getMenuItemByAction(action) {
        return this.menuItems.find(item => item.action === action);
    }

    /**
     * New method: getItemKeyValue(menuItem)
     * Determines which ID key to use for this particular menuItem, based on its keyName and isContainKey.
     */
    getItemKeyValue(menuItem) {
        let keyNameToUse = this.getAppropriateKeyName(menuItem);
        return this.selectedData[keyNameToUse];
    }

    /**
     * getAppropriateKeyName(menuItem)
     * Returns the key to be used to retrieve the ID for this menu item.
     * If menuItem.isContainKey and menuItem.keyName are defined, use that keyName.
     * Otherwise, fallback to the component's default getIdKey().
     */
    getAppropriateKeyName(menuItem) {
        if (menuItem.isContainKey && menuItem.keyName && this.selectedData[menuItem.keyName] !== undefined) {
            return menuItem.keyName;
        }
        return this.getIdKey();
    }
}


/*============================== V-10 stableVersion ================================*/
//export class ContextMenuTable {
//    constructor(options) {
//        this.tables = document.querySelectorAll(options.tableSelector);
//        this.entity = options.entity;
//        this.dataIdAttribute = options.dataIdAttribute;
//        this.checkExistsUrl = options.checkExistsUrl;
//        this.menuItems = options.menuItems;
//        this.contextMenu = null;
//        this.selectedRow = null;
//        this.selectedData = null;
//        this.lastMouseX = 0;
//        this.lastMouseY = 0;

//        this.init();
//    }

//    init() {
//        this.injectStyles();
//        this.createContextMenu();
//        this.attachEvents();
//    }

//    injectStyles() {
//        const style = document.createElement('style');
//        style.innerHTML = `
//                    .context-menu {
//                        display: none;
//                        position: fixed;
//                        z-index: 1050;
//                        width: 220px;
//                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
//                        cursor: default;
//                        padding: 0;
//                        margin: 0;
//                        opacity: 0;
//                        transition: opacity 0.3s ease, transform 0.3s ease;
//                        transform: scale(0.95);
//                    }

//                    .context-menu.visible {
//                        display: block;
//                        opacity: 1;
//                        transform: scale(1);
//                        animation: fadeInUp 0.3s forwards;
//                    }

//                    .context-menu.hidden {
//                        opacity: 0;
//                        transform: scale(0.95);
//                        animation: fadeOutDown 0.3s forwards;
//                    }

//                    .context-menu ul {
//                        list-style: none;
//                        padding: 0;
//                        margin: 0;
//                    }

//                    .context-menu ul li {
//                        cursor: pointer;
//                    }

//                    .context-menu ul li:focus {
//                        outline: none; 
//                    }

//                    .highlight-row {
//                        background-color: #d4edda !important;
//                        transition: background-color 0.3s ease;
//                    }

//                    @keyframes fadeInUp {
//                        from {
//                            opacity: 0;
//                            transform: translateY(10px) scale(0.95);
//                        }
//                        to {
//                            opacity: 1;
//                            transform: translateY(0) scale(1);
//                        }
//                    }

//                    @keyframes fadeOutDown {
//                        from {
//                            opacity: 1;
//                            transform: translateY(0) scale(1);
//                        }
//                        to {
//                            opacity: 0;
//                            transform: translateY(10px) scale(0.95);
//                        }
//                    }

//                    .loader {
//                        display: flex;
//                        align-items: center;
//                        justify-content: center;
//                        padding: 10px 0;
//                    }

//                    .loader .dot {
//                        width: 8px;
//                        height: 8px;
//                        margin: 0 2px;
//                        background-color: #007bff; 
//                        border-radius: 50%;
//                        animation: loader 1s infinite;
//                    }

//                    .loader .dot:nth-child(2) {
//                        animation-delay: 0.2s;
//                    }

//                    .loader .dot:nth-child(3) {
//                        animation-delay: 0.4s;
//                    }

//                    @keyframes loader {
//                        0%, 80%, 100% {
//                            transform: scale(0);
//                        }
//                        40% {
//                            transform: scale(1);
//                        }
//                    }
//                `;
//        document.head.appendChild(style);
//    }

//    createContextMenu() {
//        this.contextMenu = document.createElement('div');
//        this.contextMenu.classList.add('context-menu', 'hidden');
//        this.contextMenu.setAttribute('role', 'menu');
//        this.contextMenu.setAttribute('aria-hidden', 'true');

//        this.contextMenu.innerHTML = `
//                    <ul>
//                        <div class="dropdown-menu dropdown-menu-end mt-1 show">
//                            ${this.menuItems.map(item => `
//                                <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}" ${item.isHidden ? 'style="display: none; visibility: hidden;"' : ''}>
//                                    ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
//                                </li>
//                            `).join('')}
//                        </div>
//                    </ul>
//                `;
//        document.body.appendChild(this.contextMenu);

//        this.menuItems.forEach(item => {
//            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
//            if (menuItem) {
//                menuItem.addEventListener('click', (e) => {
//                    e.stopPropagation();
//                    if (!item.isHidden) {
//                        this.handleMenuItemClick(item);
//                        this.hideContextMenu();
//                    }
//                });
//                menuItem.addEventListener('keydown', (e) => {
//                    if (e.key === 'Enter') {
//                        e.preventDefault();
//                        if (!item.isHidden) {
//                            this.handleMenuItemClick(item);
//                            this.hideContextMenu();
//                        }
//                    }
//                });
//            }
//        });

//        const debounce = (func, wait) => {
//            let timeout;
//            return function (...args) {
//                clearTimeout(timeout);
//                timeout = setTimeout(() => func.apply(this, args), wait);
//            };
//        };

//        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
//        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
//    }

//    attachEvents() {
//        this.tables.forEach(table => {
//            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
//        });

//        document.addEventListener('click', () => this.hideContextMenu());

//        document.addEventListener('keydown', (e) => {
//            if (e.key === 'Escape') {
//                this.hideContextMenu();
//            }
//            if (this.contextMenu.classList.contains('visible')) {
//                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
//                const focusedElement = document.activeElement;
//                const currentIndex = menuItems.indexOf(focusedElement);

//                if (e.key === 'ArrowDown') {
//                    e.preventDefault();
//                    const nextIndex = (currentIndex + 1) % menuItems.length;
//                    menuItems[nextIndex].focus();
//                } else if (e.key === 'ArrowUp') {
//                    e.preventDefault();
//                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
//                    menuItems[prevIndex].focus();
//                }
//            }
//        });

//        document.addEventListener('click', (e) => {
//            const generateInvoiceLink = e.target.closest('.generate-invoice');
//            const gotoInvoiceLink = e.target.closest('.goto-invoice');
//            const deleteSO = e.target.closest('.deleteSO');
//            const deleteSI = e.target.closest('.deleteSI');

//            if (generateInvoiceLink || gotoInvoiceLink || deleteSO || deleteSI) {
//                e.preventDefault();
//                const row = e.target.closest('tr');
//                if (!row) return;

//                this.selectedRow = row;
//                this.selectedData = this.getRowData(row);

//                let actionName = '';
//                if (generateInvoiceLink) actionName = 'create';
//                if (gotoInvoiceLink) actionName = 'open';
//                if (deleteSO) actionName = 'deleteSO'; // matches menu item action
//                if (deleteSI) actionName = 'deleteSI'; // matches menu item action

//                const menuItem = this.getMenuItemByAction(actionName);
//                if (menuItem) {
//                    if (menuItem.isConfirmationAllow) {
//                        this.confirmAndHandleAction(menuItem);
//                    } else {
//                        this.directlyPerformAction(menuItem);
//                    }
//                }
//            }
//        });
//    }

//    async handleContextMenu(e, table) {
//        e.preventDefault();
//        const target = e.target.closest('tr');
//        if (target && target.rowIndex > 0) {
//            this.selectedRow = target;
//            this.selectedData = this.getRowData(target);
//            this.lastMouseX = e.clientX;
//            this.lastMouseY = e.clientY;

//            // ADDITION: First, ensure static items (non-fetch) are always shown
//            // Show all static items that are not hidden immediately
//            this.menuItems.forEach(item => {
//                if (!item.isJsonFetchRequest && !item.isHidden) {
//                    this.showMenuItem(item.id);
//                }
//            });

//            // Proceed to check entity if we have a checkExistsUrl
//            if (this.checkExistsUrl) {
//                await this.checkEntityExists();
//            }

//            // After check, static remain shown, dynamic shown based on conditions
//            // If check fails, static remain shown. (No hideAllMenuItems call now)

//            this.showContextMenu(this.lastMouseX, this.lastMouseY);
//        } else {
//            this.hideContextMenu();
//        }
//    }

//    async checkEntityExists() {
//        const idKey = this.getIdKey();
//        if (!this.selectedData || !this.selectedData[idKey]) {
//            console.error(`${this.entity} ID is missing in the selected row data.`);
//            notyf.error('Selected row data is incomplete.');
//            // DO NOT hideAllMenuItems here. Keep static visible.
//            // Just hide dynamic items that depend on conditions.
//            this.menuItems.forEach(item => {
//                if (item.isJsonFetchRequest) {
//                    this.hideMenuItem(item.id);
//                }
//            });
//            return;
//        }

//        try {
//            const idValue = encodeURIComponent(this.selectedData[idKey]);
//            const response = await fetch(`${this.checkExistsUrl}?${this.getQueryParam()}=${idValue}`, {
//                method: 'GET',
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            });

//            const result = await response.json();
//            if (result.success) {
//                this.selectedData.entityExists = result.entityExists;

//                this.menuItems.forEach(item => {
//                    if (!item.isJsonFetchRequest) {
//                        // Static items remain shown if not hidden. Already shown above.
//                        return;
//                    }

//                    // Dynamic items based on existence
//                    if (this.selectedData.entityExists && item.showIfExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else if (!this.selectedData.entityExists && item.showIfNotExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else {
//                        this.hideMenuItem(item.id);
//                    }
//                });
//            } else {
//                console.error(`Failed to check ${this.entity} existence:`, result.message);
//                notyf.error(result.message || `Failed to check ${this.entity} existence.`);

//                // On fail, keep static visible, hide dynamic
//                this.menuItems.forEach(item => {
//                    if (item.isJsonFetchRequest) {
//                        this.hideMenuItem(item.id);
//                    }
//                });
//            }
//        }
//        catch (error) {
//            console.error(`Error checking ${this.entity} existence:`, error);
//            //notyf.error(`An error occurred while checking ${this.entity} existence.`);

//            // On error, keep static visible, hide dynamic
//            this.menuItems.forEach(item => {
//                if (item.isJsonFetchRequest) {
//                    this.hideMenuItem(item.id);
//                }
//            });
//        }
//    }

//    handleMenuItemClick(menuItem) {
//        if (menuItem.isConfirmationAllow) {
//            this.confirmAndHandleAction(menuItem);
//        } else {
//            this.directlyPerformAction(menuItem);
//        }
//    }

//    directlyPerformAction(menuItem) {
//        const idKey = this.getIdKey();
//        const idValue = this.selectedData[idKey];
//        if (menuItem.isJsonFetchRequest) {
//            this.performAction(menuItem, { [idKey]: idValue });
//        } else {
//            this.navigateToStaticLink(menuItem, idValue);
//        }
//    }

//    confirmAndHandleAction(menuItem) {
//        Swal.fire({
//            title: 'Are you sure?',
//            text: `Do you want to ${menuItem.label.toLowerCase()}?`,
//            icon: 'warning',
//            showCancelButton: true,
//            confirmButtonColor: '#3085d6',
//            cancelButtonColor: '#d33',
//            confirmButtonText: 'Yes, do it!'
//        }).then((result) => {
//            if (result.isConfirmed) {
//                this.directlyPerformAction(menuItem);
//            }
//        });
//    }

//    navigateToStaticLink(menuItem, idValue) {
//        let url = menuItem.actionUrl;
//        if (!url.endsWith('/')) {
//            url += '/';
//        }
//        url += encodeURIComponent(idValue);
//        window.location.href = url;
//    }

//    showLoader(row) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }
//        sigeneratedCell.innerHTML = `
//                    <div class="loader">
//                        <div class="dot"></div>
//                        <div class="dot"></div>
//                        <div class="dot"></div>
//                    </div>
//                `;
//    }

//    hideLoader(row) {
//        // Loader removed once updateSIGeneratedCell is called
//    }

//    async performAction(menuItem, data = {}) {
//        let currentAction = menuItem.action;
//        let create = 'create' + this.entity;
//        let edit = 'edit' + this.entity;
//        let open = 'open' + this.entity;
//        let deleted = 'delete' + this.entity;
//        try {
//            if ([create, deleted].includes(currentAction)) {
//                this.showLoader(this.selectedRow);
//            }

//            const fetchOptions = {
//                method: menuItem.method,
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            };

//            if (menuItem.method.toUpperCase() !== 'GET') {
//                fetchOptions.body = JSON.stringify(data);
//                const token = this.getAntiForgeryToken();
//                if (token) {
//                    fetchOptions.headers['RequestVerificationToken'] = token;
//                }
//            }
//            const url = (currentAction === open || currentAction === edit)
//                ? `${menuItem.actionUrl}?${this.getIdKey()}=${encodeURIComponent(data[this.getIdKey()])}`
//                : menuItem.actionUrl;

//            const response = await fetch(url, fetchOptions);
//            const result = await response.json();
//            if (result.success) {
//                switch (currentAction) {
//                    case create:
//                        //notyf.success(`Invoice created successfully!`);
//                        notyf.success(result.message);
//                        this.selectedRow.classList.add('bg-soft-primary');
//                        this.updateEntityGeneratedCell(this.selectedRow, true);
//                        this.updateGroupButton(this.selectedRow, true);
//                        break;
//                    case open:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success('Invoice opened successfully!');
//                        } else {
//                            notyf.error(`URL for ${this.entity} not found.`);
//                        }
//                        break;
//                    case edit:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success(`Invoice is ready for editing.`);
//                        } else {
//                            notyf.error(`Edit URL for Invoice not found.`);
//                        }
//                        break;
//                    case deleted:
//                        notyf.success(`Invoice deleted successfully!`);
//                        this.selectedRow.classList.remove('bg-soft-primary');
//                        this.updateEntityGeneratedCell(this.selectedRow, false);
//                        this.updateGroupButton(this.selectedRow, false);
//                        break;
//                    default:
//                        notyf.success(`Action '${menuItem.label}' completed successfully!`);
//                }
//            } else {
//                notyf.error(result.message || `Failed to ${menuItem.label}.`);
//                if ([create, deleted].includes(currentAction)) {
//                    this.hideLoader(this.selectedRow);
//                }
//            }
//        } catch (error) {
//            console.error(`Error performing action '${menuItem.action}' on ${this.entity}:`, error);
//            notyf.error(`An error occurred while performing '${menuItem.label}'.`);
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        } finally {
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        }
//    }

//    updateEntityGeneratedCell(row, isEntityExist) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }

//        if (isEntityExist) {
//            sigeneratedCell.innerHTML = `
//                        <span class="badge bg-soft-success text-success">
//                            <span class="legend-indicator bg-success"></span> Yes
//                        </span>
//                    `;
//        } else {
//            sigeneratedCell.innerHTML = `
//                        <span class="badge bg-soft-danger text-danger">
//                            <span class="legend-indicator bg-danger"></span> No
//                        </span>
//                    `;
//        }
//    }

//    updateGroupButton(row, isEntityExist) {
//        const actionsCell = row.querySelector('td:nth-child(9)');
//        if (!actionsCell) {
//            console.error('Actions cell not found.');
//            return;
//        }

//        const dropdownMenu = actionsCell.querySelector('.dropdown-menu');
//        if (!dropdownMenu) {
//            console.error('Dropdown menu not found in actions cell.');
//            return;
//        }

//        const divider = dropdownMenu.querySelector('.dropdown-divider');
//        if (!divider) {
//            console.error('Dropdown divider not found. Cannot update group button.');
//            return;
//        }

//        // Remove all links after the divider (invoice related)
//        let followingLinks = divider.nextElementSibling;
//        while (followingLinks) {
//            const next = followingLinks.nextElementSibling;
//            dropdownMenu.removeChild(followingLinks);
//            followingLinks = next;
//        }

//        const idValue = this.selectedData[this.getIdKey()];
//        if (!isEntityExist) {
//            const generateLinkHtml = `
//                        <a href="#" class="dropdown-item generate-invoice" data-so-id="${idValue}" title="Generate Invoice">
//                            <i class="bi-plus-circle dropdown-item-icon"></i> Generate Invoice
//                        </a>
//                    `;
//            dropdownMenu.insertAdjacentHTML('beforeend', generateLinkHtml);
//        } else {
//            const gotoLinkHtml = `
//                        <a href="#" class="dropdown-item goto-invoice" data-so-id="${idValue}" title="Goto Invoice">
//                            <i class="bi-box-arrow-up-left dropdown-item-icon"></i> Goto Invoice
//                        </a>
//                    `;
//            dropdownMenu.insertAdjacentHTML('beforeend', gotoLinkHtml);
//        }
//    }

//    getIdKey() {
//        return this.dataIdAttribute.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//    }

//    getQueryParam() {
//        return this.getIdKey();
//    }

//    showMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'flex';
//            menuItem.style.visibility = 'visible';
//        }
//    }

//    hideMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'none';
//            menuItem.style.visibility = 'hidden';
//        }
//    }

//    hideAllMenuItems() {
//        this.menuItems.forEach(item => {
//            this.hideMenuItem(item.id);
//        });
//    }

//    showContextMenu(x, y) {
//        this.contextMenu.setAttribute('aria-hidden', 'false');
//        this.contextMenu.style.display = 'block';
//        this.contextMenu.style.opacity = '0';
//        this.contextMenu.style.transform = 'scale(0.95)';

//        const menuWidth = this.contextMenu.offsetWidth;
//        const menuHeight = this.contextMenu.offsetHeight;
//        const viewportWidth = window.innerWidth;
//        const viewportHeight = window.innerHeight;

//        let finalX = x;
//        let finalY = y;

//        if (x + menuWidth > viewportWidth) {
//            finalX = viewportWidth - menuWidth - 10;
//        }

//        if (y + menuHeight > viewportHeight) {
//            finalY = viewportHeight - menuHeight - 10;
//        }

//        this.contextMenu.style.top = `${finalY}px`;
//        this.contextMenu.style.left = `${finalX}px`;
//        this.contextMenu.classList.remove('hidden');
//        this.contextMenu.classList.add('visible');

//        this.contextMenu.style.display = '';

//        const menuItems = this.contextMenu.querySelectorAll('li');
//        menuItems.forEach(item => {
//            item.setAttribute('tabindex', '-1');
//        });
//    }

//    hideContextMenu() {
//        if (this.contextMenu) {
//            this.contextMenu.setAttribute('aria-hidden', 'true');
//            this.contextMenu.classList.remove('visible');
//            this.contextMenu.classList.add('hidden');

//            const focusedElement = document.activeElement;
//            if (this.contextMenu.contains(focusedElement)) {
//                focusedElement.blur();
//            }
//        }
//    }

//    getRowData(row) {
//        const data = {};
//        Array.from(row.attributes).forEach(attr => {
//            if (attr.name.startsWith('data-')) {
//                const key = attr.name.replace('data-', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//                data[key] = attr.value;
//            }
//        });
//        console.log(`Row Data for ${this.entity}:`, data);
//        return data;
//    }

//    getAntiForgeryToken() {
//        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
//        return tokenElement ? tokenElement.value : '';
//    }

//    getMenuItemByAction(action) {
//        return this.menuItems.find(item => item.action === action);
//    }
//}


/*============================== V-9 ================================*/
//export class ContextMenuTable {
//    constructor(options) {
//        this.tables = document.querySelectorAll(options.tableSelector);
//        this.entity = options.entity;
//        this.dataIdAttribute = options.dataIdAttribute;
//        this.checkExistsUrl = options.checkExistsUrl;
//        this.menuItems = options.menuItems;
//        this.contextMenu = null;
//        this.selectedRow = null;
//        this.selectedData = null;
//        this.lastMouseX = 0;
//        this.lastMouseY = 0;

//        this.init();
//    }

//    init() {
//        this.injectStyles();
//        this.createContextMenu();
//        this.attachEvents();
//    }

//    injectStyles() {
//        const style = document.createElement('style');
//        style.innerHTML = `
//                    .context-menu {
//                        display: none;
//                        position: fixed;
//                        z-index: 1050;
//                        width: 220px;
//                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
//                        cursor: default;
//                        padding: 0;
//                        margin: 0;
//                        opacity: 0;
//                        transition: opacity 0.3s ease, transform 0.3s ease;
//                        transform: scale(0.95);
//                    }

//                    .context-menu.visible {
//                        display: block;
//                        opacity: 1;
//                        transform: scale(1);
//                        animation: fadeInUp 0.3s forwards;
//                    }

//                    .context-menu.hidden {
//                        opacity: 0;
//                        transform: scale(0.95);
//                        animation: fadeOutDown 0.3s forwards;
//                    }

//                    .context-menu ul {
//                        list-style: none;
//                        padding: 0;
//                        margin: 0;
//                    }

//                    .context-menu ul li {
//                        cursor: pointer;
//                    }

//                    .context-menu ul li:focus {
//                        outline: none; 
//                    }

//                    .highlight-row {
//                        background-color: #d4edda !important;
//                        transition: background-color 0.3s ease;
//                    }

//                    @keyframes fadeInUp {
//                        from {
//                            opacity: 0;
//                            transform: translateY(10px) scale(0.95);
//                        }
//                        to {
//                            opacity: 1;
//                            transform: translateY(0) scale(1);
//                        }
//                    }

//                    @keyframes fadeOutDown {
//                        from {
//                            opacity: 1;
//                            transform: translateY(0) scale(1);
//                        }
//                        to {
//                            opacity: 0;
//                            transform: translateY(10px) scale(0.95);
//                        }
//                    }

//                    .loader {
//                        display: flex;
//                        align-items: center;
//                        justify-content: center;
//                        padding: 10px 0;
//                    }

//                    .loader .dot {
//                        width: 8px;
//                        height: 8px;
//                        margin: 0 2px;
//                        background-color: #007bff; 
//                        border-radius: 50%;
//                        animation: loader 1s infinite;
//                    }

//                    .loader .dot:nth-child(2) {
//                        animation-delay: 0.2s;
//                    }

//                    .loader .dot:nth-child(3) {
//                        animation-delay: 0.4s;
//                    }

//                    @keyframes loader {
//                        0%, 80%, 100% {
//                            transform: scale(0);
//                        }
//                        40% {
//                            transform: scale(1);
//                        }
//                    }
//                `;
//        document.head.appendChild(style);
//    }

//    createContextMenu() {
//        this.contextMenu = document.createElement('div');
//        this.contextMenu.classList.add('context-menu', 'hidden');
//        this.contextMenu.setAttribute('role', 'menu');
//        this.contextMenu.setAttribute('aria-hidden', 'true');

//        this.contextMenu.innerHTML = `
//                    <ul>
//                        <div class="dropdown-menu dropdown-menu-end mt-1 show">
//                            ${this.menuItems.map(item => `
//                                <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}" ${item.isHidden ? 'style="display: none; visibility: hidden;"' : ''}>
//                                    ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
//                                </li>
//                            `).join('')}
//                        </div>
//                    </ul>
//                `;
//        document.body.appendChild(this.contextMenu);

//        this.menuItems.forEach(item => {
//            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
//            if (menuItem) {
//                menuItem.addEventListener('click', (e) => {
//                    e.stopPropagation();
//                    if (!item.isHidden) {
//                        this.handleMenuItemClick(item);
//                        this.hideContextMenu();
//                    }
//                });
//                menuItem.addEventListener('keydown', (e) => {
//                    if (e.key === 'Enter') {
//                        e.preventDefault();
//                        if (!item.isHidden) {
//                            this.handleMenuItemClick(item);
//                            this.hideContextMenu();
//                        }
//                    }
//                });
//            }
//        });

//        const debounce = (func, wait) => {
//            let timeout;
//            return function (...args) {
//                clearTimeout(timeout);
//                timeout = setTimeout(() => func.apply(this, args), wait);
//            };
//        };

//        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
//        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
//    }

//    attachEvents() {
//        this.tables.forEach(table => {
//            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
//        });

//        document.addEventListener('click', () => this.hideContextMenu());

//        document.addEventListener('keydown', (e) => {
//            if (e.key === 'Escape') {
//                this.hideContextMenu();
//            }
//            if (this.contextMenu.classList.contains('visible')) {
//                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
//                const focusedElement = document.activeElement;
//                const currentIndex = menuItems.indexOf(focusedElement);

//                if (e.key === 'ArrowDown') {
//                    e.preventDefault();
//                    const nextIndex = (currentIndex + 1) % menuItems.length;
//                    menuItems[nextIndex].focus();
//                } else if (e.key === 'ArrowUp') {
//                    e.preventDefault();
//                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
//                    menuItems[prevIndex].focus();
//                }
//            }
//        });

//        document.addEventListener('click', (e) => {
//            const generateInvoiceLink = e.target.closest('.generate-invoice');
//            const gotoInvoiceLink = e.target.closest('.goto-invoice');
//            const deleteSO = e.target.closest('.deleteSO');
//            const deleteSI = e.target.closest('.deleteSI');

//            if (generateInvoiceLink || gotoInvoiceLink || deleteSO || deleteSI) {
//                e.preventDefault();
//                const row = e.target.closest('tr');
//                if (!row) return;

//                this.selectedRow = row;
//                this.selectedData = this.getRowData(row);

//                let actionName = '';
//                if (generateInvoiceLink) actionName = 'create';
//                if (gotoInvoiceLink) actionName = 'open';
//                if (deleteSO) actionName = 'deleteSO'; // matches menu item action
//                if (deleteSI) actionName = 'deleteSI'; // matches menu item action

//                const menuItem = this.getMenuItemByAction(actionName);
//                if (menuItem) {
//                    if (menuItem.isConfirmationAllow) {
//                        this.confirmAndHandleAction(menuItem);
//                    } else {
//                        this.directlyPerformAction(menuItem);
//                    }
//                }
//            }
//        });
//    }

//    async handleContextMenu(e, table) {
//        e.preventDefault();
//        const target = e.target.closest('tr');
//        if (target && target.rowIndex > 0) {
//            this.selectedRow = target;
//            this.selectedData = this.getRowData(target);
//            this.lastMouseX = e.clientX;
//            this.lastMouseY = e.clientY;

//            await this.checkEntityExists();
//            this.showContextMenu(this.lastMouseX, this.lastMouseY);
//        } else {
//            this.hideContextMenu();
//        }
//    }

//    async checkEntityExists() {
//        const idKey = this.getIdKey();
//        if (!this.selectedData || !this.selectedData[idKey]) {
//            console.error(`${this.entity} ID is missing in the selected row data.`);
//            notyf.error('Selected row data is incomplete.');
//            this.hideAllMenuItems();
//            return;
//        }

//        try {
//            const idValue = encodeURIComponent(this.selectedData[idKey]);
//            const response = await fetch(`${this.checkExistsUrl}?${this.getQueryParam()}=${idValue}`, {
//                method: 'GET',
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            });

//            const result = await response.json();
//            if (result.success) {
//                this.selectedData.entityExists = result.entityExists;

//                this.menuItems.forEach(item => {
//                    if (!item.isJsonFetchRequest) {
//                        if (!item.isHidden) {
//                            this.showMenuItem(item.id);
//                        }
//                        return;
//                    }
//                    if (this.selectedData.entityExists && item.showIfExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else if (!this.selectedData.entityExists && item.showIfNotExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else {
//                        this.hideMenuItem(item.id);
//                    }
//                });
//            } else {
//                console.error(`Failed to check ${this.entity} existence:`, result.message);
//                notyf.error(result.message || `Failed to check ${this.entity} existence.`);
//                this.hideAllMenuItems();
//            }
//        } catch (error) {
//            console.error(`Error checking ${this.entity} existence:`, error);
//            notyf.error(`An error occurred while checking ${this.entity} existence.`);
//            this.hideAllMenuItems();
//        }
//    }

//    handleMenuItemClick(menuItem) {
//        if (menuItem.isConfirmationAllow) {
//            this.confirmAndHandleAction(menuItem);
//        } else {
//            this.directlyPerformAction(menuItem);
//        }
//    }

//    directlyPerformAction(menuItem) {
//        const idKey = this.getIdKey();
//        const idValue = this.selectedData[idKey];
//        if (menuItem.isJsonFetchRequest) {
//            this.performAction(menuItem, { [idKey]: idValue });
//        } else {
//            this.navigateToStaticLink(menuItem, idValue);
//        }
//    }

//    // A generic confirmation prompt for any action that requires confirmation
//    confirmAndHandleAction(menuItem) {
//        Swal.fire({
//            title: 'Are you sure?',
//            text: `Do you want to ${menuItem.label.toLowerCase()}?`,
//            icon: 'warning',
//            showCancelButton: true,
//            confirmButtonColor: '#3085d6',
//            cancelButtonColor: '#d33',
//            confirmButtonText: 'Yes, do it!'
//        }).then((result) => {
//            if (result.isConfirmed) {
//                this.directlyPerformAction(menuItem);
//            }
//        });
//    }

//    navigateToStaticLink(menuItem, idValue) {
//        let url = menuItem.actionUrl;
//        if (!url.endsWith('/')) {
//            url += '/';
//        }
//        url += encodeURIComponent(idValue);
//        window.location.href = url;
//    }

//    showLoader(row) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }
//        sigeneratedCell.innerHTML = `
//                    <div class="loader">
//                        <div class="dot"></div>
//                        <div class="dot"></div>
//                        <div class="dot"></div>
//                    </div>
//                `;
//    }

//    hideLoader(row) {
//        // Loader removed once updateSIGeneratedCell is called
//    }

//    async performAction(menuItem, data = {}) {
//        let currentAction = menuItem.action + this.entity;
//        let create = 'create' + this.entity;
//        let edit = 'edit' + this.entity;
//        let open = 'open' + this.entity;
//        let deleted = 'delete' + this.entity;

//        try {
//            if ([create, deleted].includes(currentAction)) {
//                this.showLoader(this.selectedRow);
//            }

//            const fetchOptions = {
//                method: menuItem.method,
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            };

//            if (menuItem.method.toUpperCase() !== 'GET') {
//                fetchOptions.body = JSON.stringify(data);
//                const token = this.getAntiForgeryToken();
//                if (token) {
//                    fetchOptions.headers['RequestVerificationToken'] = token;
//                }
//            }
//            const url = (currentAction === open || currentAction === edit)
//                ? `${menuItem.actionUrl}?${this.getIdKey()}=${encodeURIComponent(data[this.getIdKey()])}`
//                : menuItem.actionUrl;

//            const response = await fetch(url, fetchOptions);
//            const result = await response.json();
//            if (result.success) {
//                switch (currentAction) {
//                    case create:
//                        //notyf.success(`Invoice created successfully!`);
//                        notyf.success(result.message);
//                        this.selectedRow.classList.add('bg-soft-primary');
//                        this.updateEntityGeneratedCell(this.selectedRow, true);
//                        this.updateGroupButton(this.selectedRow, true);
//                        break;
//                    case open:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success('Invoice opened successfully!');
//                        } else {
//                            notyf.error(`URL for ${this.entity} not found.`);
//                        }
//                        break;
//                    case edit:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success(`Invoice is ready for editing.`);
//                        } else {
//                            notyf.error(`Edit URL for Invoice not found.`);
//                        }
//                        break;
//                    case deleted:
//                        notyf.success(`Invoice deleted successfully!`);
//                        this.selectedRow.classList.remove('bg-soft-primary');
//                        this.updateEntityGeneratedCell(this.selectedRow, false);
//                        this.updateGroupButton(this.selectedRow, false);
//                        break;
//                    default:
//                        notyf.success(`Action '${menuItem.label}' completed successfully!`);
//                }
//            } else {
//                notyf.error(result.message || `Failed to ${menuItem.label}.`);
//                if ([create, deleted].includes(currentAction)) {
//                    this.hideLoader(this.selectedRow);
//                }
//            }
//        } catch (error) {
//            console.error(`Error performing action '${menuItem.action}' on ${this.entity}:`, error);
//            notyf.error(`An error occurred while performing '${menuItem.label}'.`);
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        } finally {
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        }
//    }

//    updateEntityGeneratedCell(row, isEntityExist) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }

//        if (isEntityExist) {
//            sigeneratedCell.innerHTML = `
//                        <span class="badge bg-soft-success text-success">
//                            <span class="legend-indicator bg-success"></span> Yes
//                        </span>
//                    `;
//        } else {
//            sigeneratedCell.innerHTML = `
//                        <span class="badge bg-soft-danger text-danger">
//                            <span class="legend-indicator bg-danger"></span> No
//                        </span>
//                    `;
//        }
//    }

//    updateGroupButton(row, isEntityExist) {
//        const actionsCell = row.querySelector('td:nth-child(9)');
//        if (!actionsCell) {
//            console.error('Actions cell not found.');
//            return;
//        }

//        const dropdownMenu = actionsCell.querySelector('.dropdown-menu');
//        if (!dropdownMenu) {
//            console.error('Dropdown menu not found in actions cell.');
//            return;
//        }

//        const divider = dropdownMenu.querySelector('.dropdown-divider');
//        if (!divider) {
//            console.error('Dropdown divider not found. Cannot update group button.');
//            return;
//        }

//        // Remove all links after the divider (invoice related)
//        let followingLinks = divider.nextElementSibling;
//        while (followingLinks) {
//            const next = followingLinks.nextElementSibling;
//            dropdownMenu.removeChild(followingLinks);
//            followingLinks = next;
//        }

//        const idValue = this.selectedData[this.getIdKey()];
//        if (!isEntityExist) {
//            const generateLinkHtml = `
//                        <a href="#" class="dropdown-item generate-invoice" data-so-id="${idValue}" title="Generate Invoice">
//                            <i class="bi-plus-circle dropdown-item-icon"></i> Generate Invoice
//                        </a>
//                    `;
//            dropdownMenu.insertAdjacentHTML('beforeend', generateLinkHtml);
//        } else {
//            const gotoLinkHtml = `
//                        <a href="#" class="dropdown-item goto-invoice" data-so-id="${idValue}" title="Goto Invoice">
//                            <i class="bi-box-arrow-up-left dropdown-item-icon"></i> Goto Invoice
//                        </a>
//                    `;
//            dropdownMenu.insertAdjacentHTML('beforeend', gotoLinkHtml);
//        }
//    }

//    getIdKey() {
//        return this.dataIdAttribute.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//    }

//    getQueryParam() {
//        return this.getIdKey();
//    }

//    showMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'flex';
//            menuItem.style.visibility = 'visible';
//        }
//    }

//    hideMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'none';
//            menuItem.style.visibility = 'hidden';
//        }
//    }

//    hideAllMenuItems() {
//        this.menuItems.forEach(item => {
//            this.hideMenuItem(item.id);
//        });
//    }

//    showContextMenu(x, y) {
//        this.contextMenu.setAttribute('aria-hidden', 'false');
//        this.contextMenu.style.display = 'block';
//        this.contextMenu.style.opacity = '0';
//        this.contextMenu.style.transform = 'scale(0.95)';

//        const menuWidth = this.contextMenu.offsetWidth;
//        const menuHeight = this.contextMenu.offsetHeight;
//        const viewportWidth = window.innerWidth;
//        const viewportHeight = window.innerHeight;

//        let finalX = x;
//        let finalY = y;

//        if (x + menuWidth > viewportWidth) {
//            finalX = viewportWidth - menuWidth - 10;
//        }

//        if (y + menuHeight > viewportHeight) {
//            finalY = viewportHeight - menuHeight - 10;
//        }

//        this.contextMenu.style.top = `${finalY}px`;
//        this.contextMenu.style.left = `${finalX}px`;
//        this.contextMenu.classList.remove('hidden');
//        this.contextMenu.classList.add('visible');

//        this.contextMenu.style.display = '';

//        const menuItems = this.contextMenu.querySelectorAll('li');
//        menuItems.forEach(item => {
//            item.setAttribute('tabindex', '-1');
//        });
//    }

//    hideContextMenu() {
//        if (this.contextMenu) {
//            this.contextMenu.setAttribute('aria-hidden', 'true');
//            this.contextMenu.classList.remove('visible');
//            this.contextMenu.classList.add('hidden');

//            const focusedElement = document.activeElement;
//            if (this.contextMenu.contains(focusedElement)) {
//                focusedElement.blur();
//            }
//        }
//    }

//    getRowData(row) {
//        const data = {};
//        Array.from(row.attributes).forEach(attr => {
//            if (attr.name.startsWith('data-')) {
//                const key = attr.name.replace('data-', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//                data[key] = attr.value;
//            }
//        });
//        console.log(`Row Data for ${this.entity}:`, data);
//        return data;
//    }

//    getAntiForgeryToken() {
//        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
//        return tokenElement ? tokenElement.value : '';
//    }

//    getMenuItemByAction(action) {
//        // If actions like deleteSO or deleteSI exist, we match them as-is.
//        return this.menuItems.find(item => item.action === action);
//    }
//}


/*============================== V-8 ================================*/
//export class ContextMenuTable {
//    constructor(options) {
//        this.tables = document.querySelectorAll(options.tableSelector);
//        this.entity = options.entity;
//        this.dataIdAttribute = options.dataIdAttribute;
//        this.checkExistsUrl = options.checkExistsUrl;
//        this.menuItems = options.menuItems;
//        this.contextMenu = null;
//        this.selectedRow = null;
//        this.selectedData = null;
//        this.lastMouseX = 0;
//        this.lastMouseY = 0;

//        this.init();
//    }

//    init() {
//        this.injectStyles();
//        this.createContextMenu();
//        this.attachEvents();
//    }

//    injectStyles() {
//        const style = document.createElement('style');
//        style.innerHTML = `
//                    .context-menu {
//                        display: none;
//                        position: fixed;
//                        z-index: 1050;
//                        width: 220px;
//                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
//                        cursor: default;
//                        padding: 0;
//                        margin: 0;
//                        opacity: 0;
//                        transition: opacity 0.3s ease, transform 0.3s ease;
//                        transform: scale(0.95);
//                    }

//                    .context-menu.visible {
//                        display: block;
//                        opacity: 1;
//                        transform: scale(1);
//                        animation: fadeInUp 0.3s forwards;
//                    }

//                    .context-menu.hidden {
//                        opacity: 0;
//                        transform: scale(0.95);
//                        animation: fadeOutDown 0.3s forwards;
//                    }

//                    .context-menu ul {
//                        list-style: none;
//                        padding: 0;
//                        margin: 0;
//                    }

//                    .context-menu ul li {
//                        cursor: pointer;
//                    }

//                    .context-menu ul li:focus {
//                        outline: none; 
//                    }

//                    .highlight-row {
//                        background-color: #d4edda !important;
//                        transition: background-color 0.3s ease;
//                    }

//                    @keyframes fadeInUp {
//                        from {
//                            opacity: 0;
//                            transform: translateY(10px) scale(0.95);
//                        }
//                        to {
//                            opacity: 1;
//                            transform: translateY(0) scale(1);
//                        }
//                    }

//                    @keyframes fadeOutDown {
//                        from {
//                            opacity: 1;
//                            transform: translateY(0) scale(1);
//                        }
//                        to {
//                            opacity: 0;
//                            transform: translateY(10px) scale(0.95);
//                        }
//                    }

//                    .loader {
//                        display: flex;
//                        align-items: center;
//                        justify-content: center;
//                        padding: 10px 0;
//                    }

//                    .loader .dot {
//                        width: 8px;
//                        height: 8px;
//                        margin: 0 2px;
//                        background-color: #007bff; 
//                        border-radius: 50%;
//                        animation: loader 1s infinite;
//                    }

//                    .loader .dot:nth-child(2) {
//                        animation-delay: 0.2s;
//                    }

//                    .loader .dot:nth-child(3) {
//                        animation-delay: 0.4s;
//                    }

//                    @keyframes loader {
//                        0%, 80%, 100% {
//                            transform: scale(0);
//                        }
//                        40% {
//                            transform: scale(1);
//                        }
//                    }
//                `;
//        document.head.appendChild(style);
//    }

//    createContextMenu() {
//        this.contextMenu = document.createElement('div');
//        this.contextMenu.classList.add('context-menu', 'hidden');
//        this.contextMenu.setAttribute('role', 'menu');
//        this.contextMenu.setAttribute('aria-hidden', 'true');

//        this.contextMenu.innerHTML = `
//                    <ul>
//                        <div class="dropdown-menu dropdown-menu-end mt-1 show">
//                            ${this.menuItems.map(item => `
//                                <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}" ${item.isHidden ? 'style="display: none; visibility: hidden;"' : ''}>
//                                    ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
//                                </li>
//                            `).join('')}
//                        </div>
//                    </ul>
//                `;
//        document.body.appendChild(this.contextMenu);

//        this.menuItems.forEach(item => {
//            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
//            if (menuItem) {
//                menuItem.addEventListener('click', (e) => {
//                    e.stopPropagation();
//                    if (!item.isHidden) {
//                        this.handleMenuItemClick(item);
//                        this.hideContextMenu();
//                    }
//                });
//                menuItem.addEventListener('keydown', (e) => {
//                    if (e.key === 'Enter') {
//                        e.preventDefault();
//                        if (!item.isHidden) {
//                            this.handleMenuItemClick(item);
//                            this.hideContextMenu();
//                        }
//                    }
//                });
//            }
//        });

//        const debounce = (func, wait) => {
//            let timeout;
//            return function (...args) {
//                clearTimeout(timeout);
//                timeout = setTimeout(() => func.apply(this, args), wait);
//            };
//        };

//        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
//        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
//    }

//    attachEvents() {
//        this.tables.forEach(table => {
//            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
//        });

//        document.addEventListener('click', () => this.hideContextMenu());

//        document.addEventListener('keydown', (e) => {
//            if (e.key === 'Escape') {
//                this.hideContextMenu();
//            }
//            if (this.contextMenu.classList.contains('visible')) {
//                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
//                const focusedElement = document.activeElement;
//                const currentIndex = menuItems.indexOf(focusedElement);

//                if (e.key === 'ArrowDown') {
//                    e.preventDefault();
//                    const nextIndex = (currentIndex + 1) % menuItems.length;
//                    menuItems[nextIndex].focus();
//                } else if (e.key === 'ArrowUp') {
//                    e.preventDefault();
//                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
//                    menuItems[prevIndex].focus();
//                }
//            }
//        });

//        // Event delegation for generate-invoice and goto-invoice
//        document.addEventListener('click', (e) => {
//            const generateInvoiceLink = e.target.closest('.generate-invoice');
//            const gotoInvoiceLink = e.target.closest('.goto-invoice');

//            if (generateInvoiceLink || gotoInvoiceLink) {
//                e.preventDefault();
//                const row = e.target.closest('tr');
//                if (!row) return;

//                this.selectedRow = row;
//                this.selectedData = this.getRowData(row);

//                if (generateInvoiceLink) {
//                    const menuItem = this.getMenuItemByAction('create');
//                    if (menuItem) {
//                        // Confirm before create
//                        this.confirmAndHandleCreate(menuItem);
//                    }
//                } else if (gotoInvoiceLink) {
//                    const menuItem = this.getMenuItemByAction('open');
//                    if (menuItem) {
//                        this.handleMenuItemClick(menuItem);
//                    }
//                }
//            }
//        });
//    }

//    async handleContextMenu(e, table) {
//        e.preventDefault();
//        const target = e.target.closest('tr');
//        if (target && target.rowIndex > 0) {
//            this.selectedRow = target;
//            this.selectedData = this.getRowData(target);
//            this.lastMouseX = e.clientX;
//            this.lastMouseY = e.clientY;

//            await this.checkEntityExists();
//            this.showContextMenu(this.lastMouseX, this.lastMouseY);
//        } else {
//            this.hideContextMenu();
//        }
//    }

//    async checkEntityExists() {
//        const idKey = this.getIdKey();
//        if (!this.selectedData || !this.selectedData[idKey]) {
//            console.error(`${this.entity} ID is missing in the selected row data.`);
//            notyf.error('Selected row data is incomplete.');
//            this.hideAllMenuItems();
//            return;
//        }

//        try {
//            const idValue = encodeURIComponent(this.selectedData[idKey]);
//            const response = await fetch(`${this.checkExistsUrl}?${this.getQueryParam()}=${idValue}`, {
//                method: 'GET',
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            });

//            const result = await response.json();
//            if (result.success) {
//                this.selectedData.entityExists = result.entityExists;

//                this.menuItems.forEach(item => {
//                    if (!item.isJsonFetchRequest) {
//                        if (!item.isHidden) {
//                            this.showMenuItem(item.id);
//                        }
//                        return;
//                    }
//                    if (this.selectedData.entityExists && item.showIfExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else if (!this.selectedData.entityExists && item.showIfNotExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else {
//                        this.hideMenuItem(item.id);
//                    }
//                });
//            } else {
//                console.error(`Failed to check ${this.entity} existence:`, result.message);
//                notyf.error(result.message || `Failed to check ${this.entity} existence.`);
//                this.hideAllMenuItems();
//            }
//        } catch (error) {
//            console.error(`Error checking ${this.entity} existence:`, error);
//            notyf.error(`An error occurred while checking ${this.entity} existence.`);
//            this.hideAllMenuItems();
//        }
//    }

//    handleMenuItemClick(menuItem) {
//        // If this is a "create" action, we confirm first
//        if (menuItem.action === 'create') {
//            this.confirmAndHandleCreate(menuItem);
//        } else {
//            const idKey = this.getIdKey();
//            const idValue = this.selectedData[idKey];
//            if (menuItem.isJsonFetchRequest) {
//                this.performAction(menuItem, { [idKey]: idValue });
//            } else {
//                this.navigateToStaticLink(menuItem, idValue);
//            }
//        }
//    }

//    // New method to show SweetAlert2 confirmation before creating an invoice
//    confirmAndHandleCreate(menuItem) {
//        Swal.fire({
//            title: 'Are you sure?',
//            text: "Do you want to generate the invoice?",
//            icon: 'warning',
//            showCancelButton: true,
//            confirmButtonColor: '#3085d6',
//            cancelButtonColor: '#d33',
//            confirmButtonText: 'Yes, generate it!'
//        }).then((result) => {
//            if (result.isConfirmed) {
//                const idKey = this.getIdKey();
//                const idValue = this.selectedData[idKey];
//                if (menuItem.isJsonFetchRequest) {
//                    this.performAction(menuItem, { [idKey]: idValue });
//                } else {
//                    this.navigateToStaticLink(menuItem, idValue);
//                }
//            }
//        });
//    }

//    navigateToStaticLink(menuItem, idValue) {
//        let url = menuItem.actionUrl;
//        if (!url.endsWith('/')) {
//            url += '/';
//        }
//        url += encodeURIComponent(idValue);
//        window.location.href = url;
//    }

//    showLoader(row) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }
//        sigeneratedCell.innerHTML = `
//                    <div class="loader">
//                        <div class="dot"></div>
//                        <div class="dot"></div>
//                        <div class="dot"></div>
//                    </div>
//                `;
//    }

//    hideLoader(row) {
//        // Loader removed once updateSIGeneratedCell is called
//    }

//    async performAction(menuItem, data = {}) {
//        let currentAction = menuItem.action + this.entity;
//        let create = 'create' + this.entity;
//        let edit = 'edit' + this.entity;
//        let open = 'open' + this.entity;
//        let deleted = 'delete' + this.entity;

//        try {
//            if ([create, deleted].includes(currentAction)) {
//                this.showLoader(this.selectedRow);
//            }

//            const fetchOptions = {
//                method: menuItem.method,
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            };

//            if (menuItem.method.toUpperCase() !== 'GET') {
//                fetchOptions.body = JSON.stringify(data);
//                const token = this.getAntiForgeryToken();
//                if (token) {
//                    fetchOptions.headers['RequestVerificationToken'] = token;
//                }
//            }
//            const url = (currentAction === open || currentAction === edit)
//                ? `${menuItem.actionUrl}?${this.getIdKey()}=${encodeURIComponent(data[this.getIdKey()])}`
//                : menuItem.actionUrl;

//            const response = await fetch(url, fetchOptions);
//            const result = await response.json();
//            if (result.success) {
//                switch (currentAction) {
//                    case create:
//                        notyf.success(`Invoice created successfully!`);
//                        this.selectedRow.classList.add('bg-soft-primary');
//                        this.updateSIGeneratedCell(this.selectedRow, true);
//                        this.updateGroupButton(this.selectedRow, true);
//                        break;
//                    case open:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success('Invoice opened successfully!');
//                        } else {
//                            notyf.error(`URL for ${this.entity} not found.`);
//                        }
//                        break;
//                    case edit:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success(`Invoice is ready for editing.`);
//                        } else {
//                            notyf.error(`Edit URL for Invoice not found.`);
//                        }
//                        break;
//                    case deleted:
//                        notyf.success(`Invoice deleted successfully!`);
//                        this.selectedRow.classList.remove('bg-soft-primary');
//                        this.updateSIGeneratedCell(this.selectedRow, false);
//                        this.updateGroupButton(this.selectedRow, false);
//                        break;
//                    default:
//                        notyf.success(`Invoice '${menuItem.label}' completed successfully!`);
//                }
//            } else {
//                notyf.error(result.message || `Failed to ${menuItem.label}.`);
//                if ([create, deleted].includes(currentAction)) {
//                    this.hideLoader(this.selectedRow);
//                }
//            }
//        } catch (error) {
//            console.error(`Error performing action '${menuItem.action}' on ${this.entity}:`, error);
//            notyf.error(`An error occurred while performing '${menuItem.label}'.`);
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        } finally {
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        }
//    }

//    updateSIGeneratedCell(row, isSIExist) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }

//        if (isSIExist) {
//            sigeneratedCell.innerHTML = `
//                        <span class="badge bg-soft-success text-success">
//                            <span class="legend-indicator bg-success"></span> Yes
//                        </span>
//                    `;
//        } else {
//            sigeneratedCell.innerHTML = `
//                        <span class="badge bg-soft-danger text-danger">
//                            <span class="legend-indicator bg-danger"></span> No
//                        </span>
//                    `;
//        }
//    }

//    updateGroupButton(row, isSIExist) {
//        const actionsCell = row.querySelector('td:nth-child(9)');
//        if (!actionsCell) {
//            console.error('Actions cell not found.');
//            return;
//        }

//        const dropdownMenu = actionsCell.querySelector('.dropdown-menu');
//        if (!dropdownMenu) {
//            console.error('Dropdown menu not found in actions cell.');
//            return;
//        }

//        const divider = dropdownMenu.querySelector('.dropdown-divider');
//        if (!divider) {
//            console.error('Dropdown divider not found. Cannot update group button.');
//            return;
//        }

//        // Remove all links after the divider (invoice related)
//        let followingLinks = divider.nextElementSibling;
//        while (followingLinks) {
//            const next = followingLinks.nextElementSibling;
//            dropdownMenu.removeChild(followingLinks);
//            followingLinks = next;
//        }

//        // Insert the appropriate link based on isSIExist
//        if (!isSIExist) {
//            const generateLinkHtml = `
//                        <a href="#" class="dropdown-item generate-invoice" data-so-id="${this.selectedData[this.getIdKey()]}" title="Generate Invoice">
//                            <i class="bi-plus-circle dropdown-item-icon"></i> Generate Invoice
//                        </a>
//                    `;
//            dropdownMenu.insertAdjacentHTML('beforeend', generateLinkHtml);
//        } else {
//            const gotoLinkHtml = `
//                        <a href="#" class="dropdown-item goto-invoice" data-so-id="${this.selectedData[this.getIdKey()]}" title="Goto Invoice">
//                            <i class="bi-box-arrow-up-left dropdown-item-icon"></i> Goto Invoice
//                        </a>
//                    `;
//            dropdownMenu.insertAdjacentHTML('beforeend', gotoLinkHtml);
//        }
//    }

//    getIdKey() {
//        return this.dataIdAttribute.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//    }

//    getQueryParam() {
//        return this.getIdKey();
//    }

//    showMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'flex';
//            menuItem.style.visibility = 'visible';
//        }
//    }

//    hideMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'none';
//            menuItem.style.visibility = 'hidden';
//        }
//    }

//    hideAllMenuItems() {
//        this.menuItems.forEach(item => {
//            this.hideMenuItem(item.id);
//        });
//    }

//    showContextMenu(x, y) {
//        this.contextMenu.setAttribute('aria-hidden', 'false');
//        this.contextMenu.style.display = 'block';
//        this.contextMenu.style.opacity = '0';
//        this.contextMenu.style.transform = 'scale(0.95)';

//        const menuWidth = this.contextMenu.offsetWidth;
//        const menuHeight = this.contextMenu.offsetHeight;
//        const viewportWidth = window.innerWidth;
//        const viewportHeight = window.innerHeight;

//        let finalX = x;
//        let finalY = y;

//        if (x + menuWidth > viewportWidth) {
//            finalX = viewportWidth - menuWidth - 10;
//        }

//        if (y + menuHeight > viewportHeight) {
//            finalY = viewportHeight - menuHeight - 10;
//        }

//        this.contextMenu.style.top = `${finalY}px`;
//        this.contextMenu.style.left = `${finalX}px`;
//        this.contextMenu.classList.remove('hidden');
//        this.contextMenu.classList.add('visible');

//        this.contextMenu.style.display = '';

//        const menuItems = this.contextMenu.querySelectorAll('li');
//        menuItems.forEach(item => {
//            item.setAttribute('tabindex', '-1');
//        });
//    }

//    hideContextMenu() {
//        if (this.contextMenu) {
//            this.contextMenu.setAttribute('aria-hidden', 'true');
//            this.contextMenu.classList.remove('visible');
//            this.contextMenu.classList.add('hidden');

//            const focusedElement = document.activeElement;
//            if (this.contextMenu.contains(focusedElement)) {
//                focusedElement.blur();
//            }
//        }
//    }

//    getRowData(row) {
//        const data = {};
//        Array.from(row.attributes).forEach(attr => {
//            if (attr.name.startsWith('data-')) {
//                const key = attr.name.replace('data-', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//                data[key] = attr.value;
//            }
//        });
//        console.log(`Row Data for ${this.entity}:`, data);
//        return data;
//    }

//    getAntiForgeryToken() {
//        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
//        return tokenElement ? tokenElement.value : '';
//    }

//    getMenuItemByAction(action) {
//        return this.menuItems.find(item => item.action === action);
//    }
//}


/*============================== V-7 ================================*/
//export class ContextMenuTable {
//    constructor(options) {
//        this.tables = document.querySelectorAll(options.tableSelector);
//        this.entity = options.entity; // e.g., 'SI', 'PO', 'PI', etc.
//        this.dataIdAttribute = options.dataIdAttribute; // e.g., 'so-id', 'po-id'
//        this.checkExistsUrl = options.checkExistsUrl; // e.g., '/Inventory/SO/CheckSIExists'
//        this.menuItems = options.menuItems; // Array of menu item configurations
//        this.contextMenu = null;
//        this.selectedRow = null;
//        this.selectedData = null; // Object containing data attributes from the selected row
//        this.lastMouseX = 0;
//        this.lastMouseY = 0;

//        this.init();
//    }

//    init() {
//        this.injectStyles();
//        this.createContextMenu();
//        this.attachEvents();
//    }

//    injectStyles() {
//        const style = document.createElement('style');
//        style.innerHTML = `
//            /* Existing Styles */

//            .context-menu {
//                display: none;
//                position: fixed;
//                z-index: 1050;
//                width: 220px;
//                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
//                cursor: default;
//                padding: 0;
//                margin: 0;
//                opacity: 0;
//                transition: opacity 0.3s ease, transform 0.3s ease;
//                transform: scale(0.95);
//            }

//            .context-menu.visible {
//                display: block;
//                opacity: 1;
//                transform: scale(1);
//                animation: fadeInUp 0.3s forwards;
//            }

//            .context-menu.hidden {
//                opacity: 0;
//                transform: scale(0.95);
//                animation: fadeOutDown 0.3s forwards;
//            }

//            .context-menu ul {
//                list-style: none;
//                padding: 0;
//                margin: 0;
//            }

//            .context-menu ul li {
//                cursor: pointer;
//            }

//            .context-menu ul li:focus {
//                outline: none; 
//            }

//            .highlight-row {
//                background-color: #d4edda !important;
//                transition: background-color 0.3s ease;
//            }

//            @keyframes fadeInUp {
//                from {
//                    opacity: 0;
//                    transform: translateY(10px) scale(0.95);
//                }
//                to {
//                    opacity: 1;
//                    transform: translateY(0) scale(1);
//                }
//            }

//            @keyframes fadeOutDown {
//                from {
//                    opacity: 1;
//                    transform: translateY(0) scale(1);
//                }
//                to {
//                    opacity: 0;
//                    transform: translateY(10px) scale(0.95);
//                }
//            }

//            /* New Loader Styles */
//            .loader {
//                display: flex;
//                align-items: center;
//                justify-content: center;
//                padding: 10px 0;
//            }

//            .loader .dot {
//                width: 8px;
//                height: 8px;
//                margin: 0 2px;
//                background-color: #007bff; /* Customize the color as needed */
//                border-radius: 50%;
//                animation: loader 1s infinite;
//            }

//            .loader .dot:nth-child(2) {
//                animation-delay: 0.2s;
//            }

//            .loader .dot:nth-child(3) {
//                animation-delay: 0.4s;
//            }

//            @keyframes loader {
//                0%, 80%, 100% {
//                    transform: scale(0);
//                }
//                40% {
//                    transform: scale(1);
//                }
//            }
//        `;
//        document.head.appendChild(style);
//    }

//    createContextMenu() {
//        // Create the context menu element
//        this.contextMenu = document.createElement('div');
//        this.contextMenu.classList.add('context-menu', 'hidden');
//        this.contextMenu.setAttribute('role', 'menu');
//        this.contextMenu.setAttribute('aria-hidden', 'true');

//        // Proper HTML structure: Only <ul> and <li> elements
//        this.contextMenu.innerHTML = `
//            <ul>
//                <div class="dropdown-menu dropdown-menu-end mt-1 show">
//                    ${this.menuItems.map(item => `
//                        <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}" ${item.isHidden ? 'style="display: none; visibility: hidden;"' : ''}>
//                            ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
//                        </li>
//                    `).join('')}
//                </div>
//            </ul>
//        `;
//        document.body.appendChild(this.contextMenu);

//        // Attach event listeners for menu items with correct binding
//        this.menuItems.forEach(item => {
//            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
//            if (menuItem) {
//                // Bind the onClick handler to the ContextMenuTable instance
//                menuItem.addEventListener('click', (e) => {
//                    e.stopPropagation();
//                    if (!item.isHidden) { // Ensure the item is not hidden
//                        this.handleMenuItemClick(item);
//                        this.hideContextMenu();
//                    }
//                });

//                // Keyboard accessibility for Enter key with correct binding
//                menuItem.addEventListener('keydown', (e) => {
//                    if (e.key === 'Enter') {
//                        e.preventDefault();
//                        if (!item.isHidden) { // Ensure the item is not hidden
//                            this.handleMenuItemClick(item);
//                            this.hideContextMenu();
//                        }
//                    }
//                });
//            }
//        });

//        // Hide context menu on window scroll or resize with debouncing
//        const debounce = (func, wait) => {
//            let timeout;
//            return function (...args) {
//                clearTimeout(timeout);
//                timeout = setTimeout(() => func.apply(this, args), wait);
//            };
//        };

//        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
//        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
//    }

//    attachEvents() {
//        this.tables.forEach(table => {
//            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
//        });

//        // Hide context menu on clicking elsewhere
//        document.addEventListener('click', () => this.hideContextMenu());

//        // Hide context menu on pressing the Escape key and handle keyboard navigation
//        document.addEventListener('keydown', (e) => {
//            if (e.key === 'Escape') {
//                this.hideContextMenu();
//            }
//            // Handle keyboard navigation within the context menu
//            if (this.contextMenu.classList.contains('visible')) {
//                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
//                const focusedElement = document.activeElement;
//                const currentIndex = menuItems.indexOf(focusedElement);

//                if (e.key === 'ArrowDown') {
//                    e.preventDefault();
//                    const nextIndex = (currentIndex + 1) % menuItems.length;
//                    menuItems[nextIndex].focus();
//                } else if (e.key === 'ArrowUp') {
//                    e.preventDefault();
//                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
//                    menuItems[prevIndex].focus();
//                }
//            }
//        });
//    }

//    async handleContextMenu(e, table) {
//        e.preventDefault();
//        const target = e.target.closest('tr');
//        if (target && target.rowIndex > 0) { // Ensure it's not the header
//            this.selectedRow = target;
//            this.selectedData = this.getRowData(target);
//            this.lastMouseX = e.clientX;
//            this.lastMouseY = e.clientY;

//            // Fetch entity existence and prepare the menu
//            await this.checkEntityExists();
//            this.showContextMenu(this.lastMouseX, this.lastMouseY);
//        } else {
//            this.hideContextMenu();
//        }
//    }

//    async checkEntityExists() {
//        const idKey = this.getIdKey();
//        if (!this.selectedData || !this.selectedData[idKey]) {
//            console.error(`${this.entity} ID is missing in the selected row data.`);
//            notyf.error('Selected row data is incomplete.');
//            this.hideAllMenuItems();
//            return;
//        }

//        try {
//            const idValue = encodeURIComponent(this.selectedData[idKey]);
//            const response = await fetch(`${this.checkExistsUrl}?${this.getQueryParam()}=${idValue}`, {
//                method: 'GET',
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            });

//            const result = await response.json(); // Properly await the JSON response

//            if (result.success) {
//                this.selectedData.entityExists = result.entityExists;

//                // Iterate through menu items to show/hide based on existence and static links
//                this.menuItems.forEach(item => {
//                    if (!item.isJsonFetchRequest) {
//                        // Static Links: Show if not hidden
//                        if (!item.isHidden) {
//                            this.showMenuItem(item.id);
//                        }
//                        return; // Skip further conditions for static links
//                    }

//                    // Dynamic Fetch Links: Show based on SI existence and not hidden
//                    if (this.selectedData.entityExists && item.showIfExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else if (!this.selectedData.entityExists && item.showIfNotExists && !item.isHidden) {
//                        this.showMenuItem(item.id);
//                    } else {
//                        this.hideMenuItem(item.id);
//                    }
//                });
//            } else {
//                console.error(`Failed to check ${this.entity} existence:`, result.message);
//                notyf.error(result.message || `Failed to check ${this.entity} existence.`);
//                this.hideAllMenuItems();
//            }
//        } catch (error) {
//            console.error(`Error checking ${this.entity} existence:`, error);
//            notyf.error(`An error occurred while checking ${this.entity} existence.`);
//            this.hideAllMenuItems();
//        }
//    }

//    handleMenuItemClick(menuItem) {
//        const idKey = this.getIdKey();
//        const idValue = this.selectedData[idKey];

//        if (menuItem.isJsonFetchRequest) {
//            // Dynamic Fetch Links
//            this.performAction(menuItem, { [idKey]: idValue });
//        } else {
//            // Static Links: Direct Navigation
//            this.navigateToStaticLink(menuItem, idValue);
//        }
//    }

//    /**
//     * Navigate to Static Link
//     * @param {Object} menuItem - The menu item configuration.
//     * @param {string|number} idValue - The ID value to append to the URL.
//     */
//    navigateToStaticLink(menuItem, idValue) {
//        let url = menuItem.actionUrl;

//        // Ensure there is a trailing slash
//        if (!url.endsWith('/')) {
//            url += '/';
//        }

//        // Append the ID as a route segment
//        url += encodeURIComponent(idValue);

//        // Navigate to the URL in the same tab
//        window.location.href = url;

//        // Alternatively, to open in a new tab, uncomment the line below:
//        // window.open(url, '_blank');
//    }

//    /**
//     * New Method: Show Loader
//     * Adds the loader to the "SI Generated" cell.
//     * @param {HTMLElement} row - The table row where the loader should appear.
//     */
//    showLoader(row) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }

//        // Insert the loader HTML
//        sigeneratedCell.innerHTML = `
//            <div class="loader">
//                <div class="dot"></div>
//                <div class="dot"></div>
//                <div class="dot"></div>
//            </div>
//        `;
//    }

//    /**
//     * New Method: Hide Loader
//     * Removes the loader from the "SI Generated" cell.
//     * @param {HTMLElement} row - The table row from which the loader should be removed.
//     */
//    hideLoader(row) {
//        // Since `updateSIGeneratedCell` replaces the cell's content,
//        // the loader will automatically be removed. This method can remain empty.
//        // However, it's kept for potential future enhancements.
//    }

//    async performAction(menuItem, data = {}) {
//        try {
//            // Show loader for dynamic fetch actions
//            let currentAction = menuItem.action + this.entity;
//            let create = 'create' + this.entity;
//            let edit = 'edit' + this.entity;
//            let open = 'open' + this.entity;
//            let deleted = 'delete' + this.entity;
//            if ([create, deleted].includes(currentAction)) {
//                this.showLoader(this.selectedRow);
//            }

//            const fetchOptions = {
//                method: menuItem.method,
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            };

//            if (menuItem.method.toUpperCase() !== 'GET') {
//                fetchOptions.body = JSON.stringify(data);
//                const token = this.getAntiForgeryToken();
//                if (token) {
//                    fetchOptions.headers['RequestVerificationToken'] = token;
//                }
//            }
//            const url = (currentAction === open || currentAction === edit)
//                ? `${menuItem.actionUrl}?${this.getIdKey()}=${encodeURIComponent(data[this.getIdKey()])}`
//                : menuItem.actionUrl;

//            const response = await fetch(url, fetchOptions);
//            const result = await response.json();
//            if (result.success) {
//                switch (currentAction) {
//                    case create:
//                        notyf.success(`Invoice created successfully!`);
//                        this.selectedRow.classList.add('bg-soft-primary');
//                        this.updateSIGeneratedCell(this.selectedRow, true);
//                        break;
//                    case open:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success('Invoice opened successfully!');
//                        } else {
//                            notyf.error(`URL for ${this.entity} not found.`);
//                        }
//                        break;
//                    case edit:
//                        if (result.entityUrl) {
//                            window.open(result.entityUrl, '_blank');
//                            notyf.success(`Invoice is ready for editing.`);
//                        } else {
//                            notyf.error(`Edit URL for Invoice not found.`);
//                        }
//                        break;
//                    case deleted:
//                        notyf.success(`Invoice deleted successfully!`);
//                        this.selectedRow.classList.remove('bg-soft-primary');
//                        this.updateSIGeneratedCell(this.selectedRow, false);
//                        break;
//                    default:
//                        notyf.success(`Invoice '${menuItem.label}' completed successfully!`);
//                }
//            }
//            else
//            {
//                notyf.error(result.message || `Failed to ${menuItem.label}.`);
//                if ([create, deleted].includes(currentAction)) {
//                    this.hideLoader(this.selectedRow);
//                }
//            }
//        } catch (error) {
//            console.error(`Error performing action '${menuItem.action}' on ${this.entity}:`, error);
//            notyf.error(`An error occurred while performing '${menuItem.label}'.`);
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        } finally {
//            if ([create, deleted].includes(currentAction)) {
//                this.hideLoader(this.selectedRow);
//            }
//        }
//    }

//    updateSIGeneratedCell(row, isSIExist) {
//        const sigeneratedCell = row.querySelector('td:nth-child(7)');
//        if (!sigeneratedCell) {
//            console.error('SI Generated cell not found.');
//            return;
//        }
        
//        if (isSIExist) {
//            sigeneratedCell.innerHTML = `
//                <span class="badge bg-soft-success text-success">
//                    <span class="legend-indicator bg-success"></span> Yes
//                </span>
//            `;
//        } else {
//            sigeneratedCell.innerHTML = `
//                <span class="badge bg-soft-danger text-danger">
//                    <span class="legend-indicator bg-danger"></span> No
//                </span>
//            `;
//        }
//    }

//    getIdKey() {
//        // Convert data attribute to camelCase key
//        // e.g., 'so-id' => 'soId', 'po-id' => 'poId'
//        return this.dataIdAttribute.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//    }

//    getQueryParam() {
//        // Return the camelCase key for query parameters
//        return this.getIdKey();
//    }

//    showMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'flex';
//            menuItem.style.visibility = 'visible';
//        }
//    }

//    hideMenuItem(id) {
//        const menuItem = this.contextMenu.querySelector(`#${id}`);
//        if (menuItem) {
//            menuItem.style.display = 'none';
//            menuItem.style.visibility = 'hidden';
//        }
//    }

//    hideAllMenuItems() {
//        this.menuItems.forEach(item => {
//            this.hideMenuItem(item.id);
//        });
//    }

//    showContextMenu(x, y) {
//        // Set aria-hidden to false
//        this.contextMenu.setAttribute('aria-hidden', 'false');

//        // Temporarily display the menu to calculate its size
//        this.contextMenu.style.display = 'block';
//        this.contextMenu.style.opacity = '0';
//        this.contextMenu.style.transform = 'scale(0.95)';

//        // Calculate position to prevent overflow
//        const menuWidth = this.contextMenu.offsetWidth;
//        const menuHeight = this.contextMenu.offsetHeight;
//        const viewportWidth = window.innerWidth;
//        const viewportHeight = window.innerHeight;

//        let finalX = x;
//        let finalY = y;

//        if (x + menuWidth > viewportWidth) {
//            finalX = viewportWidth - menuWidth - 10; // 10px padding
//        }

//        if (y + menuHeight > viewportHeight) {
//            finalY = viewportHeight - menuHeight - 10; // 10px padding
//        }

//        // Apply calculated position and show the menu
//        this.contextMenu.style.top = `${finalY}px`;
//        this.contextMenu.style.left = `${finalX}px`;
//        this.contextMenu.classList.remove('hidden');
//        this.contextMenu.classList.add('visible');

//        // Reset temporary styles
//        this.contextMenu.style.display = '';

//        // Remove focus from any menu items to prevent default focus styling
//        const menuItems = this.contextMenu.querySelectorAll('li');
//        menuItems.forEach(item => {
//            item.setAttribute('tabindex', '-1');
//        });
//    }

//    hideContextMenu() {
//        if (this.contextMenu) {
//            // Set aria-hidden to true
//            this.contextMenu.setAttribute('aria-hidden', 'true');
//            this.contextMenu.classList.remove('visible');
//            this.contextMenu.classList.add('hidden');

//            // Remove focus from any menu items
//            const focusedElement = document.activeElement;
//            if (this.contextMenu.contains(focusedElement)) {
//                focusedElement.blur();
//            }
//        }
//    }

//    getRowData(row) {
//        const data = {};

//        Array.from(row.attributes).forEach(attr => {
//            if (attr.name.startsWith('data-')) {
//                // Remove 'data-' prefix and hyphens, then convert to camelCase
//                const key = attr.name.replace('data-', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
//                data[key] = attr.value;
//            }
//        });

//        console.log(`Row Data for ${this.entity}:`, data); // For debugging purposes
//        return data;
//    }

//    // Function to get anti-forgery token
//    getAntiForgeryToken() {
//        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
//        return tokenElement ? tokenElement.value : '';
//    }
//}


/*============================== V-5 ================================*/
/*
export class ContextMenuTable {
    constructor(options) {
        this.tables = document.querySelectorAll(options.tableSelector);
        this.entity = options.entity; // e.g., 'SI', 'PO', 'PI', etc.
        this.dataIdAttribute = options.dataIdAttribute; // e.g., 'so-id', 'po-id'
        this.checkExistsUrl = options.checkExistsUrl; // e.g., '/SI/CheckSIExists'
        this.menuItems = options.menuItems; // Array of menu item configurations
        this.contextMenu = null;
        this.selectedRow = null;
        this.selectedData = null; // Object containing data attributes from the selected row
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.init();
    }

    init() {
        this.injectStyles();
        this.createContextMenu();
        this.attachEvents();
    }

    injectStyles() {
        const style = document.createElement('style');
        style.innerHTML = `

            .context-menu {
                display: none;
                position: fixed;
                z-index: 1050;
                width: 220px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                cursor: default;
                padding: 0;
                margin: 0;
                opacity: 0;
                transition: opacity 0.3s ease, transform 0.3s ease;
                transform: scale(0.95);
            }

            .context-menu.visible {
                display: block;
                opacity: 1;
                transform: scale(1);
                animation: fadeInUp 0.3s forwards;
            }

            .context-menu.hidden {
                opacity: 0;
                transform: scale(0.95);
                animation: fadeOutDown 0.3s forwards;
            }

            .context-menu ul {
                list-style: none;
                padding: 0;
                margin: 0;
            }

            .context-menu ul li {
                cursor: pointer;
            }

            .context-menu ul li:focus {
                outline: none; 
            }

            .highlight-row {
                background-color: #d4edda !important;
                transition: background-color 0.3s ease;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
                to {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
            }

            @keyframes fadeOutDown {
                from {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
                to {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
            }

            .loader {
                display: flex;
                align-items: center;
                justify-content: center;
                padding: 10px 0;
            }

            .loader .dot {
                width: 8px;
                height: 8px;
                margin: 0 2px;
                background-color: #007bff; 
                border-radius: 50%;
                animation: loader 1s infinite;
            }

            .loader .dot:nth-child(2) {
                animation-delay: 0.2s;
            }

            .loader .dot:nth-child(3) {
                animation-delay: 0.4s;
            }

            @keyframes loader {
                0%, 80%, 100% {
                    transform: scale(0);
                }
                40% {
                    transform: scale(1);
                }
            }
        `;
        document.head.appendChild(style);
    }

    createContextMenu() {
        // Create the context menu element
        this.contextMenu = document.createElement('div');
        this.contextMenu.classList.add('context-menu', 'hidden');
        this.contextMenu.setAttribute('role', 'menu');
        this.contextMenu.setAttribute('aria-hidden', 'true');

        // Proper HTML structure: Only <ul> and <li> elements
        this.contextMenu.innerHTML = `
            <ul>
                <div class="dropdown-menu dropdown-menu-end mt-1 show">

                ${this.menuItems.map(item => `
                    <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}">
                        ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
                    </li>
                `).join('')}
                </div>

            </ul>
        `;
        document.body.appendChild(this.contextMenu);

        // Attach event listeners for menu items with correct binding
        this.menuItems.forEach(item => {
            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
            if (menuItem) {
                // Bind the onClick handler to the ContextMenuTable instance
                menuItem.addEventListener('click', (e) => {
                    e.stopPropagation();
                    this.handleMenuItemClick(item);
                    this.hideContextMenu();
                });

                // Keyboard accessibility for Enter key with correct binding
                menuItem.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        this.handleMenuItemClick(item);
                        this.hideContextMenu();
                    }
                });
            }
        });

        // Hide context menu on window scroll or resize with debouncing
        const debounce = (func, wait) => {
            let timeout;
            return function (...args) {
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(this, args), wait);
            };
        };

        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
    }

    attachEvents() {
        this.tables.forEach(table => {
            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
        });

        // Hide context menu on clicking elsewhere
        document.addEventListener('click', () => this.hideContextMenu());

        // Hide context menu on pressing the Escape key and handle keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.hideContextMenu();
            }
            // Handle keyboard navigation within the context menu
            if (this.contextMenu.classList.contains('visible')) {
                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
                const focusedElement = document.activeElement;
                const currentIndex = menuItems.indexOf(focusedElement);

                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = (currentIndex + 1) % menuItems.length;
                    menuItems[nextIndex].focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
                    menuItems[prevIndex].focus();
                }
            }
        });
    }

    async handleContextMenu(e, table) {
        e.preventDefault();
        const target = e.target.closest('tr');
        if (target && target.rowIndex > 0) { // Ensure it's not the header
            this.selectedRow = target;
            this.selectedData = this.getRowData(target);
            this.lastMouseX = e.clientX;
            this.lastMouseY = e.clientY;

            // Fetch entity existence and prepare the menu
            await this.checkEntityExists();
            this.showContextMenu(this.lastMouseX, this.lastMouseY);
        } else {
            this.hideContextMenu();
        }
    }

    async checkEntityExists() {
        const idKey = this.getIdKey();
        if (!this.selectedData || !this.selectedData[idKey]) {
            console.error(`${this.entity} ID is missing in the selected row data.`);
            notyf.error('Selected row data is incomplete.');
            this.hideAllMenuItems();
            return;
        }

        try {
            const idValue = encodeURIComponent(this.selectedData[idKey]);
            const response = await fetch(`${this.checkExistsUrl}?${this.getQueryParam()}=${idValue}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const result = await response.json(); // Properly await the JSON response

            if (result.success) {
                this.selectedData.entityExists = result.entityExists;

                // First, hide all menu items
                this.hideAllMenuItems();

                // Iterate through menu items to show/hide based on existence
                this.menuItems.forEach(item => {
                    if (this.selectedData.entityExists) {
                        if (item.showIfExists) {
                            this.showMenuItem(item.id);
                        }
                    } else {
                        if (item.showIfNotExists) {
                            this.showMenuItem(item.id);
                        }
                    }
                });
            } else {
                console.error(`Failed to check ${this.entity} existence:`, result.message);
                notyf.error(result.message || `Failed to check ${this.entity} existence.`);
                this.hideAllMenuItems();
            }
        } catch (error) {
            console.error(`Error checking ${this.entity} existence:`, error);
            notyf.error(`An error occurred while checking ${this.entity} existence.`);
            this.hideAllMenuItems();
        }
    }

    handleMenuItemClick(menuItem) {
        const idKey = this.getIdKey();
        const idValue = this.selectedData[idKey];

        switch (menuItem.action) {
            case 'create':
                this.performAction(menuItem, { [idKey]: idValue });
                break;
            case 'open':
                this.performAction(menuItem, { [idKey]: idValue }, true);
                break;
            case 'delete':
                this.performAction(menuItem, { [idKey]: idValue });
                break;
            case 'edit': // **Handle 'edit' action**
                this.performAction(menuItem, { [idKey]: idValue });
                break;
            default:
                console.warn(`Unhandled action: ${menuItem.action}`);
        }
    }

    showLoader(row) {
        const sigeneratedCell = row.querySelector('td:nth-child(7)');
        if (!sigeneratedCell) {
            console.error('SI Generated cell not found.');
            return;
        }

        // Insert the loader HTML
        sigeneratedCell.innerHTML = `
            <div class="loader">
                <div class="dot"></div>
                <div class="dot"></div>
                <div class="dot"></div>
            </div>
        `;
    }

    hideLoader(row) {
        // Since `updateSIGeneratedCell` replaces the cell's content,
        // the loader will automatically be removed. This method can remain empty.
        // However, it's kept for potential future enhancements.
    }

    async performAction(menuItem, data = {}, isOpen = false) {
        console.log('Data Sent:', data);
        try {
            // **New Addition: Show Loader for 'create', 'delete', and 'edit' Actions**
            if (['create', 'delete'].includes(menuItem.action)) {
                this.showLoader(this.selectedRow);
            }

            const fetchOptions = {
                method: menuItem.method,
                headers: {
                    'Content-Type': 'application/json'
                }
            };

            if (menuItem.method.toUpperCase() !== 'GET') {
                fetchOptions.body = JSON.stringify(data);
                const token = this.getAntiForgeryToken();
                if (token) {
                    fetchOptions.headers['RequestVerificationToken'] = token;
                }
            }

            // For 'open' and 'edit' actions, include the ID in the URL as a query parameter
            // For 'create' and 'delete', send the ID in the request body
            const url = (menuItem.action === 'open' || menuItem.action === 'edit')
                ? `${menuItem.actionUrl}?${this.getQueryParam()}=${encodeURIComponent(data[this.getIdKey()])}`
                : menuItem.actionUrl;

 
            const response = await fetch(url, fetchOptions);
            const result = await response.json();
            console.log(result);

            if (result.success) {
                switch (menuItem.action) {
                    case 'create':
                        notyf.success(`${this.entity} created successfully!`);
                        this.selectedRow.classList.add('bg-soft-primary');
                        // Update the "SI Generated" column with "Yes" badge
                        this.updateSIGeneratedCell(this.selectedRow, true);
                        break;
                    case 'open':
                        if (result.entityUrl) {
                            window.open(result.entityUrl, '_blank');
                        } else {
                            notyf.error(`URL for ${this.entity} not found.`);
                        }
                        break;
                    case 'edit': // **Handle 'edit' action**
                        if (result.entityUrl) {
                            window.open(result.entityUrl, '_blank'); // Open edit page
                            notyf.success(`${this.entity} is ready for editing.`);
                        } else {
                            notyf.error(`Edit URL for ${this.entity} not found.`);
                        }
                        break;
                    case 'delete':
                        notyf.success(`${this.entity} deleted successfully!`);
                        this.selectedRow.classList.remove('bg-soft-primary');
                        // Update the "SI Generated" column with "No" badge
                        this.updateSIGeneratedCell(this.selectedRow, false);
                        break;
                    default:
                        notyf.success(`${this.entity} action '${menuItem.label}' completed successfully!`);
                }
            } else {
                notyf.error(result.message || `Failed to ${menuItem.label}.`);
                // **Hide Loader if operation failed**
                if (['create', 'delete', 'edit'].includes(menuItem.action)) {
                    this.hideLoader(this.selectedRow);
                }
            }
        } catch (error) {
            console.error(`Error performing action '${menuItem.action}' on ${this.entity}:`, error);
            notyf.error(`An error occurred while performing '${menuItem.label}'.`);
        } finally {
            // **New Addition: Hide Loader for 'create', 'delete', and 'edit' Actions**
            if (['create', 'delete'].includes(menuItem.action)) {
                this.hideLoader(this.selectedRow);
            }
        }
    }

    updateSIGeneratedCell(row, isSIExist) {
        const sigeneratedCell = row.querySelector('td:nth-child(7)');
        if (!sigeneratedCell) {
            console.error('SI Generated cell not found.');
            return;
        }

        if (isSIExist) {
            sigeneratedCell.innerHTML = `
                <span class="badge bg-soft-success text-success">
                    <span class="legend-indicator bg-success"></span> Yes
                </span>
            `;
        } else {
            sigeneratedCell.innerHTML = `
                <span class="badge bg-soft-danger text-danger">
                    <span class="legend-indicator bg-danger"></span> No
                </span>
            `;
        }
    }

    getIdKey() {
        // Convert data attribute to camelCase key
        // e.g., 'so-id' => 'soId', 'po-id' => 'poId'
        return this.dataIdAttribute.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
    }

    getQueryParam() {
        // Return the camelCase key for query parameters
        return this.getIdKey();
    }

    showMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'flex';
            menuItem.style.visibility = 'visible';
        }
    }

    hideMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'none';
            menuItem.style.visibility = 'hidden';
        }
    }

    hideAllMenuItems() {
        this.menuItems.forEach(item => {
            this.hideMenuItem(item.id);
        });
    }

    showContextMenu(x, y) {
        // Set aria-hidden to false
        this.contextMenu.setAttribute('aria-hidden', 'false');

        // Temporarily display the menu to calculate its size
        this.contextMenu.style.display = 'block';
        this.contextMenu.style.opacity = '0';
        this.contextMenu.style.transform = 'scale(0.95)';

        // Calculate position to prevent overflow
        const menuWidth = this.contextMenu.offsetWidth;
        const menuHeight = this.contextMenu.offsetHeight;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let finalX = x;
        let finalY = y;

        if (x + menuWidth > viewportWidth) {
            finalX = viewportWidth - menuWidth - 10; // 10px padding
        }

        if (y + menuHeight > viewportHeight) {
            finalY = viewportHeight - menuHeight - 10; // 10px padding
        }

        // Apply calculated position and show the menu
        this.contextMenu.style.top = `${finalY}px`;
        this.contextMenu.style.left = `${finalX}px`;
        this.contextMenu.classList.remove('hidden');
        this.contextMenu.classList.add('visible');

        // Reset temporary styles
        this.contextMenu.style.display = '';

        // Remove focus from any menu items to prevent default focus styling
        const menuItems = this.contextMenu.querySelectorAll('li');
        menuItems.forEach(item => {
            item.setAttribute('tabindex', '-1');
        });
    }

    hideContextMenu() {
        if (this.contextMenu) {
            // Set aria-hidden to true
            this.contextMenu.setAttribute('aria-hidden', 'true');
            this.contextMenu.classList.remove('visible');
            this.contextMenu.classList.add('hidden');

            // Remove focus from any menu items
            const focusedElement = document.activeElement;
            if (this.contextMenu.contains(focusedElement)) {
                focusedElement.blur();
            }
        }
    }

    getRowData(row) {
        const data = {};

        Array.from(row.attributes).forEach(attr => {
            if (attr.name.startsWith('data-')) {
                // Remove 'data-' prefix and hyphens, then convert to camelCase
                const key = attr.name.replace('data-', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
                data[key] = attr.value;
            }
        });

        console.log(`Row Data for ${this.entity}:`, data); // For debugging purposes
        return data;
    }

    // Function to get anti-forgery token
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }
}
*/


//============================== V-4 ================================
/*export class ContextMenuTable {
    constructor(options) {
        this.tables = document.querySelectorAll(options.tableSelector);
        this.entity = options.entity; // e.g., 'SI', 'PO', 'PI', etc.
        this.dataIdAttribute = options.dataIdAttribute; // e.g., 'so-id', 'po-id'
        this.checkExistsUrl = options.checkExistsUrl; // e.g., '/SI/CheckSIExists'
        this.menuItems = options.menuItems; // Array of menu item configurations
        this.contextMenu = null;
        this.selectedRow = null;
        this.selectedData = null; // Object containing data attributes from the selected row
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.init();
    }

    init() {
        this.injectStyles();
        this.createContextMenu();
        this.attachEvents();
    }

    injectStyles() {
        const style = document.createElement('style');
        style.innerHTML = `
            .context-menu {
                display: none;
                position: fixed;
                z-index: 1050;
                width: 220px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                cursor: default;
                padding: 0;
                margin: 0;
                opacity: 0;
                transition: opacity 0.3s ease, transform 0.3s ease;
                transform: scale(0.95);
            }

            .context-menu.visible {
                display: block;
                opacity: 1;
                transform: scale(1);
                animation: fadeInUp 0.3s forwards;
            }

            .context-menu.hidden {
                opacity: 0;
                transform: scale(0.95);
                animation: fadeOutDown 0.3s forwards;
            }

            .context-menu ul {
                list-style: none;
                padding: 0;
                margin: 0;
            }

            .context-menu ul li {
                cursor: pointer;
            }

           
            .context-menu ul li:focus {
                outline: none; 
            }

            .highlight-row {
                background-color: #d4edda !important;
                transition: background-color 0.3s ease;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
                to {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
            }

            @keyframes fadeOutDown {
                from {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
                to {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
            }
        `;
        document.head.appendChild(style);
    }

    createContextMenu() {
        // Create the context menu element
        this.contextMenu = document.createElement('div');
        this.contextMenu.classList.add('context-menu', 'hidden');
        this.contextMenu.setAttribute('role', 'menu');
        this.contextMenu.setAttribute('aria-hidden', 'true');

        // Proper HTML structure: Only <ul> and <li> elements
        this.contextMenu.innerHTML = `
            <ul>
                <div class="dropdown-menu dropdown-menu-end mt-1 show">

                ${this.menuItems.map(item => `
                    <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}">
                        ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
                    </li>
                `).join('')}
                </div>

            </ul>
        `;
        document.body.appendChild(this.contextMenu);

        // Attach event listeners for menu items with correct binding
        this.menuItems.forEach(item => {
            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
            if (menuItem) {
                // Bind the onClick handler to the ContextMenuTable instance
                menuItem.addEventListener('click', (e) => {
                    e.stopPropagation();
                    this.handleMenuItemClick(item);
                    this.hideContextMenu();
                });

                // Keyboard accessibility for Enter key with correct binding
                menuItem.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        this.handleMenuItemClick(item);
                        this.hideContextMenu();
                    }
                });
            }
        });

        // Hide context menu on window scroll or resize with debouncing
        const debounce = (func, wait) => {
            let timeout;
            return function (...args) {
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(this, args), wait);
            };
        };

        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
    }

    attachEvents() {
        this.tables.forEach(table => {
            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
        });

        // Hide context menu on clicking elsewhere
        document.addEventListener('click', () => this.hideContextMenu());

        // Hide context menu on pressing the Escape key and handle keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.hideContextMenu();
            }
            // Handle keyboard navigation within the context menu
            if (this.contextMenu.classList.contains('visible')) {
                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
                const focusedElement = document.activeElement;
                const currentIndex = menuItems.indexOf(focusedElement);

                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = (currentIndex + 1) % menuItems.length;
                    menuItems[nextIndex].focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
                    menuItems[prevIndex].focus();
                }
            }
        });
    }

    async handleContextMenu(e, table) {
        e.preventDefault();
        const target = e.target.closest('tr');
        if (target && target.rowIndex > 0) { // Ensure it's not the header
            this.selectedRow = target;
            this.selectedData = this.getRowData(target);
            this.lastMouseX = e.clientX;
            this.lastMouseY = e.clientY;

            // Fetch entity existence and prepare the menu
            await this.checkEntityExists();
            this.showContextMenu(this.lastMouseX, this.lastMouseY);
        } else {
            this.hideContextMenu();
        }
    }

    async checkEntityExists() {
        const idKey = this.getIdKey();
        if (!this.selectedData || !this.selectedData[idKey]) {
            console.error(`${this.entity} ID is missing in the selected row data.`);
            notyf.error('Selected row data is incomplete.');
            this.hideAllMenuItems();
            return;
        }

        try {
            const idValue = encodeURIComponent(this.selectedData[idKey]);
            const response = await fetch(`${this.checkExistsUrl}?${this.getQueryParam()}=${idValue}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const result = await response.json(); // Properly await the JSON response

            if (result.success) {
                this.selectedData.entityExists = result.entityExists;

                // First, hide all menu items
                this.hideAllMenuItems();

                // Iterate through menu items to show/hide based on existence
                this.menuItems.forEach(item => {
                    if (this.selectedData.entityExists) {
                        if (item.showIfExists) {
                            this.showMenuItem(item.id);
                        }
                    } else {
                        if (item.showIfNotExists) {
                            this.showMenuItem(item.id);
                        }
                    }
                });
            } else {
                console.error(`Failed to check ${this.entity} existence:`, result.message);
                notyf.error(result.message || `Failed to check ${this.entity} existence.`);
                this.hideAllMenuItems();
            }
        } catch (error) {
            console.error(`Error checking ${this.entity} existence:`, error);
            notyf.error(`An error occurred while checking ${this.entity} existence.`);
            this.hideAllMenuItems();
        }
    }

    handleMenuItemClick(menuItem) {
        const idKey = this.getIdKey();
        const idValue = this.selectedData[idKey];

        switch (menuItem.action) {
            case 'create':
                this.performAction(menuItem, { [idKey]: idValue });
                break;
            case 'open':
                this.performAction(menuItem, { [idKey]: idValue }, true);
                break;
            case 'delete':
                this.performAction(menuItem, { [idKey]: idValue });
                break;
            default:
                console.warn(`Unhandled action: ${menuItem.action}`);
        }
    }

    async performAction(menuItem, data = {}, isOpen = false) {
        debugger;
        console.log('Data Sent:', data);
        try {

            const fetchOptions = {
                method: menuItem.method,
                headers: {
                    'Content-Type': 'application/json'
                }
            };

            if (menuItem.method.toUpperCase() !== 'GET') {
                fetchOptions.body = JSON.stringify(data);
                const token = this.getAntiForgeryToken();
                if (token) {
                    fetchOptions.headers['RequestVerificationToken'] = token;
                }
            }

            // For 'open' action, include the ID in the URL as a query parameter
            // For 'create' and 'delete', send the ID in the request body
            const url = isOpen
                ? `${menuItem.actionUrl}?${this.getQueryParam()}=${encodeURIComponent(data[this.getIdKey()])}`
                : menuItem.actionUrl;

            console.log('Request URL:', url);
            console.log('Fetch Options:', fetchOptions);
            console.log('Is Open:', isOpen);

            const response = await fetch(url, fetchOptions);
            const result = await response.json();
            console.log(result);
            if (result.success) {
                switch (menuItem.action) {
                    case 'create':
                        notyf.success(`${this.entity} created successfully!`);
                        this.selectedRow.classList.add('bg-soft-primary');
                        // Update the "SI Generated" column with "Yes" badge
                        this.updateSIGeneratedCell(this.selectedRow, true);
                        break;
                    case 'open':
                        if (result.entityUrl) {
                            window.open(result.entityUrl, '_blank');
                        } else {
                            notyf.error(`URL for ${this.entity} not found.`);
                        }
                        break;
                    case 'delete':
                        notyf.success(`${this.entity} deleted successfully!`);
                        this.selectedRow.classList.remove('bg-soft-primary');
                        // Update the "SI Generated" column with "No" badge
                        this.updateSIGeneratedCell(this.selectedRow, false);
                        break;
                    default:
                        notyf.success(`${this.entity} action '${menuItem.label}' completed successfully!`);
                }
            } else {
                notyf.error(result.message || `Failed to ${menuItem.label}.`);
            }
        } catch (error) {
            console.error(`Error performing action '${menuItem.action}' on ${this.entity}:`, error);
            notyf.error(`An error occurred while performing '${menuItem.label}'.`);
        }
    }

    updateSIGeneratedCell(row, isSIExist) {
        const sigeneratedCell = row.querySelector('td:nth-child(7)');
        if (!sigeneratedCell) {
            console.error('SI Generated cell not found.');
            return;
        }

        if (isSIExist) {
            sigeneratedCell.innerHTML = `
                <span class="badge bg-soft-success text-success">
                    <span class="legend-indicator bg-success"></span> Yes
                </span>
            `;
        } else {
            sigeneratedCell.innerHTML = `
                <span class="badge bg-soft-danger text-danger">
                    <span class="legend-indicator bg-danger"></span> No
                </span>
            `;
        }
    }

    getIdKey() {
        // Convert data attribute to camelCase key
        // e.g., 'so-id' => 'soId', 'po-id' => 'poId'
        return this.dataIdAttribute.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
    }

    getQueryParam() {
        // Return the camelCase key for query parameters
        return this.getIdKey();
    }

    showMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'flex';
            menuItem.style.visibility = 'visible';
        }
    }

    hideMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'none';
            menuItem.style.visibility = 'hidden';
        }
    }

    hideAllMenuItems() {
        this.menuItems.forEach(item => {
            this.hideMenuItem(item.id);
        });
    }

    showContextMenu(x, y) {
        // Set aria-hidden to false
        this.contextMenu.setAttribute('aria-hidden', 'false');

        // Temporarily display the menu to calculate its size
        this.contextMenu.style.display = 'block';
        this.contextMenu.style.opacity = '0';
        this.contextMenu.style.transform = 'scale(0.95)';

        // Calculate position to prevent overflow
        const menuWidth = this.contextMenu.offsetWidth;
        const menuHeight = this.contextMenu.offsetHeight;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let finalX = x;
        let finalY = y;

        if (x + menuWidth > viewportWidth) {
            finalX = viewportWidth - menuWidth - 10; // 10px padding
        }

        if (y + menuHeight > viewportHeight) {
            finalY = viewportHeight - menuHeight - 10; // 10px padding
        }

        // Apply calculated position and show the menu
        this.contextMenu.style.top = `${finalY}px`;
        this.contextMenu.style.left = `${finalX}px`;
        this.contextMenu.classList.remove('hidden');
        this.contextMenu.classList.add('visible');

        // Reset temporary styles
        this.contextMenu.style.display = '';

        // Remove focus from any menu items to prevent default focus styling
        const menuItems = this.contextMenu.querySelectorAll('li');
        menuItems.forEach(item => {
            item.setAttribute('tabindex', '-1');
        });
    }

    hideContextMenu() {
        if (this.contextMenu) {
            // Set aria-hidden to true
            this.contextMenu.setAttribute('aria-hidden', 'true');
            this.contextMenu.classList.remove('visible');
            this.contextMenu.classList.add('hidden');

            // Remove focus from any menu items
            const focusedElement = document.activeElement;
            if (this.contextMenu.contains(focusedElement)) {
                focusedElement.blur();
            }
        }
    }

    getRowData(row) {
        const data = {};

        Array.from(row.attributes).forEach(attr => {
            if (attr.name.startsWith('data-')) {
                // Remove 'data-' prefix and hyphens, then convert to camelCase
                const key = attr.name.replace('data-', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
                data[key] = attr.value;
            }
        });

        console.log(`Row Data for ${this.entity}:`, data); // For debugging purposes
        return data;
    }

    // Function to get anti-forgery token
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }
}
*/





//============================== V-3 ================================
/*
export class ContextMenuTable {
    constructor(options) {
        this.tables = document.querySelectorAll(options.tableSelector);
        this.menuItems = options.menuItems; // Array of menu item configurations
        this.contextMenu = null;
        this.selectedRow = null;
        this.selectedData = null; // Generic data object from the selected row
        this.prepareMenuCallback = options.prepareMenu || null; // Optional prepareMenu function
        this.checkExistsUrl = options.checkSIExistsUrl || '/SI/CheckSIExists'; // URL to check SI existence
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.init();
    }

    init() {
        this.injectStyles();
        this.createContextMenu();
        this.attachEvents();
    }

    injectStyles() {
        const style = document.createElement('style');
        style.innerHTML = `
            .context-menu {
                display: none;
                position: fixed;
                z-index: 1050;
                width: 220px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                cursor: default;
                padding: 0;
                margin: 0;
                opacity: 0;
                transition: opacity 0.3s ease, transform 0.3s ease;
                transform: scale(0.95);
                background-color: #ffffff;
                border: 1px solid #ccc;
                border-radius: 4px;
            }

            .context-menu.visible {
                display: block;
                opacity: 1;
                transform: scale(1);
                animation: fadeInUp 0.3s forwards;
            }

            .context-menu.hidden {
                opacity: 0;
                transform: scale(0.95);
                animation: fadeOutDown 0.3s forwards;
            }

            .context-menu ul {
                list-style: none;
                padding: 0;
                margin: 0;
            }

            .context-menu ul li {
                padding: 12px 20px;
                cursor: pointer;
                transition: background-color 0.2s;
                display: flex;
                align-items: center;
                color: #000000; 
            }

            .context-menu ul li:hover {
                background-color: #efefef;
            }

            .context-menu ul li:focus {
                outline: none; 
                background-color: #efefef;
            }

            .highlight-row {
                background-color: #d4edda !important;
                transition: background-color 0.3s ease;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
                to {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
            }

            @keyframes fadeOutDown {
                from {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
                to {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
            }
        `;
        document.head.appendChild(style);
    }

    createContextMenu() {
        // Create the context menu element
        this.contextMenu = document.createElement('div');
        this.contextMenu.classList.add('context-menu', 'hidden');
        this.contextMenu.setAttribute('role', 'menu');
        this.contextMenu.setAttribute('aria-hidden', 'true');

        // Proper HTML structure: Only <ul> and <li> elements
        this.contextMenu.innerHTML = `
            <ul>
                ${this.menuItems.map(item => `
                    <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}">
                        ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
                    </li>
                `).join('')}
            </ul>
        `;
        document.body.appendChild(this.contextMenu);

        // Attach event listeners for menu items with correct binding
        this.menuItems.forEach(item => {
            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
            if (menuItem) {
                // Bind the onClick handler to the ContextMenuTable instance
                menuItem.addEventListener('click', (e) => {
                    e.stopPropagation();
                    item.onClick.call(this, this.selectedRow, this.selectedData);
                    this.hideContextMenu();
                });

                // Keyboard accessibility for Enter key with correct binding
                menuItem.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        item.onClick.call(this, this.selectedRow, this.selectedData);
                        this.hideContextMenu();
                    }
                });
            }
        });

        // Hide context menu on window scroll or resize with debouncing
        const debounce = (func, wait) => {
            let timeout;
            return function (...args) {
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(this, args), wait);
            };
        };

        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
    }

    attachEvents() {
        this.tables.forEach(table => {
            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
        });

        // Hide context menu on clicking elsewhere
        document.addEventListener('click', () => this.hideContextMenu());

        // Hide context menu on pressing the Escape key and handle keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.hideContextMenu();
            }
            // Handle keyboard navigation within the context menu
            if (this.contextMenu.classList.contains('visible')) {
                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
                const focusedElement = document.activeElement;
                const currentIndex = menuItems.indexOf(focusedElement);

                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = (currentIndex + 1) % menuItems.length;
                    menuItems[nextIndex].focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
                    menuItems[prevIndex].focus();
                }
            }
        });
    }

    async handleContextMenu(e, table) {
        e.preventDefault();
        const target = e.target.closest('tr');
        if (target && target.rowIndex > 0) { // Ensure it's not the header
            this.selectedRow = target;
            this.selectedData = this.getRowData(target);
            this.lastMouseX = e.clientX;
            this.lastMouseY = e.clientY;

            // Fetch SI existence and prepare the menu
            await this.prepareMenu();
            this.showContextMenu(this.lastMouseX, this.lastMouseY);
        } else {
            this.hideContextMenu();
        }
    }

    async prepareMenu() {
        if (!this.selectedData || !this.selectedData.soid) {
            console.error('SO ID is missing in the selected row data.');
            notyf.error('Selected row data is incomplete.');
            this.hideAllMenuItems();
            return;
        }

        try {
            const response = await fetch(`${this.checkExistsUrl}?soId=${this.selectedData.soid}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const result = await response.json();
            if (result.success) {
                this.selectedData.siexists = result.siexists;

                if (this.selectedData.siexists === true) {
                    // Invoice exists: Show "Goto Invoice" and "Delete Invoice"
                    this.showMenuItem('openSI');
                    this.showMenuItem('deleteSI');
                    // Hide "Generate Invoice"
                    this.hideMenuItem('createSI');
                } else {
                    // Invoice does not exist: Show "Generate Invoice"
                    this.showMenuItem('createSI');
                    // Hide "Goto Invoice" and "Delete Invoice"
                    this.hideMenuItem('openSI');
                    this.hideMenuItem('deleteSI');
                }
            } else {
                console.error('Failed to check SI existence:', result.message);
                notyf.error(result.message || 'Failed to check SI existence.');

                // Fallback: Hide all menu items if check fails
                this.hideAllMenuItems();
            }
        } catch (error) {
            console.error('Error checking SI existence:', error);
            notyf.error('An error occurred while checking SI existence.');

            // Fallback: Hide all menu items if fetch fails
            this.hideAllMenuItems();
        }
    }

    showMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'flex';
            menuItem.style.visibility = 'visible';
        }
    }

    hideMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'none';
            menuItem.style.visibility = 'hidden';
        }
    }

    hideAllMenuItems() {
        this.menuItems.forEach(item => {
            this.hideMenuItem(item.id);
        });
    }

    showContextMenu(x, y) {
        // Set aria-hidden to false
        this.contextMenu.setAttribute('aria-hidden', 'false');

        // Temporarily display the menu to calculate its size
        this.contextMenu.style.display = 'block';
        this.contextMenu.style.opacity = '0';
        this.contextMenu.style.transform = 'scale(0.95)';

        // Calculate position to prevent overflow
        const menuWidth = this.contextMenu.offsetWidth;
        const menuHeight = this.contextMenu.offsetHeight;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let finalX = x;
        let finalY = y;

        if (x + menuWidth > viewportWidth) {
            finalX = viewportWidth - menuWidth - 10; // 10px padding
        }

        if (y + menuHeight > viewportHeight) {
            finalY = viewportHeight - menuHeight - 10; // 10px padding
        }

        // Apply calculated position and show the menu
        this.contextMenu.style.top = `${finalY}px`;
        this.contextMenu.style.left = `${finalX}px`;
        this.contextMenu.classList.remove('hidden');
        this.contextMenu.classList.add('visible');

        // Reset temporary styles
        this.contextMenu.style.display = '';

        // Remove focus from any menu items to prevent default focus styling
        const menuItems = this.contextMenu.querySelectorAll('li');
        menuItems.forEach(item => {
            item.setAttribute('tabindex', '-1');
        });
    }

    hideContextMenu() {
        if (this.contextMenu) {
            // Set aria-hidden to true
            this.contextMenu.setAttribute('aria-hidden', 'true');
            this.contextMenu.classList.remove('visible');
            this.contextMenu.classList.add('hidden');

            // Remove focus from any menu items
            const focusedElement = document.activeElement;
            if (this.contextMenu.contains(focusedElement)) {
                focusedElement.blur();
            }
        }
    }

    getRowData(row) {
        const data = {};

        Array.from(row.attributes).forEach(attr => {
            if (attr.name.startsWith('data-')) {
                // Remove 'data-' prefix and hyphens, then convert to lowercase
                const key = attr.name.replace('data-', '').replace(/-/g, '').toLowerCase();
                data[key] = attr.value;
            }
        });

        console.log('Row Data:', data); // For debugging purposes
        return data;
    }

    // Function to get anti-forgery token
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }
}
*/



//============================== V-2 ================================
/*
export class ContextMenuTable {
    constructor(options) {
        this.tables = document.querySelectorAll(options.tableSelector);
        this.menuItems = options.menuItems; // Array of menu item configurations
        this.contextMenu = null;
        this.selectedRow = null;
        this.selectedData = null; // Generic data object from the selected row
        this.prepareMenuCallback = options.prepareMenu || null; // Optional prepareMenu function
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.init();
    }

    init() {
        this.injectStyles();
        this.createContextMenu();
        this.attachEvents();
    }

    injectStyles() {
        const style = document.createElement('style');
        style.innerHTML = `
            .context-menu {
                display: none;
                position: fixed;
                z-index: 1050;
                width: 220px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                cursor: default;
                padding: 0;
                margin: 0;
                opacity: 0;
                transition: opacity 0.3s ease, transform 0.3s ease;
                transform: scale(0.95);
            }

            .context-menu.visible {
                display: block;
                opacity: 1;
                transform: scale(1);
                animation: fadeInUp 0.3s forwards;
            }

            .context-menu.hidden {
                opacity: 0;
                transform: scale(0.95);
                animation: fadeOutDown 0.3s forwards;
            }

            .context-menu ul {
                list-style: none;
                padding: 0;
                margin: 0;
            }

            .context-menu ul li {
                cursor: pointer;
            }

           
            .context-menu ul li:focus {
                outline: none; 
            }

            .highlight-row {
                background-color: #d4edda !important;
                transition: background-color 0.3s ease;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
                to {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
            }

            @keyframes fadeOutDown {
                from {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
                to {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
            }
        `;
        document.head.appendChild(style);
    }

    createContextMenu() {
        // Create the context menu element
        this.contextMenu = document.createElement('div');
        this.contextMenu.classList.add('context-menu', 'hidden');
        this.contextMenu.setAttribute('role', 'menu');
        this.contextMenu.setAttribute('aria-hidden', 'true');

        // Proper HTML structure: Only <ul> and <li> elements
        this.contextMenu.innerHTML = `
            <ul>
                <div class="dropdown-menu dropdown-menu-end mt-1 show">
                ${this.menuItems.map(item => `
                    <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}">
                        ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
                    </li>
                `).join('')}
                </div>
            </ul>
        `;
        document.body.appendChild(this.contextMenu);

        // Attach event listeners for menu items with correct binding
        this.menuItems.forEach(item => {
            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
            if (menuItem) {
                // Bind the onClick handler to the ContextMenuTable instance
                menuItem.addEventListener('click', (e) => {
                    e.stopPropagation();
                    item.onClick.call(this, this.selectedRow, this.selectedData);
                    this.hideContextMenu();
                });

                // Keyboard accessibility for Enter key with correct binding
                menuItem.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        item.onClick.call(this, this.selectedRow, this.selectedData);
                        this.hideContextMenu();
                    }
                });
            }
        });

        // Hide context menu on window scroll or resize with debouncing
        const debounce = (func, wait) => {
            let timeout;
            return function (...args) {
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(this, args), wait);
            };
        };

        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
    }

    attachEvents() {
        this.tables.forEach(table => {
            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
        });

        // Hide context menu on clicking elsewhere
        document.addEventListener('click', () => this.hideContextMenu());

        // Hide context menu on pressing the Escape key and handle keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.hideContextMenu();
            }
            // Handle keyboard navigation within the context menu
            if (this.contextMenu.classList.contains('visible')) {
                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
                const focusedElement = document.activeElement;
                const currentIndex = menuItems.indexOf(focusedElement);

                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = (currentIndex + 1) % menuItems.length;
                    menuItems[nextIndex].focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
                    menuItems[prevIndex].focus();
                }
            }
        });
    }

    handleContextMenu(e, table) {
        e.preventDefault();
        const target = e.target.closest('tr');
        if (target && target.rowIndex > 0) { // Ensure it's not the header
            this.selectedRow = target;
            this.selectedData = this.getRowData(target);
            this.lastMouseX = e.clientX;
            this.lastMouseY = e.clientY;

            // Prepare the menu (e.g., show/hide items based on data)
            if (typeof this.prepareMenuCallback === 'function') {
                this.prepareMenuCallback(this.selectedData, this);
            } else {
                this.prepareMenu().then(() => {
                    // Show the context menu at the cursor's position
                    this.showContextMenu(this.lastMouseX, this.lastMouseY);
                }).catch(err => {
                    console.error(err);
                    this.showContextMenu(this.lastMouseX, this.lastMouseY);
                });
            }
        } else {
            this.hideContextMenu();
        }
    }

    async prepareMenu() {
        // Default prepareMenu does nothing
    }

    showMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'flex';
            menuItem.style.visibility = 'visible';
        }
    }

    hideMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'none';
            menuItem.style.visibility = 'hidden';
        }
    }

    showContextMenu(x, y) {
        // Set aria-hidden to false
        this.contextMenu.setAttribute('aria-hidden', 'false');

        // Temporarily display the menu to calculate its size
        this.contextMenu.style.display = 'block';
        this.contextMenu.style.opacity = '0';
        this.contextMenu.style.transform = 'scale(0.95)';

        // Calculate position to prevent overflow
        const menuWidth = this.contextMenu.offsetWidth;
        const menuHeight = this.contextMenu.offsetHeight;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let finalX = x;
        let finalY = y;

        if (x + menuWidth > viewportWidth) {
            finalX = viewportWidth - menuWidth - 10; // 10px padding
        }

        if (y + menuHeight > viewportHeight) {
            finalY = viewportHeight - menuHeight - 10; // 10px padding
        }

        // Apply calculated position and show the menu
        this.contextMenu.style.top = `${finalY}px`;
        this.contextMenu.style.left = `${finalX}px`;
        this.contextMenu.classList.remove('hidden');
        this.contextMenu.classList.add('visible');

        // Reset temporary styles
        this.contextMenu.style.display = '';

        // Remove focus from any menu items to prevent default focus styling
        const menuItems = this.contextMenu.querySelectorAll('li');
        menuItems.forEach(item => {
            item.setAttribute('tabindex', '-1');
        });
    }

    hideContextMenu() {
        if (this.contextMenu) {
            // Set aria-hidden to true
            this.contextMenu.setAttribute('aria-hidden', 'true');
            this.contextMenu.classList.remove('visible');
            this.contextMenu.classList.add('hidden');

            // Remove focus from any menu items
            const focusedElement = document.activeElement;
            if (this.contextMenu.contains(focusedElement)) {
                focusedElement.blur();
            }
        }
    }

    getRowData(row) {
        const data = {};

        Array.from(row.attributes).forEach(attr => {
            if (attr.name.startsWith('data-')) {
                // Remove 'data-' prefix and hyphens, then convert to lowercase
                const key = attr.name.replace('data-', '').replace(/-/g, '').toLowerCase();
                data[key] = attr.value;
            }
        });

        console.log(data); // For debugging purposes
        return data;
    }

    // Function to get anti-forgery token
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }
}
*/

//============================== V-1 ====================================
/*
export class ContextMenuTable {
    constructor(options) {
        this.tables = document.querySelectorAll(options.tableSelector);
        this.menuItems = options.menuItems; // Array of menu item configurations
        this.contextMenu = null;
        this.selectedRow = null;
        this.selectedData = null; // Generic data object from the selected row
        this.prepareMenuCallback = options.prepareMenu || null; // Optional prepareMenu function
        this.lastMouseX = 0;
        this.lastMouseY = 0;

        this.init();
    }

    init() {
        this.injectStyles();
        this.createContextMenu();
        this.attachEvents();
    }

    injectStyles() {
        const style = document.createElement('style');
        style.innerHTML = `
            .context-menu {
                display: none;
                position: fixed;
                z-index: 1050;
                width: 220px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                cursor: default;
                padding: 0;
                margin: 0;
                opacity: 0;
                transition: opacity 0.3s ease, transform 0.3s ease;
                transform: scale(0.95);
            }

            .context-menu.visible {
                display: block;
                opacity: 1;
                transform: scale(1);
                animation: fadeInUp 0.3s forwards;
            }

            .context-menu.hidden {
                opacity: 0;
                transform: scale(0.95);
                animation: fadeOutDown 0.3s forwards;
            }

            .context-menu ul {
                list-style: none;
                padding: 0;
                margin: 0;
            }

            .context-menu ul li {
                cursor: pointer;
                transition: background-color 0.2s;
                display: flex;
                align-items: center;
                color: #000000; 
            }

            .context-menu ul li:hover {
                background-color: #efefef;
            }

            .context-menu ul li:focus {
                outline: none; 
                background-color: #efefef;
            }

            .highlight-row {
                background-color: #d4edda !important;
                transition: background-color 0.3s ease;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
                to {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
            }

            @keyframes fadeOutDown {
                from {
                    opacity: 1;
                    transform: translateY(0) scale(1);
                }
                to {
                    opacity: 0;
                    transform: translateY(10px) scale(0.95);
                }
            }
        `;
        document.head.appendChild(style);
    }

    createContextMenu() {
        // Create the context menu element
        this.contextMenu = document.createElement('div');
        this.contextMenu.classList.add('context-menu', 'hidden');
        this.contextMenu.setAttribute('role', 'menu');
        this.contextMenu.setAttribute('aria-hidden', 'true');


        this.contextMenu.innerHTML = `
            <ul>
                <div class="dropdown-menu dropdown-menu-end mt-1 show">
                ${this.menuItems.map(item => `
                    <li id="${item.id}" class="dropdown-item" role="menuitem" tabindex="-1" aria-label="${item.label}">
                        ${item.iconSVG ? item.iconSVG : `<i class="${item.iconClass} dropdown-item-icon"></i>`} ${item.label}
                    </li>
                `).join('')}
                </div>
            </ul>
        `;
        document.body.appendChild(this.contextMenu);

        // Attach event listeners for menu items with correct binding
        this.menuItems.forEach(item => {
            const menuItem = this.contextMenu.querySelector(`#${item.id}`);
            if (menuItem) {
                // Bind the onClick handler to the ContextMenuTable instance
                menuItem.addEventListener('click', (e) => {
                    e.stopPropagation();
                    item.onClick.call(this, this.selectedRow, this.selectedData);
                    this.hideContextMenu();
                });

                // Keyboard accessibility for Enter key with correct binding
                menuItem.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        item.onClick.call(this, this.selectedRow, this.selectedData);
                        this.hideContextMenu();
                    }
                });
            }
        });

        // Hide context menu on window scroll or resize with debouncing
        const debounce = (func, wait) => {
            let timeout;
            return function (...args) {
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(this, args), wait);
            };
        };

        window.addEventListener('scroll', debounce(() => this.hideContextMenu(), 200));
        window.addEventListener('resize', debounce(() => this.hideContextMenu(), 200));
    }

    attachEvents() {
        this.tables.forEach(table => {
            table.addEventListener('contextmenu', (e) => this.handleContextMenu(e, table));
        });

        // Hide context menu on clicking elsewhere
        document.addEventListener('click', () => this.hideContextMenu());

        // Hide context menu on pressing the Escape key and handle keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.hideContextMenu();
            }
            // Handle keyboard navigation within the context menu
            if (this.contextMenu.classList.contains('visible')) {
                const menuItems = Array.from(this.contextMenu.querySelectorAll('li'));
                const focusedElement = document.activeElement;
                const currentIndex = menuItems.indexOf(focusedElement);

                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = (currentIndex + 1) % menuItems.length;
                    menuItems[nextIndex].focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = (currentIndex - 1 + menuItems.length) % menuItems.length;
                    menuItems[prevIndex].focus();
                }
            }
        });
    }

    handleContextMenu(e, table) {
        e.preventDefault();
        const target = e.target.closest('tr');
        if (target && target.rowIndex > 0) { // Ensure it's not the header
            this.selectedRow = target;
            this.selectedData = this.getRowData(target);
            this.lastMouseX = e.clientX;
            this.lastMouseY = e.clientY;

            // Prepare the menu (e.g., show/hide items based on data)
            if (typeof this.prepareMenuCallback === 'function') {
                this.prepareMenuCallback(this.selectedData, this);
            } else {
                this.prepareMenu().then(() => {
                    // Show the context menu at the cursor's position
                    this.showContextMenu(this.lastMouseX, this.lastMouseY);
                }).catch(err => {
                    console.error(err);
                    this.showContextMenu(this.lastMouseX, this.lastMouseY);
                });
            }
        } else {
            this.hideContextMenu();
        }
    }

    async prepareMenu() {
        // Default prepareMenu does nothing
    }

    showMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'flex';
            menuItem.style.visibility = 'visible';
        }
    }

    hideMenuItem(id) {
        const menuItem = this.contextMenu.querySelector(`#${id}`);
        if (menuItem) {
            menuItem.style.display = 'none';
            menuItem.style.visibility = 'hidden';
        }
    }

    showContextMenu(x, y) {
        // Set aria-hidden to false
        this.contextMenu.setAttribute('aria-hidden', 'false');

        // Calculate position to prevent overflow
        const menuWidth = this.contextMenu.offsetWidth;
        const menuHeight = this.contextMenu.offsetHeight;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let finalX = x;
        let finalY = y;

        if (x + menuWidth > viewportWidth) {
            finalX = viewportWidth - menuWidth - 10; // 10px padding
        }

        if (y + menuHeight > viewportHeight) {
            finalY = viewportHeight - menuHeight - 10; // 10px padding
        }

        // Apply calculated position and show the menu
        this.contextMenu.style.top = `${finalY}px`;
        this.contextMenu.style.left = `${finalX}px`;
        this.contextMenu.classList.remove('hidden');
        this.contextMenu.classList.add('visible');

        // Remove focus from any menu items to prevent default focus styling
        const menuItems = this.contextMenu.querySelectorAll('li');
        menuItems.forEach(item => {
            item.setAttribute('tabindex', '-1');
        });
    }

    hideContextMenu() {
        if (this.contextMenu) {
            // Set aria-hidden to true
            this.contextMenu.setAttribute('aria-hidden', 'true');
            this.contextMenu.classList.remove('visible');
            this.contextMenu.classList.add('hidden');

            // Remove focus from any menu items
            const focusedElement = document.activeElement;
            if (this.contextMenu.contains(focusedElement)) {
                focusedElement.blur();
            }
        }
    }

    getRowData(row) {
        // Extract data attributes from the row
        debugger;
        const data = {};

        // Example: If you have data attributes like data-so-id, data-po-id, etc.
        Array.from(row.attributes).forEach(attr => {
            if (attr.name.startsWith('data-')) {
                const key = attr.name.replace('data-', '');
                data[key] = attr.value;
            }
        });
        console.log(data)
        return data;
    }

    // Function to get anti-forgery token
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }
}

*/