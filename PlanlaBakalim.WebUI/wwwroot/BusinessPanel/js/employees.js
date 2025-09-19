
document.addEventListener('DOMContentLoaded', function() {
setupCardFlipAnimations();
});   

function setupCardFlipAnimations() {
    const employeeCards = document.querySelectorAll('.employee-card');
    
    employeeCards.forEach(card => {
        card.addEventListener('click', function(e) {
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



