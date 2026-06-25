function toggleMobileMenu() {
    var menu = document.getElementById('mobileMenu');
    if (menu) {
        menu.classList.toggle('open');
        document.body.classList.toggle('mobile-menu-open', menu.classList.contains('open'));
    }
}

function closeMobileMenu() {
    var menu = document.getElementById('mobileMenu');
    if (menu) {
        menu.classList.remove('open');
    }
    document.body.classList.remove('mobile-menu-open');
}

function switchTab(btn) {
    var tabs = btn.closest('.search-tabs');
    if (!tabs) {
        return;
    }

    tabs.querySelectorAll('.search-tab').forEach(function (tab) {
        tab.classList.remove('active');
    });
    btn.classList.add('active');
}

function selectRole(el) {
    var grid = el.closest('.role-grid');
    if (!grid) {
        return;
    }

    grid.querySelectorAll('.role-option').forEach(function (role) {
        role.classList.remove('active');
    });
    el.classList.add('active');

    var input = el.querySelector('input[type="radio"]');
    if (input) {
        input.checked = true;
    }
}

function selectPurpose(el) {
    var group = el.closest('.purpose-btns');
    if (!group) {
        return;
    }

    group.querySelectorAll('.purpose-btn').forEach(function (purpose) {
        purpose.classList.remove('active');
    });
    el.classList.add('active');

    var input = el.querySelector('input[type="radio"]');
    if (input) {
        input.checked = true;
    }
}

function previewPropertyImages(input, targetId) {
    var target = document.getElementById(targetId);
    if (!target) {
        return;
    }

    target.innerHTML = '';
    if (!input.files || !input.files.length) {
        return;
    }

    Array.prototype.forEach.call(input.files, function (file) {
        if (!file.type || file.type.indexOf('image/') !== 0) {
            return;
        }

        var reader = new FileReader();
        reader.onload = function (event) {
            var item = document.createElement('div');
            item.className = 'gallery-thumb';
            item.style.backgroundImage = 'url(' + event.target.result + ')';
            item.style.backgroundSize = 'cover';
            item.style.backgroundPosition = 'center';
            target.appendChild(item);
        };
        reader.readAsDataURL(file);
    });
}

// function initPropertyGallery() {
//     var galleryRoot = document.querySelector('[data-gallery-root]');

//     if (!galleryRoot || galleryRoot.getAttribute('data-gallery-bound') === 'true') {
//         return;
//     }

//     var mainImage = galleryRoot.querySelector('[data-gallery-main]');
//     var thumbsWrapper = document.querySelector('.gallery-thumbs');
//     var thumbs = thumbsWrapper ? Array.prototype.slice.call(thumbsWrapper.querySelectorAll('[data-gallery-thumb]')) : [];

//     if (!mainImage || !thumbs.length) {
//         return;
//     }

//     galleryRoot.setAttribute('data-gallery-bound', 'true');

//     var images = thumbs.map(function (thumb) {
//         return {
//             src: thumb.getAttribute('data-gallery-src'),
//             alt: thumb.getAttribute('data-gallery-alt') || ''
//         };
//     }).filter(function (image) {
//         return !!image.src;
//     });

//     var counter = galleryRoot.querySelector('[data-gallery-counter]');
//     var prev = galleryRoot.querySelector('[data-gallery-prev]');
//     var next = galleryRoot.querySelector('[data-gallery-next]');
//     var currentIndex = parseInt(mainImage.getAttribute('data-gallery-index'), 10) || 0;

//     if (images.length <= 1) {
//         if (prev) {
//             prev.style.display = 'none';
//         }

//         if (next) {
//             next.style.display = 'none';
//         }

//         if (counter) {
//             counter.style.display = 'none';
//         }
//     }

//     function showImage(index, shouldScrollThumb) {
//         if (!images.length) {
//             return;
//         }

//         index = ((index % images.length) + images.length) % images.length;

//         var selected = thumbs[index];
//         var image = images[index];

//         if (!selected || !image || !image.src) {
//             return;
//         }

//         mainImage.style.display = 'block';
//         mainImage.src = image.src;
//         mainImage.alt = image.alt;
//         mainImage.setAttribute('data-gallery-index', index);
//         currentIndex = index;

//         thumbs.forEach(function (thumb) {
//             thumb.classList.remove('active');
//         });
//         selected.classList.add('active');

//         if (counter) {
//             counter.textContent = (index + 1) + ' / ' + images.length;
//         }

//         if (shouldScrollThumb && selected.scrollIntoView) {
//             selected.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'nearest' });
//         }
//     }

//     thumbs.forEach(function (thumb, index) {
//         thumb.addEventListener('click', function (event) {
//             event.preventDefault();
//             showImage(index, true);
//         });
//     });

//     if (prev) {
//         prev.addEventListener('click', function (event) {
//             event.preventDefault();
//             event.stopPropagation();
//             showImage(currentIndex - 1, true);
//         });
//     }

//     if (next) {
//         next.addEventListener('click', function (event) {
//             event.preventDefault();
//             event.stopPropagation();
//             showImage(currentIndex + 1, true);
//         });
//     }

//     showImage(currentIndex, false);
// }


function initPropertyGallery() {
    var galleryRoot = document.querySelector('[data-gallery-root]');
    if (!galleryRoot) {
        return;
    }

    var mainImage = galleryRoot.querySelector('[data-gallery-main]');
    var prevBtn = galleryRoot.querySelector('[data-gallery-prev]');
    var nextBtn = galleryRoot.querySelector('[data-gallery-next]');
    var counter = galleryRoot.querySelector('[data-gallery-counter]');

    var thumbsWrapper = galleryRoot.parentElement
        ? galleryRoot.parentElement.querySelector('.gallery-thumbs')
        : document.querySelector('.gallery-thumbs');

    var thumbs = thumbsWrapper
        ? Array.prototype.slice.call(thumbsWrapper.querySelectorAll('[data-gallery-thumb]'))
        : [];

    if (!mainImage || thumbs.length === 0) {
        return;
    }

    var images = thumbs.map(function (thumb) {
        return {
            src: thumb.getAttribute('data-gallery-src'),
            alt: thumb.getAttribute('data-gallery-alt') || ''
        };
    }).filter(function (img) {
        return img.src && img.src.trim() !== '';
    });

    if (images.length === 0) {
        return;
    }

    var currentIndex = parseInt(mainImage.getAttribute('data-gallery-index'), 10);
    if (isNaN(currentIndex)) {
        currentIndex = 0;
    }

    function updateGallery(index) {
        if (index < 0) {
            index = images.length - 1;
        }

        if (index >= images.length) {
            index = 0;
        }

        currentIndex = index;

        mainImage.src = images[currentIndex].src;
        mainImage.alt = images[currentIndex].alt;
        mainImage.setAttribute('data-gallery-index', currentIndex);

        thumbs.forEach(function (thumb, i) {
            thumb.classList.toggle('active', i === currentIndex);
        });

        if (counter) {
            counter.textContent = (currentIndex + 1) + ' / ' + images.length;
        }
    }

    thumbs.forEach(function (thumb, index) {
        thumb.onclick = function (event) {
            event.preventDefault();
            updateGallery(index);
        };
    });

    if (prevBtn) {
        prevBtn.onclick = function (event) {
            event.preventDefault();
            event.stopPropagation();
            updateGallery(currentIndex - 1);
        };
    }

    if (nextBtn) {
        nextBtn.onclick = function (event) {
            event.preventDefault();
            event.stopPropagation();
            updateGallery(currentIndex + 1);
        };
    }

    if (images.length <= 1) {
        if (prevBtn) prevBtn.style.display = 'none';
        if (nextBtn) nextBtn.style.display = 'none';
        if (counter) counter.style.display = 'none';
    }

    updateGallery(currentIndex);
}

document.addEventListener('click', function (event) {
    var menu = document.getElementById('mobileMenu');
    var nav = document.getElementById('mainNav');

    if (!menu || !menu.classList.contains('open')) {
        return;
    }

    if ((nav && nav.contains(event.target)) || menu.contains(event.target)) {
        return;
    }

    closeMobileMenu();
});

document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeMobileMenu();
    }
});

window.addEventListener('resize', function () {
    if (window.innerWidth > 1024) {
        closeMobileMenu();
    }
});

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initPropertyGallery);
} else {
    initPropertyGallery();
}
