import { HttpClient } from '../Common/common.js';

const client = new HttpClient('/CompanySites/CompanyBusinessCategory');

//--------------------------------------
//--------------- Elements -------------
//--------------------------------------
const btnAddUpdate = document.querySelector('#addUpdateBtn');
const btnUpdate = document.querySelector('#updateBtn');
const btnReset = document.querySelector('#resetBtn');
const btnDelete = document.querySelectorAll('.deleteBtn');

const input_hiddenId_CompanyBusinessCategory = document.querySelector('#hiddenId');
const input_hiddenCompanyId_CompanyBusinessCategory = document.querySelector('#CompanyId');
const input_Name_CompanyBusinessCategory = document.querySelector('#Name');
const input_NameCompany_CompanyBusinessCategory = document.querySelector('#CompanyName');


//--------------------------------------
//------------- Event ------------------
//--------------------------------------

//Row Click Set Data
document.querySelector('.datanew').addEventListener('click', function (event) {
    const dataTable = $('.datanew').DataTable();

    const target = event.target.closest('tr');
    if (target) {
        const rowData = dataTable.row(target).data();
        input_hiddenId_CompanyBusinessCategory.value = rowData[0];
        input_Name_CompanyBusinessCategory.value = rowData[1];
        input_NameCompany_CompanyBusinessCategory.value = rowData[2];

        btnAddUpdate.classList.add('d-none');
        btnUpdate.classList.remove('d-none');
        btnReset.classList.remove('d-none');
    }
});

// Insert event
btnAddUpdate.addEventListener('click', async function () {
    if (input_Name_CompanyBusinessCategory.value) {
        await createCompanyBusinessCategory();
    }
    else {
        alert("Empty Values not Valid")
    }
});

// update envent
btnUpdate.addEventListener('click', async function () {
    await updateCompanyBusinessCategory(parseInt(input_hiddenId_CompanyBusinessCategory.value));
});

//Delete Event
btnDelete.forEach(button => {
    button.addEventListener('click', async (event) => {
        const id = event.currentTarget.getAttribute('data-id');
        if (confirm('Are you sure you want to delete this record?')) {
            await deleteCompanyBusinessCategory(id);
        }
    });
});

// Reset Event
btnReset.addEventListener('click', function () {
    reset()
});


//--------------------------------------
//----------- Common -------------------
//--------------------------------------
function reset() {
    // Clear input fields
    input_hiddenId_CompanyBusinessCategory.value = '';
    input_Name_CompanyBusinessCategory.value = '';

    // Show add button, hide update and reset buttons
    btnAddUpdate.classList.remove('d-none');
    btnUpdate.classList.add('d-none');
    btnReset.classList.add('d-none');
}

//--------------------------------------
//------------- CRUD -------------------
//--------------------------------------
//Create
const createCompanyBusinessCategory = async () => {
    const dataTable = $('.datanew').DataTable();

    const categoryDto = {
        id: 0,
        name: input_Name_CompanyBusinessCategory.value,
        CompanyId: Number(input_hiddenCompanyId_CompanyBusinessCategory.value),
    };

    try {
        const response = await client.post('CreateBusinessCategory', categoryDto);

        if (response.success) {
            console.log(response.message);

            const newRowData = [
                response.obj.id,
                response.obj.name,
                response.obj.companyName,
                `<a class="delete-set" data-id="${response.obj.id}"><img src="/Template/img/icons/delete.svg" alt="svg"></a>`
            ];

            dataTable.row.add(newRowData).draw(false);
        } else {
            console.error(response.message);
        }
    } catch (error) {
        console.error('Error:', error);
    }
};

// Update
const updateCompanyBusinessCategory = async (Id) => {
    const dataTable = $('.datanew').DataTable();
    const categoryDto = {
        id: Id,
        name: input_Name_CompanyBusinessCategory.value,
        companyId: parseInt(input_hiddenCompanyId_CompanyBusinessCategory.value),
        companyName: input_NameCompany_CompanyBusinessCategory.value
    };

    try {
        const response = await client.post(`UpdateCompanyBusinessCategory/${Id}`, categoryDto);

        if (response.success) {
            const rowIndex = dataTable.row((idx, data, node) => {
                console.log('Comparing:', data[0], categoryDto.id); // Debugging
                return data[0] == categoryDto.id;
            }).index();

            console.log(rowIndex)

            if (rowIndex !== undefined && rowIndex !== null) {
                dataTable.row(rowIndex).data([
                    categoryDto.id,
                    categoryDto.name,
                    categoryDto.companyName,
                    `<a class="delete-set" data-id="${categoryDto.id}"><img src="/Template/img/icons/delete.svg" alt="svg"></a>`
                ]).draw(false);
            } else {
                console.error('Row index not found!');
            }

            reset();

        } else {
            console.error(response.message);
        }
    } catch (error) {
        console.error('Error:', error);
    }
};

// Delete
const deleteCompanyBusinessCategory = async (id) => {
    const dataTable = $('.datanew').DataTable();

    try {
        const response = await client.post(`DeleteCompanyBusinessCategory/${id}`);
        if (response.success) {
            dataTable.rows().every(function () {
                const row = this.node();
                const deleteButton = row.querySelector('.deleteBtn');

                if (deleteButton && deleteButton.getAttribute('data-id') === String(id)) {
                    dataTable.row(row).remove().draw(false);
                }
            });
        } else {
            console.error(response.message);
        }
    } catch (error) {
        console.error('Error:', error);
    }
};