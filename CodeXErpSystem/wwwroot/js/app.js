/**
 * ERP System - Main Application JavaScript
 * Arabic RTL Accounting System
 * ======================================
 * Handles: Sidebar, Theme, Language, Modals, Dropdowns,
 *          Notifications, Toasts, Bulk Actions, Print, Export
 */

'use strict';

// =============================================
// STATE
// =============================================
const ERP = {
  isDark: false,
  isLTR: false,
  sidebarCollapsed: false,
};

// =============================================
// SIDEBAR
// =============================================
function initSidebar() {
  const sidebar = document.getElementById('sidebar');
  const toggleBtn = document.getElementById('sidebarToggle');
  const overlay = document.getElementById('sidebarOverlay');

  if (!sidebar) return;

  // Restore collapsed state
  const collapsed = localStorage.getItem('sidebarCollapsed') === 'true';
  if (collapsed) {
    sidebar.classList.add('collapsed');
    ERP.sidebarCollapsed = true;
  }

  // Toggle on button click
  if (toggleBtn) {
    toggleBtn.addEventListener('click', () => toggleSidebar());
  }

  // Overlay click (mobile)
  if (overlay) {
    overlay.addEventListener('click', () => closeMobileSidebar());
  }

  // Submenu toggles
  document.querySelectorAll('.nav-link[data-submenu]').forEach(link => {
    link.addEventListener('click', (e) => {
      e.preventDefault();
      const submenuId = link.dataset.submenu;
      const submenu = document.getElementById(submenuId);
      if (!submenu) return;
      const isOpen = submenu.classList.contains('open');
      // Close all other submenus
      document.querySelectorAll('.nav-submenu.open').forEach(m => m.classList.remove('open'));
      document.querySelectorAll('.nav-link[data-submenu]').forEach(l => l.classList.remove('submenu-open'));
      // Toggle current
      if (!isOpen) {
        submenu.classList.add('open');
        link.classList.add('submenu-open');
      }
    });
  });

  // Set active link based on current page
  setActiveNavLink();
}

function toggleSidebar() {
  const sidebar = document.getElementById('sidebar');
  const overlay = document.getElementById('sidebarOverlay');

  if (window.innerWidth <= 768) {
    // Mobile: show/hide with overlay
    sidebar.classList.toggle('mobile-open');
    if (overlay) overlay.classList.toggle('active');
    return;
  }

  // Desktop: collapse/expand
  sidebar.classList.toggle('collapsed');
  ERP.sidebarCollapsed = sidebar.classList.contains('collapsed');
  localStorage.setItem('sidebarCollapsed', ERP.sidebarCollapsed);
}

function closeMobileSidebar() {
  const sidebar = document.getElementById('sidebar');
  const overlay = document.getElementById('sidebarOverlay');
  if (sidebar) sidebar.classList.remove('mobile-open');
  if (overlay) overlay.classList.remove('active');
}

function setActiveNavLink() {
  const currentPath = window.location.pathname.toLowerCase();
  
  // Clear any hardcoded active classes first to prevent duplicates
  document.querySelectorAll('.sidebar-nav .active').forEach(el => el.classList.remove('active'));

  let bestMatch = null;
  let bestMatchScore = 0;

  document.querySelectorAll('.sidebar-nav a.nav-link').forEach(link => {
    if (!link.href || link.getAttribute('href') === '#') return;

    const linkPath = link.pathname.toLowerCase();
    
    if (currentPath.endsWith(linkPath) || linkPath.endsWith(currentPath) || currentPath === linkPath) {
      // Exact match
      bestMatch = link;
      bestMatchScore = 100;
    } else {
      // Partial match (e.g. current is View.html, link is Index.html in same folder)
      const currentFolder = currentPath.split('/').filter(p => p.length > 0).slice(-2, -1)[0];
      const linkFolder = linkPath.split('/').filter(p => p.length > 0).slice(-2, -1)[0];
      const linkFilename = linkPath.split('/').pop();
      
      if (currentFolder === linkFolder && currentFolder !== undefined) {
        if (linkFilename === 'index.html' && bestMatchScore < 50) {
          bestMatch = link;
          bestMatchScore = 50;
        } else if (bestMatchScore < 10) {
          bestMatch = link;
          bestMatchScore = 10;
        }
      }
    }
  });

  if (bestMatch) {
    bestMatch.classList.add('active');
    
    // Open parent submenu if inside one (legacy support if any dropdowns remain)
    const submenu = bestMatch.closest('.nav-submenu');
    if (submenu) {
      submenu.classList.add('open');
      const parentLink = document.querySelector(`[data-submenu="${submenu.id}"]`);
      if (parentLink) parentLink.classList.add('submenu-open');
    }
  }
}

// =============================================
// DARK MODE
// =============================================
function initTheme() {
  const saved = localStorage.getItem('theme');
  ERP.isDark = saved === 'dark';
  applyTheme();

  const btn = document.getElementById('themeToggle');
  if (btn) {
    btn.addEventListener('click', toggleTheme);
  }
}

function toggleTheme() {
  ERP.isDark = !ERP.isDark;
  applyTheme();
  localStorage.setItem('theme', ERP.isDark ? 'dark' : 'light');
}

function applyTheme() {
  document.body.classList.toggle('dark-mode', ERP.isDark);
  const btn = document.getElementById('themeToggle');
  if (btn) {
    btn.innerHTML = ERP.isDark
      ? '<i class="fas fa-sun"></i>'
      : '<i class="fas fa-moon"></i>';
    btn.title = ERP.isDark ? 'وضع النهار' : 'الوضع الليلي';
  }
}

// =============================================
// LANGUAGE SWITCHER
// =============================================

// Translation strings
const translations = {
  ar: {
    'nav.dashboard': 'الرئيسية',
    'nav.sales': 'المبيعات',
    'btn.add': 'إضافة جديد',
    'btn.export': 'تصدير',
    'btn.print': 'طباعة',
    'btn.save': 'حفظ',
    'btn.cancel': 'إلغاء',
    'btn.delete': 'حذف',
    'btn.confirm': 'تأكيد',
  },
  en: {
    'nav.dashboard': 'Dashboard',
    'nav.sales': 'Sales',
    'btn.add': 'Add New',
    'btn.export': 'Export',
    'btn.print': 'Print',
    'btn.save': 'Save',
    'btn.cancel': 'Cancel',
    'btn.delete': 'Delete',
    'btn.confirm': 'Confirm',
  }
};

function initLanguage() {
  const saved = localStorage.getItem('lang') || 'ar';
  ERP.isLTR = saved === 'en';
  applyLanguage(saved);

  document.querySelectorAll('.lang-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      const lang = btn.dataset.lang;
      ERP.isLTR = lang === 'en';
      applyLanguage(lang);
      localStorage.setItem('lang', lang);
    });
  });
}

function applyLanguage(lang) {
  const html = document.documentElement;
  const body = document.body;

  if (lang === 'en') {
    html.setAttribute('dir', 'ltr');
    html.setAttribute('lang', 'en');
    body.classList.add('ltr');
    body.classList.remove('rtl');
  } else {
    html.setAttribute('dir', 'rtl');
    html.setAttribute('lang', 'ar');
    body.classList.remove('ltr');
    body.classList.add('rtl');
  }

  // Update lang buttons
  document.querySelectorAll('.lang-btn').forEach(btn => {
    btn.classList.toggle('active', btn.dataset.lang === lang);
  });

  // Swap data-ar / data-en text
  document.querySelectorAll('[data-ar][data-en]').forEach(el => {
    el.textContent = lang === 'en' ? el.dataset.en : el.dataset.ar;
  });
}

// =============================================
// MODAL SYSTEM
// =============================================
const modalStack = [];

function openModal(modalId) {
  const overlay = document.getElementById(modalId);
  if (!overlay) return;
  overlay.classList.add('open');
  modalStack.push(modalId);
  document.body.style.overflow = 'hidden';

  // Close on outside click
  overlay.addEventListener('click', (e) => {
    if (e.target === overlay) closeModal(modalId);
  });
}

function closeModal(modalId) {
  const overlay = document.getElementById(modalId);
  if (!overlay) return;
  overlay.classList.remove('open');
  const idx = modalStack.indexOf(modalId);
  if (idx > -1) modalStack.splice(idx, 1);
  if (modalStack.length === 0) {
    document.body.style.overflow = '';
  }
}

function closeAllModals() {
  [...modalStack].forEach(id => closeModal(id));
}

// ESC key closes top modal
document.addEventListener('keydown', (e) => {
  if (e.key === 'Escape' && modalStack.length > 0) {
    closeModal(modalStack[modalStack.length - 1]);
  }
});

// Wire up close buttons
function initModals() {
  document.querySelectorAll('[data-modal-close]').forEach(btn => {
    btn.addEventListener('click', () => {
      const modalId = btn.dataset.modalClose || btn.closest('.modal-overlay')?.id;
      if (modalId) closeModal(modalId);
    });
  });

  document.querySelectorAll('[data-modal-open]').forEach(btn => {
    btn.addEventListener('click', () => {
      openModal(btn.dataset.modalOpen);
    });
  });
}

// Confirm Dialog
function confirmDialog(message, onConfirm, options = {}) {
  const title = options.title || 'تأكيد الإجراء';
  const confirmText = options.confirmText || 'تأكيد';
  const cancelText = options.cancelText || 'إلغاء';
  const type = options.type || 'danger';
  const icon = options.icon || (type === 'danger' ? 'fa-trash' : 'fa-question');

  // Create or reuse confirm modal
  let overlay = document.getElementById('confirmModal');
  if (!overlay) {
    overlay = document.createElement('div');
    overlay.id = 'confirmModal';
    overlay.className = 'modal-overlay';
    overlay.innerHTML = `
      <div class="modal modal-sm">
        <div class="modal-header">
          <h5 class="modal-title" id="confirmModalTitle"></h5>
          <button class="modal-close" onclick="closeModal('confirmModal')"><i class="fas fa-times"></i></button>
        </div>
        <div class="modal-body" style="text-align: center;">
          <div class="confirm-icon ${type}" id="confirmIcon">
            <i class="fas ${icon}"></i>
          </div>
          <p id="confirmMessage" style="margin-bottom: 0;"></p>
        </div>
        <div class="modal-footer" style="justify-content: center;">
          <button class="btn btn-secondary" onclick="closeModal('confirmModal')" id="confirmCancelBtn"></button>
          <button class="btn btn-${type}" id="confirmOkBtn"></button>
        </div>
      </div>
    `;
    document.body.appendChild(overlay);
  }

  overlay.querySelector('#confirmModalTitle').textContent = title;
  overlay.querySelector('#confirmMessage').textContent = message;
  overlay.querySelector('#confirmIcon').className = `confirm-icon ${type}`;
  overlay.querySelector('#confirmIcon i').className = `fas ${icon}`;
  overlay.querySelector('#confirmCancelBtn').textContent = cancelText;
  overlay.querySelector('#confirmOkBtn').textContent = confirmText;
  overlay.querySelector('#confirmOkBtn').className = `btn btn-${type}`;

  const okBtn = overlay.querySelector('#confirmOkBtn');
  const newOkBtn = okBtn.cloneNode(true);
  okBtn.parentNode.replaceChild(newOkBtn, okBtn);
  newOkBtn.addEventListener('click', () => {
    closeModal('confirmModal');
    if (typeof onConfirm === 'function') onConfirm();
  });

  openModal('confirmModal');
}

// =============================================
// TOAST NOTIFICATIONS
// =============================================
function showToast(message, type = 'info', duration = 4000) {
  let container = document.querySelector('.toast-container');
  if (!container) {
    container = document.createElement('div');
    container.className = 'toast-container';
    document.body.appendChild(container);
  }

  const icons = {
    success: 'fa-check-circle',
    danger:  'fa-times-circle',
    warning: 'fa-exclamation-triangle',
    info:    'fa-info-circle',
  };

  const toast = document.createElement('div');
  toast.className = `toast ${type}`;
  toast.innerHTML = `
    <span class="toast-icon"><i class="fas ${icons[type] || icons.info}"></i></span>
    <span class="toast-message">${message}</span>
    <button class="toast-close"><i class="fas fa-times"></i></button>
    <div class="toast-progress" style="animation-duration: ${duration}ms;"></div>
  `;

  container.appendChild(toast);

  toast.querySelector('.toast-close').addEventListener('click', () => removeToast(toast));

  const timer = setTimeout(() => removeToast(toast), duration);
  toast._timer = timer;
}

function removeToast(toast) {
  clearTimeout(toast._timer);
  toast.style.animation = 'none';
  toast.style.opacity = '0';
  toast.style.transform = 'translateX(100%)';
  toast.style.transition = 'all 0.3s ease';
  setTimeout(() => toast.remove(), 300);
}

// =============================================
// DROPDOWN MENUS
// =============================================
function initDropdowns() {
  document.querySelectorAll('[data-dropdown-toggle]').forEach(toggle => {
    toggle.addEventListener('click', (e) => {
      e.stopPropagation();
      const targetId = toggle.dataset.dropdownToggle;
      const menu = document.getElementById(targetId);
      if (!menu) return;

      // Close all others
      document.querySelectorAll('.dropdown-menu.open, .notification-dropdown.open').forEach(m => {
        if (m.id !== targetId) m.classList.remove('open');
      });

      menu.classList.toggle('open');
    });
  });

  // Close on outside click
  document.addEventListener('click', () => {
    document.querySelectorAll('.dropdown-menu.open, .notification-dropdown.open').forEach(m => {
      m.classList.remove('open');
    });
  });
}

// =============================================
// TABLE BULK ACTIONS
// =============================================
function initBulkActions() {
  document.querySelectorAll('table[data-bulk]').forEach(table => {
    const selectAll = table.querySelector('thead input[type="checkbox"]');
    const bulkBar = document.getElementById(table.dataset.bulk);
    const countEl = bulkBar?.querySelector('.bulk-count');

    const updateBulk = () => {
      const checked = table.querySelectorAll('tbody input[type="checkbox"]:checked');
      if (bulkBar) {
        bulkBar.classList.toggle('visible', checked.length > 0);
      }
      if (countEl) {
        countEl.textContent = `${checked.length} عناصر محددة`;
      }
    };

    if (selectAll) {
      selectAll.addEventListener('change', () => {
        table.querySelectorAll('tbody input[type="checkbox"]').forEach(cb => {
          cb.checked = selectAll.checked;
          cb.closest('tr').classList.toggle('selected', selectAll.checked);
        });
        updateBulk();
      });
    }

    table.querySelectorAll('tbody input[type="checkbox"]').forEach(cb => {
      cb.addEventListener('change', () => {
        cb.closest('tr').classList.toggle('selected', cb.checked);
        updateBulk();
        // Update select-all state
        if (selectAll) {
          const all = table.querySelectorAll('tbody input[type="checkbox"]');
          const checked = table.querySelectorAll('tbody input[type="checkbox"]:checked');
          selectAll.checked = all.length > 0 && all.length === checked.length;
          selectAll.indeterminate = checked.length > 0 && checked.length < all.length;
        }
      });
    });
  });

  // Deselect all buttons
  document.querySelectorAll('[data-deselect-all]').forEach(btn => {
    btn.addEventListener('click', () => {
      const tableId = btn.dataset.deselectAll;
      const table = document.getElementById(tableId) || document.querySelector('table[data-bulk]');
      if (!table) return;
      table.querySelectorAll('input[type="checkbox"]').forEach(cb => {
        cb.checked = false;
        cb.closest('tr')?.classList.remove('selected');
      });
      // Reset selectAll
      const selectAll = table.querySelector('thead input[type="checkbox"]');
      if (selectAll) { selectAll.checked = false; selectAll.indeterminate = false; }
      const bulkId = table.dataset.bulk;
      const bulkBar = document.getElementById(bulkId);
      if (bulkBar) bulkBar.classList.remove('visible');
    });
  });
}

// =============================================
// GLOBAL SEARCH
// =============================================
function initGlobalSearch() {
  const input = document.getElementById('globalSearch');
  if (!input) return;

  // Filter current table on input
  input.addEventListener('input', (e) => {
    const q = e.target.value.toLowerCase().trim();
    document.querySelectorAll('.data-table tbody tr').forEach(row => {
      const text = row.textContent.toLowerCase();
      row.style.display = q === '' || text.includes(q) ? '' : 'none';
    });
  });

  // Create Quick Menu Dropdown
  const searchContainer = input.closest('.navbar-search');
  if (searchContainer) {
    const dropdown = document.createElement('div');
    dropdown.className = 'search-dropdown';
    dropdown.innerHTML = `
      <div class="search-section-title">قوائم سريعة</div>
      <a href="../Inventory/Products.html" class="search-item"><i class="fas fa-box"></i> المنتجات (Products)</a>
      <a href="../Accounting/Customers.html" class="search-item"><i class="fas fa-users"></i> العملاء (Clients)</a>
      <a href="../Accounting/Suppliers.html" class="search-item"><i class="fas fa-truck"></i> الموردون (Suppliers)</a>
      <a href="../SalesInvoice/Index.html" class="search-item"><i class="fas fa-file-invoice-dollar"></i> المبيعات (Sales)</a>
    `;
    searchContainer.appendChild(dropdown);

    // Show on double click
    input.addEventListener('dblclick', () => {
      dropdown.classList.add('active');
    });

    // Hide when clicking outside
    document.addEventListener('click', (e) => {
      if (!e.target.closest('.navbar-search')) {
        dropdown.classList.remove('active');
      }
    });
  }
}

// =============================================
// TABS
// =============================================
function initTabs() {
  document.querySelectorAll('.tab[data-tab]').forEach(tab => {
    tab.addEventListener('click', () => {
      const panelId = tab.dataset.tab;
      const tabGroup = tab.closest('[data-tab-group]') || tab.closest('.card');

      // Deactivate all tabs in group
      const siblingTabs = tabGroup
        ? tabGroup.querySelectorAll('.tab[data-tab]')
        : document.querySelectorAll('.tab[data-tab]');
      siblingTabs.forEach(t => t.classList.remove('active'));

      // Activate clicked tab
      tab.classList.add('active');

      // Hide all panels
      let panels = tabGroup ? tabGroup.querySelectorAll('.tab-panel') : [];
      if (panels.length === 0) {
        panels = document.querySelectorAll('.tab-panel');
      }
      panels.forEach(p => p.classList.remove('active'));

      // Show target panel
      const panel = document.getElementById(panelId);
      if (panel) panel.classList.add('active');
    });
  });

  // Activate first tab in each group
  document.querySelectorAll('[data-tab-group]').forEach(group => {
    const firstTab = group.querySelector('.tab[data-tab]');
    if (firstTab && !group.querySelector('.tab.active')) {
      firstTab.click();
    }
  });
}

// =============================================
// PRINT & EXPORT
// =============================================
function printPage() {
  window.print();
}

function exportToExcel(tableId) {
  showToast('جاري التصدير إلى Excel...', 'info', 2000);
  setTimeout(() => {
    // In production: replace with real export logic or call /api/{Module}/ExportExcel
    showToast('تم التصدير بنجاح', 'success');
    // Simulate CSV download
    const table = document.getElementById(tableId) || document.querySelector('.data-table');
    if (!table) return;
    const rows = [];
    table.querySelectorAll('tr').forEach(tr => {
      const cells = [];
      tr.querySelectorAll('th, td').forEach(cell => {
        // Skip checkbox/action columns
        if (!cell.querySelector('input[type="checkbox"]') && !cell.classList.contains('table-actions')) {
          cells.push(`"${cell.textContent.trim().replace(/"/g, '""')}"`);
        }
      });
      if (cells.length) rows.push(cells.join(','));
    });
    const csv = rows.join('\n');
    const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `export_${Date.now()}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  }, 500);
}

function exportToPDF(tableId) {
  showToast('جاري التصدير إلى PDF...', 'info', 2000);
  setTimeout(() => {
    // In production: call /api/{Module}/ExportPDF
    showToast('تم إنشاء ملف PDF بنجاح', 'success');
    window.print(); // Fallback: use print as PDF
  }, 500);
}

// =============================================
// DELETE ROW HELPER
// =============================================
function deleteRow(btn, itemName) {
  confirmDialog(
    `هل أنت متأكد من حذف "${itemName}"? سيتم نقله إلى سلة المحذوفات.`,
    () => {
      const row = btn.closest('tr');
      if (row) {
        row.style.transition = 'opacity 0.3s';
        row.style.opacity = '0';
        setTimeout(() => row.remove(), 300);
        showToast(`تم حذف "${itemName}" بنجاح`, 'success');
      }
    },
    { title: 'تأكيد الحذف', confirmText: 'حذف', cancelText: 'إلغاء', type: 'danger', icon: 'fa-trash' }
  );
}

// =============================================
// NOTIFICATIONS
// =============================================
function initNotifications() {
  // Mock notification data
  const notifications = [
    { id: 1, msg: 'فاتورة INV-2024-0045 متأخرة عن الدفع', time: 'منذ 5 دقائق', icon: 'fa-file-invoice', color: 'danger', unread: true },
    { id: 2, msg: 'مخزون منخفض: لاب توب Dell XPS', time: 'منذ 20 دقيقة', icon: 'fa-box-open', color: 'warning', unread: true },
    { id: 3, msg: 'تم تأكيد الطلب PO-2024-0012', time: 'منذ ساعة', icon: 'fa-check-circle', color: 'success', unread: false },
    { id: 4, msg: 'عميل جديد: شركة الأمل للتجارة', time: 'منذ 3 ساعات', icon: 'fa-user-plus', color: 'info', unread: false },
    { id: 5, msg: 'تقرير نهاية الشهر جاهز للمراجعة', time: 'أمس', icon: 'fa-chart-bar', color: 'primary', unread: false },
  ];

  const list = document.getElementById('notificationList');
  if (list) {
    list.innerHTML = notifications.map(n => `
      <div class="notification-item ${n.unread ? 'unread' : ''}">
        <div class="notification-icon" style="background: var(--${n.color}-light); color: var(--${n.color});">
          <i class="fas ${n.icon}"></i>
        </div>
        <div class="notification-text">
          <div class="notification-msg">${n.msg}</div>
          <div class="notification-time">${n.time}</div>
        </div>
      </div>
    `).join('');
  }

  // Update badge count
  const badge = document.querySelector('.notification-badge');
  const unread = notifications.filter(n => n.unread).length;
  if (badge) badge.textContent = unread;
}

// =============================================
// ACTIVE SECTION LABEL IN SIDEBAR
// =============================================
function initActiveSection() {
  // Expand submenu that contains the active link
  document.querySelectorAll('.nav-submenu').forEach(menu => {
    if (menu.querySelector('.nav-link.active')) {
      menu.classList.add('open');
      const toggle = document.querySelector(`[data-submenu="${menu.id}"]`);
      if (toggle) toggle.classList.add('submenu-open');
    }
  });
}

// =============================================
// DATE HELPERS
// =============================================
function formatDate(date, locale = 'ar-SA') {
  return new Date(date).toLocaleDateString(locale, {
    year: 'numeric', month: 'long', day: 'numeric'
  });
}

function formatCurrency(amount, currency = 'SAR', locale = 'ar-SA') {
  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency: currency,
  }).format(amount);
}

// =============================================
// MAKE FUNCTIONS GLOBAL
// =============================================
window.openModal    = openModal;
window.closeModal   = closeModal;
window.confirmDialog = confirmDialog;
window.showToast    = showToast;
window.printPage    = printPage;
window.exportToExcel = exportToExcel;
window.exportToPDF  = exportToPDF;
window.deleteRow    = deleteRow;
window.formatDate   = formatDate;
window.formatCurrency = formatCurrency;

// =============================================
// INIT
// =============================================
document.addEventListener('DOMContentLoaded', () => {
  initSidebar();
  initTheme();
  initLanguage();
  initModals();
  initDropdowns();
  initBulkActions();
  initGlobalSearch();
  initTabs();
  initNotifications();
  initActiveSection();

  // PROTOTYPE AUTO-BINDER: Make all dead buttons active for the presentation
  document.querySelectorAll('button:not([onclick]):not([type="submit"]):not([data-modal-close]):not([data-dropdown-toggle])').forEach(btn => {
    if (btn.classList.contains('page-btn') || btn.classList.contains('tab') || btn.id === 'confirmOkBtn' || btn.id === 'sidebarToggle' || btn.id === 'themeToggle') return;
    
    if (btn.classList.contains('view') || btn.querySelector('.fa-eye') || btn.querySelector('.fa-file-invoice')) {
      btn.addEventListener('click', () => showPreview('معاينة السجل'));
    } else if (btn.classList.contains('remove-row-btn')) {
      btn.addEventListener('click', function() {
        const tr = this.closest('tr');
        if (tr) {
          tr.style.opacity = '0';
          setTimeout(() => tr.remove(), 300);
        }
      });
    } else if (btn.classList.contains('add-line-btn')) {
      btn.addEventListener('click', () => showToast('تمت إضافة سطر جديد', 'success'));
    } else if (btn.querySelector('.fa-filter')) {
      btn.addEventListener('click', () => showToast('جارٍ تطبيق الفلاتر وتحديث البيانات...', 'info', 1500));
    } else if (btn.querySelector('.fa-file-export')) {
      btn.addEventListener('click', () => exportToExcel());
    } else if (btn.querySelector('.fa-paperclip')) {
      btn.addEventListener('click', () => showToast('لا توجد مرفقات حالياً', 'warning'));
    } else if (btn.classList.contains('btn-danger')) {
      btn.addEventListener('click', () => confirmDialog('هل أنت متأكد من هذا الإجراء؟', () => showToast('تم الحذف بنجاح', 'success')));
    } else {
      btn.addEventListener('click', () => showToast('هذه الميزة قيد التطوير في النموذج المبدئي', 'info'));
    }
  });

  document.querySelectorAll('a[href="#"]:not([onclick])').forEach(link => {
    link.addEventListener('click', (e) => {
      e.preventDefault();
      showToast('هذه الميزة قيد التطوير في النموذج المبدئي', 'info');
    });
  });

  console.log('%c ERP System Initialized ✓', 'color:#1a56db; font-weight:bold; font-size:14px;');
});

// =============================================
// GLOBAL ACTIONS (SAVE, DRAFT, POST, EMAIL)
// =============================================
function saveAsDraft() {
  showToast('تم الحفظ كمسودة بنجاح', 'success');
  setTimeout(() => {
    if (window.location.pathname.toLowerCase().includes('create.html')) {
      window.location.href = 'Index.html';
    }
  }, 1000);
}

function confirmAndPost() {
  let overlay = document.getElementById('saveOptionsModal');
  if (!overlay) {
    overlay = document.createElement('div');
    overlay.id = 'saveOptionsModal';
    overlay.className = 'modal-overlay';
    document.body.appendChild(overlay);
  }
  
  overlay.innerHTML = `
    <div class="modal modal-md">
      <div class="modal-header">
        <h5 class="modal-title"><i class="fas fa-save"></i> خيارات الحفظ</h5>
        <button class="modal-close" onclick="closeModal('saveOptionsModal')"><i class="fas fa-times"></i></button>
      </div>
      <div class="modal-body" style="text-align: center; padding: 30px;">
        <i class="fas fa-question-circle" style="font-size: 3rem; color: var(--primary); margin-bottom: 15px;"></i>
        <h3 style="margin-bottom: 15px; color: #333;">كيف تود حفظ المستند؟</h3>
        <p style="color: #666; margin-bottom: 25px;">يمكنك حفظ المستند فقط، أو حفظه وتحميل نسخة بصيغة PDF فوراً.</p>
        
        <div style="display: flex; gap: 15px; justify-content: center;">
          <button class="btn btn-outline-primary" style="flex: 1; padding: 12px;" onclick="executeSave(false)">
            <i class="fas fa-save"></i> حفظ فقط
          </button>
          <button class="btn btn-primary" style="flex: 1; padding: 12px;" onclick="executeSave(true)">
            <i class="fas fa-file-pdf"></i> حفظ وتحميل PDF
          </button>
        </div>
      </div>
    </div>
  `;
  
  openModal('saveOptionsModal');
}

function executeSave(downloadPDF) {
  closeModal('saveOptionsModal');
  
  if (downloadPDF) {
    downloadFakePDF('فاتورة_معتمدة');
  }
  
  showToast('تم التأكيد والترحيل بنجاح', 'success');
  
  setTimeout(() => {
    if (window.location.pathname.toLowerCase().includes('create.html')) {
      window.location.href = 'Index.html';
    }
  }, downloadPDF ? 1800 : 1000);
}
window.executeSave = executeSave;

function sendInvoiceByEmail() {
  showToast('تم إرسال الفاتورة بالبريد الإلكتروني بنجاح', 'success');
}

window.saveAsDraft = saveAsDraft;
window.confirmAndPost = confirmAndPost;
window.sendInvoiceByEmail = sendInvoiceByEmail;


// =============================================
// PREVIEW MODAL & PDF EXPORT
// =============================================
function downloadFakePDF(title) {
  const link = document.createElement('a');
  // Blank PDF data URI
  link.href = 'data:application/pdf;base64,JVBERi0xLjQKJcOkw7zDtsOfCjIgMCBvYmoKPDwvTGVuZ3RoIDMgMCBSL0ZpbHRlci9GbGF0ZURlY29kZT4+CnN0cmVhbQp4nDPQM1Qo5ypUMFAwALJMLU31jBQsTAz1DBSK0gtSU4uVrAwNDAyMDMwNjI3NjXSMlKwMlDJS0vNSSxPzi1JTFQzMlSwM3A0NdI2MFaByZqYGBsYKoDFGQCkDEwVjMwMzQwMjIyA3tbg4MS+lKDWxWCE/LzU3sSQ1V8FMoZhXwaGoNK8kNTi1OLXIKT8vV6GgIDUvNbm0uKSoRCEnMy9ZIT2/LBHIdvQJcvL19XENdHF0CfXxDPT1c/UNdggO9gt2dg/2d/L0DXL2DQxycvXycXV19/J1Dvb0CXYJcvbxDfT0dvXw8AtycffxDQ4NcvYN9vUPcvYNCfaD2AcAD98/BwplbmRzdHJlYW0KZW5kb2JqCgozIDAgb2JqCjIzNAplbmRvYmoKCjUgMCBvYmoKPDwvTGVuZ3RoIDYgMCBSL0ZpbHRlci9GbGF0ZURlY29kZS9MZW5ndGgxIDc1MzY+PgpzdHJlYW0KeJzFOwlYVNcWPy6MMG8YYFhEAQHFDQUVFRU1UeMWNa6JGneNMca4G2OMu8ZojLvu+5q4xL3FmKixT5OqSVOTNul7X5P72p6+V+/3+P3nO/e+e+4556yz/2utvc8QACwQhRjC8tWrl23Z9/JDAXAAIqw+rVu5wB/eLgB6GZCyduXy9VtfP6+rAC7LhP75q3as3x01+eUoQO5mIL5/48oNa7w5DkcA2S6EnG0bN299863rN0DuDkDmsU1rN6zZ2jQ5GsjZCv0n1q/asPGj3FdjAXHVA8QW27N51U/lBbkAeW+AP71twwY2i+aA2lYAX79q1aq39l+/vASoOgb4e5YtX7Fk+92N64GsH0CfrZcvW1z8dOk2gI4nACn+0g+kGqUq5wD/4W8hR+Fp8Cg8CU/Ck/A0PA1Pw+P8p1b85350+b8a/l8G4Hl4Hp6H5+FFeBFehBfhRXgJngEHgIPwc/g5/Bx+Dj+Hn8PP4efwc/g5/Bx+Dj+Hn8PP4efwc/g5/Bx+Dj+Hn/83818e3v/a0J6O7el4GZ6Gl+Fp+Dn8HH4OP4efw8/h5/Bz+Dn8HH4OP4efw8/h5/Bz+Dn8HH4OP4ef/1/n/zF8fV2D19U1eF1dg9fVNXhdXYPX1TV4XV2D19U1eF1dg9fVNXhdXYPX1TV4XV2D/zfzX77L5eHl4eV4GZ6Gl+Fp+Dn8HH4OP4efw8/h5/Bz+Dn8HH4OP4efw8/h5/Bz+Dn8HH4OP4ef/1/v/2b4//U9Q4N+09Cgr2/Qbxo0NNA3aGgQaGjQbxoEGs4aBBoEGgT6Bg2/Y+B2gGfgaXganoaX4Rn4JTwPT8PT8DQ8A7+E5+FpeBqehmfgl/A8PA1Pw9PwDPwSnoen4Wl4Gp6BX8Lz8DQ8DU/DM/BLeB6ehqfhaXgGfgnPw9PwNDwNz8Av4Xl4Gp6Gp+EZ+CU8D0/D0/A0PAO/hOfhaXgang45Q05X1yFnyBlyhpyurEPOkNMVc7piTlfM6Yo5XTGnnXbMaaeddsxppx1z2mnHnK6YU8Yp45RxyjhlnDJOu8ppVzntKqdd5bSrnHaV065y2lVOu8ppVznt6v/1u6T1DbnD7n7+X4b5P5w/Xf+Z09d/5vT1nzl9/f/P6eN1fX2cPt7/x3E6/R/Haf13jvvvHHf/neP+O8f9d4777xz33zn+1zmO0s/h5/Bz+Dn8HH4OP4efw8/h5/Bz+Dn8HH4OP4efw8/h5/Bz+Dn8HH4OP//vzn+D/+2oK4664qgrjrrSD/pBPywUhkV4Fl6AV+G3sBhvwdvwHrSMDqOQhR2xO87E8TgfF+BCXIKLMRnTMQsX4lJcisvwB1yGy/EXXIErcBVuwJ/xCtyI23AH7sI9uA8P4GE8gsfxCj6HL3AVvsI3+A4HcBCH8SMeRz90QA/0Qm/0QQ/40R9H0A+HcAQHcRQ/xzH4CU7F6TgL5+F8XIALcQkuxhRMgw3TcD4uxKW4FFfgClyBq3AD/oxX4Cbcho/hU/g0/oy/4i+4FXfiLtyDu3Av7sMDeBiP4HG8gs/hC1yFr/ANvsMBHMSHeBx98Xv0Qk/40B/9cAT9cAhHcBAn8HNkMA1nYCYuwhT4YMNcXIhLcSkuxYsxH2/BS/ByXI4/4QpcgaswDz/Eq/FGvB234+94B/6BN+FuvAv34F7chwfwMB7B43gFn8MXuApf4Rt8hwM4iMP4EY/jr/gdTsdvMRXTO2d29uqc1jkrs1NaenpqikNyc/T0k5zEOCEuNto/yNfLw00+cM7f/mYBAw0D5QplbmRzdHJlYW0KZW5kb2JqCgo2IDAgb2JqCjExNTcKZW5kb2JqCgo0IDAgb2JqCjw8L1R5cGUvUGFnZS9NZWRpYUJveFswIDAgNTk1IDg0Ml0vUmVzb3VyY2VzPDwvWE9iamVjdDw8L0ltMSAyIDAgbj4+L1Byb2NTZXRbL1BERi9UZXh0L0ltYWdlQi9JbWFnZUMvSW1hZ2VJXT4+L0NvbnRlbnRzWzUgMCBSXT4+CmVuZG9iagoKMSAwIG9iago8PC9UeXBlL1BhZ2VzL0NvdW50IDEvS2lkc1s0IDAgUl0+PgplbmRvYmoKCjcgMCBvYmoKPDwvVHlwZS9DYXRhbG9nL1BhZ2VzIDEgMCBSPj4KZW5kb2JqCgp4cmVmCjAgOAowMDAwMDAwMDAwIDY1NTM1IGYgCjAwMDAwMDE0MjggMDAwMDAgbiAKMDAwMDAwMDAxNSAwMDAwMCBuIAowMDAwMDAwMjM5IDAwMDAwIG4gCjAwMDAwMDEzMTEgMDAwMDAgbiAKMDAwMDAwMDI1OCAwMDAwMCBuIAowMDAwMDAxMjg5IDAwMDAwIG4gCjAwMDAwMDE0ODUgMDAwMDAgbiAKdHJhaWxlcgo8PC9TaXplIDgvUm9vdCA3IDAgUi9JbmZvPDw+Pj4+CnN0YXJ0eHJlZgoxNTM1CiUlRU9GCg==';
  link.download = title + '.pdf';
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  showToast('تم تحميل المستند كـ PDF بنجاح', 'success');
}
window.downloadFakePDF = downloadFakePDF;


function showPreview(title, contentHTML) {
  let overlay = document.getElementById('previewModal');
  if (!overlay) {
    overlay = document.createElement('div');
    overlay.id = 'previewModal';
    overlay.className = 'modal-overlay';
    document.body.appendChild(overlay);
  }

  const defaultContent = `
    <div style="background: white; border: 1px solid #ddd; padding: 30px; border-radius: 8px; box-shadow: 0 5px 15px rgba(0,0,0,0.05); min-height: 450px; position: relative; margin: 0 auto;">
      
      <!-- HEADER -->
      <div style="display: flex; justify-content: space-between; border-bottom: 2px solid #eee; padding-bottom: 20px; margin-bottom: 25px;">
        <div>
          <h2 style="margin: 0; color: var(--primary); font-size: 1.8rem; font-weight: 800;">شركة كودكس للحلول التقنية</h2>
          <p style="margin: 5px 0 0; color: #777; font-size: 0.95rem;">الرقم الضريبي: 300000000000003</p>
          <p style="margin: 2px 0 0; color: #777; font-size: 0.95rem;">الرياض، المملكة العربية السعودية</p>
        </div>
        <div style="text-align: left;">
          <h3 style="margin: 0; color: #333; font-size: 1.4rem;"> + title + </h3>
          <p style="margin: 8px 0 0; color: #555; font-size: 1rem; font-weight: 600;">رقم المرجع: INV-2026-0042</p>
          <p style="margin: 2px 0 0; color: #777; font-size: 0.95rem;">التاريخ:  + new Date().toLocaleDateString('ar-SA') + </p>
        </div>
      </div>

      <!-- INFO SECTION -->
      <div style="display: flex; justify-content: space-between; margin-bottom: 25px; background: #fafafa; padding: 15px; border-radius: 6px; border: 1px solid #eee;">
        <div>
          <h4 style="margin: 0 0 10px; color: #444; font-size: 1rem;">بيانات العميل / الطرف الآخر:</h4>
          <p style="margin: 0; color: #333; font-weight: 600;">شركة الأفق للتجارة</p>
          <p style="margin: 2px 0 0; color: #666; font-size: 0.9rem;">الرقم الضريبي: 311111111111113</p>
        </div>
        <div style="text-align: left;">
          <h4 style="margin: 0 0 10px; color: #444; font-size: 1rem;">تفاصيل إضافية:</h4>
          <p style="margin: 0; color: #666; font-size: 0.9rem;">شروط الدفع: 30 يوم</p>
          <p style="margin: 2px 0 0; color: #666; font-size: 0.9rem;">تاريخ الاستحقاق: 09/08/2026</p>
        </div>
      </div>

      <!-- TABLE -->
      <table style="width: 100%; border-collapse: collapse; margin-bottom: 25px;">
        <thead>
          <tr style="background: #f1f5f9;">
            <th style="padding: 12px; border: 1px solid #ddd; text-align: right; color: #444;">البيان</th>
            <th style="padding: 12px; border: 1px solid #ddd; text-align: center; color: #444;">الكمية</th>
            <th style="padding: 12px; border: 1px solid #ddd; text-align: center; color: #444;">السعر</th>
            <th style="padding: 12px; border: 1px solid #ddd; text-align: left; color: #444;">الإجمالي</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td style="padding: 12px; border: 1px solid #ddd; color: #333;">خدمات استشارية متقدمة (معاينة)</td>
            <td style="padding: 12px; border: 1px solid #ddd; text-align: center; color: #333;">1</td>
            <td style="padding: 12px; border: 1px solid #ddd; text-align: center; color: #333;">1,500.00 ر.س</td>
            <td style="padding: 12px; border: 1px solid #ddd; text-align: left; color: #333;">1,500.00 ر.س</td>
          </tr>
          <tr>
            <td style="padding: 12px; border: 1px solid #ddd; color: #333;">تطوير نظام برمجيات (الدفعة الأولى)</td>
            <td style="padding: 12px; border: 1px solid #ddd; text-align: center; color: #333;">1</td>
            <td style="padding: 12px; border: 1px solid #ddd; text-align: center; color: #333;">3,500.00 ر.س</td>
            <td style="padding: 12px; border: 1px solid #ddd; text-align: left; color: #333;">3,500.00 ر.س</td>
          </tr>
        </tbody>
      </table>

      <!-- TOTALS -->
      <div style="display: flex; justify-content: flex-end; flex-direction: row-reverse;">
        <div style="width: 280px; background: #fafafa; padding: 20px; border-radius: 6px; border: 1px solid #eee;">
          <div style="display: flex; justify-content: space-between; margin-bottom: 12px; color: #555;">
            <strong>5,000.00 ر.س</strong> <span>المجموع الفرعي:</span> 
          </div>
          <div style="display: flex; justify-content: space-between; margin-bottom: 12px; color: #555;">
            <strong>750.00 ر.س</strong> <span>الضريبة (15%):</span> 
          </div>
          <div style="display: flex; justify-content: space-between; border-top: 2px solid #ddd; padding-top: 15px; color: var(--primary); font-size: 1.2rem; font-weight: 700;">
            <strong>5,750.00 ر.س</strong> <span>الإجمالي:</span> 
          </div>
        </div>
      </div>
      
      <!-- FOOTER TEXT -->
      <div style="margin-top: 40px; text-align: center; color: #888; font-size: 0.85rem; border-top: 1px solid #eee; padding-top: 15px;">
        <p style="margin: 0;">شكراً لتعاملكم معنا. تم إنشاء هذا المستند آلياً من النظام.</p>
      </div>

    </div>
  `;
  
  overlay.innerHTML = `
    <div class="modal modal-xl">
      <div class="modal-header">
        <h5 class="modal-title"><i class="fas fa-file-invoice"></i> ` + title + `</h5>
        <button class="modal-close" onclick="closeModal('previewModal')"><i class="fas fa-times"></i></button>
      </div>
      <div class="modal-body" style="background-color: #e2e8f0; padding: 30px;">
        ` + (contentHTML || defaultContent) + `
      </div>
      <div class="modal-footer" style="justify-content: space-between;">
        <div>
           <button class="btn btn-secondary" onclick="closeModal('previewModal')">إغلاق</button>
        </div>
        <div>
           <button class="btn btn-outline-secondary" onclick="printPage()"><i class="fas fa-print"></i> طباعة</button>
           <button class="btn btn-primary" onclick="downloadFakePDF('` + title + `')"><i class="fas fa-file-pdf"></i> تحميل كـ PDF</button>
        </div>
      </div>
    </div>
  `;
  
  openModal('previewModal');
}
window.showPreview = showPreview;