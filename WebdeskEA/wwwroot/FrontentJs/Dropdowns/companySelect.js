function initializeCompanySelect(companyId) {
    var selectElement = document.getElementById('companySelect');
    var tomSelectInstance = selectElement.tomselect;

    if (companyId && tomSelectInstance) {
        //alert(companyId);
        tomSelectInstance.setValue(companyId);
    }

    tomSelectInstance.on('change', function (value) {
        //alert(value);
        var selectedCompanyId = value;

        fetch(`/Common/DropDown/UpdateCompany?CompanyId=${selectedCompanyId}`, {
            method: 'GET',
            headers: {
                'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('Company changed successfully:', data);
                if (data) {
                    window.location.reload(); // Reload the page to reflect updated claims
                }
            })
            .catch(error => {
                console.error('There was a problem with the fetch operation:', error);
            });
    });
}
