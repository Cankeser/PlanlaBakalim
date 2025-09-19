document.addEventListener('DOMContentLoaded', function () {
    const Appointment = {
        appVersion: '2',
        maxStateAgeMs: 60 * 60 * 1000, // 1 saat
        currentStep: 0,
        selectedServices: [],
        selectedEmployeeId: null,
        selectedDate: null,
        selectedTime: null,
        currentWeekStart: new Date(),
        userType: 'guest',
        userId: null,
        getServerUserId: function () {
            const el = document.querySelector('#user-id');
            const val = el ? el.value : '';
            return val ? parseInt(val) : null;
        },
        getIsAuthenticated: function () {
            const el = document.querySelector('#is-authenticated');
            return el && el.value === 'true';
        },
        setGuestFieldRequired: function (isRequired) {
            const ids = [
                '#appointment-guest-name',
                '#appointment-guest-surname',
                '#appointment-guest-email',
                '#appointment-guest-phone'
            ];
            ids.forEach(sel => {
                const el = document.querySelector(sel);
                if (el) {
                    if (isRequired) el.setAttribute('required', 'required');
                    else el.removeAttribute('required');
                }
            });
        },

        init: function () {
            // Önce sunucudan auth durumunu oku
            const serverUserId = this.getServerUserId();
            const isAuth = this.getIsAuthenticated();
            if (isAuth) {
                this.userId = serverUserId;
                this.userType = 'account';
            }

            this.loadState(); // State’i yükle

            // State yüklendikten sonra da sunucu auth’u baskın olsun
            if (isAuth) {
                this.userId = serverUserId;
                this.userType = 'account';
                this.setGuestFieldRequired(false);
            } else {
                this.setGuestFieldRequired(this.userType === 'guest');
            }
            this.loadWeekDays();
            this.loadServices();
            this.loadEmployees();
            this.bindEvents();
            this.updateSteps();
            this.updateSidebar();
            this.initUserTypeToggle();
            this.saveState();
        },

        saveState: function () {
            const state = {
                appVersion: this.appVersion,
                businessId: parseInt(document.querySelector('#business-id').value),
                savedAt: Date.now(),
                currentStep: this.currentStep,
                selectedServices: this.selectedServices,
                selectedEmployeeId: this.selectedEmployeeId,
                selectedDate: this.selectedDate,
                selectedTime: this.selectedTime,
                userType: (this.getIsAuthenticated() ? 'account' : this.userType),
                userId: this.userId
            };
            sessionStorage.setItem('appointmentState', JSON.stringify(state));
        },

        loadState: function () {
            const raw = sessionStorage.getItem('appointmentState');
            if (!raw) return;
            let state;
            try { state = JSON.parse(raw); } catch { sessionStorage.removeItem('appointmentState'); return; }

            const currentBusinessId = parseInt(document.querySelector('#business-id').value);
            const isExpired = !state.savedAt || (Date.now() - state.savedAt) > this.maxStateAgeMs;
            const isDifferentBusiness = state.businessId && state.businessId !== currentBusinessId;
            const isVersionMismatch = state.appVersion !== this.appVersion;

            if (isExpired || isDifferentBusiness || isVersionMismatch) {
                sessionStorage.removeItem('appointmentState');
                return;
            }

            this.currentStep = state.currentStep || 0;
            this.selectedServices = state.selectedServices || [];
            this.selectedEmployeeId = state.selectedEmployeeId || null;
            this.selectedDate = state.selectedDate || null;
            this.selectedTime = state.selectedTime || null;
            this.userType = state.userType || 'guest';
            this.userId = state.userId || null;

            // Geçmiş tarih/saat temizleme
            if (this.selectedDate) {
                const today = new Date(); today.setHours(0, 0, 0, 0);
                const selDate = new Date(this.selectedDate); selDate.setHours(0, 0, 0, 0);
                if (selDate < today) {
                    this.selectedDate = null;
                    this.selectedTime = null;
                } else if (selDate.getTime() === today.getTime() && this.selectedTime) {
                    const [hh, mm] = this.selectedTime.split(':').map(Number);
                    const now = new Date();
                    const selected = new Date(); selected.setHours(hh, mm, 0, 0);
                    if (selected < now) this.selectedTime = null;
                }
            }
        },

        bindEvents: function () {
            const self = this;

            // Step navigation
            document.querySelector('#new-to-step-1').addEventListener('click', () => self.nextStep(1));
            document.querySelector('#new-to-step-2').addEventListener('click', () => self.nextStep(2));
            document.querySelector('#new-to-step-3').addEventListener('click', () => self.nextStep(3));
            document.querySelector('#new-to-step-4').addEventListener('click', () => self.createAppointment());

            document.querySelector('#new-back-to-step-0').addEventListener('click', () => self.prevStep(0));
            document.querySelector('#new-back-to-step-1').addEventListener('click', () => self.prevStep(1));
            document.querySelector('#new-back-to-step-2').addEventListener('click', () => self.prevStep(2));

            // Hizmet seçimi
            document.querySelector('#services-container').addEventListener('click', function (e) {
                const card = e.target.closest('.appointment-service-card');
                if (!card) return;
                const serviceId = parseInt(card.dataset.id);
                card.classList.toggle('selected');
                if (card.classList.contains('selected')) {
                    if (!self.selectedServices.includes(serviceId)) self.selectedServices.push(serviceId);
                } else {
                    self.selectedServices = self.selectedServices.filter(id => id !== serviceId);
                }
                document.querySelector('#selected-services-hidden').value = self.selectedServices.join(',');
                self.updateSidebar();
                self.saveState();
            });

            // Personel seçimi
            document.querySelector('#employees-container').addEventListener('click', function (e) {
                const card = e.target.closest('.appointment-employee-card');
                if (!card) return;
                document.querySelectorAll('#employees-container .appointment-employee-card').forEach(c => c.classList.remove('selected'));
                card.classList.add('selected');
                self.selectedEmployeeId = parseInt(card.dataset.id);
                document.querySelector('#selected-employee-id').value = self.selectedEmployeeId;
                self.selectedTime = null;
                self.loadTimeSlots();
                self.updateSidebar();
                self.saveState();
            });

            // Tarih seçimi
            document.querySelectorAll('.appointment-week-day').forEach(day => {
                day.addEventListener('click', function () {
                    if (day.classList.contains('disabled')) return;
                    self.selectDate(day.dataset.date);
                });
            });

            // Saat seçimi
            document.querySelector('#new-time-grid').addEventListener('click', function (e) {
                const slot = e.target.closest('.appointment-time-slot');
                if (!slot || slot.classList.contains('disabled')) return;
                document.querySelectorAll('#new-time-grid .appointment-time-slot').forEach(s => s.classList.remove('selected'));
                slot.classList.add('selected');
                self.selectedTime = slot.dataset.time;
                document.querySelector('#new-appointment-time').value = self.selectedTime;
                self.updateSidebar();
                self.saveState();
            });

            // Hafta navigasyonu
            document.querySelector('#new-week-prev').addEventListener('click', () => self.previousWeek());
            document.querySelector('#new-week-next').addEventListener('click', () => self.nextWeek());

            // Hesap login (eleman varsa bağla)
            const loginBtn = document.querySelector('#appointment-login-submit');
            if (loginBtn) loginBtn.addEventListener('click', () => self.loginAccount());

            // Telefon alanına input filtresi (yalnızca rakam, +, boşluk, parantez, tire)
            const phone = document.querySelector('#appointment-guest-phone');
            if (phone) {
                phone.addEventListener('input', function () {
                    const v = this.value;
                    const cleaned = v.replace(/[^0-9+()\-\s]/g, '');
                    if (v !== cleaned) this.value = cleaned;
                });
            }
        },

        nextStep: function (step) {
            if (step === 1 && this.selectedServices.length === 0) { this.showNotification("Lütfen en az bir hizmet seçin."); return; }
            if (step === 2 && !this.selectedEmployeeId) { this.showNotification("Lütfen bir personel seçin."); return; }
            if (step === 3 && (!this.selectedDate || !this.selectedTime)) { this.showNotification("Lütfen tarih ve saat seçin."); return; }
            this.currentStep = step;
            this.updateSteps();
            this.saveState();
        },

        prevStep: function (step) { this.currentStep = step; this.updateSteps(); this.saveState(); },

        updateSteps: function () {
            document.querySelectorAll('.appointment-step').forEach(el => {
                el.classList.toggle('active', parseInt(el.dataset.step) <= this.currentStep);
            });
            document.querySelectorAll('.appointment-step-section').forEach(sec => {
                sec.classList.remove('active');
                sec.style.display = 'none';
            });
            const activeSec = document.querySelector('#appointment-step-' + this.currentStep);
            if (activeSec) { activeSec.classList.add('active'); activeSec.style.display = 'block'; }
        },

        loadServices: function () {
            const container = document.querySelector('#services-container');
            container.innerHTML = '<div class="loading-spinner"><i class="fas fa-spinner fa-spin"></i> Hizmetler yükleniyor...</div>';
            fetch(`/randevu/hizmetler?businessId=${document.querySelector('#business-id').value}`)
                .then(res => res.json())
                .then(res => {
                    if (res.success) {
                        container.innerHTML = '';
                        res.data.forEach(s => {
                            const div = document.createElement('div');
                            div.className = 'appointment-service-card';
                            div.dataset.id = s.id;
                            div.dataset.name = s.name;
                            div.dataset.price = s.price;
                            if (this.selectedServices.includes(s.id)) div.classList.add('selected'); // önceki seçim
                            div.innerHTML = `<div class="appointment-service-content"><div class="appointment-service-info"><h4>${s.name}</h4></div><div class="appointment-service-price">${s.price}₺</div></div>`;
                            container.appendChild(div);
                        });
                        this.updateSidebar();
                    } else { this.showNotification("Hizmetler yüklenemedi."); }
                });
        },

        loadEmployees: function () {
            const container = document.querySelector('#employees-container');
            container.innerHTML = '<div class="loading-spinner"><i class="fas fa-spinner fa-spin"></i> Personeller yükleniyor...</div>';
            fetch(`/randevu/calisanlar?businessId=${document.querySelector('#business-id').value}`)
                .then(res => res.json())
                .then(res => {
                    if (res.success) {
                        container.innerHTML = '';
                        res.data.forEach(e => {
                            const div = document.createElement('div');
                            div.className = 'appointment-employee-card';
                            div.dataset.id = e.id;
                            div.dataset.name = e.user.fullName;
                            div.innerHTML = `<h4>${e.user.fullName}</h4><p>${e.position}</p>`;
                            if (this.selectedEmployeeId === e.id) div.classList.add('selected'); // önceki seçim
                            container.appendChild(div);
                        });
                        this.updateSidebar();
                    } else { this.showNotification("Personeller yüklenemedi."); }
                });
        },

        loadTimeSlots: function () {
            if (!this.selectedEmployeeId || !this.selectedDate) return;
            const container = document.querySelector('#new-time-grid');
            container.innerHTML = '<div class="loading-spinner"><i class="fas fa-spinner fa-spin"></i> Saatler yükleniyor...</div>';
            fetch(`/randevu/uygunsaatler?businessId=${document.querySelector('#business-id').value}&employeeId=${this.selectedEmployeeId}&date=${this.selectedDate}`)
                .then(res => res.json())
                .then(res => {
                    container.innerHTML = '';
                    if (res.success && res.data.length > 0) {
                        res.data.forEach(slot => {
                            const formatted = slot.time.substring(0, 5);
                            const div = document.createElement('div');
                            div.className = 'appointment-time-slot' + (slot.isAvailable ? '' : ' disabled');
                            div.dataset.time = formatted;
                            if (this.selectedTime === formatted) div.classList.add('selected'); // önceki seçim
                            div.innerHTML = `<div class="time">${formatted}</div><div class="status">${slot.isAvailable ? 'Müsait' : 'Dolu'}</div><div class="status-dot ${slot.isAvailable ? 'available' : 'occupied'}"></div>`;
                            container.appendChild(div);
                        });
                    } else {
                        container.innerHTML = '<div class="appointment-no-slots"><i class="fas fa-exclamation-circle"></i><p>Seçilen gün için uygun saat yok.</p></div>';
                    }
                });
        },

        selectDate: function (dateString) {
            this.selectedDate = dateString;
            document.querySelector('#new-appointment-date').value = dateString;
            document.querySelectorAll('.appointment-week-day').forEach(d => d.classList.remove('selected'));
            const day = Array.from(document.querySelectorAll('.appointment-week-day')).find(d => d.dataset.date === dateString);
            if (day) day.classList.add('selected');
            this.selectedTime = null;
            document.querySelector('#new-appointment-time').value = '';
            this.loadTimeSlots();
            this.updateSidebar();
            this.saveState();
        },

        loadWeekDays: function () {
            const self = this;
            const daysShort = ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'];
            const months = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];

            const startDate = new Date(this.currentWeekStart);
            const today = new Date(); today.setHours(0, 0, 0, 0);
            const endDate = new Date(startDate); endDate.setDate(startDate.getDate() + 6);

            document.querySelector('#new-week-title').textContent = 'Bu Hafta';
            document.querySelector('#new-week-date').textContent = `${startDate.getDate()}-${endDate.getDate()} ${months[startDate.getMonth()]} ${startDate.getFullYear()}`;

            document.querySelectorAll('.appointment-week-day').forEach((el, i) => {
                const dayDate = new Date(startDate);
                dayDate.setDate(startDate.getDate() + i);
                dayDate.setHours(0, 0, 0, 0);
                const dateStr = `${dayDate.getFullYear()}-${String(dayDate.getMonth() + 1).padStart(2, '0')}-${String(dayDate.getDate()).padStart(2, '0')}`;

                if (dayDate < today) {
                    el.classList.add('disabled');
                    el.style.display = 'none';
                } else {
                    el.classList.remove('disabled');
                    el.style.display = 'block';
                    el.querySelector('.appointment-day-name').textContent = daysShort[dayDate.getDay()];
                    el.querySelector('.appointment-day-number').textContent = dayDate.getDate();
                    el.querySelector('.appointment-day-month').textContent = months[dayDate.getMonth()];
                    el.dataset.date = dateStr;

                    if (self.selectedDate === dateStr || (!self.selectedDate && dayDate.getTime() === today.getTime())) {
                        el.classList.add('selected');
                        if (!self.selectedDate) self.selectedDate = dateStr;
                    } else el.classList.remove('selected');
                }
            });
        },

        previousWeek: function () {
            const today = new Date(); today.setHours(0, 0, 0, 0);
            const newStart = new Date(this.currentWeekStart);
            newStart.setDate(newStart.getDate() - 7);
            if (newStart < today) this.currentWeekStart = today;
            else this.currentWeekStart = newStart;
            this.loadWeekDays();
        },

        nextWeek: function () {
            this.currentWeekStart.setDate(this.currentWeekStart.getDate() + 7);
            this.loadWeekDays();
        },

        updateSidebar: function () {
            const sidebarContent = document.querySelector('#appointment-sidebar-content');
            const sidebarTotal = document.querySelector('#appointment-sidebar-total');
            if (!sidebarContent || !sidebarTotal) return;

            let content = '';
            let total = 0;

            if (this.selectedServices.length) {
                content += '<div class="appointment-summary-section"><h4>Seçilen Hizmetler</h4><div class="appointment-summary-list">';
                this.selectedServices.forEach(id => {
                    const card = document.querySelector(`.appointment-service-card[data-id="${id}"]`);
                    if (card) {
                        const name = card.dataset.name;
                        const price = parseInt(card.dataset.price);
                        total += price;
                        content += `<div class="appointment-summary-list-item"><span class="list-title">${name}</span><span class="list-price">${price}₺</span></div>`;
                    }
                });
                content += '</div></div>';
            }

            if (this.selectedEmployeeId) {
                const empEl = document.querySelector(`.appointment-employee-card[data-id="${this.selectedEmployeeId}"]`);
                if (empEl) {
                    const empName = empEl.dataset.name;
                    content += `<div class="appointment-summary-section"><h4>Seçilen Personel</h4><div class="appointment-summary-list"><div class="appointment-summary-list-item"><span class="list-title">${empName}</span><span class="list-price status-selected">Seçildi</span></div></div></div>`;
                }
            }

            if (this.selectedDate && this.selectedTime) {
                const date = new Date(this.selectedDate);
                const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                content += `<div class="appointment-summary-section"><h4>Randevu Detayları</h4><div class="appointment-summary-list"><div class="appointment-summary-list-item"><span class="list-title">${date.toLocaleDateString('tr-TR', options)}</span><span class="list-price">${this.selectedTime}</span></div></div></div>`;
            }

            if (content === '') content = `<div class="appointment-summary-empty"><i class="fas fa-info-circle"></i><p>Henüz seçim yapılmadı</p><small>Adımları takip ederek randevu oluşturun</small></div>`;

            sidebarContent.innerHTML = content;
            sidebarTotal.innerHTML = total > 0 ? `<div class="appointment-summary-total"><span>Toplam</span><span>₺${total}</span></div>` : '';
        },

        showNotification: function (msg, type = 'error', duration = 3000) {
            const notif = document.createElement('div');
            notif.className = `notification notification-${type}`;
            notif.innerHTML = `<div class="notification-content"><span>${msg}</span></div><button class="notification-close">&times;</button>`;
            document.body.appendChild(notif);

            notif.querySelector('.notification-close').addEventListener('click', () => notif.remove());

            setTimeout(() => notif.remove(), duration);
        },

        initUserTypeToggle: function () {
            // Sunucudan gelen kullanıcı Id’sini gizli inputtan al
            const userIdInput = document.querySelector('#user-id');
            const serverUserId = userIdInput ? (userIdInput.value || null) : null;

            if (serverUserId) {
                // Kullanıcı zaten giriş yapmış: userType/account, alanları gizle
                this.userId = parseInt(serverUserId);
                this.userType = 'account';
                // Bu view’da giriş/guest alanları render edilmemiş olabilir, yine de varsa sakla
                const guestFields = document.querySelector('#appointment-guest-fields');
                const accountFields = document.querySelector('#appointment-account-fields');
                if (guestFields) guestFields.style.display = 'none';
                if (accountFields) accountFields.style.display = 'none';
                // Radio’lar yoksa sorun değil; varsa işaretlemeyi güncelle
                const accountRadio = document.querySelector('input[name="appointment-usertype"][value="account"]');
                if (accountRadio) accountRadio.checked = true;
                this.saveState();
                return;
            }

            // Misafir/giriş akışı
            const guestFields = document.querySelector('#appointment-guest-fields');
            const accountFields = document.querySelector('#appointment-account-fields');
            if (guestFields) guestFields.style.display = this.userType === 'guest' ? 'block' : 'none';
            if (accountFields) accountFields.style.display = this.userType === 'account' ? 'block' : 'none';

            document.querySelectorAll('input[name="appointment-usertype"]').forEach(radio => {
                radio.addEventListener('change', function () {
                    if (this.value === 'guest') {
                        if (guestFields) guestFields.style.display = 'block';
                        if (accountFields) accountFields.style.display = 'none';
                        Appointment.userType = 'guest';
                        Appointment.setGuestFieldRequired(true);
                    } else {
                        if (guestFields) guestFields.style.display = 'none';
                        if (accountFields) accountFields.style.display = 'block';
                        Appointment.userType = 'account';
                        Appointment.setGuestFieldRequired(false);
                    }
                    Appointment.saveState();
                });
            });
        },

        createAppointment: function () {
            const isAuth = this.getIsAuthenticated();
            // Yalnızca misafir modunda native form doğrulamasını tetikle
            if (!isAuth && this.userType === 'guest') {
                const form = document.getElementById('appointment-info-form');
                if (form && !form.checkValidity()) {
                    form.reportValidity();
                    return; // Geçersizse gönderme
                }
            }
            const serverUserId = this.getServerUserId();
            const effectiveUserType = isAuth ? 'account' : this.userType;
            if (isAuth) this.userId = serverUserId;
            const data = {
                BusinessId: parseInt(document.querySelector('#business-id').value),
                EmployeeId: parseInt(this.selectedEmployeeId),
                AppointmentDate: this.selectedDate,
                AppointmentTime: this.selectedTime,
                SelectedServices: this.selectedServices,
                FullName: (document.querySelector('#appointment-guest-name') ? (document.querySelector('#appointment-guest-name').value + ' ' + document.querySelector('#appointment-guest-surname').value) : null),
                Email: (document.querySelector('#appointment-guest-email') ? document.querySelector('#appointment-guest-email').value : null),
                Phone: (document.querySelector('#appointment-guest-phone') ? document.querySelector('#appointment-guest-phone').value : null),
                Note: (document.querySelector('#appointment-guest-note') ? document.querySelector('#appointment-guest-note').value : null),
                userId: this.userId,
                UserType: effectiveUserType
            };

            fetch('/Randevu/Olustur', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            }).then(res => res.json()).then(res => {
                if (res.success) {
                    this.currentStep = 4;
                    this.updateSteps();
                    this.showNotification(res.message, 'success');
                    // Randevu oluşturulduktan sonra eski seçimler tutulmasın
                    sessionStorage.removeItem('appointmentState');
                } else {
                    this.showNotification(res.message, 'error');
                }
            }).catch(() => this.showNotification('Sunucuya bağlanırken hata oluştu.', 'error'));
        },

        loginAccount: function () {
            const email = document.querySelector('#appointment-login-email').value;
            const password = document.querySelector('#appointment-login-password').value;

            if (!email || !password) { this.showNotification('E-posta ve şifre gerekli'); return; }

            fetch('/Hesap/AjaxLogin', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Email: email, Password: password })
            }).then(res => res.json()).then(res => {
                if (res.success) {
                    this.userId = res.userId;
                    this.userType = 'account';
                    try { sessionStorage.removeItem('appointmentState'); } catch {}
                    this.saveState();
                    location.reload(); // login sonrası sayfayı yenile ama seçimler korunur
                } else this.showNotification(res.message || 'Giriş başarısız');
            });
        }
    };

    Appointment.init();
});
