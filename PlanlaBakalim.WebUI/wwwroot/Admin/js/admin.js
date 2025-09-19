// Modern Admin Panel JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Enhanced Mobile menu toggle
    const mobileMenuToggle = document.getElementById('mobileMenuToggle');
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    
    if (mobileMenuToggle && sidebar) {
        mobileMenuToggle.addEventListener('click', toggleSidebar);
        
        // Add touch feedback
        mobileMenuToggle.addEventListener('touchstart', function() {
            this.style.transform = 'scale(0.95)';
        });
        
        mobileMenuToggle.addEventListener('touchend', function() {
            this.style.transform = 'scale(1)';
        });
    }
    
    // Close sidebar when clicking overlay
    if (overlay) {
        overlay.addEventListener('click', closeSidebar);
    }
    
    // Enhanced mobile interactions
    initializeMobileInteractions();
    
    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 768) {
            const sidebar = document.getElementById('sidebar');
            const mobileToggle = document.getElementById('mobileMenuToggle');
            
            if (sidebar && sidebar.classList.contains('open')) {
                if (!sidebar.contains(e.target) && !mobileToggle.contains(e.target)) {
                    closeSidebar();
                }
            }
        }
    });
    
    // Handle window resize
    window.addEventListener('resize', handleWindowResize);
    
    // Initialize swipe gestures for mobile
    initializeSwipeGestures();
    
    // Auto-hide alerts
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert.classList.contains('show')) {
                alert.classList.remove('show');
                setTimeout(() => alert.remove(), 300);
            }
        }, 5000);
    });
    
    // Active menu item highlighting
    highlightActiveMenuItem();
    
    // Image upload functionality
    initializeImageUpload();
    
    // Form validation
    initializeFormValidation();
    
    // Table interactions
    initializeTableInteractions();
});

// Toggle sidebar for mobile
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    
    if (sidebar && overlay) {
        const isOpen = sidebar.classList.contains('open');
        
        if (isOpen) {
            // Close sidebar
            sidebar.classList.remove('open');
            overlay.classList.remove('show');
            document.body.style.overflow = '';
        } else {
            // Open sidebar
            sidebar.classList.add('open');
            overlay.classList.add('show');
            document.body.style.overflow = 'hidden';
        }
    }
}

// Close sidebar when clicking overlay
function closeSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    
    if (sidebar && overlay) {
        sidebar.classList.remove('open');
        overlay.classList.remove('show');
        document.body.style.overflow = '';
    }
}

// Initialize enhanced mobile interactions
function initializeMobileInteractions() {
    // Add touch feedback to buttons
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('touchstart', function() {
            this.style.transform = 'scale(0.98)';
            this.style.transition = 'transform 0.1s ease';
        });
        
        button.addEventListener('touchend', function() {
            this.style.transform = 'scale(1)';
        });
    });
    
    // Add touch feedback to cards
    const cards = document.querySelectorAll('.card, .stat-card');
    cards.forEach(card => {
        card.addEventListener('touchstart', function() {
            this.style.transform = 'scale(0.99)';
            this.style.transition = 'transform 0.1s ease';
        });
        
        card.addEventListener('touchend', function() {
            this.style.transform = 'scale(1)';
        });
    });
    
    // Optimize table scrolling on mobile
    const tables = document.querySelectorAll('.table-responsive');
    tables.forEach(table => {
        table.style.webkitOverflowScrolling = 'touch';
        table.style.overflowX = 'auto';
    });
    
    // Add pull-to-refresh functionality
    initializePullToRefresh();
}

// Handle window resize
function handleWindowResize() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    
    if (window.innerWidth > 768) {
        // Desktop view - close mobile sidebar
        if (sidebar && sidebar.classList.contains('open')) {
            sidebar.classList.remove('open');
            if (overlay) overlay.classList.remove('show');
            document.body.style.overflow = '';
        }
    }
}

// Initialize swipe gestures
function initializeSwipeGestures() {
    let startX = 0;
    let startY = 0;
    let endX = 0;
    let endY = 0;
    
    document.addEventListener('touchstart', function(e) {
        startX = e.touches[0].clientX;
        startY = e.touches[0].clientY;
    });
    
    document.addEventListener('touchend', function(e) {
        endX = e.changedTouches[0].clientX;
        endY = e.changedTouches[0].clientY;
        
        const diffX = startX - endX;
        const diffY = startY - endY;
        
        // Swipe right to open sidebar (only on mobile)
        if (window.innerWidth <= 768 && diffX < -50 && Math.abs(diffY) < 100) {
            const sidebar = document.getElementById('sidebar');
            if (sidebar && !sidebar.classList.contains('open')) {
                toggleSidebar();
            }
        }
        
        // Swipe left to close sidebar
        if (diffX > 50 && Math.abs(diffY) < 100) {
            const sidebar = document.getElementById('sidebar');
            if (sidebar && sidebar.classList.contains('open')) {
                closeSidebar();
            }
        }
    });
}

// Initialize pull-to-refresh
function initializePullToRefresh() {
    let startY = 0;
    let currentY = 0;
    let isPulling = false;
    let pullDistance = 0;
    
    const pullThreshold = 80;
    const refreshIndicator = createRefreshIndicator();
    
    document.addEventListener('touchstart', function(e) {
        if (window.scrollY === 0) {
            startY = e.touches[0].clientY;
            isPulling = true;
        }
    });
    
    document.addEventListener('touchmove', function(e) {
        if (isPulling && window.scrollY === 0) {
            currentY = e.touches[0].clientY;
            pullDistance = currentY - startY;
            
            if (pullDistance > 0) {
                e.preventDefault();
                refreshIndicator.style.transform = `translateY(${Math.min(pullDistance * 0.5, pullThreshold)}px)`;
                refreshIndicator.style.opacity = Math.min(pullDistance / pullThreshold, 1);
            }
        }
    });
    
    document.addEventListener('touchend', function() {
        if (isPulling && pullDistance > pullThreshold) {
            // Trigger refresh
            refreshIndicator.style.transform = 'translateY(0)';
            refreshIndicator.style.opacity = '0';
            location.reload();
        } else {
            refreshIndicator.style.transform = 'translateY(0)';
            refreshIndicator.style.opacity = '0';
        }
        
        isPulling = false;
        pullDistance = 0;
    });
}

// Create refresh indicator
function createRefreshIndicator() {
    const indicator = document.createElement('div');
    indicator.innerHTML = '<i class="fas fa-sync-alt"></i> Yenilemek için çekin';
    indicator.style.cssText = `
        position: fixed;
        top: -50px;
        left: 50%;
        transform: translateX(-50%);
        background: var(--admin-primary);
        color: white;
        padding: 0.75rem 1.5rem;
        border-radius: 0 0 1rem 1rem;
        font-size: 0.875rem;
        font-weight: 500;
        z-index: 1000;
        transition: all 0.3s ease;
        opacity: 0;
        box-shadow: var(--admin-shadow-lg);
    `;
    
    document.body.appendChild(indicator);
    return indicator;
}

// Highlight active menu item
function highlightActiveMenuItem() {
    const currentPath = window.location.pathname;
    const menuLinks = document.querySelectorAll('.sidebar-menu a');
    
    menuLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });
}

// Modern Toast Notification System
function showAlert(message, type = 'info') {
    // Create toast container if it doesn't exist
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        toastContainer.style.zIndex = '9999';
        document.body.appendChild(toastContainer);
    }
    
    // Create toast element
    const toastId = 'toast-' + Date.now();
    const toastDiv = document.createElement('div');
    toastDiv.id = toastId;
    toastDiv.className = `toast align-items-center text-white bg-${getToastBgColor(type)} border-0`;
    toastDiv.setAttribute('role', 'alert');
    toastDiv.setAttribute('aria-live', 'assertive');
    toastDiv.setAttribute('aria-atomic', 'true');
    
    // Add special effects for success messages
    const isSuccess = type === 'success';
    const iconClass = isSuccess ? 'fas fa-check-circle' : `fas fa-${getAlertIcon(type)}`;
    const extraClass = isSuccess ? 'success-pulse' : '';
    
    toastDiv.innerHTML = `
        <div class="d-flex">
            <div class="toast-body d-flex align-items-center">
                <i class="${iconClass} me-2 ${extraClass}"></i>
                <span>${message}</span>
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;
    
    toastContainer.appendChild(toastDiv);
    
    // Initialize and show toast
    const toast = new bootstrap.Toast(toastDiv, {
        autohide: true,
        delay: isSuccess ? 3000 : 4000
    });
    
    toast.show();
    
    // Add success animation
    if (isSuccess) {
        setTimeout(() => {
            const icon = toastDiv.querySelector('.success-pulse');
            if (icon) {
                icon.style.animation = 'pulse 0.6s ease-in-out';
            }
        }, 100);
    }
    
    // Click to dismiss toast
    toastDiv.addEventListener('click', (e) => {
        if (e.target !== toastDiv.querySelector('.btn-close')) {
            toast.hide();
        }
    });
    
    // Remove toast element after it's hidden
    toastDiv.addEventListener('hidden.bs.toast', () => {
        toastDiv.remove();
    });
    
    // Add haptic feedback for mobile devices
    if (navigator.vibrate && isSuccess) {
        navigator.vibrate([100, 50, 100]);
    }
}

// Get toast background color based on type
function getToastBgColor(type) {
    const colors = {
        'success': 'success',
        'danger': 'danger',
        'warning': 'warning',
        'info': 'info'
    };
    return colors[type] || 'info';
}

// Get alert icon based on type
function getAlertIcon(type) {
    const icons = {
        'success': 'check-circle',
        'danger': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle'
    };
    return icons[type] || 'info-circle';
}

// Confirm action function (legacy - will be replaced)
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Modern Confirmation Modal System
function showConfirmation(options) {
    const {
        title = 'Onay Gerekli',
        message = 'Bu işlemi gerçekleştirmek istediğinizden emin misiniz?',
        confirmText = 'Evet',
        cancelText = 'Hayır',
        type = 'warning',
        onConfirm = () => {},
        onCancel = () => {}
    } = options;

    // Create modal container if it doesn't exist
    let modalContainer = document.getElementById('confirmation-modal-container');
    if (!modalContainer) {
        modalContainer = document.createElement('div');
        modalContainer.id = 'confirmation-modal-container';
        modalContainer.className = 'modal fade';
        modalContainer.setAttribute('tabindex', '-1');
        modalContainer.setAttribute('aria-hidden', 'true');
        document.body.appendChild(modalContainer);
    }

    // Get icon and color based on type
    const iconClass = getConfirmationIcon(type);
    const colorClass = getConfirmationColor(type);

    modalContainer.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow-lg">
                <div class="modal-body text-center p-4">
                    <div class="mb-3">
                        <i class="fas ${iconClass} ${colorClass}" style="font-size: 3rem;"></i>
                    </div>
                    <h5 class="modal-title mb-3">${title}</h5>
                    <p class="text-muted mb-4">${message}</p>
                    <div class="d-flex gap-2 justify-content-center">
                        <button type="button" class="btn btn-light btn-lg px-4" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>${cancelText}
                        </button>
                        <button type="button" class="btn ${getConfirmationButtonClass(type)} btn-lg px-4" id="confirm-action">
                            <i class="fas fa-check me-2"></i>${confirmText}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Initialize modal
    const modal = new bootstrap.Modal(modalContainer);
    
    // Add event listeners
    const confirmBtn = modalContainer.querySelector('#confirm-action');
    confirmBtn.addEventListener('click', () => {
        modal.hide();
        onConfirm();
    });

    modalContainer.addEventListener('hidden.bs.modal', () => {
        onCancel();
    });

    // Show modal
    modal.show();
}

// Get confirmation icon based on type
function getConfirmationIcon(type) {
    const icons = {
        'warning': 'exclamation-triangle',
        'danger': 'exclamation-circle',
        'info': 'info-circle',
        'success': 'check-circle'
    };
    return icons[type] || 'exclamation-triangle';
}

// Get confirmation color based on type
function getConfirmationColor(type) {
    const colors = {
        'warning': 'text-warning',
        'danger': 'text-danger',
        'info': 'text-info',
        'success': 'text-success'
    };
    return colors[type] || 'text-warning';
}

// Get confirmation button class based on type
function getConfirmationButtonClass(type) {
    const classes = {
        'warning': 'btn-warning',
        'danger': 'btn-danger',
        'info': 'btn-info',
        'success': 'btn-success'
    };
    return classes[type] || 'btn-warning';
}

// Format date function
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Format number function
function formatNumber(number) {
    return new Intl.NumberFormat('tr-TR').format(number);
}

// Initialize image upload functionality
function initializeImageUpload() {
    const uploadAreas = document.querySelectorAll('.image-upload-area');
    
    uploadAreas.forEach(area => {
        const fileInput = area.querySelector('input[type="file"]');
        if (!fileInput) return;
        
        // Drag and drop events
        area.addEventListener('dragover', (e) => {
            e.preventDefault();
            area.classList.add('dragover');
        });
        
        area.addEventListener('dragleave', () => {
            area.classList.remove('dragover');
        });
        
        area.addEventListener('drop', (e) => {
            e.preventDefault();
            area.classList.remove('dragover');
            
            const files = e.dataTransfer.files;
            if (files.length > 0) {
                handleFileUpload(files[0], fileInput);
            }
        });
        
        // Click to upload
        area.addEventListener('click', () => {
            fileInput.click();
        });
        
        // File input change
        fileInput.addEventListener('change', (e) => {
            if (e.target.files.length > 0) {
                handleFileUpload(e.target.files[0], fileInput);
            }
        });
    });
}

// Handle file upload
function handleFileUpload(file, fileInput) {
    // Validate file type
    if (!file.type.startsWith('image/')) {
        showAlert('Lütfen sadece resim dosyası seçin.', 'danger');
        return;
    }
    
    // Validate file size (5MB limit)
    if (file.size > 5 * 1024 * 1024) {
        showAlert('Dosya boyutu 5MB\'dan küçük olmalıdır.', 'danger');
        return;
    }
    
    // Show preview
    const reader = new FileReader();
    reader.onload = (e) => {
        const preview = fileInput.parentElement.querySelector('.image-preview');
        if (preview) {
            preview.src = e.target.result;
            preview.style.display = 'block';
        }
    };
    reader.readAsDataURL(file);
    
    // Upload file via AJAX
    uploadFile(file);
}

// Upload file via AJAX
function uploadFile(file) {
    const formData = new FormData();
    formData.append('file', file);
    
    fetch('/Admin/Gallery/Upload', {
        method: 'POST',
        body: formData,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showAlert('Resim başarıyla yüklendi.', 'success');
            // Update hidden input with file path
            const hiddenInput = document.querySelector('input[name="ImagePath"]');
            if (hiddenInput) {
                hiddenInput.value = data.filePath;
            }
        } else {
            showAlert(data.message || 'Resim yüklenirken hata oluştu.', 'danger');
        }
    })
    .catch(error => {
        console.error('Upload error:', error);
        showAlert('Resim yüklenirken hata oluştu.', 'danger');
    });
}

// Initialize form validation
function initializeFormValidation() {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        form.addEventListener('submit', (e) => {
            if (!validateForm(form)) {
                e.preventDefault();
            }
        });
    });
}

// Validate form
function validateForm(form) {
    let isValid = true;
    const requiredFields = form.querySelectorAll('[required]');
    
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            isValid = false;
            field.classList.add('is-invalid');
            showFieldError(field, 'Bu alan zorunludur.');
        } else {
            field.classList.remove('is-invalid');
            clearFieldError(field);
        }
    });
    
    return isValid;
}

// Show field error
function showFieldError(field, message) {
    clearFieldError(field);
    
    const errorDiv = document.createElement('div');
    errorDiv.className = 'invalid-feedback';
    errorDiv.textContent = message;
    
    field.parentElement.appendChild(errorDiv);
}

// Clear field error
function clearFieldError(field) {
    const existingError = field.parentElement.querySelector('.invalid-feedback');
    if (existingError) {
        existingError.remove();
    }
}

// Initialize table interactions
function initializeTableInteractions() {
    const tables = document.querySelectorAll('.table');
    
    tables.forEach(table => {
        // Add hover effects
        const rows = table.querySelectorAll('tbody tr');
        rows.forEach(row => {
            row.addEventListener('mouseenter', () => {
                row.style.backgroundColor = '#f8fafc';
            });
            
            row.addEventListener('mouseleave', () => {
                row.style.backgroundColor = '';
            });
        });
    });
}

// Update system status
function updateSystemStatus() {
    fetch('/Admin/Settings/SystemStatus')
        .then(response => response.json())
        .then(data => {
            const statusElement = document.querySelector('.system-status');
            if (statusElement) {
                statusElement.className = `system-status badge badge-${data.status === 'online' ? 'success' : 'danger'}`;
                statusElement.textContent = data.status === 'online' ? 'Çevrimiçi' : 'Çevrimdışı';
            }
        })
        .catch(error => {
            console.error('Status update error:', error);
        });
}

// Delete confirmation
function confirmDelete(itemName, deleteUrl) {
    confirmAction(`${itemName} silmek istediğinizden emin misiniz?`, () => {
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = deleteUrl;
        
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (token) {
            form.appendChild(token.cloneNode());
        }
        
        document.body.appendChild(form);
        form.submit();
    });
}

// Toggle status
function toggleStatus(itemId, currentStatus, toggleUrl) {
    const newStatus = currentStatus ? false : true;
    const statusText = newStatus ? 'aktif' : 'pasif';
    
    confirmAction(`Bu öğeyi ${statusText} yapmak istediğinizden emin misiniz?`, () => {
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = toggleUrl;
        
        const statusInput = document.createElement('input');
        statusInput.type = 'hidden';
        statusInput.name = 'status';
        statusInput.value = newStatus;
        form.appendChild(statusInput);
        
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (token) {
            form.appendChild(token.cloneNode());
        }
        
        document.body.appendChild(form);
        form.submit();
    });
}

// Export data
function exportData(format, url) {
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = url;
    
    const formatInput = document.createElement('input');
    formatInput.type = 'hidden';
    formatInput.name = 'format';
    formatInput.value = format;
    form.appendChild(formatInput);
    
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    if (token) {
        form.appendChild(token.cloneNode());
    }
    
    document.body.appendChild(form);
    form.submit();
}

// Search functionality
function initializeSearch() {
    const searchInputs = document.querySelectorAll('.search-input');
    
    searchInputs.forEach(input => {
        input.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            const table = e.target.closest('.card').querySelector('.table');
            
            if (table) {
                const rows = table.querySelectorAll('tbody tr');
                
                rows.forEach(row => {
                    const text = row.textContent.toLowerCase();
                    if (text.includes(searchTerm)) {
                        row.style.display = '';
                    } else {
                        row.style.display = 'none';
                    }
                });
            }
        });
    });
}

// Initialize search on page load
document.addEventListener('DOMContentLoaded', initializeSearch);

// Global admin panel object
window.AdminPanel = {
    showAlert,
    confirmAction,
    formatDate,
    formatNumber,
    updateSystemStatus,
    confirmDelete,
    toggleStatus,
    exportData,
    showConfirmation
};

