// ===== LOGIN PAGE JAVASCRIPT - UI ONLY =====

document.addEventListener('DOMContentLoaded', function() {
    // DOM Elements
    const loginForm = document.getElementById('loginForm');
    const passwordInput = document.getElementById('Password');
    const passwordToggle = document.getElementById('passwordToggle');
    const rememberMeCheckbox = document.getElementById('RememberMe');

    // ===== PASSWORD TOGGLE FUNCTIONALITY =====
    passwordToggle.addEventListener('click', function() {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);
        
        const icon = this.querySelector('i');
        icon.className = type === 'password' ? 'bi bi-eye' : 'bi bi-eye-slash';
    });
});
