// ===== SERVICES PAGE JAVASCRIPT =====


// Show add service modal
function showAddModal() {
    const modal = document.getElementById('addServiceModal');
    const overlay = document.getElementById('overlay');
    modal.classList.add('show');
    overlay.classList.add('show');
    document.body.style.overflow = 'hidden';
}

// Hide add service modal
function hideAddModal() {
    const modal = document.getElementById('addServiceModal');
    const overlay = document.getElementById('overlay');
    modal.classList.remove('show');
    overlay.classList.remove('show');
    document.body.style.overflow = '';
    
    // Reset form
    document.getElementById('addServiceForm').reset();
}

// Show edit service modal
function editService(serviceId) {
    const modal = document.getElementById('editServiceModal');
    const overlay = document.getElementById('overlay');
    modal.classList.add('show');
    overlay.classList.add('show');
    document.body.style.overflow = 'hidden';

    // Formu sıfırla
    document.getElementById('editServiceForm').reset();

    // Servis bilgilerini getir
    fetch(`/BusinessPanel/Services/GetServiceDetails?id=${serviceId}`)
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                const s = result.data;

                // Formu doldur
                document.getElementById('editServiceId').value = s.id;
                document.getElementById('editServiceName').value = s.name;
                document.getElementById('editServiceDescription').value = s.description;
                document.getElementById('editServicePrice').value = s.price;
                document.getElementById('editServiceActive').checked = s.isActive;

            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: result.message,
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
                hideEditModal();
            }
        })
        .catch(err => {
            console.error("Servis detayı alınamadı:", err);
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Sunucu hatası!',
                showConfirmButton: false,
                timer: 2000,
                timerProgressBar: true
            });
            hideEditModal();
        });
}
function updateService(){
    const data = {
        Id: document.getElementById('editServiceId').value,
        Name: document.getElementById('editServiceName').value,
        Description: document.getElementById('editServiceDescription').value,
        Price: parseFloat(document.getElementById('editServicePrice').value),
        Active: document.getElementById('editServiceActive').checked
    };

    fetch('/BusinessPanel/Services/Update', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Hizmet başarıyla güncellendi',
                    showConfirmButton: false,
                    timer: 800,
                    timerProgressBar: true
                }).then(() => {
                    // ASP.NET MVC sayfasına yönlendirme
                    window.location.href = "/BusinessPanel/Services/Index";
                });

                hideEditModal();
                // gerekirse tabloyu yenile
                // location.reload();
            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: result.message || 'Güncelleme başarısız',
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
            }
        })
        .catch(err => {
            console.error("Hizmet güncellenemedi:", err);
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Sunucu hatası!',
                showConfirmButton: false,
                timer: 2000,
                timerProgressBar: true
            });
        });
}

function addService() {
    const data = {
        Name: document.getElementById('serviceName').value,
        Description: document.getElementById('serviceDescription').value,
        Price: parseFloat(document.getElementById('servicePrice').value)
    };

    fetch('/BusinessPanel/Services/Add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Hizmet başarıyla eklendi',
                    showConfirmButton: false,
                    timer: 800,
                    timerProgressBar: true
                }).then(() => {
                    // ASP.NET MVC sayfasına yönlendirme
                    window.location.href = "/BusinessPanel/Services/Index";
                });

                hideEditModal();
                // gerekirse tabloyu yenile
                // location.reload();
            } else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    title: result.message || 'Ekleme başarısız',
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
            }
        })
        .catch(err => {
            console.error("Hizmet eklenemedi:", err);
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Sunucu hatası!',
                showConfirmButton: false,
                timer: 2000,
                timerProgressBar: true
            });
        });
}

// Hide edit service modal
function hideEditModal() {
    const modal = document.getElementById('editServiceModal');

    modal.classList.remove('show');
 
    document.body.style.overflow = '';
}

// Hide all modals
function hideAllModals() {
    const modals = document.querySelectorAll('.modal');
    
    modals.forEach(modal => {
        modal.classList.remove('show');
    });
 
    document.body.style.overflow = '';
   
}
 
 
document.addEventListener('click', function(event) {
    if (event.target.classList.contains('modal')) {
        hideAllModals();
    }
});

// Close modals with Escape key
document.addEventListener('keydown', function(event) {
    if (event.key === 'Escape') {
        hideAllModals();
    }
});

// Export functions for global access
window.showAddModal = showAddModal;
window.hideAddModal = hideAddModal;
window.editService = editService;
window.hideEditModal = hideEditModal;
window.hideAllModals = hideAllModals;
window.addService = addService;
window.updateService = updateService;

