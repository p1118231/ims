function selectValue(id, name) {
    document.getElementById("newValue").value = id;
    const dropdownButton = document.querySelector(".dropdown-toggle");
    dropdownButton.textContent = name;
}

function showToast(message) {
    document.getElementById('toastMessage').textContent = message;
    const toastElement = document.getElementById('successToast');
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
}

function confirmAndSave() {
    let fieldToEdit = "@ViewBag.FieldToEdit".toLowerCase();
    let confirmationMessage = fieldToEdit === "restock" 
        ? "Are you sure you want to place a restock order?" 
        : "Are you sure you want to save the changes?";

    const isConfirmed = confirm(confirmationMessage);
    
    if (isConfirmed) {
        let toastMessage = fieldToEdit === "restock" 
            ? "Order made to supplier!" 
            : "Changes have been saved successfully!";
            
        showToast(toastMessage);
        return true;
    }
    return false;
}