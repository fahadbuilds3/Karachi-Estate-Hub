function toggleMobileMenu() {
    var menu = document.getElementById('mobileMenu');
    if (menu) {
        menu.classList.toggle('open');
    }
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
