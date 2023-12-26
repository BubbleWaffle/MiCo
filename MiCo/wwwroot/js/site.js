/* Edit profile checkbox action */
var formFileInput = document.getElementById('formFile');
var deleteCheckbox = document.getElementById('flexCheckDefault');
var deleteText = document.getElementById('deleteText');

deleteCheckbox.addEventListener('change', function () {
    formFileInput.disabled = this.checked;
    deleteText.classList.toggle('text-danger', this.checked);
});