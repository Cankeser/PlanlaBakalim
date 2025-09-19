// ===== İŞLETME DETAY SAYFASI - SADECE UI KODLARI =====

document.addEventListener('DOMContentLoaded', function() {
    initTabs();
    
    // İlk tab'ı aktif yap
    const firstTab = document.querySelector('.tab-btn');
    if (firstTab) firstTab.click();
});

// Tab sistemi
function initTabs() {
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabPanes = document.querySelectorAll('.tab-pane');
    
    tabBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const targetTab = this.getAttribute('data-tab');
            
            // Aktif tab'ı değiştir
            tabBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            
            // Tab içeriğini göster
            tabPanes.forEach(pane => {
                pane.classList.remove('active');
                if (pane.id === targetTab) {
                    pane.classList.add('active');
                    
                    // Lazy initialization
                    if (targetTab === 'gallery') {
                        setTimeout(() => initGallery(), 100);
                    } else if (targetTab === 'employees') {
                        setTimeout(() => initEmployees(), 100);
                    } else if (targetTab === 'reviews') {
                        setTimeout(() => initReviews(), 100);
                    }
                }
            });
        });
    });
}

// Galeri işlemleri
function initGallery() {
    const galleryContainer = document.querySelector('#gallery .gallery-container');
    const galleryScroll = galleryContainer?.querySelector('.gallery-scroll');
    const prevBtn = galleryContainer?.querySelector('.gallery-prev');
    const nextBtn = galleryContainer?.querySelector('.gallery-next');
    
    if (!galleryContainer || !galleryScroll || !prevBtn || !nextBtn) return;
    
    // Dinamik scroll miktarı
    function getScrollAmount() {
        const firstItem = galleryScroll.querySelector('.gallery-item');
        if (firstItem) {
            return firstItem.offsetWidth + 16; // gap: 16px
        }
        return 200;
    }
    
    // Buton durumları
    function updateButtons() {
        const isAtStart = galleryScroll.scrollLeft === 0;
        const isAtEnd = galleryScroll.scrollLeft + galleryScroll.clientWidth >= galleryScroll.scrollWidth;
        
        prevBtn.style.opacity = isAtStart ? '0.5' : '1';
        prevBtn.style.pointerEvents = isAtStart ? 'none' : 'auto';
        nextBtn.style.opacity = isAtEnd ? '0.5' : '1';
        nextBtn.style.pointerEvents = isAtEnd ? 'none' : 'auto';
    }
    
    // Galeri resimlerine tıklama
    galleryScroll.addEventListener('click', (e) => {
        if (e.target.closest('.gallery-item img')) {
            const img = e.target.closest('.gallery-item img');
            showImageModal(img.src, img.alt);
        }
    });
    
    // Event listeners
    galleryScroll.addEventListener('scroll', updateButtons);
    prevBtn.addEventListener('click', (e) => {
        e.preventDefault();
        galleryScroll.scrollBy({ left: -getScrollAmount(), behavior: 'smooth' });
    });
    nextBtn.addEventListener('click', (e) => {
        e.preventDefault();
        galleryScroll.scrollBy({ left: getScrollAmount(), behavior: 'smooth' });
    });
    
    updateButtons();
}

// Ekip işlemleri
function initEmployees() {
    const employeesContainer = document.querySelector('#employees .employees-container');
    const employeesScroll = employeesContainer?.querySelector('.employees-scroll');
    const prevBtn = employeesContainer?.querySelector('.employees-prev');
    const nextBtn = employeesContainer?.querySelector('.employees-next');
    
    if (!employeesContainer || !employeesScroll || !prevBtn || !nextBtn) return;
    
    // Dinamik scroll miktarı
    function getScrollAmount() {
        const firstCard = employeesScroll.querySelector('.employee-card');
        if (firstCard) {
            return firstCard.offsetWidth + 24; // gap: 24px
        }
        return 264;
    }
    
    // Buton durumları
    function updateButtons() {
        const isAtStart = employeesScroll.scrollLeft === 0;
        const isAtEnd = employeesScroll.scrollLeft + employeesScroll.clientWidth >= employeesScroll.scrollWidth;
        
        prevBtn.style.opacity = isAtStart ? '0.5' : '1';
        prevBtn.style.pointerEvents = isAtStart ? 'none' : 'auto';
        nextBtn.style.opacity = isAtEnd ? '0.5' : '1';
        nextBtn.style.pointerEvents = isAtEnd ? 'none' : 'auto';
    }
    
    // Event listeners
    employeesScroll.addEventListener('scroll', updateButtons);
    prevBtn.addEventListener('click', (e) => {
        e.preventDefault();
        employeesScroll.scrollBy({ left: -getScrollAmount(), behavior: 'smooth' });
    });
    nextBtn.addEventListener('click', (e) => {
        e.preventDefault();
        employeesScroll.scrollBy({ left: getScrollAmount(), behavior: 'smooth' });
    });
    
    updateButtons();
}

// Yorumlar işlemleri
function initReviews() {
    const reviewsScroll = document.querySelector('.reviews-scroll');
    const prevBtn = document.querySelector('.reviews-prev');
    const nextBtn = document.querySelector('.reviews-next');
    
    if (!reviewsScroll || !prevBtn || !nextBtn) return;
    
    // Dinamik scroll miktarı
    function getScrollAmount() {
        const firstReview = reviewsScroll.querySelector('.review-item');
        if (firstReview) {
            return firstReview.offsetWidth + 20; // gap: 20px
        }
        return 300;
    }
    
    // Buton durumları
    function updateButtons() {
        const isAtStart = reviewsScroll.scrollLeft === 0;
        const isAtEnd = reviewsScroll.scrollLeft + reviewsScroll.clientWidth >= reviewsScroll.scrollWidth;
        
        prevBtn.style.opacity = isAtStart ? '0.5' : '1';
        prevBtn.style.pointerEvents = isAtStart ? 'none' : 'auto';
        nextBtn.style.opacity = isAtEnd ? '0.5' : '1';
        nextBtn.style.pointerEvents = isAtEnd ? 'none' : 'auto';
    }
    
    // Event listeners
    reviewsScroll.addEventListener('scroll', updateButtons);
    prevBtn.addEventListener('click', (e) => {
        e.preventDefault();
        reviewsScroll.scrollBy({ left: -getScrollAmount(), behavior: 'smooth' });
    });
    nextBtn.addEventListener('click', (e) => {
        e.preventDefault();
        reviewsScroll.scrollBy({ left: getScrollAmount(), behavior: 'smooth' });
    });
    
    updateButtons();
}



// Resim modal'ı
function showImageModal(src, alt) {
    const existingModal = document.querySelector('.image-modal');
    if (existingModal) existingModal.remove();
    
    const modal = document.createElement('div');
    modal.className = 'image-modal';
    modal.innerHTML = `
        <div class="image-modal-content">
            <img src="${src}" alt="${alt}">
            <button class="image-modal-close">×</button>
        </div>
    `;
    
    document.body.appendChild(modal);
    
    // Modal stilleri
    modal.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0,0,0,0.9);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 10000;
        animation: fadeIn 0.3s ease-out;
    `;
    
    const modalContent = modal.querySelector('.image-modal-content');
    modalContent.style.cssText = `
        position: relative;
        max-width: 90vw;
        max-height: 90vh;
        animation: zoomIn 0.3s ease-out;
    `;
    
    const modalImg = modal.querySelector('img');
    modalImg.style.cssText = `
        width: 100%;
        height: 100%;
        object-fit: contain;
        border-radius: 8px;
    `;
    
    const closeBtn = modal.querySelector('.image-modal-close');
    closeBtn.style.cssText = `
        position: absolute;
        top: -40px;
        right: 0;
        background: none;
        border: none;
        color: white;
        font-size: 1.5rem;
        cursor: pointer;
        padding: 0.5rem;
    `;
    
    // Kapatma işlevleri
    closeBtn.addEventListener('click', () => modal.remove());
    modal.addEventListener('click', (e) => {
        if (e.target === modal) modal.remove();
    });
    
    // CSS animasyonları
    if (!document.querySelector('#image-modal-styles')) {
        const style = document.createElement('style');
        style.id = 'image-modal-styles';
        style.textContent = `
            @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
            @keyframes zoomIn { from { transform: scale(0.8); opacity: 0; } to { transform: scale(1); opacity: 1; } }
        `;
        document.head.appendChild(style);
    }
    
    // ESC tuşu ile kapatma
    document.addEventListener('keydown', function closeOnEsc(e) {
        if (e.key === 'Escape') {
            modal.remove();
            document.removeEventListener('keydown', closeOnEsc);
        }
    });
}
