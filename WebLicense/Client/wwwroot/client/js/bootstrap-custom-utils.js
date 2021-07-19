function toggleModalWindow(modalName, state) {
    if (!modalName) return;

    try {
        var modal = document.getElementById(modalName);
        if (!modal) return;

        var bsModal = bootstrap.Modal.getInstance(modal);

        if (!state) {
            bsModal.hide();
        } else {
            bsModal.show();
        }
    } catch (e) {
        /* ignore */
        console.log(e);
    } 
}