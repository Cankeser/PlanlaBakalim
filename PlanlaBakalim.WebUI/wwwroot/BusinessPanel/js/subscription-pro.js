// ===== SUBSCRIPTION PRO JAVASCRIPT - TEMİZLENMİŞ =====

// Plan Management
function selectPlan(planType) {
    const planNames = {
        'monthly': 'Aylık Plan',
        'yearly': 'Yıllık Plan'
    };
    
    if (confirm(`${planNames[planType]} planına geçmek istediğinizden emin misiniz?`)) {
        // Plan değiştirildi
    }
}

// Payment Management
function addPaymentMethod() {
    showModal('paymentModal');
}

function editPayment(paymentId) {
    showModal('editPaymentModal');
}

function showDeleteConfirm(paymentId) {
    window.currentPaymentId = paymentId;
    showModal('deleteConfirmModal');
}

function deletePayment(paymentId) {
    closeModal();
}

function setDefault(paymentId) {
    // Varsayılan ödeme yöntemi güncellendi
}

function savePayment() {
    const form = document.getElementById('paymentForm');
    const formData = new FormData(form);
    
    // Basic validation
    const cardNumber = formData.get('cardNumber') || document.querySelector('#paymentForm input[placeholder*="1234"]').value;
    const expiry = formData.get('expiry') || document.querySelector('#paymentForm input[placeholder*="MM/YY"]').value;
    const cvv = formData.get('cvv') || document.querySelector('#paymentForm input[placeholder*="123"]').value;
    const cardholder = formData.get('cardholder') || document.querySelector('#paymentForm input[placeholder*="Ad Soyad"]').value;
    
    if (!cardNumber || !expiry || !cvv || !cardholder) {
        return;
    }
    
    if (cardNumber.replace(/\s/g, '').length < 13) {
        return;
    }
    
    closeModal();
    resetForm();
}

function resetForm() {
    const form = document.getElementById('paymentForm');
    if (form) {
        form.reset();
    }
}

function updatePayment() {
    const form = document.getElementById('editPaymentForm');
    const formData = new FormData(form);
    
    // Basic validation
    const cardNumber = formData.get('cardNumber') || document.querySelector('#editPaymentForm input[placeholder*="1234"]').value;
    const expiry = formData.get('expiry') || document.querySelector('#editPaymentForm input[placeholder*="MM/YY"]').value;
    const cvv = formData.get('cvv') || document.querySelector('#editPaymentForm input[placeholder*="123"]').value;
    const cardholder = formData.get('cardholder') || document.querySelector('#editPaymentForm input[placeholder*="Ad Soyad"]').value;
    
    if (!cardNumber || !expiry || !cvv || !cardholder) {
        return;
    }
    
    if (cardNumber.replace(/\s/g, '').length < 13) {
        return;
    }
    
    closeModal();
    resetForm();
}

function confirmDelete() {
    if (window.currentPaymentId) {
        deletePayment(window.currentPaymentId);
    }
}

// Billing Management
function downloadInvoice(invoiceId) {
    // Fatura indiriliyor
}

function downloadAll() {
    // Tüm faturalar indiriliyor
}

// Modal Management
function showModal(modalId) {
    const modal = document.getElementById(modalId);
    const overlay = document.getElementById('overlay');
    
    if (modal && overlay) {
        modal.classList.add('show');
        overlay.style.display = 'block';
        document.body.style.overflow = 'hidden';
    }
}

function closeModal() {
    const modals = document.querySelectorAll('.modal');
    const overlay = document.getElementById('overlay');
    
    modals.forEach(modal => {
        modal.classList.remove('show');
    });
    
    if (overlay) {
        overlay.style.display = 'none';
    }
    
    document.body.style.overflow = 'auto';
}



// Message System - Kaldırıldı
function showMessage(message, type = 'info') {
    // Mesaj gösterme kaldırıldı
}

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    // Sayfa yüklendi
});
