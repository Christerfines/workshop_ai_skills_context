async function submitPortalForm(form) {
  const data = Object.fromEntries(new FormData(form).entries());
  form.querySelectorAll('input[type=checkbox]').forEach(input => data[input.name] = input.checked);
  const response = await fetch(form.dataset.api, { method: form.dataset.method || 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
  if (!response.ok) { const problem = await response.json().catch(() => ({})); alert(problem.detail || 'The request could not be completed.'); return; }
  const result = await response.json().catch(() => null);
  if (form.dataset.order && result?.id) location.href = `/orders/${result.id}`;
  else if (form.dataset.case && result?.id) location.href = `/support/${result.id}`;
  else location.href = form.dataset.redirect || location.href;
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