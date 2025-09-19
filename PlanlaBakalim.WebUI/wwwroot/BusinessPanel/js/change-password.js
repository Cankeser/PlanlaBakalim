// Change Password - Minimal JavaScript - UI Only

// Toggle Password Visibility
function togglePassword(inputId) {
    const input = document.getElementById(inputId);
    const icon = document.getElementById(inputId + 'Icon');
    
    if (input.type === 'password') {
        input.type = 'text';
        icon.className = 'bi bi-eye-slash';
    } else {
        input.type = 'password';
        icon.className = 'bi bi-eye';
    }
}

// Check Password Strength
function checkPasswordStrength(password) {
    let score = 0;
    let feedback = [];
    
    // Length check
    if (password.length >= 8) {
        score += 1;
        feedback.push('length');
    }
    
    // Uppercase check
    if (/[A-Z]/.test(password)) {
        score += 1;
        feedback.push('uppercase');
    }
    
    // Lowercase check
    if (/[a-z]/.test(password)) {
        score += 1;
        feedback.push('lowercase');
    }
    
    // Number check
    if (/\d/.test(password)) {
        score += 1;
        feedback.push('number');
    }
    
    // Special character check
    if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
        score += 1;
        feedback.push('special');
    }
    
    return { score, feedback };
}

// Update Password Requirements
function updateRequirements(feedback) {
    const requirements = ['length', 'uppercase', 'lowercase', 'number', 'special'];
    
    requirements.forEach(req => {
        const element = document.getElementById(req + 'Req');
        const icon = element.querySelector('i');
        
        if (feedback.includes(req)) {
            element.classList.add('valid');
            element.classList.remove('invalid');
            icon.className = 'bi bi-check-circle-fill';
        } else {
            element.classList.add('invalid');
            element.classList.remove('valid');
            icon.className = 'bi bi-circle';
        }
    });
}

// Update Strength Bar
function updateStrengthBar(score) {
    const strengthFill = document.getElementById('strengthFill');
    const strengthText = document.getElementById('strengthText');
    
    // Remove all classes
    strengthFill.className = 'strength-fill';
    
    if (score === 0) {
        strengthText.textContent = 'Şifre gücü';
    } else if (score <= 2) {
        strengthFill.classList.add('weak');
        strengthText.textContent = 'Zayıf';
    } else if (score <= 3) {
        strengthFill.classList.add('fair');
        strengthText.textContent = 'Orta';
    } else if (score <= 4) {
        strengthFill.classList.add('good');
        strengthText.textContent = 'İyi';
    } else {
        strengthFill.classList.add('strong');
        strengthText.textContent = 'Güçlü';
    }
}

// Validate Form
function validateForm() {
    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    
    if (!currentPassword || !newPassword || !confirmPassword) {
        alert('Lütfen tüm alanları doldurun');
        return false;
    }
    
    if (newPassword !== confirmPassword) {
        alert('Yeni şifreler eşleşmiyor');
        return false;
    }
    
    const strength = checkPasswordStrength(newPassword);
    if (strength.score < 3) {
        alert('Lütfen daha güçlü bir şifre seçin');
        return false;
    }
    
    return true;
}

// Show Success Modal
function showSuccessModal() {
    const successModal = document.getElementById('successModal');
    if (successModal) {
        successModal.classList.add('active');
    }
}

// Hide Success Modal
function hideSuccessModal() {
    const successModal = document.getElementById('successModal');
    if (successModal) {
        successModal.classList.remove('active');
    }
}

// Initialize UI Components
document.addEventListener('DOMContentLoaded', function() {
    // New password input listener
    const newPasswordInput = document.getElementById('newPassword');
    if (newPasswordInput) {
        newPasswordInput.addEventListener('input', function() {
            const strength = checkPasswordStrength(this.value);
            updateStrengthBar(strength.score);
            updateRequirements(strength.feedback);
        });
    }
    
    // Form submit listener
    const passwordForm = document.getElementById('passwordForm');
    if (passwordForm) {
        passwordForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            if (validateForm()) {
                // Here you would typically send data to backend
                console.log('Password change requested');
                
                // Show success modal
                showSuccessModal();
                
                // Reset form
                this.reset();
                
                // Reset strength bar and requirements
                updateStrengthBar(0);
                updateRequirements([]);
            }
        });
    }
});
