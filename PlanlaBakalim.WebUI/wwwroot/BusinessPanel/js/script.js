let currentTheme = localStorage.getItem('theme') || 'dark';

document.addEventListener('DOMContentLoaded', function () {
    initializeApp();
    loadTheme();
    setupCardFlipAnimations();
});
function setupCardFlipAnimations() {
    const employeeCards = document.querySelectorAll('.employee-card');

    employeeCards.forEach(card => {
        card.addEventListener('click', function (e) {
            // Eğer tıklanan element bir buton ise kartı çevirme
            if (e.target.closest('.employee-actions') ||
                e.target.closest('.back-btn')) {
                return;
            }

            const cardInner = this.querySelector('.card-inner');
            cardInner.style.transform = cardInner.style.transform === 'rotateY(180deg)'
                ? 'rotateY(0deg)'
                : 'rotateY(180deg)';
        });
    });
}
function initializeApp() {
    loadTheme();
}

function toggleTheme() {
    currentTheme = currentTheme === 'dark' ? 'light' : 'dark';
    document.documentElement.setAttribute('data-theme', currentTheme);
    localStorage.setItem('theme', currentTheme);
    updateThemeIcon();
}


function loadTheme() {
    const savedTheme = localStorage.getItem('theme');
    currentTheme = savedTheme || 'light';
    document.documentElement.setAttribute('data-theme', currentTheme);
    updateThemeIcon();
}

function updateThemeIcon() {
    const themeIcon = document.getElementById('theme-icon');

    if (themeIcon) {
        if (currentTheme === 'dark') {
            themeIcon.className = 'bi bi-sun-fill';
        } else {
            themeIcon.className = 'bi bi-moon-fill';
        }
    }
}

function showMoreMenu() {
    const actionSheet = document.getElementById('moreMenu');
    const overlay = document.getElementById('overlay');

    if (actionSheet && overlay) {
        actionSheet.classList.add('show');
        overlay.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
}

function hideMoreMenu() {
    const actionSheet = document.getElementById('moreMenu');
    const overlay = document.getElementById('overlay');

    if (actionSheet && overlay) {
        actionSheet.classList.remove('show');
        overlay.classList.remove('active');
        document.body.style.overflow = '';
    }
}
function showNotifications() {
    // Mobilde direkt bildirim sayfasına yönlendir
    if (window.innerWidth <= 768) {
        window.location.href = '/Business-panel/notifications.html';
        return;
    }

    // Desktop'ta modal göster
    const notificationModal = document.getElementById('notificationModal');
    if (notificationModal) {
        notificationModal.classList.add('show');
        document.body.style.overflow = 'hidden';
        // Bildirimleri yükle
        loadNotifications();

        // Boşluğa tıklayınca modal kapanması için event listener ekle
        notificationModal.addEventListener('click', function (e) {
            if (e.target === notificationModal) {
                closeNotificationModal();
            }
        });
    }
}

function closeNotificationModal() {
    const notificationModal = document.getElementById('notificationModal');
    if (notificationModal) {
        notificationModal.classList.remove('show');
        document.body.style.overflow = '';
    }
}


window.toggleTheme = toggleTheme;
window.showMoreMenu = showMoreMenu;
window.hideMoreMenu = hideMoreMenu;
window.showNotifications = showNotifications;
window.closeNotificationModal = closeNotificationModal;