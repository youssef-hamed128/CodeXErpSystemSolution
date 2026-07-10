/**
 * ERP System - DataTable JavaScript
 * Client-side: search, sort, paginate, filter
 * API-ready: swap mock data URL for real endpoint
 * ======================================
 */

'use strict';

// =============================================
// MOCK DATA SETS
// In production: fetch('/api/{Module}/GetData', { params })
// =============================================

const MOCK_DATA = {
  invoices: [
    { id: 1, number: 'INV-2024-0001', customer: 'شركة الأمل للتجارة',    date: '2024-01-15', dueDate: '2024-02-15', total: 52500.00, paid: 52500.00, status: 'paid',     salesperson: 'أحمد محمد' },
    { id: 2, number: 'INV-2024-0002', customer: 'مؤسسة النور التجارية',   date: '2024-01-18', dueDate: '2024-02-18', total: 18750.00, paid: 10000.00, status: 'partial',   salesperson: 'سارة أحمد' },
    { id: 3, number: 'INV-2024-0003', customer: 'شركة الرياض للمقاولات',  date: '2024-01-20', dueDate: '2024-02-20', total: 95000.00, paid: 0,       status: 'confirmed', salesperson: 'خالد علي' },
    { id: 4, number: 'INV-2024-0004', customer: 'مجموعة الخليج الصناعية', date: '2024-01-22', dueDate: '2024-01-30', total: 34200.00, paid: 0,       status: 'overdue',   salesperson: 'أحمد محمد' },
    { id: 5, number: 'INV-2024-0005', customer: 'شركة بترو تك',           date: '2024-01-25', dueDate: '2024-02-25', total: 12600.00, paid: 0,       status: 'draft',     salesperson: 'سارة أحمد' },
    { id: 6, number: 'INV-2024-0006', customer: 'شركة الأمل للتجارة',    date: '2024-01-28', dueDate: '2024-02-28', total: 67800.00, paid: 67800.00, status: 'paid',     salesperson: 'محمد السعيد' },
    { id: 7, number: 'INV-2024-0007', customer: 'شركة التقنية الحديثة',   date: '2024-02-01', dueDate: '2024-03-01', total: 23400.00, paid: 0,       status: 'confirmed', salesperson: 'خالد علي' },
    { id: 8, number: 'INV-2024-0008', customer: 'مؤسسة الريادة',          date: '2024-02-05', dueDate: '2024-03-05', total: 8900.00,  paid: 0,       status: 'draft',     salesperson: 'أحمد محمد' },
    { id: 9, number: 'INV-2024-0009', customer: 'شركة الشرق الأوسط',     date: '2024-02-08', dueDate: '2024-02-20', total: 45600.00, paid: 45600.00, status: 'paid',     salesperson: 'سارة أحمد' },
    { id: 10, number: 'INV-2024-0010', customer: 'مجموعة الخليج الصناعية', date: '2024-02-10', dueDate: '2024-03-10', total: 129000.00, paid: 50000.00, status: 'partial', salesperson: 'محمد السعيد' },
    { id: 11, number: 'INV-2024-0011', customer: 'شركة بترو تك',           date: '2024-02-12', dueDate: '2024-01-30', total: 38000.00, paid: 0,       status: 'overdue',   salesperson: 'خالد علي' },
    { id: 12, number: 'INV-2024-0012', customer: 'شركة الأمل للتجارة',    date: '2024-02-15', dueDate: '2024-03-15', total: 15700.00, paid: 0,       status: 'confirmed', salesperson: 'أحمد محمد' },
  ],

  products: [
    { id: 1, sku: 'LAP-001', name: 'لاب توب Dell XPS 15',   category: 'حواسيب محمولة', unit: 'قطعة', costPrice: 3500, salePrice: 4500, stock: 25, reorderLevel: 5,  status: 'active' },
    { id: 2, sku: 'PRN-002', name: 'طابعة HP LaserJet',     category: 'طابعات',        unit: 'قطعة', costPrice: 850,  salePrice: 1200, stock: 12, reorderLevel: 3,  status: 'active' },
    { id: 3, sku: 'MOU-003', name: 'ماوس لاسلكي Logitech', category: 'ملحقات',        unit: 'قطعة', costPrice: 90,   salePrice: 150,  stock: 3,  reorderLevel: 10, status: 'low' },
    { id: 4, sku: 'MON-004', name: 'شاشة Samsung 27"',     category: 'شاشات',         unit: 'قطعة', costPrice: 1600, salePrice: 2200, stock: 8,  reorderLevel: 4,  status: 'active' },
    { id: 5, sku: 'KEY-005', name: 'كيبورد Mechanical',    category: 'ملحقات',        unit: 'قطعة', costPrice: 250,  salePrice: 380,  stock: 0,  reorderLevel: 5,  status: 'out' },
    { id: 6, sku: 'CHR-006', name: 'كرسي مكتب Premium',   category: 'أثاث مكتبي',    unit: 'قطعة', costPrice: 600,  salePrice: 850,  stock: 18, reorderLevel: 3,  status: 'active' },
    { id: 7, sku: 'INK-007', name: 'حبر طابعة أسود',      category: 'مستلزمات طباعة', unit: 'علبة', costPrice: 55,   salePrice: 85,   stock: 45, reorderLevel: 20, status: 'active' },
    { id: 8, sku: 'PAP-008', name: 'ورق طباعة A4',        category: 'مستلزمات طباعة', unit: 'رزمة', costPrice: 15,   salePrice: 25,   stock: 2,  reorderLevel: 15, status: 'low' },
    { id: 9, sku: 'USB-009', name: 'مبدل USB Hub',        category: 'ملحقات',        unit: 'قطعة', costPrice: 60,   salePrice: 95,   stock: 30, reorderLevel: 8,  status: 'active' },
    { id: 10, sku: 'AUD-010', name: 'سماعات Headset',     category: 'صوتيات',        unit: 'قطعة', costPrice: 220,  salePrice: 320,  stock: 15, reorderLevel: 5,  status: 'active' },
  ],

  customers: [
    { id: 1, name: 'شركة الأمل للتجارة',    phone: '+966 11 234 5678', email: 'info@alamal.sa',  city: 'الرياض',  balance: 25600,  creditLimit: 50000, status: 'active' },
    { id: 2, name: 'مؤسسة النور التجارية',   phone: '+966 12 345 6789', email: 'info@alnour.sa',  city: 'جدة',     balance: 8750,   creditLimit: 30000, status: 'active' },
    { id: 3, name: 'شركة الرياض للمقاولات',  phone: '+966 11 456 7890', email: 'info@rcc.sa',     city: 'الرياض',  balance: 95000,  creditLimit: 75000, status: 'active' },
    { id: 4, name: 'مجموعة الخليج الصناعية', phone: '+966 13 567 8901', email: 'info@gulf-ind.sa', city: 'الدمام',  balance: 79000,  creditLimit: 100000, status: 'active' },
    { id: 5, name: 'شركة بترو تك',           phone: '+966 11 678 9012', email: 'info@petrotech.sa', city: 'الرياض', balance: -15000, creditLimit: 200000, status: 'active' },
    { id: 6, name: 'شركة التقنية الحديثة',   phone: '+966 12 789 0123', email: 'info@modern-tech.sa', city: 'جدة',  balance: 23400,  creditLimit: 40000, status: 'active' },
    { id: 7, name: 'مؤسسة الريادة',          phone: '+966 11 890 1234', email: 'info@riyadah.sa', city: 'الرياض', balance: 0,      creditLimit: 20000, status: 'inactive' },
    { id: 8, name: 'شركة الشرق الأوسط',     phone: '+966 13 901 2345', email: 'info@me-co.sa',   city: 'الدمام',  balance: 0,      creditLimit: 60000, status: 'active' },
  ],
};

// =============================================
// STATUS BADGE RENDERER
// =============================================
const STATUS_BADGES = {
  // Invoices
  paid:      { ar: 'مدفوع',         en: 'Paid',          class: 'badge-success' },
  partial:   { ar: 'مدفوع جزئياً',  en: 'Partial',       class: 'badge-info' },
  confirmed: { ar: 'مؤكد',          en: 'Confirmed',     class: 'badge-primary' },
  overdue:   { ar: 'متأخر',         en: 'Overdue',       class: 'badge-danger' },
  draft:     { ar: 'مسودة',         en: 'Draft',         class: 'badge-warning' },
  cancelled: { ar: 'ملغى',          en: 'Cancelled',     class: 'badge-secondary' },
  returned:  { ar: 'مرتجع',         en: 'Returned',      class: 'badge-secondary' },
  received:  { ar: 'مستلم',         en: 'Received',      class: 'badge-success' },

  // Products
  active:    { ar: 'نشط',           en: 'Active',        class: 'badge-success' },
  inactive:  { ar: 'غير نشط',       en: 'Inactive',      class: 'badge-secondary' },
  low:       { ar: 'مخزون منخفض',   en: 'Low Stock',     class: 'badge-warning' },
  out:       { ar: 'نفد المخزون',   en: 'Out of Stock',  class: 'badge-danger' },

  // Users
  online:    { ar: 'متصل',          en: 'Online',        class: 'badge-success' },
  offline:   { ar: 'غير متصل',      en: 'Offline',       class: 'badge-secondary' },
};

function renderBadge(status) {
  const s = STATUS_BADGES[status] || { ar: status, en: status, class: 'badge-secondary' };
  return `<span class="badge ${s.class}" data-ar="${s.ar}" data-en="${s.en}">${s.ar}</span>`;
}

// =============================================
// ERP DATA TABLE CLASS
// =============================================
class ERPDataTable {
  constructor(containerId, options = {}) {
    this.container    = document.getElementById(containerId);
    this.options      = {
      pageSize: options.pageSize || 10,
      apiUrl:   options.apiUrl   || null,
      mockData: options.mockData || [],
      columns:  options.columns  || [],
      searchable: options.searchable !== false,
      sortable:   options.sortable   !== false,
      ...options
    };

    this.currentPage  = 1;
    this.sortCol      = null;
    this.sortDir      = 'asc';
    this.filters      = {};
    this.searchQuery  = '';
    this.allData      = [];
    this.filteredData = [];

    if (this.container) this.init();
  }

  init() {
    this.tbody = this.container.querySelector('tbody');
    this.paginationEl = this.container.querySelector('.pagination-controls');
    this.paginationInfo = this.container.querySelector('.pagination-info');
    this.searchInput = this.container.querySelector('[data-table-search]');
    this.filterInputs = this.container.querySelectorAll('[data-filter]');

    this.loadData();
    this.bindEvents();
  }

  loadData() {
    if (this.options.apiUrl) {
      // API call — replace with real endpoint
      // fetch(this.options.apiUrl, { method: 'GET', headers: { 'Content-Type': 'application/json' } })
      //   .then(r => r.json())
      //   .then(data => { this.allData = data.items; this.render(); });
      this.allData = this.options.mockData;
    } else {
      this.allData = this.options.mockData;
    }
    this.applyFilters();
  }

  bindEvents() {
    // Search
    if (this.searchInput) {
      this.searchInput.addEventListener('input', () => {
        this.searchQuery = this.searchInput.value.toLowerCase();
        this.currentPage = 1;
        this.applyFilters();
      });
    }

    // Filter inputs
    this.filterInputs.forEach(input => {
      input.addEventListener('change', () => {
        this.filters[input.dataset.filter] = input.value;
        this.currentPage = 1;
        this.applyFilters();
      });
    });

    // Sortable headers
    if (this.options.sortable) {
      this.container.querySelectorAll('th[data-sort]').forEach(th => {
        th.classList.add('sortable');
        th.innerHTML += ' <i class="fas fa-sort sort-icon"></i>';
        th.addEventListener('click', () => {
          const col = th.dataset.sort;
          if (this.sortCol === col) {
            this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
          } else {
            this.sortCol = col;
            this.sortDir = 'asc';
          }
          this.applyFilters();
          // Update icons
          this.container.querySelectorAll('th[data-sort]').forEach(t => {
            t.classList.remove('sort-asc', 'sort-desc');
          });
          th.classList.add(`sort-${this.sortDir}`);
        });
      });
    }
  }

  applyFilters() {
    let data = [...this.allData];

    // Apply search
    if (this.searchQuery) {
      data = data.filter(item =>
        Object.values(item).some(v =>
          String(v).toLowerCase().includes(this.searchQuery)
        )
      );
    }

    // Apply filters
    Object.entries(this.filters).forEach(([key, val]) => {
      if (val && val !== '') {
        data = data.filter(item => String(item[key]).toLowerCase() === val.toLowerCase());
      }
    });

    // Apply sort
    if (this.sortCol) {
      data.sort((a, b) => {
        const av = a[this.sortCol];
        const bv = b[this.sortCol];
        const aNum = parseFloat(av);
        const bNum = parseFloat(bv);
        if (!isNaN(aNum) && !isNaN(bNum)) {
          return this.sortDir === 'asc' ? aNum - bNum : bNum - aNum;
        }
        return this.sortDir === 'asc'
          ? String(av).localeCompare(String(bv), 'ar')
          : String(bv).localeCompare(String(av), 'ar');
      });
    }

    this.filteredData = data;
    this.renderPage();
    this.renderPagination();
  }

  renderPage() {
    if (!this.tbody) return;
    const start = (this.currentPage - 1) * this.options.pageSize;
    const end   = start + this.options.pageSize;
    const pageData = this.filteredData.slice(start, end);

    if (pageData.length === 0) {
      this.tbody.innerHTML = `
        <tr>
          <td colspan="100" class="empty-state" style="padding: 3rem; text-align: center; color: var(--text-muted);">
            <i class="fas fa-search" style="font-size: 2rem; margin-bottom: 1rem; display: block;"></i>
            لا توجد بيانات مطابقة
          </td>
        </tr>
      `;
      return;
    }

    if (typeof this.options.renderRow === 'function') {
      this.tbody.innerHTML = pageData.map((item, i) =>
        this.options.renderRow(item, start + i)
      ).join('');
    }

    // Reattach bulk checkbox events
    if (window.ERP) initBulkActionsForTable(this.container);
  }

  renderPagination() {
    const total     = this.filteredData.length;
    const pageCount = Math.ceil(total / this.options.pageSize);
    const start     = Math.min((this.currentPage - 1) * this.options.pageSize + 1, total);
    const end       = Math.min(this.currentPage * this.options.pageSize, total);

    if (this.paginationInfo) {
      this.paginationInfo.textContent = total === 0
        ? 'لا توجد نتائج'
        : `عرض ${start} - ${end} من ${total} سجل`;
    }

    if (!this.paginationEl) return;

    const pages = [];
    // Previous button
    pages.push(`<button class="page-btn" data-page="${this.currentPage - 1}" ${this.currentPage === 1 ? 'disabled' : ''}>
      <i class="fas fa-chevron-right"></i>
    </button>`);

    // Page numbers
    const range = this.pageRange(this.currentPage, pageCount);
    range.forEach(p => {
      if (p === '...') {
        pages.push(`<span style="padding: 0 4px; color: var(--text-muted);">...</span>`);
      } else {
        pages.push(`<button class="page-btn ${p === this.currentPage ? 'active' : ''}" data-page="${p}">${p}</button>`);
      }
    });

    // Next button
    pages.push(`<button class="page-btn" data-page="${this.currentPage + 1}" ${this.currentPage >= pageCount ? 'disabled' : ''}>
      <i class="fas fa-chevron-left"></i>
    </button>`);

    this.paginationEl.innerHTML = pages.join('');

    // Bind page buttons
    this.paginationEl.querySelectorAll('.page-btn:not(:disabled)').forEach(btn => {
      btn.addEventListener('click', () => {
        const p = parseInt(btn.dataset.page);
        if (!isNaN(p) && p >= 1 && p <= pageCount) {
          this.currentPage = p;
          this.renderPage();
          this.renderPagination();
        }
      });
    });
  }

  pageRange(current, total) {
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    if (current <= 4) return [1, 2, 3, 4, 5, '...', total];
    if (current >= total - 3) return [1, '...', total-4, total-3, total-2, total-1, total];
    return [1, '...', current-1, current, current+1, '...', total];
  }

  refresh() {
    this.currentPage = 1;
    this.applyFilters();
  }
}

// Helper for re-initializing bulk actions after render
function initBulkActionsForTable(table) {
  // Will be handled by app.js initBulkActions on next tick
}

// =============================================
// SIMPLE CLIENT-SIDE FILTER FOR STATIC TABLES
// (For pages that don't use ERPDataTable class)
// =============================================
function initStaticTableFilter() {
  document.querySelectorAll('[data-table-search-simple]').forEach(input => {
    const tableId = input.dataset.tableSearchSimple;
    const table = document.getElementById(tableId) || document.querySelector('.data-table');

    input.addEventListener('input', () => {
      const q = input.value.toLowerCase();
      if (!table) return;
      table.querySelectorAll('tbody tr').forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = (q === '' || text.includes(q)) ? '' : 'none';
      });
      updatePaginationInfo(table, q);
    });
  });

  // Status filter dropdowns
  document.querySelectorAll('[data-filter-status]').forEach(select => {
    select.addEventListener('change', () => {
      const val = select.value.toLowerCase();
      document.querySelectorAll('.data-table tbody tr').forEach(row => {
        if (val === '' || val === 'all') {
          row.style.display = '';
          return;
        }
        const badge = row.querySelector('.badge');
        const rowStatus = badge?.className.toLowerCase() || row.dataset.status?.toLowerCase() || '';
        row.style.display = rowStatus.includes(val) ? '' : 'none';
      });
    });
  });
}

function updatePaginationInfo(table, query) {
  const info = document.querySelector('.pagination-info');
  if (!info) return;
  const visible = table.querySelectorAll('tbody tr:not([style*="display: none"])').length;
  const total   = table.querySelectorAll('tbody tr').length;
  info.textContent = query
    ? `عرض ${visible} نتيجة من ${total} سجل`
    : `عرض ${total} سجل`;
}

// =============================================
// EXPOSE
// =============================================
window.ERPDataTable = ERPDataTable;
window.MOCK_DATA    = MOCK_DATA;
window.renderBadge  = renderBadge;

// =============================================
// INIT
// =============================================
document.addEventListener('DOMContentLoaded', () => {
  initStaticTableFilter();
});
