document.addEventListener('DOMContentLoaded', () => {
    const steps = Array.from(document.querySelectorAll('.step-content'));
    let currentStep = 0;

    const state = {
        customer: { name: '', phone: '', email: '' },
        selectedEmployeeId: null,
        selectedEmployeeName: null,
        date: null,
        time: null,
        services: [],
        total: 0,
        notes: '',
        usertype: 'guest'
    };

    // --- Tarih input ve min-date ayarı ---
    const dateInput = document.getElementById('appointmentDate');
    const today = new Date();
    const yyyy = today.getFullYear();
    const mm = String(today.getMonth() + 1).padStart(2, '0');
    const dd = String(today.getDate()).padStart(2, '0');
    const minDate = `${yyyy}-${mm}-${dd}`;

    dateInput.min = minDate;     // geçmişi engelle
    dateInput.value = minDate;   // default bugün
    state.date = minDate;

    dateInput.addEventListener('change', handleDateChange);

    // --- Step visibility ---
    function updateStepVisibility() {
        steps.forEach((step, i) => step.classList.toggle('active', i === currentStep));
        checkStepButtons();
        updateConfirmation();
    }

    function checkStepButtons() {
        document.getElementById('step1Next').disabled = !state.customer.name.trim() || !state.customer.phone.trim();
        document.getElementById('step2Next').disabled = !state.selectedEmployeeId;
        document.getElementById('step3Next').disabled = !state.date || !state.time || state.services.length === 0;
    }

    function updateConfirmation() {
        document.getElementById('confirmCustomer').textContent = `${state.customer.name} (${state.customer.phone})`;
        document.getElementById('confirmEmployee').textContent = state.selectedEmployeeName || '-';
        document.getElementById('confirmDateTime').textContent = state.date && state.time ? `${state.date} ${state.time}` : '-';
        document.getElementById('confirmServices').textContent = state.services.map(s => s.name).join(', ') || '-';
        document.getElementById('confirmTotal').textContent = `₺${state.total}`;
    }

    // --- Step navigation ---
    window.nextStep = () => { if (currentStep < steps.length - 1) currentStep++; updateStepVisibility(); };
    window.prevStep = () => { if (currentStep > 0) currentStep--; updateStepVisibility(); };

    // --- Customer input ---
    ['newCustomerName', 'newCustomerPhone', 'newCustomerEmail'].forEach(id => {
        document.getElementById(id).addEventListener('input', e => {
            if (id === 'newCustomerName') state.customer.name = e.target.value;
            if (id === 'newCustomerPhone') state.customer.phone = e.target.value;
            if (id === 'newCustomerEmail') state.customer.email = e.target.value;
            checkStepButtons();
        });
    });

    // --- Employee selection ---
    window.selectEmployee = (el, employeeId, name) => {
        document.querySelectorAll('.employee-card').forEach(c => c.classList.remove('selected'));
        el.classList.add('selected');
        state.selectedEmployeeId = employeeId;
        state.selectedEmployeeName = name;
        checkStepButtons();
        updateConfirmation();
        loadAvailableTimes();
    };


    // --- Date & Time ---
    function handleDateChange() {
        let val = dateInput.value;
        if (val < dateInput.min) val = dateInput.min; // geçmiş kontrol
        dateInput.value = val;
        state.date = val;
        state.time = null;
        checkStepButtons();
        updateConfirmation();
        loadAvailableTimes();
    }

    window.selectTimeSlot = (el, time) => {
        document.querySelectorAll('.time-slot').forEach(c => c.classList.remove('selected'));
        el.classList.add('selected');
        state.time = time;
        checkStepButtons();
        updateConfirmation();
    };

    // --- Service selection ---
    window.toggleService = (el, serviceId) => {
        const name = el.querySelector('.service-name').textContent;
        const price = parseInt(el.querySelector('.service-price').textContent.replace('₺', ''));
        const index = state.services.findIndex(s => s.id === serviceId);
        if (index === -1) {
            state.services.push({ id: serviceId, name, price });
            el.classList.add('selected');
        } else {
            state.services.splice(index, 1);
            el.classList.remove('selected');
        }
        state.total = state.services.reduce((sum, s) => sum + s.price, 0);
        checkStepButtons();
        updateConfirmation();
    };

    // --- AJAX: Load Employees ---
    function loadEmployees() {
        const container = document.querySelector('.employee-grid');
        container.innerHTML = '<div>Yükleniyor...</div>';
        fetch('/BusinessPanel/Appointment/GetEmployees')
            .then(res => res.json())
            .then(res => {
                if (!res.success) throw new Error('Çalışanlar yüklenemedi!');
                container.innerHTML = '';
                res.data.forEach(emp => {
                    const div = document.createElement('div');
                    div.className = 'employee-card';
                    div.innerHTML = `<div class="employee-avatar">
                                        <img src="${emp.user.avatar}" alt="${emp.user.fullName}" class="avatar-img">
                                     </div>
                                     <div class="employee-info">
                                        <div class="employee-name">${emp.user.fullName}</div>
                                        <div class="employee-role">${emp.position}</div>
                                     </div>`;
                    div.onclick = () => selectEmployee(div, emp.id, emp.user.fullName);
                    container.appendChild(div);
                });
            })
            .catch(err => Swal.fire({ icon: 'error', title: 'Hata', text: err.message }));
    }
    function loadServices() {
        const container = document.querySelector('.services-list');
        container.innerHTML = '<div>Yükleniyor...</div>';
        fetch('/BusinessPanel/Appointment/GetServices')
            .then(res => res.json())
            .then(res => {
                if (!res.success) throw new Error('Servisler yüklenemedi!');
                container.innerHTML = '';
                res.data.forEach(s => {
                    const div = document.createElement('div');
                    div.className = 'service-item';
                    div.innerHTML = `<div class="service-info"><div class="service-name">${s.name}</div>
                                     <div class="service-price">₺${s.price}</div></div>
                                     <div class="service-checkbox"><i class="bi bi-check-circle"></i></div>`;
                    div.onclick = () => toggleService(div, s.id);

                    container.appendChild(div);
                });
            })
            .catch(err => Swal.fire({ icon: 'error', title: 'Hata', text: err.message }));
    }

    // --- AJAX: Load Available Times ---
    function loadAvailableTimes() {
        if (!state.date || !state.selectedEmployeeId) return;
        const container = document.querySelector('.time-slots-grid');
        container.innerHTML = '<div>Yükleniyor...</div>';
        fetch(`/BusinessPanel/Appointment/GetAvailableSlots?employeeId=${state.selectedEmployeeId}&date=${state.date}`)
            .then(res => res.json())
            .then(response => {
                const slots = response.data;
                container.innerHTML = '';

                if (!slots || !slots.length) {
                    container.innerHTML = `<div class="appointment-no-slots">
                        <i class="fas fa-exclamation-circle"></i>
                        <p>Seçilen gün için uygun saat yok.</p>
                    </div>`;
                    return;
                }

                slots.forEach(slot => {
                    const formatted = slot.time.substring(0, 5);
                    const slotDateTime = new Date(`${state.date}T${slot.time}`);
                    if (slotDateTime < new Date()) return; // geçmiş saatleri atla

                    const div = document.createElement('div');
                    div.className = `time-slot ${slot.isAvailable ? 'available' : 'occupied'}`;
                    div.dataset.time = formatted;
                    div.innerHTML = `<div class="time">${formatted}</div>
                                     <div class="status">${slot.isAvailable ? 'Müsait' : 'Dolu'}</div>
                                     <div class="status-dot ${slot.isAvailable ? 'available' : 'occupied'}"></div>`;
                    if (slot.isAvailable) div.onclick = () => selectTimeSlot(div, formatted);
                    container.appendChild(div);
                });
            })
            .catch(err => Swal.fire({ icon: 'error', title: 'Hata', text: err.message }));
    }

    // --- Create Appointment ---
    window.createAppointment = () => {
        state.notes = document.getElementById('appointmentNotes').value;
        fetch('/BusinessPanel/Appointment/Create', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                EmployeeId: state.selectedEmployeeId,
                AppointmentDate: state.date,
                AppointmentTime: state.time,
                SelectedServices: state.services.map(s => s.id),
                FullName: state.customer.name,
                Email: state.customer.email,
                Phone: state.customer.phone,
                Note: state.notes,
                Usertype: state.usertype
            })
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'success',
                        title: data.message,
                        showConfirmButton: false,
                        timer: 2000,
                        timerProgressBar: true,
                        didClose: () => {
                            location.reload(); // Toast kapandıktan sonra sayfayı yeniler
                        }
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: data.message || 'Bir hata oluştu!'
                    });
                }
            })
            .catch(err => Swal.fire({ icon: 'error', title: 'Hata', text: err.message }));
    };

    // --- Initial load ---
    loadEmployees();
    loadServices();
    updateStepVisibility();
});