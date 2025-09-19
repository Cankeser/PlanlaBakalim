document.addEventListener('DOMContentLoaded', function () {
    // Gün toggle işlemleri
    initializeDayToggles();

    // Kaydet butonları
    initializeSaveButtons();
});

function initializeDayToggles() {
    const dayCards = document.querySelectorAll('.day-card');

    dayCards.forEach(card => {
        const toggle = card.querySelector('.toggle-switch input');
        const timeInputs = card.querySelectorAll('.time-input');
        const toggleText = card.querySelector('.toggle-text');

        toggle.addEventListener('change', function () {
            if (this.checked) {
                card.classList.add('active');
                toggleText.textContent = 'Açık';
                timeInputs.forEach(input => input.disabled = false);
            } else {
                card.classList.remove('active');
                toggleText.textContent = 'Kapalı';
                timeInputs.forEach(input => input.disabled = true);
            }
        });
    });
}
function initializeSaveButtons() {
    document.querySelectorAll(".save-day-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            const card = this.closest(".day-card");
            const id = card.getAttribute("data-id");

            // TimeSpan formatına çevir: hh:mm:ss
            const openTime = card.querySelector(".open-time").value + ":00";
            const closeTime = card.querySelector(".close-time").value + ":00";

            const isOpen = card.querySelector(".toggle-switch input").checked;

            fetch("/BusinessPanel/WorkingHours/UpdateDay", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    Id: parseInt(id),
                    IsOpen: isOpen,
                    OpenTime: openTime,
                    CloseTime: closeTime
                })
            })
                .then(res => res.json())
                .then(result => {
                    if (result.success) {
                        Swal.fire({
                            toast: true,
                            position: 'top-end',
                            icon: 'success',
                            title: result.message,
                            showConfirmButton: false,
                            timer: 800,
                            timerProgressBar: true,
                            didClose: () => {
                                location.reload(); // Toast kapandıktan sonra sayfayı yeniler
                            }
                        });
                    } else {
                        Swal.fire({
                            toast: true,
                            position: 'top-end',
                            icon: 'error',
                            title: result.message,
                            showConfirmButton: false,
                            timer: 2000,
                            timerProgressBar: true
                        });
                    }
                })


                .catch(err => {
                    console.error(err);
                    Swal.fire("Sunucu hatası", "Lütfen tekrar deneyin", "error");
                });
        });
    });
}