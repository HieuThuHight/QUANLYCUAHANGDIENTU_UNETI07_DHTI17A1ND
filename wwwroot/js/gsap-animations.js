/**
 * UNETI STORE - GSAP Animation Controller
 * Built following official GreenSock Best Practices (greensock/gsap-skills)
 */

document.addEventListener('DOMContentLoaded', () => {
    // Check if GSAP is available
    if (typeof gsap === 'undefined') return;

    // Check prefers-reduced-motion
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (prefersReducedMotion) return;

    // Register ScrollTrigger if available
    if (typeof ScrollTrigger !== 'undefined') {
        gsap.registerPlugin(ScrollTrigger);
    }

    // ==========================================
    // 1. NAVBAR & BRAND ANIMATION
    // ==========================================
    gsap.from('.navbar-brand', {
        duration: 0.8,
        y: -20,
        opacity: 0,
        ease: 'power3.out'
    });

    gsap.from('.navbar-nav .nav-item', {
        duration: 0.6,
        y: -15,
        opacity: 0,
        stagger: 0.06,
        delay: 0.2,
        ease: 'power2.out'
    });

    // ==========================================
    // 2. HERO SECTION TIMELINE (Trang chủ)
    // ==========================================
    if (document.querySelector('.hero-wrapper')) {
        const heroTl = gsap.timeline({ defaults: { ease: 'power3.out' } });

        heroTl.from('.hero-badge', {
            duration: 0.6,
            y: 20,
            opacity: 0,
            scale: 0.9
        })
        .from('.hero-title', {
            duration: 0.8,
            y: 30,
            opacity: 0
        }, '-=0.3')
        .from('.hero-subtitle', {
            duration: 0.6,
            y: 20,
            opacity: 0
        }, '-=0.4')
        .from('.hero-btn-group .btn', {
            duration: 0.5,
            y: 20,
            opacity: 0,
            stagger: 0.1,
            scale: 0.95
        }, '-=0.3');
    }

    // ==========================================
    // 3. VALUE PROPOSITION / FEATURE CARDS
    // ==========================================
    if (document.querySelectorAll('.feature-box').length > 0) {
        gsap.from('.feature-box', {
            scrollTrigger: {
                trigger: '.feature-section',
                start: 'top 85%'
            },
            duration: 0.7,
            y: 40,
            opacity: 0,
            stagger: 0.15,
            ease: 'power2.out'
        });
    }

    // ==========================================
    // 4. PRODUCT CARDS STAGGER (Danh sách sản phẩm)
    // ==========================================
    if (document.querySelectorAll('.product-card').length > 0) {
        gsap.from('.product-card', {
            duration: 0.6,
            y: 30,
            opacity: 0,
            stagger: 0.08,
            ease: 'power2.out',
            clearProps: 'transform,opacity'
        });
    }

    // ==========================================
    // 5. PRODUCT DETAILS PAGE (Chi tiết sản phẩm)
    // ==========================================
    if (document.querySelector('.product-details-gallery')) {
        gsap.from('.product-details-gallery', {
            duration: 0.8,
            x: -40,
            opacity: 0,
            ease: 'power3.out'
        });

        gsap.from('.product-details-info > *', {
            duration: 0.5,
            x: 30,
            opacity: 0,
            stagger: 0.08,
            ease: 'power2.out'
        });
    }

    // ==========================================
    // 6. DASHBOARD COUNTER ANIMATION (Trang Admin)
    // ==========================================
    document.querySelectorAll('.stat-count').forEach(counter => {
        const rawTarget = counter.getAttribute('data-count') || counter.innerText;
        const targetValue = parseFloat(rawTarget.replace(/[^0-9.-]+/g, '')) || 0;
        const isCurrency = counter.getAttribute('data-is-currency') === 'true';

        const obj = { val: 0 };
        gsap.to(obj, {
            val: targetValue,
            duration: 1.6,
            ease: 'power2.out',
            onUpdate: () => {
                if (isCurrency) {
                    counter.innerText = Math.round(obj.val).toLocaleString('vi-VN') + ' đ';
                } else {
                    counter.innerText = Math.round(obj.val).toLocaleString('vi-VN');
                }
            }
        });
    });

    // ==========================================
    // 7. CART BADGE POPPING (Khi thêm giỏ hàng)
    // ==========================================
    window.animateCartBadge = function () {
        const badge = document.getElementById('cart-count');
        if (!badge) return;
        gsap.fromTo(badge, 
            { scale: 1 }, 
            { scale: 1.5, duration: 0.2, yoyo: true, repeat: 1, ease: 'back.out(2)' }
        );
    };

    // Global listener for '.btn-add-cart'
    document.querySelectorAll('.btn-add-cart').forEach(btn => {
        btn.addEventListener('click', () => {
            gsap.fromTo(btn, 
                { scale: 0.95 }, 
                { scale: 1, duration: 0.3, ease: 'elastic.out(1, 0.4)' }
            );
            setTimeout(window.animateCartBadge, 300);
        });
    });
});
