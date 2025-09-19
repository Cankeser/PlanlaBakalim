
// Modal aç/kapa
function showAddModal() {
    document.getElementById('addModal').classList.add('show');
    document.getElementById('overlay').classList.add('show');
}
function hideAddModal() {
    document.getElementById('addModal').classList.remove('show');
    document.getElementById('overlay').classList.remove('show');
    resetUploadArea();
    document.getElementById('photoTitle').value = '';
    document.getElementById('showProfile').checked = true;
}

// Upload area tıklama
const uploadArea = document.getElementById('uploadArea');
const fileInput = document.getElementById('fileInput');
uploadArea.addEventListener('click', () => fileInput.click());

// Preview ve kaldır
fileInput.addEventListener('change', function () {
    const file = this.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById('uploadContent').style.display = 'none';
        document.getElementById('previewContent').style.display = 'block';
        document.getElementById('previewImage').src = e.target.result;
    };
    reader.readAsDataURL(file);
});

document.getElementById('removePreviewBtn').addEventListener('click', function (e) {
    e.stopPropagation();
    resetUploadArea();
});

// Reset upload area
function resetUploadArea() {
    fileInput.value = '';
    document.getElementById('uploadContent').style.display = 'block';
    document.getElementById('previewContent').style.display = 'none';
    document.getElementById('previewImage').src = '';
}

// Submit fotoğraf
document.getElementById('submitPhotoBtn').addEventListener('click', function () {
    const form = document.getElementById('addPhotoForm');
    const formData = new FormData(form);

    fetch('/BusinessPanel/Gallery/AddPhoto', {
        method: 'POST',
        body: formData
    })
        .then(res => res.json())
        .then(result => {
            if (result.success) {
                hideAddModal();
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: result.message,
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
                setTimeout(() => location.reload(), 1000);
            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: result.message || "Bir hata oluştu",
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
            }
        })
        .catch(err => {
            console.error(err);
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: "Sunucu hatası",
                showConfirmButton: false,
                timer: 2000,
                timerProgressBar: true
            });
        });
});

function toggleProfileVisibility(id) {
    const checkbox = document.getElementById(`showProfile-${id}`);
    const isVisible = checkbox.checked;

    fetch(`/BusinessPanel/Gallery/ToggleProfileVisibility?id=${id}&isVisible=${isVisible}`, {
        method: 'POST'
    })
        .then(res => res.json())
        .then(result => {
            if (!result.success) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: result.message || "Bir hata oluştu",
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
                checkbox.checked = !isVisible; // hata olursa geri al
            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: result.message,
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
            }
        })
        .catch(err => {
            console.error(err);
            checkbox.checked = !isVisible;
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: "Sunucu hatası",
                showConfirmButton: false,
                timer: 2000,
                timerProgressBar: true
            });
        });
}



function hideAllModals() {
    hideAddModal();
    hideDeleteModal();
}
