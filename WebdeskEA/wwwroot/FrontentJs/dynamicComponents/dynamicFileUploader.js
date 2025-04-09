//<script type="module">
//    import {ProBaseUploader} from './proBaseUploader.js';

//    // Edit scenario: single image
//    const uploader = new ProBaseUploader({
//        inputId: 'profileImageInput',
//    isImage: true,
//    isMultiImage: false,
//    actionType: 'edit',
//    editImage: '/images/cat1.jpg' // existing single image path
//    });

//    // Another scenario: multiple files, read from JSON array
//    const multiFileUploader = new ProBaseUploader({
//        inputId: 'documentsInput',
//    isFiles: false,       // Not single-file mode
//    isMultiFile: true,    // multiple
//    actionType: 'edit',
//    editFileList: [
//    {id: 101, fileName: 'Resume.pdf', downloadUrl: '/docs/resume.pdf' },
//    {id: 102, fileName: 'ProjectPlan.txt', downloadUrl: '/docs/plan.txt' }
//    ]
//    });
//</script>


// proBaseUploader.js
export class ProBaseUploader {
    /**
     * @param {Object} options
     * @param {string} options.inputId - The ID of the file input element in the DOM.
     *
     * Upload Mode Flags:
     * @param {boolean} [options.isImage=false] - Single image mode (accept images).
     * @param {boolean} [options.isMultiImage=false] - Multi-image mode (accept multiple images).
     * @param {boolean} [options.isFiles=false] - Single file (non-image) mode.
     * @param {boolean} [options.isMultiFile=false] - Multi-file (non-image) mode.
     *
     * Accept Filtering:
     * @param {string} [options.imageTypes='all'] - Comma-separated image extensions, e.g. "png,jpg,gif".
     *                                              "all" => "image/*"
     * @param {string} [options.fileTypes='all']  - Comma-separated file extensions, e.g. "pdf,txt,docx".
     *                                              "all" => 
     *
     * Action Type & Existing Items:
     * @param { string } [options.actionType = 'create'] - e.g. "create", "edit", "details", "delete", etc.
     * @param { string } [options.editImage] - If single image and editing, path / URL to the existing image.
     * @param { Array < string >} [options.editImageList] - If multiple images, array of existing image URLs.
     * @param { Object | string } [options.editFile] - For single file.E.g. { id: 12, fileName: "doc.pdf", downloadUrl: "/doc.pdf" }
     * @param { Array < Object >} [options.editFileList] - For multiple files.E.g. [{ id: 1, fileName: "abc.pdf", downloadUrl: "/abc.pdf" }, ... ]
    
     */
    constructor({
        inputId,
        labelName = 'No files selected',
        linkName = 'Click or Drag & Drop to select files',
        isImage = false,
        isMultiImage = false,
        isFiles = false,
        isMultiFile = false,
        imageTypes = 'all',
        fileTypes = 'all',
        actionType = 'create',
        editImage = null,
        editImageList = null,
        editFile = null,
        editFileList = null
    }) {
        // 1) Basic references
        this.input = document.getElementById(inputId);
        if (!this.input) {
            console.error('ProBaseUploader: No element found with ID "' + inputId + '".');
            return;
        }

        this.isImage = isImage;
        this.isMultiImage = isMultiImage;
        this.isFiles = isFiles;
        this.isMultiFile = isMultiFile;
        this.imageTypes = imageTypes;
        this.fileTypes = fileTypes;

        // File Name
        this.labelName = labelName;
        this.linkName = linkName;

        // Action + existing items
        this.actionType = actionType;
        this.editImage = editImage;         // single
        this.editImageList = editImageList; // array
        this.editFile = editFile;           // single
        this.editFileList = editFileList;   // array

        // We'll keep track of "existing" items that user might remove
        this.removedExistingItems = [];

        // 2) Set up <input> attributes:
        this.configureInputAttributes();

        // 3) Inject styles
        this.injectStyles();

        // 4) Build container & sub-elements
        this.wrapper = this.createWrapper();
        this.statusBar = this.createStatusBar();
        this.triggerButton = this.createTriggerButton();
        this.progressBarContainer = this.createProgressBarContainer();
        this.previewContainer = this.createPreviewContainer();

        // Insert wrapper before <input> and then append everything
        this.input.parentNode.insertBefore(this.wrapper, this.input);
        this.wrapper.appendChild(this.input);
        this.wrapper.appendChild(this.statusBar);
        this.wrapper.appendChild(this.triggerButton);
        this.wrapper.appendChild(this.progressBarContainer);
        this.wrapper.appendChild(this.previewContainer);

        // If multi-file scenario, create "Remove All" container
        if (this.isMultiFile || this.isMultiImage) {
            this.removeAllContainer = this.createRemoveAllContainer();
            this.wrapper.insertBefore(this.removeAllContainer, this.previewContainer);
            this.removeAllContainer.style.display = 'none'; // hidden by default
        }

        // 5) Setup click & drag
        this.setupWrapperClick();
        this.setupDragAndDrop();

        // 6) Listen for user-chosen files
        this.input.addEventListener('change', () => this.handleFileChange());

        // 7) If we are in "edit" mode and have existing items, we show them
        if (this.actionType === 'edit' || this.actionType === 'details' || this.actionType === 'delete') {
            this.renderExistingItems();
        }
    }

    //-----------------------------------------------
    // A) Configure <input> accept/multiple attributes
    //-----------------------------------------------
    configureInputAttributes() {
        // Single vs multiple
        if (this.isMultiImage || this.isMultiFile) {
            this.input.setAttribute('multiple', 'multiple');
        } else {
            this.input.removeAttribute('multiple');
        }

        // If user specifically wants images
        if (this.isImage || this.isMultiImage) {
            if (this.imageTypes.toLowerCase() === 'all') {
                // user wants "image/*"
                this.input.setAttribute('accept', 'image/*');
            } else {
                // parse comma list => generate MIME list
                const exts = this.imageTypes.split(',').map(s => s.trim().toLowerCase());
                const acceptList = exts.map(ext => this.mapImageExtensionToMime(ext));
                this.input.setAttribute('accept', acceptList.join(','));
            }
        }
        // If user specifically wants files
        else if (this.isFiles || this.isMultiFile) {
            if (this.fileTypes.toLowerCase() === 'all') {
                // user wants "*/*"
                this.input.setAttribute('accept', '*/*');
            } else {
                const exts = this.fileTypes.split(',').map(s => s.trim().toLowerCase());
                const acceptList = exts.map(ext => {
                    if (ext.startsWith('.')) return ext;
                    return '.' + ext;
                });
                this.input.setAttribute('accept', acceptList.join(','));
            }
        }
        // else, remove accept entirely
        else {
            this.input.removeAttribute('accept');
        }
    }

    mapImageExtensionToMime(ext) {
        switch (ext) {
            case 'jpg':
            case 'jpeg':
                return 'image/jpeg';
            case 'png':
                return 'image/png';
            case 'gif':
                return 'image/gif';
            case 'webp':
                return 'image/webp';
            case 'svg':
                return 'image/svg+xml';
            default:
                return 'image/' + ext; // fallback
        }
    }

    //-----------------------------------------------
    // B) Insert inline styles
    //-----------------------------------------------
    injectStyles() {
        const styleTag = document.createElement('style');
        styleTag.innerHTML = `
            .proBaseUploader-wrapper {
                border: 2px dashed #ccc;
                border-radius: 6px;
                padding: 1.5rem;
                text-align: center;
                position: relative;
                margin-bottom: 1rem;
                transition: background-color 0.2s ease-in-out;
                cursor: pointer;
            }
            .proBaseUploader-wrapper:hover {
                background-color: #fafafa;
            }
            .proBaseUploader-input {
                display: none;
            }
            .proBaseUploader-statusBar {
                margin-bottom: 0.5rem;
                font-weight: 600;
                font-size: 0.9rem;
                color: #444;
            }
            .proBaseUploader-triggerBtn {
                display: inline-block;
                font-size: 0.95rem;
                margin-bottom: 1rem;
                color: #007bff;
                text-decoration: underline;
                cursor: pointer;
            }
            .proBaseUploader-triggerBtn:hover {
                text-decoration: none;
            }
            .proBaseUploader-removeAllContainer {
                margin-bottom: 1rem;
            }
            .proBaseUploader-removeAllLink {
                font-size: 0.85rem;
                color: #e74c3c;
                text-decoration: underline;
                cursor: pointer;
            }
            .proBaseUploader-removeAllLink:hover {
                text-decoration: none;
            }
            .proBaseUploader-preview {
                display: flex;
                flex-wrap: wrap;
                gap: 10px;
                justify-content: center;
                margin-top: 0.5rem;
            }
            .proBaseUploader-preview-item {
                position: relative;
                width: 80px;
                height: 80px;
                border: 1px solid #ddd;
                border-radius: 4px;
                overflow: hidden;
                display: flex;
                align-items: center;
                justify-content: center;
            }
            .proBaseUploader-preview-item img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }
            .proBaseUploader-fileName {
                font-size: 0.75rem;
                text-align: center;
                padding: 4px;
            }
            .proBaseUploader-removeIcon {
                position: absolute;
                top: 4px;
                right: 4px;
                width: 20px;
                height: 20px;
                line-height: 20px;
                text-align: center;
                border-radius: 50%;
                background-color: rgba(255,255,255,0.7);
                color: #333;
                cursor: pointer;
                font-weight: bold;
            }
            .proBaseUploader-removeIcon:hover {
                background-color: #fff;
            }
            .proBaseUploader-dragOver {
                background-color: #eef;
                border-color: #4da3ff;
            }
            .proBaseUploader-progressBarContainer {
                width: 100%;
                background-color: #f3f3f3;
                border-radius: 4px;
                overflow: hidden;
                height: 8px;
                margin: 0 auto 1rem auto;
                display: none;
            }
            .proBaseUploader-progressBar {
                height: 100%;
                width: 0%;
                background-color: #4caf50;
                transition: width 0.1s linear;
            }
        `;
        document.head.appendChild(styleTag);
    }

    //-----------------------------------------------
    // C) Create DOM elements
    //-----------------------------------------------
    createWrapper() {
        const wrapper = document.createElement('div');
        wrapper.classList.add('proBaseUploader-wrapper');
        this.input.classList.add('proBaseUploader-input');
        return wrapper;
    }

    createStatusBar() {
        const status = document.createElement('div');
        status.classList.add('proBaseUploader-statusBar');
        status.textContent = this.labelName;//'No files selected';
        return status;
    }

    createTriggerButton() {
        const btn = document.createElement('div');
        btn.className = 'proBaseUploader-triggerBtn';
        btn.textContent = this.linkName;//'Click or Drag & Drop to select files';
        return btn;
    }

    createProgressBarContainer() {
        const container = document.createElement('div');
        container.classList.add('proBaseUploader-progressBarContainer');
        this.progressBar = document.createElement('div');
        this.progressBar.classList.add('proBaseUploader-progressBar');
        container.appendChild(this.progressBar);
        return container;
    }

    createPreviewContainer() {
        const preview = document.createElement('div');
        preview.classList.add('proBaseUploader-preview');
        return preview;
    }

    createRemoveAllContainer() {
        const container = document.createElement('div');
        container.classList.add('proBaseUploader-removeAllContainer');

        this.removeAllLink = document.createElement('span');
        this.removeAllLink.classList.add('proBaseUploader-removeAllLink');
        this.removeAllLink.textContent = 'Remove All Files';

        this.removeAllLink.addEventListener('click', (e) => {
            e.stopPropagation();
            this.removeAllFiles();
        });

        container.appendChild(this.removeAllLink);
        return container;
    }

    //-----------------------------------------------
    // D) Setup click / drag behavior
    //-----------------------------------------------
    setupWrapperClick() {
        this.wrapper.addEventListener('click', (e) => {
            // Don't trigger if user clicked remove icon or "Remove All" link
            if (
                e.target.classList.contains('proBaseUploader-removeIcon') ||
                e.target.classList.contains('proBaseUploader-removeAllLink')
            ) {
                return;
            }
            // If user clicked on the wrapper or the text, open file dialog
            if (e.target === this.wrapper || e.target === this.triggerButton) {
                this.input.click();
            }
        });
    }

    setupDragAndDrop() {
        this.wrapper.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.wrapper.classList.add('proBaseUploader-dragOver');
        });

        this.wrapper.addEventListener('dragleave', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.wrapper.classList.remove('proBaseUploader-dragOver');
        });

        this.wrapper.addEventListener('drop', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.wrapper.classList.remove('proBaseUploader-dragOver');

            const dt = e.dataTransfer;
            if (!dt || !dt.files || !dt.files.length) return;

            // For single-file input, only take the first
            if (!this.isMultiFile && !this.isMultiImage) {
                const singleFileList = new DataTransfer();
                singleFileList.items.add(dt.files[0]);
                this.input.files = singleFileList.files;
            } else {
                this.input.files = dt.files;
            }
            this.handleFileChange();
        });
    }

    //-----------------------------------------------
    // E) On file input change
    //-----------------------------------------------
    handleFileChange() {
        // Clear new-file previews
        this.previewContainer.innerHTML = '';

        const files = Array.from(this.input.files || []);
        const fileCount = files.length;

        // Show/Hide the "Remove All" container if multi-file
        if (this.isMultiFile || this.isMultiImage) {
            this.removeAllContainer.style.display = fileCount > 0 ? 'block' : 'none';
        }

        if (fileCount === 0) {
            this.statusBar.textContent = 'No files selected';
            this.triggerButton.style.display = 'inline-block';
            this.progressBarContainer.style.display = 'none';

            // Also re-render existing items if we have them
            this.renderExistingItems();
            return;
        }

        // Hide the "Click or Drag & Drop" text
        this.triggerButton.style.display = 'none';
        this.statusBar.textContent = (fileCount === 1) ? '1 file selected' : fileCount + ' files selected';

        // Show progress bar
        this.progressBarContainer.style.display = 'block';
        this.progressBar.style.width = '0%';

        const totalBytes = files.reduce((sum, f) => sum + f.size, 0);
        let loadedBytes = 0;
        let filesReadCount = 0;

        files.forEach((file, index) => {
            const reader = new FileReader();
            let lastLoaded = 0;

            reader.onprogress = (e) => {
                if (e.lengthComputable) {
                    loadedBytes = loadedBytes - lastLoaded + e.loaded;
                    lastLoaded = e.loaded;
                    const percent = (loadedBytes / totalBytes) * 100;
                    this.progressBar.style.width = percent.toFixed(1) + '%';
                }
            };

            reader.onloadend = () => {
                filesReadCount++;
                if (this.isImage || this.isMultiImage) {
                    this.createImagePreview(reader.result, index, true); // "true" => it's a newly selected item
                } else {
                    this.createFilePreview(file.name, index, true);
                }

                // If done with all
                if (filesReadCount === fileCount) {
                    this.progressBar.style.width = '100%';
                    setTimeout(() => {
                        this.progressBarContainer.style.display = 'none';
                    }, 600);

                    // Also render existing items again
                    this.renderExistingItems();
                }
            };

            // For images => readAsDataURL, else => readAsArrayBuffer
            if (this.isImage || this.isMultiImage) {
                reader.readAsDataURL(file);
            } else {
                reader.readAsArrayBuffer(file);
            }
        });
    }

    //-----------------------------------------------
    // F) Create previews
    //-----------------------------------------------
    createImagePreview(fileDataUrl, index, isNew = false) {
        const previewItem = document.createElement('div');
        previewItem.classList.add('proBaseUploader-preview-item');

        const img = document.createElement('img');
        img.src = fileDataUrl;
        previewItem.appendChild(img);

        // "X" icon
        const removeIcon = this.createRemoveIcon(index, isNew);
        previewItem.appendChild(removeIcon);

        this.previewContainer.appendChild(previewItem);
    }

    createFilePreview(fileName, index, isNew = false) {
        const previewItem = document.createElement('div');
        previewItem.classList.add('proBaseUploader-preview-item');

        const fileNameElem = document.createElement('div');
        fileNameElem.classList.add('proBaseUploader-fileName');
        fileNameElem.textContent = fileName;
        previewItem.appendChild(fileNameElem);

        const removeIcon = this.createRemoveIcon(index, isNew);
        previewItem.appendChild(removeIcon);

        this.previewContainer.appendChild(previewItem);
    }

    createRemoveIcon(index, isNew) {
        const icon = document.createElement('div');
        icon.classList.add('proBaseUploader-removeIcon');
        icon.innerHTML = '&times;';

        // If isNew, we remove from the input's FileList
        // If not new, we remove from existing items array
        icon.addEventListener('click', (e) => {
            e.stopPropagation();
            if (isNew) {
                this.removeFileAt(index);
            } else {
                // It's an existing item => remove from existing previews
                this.removeExistingItem(index);
            }
        });
        return icon;
    }

    //-----------------------------------------------
    // G) Remove from input's FileList
    //-----------------------------------------------
    removeFileAt(indexToRemove) {
        const dt = new DataTransfer();
        const { files } = this.input;

        for (let i = 0; i < files.length; i++) {
            if (i !== indexToRemove) {
                dt.items.add(files[i]);
            }
        }
        this.input.files = dt.files;
        this.handleFileChange(); // re-render
    }

    removeAllFiles() {
        this.input.value = '';
        this.handleFileChange();
    }

    //-----------------------------------------------
    // H) Existing items rendering
    //-----------------------------------------------
    renderExistingItems() {
        // If the user is editing or viewing details, etc.
        // We'll show existing images/files along with any newly selected items
        if (this.actionType !== 'edit' && this.actionType !== 'details' && this.actionType !== 'delete') {
            return; // no existing items to show in "create" mode
        }

        // 1) Render single or multiple images
        if ((this.isImage || this.isMultiImage) && this.editImage) {
            // single image mode
            this.renderOneExistingImage(this.editImage);
        }
        if ((this.isImage || this.isMultiImage) && Array.isArray(this.editImageList) && this.editImageList.length > 0) {
            // multi-image mode
            this.editImageList.forEach((imgUrl, idx) => {
                this.renderOneExistingImage(imgUrl);
            });
        }

        // 2) Render single or multiple files
        if ((this.isFiles || this.isMultiFile) && this.editFile) {
            // Could be an object or just a string
            this.renderOneExistingFile(this.editFile);
        }
        if ((this.isFiles || this.isMultiFile) && Array.isArray(this.editFileList) && this.editFileList.length > 0) {
            this.editFileList.forEach((fObj) => {
                this.renderOneExistingFile(fObj);
            });
        }
    }

    renderOneExistingImage(imageUrl) {
        const previewItem = document.createElement('div');
        previewItem.classList.add('proBaseUploader-preview-item');

        const img = document.createElement('img');
        img.src = imageUrl;
        previewItem.appendChild(img);

        // Only show "X" if user can remove in "edit" or "delete" mode
        if (this.actionType === 'edit') {
            const removeIcon = this.createRemoveExistingIcon(imageUrl, null, true);
            previewItem.appendChild(removeIcon);
        }

        this.previewContainer.appendChild(previewItem);
    }

    renderOneExistingFile(fileObj) {
        // fileObj can be a string or an object with { id, fileName, downloadUrl }
        let displayName = '';
        let uniqueId = null;
        if (typeof fileObj === 'string') {
            displayName = fileObj;
        } else {
            displayName = fileObj.fileName || '(file)';
            uniqueId = fileObj.id || null;
        }

        const previewItem = document.createElement('div');
        previewItem.classList.add('proBaseUploader-preview-item');

        const fileNameElem = document.createElement('div');
        fileNameElem.classList.add('proBaseUploader-fileName');
        fileNameElem.textContent = displayName;
        previewItem.appendChild(fileNameElem);

        if (this.actionType === 'edit') {
            const removeIcon = this.createRemoveExistingIcon(fileObj, uniqueId, false);
            previewItem.appendChild(removeIcon);
        }

        this.previewContainer.appendChild(previewItem);
    }

    // Creates an icon that removes an existing item from the preview
    // In "removedExistingItems", we store the item so the server can handle the deletion logic
    createRemoveExistingIcon(itemData, uniqueId, isImage) {
        const icon = document.createElement('div');
        icon.classList.add('proBaseUploader-removeIcon');
        icon.innerHTML = '&times;';

        icon.addEventListener('click', (e) => {
            e.stopPropagation();
            // Mark it as removed
            this.removedExistingItems.push({
                isImage: isImage,
                data: itemData, // either the URL or the file object
                id: uniqueId
            });
            // remove from the UI
            icon.parentElement.remove();
        });

        return icon;
    }

    // If needed, you can read this.removedExistingItems in your form submission
    // to know which items the user removed from the server side.
}


//// proBaseUploader.js
//export class ProBaseUploader {
//    /**
//     * @param {Object} options
//     * @param {string} options.inputId - The ID of the file input element in the DOM.
//     * @param {boolean} [options.isImage=false] - Single image mode.
//     * @param {boolean} [options.isMultiImage=false] - Multi-image mode.
//     * @param {boolean} [options.isFiles=false] - Single file (non-image) mode.
//     * @param {boolean} [options.isMultiFile=false] - Multi-file (non-image) mode.
//     * @param {string} [options.imageTypes='all'] - Comma-separated image extensions, e.g. "png,jpg,gif".
//     *                                              "all" => "image/*"
//     * @param {string} [options.fileTypes='all']  - Comma-separated file extensions, e.g. "pdf,txt,docx".
//     *
//     */
//constructor({
//    inputId,
//    isImage = false,
//    isMultiImage = false,
//    isFiles = false,
//    isMultiFile = false,
//    imageTypes = 'all',
//    fileTypes = 'all'
//}) {
//    this.input = document.getElementById(inputId);
//    if (!this.input) {
//        console.error(`ProBaseUploader: No element found with ID "${inputId}".`);
//        return;
//    }

//    // Store config
//    this.isImage = isImage;
//    this.isMultiImage = isMultiImage;
//    this.isFiles = isFiles;
//    this.isMultiFile = isMultiFile;
//    this.imageTypes = imageTypes;
//    this.fileTypes = fileTypes;

//    // Prepare the file input’s accept/multiple attributes:
//    this.configureInputAttributes();

//    // Insert the necessary styles into the document head:
//    this.injectStyles();

//    // Create the main container & sub-elements:
//    this.wrapper = this.createWrapper();
//    this.statusBar = this.createStatusBar();
//    this.triggerButton = this.createTriggerButton();
//    this.progressBarContainer = this.createProgressBarContainer();
//    this.previewContainer = this.createPreviewContainer();

//    // Insert the wrapper before the input:
//    this.input.parentNode.insertBefore(this.wrapper, this.input);

//    // Append input & sub-elements inside the wrapper:
//    this.wrapper.appendChild(this.input);
//    this.wrapper.appendChild(this.statusBar);
//    this.wrapper.appendChild(this.triggerButton);
//    this.wrapper.appendChild(this.progressBarContainer);
//    this.wrapper.appendChild(this.previewContainer);

//    // For multi-file scenarios, create a “Remove All” link container:
//    if (this.isMultiFile || this.isMultiImage) {
//        this.removeAllContainer = this.createRemoveAllContainer();
//        this.wrapper.insertBefore(this.removeAllContainer, this.previewContainer);
//        // By default, hide it if there are no files initially
//        this.removeAllContainer.style.display = 'none';
//    }

//    // Make the entire wrapper clickable (except for remove icons/links):
//    this.setupWrapperClick();

//    // Handle file selection (change event):
//    this.input.addEventListener('change', () => this.handleFileChange());

//    // Optional: If you want to support “drag & drop”:
//    this.setupDragAndDrop();
//}

//// -----------------------------------------------------
//// 1) Set up input’s multiple/accept attributes
//// -----------------------------------------------------
//configureInputAttributes() {
//    // Single vs multiple
//    if (this.isMultiImage || this.isMultiFile) {
//        this.input.setAttribute('multiple', 'multiple');
//    } else {
//        this.input.removeAttribute('multiple');
//    }

//    // If user specifically wants images
//    if (this.isImage || this.isMultiImage) {
//        // If user sets imageTypes='all' => 'image/*'
//        if (this.imageTypes.toLowerCase() === 'all') {
//            this.input.setAttribute('accept', 'image/*');
//        } else {
//            // Parse comma-separated ext: "png,jpg" => ["png","jpg"]
//            const exts = this.imageTypes.split(',').map((s) => s.trim().toLowerCase());
//            // Convert each ext to a MIME type if well-known, or fallback to "image/EXT"
//            // e.g. "jpg" => "image/jpeg", "png" => "image/png"
//            const acceptList = exts.map((ext) => this.mapImageExtensionToMime(ext));
//            // Join into a comma string
//            this.input.setAttribute('accept', acceptList.join(','));
//        }
//    }
//    // If user specifically wants files
//    else if (this.isFiles || this.isMultiFile) {
//        // If user sets fileTypes='all' => '*/*'
//        if (this.fileTypes.toLowerCase() === 'all') {
//            this.input.setAttribute('accept', '*/*');
//        } else {
//            // Parse comma-separated ext: "pdf,txt" => [".pdf",".txt"]
//            // We'll just use extension-based accepts for simplicity
//            // e.g. "pdf" => ".pdf", "doc" => ".doc"
//            const exts = this.fileTypes.split(',').map((s) => s.trim().toLowerCase());
//            const acceptList = exts.map((ext) => {
//                // If it starts with '.' already, return as is
//                if (ext.startsWith('.')) return ext;
//                // Otherwise prepend a dot
//                return '.' + ext;
//            });
//            this.input.setAttribute('accept', acceptList.join(','));
//        }
//    }
//    // If none of the above, we remove accept entirely (the user can pick anything).
//    else {
//        this.input.removeAttribute('accept');
//    }
//}

//// A helper for well-known image extensions:
//mapImageExtensionToMime(ext) {
//    switch (ext) {
//        case 'jpg':
//        case 'jpeg':
//            return 'image/jpeg';
//        case 'png':
//            return 'image/png';
//        case 'gif':
//            return 'image/gif';
//        case 'webp':
//            return 'image/webp';
//        case 'svg':
//            return 'image/svg+xml';
//        default:
//            // fallback: "image/XXX"
//            return `image/${ext}`;
//    }
//}

//// -----------------------------------------------------
//// 2) Insert styling for the custom uploader
//// -----------------------------------------------------
//injectStyles() {
//    const styleTag = document.createElement('style');
//    styleTag.innerHTML = `
//            .proBaseUploader-wrapper {
//                border: 2px dashed #ccc;
//                border-radius: 6px;
//                padding: 1.5rem;
//                text-align: center;
//                position: relative;
//                margin-bottom: 1rem;
//                transition: background-color 0.2s ease-in-out;
//                cursor: pointer;
//            }
//            .proBaseUploader-wrapper:hover {
//                background-color: #fafafa;
//            }
//            .proBaseUploader-input {
//                display: none;
//            }
//            /* Status bar (shows how many files are selected) */
//            .proBaseUploader-statusBar {
//                margin-bottom: 0.5rem;
//                font-weight: 600;
//                font-size: 0.9rem;
//                color: #444;
//            }
//            /* "Click or Drag & Drop" text */
//            .proBaseUploader-triggerBtn {
//                display: inline-block;
//                font-size: 0.95rem;
//                margin-bottom: 1rem;
//                color: #007bff;
//                text-decoration: underline;
//                cursor: pointer;
//            }
//            .proBaseUploader-triggerBtn:hover {
//                text-decoration: none;
//            }
//            /* Remove All container/link */
//            .proBaseUploader-removeAllContainer {
//                margin-bottom: 1rem;
//            }
//            .proBaseUploader-removeAllLink {
//                font-size: 0.85rem;
//                color: #e74c3c;
//                text-decoration: underline;
//                cursor: pointer;
//            }
//            .proBaseUploader-removeAllLink:hover {
//                text-decoration: none;
//            }
//            /* Preview container */
//            .proBaseUploader-preview {
//                display: flex;
//                flex-wrap: wrap;
//                gap: 10px;
//                justify-content: center;
//                margin-top: 0.5rem;
//            }
//            .proBaseUploader-preview-item {
//                position: relative;
//                width: 80px;
//                height: 80px;
//                border: 1px solid #ddd;
//                border-radius: 4px;
//                overflow: hidden;
//                display: flex;
//                align-items: center;
//                justify-content: center;
//            }
//            .proBaseUploader-preview-item img {
//                width: 100%;
//                height: 100%;
//                object-fit: cover;
//            }
//            .proBaseUploader-fileName {
//                font-size: 0.75rem;
//                text-align: center;
//                padding: 4px;
//            }
//            /* "X" icon for removing a file */
//            .proBaseUploader-removeIcon {
//                position: absolute;
//                top: 4px;
//                right: 4px;
//                width: 20px;
//                height: 20px;
//                line-height: 20px;
//                text-align: center;
//                border-radius: 50%;
//                background-color: rgba(255,255,255,0.7);
//                color: #333;
//                cursor: pointer;
//                font-weight: bold;
//            }
//            .proBaseUploader-removeIcon:hover {
//                background-color: #fff;
//            }
//            /* Drag & Drop highlight */
//            .proBaseUploader-dragOver {
//                background-color: #eef;
//                border-color: #4da3ff;
//            }

//            /* Progress Bar */
//            .proBaseUploader-progressBarContainer {
//                width: 100%;
//                background-color: #f3f3f3;
//                border-radius: 4px;
//                overflow: hidden;
//                height: 8px;
//                margin: 0 auto 1rem auto;
//                display: none; /* hidden by default */
//            }
//            .proBaseUploader-progressBar {
//                height: 100%;
//                width: 0%;
//                background-color: #4caf50;
//                transition: width 0.1s linear;
//            }
//        `;
//    document.head.appendChild(styleTag);
//}

//// -----------------------------------------------------
//// 3) Create container, status, preview, progress bar, etc.
//// -----------------------------------------------------
//createWrapper() {
//    const wrapper = document.createElement('div');
//    wrapper.classList.add('proBaseUploader-wrapper');
//    this.input.classList.add('proBaseUploader-input'); // Hide original
//    return wrapper;
//}

//createStatusBar() {
//    const status = document.createElement('div');
//    status.classList.add('proBaseUploader-statusBar');
//    status.textContent = 'No files selected';
//    return status;
//}

//createTriggerButton() {
//    const btn = document.createElement('div');
//    btn.className = 'proBaseUploader-triggerBtn';
//    btn.textContent = 'Click or Drag & Drop to select files';
//    return btn;
//}

//createProgressBarContainer() {
//    const container = document.createElement('div');
//    container.classList.add('proBaseUploader-progressBarContainer');
//    this.progressBar = document.createElement('div');
//    this.progressBar.classList.add('proBaseUploader-progressBar');
//    container.appendChild(this.progressBar);
//    return container;
//}

//createPreviewContainer() {
//    const preview = document.createElement('div');
//    preview.classList.add('proBaseUploader-preview');
//    return preview;
//}

//createRemoveAllContainer() {
//    const container = document.createElement('div');
//    container.classList.add('proBaseUploader-removeAllContainer');

//    this.removeAllLink = document.createElement('span');
//    this.removeAllLink.classList.add('proBaseUploader-removeAllLink');
//    this.removeAllLink.textContent = 'Remove All Files';

//    // “Remove All” simply clears the input
//    this.removeAllLink.addEventListener('click', (e) => {
//        e.stopPropagation();
//        this.removeAllFiles();
//    });

//    container.appendChild(this.removeAllLink);
//    return container;
//}

//// -----------------------------------------------------
//// Click handling
//// -----------------------------------------------------
//setupWrapperClick() {
//    this.wrapper.addEventListener('click', (e) => {
//        // Don’t trigger if user clicked on remove icon or “Remove All” link
//        if (
//            e.target.classList.contains('proBaseUploader-removeIcon') ||
//            e.target.classList.contains('proBaseUploader-removeAllLink')
//        ) {
//            return;
//        }
//        // If user clicked on wrapper or “Click or Drag & Drop...” text, open file dialog
//        if (e.target === this.wrapper || e.target === this.triggerButton) {
//            this.input.click();
//        }
//    });
//}

//// -----------------------------------------------------
//// 4) File selection & reading
//// -----------------------------------------------------
//handleFileChange() {
//    // Clear current previews:
//    this.previewContainer.innerHTML = '';

//    const files = Array.from(this.input.files || []);
//    const fileCount = files.length;

//    // SHOW/HIDE the "Remove All" container if multi-file
//    if (this.isMultiFile || this.isMultiImage) {
//        if (fileCount > 0) {
//            this.removeAllContainer.style.display = 'block';
//        } else {
//            this.removeAllContainer.style.display = 'none';
//        }
//    }

//    if (fileCount === 0) {
//        this.statusBar.textContent = 'No files selected';
//        this.triggerButton.style.display = 'inline-block';
//        this.progressBarContainer.style.display = 'none';
//        return;
//    }

//    // Hide “Click or Drag & Drop” text once we have files
//    this.triggerButton.style.display = 'none';

//    // Update the status bar text:
//    this.statusBar.textContent = fileCount === 1 ? '1 file selected' : `${fileCount} files selected`;

//    // Show the progress bar
//    this.progressBarContainer.style.display = 'block';
//    this.progressBar.style.width = '0%';

//    // We'll track the total bytes of all files:
//    const totalBytes = files.reduce((sum, f) => sum + f.size, 0);
//    let loadedBytes = 0;
//    let filesReadCount = 0;

//    // We'll read each file in parallel, but maintain a shared loadedBytes.
//    files.forEach((file, index) => {
//        const reader = new FileReader();
//        let lastLoaded = 0; // track how many bytes have been read so far for this file

//        reader.onprogress = (e) => {
//            if (e.lengthComputable) {
//                loadedBytes = loadedBytes - lastLoaded + e.loaded;
//                lastLoaded = e.loaded;
//                const percent = (loadedBytes / totalBytes) * 100;
//                this.progressBar.style.width = percent.toFixed(1) + '%';
//            }
//        };

//        reader.onloadend = () => {
//            filesReadCount++;
//            // create the preview
//            if (this.isImage || this.isMultiImage) {
//                this.createImagePreview(reader.result, index);
//            } else {
//                this.createFilePreview(file.name, index);
//            }

//            // If we've finished reading all files, ensure progress is 100% and hide the bar
//            if (filesReadCount === fileCount) {
//                this.progressBar.style.width = '100%';
//                setTimeout(() => {
//                    this.progressBarContainer.style.display = 'none';
//                }, 600);
//            }
//        };

//        // Kick off reading (readAsDataURL for images, readAsArrayBuffer for non-image)
//        if (this.isImage || this.isMultiImage) {
//            reader.readAsDataURL(file);
//        } else {
//            reader.readAsArrayBuffer(file);
//        }
//    });
//}

//// -----------------------------------------------------
//// 5) Create preview elements
//// -----------------------------------------------------
//createImagePreview(fileDataUrl, index) {
//    const previewItem = document.createElement('div');
//    previewItem.classList.add('proBaseUploader-preview-item');

//    // <img> that displays the file
//    const img = document.createElement('img');
//    img.src = fileDataUrl;
//    previewItem.appendChild(img);

//    // "X" icon to remove this file
//    const removeIcon = this.createRemoveIcon(index);
//    previewItem.appendChild(removeIcon);

//    // Add to preview container
//    this.previewContainer.appendChild(previewItem);
//}

//createFilePreview(fileName, index) {
//    const previewItem = document.createElement('div');
//    previewItem.classList.add('proBaseUploader-preview-item');

//    // Show file name
//    const fileNameElem = document.createElement('div');
//    fileNameElem.classList.add('proBaseUploader-fileName');
//    fileNameElem.textContent = fileName;
//    previewItem.appendChild(fileNameElem);

//    // "X" icon to remove
//    const removeIcon = this.createRemoveIcon(index);
//    previewItem.appendChild(removeIcon);

//    // Add to preview container
//    this.previewContainer.appendChild(previewItem);
//}

//// -----------------------------------------------------
//// 6) “X” icon for each file
//// -----------------------------------------------------
//createRemoveIcon(index) {
//    const icon = document.createElement('div');
//    icon.classList.add('proBaseUploader-removeIcon');
//    icon.innerHTML = '&times;';
//    icon.addEventListener('click', (e) => {
//        e.stopPropagation();
//        this.removeFileAt(index);
//    });
//    return icon;
//}

//removeFileAt(indexToRemove) {
//    const dt = new DataTransfer();
//    const { files } = this.input;

//    // Rebuild the FileList, skipping the removed index
//    for (let i = 0; i < files.length; i++) {
//        if (i !== indexToRemove) {
//            dt.items.add(files[i]);
//        }
//    }

//    this.input.files = dt.files;
//    // Re-run handleFileChange to update previews & progress
//    this.handleFileChange();
//}

//removeAllFiles() {
//    // Reset the input’s FileList to empty
//    this.input.value = '';
//    this.handleFileChange();
//}

//// -----------------------------------------------------
//// 7) (Optional) Drag & Drop
//// -----------------------------------------------------
//setupDragAndDrop() {
//    this.wrapper.addEventListener('dragover', (e) => {
//        e.preventDefault();
//        e.stopPropagation();
//        this.wrapper.classList.add('proBaseUploader-dragOver');
//    });

//    this.wrapper.addEventListener('dragleave', (e) => {
//        e.preventDefault();
//        e.stopPropagation();
//        this.wrapper.classList.remove('proBaseUploader-dragOver');
//    });

//    this.wrapper.addEventListener('drop', (e) => {
//        e.preventDefault();
//        e.stopPropagation();
//        this.wrapper.classList.remove('proBaseUploader-dragOver');

//        const dt = e.dataTransfer;
//        if (!dt || !dt.files || !dt.files.length) return;

//        // For single-file input, only take the first file
//        if (!this.isMultiFile && !this.isMultiImage) {
//            const singleFileList = new DataTransfer();
//            singleFileList.items.add(dt.files[0]);
//            this.input.files = singleFileList.files;
//        } else {
//            // For multi-file input, accept all dropped files
//            this.input.files = dt.files;
//        }
//        this.handleFileChange();
//    });
//}
//}
