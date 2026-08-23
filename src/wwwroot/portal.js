function money(value) { return `${Number(value).toLocaleString('sv-SE')} SEK`; }
function updateCartView(form, result) {
  const row = form.closest('.cart-line');
  if (!row || !result?.items) return;
  const item = result.items.find(entry => entry.id === row.dataset.lineId);
  if (!item) return;
  const quantity = row.querySelector('input[name=quantity]');
  const lineTotal = row.querySelector('[data-line-total]');
  if (quantity) quantity.value = item.quantity;
  if (lineTotal) lineTotal.textContent = money(item.total);
  const cartTotal = document.querySelector('[data-cart-total]');
  if (cartTotal && result.total != null) cartTotal.textContent = money(result.total);
}
async function submitPortalForm(form) {
  if (form.dataset.pending === 'true') return;
  form.dataset.pending = 'true';
  const submitButton = form.querySelector('button[type="submit"], button:not([type])');
  if (submitButton) submitButton.disabled = true;
  const data = Object.fromEntries(new FormData(form).entries());
  form.querySelectorAll('input').forEach(input => {
    if (input.type === 'checkbox') data[input.name] = input.checked;
    else if (input.type === 'number' || input.name === 'quantity') data[input.name] = Number(input.value);
    else if (input.value === 'true' || input.value === 'false') data[input.name] = input.value === 'true';
  });
  const headers = { 'Content-Type': 'application/json' };
  if (form.dataset.order) {
    form.dataset.idempotencyKey ||= globalThis.crypto.randomUUID();
    headers['Idempotency-Key'] = form.dataset.idempotencyKey;
  }
  try {
    const response = await fetch(form.dataset.api, { method: form.dataset.method || 'POST', headers, body: JSON.stringify(data) });
    if (!response.ok) { const problem = await response.json().catch(() => ({})); alert(problem.detail || 'The request could not be completed.'); return; }
    const result = await response.json().catch(() => null);
    if (form.dataset.cart && result?.items) { updateCartView(form, result); return; }
    if (form.dataset.order && result?.id) location.href = `/orders/${result.id}`;
    else if (form.dataset.case && result?.id) location.href = `/support/${result.id}`;
    else location.href = form.dataset.redirect || location.href;
  } finally {
    form.dataset.pending = 'false';
    if (submitButton) submitButton.disabled = false;
  }
}
document.addEventListener('submit', event => { const form = event.target.closest('.js-form'); if (form) { event.preventDefault(); submitPortalForm(form); } });
document.addEventListener('click', async event => {
  const remove = event.target.closest('.delete');
  if (remove) { await fetch(remove.dataset.api, { method: 'DELETE' }); location.reload(); }
  const advance = event.target.closest('.advance');
  if (advance) { await fetch(advance.dataset.api, { method: 'POST' }); location.reload(); }
  const returnButton = event.target.closest('.return');
  if (returnButton && confirm('Create a return support case for this delivered order?')) { const response = await fetch(returnButton.dataset.api, { method: 'POST' }); const result = await response.json(); location.href = `/support/${result.supportCaseId}`; }
});