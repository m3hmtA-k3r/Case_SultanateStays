(function () {
    const form = document.getElementById('search-form');
    if (!form) return;

    const checkIn = form.querySelector('[name="CheckIn"]');
    const checkOut = form.querySelector('[name="CheckOut"]');
    const submit = form.querySelector('[data-search-submit]');
    const childAgesInput = form.querySelector('[name="ChildAges"]');
    const childrenInput = form.querySelector('[name="Children"]');
    const ageFields = Array.from(form.querySelectorAll('[data-child-age-field]'));
    const ageSelects = Array.from(form.querySelectorAll('[data-child-age]'));

    const toIsoDate = (date) => {
        const pad = (n) => String(n).padStart(2, '0');
        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
    };

    const dayAfter = (isoDate) => {
        const date = new Date(`${isoDate}T00:00:00`);
        date.setDate(date.getDate() + 1);
        return toIsoDate(date);
    };

    const updateChildAges = () => {
        const count = Number(childrenInput.value);

        ageFields.forEach((field, index) => {
            field.classList.toggle('hidden', index >= count);
        });

        childAgesInput.value = ageSelects
            .slice(0, count)
            .map((select) => select.value)
            .join(',');
    };

    form.querySelectorAll('[data-counter]').forEach((counter) => {
        const input = counter.querySelector('input[type="hidden"]');
        const display = counter.querySelector('[data-counter-value]');
        const min = Number(counter.dataset.min);
        const max = Number(counter.dataset.max);

        const setValue = (value) => {
            input.value = value;
            display.textContent = value;

            if (input === childrenInput) {
                updateChildAges();
            }
        };

        counter.querySelector('[data-action="minus"]').addEventListener('click', () => {
            setValue(Math.max(min, Number(input.value) - 1));
        });

        counter.querySelector('[data-action="plus"]').addEventListener('click', () => {
            setValue(Math.min(max, Number(input.value) + 1));
        });
    });

    ageSelects.forEach((select) => select.addEventListener('change', updateChildAges));

    checkIn.addEventListener('change', () => {
        checkOut.min = checkIn.value;
        if (checkOut.value <= checkIn.value) {
            checkOut.value = dayAfter(checkIn.value);
        }
    });

    form.addEventListener('submit', () => {
        updateChildAges();
        submit.disabled = true;
        submit.querySelector('span').textContent = 'Aranıyor...';
    });

    updateChildAges();
})();
