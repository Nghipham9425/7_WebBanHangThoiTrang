let currentSlide = 0; // Slide đầu tiên
const slides = document.querySelectorAll('.slide'); // Lấy tất cả các slide

function changeSlide(index) {
    slides[currentSlide].classList.remove('active'); // Ẩn slide hiện tại

    // Tính toán slide tiếp theo
    currentSlide = (currentSlide + index + slides.length) % slides.length;

    slides[currentSlide].classList.add('active'); // Hiện slide mới
}

// Hàm tự động chuyển slide mỗi 3 giây
function autoSlide() {
    changeSlide(1); // Chuyển đến slide tiếp theo
}

// Gọi hàm autoSlide mỗi 3 giây (3000ms)
setInterval(autoSlide, 3000);
