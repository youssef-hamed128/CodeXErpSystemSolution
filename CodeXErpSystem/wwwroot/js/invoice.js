/**
 * ERP System - Invoice JavaScript
 * Dynamic line items, calculations, form validation
 * ======================================
 * Works for: Sales Invoice, Purchase Invoice, Returns
 * ASP.NET MVC ready: name attrs use Items[n].Property pattern
 */

'use strict';

// =============================================
// MOCK PRODUCT DATA
// In production: fetch('/api/Inventory/SearchProducts?q=...')
// =============================================
const MOCK_PRODUCTS = [
  { id: 1, name: 'لاب توب Dell XPS 15',  nameEn: 'Dell XPS 15 Laptop',     sku: 'LAP-001', unit: 'قطعة', unitPrice: 4500.00, taxRate: 15 },
  { id: 2, name: 'طابعة HP LaserJet',     nameEn: 'HP LaserJet Printer',    sku: 'PRN-002', unit: 'قطعة', unitPrice: 1200.00, taxRate: 15 },
  { id: 3, name: 'ماوس لاسلكي Logitech', nameEn: 'Logitech Wireless Mouse', sku: 'MOU-003', unit: 'قطعة', unitPrice: 150.00,  taxRate: 15 },
  { id: 4, name: 'شاشة Samsung 27"',     nameEn: 'Samsung 27" Monitor',    sku: 'MON-004', unit: 'قطعة', unitPrice: 2200.00, taxRate: 15 },
  { id: 5, name: 'كيبورد Mechanical',    nameEn: 'Mechanical Keyboard',    sku: 'KEY-005', unit: 'قطعة', unitPrice: 380.00,  taxRate: 15 },
  { id: 6, name: 'كرسي مكتب Premium',   nameEn: 'Premium Office Chair',   sku: 'CHR-006', unit: 'قطعة', unitPrice: 850.00,  taxRate: 15 },
  { id: 7, name: 'حبر طابعة أسود',      nameEn: 'Black Printer Ink',      sku: 'INK-007', unit: 'علبة', unitPrice: 85.00,   taxRate: 15 },
  { id: 8, name: 'ورق طباعة A4',        nameEn: 'A4 Printing Paper',      sku: 'PAP-008', unit: 'رزمة', unitPrice: 25.00,   taxRate: 15 },
  { id: 9, name: 'مبدل USB Hub',        nameEn: 'USB Hub Adapter',        sku: 'USB-009', unit: 'قطعة', unitPrice: 95.00,   taxRate: 15 },
  { id: 10, name: 'سماعات Headset',     nameEn: 'Professional Headset',   sku: 'AUD-010', unit: 'قطعة', unitPrice: 320.00,  taxRate: 15 },
];

const MOCK_CUSTOMERS = [
  { id: 1, name: 'شركة الأمل للتجارة',    nameEn: 'Al-Amal Trading Co.',     creditLimit: 50000 },
  { id: 2, name: 'مؤسسة النور التجارية',   nameEn: 'Al-Nour Commercial Est.', creditLimit: 30000 },
  { id: 3, name: 'شركة الرياض للمقاولات',  nameEn: 'Riyadh Contracting Co.',  creditLimit: 75000 },
  { id: 4, name: 'مجموعة الخليج الصناعية', nameEn: 'Gulf Industrial Group',   creditLimit: 100000 },
  { id: 5, name: 'شركة بترو تك',          nameEn: 'PetroTech Company',        creditLimit: 200000 },
];

const MOCK_SUPPLIERS = [
  { id: 1, name: 'شركة التقنية المتقدمة',  nameEn: 'Advanced Tech Co.'},
  { id: 2, name: 'مورد اللوازم المكتبية',  nameEn: 'Office Supplies Vendor' },
  { id: 3, name: 'مجموعة الإلكترونيات',   nameEn: 'Electronics Group' },
  { id: 4, name: 'شركة المواد الغذائية',   nameEn: 'Food Materials Co.' },
];

// =============================================
// LINE ITEM ROW COUNTER
// =============================================
let lineItemIndex = 0;

// =============================================
// AUTO-GENERATE INVOICE NUMBER
// =============================================
function generateInvoiceNumber(prefix = 'INV') {
  const counter = parseInt(localStorage.getItem(`${prefix}_counter`) || '0') + 1;
  localStorage.setItem(`${prefix}_counter`, counter);
  const year = new Date().getFullYear();
  return `${prefix}-${year}-${String(counter).padStart(4, '0')}`;
}

function initInvoiceNumber() {
  const el = document.getElementById('invoiceNumber');
  if (!el || el.value) return; // Don't overwrite existing number

  const prefix = el.dataset.prefix || 'INV';
  el.value = generateInvoiceNumber(prefix);
}

// =============================================
// LINE ITEM: CALCULATE ONE ROW
// =============================================
function calculateLineTotal(row) {
  const qty       = parseFloat(row.querySelector('.item-qty')?.value)      || 0;
  const price     = parseFloat(row.querySelector('.item-price')?.value)    || 0;
  const discount  = parseFloat(row.querySelector('.item-discount')?.value) || 0;
  const taxRate   = parseFloat(row.querySelector('.item-tax')?.value)      || 0;

  const subtotalBeforeDiscount = qty * price;
  const discountAmount = subtotalBeforeDiscount * (discount / 100);
  const subtotalAfterDiscount = subtotalBeforeDiscount - discountAmount;
  const taxAmount = subtotalAfterDiscount * (taxRate / 100);
  const lineTotal = subtotalAfterDiscount + taxAmount;

  const totalEl = row.querySelector('.line-total');
  if (totalEl) {
    totalEl.textContent = formatNumber(lineTotal);
    totalEl.dataset.value = lineTotal;
  }

  calculateInvoiceTotals();
}

// =============================================
// INVOICE TOTALS
// =============================================
function calculateInvoiceTotals() {
  let subtotalRaw = 0;
  let discountRaw = 0;
  let taxRaw      = 0;
  let grandTotal  = 0;

  document.querySelectorAll('.invoice-items-table tbody tr.line-item-row').forEach(row => {
    const qty      = parseFloat(row.querySelector('.item-qty')?.value)      || 0;
    const price    = parseFloat(row.querySelector('.item-price')?.value)    || 0;
    const discount = parseFloat(row.querySelector('.item-discount')?.value) || 0;
    const taxRate  = parseFloat(row.querySelector('.item-tax')?.value)      || 0;

    const rowSubtotal  = qty * price;
    const rowDiscount  = rowSubtotal * (discount / 100);
    const rowAfterDisc = rowSubtotal - rowDiscount;
    const rowTax       = rowAfterDisc * (taxRate / 100);
    const rowTotal     = rowAfterDisc + rowTax;

    subtotalRaw += rowSubtotal;
    discountRaw += rowDiscount;
    taxRaw      += rowTax;
    grandTotal  += rowTotal;
  });

  // Update DOM
  setText('#invoiceSubtotal',       formatNumber(subtotalRaw));
  setText('#invoiceTotalDiscount',  formatNumber(discountRaw));
  setText('#invoiceTotalTax',       formatNumber(taxRaw));
  setText('#invoiceGrandTotal',     formatNumber(grandTotal));

  // Balance Due
  const amountPaid = parseFloat(document.getElementById('amountPaid')?.value) || 0;
  const balanceDue = grandTotal - amountPaid;
  setText('#invoiceBalanceDue', formatNumber(balanceDue));

  // Store grand total in hidden field if present
  const hiddenTotal = document.getElementById('hiddenGrandTotal');
  if (hiddenTotal) hiddenTotal.value = grandTotal.toFixed(2);
}

function setText(selector, value) {
  const el = document.querySelector(selector);
  if (el) el.textContent = value;
}

function formatNumber(num) {
  return new Intl.NumberFormat('ar-SA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(num);
}

// =============================================
// ADD LINE ITEM ROW
// =============================================
function addLineItem() {
  const tbody = document.querySelector('.invoice-items-table tbody');
  if (!tbody) return;

  const row = document.createElement('tr');
  row.className = 'line-item-row';
  row.dataset.index = lineItemIndex;
  row.innerHTML = `
    <td>
      <div class="search-dropdown-wrapper">
        <input type="text"
               class="form-control item-product-search"
               placeholder="ابحث عن المنتج..."
               name="Items[${lineItemIndex}].ProductName"
               autocomplete="off"
               data-val="true"
               data-val-required="اختر منتجاً">
        <input type="hidden" name="Items[${lineItemIndex}].ProductId">
        <div class="search-dropdown-list product-dropdown"></div>
      </div>
    </td>
    <td>
      <input type="text"
             class="form-control item-description"
             name="Items[${lineItemIndex}].Description"
             placeholder="وصف العنصر">
    </td>
    <td style="width:90px;">
      <input type="number"
             class="form-control item-qty"
             name="Items[${lineItemIndex}].Quantity"
             value="1" min="0.001" step="0.001"
             data-val="true"
             data-val-number="يجب أن يكون رقماً">
    </td>
    <td style="width:80px;">
      <input type="text"
             class="form-control item-unit"
             name="Items[${lineItemIndex}].Unit"
             placeholder="قطعة">
    </td>
    <td style="width:120px;">
      <input type="number"
             class="form-control item-price"
             name="Items[${lineItemIndex}].UnitPrice"
             value="0.00" min="0" step="0.01"
             data-val="true"
             data-val-number="يجب أن يكون رقماً">
    </td>
    <td style="width:80px;">
      <input type="number"
             class="form-control item-discount"
             name="Items[${lineItemIndex}].Discount"
             value="0" min="0" max="100" step="0.01">
    </td>
    <td style="width:80px;">
      <input type="number"
             class="form-control item-tax"
             name="Items[${lineItemIndex}].TaxRate"
             value="15" min="0" max="100" step="0.01">
    </td>
    <td class="line-total numeric" data-value="0">0.00</td>
    <td class="center">
      <button type="button" class="remove-row-btn" onclick="removeLineItem(this)" title="حذف الصف">
        <i class="fas fa-times"></i>
      </button>
    </td>
  `;

  tbody.appendChild(row);

  // Attach events to new row
  attachRowEvents(row);

  lineItemIndex++;

  // Trigger animation
  row.style.opacity = '0';
  requestAnimationFrame(() => {
    row.style.transition = 'opacity 0.2s';
    row.style.opacity = '1';
  });
}

// =============================================
// REMOVE LINE ITEM ROW
// =============================================
function removeLineItem(btn) {
  const row = btn.closest('tr');
  const tbody = row?.parentElement;
  if (!tbody) return;

  if (tbody.querySelectorAll('tr.line-item-row').length <= 1) {
    if (window.showToast) showToast('يجب أن تحتوي الفاتورة على صف واحد على الأقل', 'warning');
    return;
  }

  row.style.transition = 'opacity 0.2s';
  row.style.opacity = '0';
  setTimeout(() => {
    row.remove();
    calculateInvoiceTotals();
  }, 200);
}

// =============================================
// ATTACH EVENTS TO ROW INPUTS
// =============================================
function attachRowEvents(row) {
  const calcInputs = row.querySelectorAll('.item-qty, .item-price, .item-discount, .item-tax');
  calcInputs.forEach(input => {
    input.addEventListener('input', () => calculateLineTotal(row));
    input.addEventListener('change', () => calculateLineTotal(row));
  });

  // Product search autocomplete
  const searchInput = row.querySelector('.item-product-search');
  const dropdown    = row.querySelector('.product-dropdown');
  const productId   = row.querySelector(`input[name*="ProductId"]`);

  if (searchInput && dropdown) {
    searchInput.addEventListener('input', () => {
      const q = searchInput.value.toLowerCase();
      if (q.length < 1) {
        dropdown.classList.remove('open');
        return;
      }

      const matches = MOCK_PRODUCTS.filter(p =>
        p.name.toLowerCase().includes(q) || p.sku.toLowerCase().includes(q)
      );

      if (matches.length === 0) {
        dropdown.innerHTML = '<div class="search-dropdown-item" style="color:var(--text-muted)">لا توجد نتائج</div>';
      } else {
        dropdown.innerHTML = matches.map(p => `
          <div class="search-dropdown-item"
               data-id="${p.id}"
               data-name="${p.name}"
               data-price="${p.unitPrice}"
               data-unit="${p.unit}"
               data-tax="${p.taxRate}">
            <div>
              <strong>${p.name}</strong>
              <span style="color:var(--text-muted); font-size:var(--fs-xs); margin-right:8px;">${p.sku}</span>
            </div>
            <div style="color:var(--text-muted); font-size:var(--fs-xs);">${formatNumber(p.unitPrice)} ج.م</div>
          </div>
        `).join('');
      }
      dropdown.classList.add('open');

      // Item click
      dropdown.querySelectorAll('.search-dropdown-item[data-id]').forEach(item => {
        item.addEventListener('click', () => {
          searchInput.value = item.dataset.name;
          if (productId) productId.value = item.dataset.id;

          // Auto-fill row
          const priceInput = row.querySelector('.item-price');
          const unitInput  = row.querySelector('.item-unit');
          const taxInput   = row.querySelector('.item-tax');
          const descInput  = row.querySelector('.item-description');

          if (priceInput) priceInput.value = item.dataset.price;
          if (unitInput)  unitInput.value  = item.dataset.unit;
          if (taxInput)   taxInput.value   = item.dataset.tax;
          if (descInput && !descInput.value) descInput.value = item.dataset.name;

          dropdown.classList.remove('open');
          calculateLineTotal(row);
        });
      });
    });

    searchInput.addEventListener('blur', () => {
      setTimeout(() => dropdown.classList.remove('open'), 200);
    });
  }
}

// =============================================
// INIT EXISTING ROWS (Edit page)
// =============================================
function initExistingRows() {
  document.querySelectorAll('.invoice-items-table tbody tr.line-item-row').forEach((row, idx) => {
    lineItemIndex = Math.max(lineItemIndex, idx + 1);
    attachRowEvents(row);
    calculateLineTotal(row);
  });
}

// =============================================
// AMOUNT PAID → BALANCE DUE
// =============================================
function initAmountPaid() {
  const amountPaidInput = document.getElementById('amountPaid');
  if (amountPaidInput) {
    amountPaidInput.addEventListener('input', calculateInvoiceTotals);
  }
}

// =============================================
// FORM VALIDATION
// =============================================
function validateInvoiceForm() {
  let valid = true;
  const errors = [];

  // Customer / Supplier
  const partyId = document.querySelector('[name="CustomerId"], [name="SupplierId"]');
  if (partyId && !partyId.value) {
    errors.push('يرجى اختيار العميل أو المورد');
    partyId.classList.add('is-invalid');
    valid = false;
  }

  // Invoice Date
  const invoiceDate = document.querySelector('[name="InvoiceDate"]');
  if (invoiceDate && !invoiceDate.value) {
    errors.push('يرجى تحديد تاريخ الفاتورة');
    invoiceDate.classList.add('is-invalid');
    valid = false;
  }

  // Line items
  const rows = document.querySelectorAll('.invoice-items-table tbody tr.line-item-row');
  if (rows.length === 0) {
    errors.push('يجب إضافة عنصر واحد على الأقل');
    valid = false;
  }

  let hasEmptyProduct = false;
  rows.forEach(row => {
    const productInput = row.querySelector('.item-product-search');
    const qty = parseFloat(row.querySelector('.item-qty')?.value) || 0;

    if (!productInput?.value?.trim()) {
      hasEmptyProduct = true;
      productInput?.classList.add('is-invalid');
      valid = false;
    }

    if (qty <= 0) {
      row.querySelector('.item-qty')?.classList.add('is-invalid');
      errors.push('الكمية يجب أن تكون أكبر من صفر');
      valid = false;
    }
  });

  if (hasEmptyProduct) errors.push('يرجى اختيار المنتج لجميع الصفوف');

  // Show errors
  if (!valid) {
    const msg = errors.filter((v, i, a) => a.indexOf(v) === i).join('<br>');
    if (window.showToast) {
      showToast(errors[0], 'danger', 5000);
    }
  }

  return valid;
}

// =============================================
// INVOICE ACTIONS
// =============================================
function saveAsDraft() {
  const statusField = document.getElementById('invoiceStatus');
  if (statusField) statusField.value = 'Draft';
  submitInvoiceForm();
}

function confirmAndPost() {
  if (!validateInvoiceForm()) return;
  const statusField = document.getElementById('invoiceStatus');
  if (statusField) statusField.value = 'Confirmed';
  submitInvoiceForm();
}

function submitInvoiceForm() {
  const form = document.getElementById('invoiceForm');
  if (form) {
    // In production: form.submit() or fetch POST
    // For prototype: show toast and simulate
    showToast('جاري الحفظ...', 'info', 1500);
    setTimeout(() => {
      showToast('تم حفظ الفاتورة بنجاح', 'success');
    }, 1500);
    // form.submit();
  }
}

function sendInvoiceByEmail() {
  if (window.openModal) openModal('sendEmailModal');
}

function convertToReturn() {
  const invoiceId = document.getElementById('invoiceId')?.value;
  if (invoiceId) {
    window.location.href = `/SalesReturn/Create?invoiceId=${invoiceId}`;
  }
}

// =============================================
// CUSTOMER/SUPPLIER SEARCH
// =============================================
function initPartySearch() {
  const partySearch = document.getElementById('partySearch');
  const partyIdField = document.getElementById('partyId');
  const partyDropdown = document.getElementById('partyDropdown');

  if (!partySearch) return;

  const isSupplier = partySearch.dataset.type === 'supplier';
  const data = isSupplier ? MOCK_SUPPLIERS : MOCK_CUSTOMERS;

  partySearch.addEventListener('input', () => {
    const q = partySearch.value.toLowerCase();
    if (q.length < 1) {
      if (partyDropdown) partyDropdown.classList.remove('open');
      return;
    }

    const matches = data.filter(c => c.name.toLowerCase().includes(q));
    if (partyDropdown) {
      partyDropdown.innerHTML = matches.length === 0
        ? '<div class="search-dropdown-item" style="color:var(--text-muted)">لا توجد نتائج</div>'
        : matches.map(c => `
            <div class="search-dropdown-item" data-id="${c.id}" data-name="${c.name}">
              ${c.name}
            </div>
          `).join('');
      partyDropdown.classList.add('open');

      partyDropdown.querySelectorAll('.search-dropdown-item[data-id]').forEach(item => {
        item.addEventListener('click', () => {
          partySearch.value = item.dataset.name;
          if (partyIdField) partyIdField.value = item.dataset.id;
          partyDropdown.classList.remove('open');
          partySearch.classList.remove('is-invalid');
        });
      });
    }
  });

  partySearch.addEventListener('blur', () => {
    setTimeout(() => {
      if (partyDropdown) partyDropdown.classList.remove('open');
    }, 200);
  });
}

// =============================================
// EXPOSE GLOBAL FUNCTIONS
// =============================================
window.addLineItem     = addLineItem;
window.removeLineItem  = removeLineItem;
window.saveAsDraft     = saveAsDraft;
window.confirmAndPost  = confirmAndPost;
window.sendInvoiceByEmail = sendInvoiceByEmail;
window.convertToReturn = convertToReturn;
window.validateInvoiceForm = validateInvoiceForm;

// =============================================
// INIT
// =============================================
document.addEventListener('DOMContentLoaded', () => {
  initInvoiceNumber();
  initExistingRows();
  initAmountPaid();
  initPartySearch();
  calculateInvoiceTotals();

  // Add Line Item button
  const addBtn = document.getElementById('addLineItemBtn');
  if (addBtn) addBtn.addEventListener('click', addLineItem);

  // If no rows exist on create page, add one
  const tbody = document.querySelector('.invoice-items-table tbody');
  if (tbody && tbody.querySelectorAll('tr.line-item-row').length === 0) {
    addLineItem();
  }
});
