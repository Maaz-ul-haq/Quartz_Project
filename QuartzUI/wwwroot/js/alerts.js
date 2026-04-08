window.showAlert = (type, title, message) => {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: type,
            title: title,
            text: message,
            confirmButtonColor: '#3085d6',
            width: '500px', 
            background: '#fff',
            customClass: {
                popup: 'perfect-box-shadow',
                confirmButton: 'px-4 py-2'
            }
        });
    } else {
        console.error("SweetAlert2 library is not loaded.");
    }
};