(function () {
    const form = document.getElementById('search-form');
    if (!form) return;

    const checkIn = form.querySelector('[name="CheckIn"]');
    const checkOut = form.querySelector('[name="CheckOut"]');
    const submit = form.querySelector('[data-search-submit]');

    const toIsoDate = (date) => {
        const pad = (n) => String(n).padStart(2, '0');
        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
    };

    const dayAfter = (isoDate) => {
        const date = new Date(`${isoDate}T00:00:00`);
        date.setDate(date.getDate() + 1);
        return toIsoDate(date);
    };

    checkIn.addEventListener('change', () => {
        checkOut.min = checkIn.value;
        if (checkOut.value <= checkIn.value) {
            checkOut.value = dayAfter(checkIn.value);
        }
    });

    form.addEventListener('submit', () => {
        submit.disabled = true;
        submit.querySelector('span').textContent = 'Aranıyor...';
    });
})();
