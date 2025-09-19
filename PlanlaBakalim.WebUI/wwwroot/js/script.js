// Planla Bakalım - Müşteri Arayüzü JavaScript (jQuery Tabanlı)
// ASP.NET ile uyumlu, optimize edilmiş versiyon

$(document).ready(function () {
    'use strict';

    // ===== MOBİL NAVİGASYON =====
    const MobileNav = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            $('#mobile-menu-btn').on('click', this.openMenu);
            $('#mobile-close-btn').on('click', this.closeMenu);
            $(document).on('click', this.handleOutsideClick);
        },

        openMenu: function () {
            $('.mobile-nav').addClass('active');
            $('body').css('overflow', 'hidden');
        },

        closeMenu: function () {
            $('.mobile-nav').removeClass('active');
            $('body').css('overflow', '');
        },

        handleOutsideClick: function (e) {
            const mobileNav = $('.mobile-nav');
            const mobileMenuBtn = $('#mobile-menu-btn');

            if (mobileNav.hasClass('active') &&
                !mobileNav.is(e.target) &&
                mobileNav.has(e.target).length === 0 &&
                !mobileMenuBtn.is(e.target) &&
                mobileMenuBtn.has(e.target).length === 0) {
                MobileNav.closeMenu();
            }
        }
    };

    // ===== DROPDOWN NAVİGASYON =====
    const DropdownNav = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            $('.dropdown-toggles').on('click', this.toggleDropdown);
            $(document).on('click', this.handleOutsideClick);
        },

        toggleDropdown: function (e) {
            e.preventDefault();
            const dropdown = $(this).closest('.dropdown');
            const isActive = dropdown.hasClass('show');

            // Diğer dropdown'ları kapat
            $('.dropdown').removeClass('show');

            // Mevcut dropdown'ı toggle et
            if (!isActive) {
                dropdown.addClass('show');
            }
        },

        handleOutsideClick: function (e) {
            if (!$(e.target).closest('.dropdown').length) {
                $('.dropdown').removeClass('show');
            }
        }
    };

    // ===== MOBİL KATEGORİ TOGGLE =====
    const MobileCategory = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            $('.mobile-category-header').on('click', this.toggleCategory);
        },

        toggleCategory: function () {
            const category = $(this).closest('.mobile-category');
            const isActive = category.hasClass('active');

            // Diğer kategorileri kapat
            $('.mobile-category').removeClass('active');

            // Mevcut kategoriyi toggle et
            if (!isActive) {
                category.addClass('active');
            }
        }
    };

    // ===== ARAMA İŞLEVLERİ =====
    const SearchFunctions = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            $('#clear-search').on('click', this.clearSearch);
        },

        clearSearch: function () {
            $('#search-text').val('').focus();
        }
    };

    // ===== FAVORİ İŞLEVLERİ =====
    const FavoriteFunctions = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            $(document).on('click', '.favorite-btn', this.toggleFavorite.bind(this));
        },

        toggleFavorite: function (e) {
            e.preventDefault();
            e.stopPropagation();

            const btn = $(e.currentTarget);
            const businessId = btn.data('businessid');
            const icon = btn.find('i');
            const isActive = btn.hasClass('active');

            $.ajax({
                url: isActive ? '/Favorites/Remove' : '/Favorites/Add',
                method: 'POST',
                data: { businessId: businessId },
                success: function (response) {
                    // Eğer backend redirectUrl gönderirse → login sayfasına yönlendir
                    if (response && response.redirectUrl) {
                        window.location.href = response.redirectUrl;
                        return;
                    }

                    if (isActive) {
                        btn.removeClass('active');
                        icon.removeClass('fas fa-heart').addClass('far fa-heart');
                        Swal.fire({
                            icon: 'info',
                            title: 'Başarılı!',
                            text: 'Favorilerden çıkarıldı!'
                        });
                    } else {
                        btn.addClass('active');
                        icon.removeClass('far fa-heart').addClass('fas fa-heart');
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı!',
                            text: 'Favorilere eklendi!'
                        });
                    }
                },
                error: function (xhr) {
                    if (xhr.status === 401) {
                        // Unauthorized → login sayfasına yönlendir
                        window.location.href = '/Hesap/Girisyap';
                    } else {
                        console.log(xhr)
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata!',
                            text: 'İşlem gerçekleştirilemedi!' + xhr.message
                        });
                    }
                }
            });
        }
    };



    // ===== PROFİL SAYFASI İŞLEVLERİ =====
    const ProfileFunctions = {
        init: function () {
            this.bindEvents();
            this.initSidebar();
            this.initTabs();
            this.initPhotoUpload();
            this.bindPasswordChange();
            this.preventPasswordAutofill();
            this.bindProfileUpdate();
        },

        bindEvents: function () {
            $('#mobileSidebarToggle').on('click', this.toggleSidebar);
            $('.mobile-sidebar-overlay').on('click', this.closeSidebar);
            $('.sidebar-nav-item').on('click', this.handleNavClick);
            $('.tab-btn').on('click', this.handleTabClick);
            $('.settings-tab-btn').on('click', this.handleSettingsTabClick);
            $('.expand-btn').on('click', this.handleExpandClick);
            $('.favorite-remove').on('click', this.handleFavoriteRemove);
            $('#deleteAccountBtn').on('click', this.handleDeleteAccount);
        },

        initSidebar: function () { },

        toggleSidebar: function () {
            const sidebar = $('.profile-sidebar');
            const overlay = $('.mobile-sidebar-overlay');
            const mobileMenuBtn = $('#mobile-menu-btn');
            const mobileCloseBtn = $('#mobile-close-btn');

            sidebar.toggleClass('active');
            overlay.toggleClass('active');

            if (sidebar.hasClass('active')) {
                mobileMenuBtn.css({ 'pointer-events': 'none', 'opacity': '0.5', 'cursor': 'not-allowed' });
                mobileCloseBtn.css({ 'pointer-events': 'none', 'opacity': '0.5', 'cursor': 'not-allowed' });
            } else {
                mobileMenuBtn.css({ 'pointer-events': 'auto', 'opacity': '1', 'cursor': 'pointer' });
                mobileCloseBtn.css({ 'pointer-events': 'auto', 'opacity': '1', 'cursor': 'pointer' });
            }
        },

        closeSidebar: function () {
            $('.profile-sidebar').removeClass('active');
            $('.mobile-sidebar-overlay').removeClass('active');
            $('#mobile-menu-btn, #mobile-close-btn').css({ 'pointer-events': 'auto', 'opacity': '1', 'cursor': 'pointer' });
        },

        handleNavClick: function (e) {
            e.preventDefault();
            const targetSection = $(this).data('section');
            $('.sidebar-nav-item').removeClass('active');
            $('.content-section').removeClass('active');
            $(this).addClass('active');
            $('#' + targetSection).addClass('active');
            if ($(window).width() <= 768) ProfileFunctions.closeSidebar();
        },

        initTabs: function () { },

        handleTabClick: function () {
            const targetTab = $(this).data('tab');
            $('.tab-btn').removeClass('active');
            $('.tab-content').removeClass('active');
            $(this).addClass('active');
            $('#' + targetTab + '-tab').addClass('active');
        },

        handleSettingsTabClick: function () {
            const targetTab = $(this).data('tab');
            $('.settings-tab-btn').removeClass('active');
            $('.settings-tab-content').removeClass('active');
            $(this).addClass('active');
            $('#' + targetTab + '-tab').addClass('active');
        },

        handleExpandClick: function () {
            const targetId = $(this).data('target');
            const detailsElement = $('#details-' + targetId);
            const icon = $(this).find('i');
            if (!detailsElement.length) return;

            if (detailsElement.hasClass('expanded')) {
                detailsElement.removeClass('expanded');
                $(this).removeClass('expanded');
                icon.removeClass('fa-chevron-up').addClass('fa-chevron-down');
            } else {
                detailsElement.addClass('expanded');
                $(this).addClass('expanded');
                icon.removeClass('fa-chevron-down').addClass('fa-chevron-up');
            }
        },

        preventPasswordAutofill: function () {
            const passwordFields = $('#currentPassword, #newPassword, #confirmPassword');

            passwordFields.attr('autocomplete', 'new-password');
            passwordFields.val('');
            $(window).on('load', function () {
                passwordFields.val('');
            });
        },

        initPhotoUpload: function () {
            const fileInput = $('<input type="file" accept="image/*" style="display:none">');
            $('body').append(fileInput);

            $('#uploadPhotoBtn').on('click', function () { fileInput.click(); });

            fileInput.on('change', function (e) {
                const file = e.target.files[0];
                if (!file) return;

                if (file.size > 5 * 1024 * 1024) return ProfileFunctions.showMessage('Dosya boyutu 5MB\'dan küçük olmalıdır.', 'error');
                if (!file.type.startsWith('image/')) return ProfileFunctions.showMessage('Lütfen geçerli bir resim dosyası seçin.', 'error');

                const reader = new FileReader();
                reader.onload = function (e) {
                    $('.current-photo img').attr('src', e.target.result);
                    $('#savePhotoBtn').prop('disabled', false).css('opacity', '1').addClass('active');
                };
                reader.readAsDataURL(file);
            });

            $('#savePhotoBtn').on('click', function () {
                const fileInputEl = $('input[type="file"]')[0];
                if (!fileInputEl.files.length) return;

                const formData = new FormData();
                formData.append('file', fileInputEl.files[0]);

                $.ajax({
                    url: '/Hesap/ProfilFotoGuncelle',
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        $('.current-photo img').attr('src', res.profileUrl);
                        $('.profile-avatar img').attr('src', res.profileUrl);
                        ProfileFunctions.showMessage(res.message, 'success');
                        $('#savePhotoBtn').prop('disabled', true).css('opacity', '0.5').removeClass('active');
                        fileInputEl.value = '';
                    },
                    error: function (err) {
                        const msg = err.responseJSON?.message || "Bir hata oluştu, tekrar deneyin.";
                        ProfileFunctions.showMessage(msg, 'error');
                    }
                });
            }).prop('disabled', true).css('opacity', '0.5');
        },

        bindPasswordChange: function () {
            $('#changePasswordBtn').on('click', function () {
                const currentPassword = $('#currentPassword').val();
                const newPassword = $('#newPassword').val();
                const confirmPassword = $('#confirmPassword').val();

                if (!currentPassword || !newPassword || !confirmPassword)
                    return ProfileFunctions.showMessage('Lütfen tüm alanları doldurun.', 'error');

                $.ajax({
                    url: '/Hesap/SifreDegistir',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ currentPassword, newPassword, confirmPassword }),
                    success: function (res) {
                        ProfileFunctions.showMessage(res.message, 'success');
                        $('#currentPassword, #newPassword, #confirmPassword').val('');
                    },
                    error: function (err) {
                        const msg = err.responseJSON?.message || "Bir hata oluştu, tekrar deneyin.";
                        ProfileFunctions.showMessage(msg, 'error');
                    }
                });
            });
        }
,

        bindProfileUpdate: function () {
            $('#updateProfileBtn').on('click', function () {
                const fullName = $('#fullName').val();
                const email = $('#email').val();
                const phone = $('#phone').val();

                if (!fullName || !email) return ProfileFunctions.showMessage('Ad ve e-posta boş olamaz.', 'error');

                $.ajax({
                    url: '/Hesap/ProfilGuncelle',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ fullName, email, phone }),
                    success: function () {
                        ProfileFunctions.showMessage('Profil başarıyla güncellendi.', 'success');
                    },
                    error: function () {
                        ProfileFunctions.showMessage('Profil güncellenirken hata oluştu.', 'error');
                    }
                });
            });
        },

        handleDeleteAccount: function () {
            ProfileFunctions.showAccountDeletionConfirm();
        },

        showMessage: function (message, type) {
            const bgColor = type === 'error' ? '#ef4444' : '#10b981';
            const messageDiv = $(`
            <div class="message-toast" style="
                position: fixed;
                top: 20px;
                right: 20px;
                background: ${bgColor};
                color: white;
                padding: 1rem;
                border-radius: 8px;
                z-index: 1000;
                animation: slideInRight 0.3s ease;
            ">${message}</div>
        `);
            $('body').append(messageDiv);
            setTimeout(() => {
                messageDiv.css('animation', 'slideOutRight 0.3s ease');
                setTimeout(() => messageDiv.remove(), 300);
            }, 3000);
        },

        showAccountDeletionConfirm: function () {
            const confirmDiv = $(`
            <div class="account-deletion-modal" style="
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(0,0,0,0.7);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 1000;
            ">
                <div class="confirm-content large">
                    <div class="modal-header danger">
                        <h3>Hesabı Sil</h3>
                    </div>
                    <div class="modal-body">
                        <div class="warning-message">
                            <i class="fas fa-exclamation-triangle"></i>
                            <p>Bu işlem geri alınamaz. Hesabınızı silmek istediğinizden emin misiniz?</p>
                        </div>
                        <div class="deletion-input">
                            <label for="confirmText">Onaylamak için "ONAY" yazın</label>
                            <input type="text" id="confirmText" placeholder="ONAY" class="form-input">
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-secondary" id="cancelBtn">İptal</button>
                        <button class="btn btn-danger" id="confirmBtn" disabled>Hesabımı Sil</button>
                    </div>
                </div>
            </div>
        `);

            $('body').append(confirmDiv);

            const confirmText = $('#confirmText');
            const confirmBtn = $('#confirmBtn');

            confirmText.on('input', function () {
                confirmBtn.prop('disabled', $(this).val() !== 'ONAY');
            });

            $('#cancelBtn').on('click', () => { confirmDiv.remove(); });

            confirmBtn.on('click', () => {
                if (confirmText.val() === 'ONAY') {
                    $.ajax({
                        url: '/Hesap/HesapSil',
                        type: 'POST',
                        success: function () {
                            ProfileFunctions.showMessage('Hesap başarıyla silindi.', 'success');
                            setTimeout(() => location.href = '/', 1500);
                        },
                        error: function () {
                            ProfileFunctions.showMessage('Hesap silinirken bir hata oluştu.', 'error');
                        }
                    });
                    confirmDiv.remove();
                }
            });
        }
    };


    // ===== FORM İŞLEVLERİ =====
    const FormFunctions = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            // Şifre göster/gizle
            $('.password-toggle').on('click', this.togglePassword);

            // Input focus efektleri
            $('.form-group input, .form-group textarea').on('focus', this.handleInputFocus);
            $('.form-group input, .form-group textarea').on('blur', this.handleInputBlur);
        },

        togglePassword: function () {
            const input = $(this).siblings('input');
            const icon = $(this).find('i');

            if (input.attr('type') === 'password') {
                input.attr('type', 'text');
                icon.removeClass('fa-eye').addClass('fa-eye-slash');
            } else {
                input.attr('type', 'password');
                icon.removeClass('fa-eye-slash').addClass('fa-eye');
            }
        },

        handleInputFocus: function () {
            $(this).parent().css('transform', 'translateY(-2px)');
        },

        handleInputBlur: function () {
            $(this).parent().css('transform', 'translateY(0)');
        }
    };

    // ===== MODAL İŞLEVLERİ =====
    const ModalFunctions = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            // Randevu detay modal
            $('.action-btn-compact[title="Detaylar"]').on('click', this.showAppointmentDetail);
            $('.action-btn-compact[title="Ara"]').on('click', this.handleCall);

            // Modal kapatma
            $(document).on('click', this.handleModalClose);
            $(document).on('keydown', this.handleEscapeKey);
        },

        showAppointmentDetail: function () {
            const appointmentItem = $(this).closest('.appointment-item-compact');
            const businessName = appointmentItem.find('.business-details h4').text();
            const businessCategory = appointmentItem.find('.business-details p').text();

            $('#modal-business-name').text(businessName);
            $('#modal-business-category').text(businessCategory);
            $('#appointmentDetailModal').removeClass('active').addClass('active');
        },

        handleCall: function () {
            const phoneNumber = $(this).data('phone');
            if (phoneNumber) {
                window.location.href = `tel:${phoneNumber}`;
            }
        },

        handleModalClose: function (e) {
            const modal = $('#appointmentDetailModal');
            if (modal.hasClass('active') && e.target === modal[0]) {
                modal.removeClass('active');
            }
        },

        handleEscapeKey: function (e) {
            if (e.key === 'Escape') {
                $('#appointmentDetailModal').removeClass('active');
            }
        }
    };

    // ===== DEĞERLENDİRME MODAL =====
    const RatingFunctions = {
        init: function () {
            this.currentRating = 5;
            this.bindEvents();
        },

        bindEvents: function () {
            $('.star-rating i').on('click', this.handleStarClick);
            $('.star-rating i').on('mouseenter', this.handleStarHover);
            $('.star-rating i').on('mouseleave', this.handleStarLeave);
        },

        handleStarClick: function () {
            const index = $(this).index();
            RatingFunctions.currentRating = index + 1;
            RatingFunctions.fillStars(RatingFunctions.currentRating);
        },

        handleStarHover: function () {
            const index = $(this).index();
            RatingFunctions.fillStars(index + 1);
        },

        handleStarLeave: function () {
            RatingFunctions.fillStars(RatingFunctions.currentRating);
        },

        fillStars: function (rating) {
            $('.star-rating i').each(function (index) {
                if (index < rating) {
                    $(this).addClass('filled');
                } else {
                    $(this).removeClass('filled');
                }
            });
        }
    };

    // ===== ANİMASYONLAR =====
    const AnimationFunctions = {
        init: function () {
            this.initScrollAnimations();
            this.initHoverEffects();
        },

        initScrollAnimations: function () {
            const observerOptions = {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            };

            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        $(entry.target).css({
                            'opacity': '1',
                            'transform': 'translateY(0)'
                        });
                    }
                });
            }, observerOptions);

            $('.category-card, .ci-business-card, .feature-item, .step').each(function () {
                $(this).css({
                    'opacity': '0',
                    'transform': 'translateY(20px)',
                    'transition': 'opacity 0.6s ease, transform 0.6s ease'
                });
                observer.observe(this);
            });
        },

        initHoverEffects: function () {
            $('.category-card, .ci-business-card, .feature-item').hover(
                function () {
                    $(this).css('transform', 'translateY(-8px)');
                },
                function () {
                    $(this).css('transform', 'translateY(0)');
                }
            );
        }
    };

    // ===== SMOOTH SCROLLING =====
    const SmoothScroll = {
        init: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            $('a[href^="#"]').on('click', this.handleAnchorClick);
        },

        handleAnchorClick: function (e) {
            const href = $(this).attr('href');
            if (href === '#') return;

            const target = $(href);
            if (target.length) {
                e.preventDefault();
                target[0].scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        }
    };

    // ===== GLOBAL FONKSİYONLAR =====
    window.showAppointmentDetailModal = function (businessName, businessCategory) {
        $('#modal-business-name').text(businessName);
        $('#modal-business-category').text(businessCategory);
        $('#appointmentDetailModal').removeClass('active').addClass('active');
    };

    window.loadPastAppointmentDetail = function (appointmentId) {
        // Basit loading
        $('#modal-business-name').text('Yükleniyor...');
        $('#modal-business-category').text('');
        $('#appointmentDetailModal').addClass('active');

        $.getJSON(`/Profile/Appointments/${appointmentId}`, function (res) {
            if (!res || !res.success) {
                ProfileFunctions.showMessage(res && res.message ? res.message : 'Detay alınamadı', 'error');
                $('#appointmentDetailModal').removeClass('active');
                return;
            }

            const d = res.data;
            $('#modal-business-name').text(d.business.name || 'İşletme');
            const serviceNames = (d.services && d.services.length) ? d.services.map(s => s.name).join(', ') : '';
            $('#modal-business-category').text(serviceNames);

            // İçerik gövdesi basitleştirilmiş (mevcut alanlara koyuyoruz)
            $('#modal-business-date').text(`${d.date}, ${d.time}`);
            $('#modal-employee').text(d.employee && d.employee.name ? d.employee.name : '-');
            $('#modal-price').text(`${(d.total || 0).toLocaleString('tr-TR')} ₺`);
            // Süre/hizmetler istenirse burada listelenebilir
        }).fail(function () {
            ProfileFunctions.showMessage('Sunucu hatası', 'error');
            $('#appointmentDetailModal').removeClass('active');
        });
    };

    window.closeAppointmentDetailModal = function () {
        $('#appointmentDetailModal').removeClass('active');
    };

    window.editAppointment = function () {
        ProfileFunctions.showMessage('Randevu düzenleme özelliği yakında eklenecek!', 'success');
        window.closeAppointmentDetailModal();
    };

    window.toggleAppointmentDetails = function () {
        const detailsElement = $('#appointmentDetails');
        const detailsBtn = $('.appointment-actions .btn-primary');

        if (!detailsElement.length || !detailsBtn.length) return;

        const detailsIcon = detailsBtn.find('i');

        if (detailsElement.css('display') === 'none') {
            detailsElement.show();
            detailsIcon.removeClass('fa-eye').addClass('fa-eye-slash');
            detailsBtn.html('<i class="fas fa-eye-slash"></i> Gizle');
        } else {
            detailsElement.hide();
            detailsIcon.removeClass('fa-eye-slash').addClass('fa-eye');
            detailsBtn.html('<i class="fas fa-eye"></i> Detaylar');
        }
    };

    window.openRatingModal = function () {
        $('#ratingModal').show();
        $('body').css('overflow', 'hidden');
        RatingFunctions.currentRating = 5;
        RatingFunctions.fillStars(5);
    };

    window.closeRatingModal = function () {
        $('#ratingModal').hide();
        $('body').css('overflow', '');
        $('#ratingComment').val('');
        $('#ratingSuccess').hide();
    };

    window.submitRating = function () {
        const comment = $('#ratingComment').val();

        if (RatingFunctions.currentRating === 0) return;

        console.log('Değerlendirme Gönderildi:', {
            rating: RatingFunctions.currentRating,
            comment: comment,
            timestamp: new Date().toISOString()
        });

        $('#ratingSuccess').show();
        setTimeout(() => {
            window.closeRatingModal();
        }, 3000);
    };

    window.toggleMobileProfile = function () {
        const trigger = $('.mobile-profile-trigger');
        const menu = $('#mobileProfileMenu');
        const chevron = $('#mobileProfileChevron');

        if (trigger.length && menu.length && chevron.length) {
            trigger.toggleClass('active');
            menu.toggleClass('active');

            if (trigger.hasClass('active')) {
                chevron.css({
                    'transform': 'rotate(180deg)',
                    'color': 'var(--primary)'
                });
            } else {
                chevron.css({
                    'transform': 'rotate(0deg)',
                    'color': 'var(--text-muted)'
                });
            }
        }
    };

    // ===== BAŞLATMA =====
    MobileNav.init();
    DropdownNav.init();
    MobileCategory.init();
    SearchFunctions.init();
    FavoriteFunctions.init();
    ProfileFunctions.init();
    FormFunctions.init();
    ModalFunctions.init();
    RatingFunctions.init();
    AnimationFunctions.init();
    SmoothScroll.init();

    // Profile countdown başlat
    if ($('.profile-main').length) {
        startProfileCountdown();
    }

    // Profile countdown fonksiyonu
    function startProfileCountdown() {
        const appointmentCountdown = $('.appointment-countdown');
        if (!appointmentCountdown.length) return;

        const appointmentDate = appointmentCountdown.data('appointment-date');
        if (!appointmentDate) return;

        const targetDate = new Date(appointmentDate);

        function updateCountdown() {
            const now = new Date();
            const timeDiff = targetDate - now;

            if (timeDiff <= 0) {
                $('#appointmentCountdown').text('Randevu zamanı!').css('color', '#ef4444');
                return;
            }

            const days = Math.floor(timeDiff / (1000 * 60 * 60 * 24));
            const hours = Math.floor((timeDiff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((timeDiff % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((timeDiff % (1000 * 60)) / 1000);

            let countdownText = '';
            if (days > 0) {
                countdownText = `${days}g ${hours}s ${minutes}d`;
            } else if (hours > 0) {
                countdownText = `${hours}s ${minutes}d ${seconds}s`;
            } else {
                countdownText = `${minutes}d ${seconds}s`;
            }

            $('#appointmentCountdown').text(countdownText);
        }

        updateCountdown();
        setInterval(updateCountdown, 1000);
    }
});