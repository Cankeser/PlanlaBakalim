function switchTab(tabName) {
    // Hide all tab contents
    const tabContents = document.querySelectorAll('.tab-content');
    tabContents.forEach(content => {
        content.classList.remove('active');
    });

    // Remove active class from all tab buttons
    const tabButtons = document.querySelectorAll('.tab-btn');
    tabButtons.forEach(btn => {
        btn.classList.remove('active');
    });

    // Show selected tab content
    const selectedTab = document.getElementById(tabName + '-tab');
    if (selectedTab) {
        selectedTab.classList.add('active');
    }

    // Add active class to clicked tab button
    const clickedButton = document.querySelector(`[onclick="switchTab('${tabName}')"]`);
    if (clickedButton) {
        clickedButton.classList.add('active');
    }
}
function acceptAppointment(appointmentId) {
    Swal.fire({
        title: 'Randevuyu Onayla',
        text: 'Bu randevuyu onaylamak istediğinizden emin misiniz?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Onayla',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch('/BusinessPanel/Appointment/AcceptAppointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ appointmentId: appointmentId })
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Başarılı!',
                        text: data.message,
                        timer: 2000,
                        showConfirmButton: false
                    }).then(() => {
                        location.reload();
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata!',
                        text: data.message
                    });
                }
            })
            .catch(error => {
                console.error('Error:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Hata!',
                    text: 'Randevu onaylanırken bir hata oluştu.'
                });
            });
        }
    });
}

function rejectAppointment(appointmentId) {
    Swal.fire({
        title: 'Randevuyu İptal Et',
        text: 'Bu randevuyu iptal etmek istediğinizden emin misiniz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, İptal Et',
        cancelButtonText: 'Vazgeç'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch('/BusinessPanel/Appointment/RejectAppointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ appointmentId: appointmentId })
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Başarılı!',
                        text: data.message,
                        timer: 2000,
                        showConfirmButton: false
                    }).then(() => {
                        location.reload();
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata!',
                        text: data.message
                    });
                }
            })
            .catch(error => {
                console.error('Error:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Hata!',
                    text: 'Randevu iptal edilirken bir hata oluştu.'
                });
            });
        }
    });
}

function callCustomer(phoneNumber) {
    window.location.href = `tel:${phoneNumber}`;
}

function viewDetails(appointmentId) {
    const modalEl = document.getElementById('appointmentDetailsModal');

    fetch(`/BusinessPanel/Appointment/GetAppointmentDetails?id=${appointmentId}`)
        .then(res => res.json())
        .then(data => {
            modalEl.querySelector('.customer-name-large').textContent = data.name || '';
            modalEl.querySelector('.customer-phone-large').textContent = data.phone || '';
            modalEl.querySelector('.customer-email').textContent = data.email || '';
            modalEl.querySelector('#appointment-avatar').src = data.avatar;
            modalEl.querySelector('#appointment-employee').textContent = data.employee || '';
            modalEl.querySelector('#appointment-date').textContent = data.date || '';           
            modalEl.querySelector('.appointment-notes').textContent = data.notes || '';
            const servicesContainer = modalEl.querySelector('#appointment-services');
            servicesContainer.textContent = data.services.length
                ? data.services.join(', ')
                : 'Hizmet eklenmemiş';

            const statusMap = {
                "Onaylandi": "confirmed",
                "IptalEdildi": "cancelled",
                "Beklemede": "pending",
                "Tamamlandi": "completed"
            };

            const status = modalEl.querySelector('#appointment-status');

            // Önce eski status-* class'larını temizle
            status.classList.forEach(cls => {
                if (!cls.startsWith('status-b')) status.classList.remove(cls);
            });

            // Yeni durumu ekle
            const cssClass = statusMap[data.status];
            if (cssClass) {
                status.classList.add('status-' + cssClass);
                status.textContent = data.status; // ekran için Türkçe gösterim
            }


            const bsModal = new bootstrap.Modal(modalEl);
            bsModal.show();
        })
        .catch(err => console.error(err));
}
function closeModal() {
    const modal = bootstrap.Modal.getInstance(document.getElementById('appointmentDetailsModal'));
    if (modal) {
        modal.hide();
    }
}


function searchAppointments() {
    const searchInput = document.querySelector('.search-input');
    const searchQuery = searchInput ? searchInput.value.trim() : '';
    
    if (searchQuery) {
        // Arama yapıldığında yönlendir (statik)
        // Burada gerçek arama sayfasına yönlendirilecek
        // window.location.href = `search-results.html?q=${encodeURIComponent(searchQuery)}`;
    }
}

// Filter change function - UI için
function filterAppointments() {
    const statusFilter = document.querySelector('select[value="confirmed"]')?.value;
    const dateFilter = document.querySelector('select[value="today"]')?.value;
    
    // Burada gerçek filtreleme yapılacak
}

// Export functions for global access
window.switchTab = switchTab;
window.callCustomer = callCustomer;
window.acceptAppointment = acceptAppointment;
window.rejectAppointment = rejectAppointment;
window.viewDetails = viewDetails;
window.closeModal = closeModal;
window.searchAppointments = searchAppointments;
window.filterAppointments = filterAppointments;
