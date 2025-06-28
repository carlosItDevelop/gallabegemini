// Global Variables
let currentTheme = 'dark';
let calendar;
let charts = {};
let leads = [];
let tasks = [];
let logs = [];
let draggedCard = null;

// API Base URL
/*const API_BASE = '/api';*/
const API_BASE = '/Crm/api';



// Initialize Application
document.addEventListener('DOMContentLoaded', function () {
    initializeApp();
    setupEventListeners();
    initializeCharts();
    loadData(); // Função principal para carregar todos os dados iniciais
});

function initializeApp() {
    const savedTheme = localStorage.getItem('theme') || 'dark';
    setTheme(savedTheme);
    showTab('dashboard');
}

function setupEventListeners() {
    // Navegação por Abas
    document.querySelectorAll('[data-tab]').forEach(item => {
        item.addEventListener('click', (e) => {
            e.preventDefault();
            const tab = e.currentTarget.getAttribute('data-tab');
            showTab(tab);
            updateActiveNav(e.currentTarget);
        });
    });

    // Filtros da Lista de Leads
    const leadsSearchInput = document.getElementById('searchInput');
    if (leadsSearchInput) leadsSearchInput.addEventListener('input', debounce(filterLeads, 300));

    const statusFilter = document.getElementById('statusFilter');
    if (statusFilter) statusFilter.addEventListener('change', filterLeads);

    const responsibleFilter = document.getElementById('responsibleFilter');
    if (responsibleFilter) responsibleFilter.addEventListener('change', filterLeads);

    // O botão agora chama a função correta
    const filterButton = document.querySelector('#leads-list .btn');
    if (filterButton && filterButton.textContent.includes('Filtrar')) {
        filterButton.onclick = filterLeads;
    }


    // Filtros da Lista de Tarefas
    document.querySelectorAll('#tasks .filter-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            document.querySelectorAll('#tasks .filter-btn').forEach(b => b.classList.remove('active'));
            e.target.classList.add('active');
            filterTasks(e.target.getAttribute('data-filter'));
        });
    });

    // Kanban Drag & Drop
    setupKanbanDragDrop();
}


// >>> COLE ESTA NOVA FUNÇÃO NO LUGAR DA ANTIGA loadSampleData <<<
async function loadData() {
    try {
        console.log('Carregando dados do servidor .NET...');
        showNotification('Carregando dados do CRM...', 'info', 2000);

        // Carregar todos os dados da nossa API em paralelo
        const [leadsData, tasksData, activitiesData, logsData] = await Promise.all([
            fetchFromAPI('/leads'),
            fetchFromAPI('/tasks'),
            fetchFromAPI('/activities'),
            fetchFromAPI('/logs')
        ]);

        // Atribuir aos nossos arrays globais
        leads = leadsData;
        tasks = tasksData;
        logs = logsData;

        console.log('Dados carregados com sucesso:', { leads: leads.length, tasks: tasks.length, logs: logs.length });

        // Agora, renderizar todas as partes da UI com os dados reais
        await loadAllLeadNotes(); // Mantido para carregar notas relacionadas
        renderLeadsTable();
        renderKanbanBoard();
        renderTasksList();
        renderLogsTimeline();
        initializeCalendar(activitiesData);
        loadImportantActions();

    } catch (error) {
        // Se a API falhar, agora apenas mostramos um erro claro.
        console.error('Erro fatal ao carregar dados:', error);
        showNotification('Não foi possível carregar os dados do CRM. Verifique a conexão com o servidor.', 'error');
    }
}

// Função para fazer requisições à API
async function fetchFromAPI(endpoint, options = {}) {
    try {
        const response = await fetch(API_BASE + endpoint, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('API Error Response:', errorText);

            // Tentar extrair mensagem de erro limpa
            let cleanErrorMessage = 'Erro interno do servidor';

            try {
                // Se a resposta é JSON, extrair a mensagem do campo 'error'
                if (errorText.startsWith('{')) {
                    const errorJson = JSON.parse(errorText);
                    if (errorJson.error) {
                        cleanErrorMessage = errorJson.error;
                    }
                }
            } catch (parseError) {
                // Se não conseguir fazer parse, usar a mensagem padrão
                console.error('Erro ao fazer parse da resposta de erro:', parseError);
            }

            // Criar um erro customizado apenas com a mensagem limpa
            const customError = new Error(cleanErrorMessage);
            customError.status = response.status;
            customError.originalMessage = errorText;
            throw customError;
        }

        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            return await response.json();
        } else {
            return await response.text();
        }
    } catch (error) {
        console.error('Erro na API:', error.message || error);

        // Se não for um erro customizado, mostrar notificação genérica
        if (!error.status) {
            showNotification('Erro na comunicação com o servidor', 'error');
        }

        throw error;
    }
}

// Theme Management
function toggleTheme() {
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    setTheme(newTheme);
}

function setTheme(theme) {
    currentTheme = theme;
    document.body.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);

    const themeIcon = document.getElementById('theme-icon');
    if (themeIcon) {
        themeIcon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    }
}

// Navigation
function showTab(tabName) {
    // Hide all tabs
    document.querySelectorAll('.tab-content').forEach(tab => {
        tab.classList.remove('active');
    });

    // Show selected tab
    const targetTab = document.getElementById(tabName);
    if (targetTab) {
        targetTab.classList.add('active');
    }

    // Refresh content based on tab
    switch (tabName) {
        case 'calendar':
            if (calendar) calendar.render();
            break;
        case 'reports':
            updateCharts();
            initializeClassicReports();
            break;
    }
}

function updateActiveNav(activeItem) {
    document.querySelectorAll('.nav-tab').forEach(item => {
        item.classList.remove('active');
    });
    activeItem.classList.add('active');
}

// Leads Management
function renderLeadsTable() {
    const tbody = document.getElementById('leadsTableBody');
    if (!tbody) return;

    tbody.innerHTML = leads.map(lead => `
        <tr style="cursor: pointer;" onclick="openLeadDetails(${lead.id})">
            <td>${lead.name}</td>
            <td>${lead.company}</td>
            <td>${lead.email}</td>
            <td>${lead.phone}</td>
            <td><span class="status-badge status-${lead.status}">${getStatusLabel(lead.status)}</span></td>
            <td>${lead.responsible}</td>
            <td>${formatDate(lead.lastContact || lead.lastContact)}</td>
            <td>${lead.score}</td>
            <td>
                <button class="btn btn-sm btn-primary" onclick="editLead(${lead.id}); event.stopPropagation();" title="Editar lead">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteLead(${lead.id}); event.stopPropagation();" title="Excluir lead">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

// FUNÇÃO DE RENDERIZAÇÃO DO KANBAN CORRIGIDA
function renderKanbanBoard() {
    const statuses = ['novo', 'contato', 'qualificado', 'proposta', 'negociacao', 'ganho', 'perdido'];

    statuses.forEach(status => {
        const container = document.getElementById(`${status}-cards`);
        if (!container) return;

        // A comparação com toLowerCase() que fizemos antes continua válida e importante
        const statusLeads = leads.filter(lead => lead.status.toLowerCase() === status);

        container.innerHTML = statusLeads.map(lead => `
            <div class="kanban-card" draggable="true" data-lead-id="${lead.id}">
                <h4>${lead.name}</h4>
                <p><i class="fas fa-building"></i> ${lead.company || 'N/A'}</p>
                <p><i class="fas fa-dollar-sign"></i> R$ ${formatCurrency(lead.value)}</p>
                <p><i class="fas fa-user"></i> ${lead.responsible || 'N/A'}</p>
                <div class="card-actions">
                    <button class="btn-icon" onclick="scheduleActivity('${lead.id}')" title="Agendar atividade"><i class="fas fa-calendar-plus"></i></button>
                    <button class="btn-icon" onclick="addNote('${lead.id}')" title="Adicionar nota"><i class="fas fa-sticky-note"></i></button>
                    <button class="btn-icon" onclick="editLead('${lead.id}')" title="Editar lead"><i class="fas fa-edit"></i></button>
                </div>
            </div>
        `).join('');

        const countElement = document.getElementById(`count-${status}`);
        if (countElement) countElement.textContent = statusLeads.length;
    });
}

function setupKanbanDragDrop() {
    document.addEventListener('dragstart', (e) => {
        if (e.target.classList.contains('kanban-card')) {
            draggedCard = e.target;
            e.target.style.opacity = '0.5';
        }
    });

    document.addEventListener('dragend', (e) => {
        if (e.target.classList.contains('kanban-card')) {
            e.target.style.opacity = '1';
            draggedCard = null;
        }
    });

    document.querySelectorAll('.column-cards').forEach(column => {
        column.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.currentTarget.classList.add('drag-over');
        });

        column.addEventListener('dragleave', (e) => {
            e.currentTarget.classList.remove('drag-over');
        });

        column.addEventListener('drop', (e) => {
            e.preventDefault();
            e.currentTarget.classList.remove('drag-over');

            if (draggedCard) {
                const leadId = draggedCard.getAttribute('data-lead-id'); // <-- APENAS REMOVA O parseInt()
                const newStatus = e.currentTarget.id.replace('-cards', '');
                updateLeadStatus(leadId, newStatus);
            }
        });
    });
}

// ATUALIZAÇÃO DO STATUS DO LEAD NO KANBAN CORRIGIDA
async function updateLeadStatus(leadId, newStatus) {
    const lead = leads.find(l => l.id === leadId);
    if (!lead) return;

    const oldStatus = lead.status;
    lead.status = newStatus; // Atualização otimista da UI
    renderKanbanBoard(); // Re-renderiza para mover o card

    try {
        await fetchFromAPI(`/leads/${leadId}`, {
            method: 'PUT',
            body: JSON.stringify(lead) // Envia o objeto 'lead' inteiro atualizado
        });
        showNotification('Status do lead atualizado!', 'success');
    } catch (error) {
        lead.status = oldStatus; // Reverte em caso de erro
        renderKanbanBoard();
        showNotification('Falha ao atualizar status.', 'error');
    }
}


// Tasks Management
let currentTasksPage = 1;
const tasksPerPage = 8;
let currentTaskFilter = 'all';
let currentTaskSort = 'order';
let sortDirection = 'asc';
let advancedFilters = {};
let currentTaskId = null;


// FUNÇÃO DE RENDERIZAÇÃO DAS TAREFAS CORRIGIDA
function renderTasksList() {
    const tasksList = document.getElementById('tasksList');
    if (!tasksList) return;

    let filteredTasks = tasks.filter(task => {
        switch (currentTaskFilter) {
            case 'pending':
                return task.status.toLowerCase() === 'pendente'; // Compara com o nome do membro C# em minúsculas
            case 'completed':
                return task.status.toLowerCase() === 'concluida';
            case 'overdue':
                const dueDate = new Date(task.dueDate);
                return task.status.toLowerCase() === 'pendente' && dueDate < new Date();
            default:
                return true;
        }
    });

    // Apply advanced filters
    if (Object.keys(advancedFilters).length > 0) {
        filteredTasks = filteredTasks.filter(task => {
            if (advancedFilters.startDate && task.dueDate < advancedFilters.startDate) return false;
            if (advancedFilters.endDate && task.dueDate > advancedFilters.endDate) return false;
            if (advancedFilters.assignee && task.assignee !== advancedFilters.assignee) return false;
            if (advancedFilters.priority && task.priority !== advancedFilters.priority) return false;
            return true;
        });
    }

    // Apply sorting
    filteredTasks.sort((a, b) => {
        let valueA = a[currentTaskSort] || '';
        let valueB = b[currentTaskSort] || '';

        if (currentTaskSort === 'dueDate') {
            valueA = new Date(a.dueDate || a.dueDate);
            valueB = new Date(b.dueDate || b.dueDate);
        } else if (currentTaskSort === 'priority') {
            const priorityOrder = { high: 3, medium: 2, low: 1 };
            valueA = priorityOrder[a.priority] || 0;
            valueB = priorityOrder[b.priority] || 0;
        } else if (currentTaskSort === 'progress') {
            valueA = a.progress || 0;
            valueB = b.progress || 0;
        }

        if (sortDirection === 'asc') {
            return valueA > valueB ? 1 : -1;
        } else {
            return valueA < valueB ? 1 : -1;
        }
    });

    // Calculate pagination
    const totalPages = Math.ceil(filteredTasks.length / tasksPerPage);
    const startIndex = (currentTasksPage - 1) * tasksPerPage;
    const endIndex = startIndex + tasksPerPage;
    const paginatedTasks = filteredTasks.slice(startIndex, endIndex);

    tasksList.innerHTML = `
        <div class="tasks-content" id="sortableTasks">
            ${paginatedTasks.map(task => renderTaskItem(task)).join('')}
        </div>
        <div class="tasks-pagination">
            <button class="btn btn-sm btn-secondary" ${currentTasksPage === 1 ? 'disabled' : ''} onclick="changeTasksPage(${currentTasksPage - 1})">
                <i class="fas fa-chevron-left"></i> Anterior
            </button>
            <span class="pagination-info">Página ${currentTasksPage} de ${totalPages || 1}</span>
            <button class="btn btn-sm btn-secondary" ${currentTasksPage === totalPages || totalPages === 0 ? 'disabled' : ''} onclick="changeTasksPage(${currentTasksPage + 1})">
                Próxima <i class="fas fa-chevron-right"></i>
            </button>
        </div>
    `;

    // Enable drag and drop if sorted by order
    if (currentTaskSort === 'order') {
        enableTaskDragAndDrop();
    }
}

// ✅ VERSÃO CORRIGIDA com aspas nos IDs dos onclick
function renderTaskItem(task) {
    const dueDate = new Date(task.dueDate || task.dueDate);
    const today = new Date();
    const isOverdue = task.status.toLowerCase() === 'pendente' && dueDate < today;
    const progress = task.progress || 0;

    // Verificar se a tarefa tem anexos e comentários
    const hasAttachments = task.attachment_count && task.attachment_count > 0;
    const hasComments = task.comment_count && task.comment_count > 0;

    return `
        <div class="task-item" data-task-id="${task.id}" data-status="${task.status}" draggable="${currentTaskSort === 'order'}">
            <input type="checkbox" class="task-checkbox" ${task.status.toLowerCase() === 'concluida' ? 'checked' : ''}
                   onchange="toggleTaskStatus('${task.id}')">
            <div class="task-content">
                <div class="task-header">
                    <div class="task-title" style="cursor: pointer; color: var(--primary-color);" onclick="openTaskDetailsModal('${task.id}')">
                        ${task.title}
                        <span class="priority-badge ${task.priority}">${getPriorityLabel(task.priority)}</span>
                        ${hasAttachments ? '<i class="fas fa-paperclip task-indicator attachment-indicator" title="Tem anexos"></i>' : ''}
                        ${hasComments ? '<i class="fas fa-comment task-indicator comment-indicator" title="Tem comentários"></i>' : ''}
                    </div>
                    <div class="task-actions">
                        <button class="task-action-btn" onclick="openTaskDetailsModal('${task.id}')" title="Ver detalhes">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="task-action-btn" onclick="editTaskInline('${task.id}')" title="Editar">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="task-action-btn" onclick="deleteTaskWithConfirmation('${task.id}')" title="Excluir">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
                <div class="task-description">${task.description || ''}</div>
                <div class="task-progress">
                    <div class="task-progress-bar">
                        <div class="task-progress-fill" style="width: ${progress}%"></div>
                    </div>
                    <span class="task-progress-text">${progress}%</span>
                </div>
            </div>
            <div class="task-meta">
                <div class="task-date ${isOverdue ? 'overdue' : ''}">
                    <i class="fas fa-calendar"></i> ${formatDate(task.dueDate || task.dueDate)}
                </div>
                <div class="task-assignee">
                    <i class="fas fa-user"></i> ${task.assignee}
                </div>
            </div>
        </div>
    `;
}

function getPriorityLabel(priority) {
    const labels = {
        high: 'Alta',
        medium: 'Média',
        low: 'Baixa'
    };
    return labels[priority] || priority;
}

function openTaskDetails(taskId) {
    const task = tasks.find(t => t.id === taskId);
    if (!task) {
        showNotification('Tarefa não encontrada', 'error');
        return;
    }

    // Buscar informações do lead relacionado, se existir
    const lead = task.leadId ? leads.find(l => l.id === task.leadId) : null;

    // Definir cores baseadas no status e prioridade
    const statusColors = {
        completed: '#10b981',
        pending: '#f59e0b'
    };

    const priorityColors = {
        high: '#ef4444',
        medium: '#f59e0b',
        low: '#10b981'
    };

    const statusLabels = {
        completed: 'Concluída',
        pending: 'Pendente'
    };

    const priorityLabels = {
        high: 'Alta',
        medium: 'Média',
        low: 'Baixa'
    };

    const statusColor = statusColors[task.status] || '#6b7280';
    const priorityColor = priorityColors[task.priority] || '#6b7280';
    const statusLabel = statusLabels[task.status] || task.status;
    const priorityLabel = priorityLabels[task.priority] || task.priority;

    // Verificar se a tarefa está atrasada
    const dueDate = new Date(task.dueDate);
    const today = new Date();
    const isOverdue = task.status === 'pending' && dueDate < today;

    // Calcular dias restantes ou em atraso
    const timeDiff = Math.ceil((dueDate - today) / (1000 * 60 * 60 * 24));
    const timeStatus = isOverdue ?
        `${Math.abs(timeDiff)} dia${Math.abs(timeDiff) !== 1 ? 's' : ''} em atraso` :
        timeDiff === 0 ? 'Vence hoje' :
            timeDiff === 1 ? 'Vence amanhã' :
                `${timeDiff} dia${timeDiff !== 1 ? 's' : ''} restante${timeDiff !== 1 ? 's' : ''}`;

    Swal.fire({
        title: 'Detalhes da Tarefa',
        html: `
            <div style="text-align: left; padding: 20px;">
                <div style="margin-bottom: 20px; display: flex; align-items: center; gap: 12px;">
                    <div style="width: 40px; height: 40px; border-radius: 20px; background-color: ${statusColor}; color: white; display: flex; align-items: center; justify-content: center;">
                        <i class="fas fa-${task.status === 'completed' ? 'check' : 'clock'}"></i>
                    </div>
                    <div>
                        <h3 style="margin: 0; color: var(--text-primary); font-size: 18px;">${task.title}</h3>
                        <span style="background-color: ${statusColor}20; color: ${statusColor}; padding: 4px 8px; border-radius: 12px; font-size: 12px; font-weight: 600;">
                            ${statusLabel}
                        </span>
                    </div>
                </div>

                ${task.description ? `
                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">Descrição:</strong><br>
                    <p style="margin: 8px 0; color: var(--text-secondary); background-color: var(--bg-secondary); padding: 12px; border-radius: 6px; border-left: 3px solid ${statusColor};">
                        ${task.description}
                    </p>
                </div>
                ` : ''}

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">Data de Vencimento:</strong><br>
                    <div style="display: flex; align-items: center; gap: 8px; margin-top: 6px;">
                        <span style="color: var(--text-secondary); font-family: monospace;">
                            ${formatDate(task.dueDate)}
                        </span>
                        <span style="color: ${isOverdue ? '#ef4444' : timeDiff <= 1 ? '#f59e0b' : '#10b981'}; font-size: 12px; font-weight: 600; background-color: ${isOverdue ? '#ef444420' : timeDiff <= 1 ? '#f59e0b20' : '#10b98120'}; padding: 2px 6px; border-radius: 8px;">
                            ${timeStatus}
                        </span>
                    </div>
                </div>

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">Prioridade:</strong><br>
                    <div style="display: flex; align-items: center; gap: 8px; margin-top: 6px;">
                        <div style="width: 12px; height: 12px; background-color: ${priorityColor}; border-radius: 50%;"></div>
                        <span style="color: ${priorityColor}; font-weight: 600;">
                            ${priorityLabel}
                        </span>
                    </div>
                </div>

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">Responsável:</strong><br>
                    <span style="color: var(--text-secondary);">
                        ${task.assignee}
                    </span>
                </div>

                ${lead ? `
                <div style="margin-bottom: 15px; padding: 12px; background-color: var(--bg-secondary); border-radius: 6px;">
                    <strong style="color: var(--text-primary);">Lead Relacionado:</strong><br>
                    <div style="margin-top: 8px;">
                        <span style="color: var(--primary-color); font-weight: 600; cursor: pointer;" onclick="openLeadDetails(${lead.id}); Swal.close();">
                            ${lead.name}
                        </span>
                        <br>
                        <span style="color: var(--text-secondary); font-size: 14px;">
                            ${lead.company} • ${lead.email}
                        </span>
                        <br>
                        <span class="status-badge status-${lead.status}" style="font-size: 11px; margin-top: 4px; display: inline-block;">
                            ${getStatusLabel(lead.status)}
                        </span>
                    </div>
                </div>
                ` : ''}

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">ID da Tarefa:</strong><br>
                    <span style="color: var(--text-muted); font-family: monospace; font-size: 12px;">
                        #${task.id}
                    </span>
                </div>
            </div>
        `,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: task.status === 'completed' ? 'Marcar como Pendente' : 'Marcar como Concluída',
        cancelButtonText: 'Fechar',
        confirmButtonColor: statusColor,
        cancelButtonColor: '#6b7280',
        width: '600px',
        background: currentTheme === 'dark' ? '#1e293b' : '#ffffff',
        color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b',
        customClass: {
            popup: 'task-details-modal'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            toggleTaskStatus(taskId);
        }
    });
}

function filterTasks(filter) {
    currentTaskFilter = filter;
    currentTasksPage = 1; // Reset to first page when filtering
    renderTasksList();
}

function changeTasksPage(page) {
    // Apply current filter to get filtered tasks count
    let filteredTasks = tasks.filter(task => {
        switch (currentTaskFilter) {
            case 'pending':
                return task.status === 'pending';
            case 'completed':
                return task.status === 'completed';
            case 'overdue':
                const dueDate = new Date(task.dueDate || task.dueDate);
                const today = new Date();
                return task.status === 'pending' && dueDate < today;
            default:
                return true;
        }
    });

    const totalPages = Math.ceil(filteredTasks.length / tasksPerPage);
    if (page < 1 || page > totalPages) return;

    currentTasksPage = page;
    renderTasksList();
}

async function toggleTaskStatus(taskId) {
    const task = tasks.find(t => t.id === taskId);
    if (task) {
        const newStatus = task.status === 'completed' ? 'pending' : 'completed';

        try {
            await fetchFromAPI(`/tasks/${taskId}/status`, {
                method: 'PUT',
                body: JSON.stringify({ status: newStatus })
            });

            addLog({
                type: 'task',
                title: 'Tarefa atualizada',
                description: `Tarefa "${task.title}" marcada como ${newStatus === 'completed' ? 'concluída' : 'pendente'}`,
                userId: 'Usuário Atual',
                leadId: task.leadId
            });

            // Recarregar dados
            await loadSampleData();
            showNotification('Status da tarefa atualizado!', 'success');
        } catch (error) {
            console.error('Erro ao atualizar tarefa:', error);
            showNotification('Erro ao atualizar tarefa', 'error');
        }
    }
}

// Calendar Management
function getTemplateColor(eventType) {
    const colors = {
        'novo': '#6366f1',
        'contato': '#f59e0b',
        'qualificado': '#10b981',
        'proposta': '#8b5cf6',
        'negociacao': '#ef4444'
    }; return colors[eventType] || '#3b82f6';
}

async function initializeCalendar() {
    const calendarEl = document.getElementById('calendar-widget');
    if (!calendarEl) return;

    // Carregar atividades do banco com retry se necessário
    let activities = [];
    try {
        activities = await fetchFromAPI('/activities');
        console.log('Atividades carregadas:', activities.length);
    } catch (error) {
        console.error('Erro ao carregar atividades:', error);
        activities = [];
        showNotification('Erro ao carregar atividades da agenda', 'warning');
    }

    // Converter atividades para eventos do calendário
    const calendarEvents = activities.map(activity => ({
        id: activity.id.toString(),
        title: activity.title,
        start: activity.scheduledDate, // ✅ CORREÇÃO
        backgroundColor: getEventColor(activity.type),
        borderColor: getEventColor(activity.type),
        extendedProps: {
            leadId: activity.leadId,
            type: activity.type,
            description: activity.description
        }
    }));

    calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'pt-br',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },
        buttonText: {
            today: 'Hoje',
            month: 'Mês',
            week: 'Semana',
            day: 'Dia',
            list: 'Lista'
        },
        dayMaxEvents: 3,
        eventDisplay: 'block',
        displayEventTime: true,
        allDaySlot: false,
        slotMinTime: '07:00:00',
        slotMaxTime: '19:00:00',
        height: 'auto',
        aspectRatio: 1.8,
        eventColor: '#3b82f6',
        eventBorderColor: '#3b82f6',
        eventTextColor: '#ffffff',
        events: calendarEvents,
        droppable: true,
        dropAccept: '.event-template',
        drop: function (info) {
            console.log('🎯 Drop detectado no calendário!', {
                date: info.date,
                draggedEl: info.draggedEl,
                eventType: info.draggedEl.getAttribute('data-event-type')
            });

            const eventType = info.draggedEl.getAttribute('data-event-type');

            if (eventType) {
                // Usar a data exata onde foi solto
                const dropDate = new Date(info.date);

                // Se for uma visualização de dia ou semana, manter a hora. Caso contrário, definir uma hora padrão
                if (calendar.view.type === 'dayGridMonth') {
                    dropDate.setHours(9, 0, 0, 0); // 9:00 AM como padrão para vista mensal
                }

                console.log('📅 Criando evento na data:', dropDate);
                createEventFromTemplate(eventType, dropDate);
            }
        },
        eventClick: function (info) {
            const event = info.event;
            const props = event.extendedProps;

            Swal.fire({
                title: event.title,
                html: `
                    <div style="text-align: left;">
                        <p><strong>Início:</strong> ${event.start.toLocaleString('pt-BR')}</p>
                        ${event.end ? `<p><strong>Término:</strong> ${event.end.toLocaleString('pt-BR')}</p>` : ''}
                        <p><strong>Tipo:</strong> ${props.type || 'Atividade'}</p>
                        ${props.description ? `<p><strong>Descrição:</strong> ${props.description}</p>` : ''}
                        ${props.leadId ? `<p><strong>Lead ID:</strong> ${props.leadId}</p>` : ''}
                    </div>
                `,
                icon: 'info',
                showCancelButton: true,
                showDenyButton: true,
                confirmButtonText: 'Editar',
                denyButtonText: 'Excluir',
                cancelButtonText: 'Fechar',
                confirmButtonColor: '#3b82f6',
                denyButtonColor: '#ef4444',
                cancelButtonColor: '#6b7280',
                background: currentTheme === 'dark' ? '#1e293b' : '#ffffff',
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }).then((result) => {
                if (result.isConfirmed) {
                    openEditEventModal(event);
                } else if (result.isDenied) {
                    confirmDeleteEvent(event);
                }
            });
        },
        // ✅ SUBSTITUA TODA A PROPRIEDADE dateClick POR ESTA:
        dateClick: function (info) {
            // 1. Chama a função que já criamos, que sabe como abrir o modal corretamente
            // Passamos a data clicada para que o formulário seja preenchido com ela.
            openEventModal(info.dateStr);
        },
        eventDidMount: function (info) {
            // Add tooltip
            info.el.setAttribute('title', info.event.title + '\n' + (info.event.extendedProps.description || ''));
        }
    });

    // Renderizar o calendário primeiro
    calendar.render();

    // Configurar drag and drop externo
    setupCalendarDragDrop();
}

function setupCalendarDragDrop() {
    // Aguardar o calendário estar totalmente carregado
    if (!calendar) {
        console.log('⚠️ Calendário não inicializado, aguardando...');
        setTimeout(setupCalendarDragDrop, 100);
        return;
    }

    // Importar Draggable do FullCalendar
    const { Draggable } = FullCalendar;

    const containerEl = document.querySelector('.predefined-events');
    if (!containerEl) {
        console.log('⚠️ Container de eventos não encontrado');
        return;
    }

    // Inicializar FullCalendar Draggable
    new Draggable(containerEl, {
        itemSelector: '.event-template',
        eventData: function (eventEl) {
            const eventType = eventEl.getAttribute('data-event-type');
            const title = eventEl.querySelector('.event-title').textContent;

            console.log('🎯 Configurando dados do evento:', { eventType, title });

            return {
                title: title,
                backgroundColor: getTemplateColor(eventType),
                borderColor: getTemplateColor(eventType),
                extendedProps: {
                    eventType: eventType,
                    isExternal: true
                }
            };
        }
    });

    console.log('✅ FullCalendar Draggable configurado com sucesso!');
}

async function createEventFromTemplate(eventType, date) {
    console.log('Criando evento do template:', { eventType, date: date.toISOString() });

    const eventTemplates = {
        'novo': {
            title: 'Novo Lead - Contato Inicial',
            type: 'call',
            color: '#6366f1',
            description: 'Primeiro contato com novo lead'
        },
        'contato': {
            title: 'Primeiro Contato',
            type: 'call',
            color: '#f59e0b',
            description: 'Ligação ou email para primeiro contato'
        },
        'qualificado': {
            title: 'Reunião de Qualificação',
            type: 'meeting',
            color: '#10b981',
            description: 'Reunião para qualificar o lead'
        },
        'proposta': {
            title: 'Apresentar Proposta',
            type: 'meeting',
            color: '#8b5cf6',
            description: 'Apresentação ou envio da proposta comercial'
        },
        'negociacao': {
            title: 'Reunião de Negociação',
            type: 'meeting',
            color: '#ef4444',
            description: 'Discussão de termos e condições'
        }
    };

    const template = eventTemplates[eventType];
    if (!template || !calendar) {
        console.error('Template não encontrado ou calendário não inicializado:', { eventType, template, calendar });
        return;
    }

    const startDate = new Date(date);
    console.log('Data final do evento:', startDate.toISOString());

    const activityData = {
        leadId: null, // <--- MUDANÇA
        type: template.type,
        title: template.title,
        description: template.description,
        scheduledDate: startDate.toISOString() // <--- MUDANÇA
    };

    try {
        // Salvar no banco
        const newActivity = await fetchFromAPI('/activities', {
            method: 'POST',
            body: JSON.stringify(activityData)
        });

        // O FullCalendar já criou o evento visual durante o drag & drop
        // Precisamos apenas atualizar as propriedades do evento existente
        const events = calendar.getEvents();
        const newEvent = events[events.length - 1]; // O último evento criado

        if (newEvent && !newEvent.id) {
            // Atualizar o evento temporário com os dados reais do banco
            newEvent.setProp('id', newActivity.id.toString());
            newEvent.setExtendedProp('leadId', newActivity.leadId);
            newEvent.setExtendedProp('type', newActivity.type);
            newEvent.setExtendedProp('description', newActivity.description);
        }

        // Log da atividade
        await addLog({
            type: 'meeting',
            title: 'Atividade agendada via template',
            description: `${template.title} agendada para ${formatDateTime(startDate)}`,
            userId: 'Usuário Atual',
            leadId: null
        });

        showNotification(`${template.title} agendada com sucesso!`, 'success');

    } catch (error) {
        console.error('Erro ao salvar atividade via template:', error);
        showNotification('Erro ao salvar atividade', 'error');
    }
}

// Charts Management
function initializeCharts() {
    initializeFunnelChart();
    initializeSourceChart();
    initializeSalesChart();
    initializeConversionChart();
    initializeActivityChart();
    initializePipelineTimeChart();
}

function initializeFunnelChart() {
    const element = document.getElementById('funnelChart');
    if (!element) return;

    const options = {
        series: [{
            name: 'Quantidade de Leads',
            data: [150, 120, 80, 45, 25, 15]
        }],
        chart: {
            type: 'bar',
            height: 350,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '60%',
                borderRadius: 4
            }
        },
        dataLabels: {
            enabled: false
        },
        colors: ['#6366f1', '#f59e0b', '#10b981', '#8b5cf6', '#ef4444', '#06b6d4'],
        xaxis: {
            categories: ['Novos', 'Contato', 'Qualificados', 'Proposta', 'Negociação', 'Ganhos'],
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        },
        legend: {
            show: false
        }
    };

    charts.funnel = new ApexCharts(element, options);
    charts.funnel.render();
}

function initializeSourceChart() {
    const element = document.getElementById('sourceChart');
    if (!element) return;

    const options = {
        series: [35, 25, 20, 15, 5],
        chart: {
            type: 'donut',
            height: 350,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        labels: ['Website', 'Indicação', 'Redes Sociais', 'Eventos', 'Cold Call'],
        colors: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ef4444'],
        dataLabels: {
            enabled: true,
            style: {
                colors: [currentTheme === 'dark' ? '#f1f5f9' : '#1e293b']
            }
        },
        legend: {
            position: 'bottom',
            labels: {
                colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
            }
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '65%'
                }
            }
        },
        theme: {
            mode: currentTheme
        }
    };

    charts.source = new ApexCharts(element, options);
    charts.source.render();
}

function initializeSalesChart() {
    const element = document.getElementById('salesChart');
    if (!element) return;

    const options = {
        series: [{
            name: 'Vendas Realizadas',
            data: [12, 19, 15, 25, 22, 30]
        }],
        chart: {
            type: 'line',
            height: 350,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        colors: ['#00ff88'],
        stroke: {
            curve: 'smooth',
            width: 5
        },
        markers: {
            size: 6,
            colors: ['#00ff88'],
            strokeColors: '#ffffff',
            strokeWidth: 2,
            hover: {
                size: 8
            }
        },
        fill: {
            type: 'gradient',
            gradient: {
                shadeIntensity: 1,
                opacityFrom: 0.4,
                opacityTo: 0.1,
                stops: [0, 90, 100]
            }
        },
        xaxis: {
            categories: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        }
    };

    charts.sales = new ApexCharts(element, options);
    charts.sales.render();
}

function initializeConversionChart() {
    const element = document.getElementById('conversionChart');
    if (!element) return;

    const options = {
        series: [{
            name: 'Taxa de Conversão (%)',
            data: [85, 78, 92, 68]
        }],
        chart: {
            type: 'bar',
            height: 350,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        colors: ['#3b82f6'],
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '60%',
                borderRadius: 4
            }
        },
        dataLabels: {
            enabled: false
        },
        xaxis: {
            categories: ['Maria Santos', 'Carlos Oliveira', 'Ana Silva', 'Pedro Costa'],
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            max: 100,
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                },
                formatter: function (value) {
                    return value + '%';
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        },
        legend: {
            show: false
        }
    };

    charts.conversion = new ApexCharts(element, options);
    charts.conversion.render();
}

function initializeActivityChart() {
    const element = document.getElementById('activityChart');
    if (!element) return;

    const options = {
        series: [{
            name: 'Ligações',
            data: [45, 52, 38, 67, 59, 73]
        }, {
            name: 'Reuniões',
            data: [28, 35, 42, 31, 45, 38]
        }, {
            name: 'Emails',
            data: [85, 93, 78, 102, 89, 95]
        }],
        chart: {
            type: 'line',
            height: 350,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        colors: ['#00d4ff', '#00ff88', '#ffaa00'],
        stroke: {
            curve: 'smooth',
            width: 5
        },
        markers: {
            size: 6,
            colors: ['#00d4ff', '#00ff88', '#ffaa00'],
            strokeColors: '#ffffff',
            strokeWidth: 2,
            hover: {
                size: 8
            }
        },
        fill: {
            type: 'gradient',
            gradient: {
                shadeIntensity: 1,
                opacityFrom: 0.4,
                opacityTo: 0.1,
                stops: [0, 90, 100]
            }
        },
        xaxis: {
            categories: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        },
        legend: {
            labels: {
                colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
            }
        }
    };

    charts.activity = new ApexCharts(element, options);
    charts.activity.render();
}

function initializePipelineTimeChart() {
    const element = document.getElementById('pipelineTimeChart');
    if (!element) return;

    const options = {
        series: [{
            name: 'Tempo Médio (dias)',
            data: [3, 7, 5, 12, 8]
        }],
        chart: {
            type: 'bar',
            height: 350,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        colors: ['#6366f1', '#f59e0b', '#10b981', '#8b5cf6', '#ef4444'],
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '60%',
                borderRadius: 4,
                distributed: true
            }
        },
        dataLabels: {
            enabled: false
        },
        xaxis: {
            categories: ['Novo → Contato', 'Contato → Qualificado', 'Qualificado → Proposta', 'Proposta → Negociação', 'Negociação → Ganho'],
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                },
                rotate: -45
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                },
                formatter: function (value) {
                    return value + ' dias';
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        },
        legend: {
            show: false
        }
    };

    charts.pipelineTime = new ApexCharts(element, options);
    charts.pipelineTime.render();
}

function updateCharts() {
    Object.values(charts).forEach(chart => {
        if (chart && chart.updateOptions) {
            // Para ApexCharts, atualizamos as opções de tema
            chart.updateOptions({
                theme: {
                    mode: currentTheme
                },
                xaxis: {
                    labels: {
                        style: {
                            colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                        }
                    }
                },
                yaxis: {
                    labels: {
                        style: {
                            colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                        }
                    }
                },
                grid: {
                    borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
                },
                legend: {
                    labels: {
                        colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                    }
                },
                dataLabels: {
                    style: {
                        colors: [currentTheme === 'dark' ? '#f1f5f9' : '#1e293b']
                    }
                }
            });
        }
    });

    // Reinicializar relatórios clássicos se estiverem visíveis
    const reportsTab = document.getElementById('reports');
    if (reportsTab && reportsTab.classList.contains('active')) {
        setTimeout(() => {
            initializeClassicReports();
        }, 100);
    }
}

// Logs Management
let currentLogsPage = 1;
const logsPerPage = 5;
let logsFilters = {
    startDate: '',
    endDate: '',
    type: ''
};

function renderLogsTimeline() {
    const logsTimeline = document.getElementById('logsTimeline');
    if (!logsTimeline) return;

    // Apply filters
    let filteredLogs = logs.filter(log => {
        const logDate = new Date(log.timestamp).toISOString().split('T')[0];

        // Filter by start date
        if (logsFilters.startDate && logDate < logsFilters.startDate) {
            return false;
        }

        // Filter by end date
        if (logsFilters.endDate && logDate > logsFilters.endDate) {
            return false;
        }

        // Filter by type
        if (logsFilters.type && log.type !== logsFilters.type) {
            return false;
        }

        return true;
    });

    const sortedLogs = filteredLogs.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp));
    const totalPages = Math.ceil(sortedLogs.length / logsPerPage);
    const startIndex = (currentLogsPage - 1) * logsPerPage;
    const endIndex = startIndex + logsPerPage;
    const paginatedLogs = sortedLogs.slice(startIndex, endIndex);

    logsTimeline.innerHTML = `
        <div class="logs-content">
            ${paginatedLogs.map(log => `
                <div class="log-item" style="cursor: pointer;" onclick="openLogDetails(${log.id})">
                    <div class="log-icon">
                        <i class="fas fa-${getLogIcon(log.type)}"></i>
                    </div>
                    <div class="log-content">
                        <div class="log-title">${log.title}</div>
                        <div class="log-description">${log.description}</div>
                        <div class="log-time">${formatDateTime(log.timestamp)} - ${log.userId || log.userId}</div>
                    </div>
                    <div class="log-actions">
                        <i class="fas fa-chevron-right" style="color: var(--text-muted); font-size: 12px;"></i>
                    </div>
                </div>
            `).join('')}
        </div>
        <div class="logs-pagination">
            <button class="btn btn-sm btn-secondary" ${currentLogsPage === 1 ? 'disabled' : ''} onclick="changeLogsPage(${currentLogsPage - 1})">
                <i class="fas fa-chevron-left"></i> Anterior
            </button>
            <span class="pagination-info">Página ${currentLogsPage} de ${totalPages}</span>
            <button class="btn btn-sm btn-secondary" ${currentLogsPage === totalPages ? 'disabled' : ''} onclick="changeLogsPage(${currentLogsPage + 1})">
                Próxima <i class="fas fa-chevron-right"></i>
            </button>
        </div>
    `;
}

function changeLogsPage(page) {
    // Apply current filters to get filtered logs count
    let filteredLogs = logs.filter(log => {
        const logDate = new Date(log.timestamp).toISOString().split('T')[0];

        if (logsFilters.startDate && logDate < logsFilters.startDate) return false;
        if (logsFilters.endDate && logDate > logsFilters.endDate) return false;
        if (logsFilters.type && log.type !== logsFilters.type) return false;

        return true;
    });

    const totalPages = Math.ceil(filteredLogs.length / logsPerPage);
    if (page < 1 || page > totalPages) return;

    currentLogsPage = page;
    renderLogsTimeline();
}

function applyLogsFilters() {
    const startDateInput = document.querySelector('.logs-filters .date-input:nth-child(1)');
    const endDateInput = document.querySelector('.logs-filters .date-input:nth-child(2)');
    const typeSelect = document.querySelector('.logs-filters .filter-select');

    logsFilters.startDate = startDateInput ? startDateInput.value : '';
    logsFilters.endDate = endDateInput ? endDateInput.value : '';
    logsFilters.type = typeSelect ? typeSelect.value : '';

    // Reset to first page when applying filters
    currentLogsPage = 1;
    renderLogsTimeline();

    showNotification('Filtros aplicados com sucesso!', 'success');
}

async function addLog(logEntry) {
    try {
        // Ensure the log entry has the correct field names for the database
        const dbLogEntry = {
            type: logEntry.type,
            title: logEntry.title,
            description: logEntry.description,
            userId: logEntry.userId || logEntry.userId,
            leadId: logEntry.leadId || logEntry.leadId
        };

        await fetchFromAPI('/logs', {
            method: 'POST',
            body: JSON.stringify(dbLogEntry)
        });

        // Recarregar logs
        logs = await fetchFromAPI('/logs');
        renderLogsTimeline();
    } catch (error) {
        console.error('Erro ao adicionar log:', error);
        showNotification('Erro ao salvar log', 'error');
    }
}

// Modal Management
//function openLeadModal() {
//    // Resetar o formulário para um novo lead
//    const form = document.getElementById('leadForm');
//    if (form) form.reset();

//    const leadIdInput = document.getElementById('leadId');
//    if (leadIdInput) leadIdInput.value = '';

//    const modalTitle = document.getElementById('leadModalTitle');
//    if (modalTitle) modalTitle.textContent = 'Novo Lead';

//    // Abrir o modal usando a API do Bootstrap
//    const modalElement = document.getElementById('leadModal');
//    if (modalElement) {
//        const bootstrapModal = new bootstrap.Modal(modalElement);
//        bootstrapModal.show();
//    }
//}


// Função para o botão "+ Novo Lead"
function openLeadModal() {
    const form = document.getElementById('leadForm');
    form.reset(); // Limpa qualquer dado de edição anterior

    document.getElementById('leadId').value = ''; // Garante que não há ID
    document.getElementById('leadModalTitle').textContent = 'Novo Lead';

    const modalElement = document.getElementById('leadModal');
    if (modalElement) {
        const bootstrapModal = bootstrap.Modal.getOrCreateInstance(modalElement);
        bootstrapModal.show();
    }
}



// Não é mais necessária
//function closeModal(modalId) {
//    document.getElementById(modalId).style.display = 'none';
//}

function openLeadDetails(leadId) {
    // Implementation for lead details modal/page
    showNotification(`Abrindo detalhes do lead ID: ${leadId}`, 'info');
}

function openEventModal() {
    const form = document.getElementById('activityForm');
    form.reset();
    document.getElementById('activityLeadId').value = '';

    const now = new Date();
    now.setMinutes(Math.ceil(now.getMinutes() / 15) * 15);
    document.getElementById('activityDateTime').value = now.toISOString().slice(0, 16);

    // Populate leads dropdown - sempre recarregar para ter dados atualizados
    const leadSelect = document.getElementById('activityLeadId');
    leadSelect.innerHTML = '<option value="">Selecione um lead (opcional)</option>';

    // Verificar se existem leads carregados
    if (leads && leads.length > 0) {
        leads.forEach(lead => {
            const option = document.createElement('option');
            option.value = lead.id;
            option.textContent = `${lead.name} - ${lead.company}`;
            leadSelect.appendChild(option);
        });
    } else {
        // Se não há leads, carregar do servidor
        fetchFromAPI('/leads').then(serverLeads => {
            if (serverLeads && serverLeads.length > 0) {
                leads = serverLeads; // Atualizar array global
                serverLeads.forEach(lead => {
                    const option = document.createElement('option');
                    option.value = lead.id;
                    option.textContent = `${lead.name} - ${lead.company}`;
                    leadSelect.appendChild(option);
                });
            }
        }).catch(error => {
            console.error('Erro ao carregar leads:', error);
        });
    }

    // ✨ RESETAR TEXTOS DO MODAL PARA MODO CRIAÇÃO
    const modalTitle = document.querySelector('#activityModal .modal-header h2');
    const submitButton = document.querySelector('#activityModal .modal-footer .btn-primary');

    if (modalTitle) {
        modalTitle.textContent = 'Agendar Atividade';
    }

    if (submitButton) {
        submitButton.innerHTML = '<i class="fas fa-plus"></i> Agendar';
    }

    // Abertura com Bootstrap
    const modalElement = document.getElementById('activityModal');
    if (modalElement) {
        const bootstrapModal = bootstrap.Modal.getOrCreateInstance(modalElement);
        bootstrapModal.show();
    }
}

function openTaskModal() {
    const form = document.getElementById('taskForm');
    form.reset();
    document.getElementById('taskId').value = '';
    document.getElementById('taskModalTitle').textContent = 'Nova Tarefa';

    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    document.getElementById('taskDueDate').value = tomorrow.toISOString().split('T')[0];

    // Populate leads dropdown
    const leadSelect = document.getElementById('taskLeadId');
    leadSelect.innerHTML = '<option value="">Selecione um lead</option>';
    leads.forEach(lead => {
        const option = document.createElement('option');
        option.value = lead.id;
        option.textContent = `${lead.name} - ${lead.company}`;
        leadSelect.appendChild(option);
    });

    // Set default assignee
    document.getElementById('taskAssignee').value = 'Maria';

    // Abertura com Bootstrap
    const modalElement = document.getElementById('taskModal');
    if (modalElement) {
        const bootstrapModal = bootstrap.Modal.getOrCreateInstance(modalElement);
        bootstrapModal.show();
    }
}

async function submitLead() {
    const form = document.getElementById('leadForm');
    const formData = new FormData(form);
    const leadId = formData.get('id'); // leadId é uma string Guid, o que é perfeito.

    const leadData = {
        name: formData.get('name'),
        company: formData.get('company'),
        email: formData.get('email'),
        phone: formData.get('phone'),
        position: formData.get('position'),
        source: formData.get('source'),
        status: formData.get('status') || 'novo',
        responsible: 'Usuário Atual',
        score: 50,
        temperature: 'morno',
        value: parseFloat(formData.get('value')) || 0,
        notes: formData.get('notes'),
        lastContact: new Date().toISOString().split('T')[0]
    };

    try {
        if (leadId) {
            // Edit existing lead
            const updatedLead = await fetchFromAPI(`/leads/${leadId}`, {
                method: 'PUT',
                // ✅ CORREÇÃO 1: Envie o ID como a string que ele já é.
                body: JSON.stringify({ ...leadData, id: leadId })
            });

            // Update local array
            // ✅ CORREÇÃO 2: Compare a string do ID diretamente.
            const leadIndex = leads.findIndex(l => l.id === leadId);
            if (leadIndex !== -1) {
                // Apenas atualizamos o lead com a resposta do servidor, que é mais segura
                leads[leadIndex] = updatedLead;
            }

            await addLog({
                type: 'lead',
                title: 'Lead atualizado',
                description: `Lead ${leadData.name} foi editado`,
                userId: 'Usuário Atual',
                // ✅ CORREÇÃO 3: Passe a string do ID.
                leadId: leadId
            });

            showNotification('Lead atualizado com sucesso!', 'success');
        } else {
            // Create new lead
            const newLead = await fetchFromAPI('/leads', {
                method: 'POST',
                body: JSON.stringify(leadData)
            });

            // Add to local array
            leads.push(newLead);

            await addLog({
                type: 'lead',
                title: 'Novo lead criado',
                description: `Lead ${leadData.name} foi adicionado ao sistema`,
                userId: 'Usuário Atual',
                leadId: newLead.id
            });

            showNotification('Lead criado com sucesso!', 'success');
        }

        // Re-render components
        renderLeadsTable();
        renderKanbanBoard();


        const modalElement = document.getElementById('leadModal');
        const bootstrapModal = bootstrap.Modal.getInstance(modalElement);
        if (bootstrapModal) {
            bootstrapModal.hide();
        }


        form.reset();
        document.getElementById('leadModalTitle').textContent = 'Novo Lead';

    } catch (error) {
        console.error('Erro ao salvar lead:', error);
        showNotification('Erro ao salvar lead', 'error');
    }
}

// Utility Functions
function getStatusLabel(status) {
    const labels = {
        novo: 'Novo',
        contato: 'Primeiro Contato',
        qualificado: 'Qualificado',
        proposta: 'Proposta',
        negociacao: 'Negociação',
        ganho: 'Ganho',
        perdido: 'Perdido'
    };
    // Adicione .toLowerCase() para garantir que a busca funcione
    return labels[status.toLowerCase()] || status;
}

function getLogIcon(type) {
    const icons = {
        lead: 'user-plus',
        email: 'envelope',
        call: 'phone',
        task: 'tasks',
        meeting: 'calendar',
        note: 'sticky-note'
    };
    return icons[type] || 'info-circle';
}

function formatDate(dateString) {
    if (!dateString || dateString === 'null' || dateString === 'undefined') {
        return 'Não informado';
    }

    try {
        const date = new Date(dateString);

        // Check if date is valid
        if (isNaN(date.getTime())) {
            return 'Data inválida';
        }

        return date.toLocaleDateString('pt-BR');
    } catch (error) {
        console.error('Erro ao formatar data:', error, 'Data recebida:', dateString);
        return 'Data inválida';
    }
}

function formatDateTime(dateString) {
    if (!dateString || dateString === 'null' || dateString === 'undefined') {
        return 'Não informado';
    }

    try {
        const date = new Date(dateString);

        // Check if date is valid
        if (isNaN(date.getTime())) {
            return 'Data inválida';
        }

        return date.toLocaleString('pt-BR');
    } catch (error) {
        console.error('Erro ao formatar data/hora:', error, 'Data recebida:', dateString);
        return 'Data inválida';
    }
}

function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
}

// Function to normalize text (remove accents and convert to lowercase)
function normalizeText(text) {
    return text.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
}

// FUNÇÃO DE FILTRO DE LEADS CORRIGIDA
function filterLeads() {
    const searchTerm = document.getElementById('searchInput')?.value.toLowerCase() || '';
    const selectedStatus = document.getElementById('statusFilter')?.value || '';
    const selectedResponsible = document.getElementById('responsibleFilter')?.value || '';

    let filteredLeads = leads.filter(lead => {
        const nameMatch = lead.name.toLowerCase().includes(searchTerm);
        const companyMatch = lead.company?.toLowerCase().includes(searchTerm) || false;

        const statusMatch = !selectedStatus || lead.status.toLowerCase() === selectedStatus;
        const responsibleMatch = !selectedResponsible || lead.responsible === selectedResponsible;

        return (nameMatch || companyMatch) && statusMatch && responsibleMatch;
    });

    renderFilteredLeadsTable(filteredLeads);
}

// FUNÇÃO DE RENDERIZAÇÃO DA TABELA DE LEADS CORRIGIDA
function renderLeadsTable() {
    renderFilteredLeadsTable(leads); // Renderiza a lista completa inicialmente
}

function renderFilteredLeadsTable(filteredLeads) {
    const tbody = document.getElementById('leadsTableBody');
    if (!tbody) return;

    tbody.innerHTML = filteredLeads.map(lead => `
        <tr style="cursor: pointer;" onclick="openLeadDetails('${lead.id}')">
            <td><input type="checkbox" onclick="event.stopPropagation();"></td>
            <td>${lead.name}</td>
            <td>${lead.company}</td>
            <td>${lead.email}</td>
            <td>${lead.phone}</td>
            <td><span class="status-badge status-${lead.status}">${getStatusLabel(lead.status)}</span></td>
            <td>${lead.responsible}</td>
            <td>${formatDate(lead.lastContact)}</td>
            <td>
                <button class="btn btn-sm btn-primary" onclick="editLead('${lead.id}'); event.stopPropagation();" title="Editar lead">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteLead('${lead.id}'); event.stopPropagation();" title="Excluir lead">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Notification System
function showNotification(message, type = 'info', duration = 5000) {
    const container = document.getElementById('notifications');
    if (!container) return;

    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.innerHTML = `
        <div style="display: flex; justify-content: space-between; align-items: center;">
            <span>${message}</span>
            <button onclick="this.parentElement.parentElement.remove()" style="background: none; border: none; color: inherit; cursor: pointer; font-size: 16px;">&times;</button>
        </div>
    `;

    container.appendChild(notification);

    setTimeout(() => {
        if (notification.parentElement) {
            notification.remove();
        }
    }, duration);
}

// Service Worker for Notifications
if ('serviceWorker' in navigator && 'PushManager' in window) {
    // Request notification permission
    Notification.requestPermission().then(permission => {
        if (permission === 'granted') {
            console.log('Notification permission granted');
        }
    });
}

// Simulated real-time updates
setInterval(() => {
    // Simulate receiving new notifications
    const notifications = [
        'Novo lead recebido via website',
        'Follow-up agendado foi completado',
        'Lead quente precisa de atenção',
        'Nova tarefa foi atribuída a você'
    ];

    if (Math.random() < 0.1) { // 10% chance every interval
        const randomNotification = notifications[Math.floor(Math.random() * notifications.length)];
        showNotification(randomNotification, 'info');
    }
}, 30000); // Check every 30 seconds

// ✅ VERSÃO CORRIGIDA
async function submitActivity() {
    const form = document.getElementById('activityForm');
    const formData = new FormData(form);
    const leadIdValue = formData.get('leadId');
    const eventId = formData.get('eventId');

    // ✅ CORREÇÃO: Definir a variável 'leadId' e buscar o objeto 'lead'
    const leadId = leadIdValue && leadIdValue !== '' ? leadIdValue : null;
    const lead = leadId ? leads.find(l => l.id === leadId) : null;

    const activityData = {
        id: eventId || '00000000-0000-0000-0000-000000000000',
        leadId: leadId,
        type: formData.get('type'),
        title: formData.get('title'),
        description: formData.get('description'),
        scheduledDate: formData.get('datetime')
    };

    try {
        if (eventId) {
            // Editar evento existente
            const updatedActivity = await fetchFromAPI(`/activities/${eventId}`, {
                method: 'PUT',
                body: JSON.stringify(activityData)
            });

            // Atualizar evento no calendário
            const calendarEvent = calendar.getEventById(eventId);
            if (calendarEvent) {
                calendarEvent.setProp('title', updatedActivity.title);
                calendarEvent.setStart(updatedActivity.scheduled_date);
                calendarEvent.setExtendedProp('leadId', updatedActivity.leadId);
                calendarEvent.setExtendedProp('type', updatedActivity.type);
                calendarEvent.setExtendedProp('description', updatedActivity.description);
                calendarEvent.setProp('backgroundColor', getEventColor(updatedActivity.type));
                calendarEvent.setProp('borderColor', getEventColor(updatedActivity.type));
            }

            // Log da edição
            await addLog({
                type: 'meeting',
                title: 'Atividade editada',
                description: `Evento "${activityData.title}" foi atualizado`,
                userId: 'Usuário Atual',
                leadId: leadId
            });

            showNotification('Atividade atualizada com sucesso!', 'success');
        } else {
            // Criar nova atividade
            const newActivity = await fetchFromAPI('/activities', {
                method: 'POST',
                body: JSON.stringify(activityData)
            });

            // Adicionar ao calendário
            if (calendar) {
                calendar.addEvent({
                    id: newActivity.id.toString(),
                    title: newActivity.title,
                    start: newActivity.scheduledDate, // ✅ CORREÇÃO
                    backgroundColor: getEventColor(newActivity.type),
                    borderColor: getEventColor(newActivity.type),
                    extendedProps: {
                        leadId: newActivity.leadId,
                        type: newActivity.type,
                        description: newActivity.description
                    }
                });
            }

            // Log da atividade
            await addLog({
                type: 'meeting',
                title: 'Atividade agendada',
                description: `${activityData.title}${lead ? ` para ${lead.name}` : ''} agendada`,
                userId: 'Usuário Atual',
                leadId: leadId
            });

            showNotification('Atividade agendada com sucesso!', 'success');
        }

        // ✅ LÓGICA DE FECHAMENTO CORRIGIDA
        const modalEl = document.getElementById('activityModal');
        if (modalEl) {
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            if (modalInstance) {
                modalInstance.hide();
            }
        }
        form.reset();

        // Remover input hidden do eventId se existir
        const eventIdInput = document.getElementById('activityEventId');
        if (eventIdInput) {
            eventIdInput.remove();
        }

        // ✨ RESETAR TEXTOS DO MODAL APÓS SALVAR
        const modalTitle = document.querySelector('#activityModal .modal-header h2');
        const submitButton = document.querySelector('#activityModal .modal-footer .btn-primary');

        if (modalTitle) {
            modalTitle.textContent = 'Agendar Atividade';
        }

        if (submitButton) {
            submitButton.innerHTML = '<i class="fas fa-plus"></i> Agendar';
        }

    } catch (error) {
        console.error('Erro ao salvar atividade:', error);
        showNotification('Erro ao salvar atividade', 'error');
    }
}

async function submitNote() {
    const form = document.getElementById('noteForm');
    const formData = new FormData(form);
    const leadId = formData.get('leadId'); // Guids são strings, não use parseInt
    const lead = leads.find(l => l.id === leadId);

    if (!lead) {
        showNotification('Lead não encontrado', 'error');
        return;
    }

    const note = formData.get('note');

    try {
        // Save note to database
        await fetchFromAPI('/notes', {
            method: 'POST',
            body: JSON.stringify({
                leadId: leadId,
                content: note,
                color: 'blue',
                userId: 'Usuário Atual'
            })
        });

        // Update local lead notes
        if (lead.notes) {
            lead.notes += '\n\n--- Nota ' + new Date().toLocaleString('pt-BR') + ' ---\n' + note;
        } else {
            lead.notes = note;
        }

        await addLog({
            type: 'note',
            title: 'Nota adicionada',
            description: `Nova nota adicionada para ${lead.name}`,
            userId: 'Usuário Atual',
            leadId: leadId
        });

        // ✅ LÓGICA DE FECHAMENTO CORRIGIDA
        const modalEl = document.getElementById('noteModal');
        if (modalEl) {
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            if (modalInstance) {
                modalInstance.hide();
            }
        }
        form.reset();
        renderLeadsTable();
        renderKanbanBoard();
        showNotification('Nota adicionada com sucesso!', 'success');

    } catch (error) {
        console.error('Erro ao salvar nota:', error);
        showNotification('Erro ao salvar nota', 'error');
    }
}

// ✅ VERSÃO CORRIGIDA E FINAL
function openLeadDetails(leadId) {
    // Garante que o evento de clique na linha <tr> não interfira
    // com outros cliques (como o do botão editar/deletar)
    if (event) {
        event.stopPropagation();
    }

    const lead = leads.find(l => l.id === leadId);
    if (!lead) {
        showNotification('Lead não encontrado', 'error');
        return;
    }

    // 1. Encontrar o formulário e preencher os campos
    const form = document.getElementById('leadForm');
    form.reset(); // Limpa o formulário antes de preencher

    document.getElementById('leadId').value = lead.id || '';
    document.getElementById('leadName').value = lead.name || '';
    document.getElementById('leadCompany').value = lead.company || '';
    document.getElementById('leadEmail').value = lead.email || '';
    document.getElementById('leadPhone').value = lead.phone || '';
    document.getElementById('leadPosition').value = lead.position || '';
    document.getElementById('leadSource').value = lead.source || 'website';
    document.getElementById('leadStatus').value = lead.status.toLowerCase(); // Garante que o valor corresponde ao <option>
    document.getElementById('leadValue').value = lead.value || 0;
    document.getElementById('leadNotes').value = lead.notes || '';
    document.getElementById('leadModalTitle').textContent = `Editar Lead - ${lead.name}`;

    // 2. Abrir o modal da forma correta com a API do Bootstrap
    const modalElement = document.getElementById('leadModal');
    if (modalElement) {
        const bootstrapModal = bootstrap.Modal.getOrCreateInstance(modalElement);
        bootstrapModal.show();
    }
}

function editLead(leadId) {
    openLeadDetails(leadId);
}

async function deleteLead(leadId) {
    const lead = leads.find(l => l.id === leadId);
    if (!lead) return;

    const result = await Swal.fire({
        title: 'Confirmar Exclusão',
        text: `Tem certeza que deseja excluir o lead "${lead.name}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sim, excluir!',
        cancelButtonText: 'Cancelar',
        background: currentTheme === 'dark' ? '#1e293b' : '#ffffff',
        color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
    });

    if (result.isConfirmed) {
        try {
            // Try to delete from server
            await fetchFromAPI(`/leads/${leadId}`, {
                method: 'DELETE'
            });

            // Remove lead from local array
            leads = leads.filter(l => l.id !== leadId);

            // Add log entry
            await addLog({
                type: 'lead',
                title: 'Lead excluído',
                description: `Lead ${lead.name} foi removido do sistema`,
                userId: 'Usuário Atual',
                leadId: leadId
            });

            // Re-render components
            renderLeadsTable();
            renderKanbanBoard();

            Swal.fire({
                title: 'Excluído!',
                text: 'O lead foi excluído com sucesso.',
                icon: 'success',
                confirmButtonColor: '#10b981',
                background: currentTheme === 'dark' ? '#1e293b' : '#ffffff',
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            });
        } catch (error) {
            console.error('Erro ao excluir lead:', error);
            showNotification('Erro ao excluir lead', 'error');
        }
    }
}

function scheduleActivity(leadId) {
    const lead = leads.find(l => l.id === leadId);
    if (!lead) {
        showNotification('Lead não encontrado', 'error');
        return;
    }

    // Reset form and set lead ID
    const form = document.getElementById('activityForm');
    form.reset();

    // Set default date to current time
    const now = new Date();
    now.setMinutes(Math.ceil(now.getMinutes() / 15) * 15);
    document.getElementById('activityDateTime').value = now.toISOString().slice(0, 16);

    // Populate leads dropdown
    const leadSelect = document.getElementById('activityLeadId');
    leadSelect.innerHTML = '<option value="">Selecione um lead (opcional)</option>';
    leads.forEach(leadOption => {
        const option = document.createElement('option');
        option.value = leadOption.id;
        option.textContent = `${leadOption.name} - ${leadOption.company}`;
        option.selected = leadOption.id === leadId;
        leadSelect.appendChild(option);
    });

    // Set lead ID in activity modal
    document.getElementById('activityLeadId').value = leadId;

    // Open activity modal
    document.getElementById('activityModal').style.display = 'block';

    showNotification(`Agendando atividade para ${lead.name}`, 'info');
}

function addNote(leadId) {
    const lead = leads.find(l => l.id === leadId);
    if (!lead) return;

    // Set lead ID in note modal
    document.getElementById('noteLeadId').value = leadId;

    // Open note modal
    document.getElementById('noteModal').style.display = 'block';

    showNotification(`Adicionando nota para ${lead.name}`, 'info');
}

// Função submitEvent removida - agora tudo usa submitActivity

function getEventColor(type) {
    const colors = {
        meeting: '#3b82f6',
        call: '#10b981',
        email: '#f59e0b',
        demo: '#8b5cf6',
        'follow-up': '#06b6d4',
        task: '#ef4444'
    };
    return colors[type] || '#3b82f6';
}

//async function submitTask() {
//    const form = document.getElementById('taskForm');
//    const formData = new FormData(form);
//    const taskId = formData.get('id');

//    const taskData = {
//        title: formData.get('title'),
//        description: formData.get('description'),
//        dueDate: formData.get('dueDate'),
//        priority: formData.get('priority'),
//        status: taskId ? undefined : 'pending', // Manter status atual se editando
//        leadId: parseInt(formData.get('leadId')) || null,
//        assignee: formData.get('assignee'),
//        progress: parseInt(formData.get('progress')) || 0
//    };

//    try {
//        if (taskId) {
//            // Edit existing task
//            const updatedTask = await fetchFromAPI(`/tasks/${taskId}`, {
//                method: 'PUT',
//                body: JSON.stringify(taskData)
//            });

//            // Update local array
//            const taskIndex = tasks.findIndex(t => t.id === parseInt(taskId));
//            if (taskIndex !== -1) {
//                tasks[taskIndex] = { ...tasks[taskIndex], ...updatedTask };
//            }

//            await addLog({
//                type: 'task',
//                title: 'Tarefa atualizada',
//                description: `Tarefa "${taskData.title}" foi editada`,
//                userId: 'Usuário Atual',
//                leadId: taskData.leadId
//            });

//            showNotification('Tarefa atualizada com sucesso!', 'success');
//        } else {
//            // Create new task
//            taskData.status = 'pending'; // Para novas tarefas
//            const newTask = await fetchFromAPI('/tasks', {
//                method: 'POST',
//                body: JSON.stringify(taskData)
//            });

//            // Add to local array
//            tasks.push(newTask);

//            await addLog({
//                type: 'task',
//                title: 'Nova tarefa criada',
//                description: `Tarefa "${taskData.title}" foi adicionada ao sistema`,
//                userId: 'Usuário Atual',
//                leadId: taskData.leadId
//            });

//            showNotification('Tarefa criada com sucesso!', 'success');
//        }

//        // Re-render tasks list
//        renderTasksList();


//        const modalElement = document.getElementById('taskModal');
//        const bootstrapModal = bootstrap.Modal.getInstance(modalElement);
//        if (bootstrapModal) {
//            bootstrapModal.hide();
//        }

//        form.reset();
//        document.getElementById('taskModalTitle').textContent = 'Nova Tarefa';

//    } catch (error) {
//        console.error('Erro ao salvar tarefa:', error);
//        showNotification('Erro ao salvar tarefa', 'error');
//    }
//}


// ✅ VERSÃO CORRIGIDA E COMPLETA
async function submitTask() {
    const form = document.getElementById('taskForm');
    const formData = new FormData(form);
    const taskId = formData.get('id'); // taskId é a string do Guid ou vazio.

    // 1. Captura de dados corrigida
    const leadIdValue = formData.get('leadId');

    const taskData = {
        title: formData.get('title'),
        description: formData.get('description'),
        dueDate: formData.get('dueDate') || null,
        priority: formData.get('priority'),
        // Corrigido: Não usamos parseInt em um Guid. Se não houver valor, será null.
        leadId: leadIdValue && leadIdValue !== '' ? leadIdValue : null,
        assignee: formData.get('assignee'),
        progress: parseInt(formData.get('progress')) || 0
    };

    try {
        if (taskId) { // Bloco de EDIÇÃO
            // Para edição, não enviamos o status, pois ele é controlado por outra função.
            // Mas precisamos enviar o ID da tarefa no payload para o model binder funcionar corretamente.
            const payload = { ...taskData, id: taskId };

            const updatedTask = await fetchFromAPI(`/tasks/${taskId}`, {
                method: 'PUT',
                body: JSON.stringify(payload)
            });

            if (updatedTask) { // Verifica se a API retornou um objeto válido
                // Atualiza o array local
                // Corrigido: Compara a string do Guid diretamente.
                const taskIndex = tasks.findIndex(t => t.id === taskId);
                if (taskIndex !== -1) {
                    tasks[taskIndex] = updatedTask; // Usa a resposta da API, que é mais confiável
                }
                showNotification('Tarefa atualizada com sucesso!', 'success');

                // Log da edição (opcional, considerando mover para o backend)
                await addLog({
                    type: 'task',
                    title: 'Tarefa atualizada',
                    description: `Tarefa "${taskData.title}" foi editada`,
                    userId: 'Usuário Atual',
                    leadId: taskData.leadId
                });
            }

        } else { // Bloco de CRIAÇÃO
            // Corrigido: Envia o nome do membro do enum em C#.
            taskData.status = 'Pendente';

            const newTask = await fetchFromAPI('/tasks', {
                method: 'POST',
                body: JSON.stringify(taskData)
            });

            if (newTask) { // Verifica se a API retornou um objeto válido
                tasks.push(newTask);
                showNotification('Tarefa criada com sucesso!', 'success');
                // Log da criação (opcional)
                await addLog({
                    type: 'task',
                    title: 'Nova tarefa criada',
                    description: `Tarefa "${taskData.title}" foi adicionada ao sistema`,
                    userId: 'Usuário Atual',
                    leadId: taskData.leadId
                });
            }
        }

        // Se chegou aqui (sem cair no catch), podemos fechar o modal e atualizar a UI
        renderTasksList(); // Re-renderiza a lista de tarefas

        const modalElement = document.getElementById('taskModal');
        const bootstrapModal = bootstrap.Modal.getInstance(modalElement);
        if (bootstrapModal) {
            bootstrapModal.hide();
        }

        form.reset();
        document.getElementById('taskModalTitle').textContent = 'Nova Tarefa';

    } catch (error) {
        console.error('Erro ao salvar tarefa:', error);
        showNotification('Erro ao salvar tarefa. Verifique o console para detalhes.', 'error');
    }
}


// Função para abrir modal de edição de evento
function openEditEventModal(event) {
    const props = event.extendedProps;

    // Reset form
    const form = document.getElementById('activityForm');
    form.reset();

    // Preencher dados do evento
    document.getElementById('activityLeadId').value = props.leadId || '';
    document.getElementById('activityType').value = props.type || 'meeting';
    document.getElementById('activityTitle').value = event.title;
    document.getElementById('activityDescription').value = props.description || '';

    // Converter data para formato datetime-local (corrigir timezone)
    const startDate = new Date(event.start);
    // Usar formato ISO sem conversão de timezone que estava causando problemas
    const year = startDate.getFullYear();
    const month = String(startDate.getMonth() + 1).padStart(2, '0');
    const day = String(startDate.getDate()).padStart(2, '0');
    const hours = String(startDate.getHours()).padStart(2, '0');
    const minutes = String(startDate.getMinutes()).padStart(2, '0');
    const dateTimeValue = `${year}-${month}-${day}T${hours}:${minutes}`;

    document.getElementById('activityDateTime').value = dateTimeValue;

    // Adicionar ID do evento como atributo hidden
    const eventIdInput = document.createElement('input');
    eventIdInput.type = 'hidden';
    eventIdInput.name = 'eventId';
    eventIdInput.value = event.id;
    eventIdInput.id = 'activityEventId';
    form.appendChild(eventIdInput);

    // Carregar leads se necessário
    const leadSelect = document.getElementById('activityLeadId');
    if (leadSelect.children.length <= 1) {
        leadSelect.innerHTML = '<option value="">Selecione um lead (opcional)</option>';
        if (leads && leads.length > 0) {
            leads.forEach(lead => {
                const option = document.createElement('option');
                option.value = lead.id;
                option.textContent = `${lead.name} - ${lead.company}`;
                option.selected = lead.id == props.leadId;
                leadSelect.appendChild(option);
            });
        }
    } else {
        // Apenas selecionar o lead correto
        leadSelect.value = props.leadId || '';
    }

    // ✨ MUDAR TEXTOS DO MODAL PARA MODO EDIÇÃO
    const modalTitle = document.querySelector('#activityModal .modal-header h2');
    const submitButton = document.querySelector('#activityModal .modal-footer .btn-primary');

    if (modalTitle) {
        modalTitle.textContent = 'Editar Atividade';
    }

    if (submitButton) {
        submitButton.innerHTML = '<i class="fas fa-save"></i> Salvar Alterações';
    }

    // Abrir modal
    document.getElementById('activityModal').style.display = 'block';
}

// ✅ FUNÇÃO CORRIGIDA PARA EDITAR UM EVENTO DO CALENDÁRIO
function openEditEventModal(event) {
    if (!event) return;

    const props = event.extendedProps;
    const form = document.getElementById('activityForm');
    form.reset();

    // 1. Preenche o formulário com os dados do evento existente
    document.querySelector('#activityModal .modal-title').textContent = `Editar: ${event.title}`;
    document.querySelector('#activityModal .btn-primary').innerHTML = '<i class="fas fa-save"></i> Salvar Alterações';

    document.getElementById('activityTitle').value = event.title;
    document.getElementById('activityDescription').value = props.description || '';
    document.getElementById('activityType').value = props.type || 'call';

    // Adiciona um input hidden com o ID do evento para sabermos que é uma edição
    // Primeiro, remove qualquer um que possa ter ficado de uma chamada anterior
    const oldEventIdInput = document.getElementById('activityEventId');
    if (oldEventIdInput) {
        oldEventIdInput.remove();
    }
    const eventIdInput = document.createElement('input');
    eventIdInput.type = 'hidden';
    eventIdInput.name = 'eventId';
    eventIdInput.id = 'activityEventId';
    eventIdInput.value = event.id;
    form.appendChild(eventIdInput);

    // Formata e preenche a data e hora corretamente para o input datetime-local
    if (event.start) {
        const startDate = new Date(event.start);
        // Corrige o fuso horário para exibição local no input
        const timezoneOffset = startDate.getTimezoneOffset() * 60000;
        const localISOTime = new Date(startDate.getTime() - timezoneOffset).toISOString().slice(0, 16);
        document.getElementById('activityDateTime').value = localISOTime;
    }

    // Popula e seleciona o lead no dropdown
    const leadSelect = document.getElementById('activityLeadId');
    leadSelect.innerHTML = '<option value="">Nenhum lead relacionado</option>';
    if (leads && leads.length > 0) {
        leads.forEach(lead => {
            const option = document.createElement('option');
            option.value = lead.id;
            option.textContent = `${lead.name} - ${lead.company}`;
            if (props.leadId && lead.id === props.leadId) {
                option.selected = true;
            }
            leadSelect.appendChild(option);
        });
    }

    // 2. Abre o modal #activityModal da forma correta com a API do Bootstrap
    const modalElement = document.getElementById('activityModal');
    if (modalElement) {
        const bootstrapModal = bootstrap.Modal.getOrCreateInstance(modalElement);
        bootstrapModal.show();
    }
}

// Função para carregar ações importantes dinamicamente
async function loadImportantActions() {
    const actionsList = document.querySelector('.action-list');
    if (!actionsList) return;

    try {
        const actions = await generateImportantActions();

        if (actions.length === 0) {
            actionsList.innerHTML = `
                <div class="no-actions">
                    <div class="no-actions-content">
                        <i class="fas fa-check-circle"></i>
                        <h4>Tudo em dia!</h4>
                        <p>Não há ações críticas pendentes no momento.</p>
                    </div>
                </div>
            `;
            return;
        }

        actionsList.innerHTML = actions.map(action => `
            <div class="action-item ${action.urgency}" onclick="${action.onClick}">
                <i class="fas fa-${action.icon}"></i>
                <div class="action-content">
                    <h4>${action.title}</h4>
                    <p>${action.description}</p>
                </div>
            </div>
        `).join('');

    } catch (error) {
        console.error('Erro ao carregar ações importantes:', error);
        actionsList.innerHTML = `
            <div class="action-item">
                <i class="fas fa-exclamation-triangle"></i>
                <div class="action-content">
                    <h4>Erro ao carregar ações</h4>
                    <p>Não foi possível carregar as ações importantes.</p>
                </div>
            </div>
        `;
    }
}

// Função para gerar ações importantes baseadas nos dados reais
async function generateImportantActions() {
    const actions = [];
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);

    // 1. Tarefas atrasadas (URGENTE)
    const overdueTasks = tasks.filter(task => {
        if (task.status === 'completed') return false;
        const dueDate = new Date(task.dueDate || task.dueDate);
        return dueDate < today;
    });

    if (overdueTasks.length > 0) {
        actions.push({
            title: `${overdueTasks.length} tarefa${overdueTasks.length > 1 ? 's' : ''} atrasada${overdueTasks.length > 1 ? 's' : ''}`,
            description: `Vencimento: ${overdueTasks[0].title}${overdueTasks.length > 1 ? ` e mais ${overdueTasks.length - 1}` : ''}`,
            icon: 'exclamation-triangle',
            urgency: 'urgent',
            onClick: 'showTab("tasks"); filterTasks("overdue");'
        });
    }

    // 2. Tarefas que vencem hoje (MÉDIO)
    const todayTasks = tasks.filter(task => {
        if (task.status === 'completed') return false;
        const dueDate = new Date(task.dueDate || task.dueDate);
        return dueDate.toDateString() === today.toDateString();
    });

    if (todayTasks.length > 0) {
        actions.push({
            title: `${todayTasks.length} tarefa${todayTasks.length > 1 ? 's' : ''} vence${todayTasks.length > 1 ? 'm' : ''} hoje`,
            description: `${todayTasks[0].title}${todayTasks.length > 1 ? ` e mais ${todayTasks.length - 1}` : ''}`,
            icon: 'clock',
            urgency: 'medium',
            onClick: 'showTab("tasks");'
        });
    }

    // 3. Leads quentes sem contato recente (URGENTE)
    const hotLeadsNoContact = leads.filter(lead => {
        if (lead.temperature !== 'quente') return false;
        if (!lead.lastContact && !lead.lastContact) return true;

        const lastContact = new Date(lead.lastContact || lead.lastContact);
        const daysSinceContact = Math.floor((today - lastContact) / (1000 * 60 * 60 * 24));
        return daysSinceContact > 3; // Mais de 3 dias sem contato
    });

    if (hotLeadsNoContact.length > 0) {
        actions.push({
            title: `${hotLeadsNoContact.length} lead${hotLeadsNoContact.length > 1 ? 's' : ''} quente${hotLeadsNoContact.length > 1 ? 's' : ''} sem contato`,
            description: `${hotLeadsNoContact[0].name}${hotLeadsNoContact.length > 1 ? ` e mais ${hotLeadsNoContact.length - 1}` : ''} precisam de atenção`,
            icon: 'fire',
            urgency: 'urgent',
            onClick: 'showTab("kanban");'
        });
    }

    // 4. Atividades do dia na agenda (NORMAL)
    const todayActivities = await getTodayActivities();
    if (todayActivities.length > 0) {
        actions.push({
            title: `${todayActivities.length} atividade${todayActivities.length > 1 ? 's' : ''} agendada${todayActivities.length > 1 ? 's' : ''} hoje`,
            description: `Próxima: ${todayActivities[0].title}`,
            icon: 'calendar-check',
            urgency: '',
            onClick: 'showTab("calendar");'
        });
    }

    // 5. Leads novos não processados (MÉDIO)
    const newLeads = leads.filter(lead => lead.status === 'novo');
    if (newLeads.length > 0) {
        actions.push({
            title: `${newLeads.length} lead${newLeads.length > 1 ? 's' : ''} novo${newLeads.length > 1 ? 's' : ''} para processar`,
            description: `${newLeads[0].name}${newLeads.length > 1 ? ` e mais ${newLeads.length - 1}` : ''} aguardando primeiro contato`,
            icon: 'user-plus',
            urgency: 'medium',
            onClick: 'showTab("kanban");'
        });
    }

    // 6. Propostas pendentes há muito tempo (URGENTE)
    const oldProposals = leads.filter(lead => {
        if (lead.status !== 'proposta') return false;
        if (!lead.lastContact && !lead.lastContact) return true;

        const lastContact = new Date(lead.lastContact || lead.lastContact);
        const daysSinceContact = Math.floor((today - lastContact) / (1000 * 60 * 60 * 24));
        return daysSinceContact > 7; // Mais de 7 dias sem movimento
    });

    if (oldProposals.length > 0) {
        actions.push({
            title: `${oldProposals.length} proposta${oldProposals.length > 1 ? 's' : ''} sem follow-up`,
            description: `${oldProposals[0].name}${oldProposals.length > 1 ? ` e mais ${oldProposals.length - 1}` : ''} há mais de 7 dias`,
            icon: 'file-contract',
            urgency: 'urgent',
            onClick: 'showTab("kanban");'
        });
    }

    // Se não há ações críticas, adicionar ações de exemplo/sugestões
    if (actions.length === 0) {
        actions.push(
            {
                title: 'Revisar pipeline de vendas',
                description: 'Analisar leads em negociação e identificar oportunidades',
                icon: 'chart-line',
                urgency: '',
                onClick: 'showTab("kanban");'
            },
            {
                title: 'Follow-up semanal',
                description: 'Entrar em contato com leads qualificados da semana passada',
                icon: 'phone',
                urgency: '',
                onClick: 'showTab("leads-list");'
            },
            {
                title: 'Agendar apresentações',
                description: 'Marcar demonstrações para leads interessados',
                icon: 'presentation',
                urgency: '',
                onClick: 'showTab("calendar");'
            },
            {
                title: 'Atualizar propostas',
                description: 'Revisar e personalizar propostas comerciais pendentes',
                icon: 'edit',
                urgency: '',
                onClick: 'showTab("tasks");'
            }
        );
    }

    return actions.slice(0, 6); // Máximo 6 ações
}

// Função auxiliar para buscar atividades do dia
async function getTodayActivities() {
    try {
        const activities = await fetchFromAPI('/activities');
        const today = new Date().toISOString().split('T')[0];

        return activities.filter(activity => {
            const activityDate = new Date(activity.scheduledDate).toISOString().split('T')[0]; // <--- MUDANÇA
            return activityDate === today;
        });
    } catch (error) {
        console.error('Erro ao buscar atividades do dia:', error);
        return [];
    }
}

// Relatórios Customizáveis - Sistema Completo
let reportData = {};
let reportFilters = {};
let currentReportType = 'sales_performance';

// Função para gerar relatórios customizáveis
async function generateCustomReport() {
    const reportType = document.getElementById('reportType').value;
    const startDate = document.getElementById('reportStartDate').value;
    const endDate = document.getElementById('reportEndDate').value;
    const vendedor = document.getElementById('reportVendedor').value;

    try {
        showNotification('Gerando relatório...', 'info');

        reportFilters = {
            type: reportType,
            startDate,
            endDate,
            vendedor
        };

        currentReportType = reportType;

        switch (reportType) {
            case 'sales_performance':
                await generateSalesPerformanceReport();
                break;
            case 'pipeline_time':
                await generatePipelineTimeReport();
                break;
            case 'vendor_analysis':
                await generateVendorAnalysisReport();
                break;
            case 'conversion_funnel':
                await generateConversionFunnelReport();
                break;
            case 'lead_source':
                await generateLeadSourceReport();
                break;
            case 'activity_summary':
                await generateActivitySummaryReport();
                break;
            default:
                await generateSalesPerformanceReport();
        }

        // Mostrar seção de resultados
        document.getElementById('reportResults').style.display = 'block';
        document.getElementById('exportActions').style.display = 'block';

        showNotification('Relatório gerado com sucesso!', 'success');

    } catch (error) {
        console.error('Erro ao gerar relatório:', error);
        showNotification('Erro ao gerar relatório', 'error');
    }
}

// Relatório de Performance de Vendas
async function generateSalesPerformanceReport() {
    const filteredLeads = filterLeadsByDateAndVendor();
    const filteredTasks = filterTasksByDateAndVendor();

    const salesData = analyzeSalesPerformance(filteredLeads, filteredTasks);

    reportData = {
        type: 'sales_performance',
        title: 'Relatório de Performance de Vendas',
        data: salesData,
        charts: ['salesByVendor', 'conversionRates', 'revenueTimeline']
    };

    renderReportResults();
    updateReportCharts();
}

// Relatório de Tempo no Pipeline
async function generatePipelineTimeReport() {
    const filteredLeads = filterLeadsByDateAndVendor();

    const pipelineData = analyzePipelineTime(filteredLeads);

    reportData = {
        type: 'pipeline_time',
        title: 'Análise de Tempo no Pipeline',
        data: pipelineData,
        charts: ['pipelineTimeByStage', 'averageTimeComparison']
    };

    renderReportResults();
    updateReportCharts();
}

// Relatório de Análise por Vendedor
async function generateVendorAnalysisReport() {
    const filteredLeads = filterLeadsByDateAndVendor();
    const filteredTasks = filterTasksByDateAndVendor();
    const filteredLogs = filterLogsByDateAndVendor();

    const vendorData = analyzeVendorPerformance(filteredLeads, filteredTasks, filteredLogs);

    reportData = {
        type: 'vendor_analysis',
        title: 'Análise Detalhada por Vendedor',
        data: vendorData,
        charts: ['vendorComparison', 'activityByVendor', 'vendorConversion']
    };

    renderReportResults();
    updateReportCharts();
}

// Relatório de Funil de Conversão
async function generateConversionFunnelReport() {
    const filteredLeads = filterLeadsByDateAndVendor();

    const funnelData = analyzeConversionFunnel(filteredLeads);

    reportData = {
        type: 'conversion_funnel',
        title: 'Análise do Funil de Conversão',
        data: funnelData,
        charts: ['conversionFunnel', 'dropoffAnalysis']
    };

    renderReportResults();
    updateReportCharts();
}

// Relatório de Origem de Leads
async function generateLeadSourceReport() {
    const filteredLeads = filterLeadsByDateAndVendor();

    const sourceData = analyzeLeadSources(filteredLeads);

    reportData = {
        type: 'lead_source',
        title: 'Análise de Origem de Leads',
        data: sourceData,
        charts: ['sourceDistribution', 'sourceConversion', 'sourceRevenue']
    };

    renderReportResults();
    updateReportCharts();
}

// Relatório de Resumo de Atividades
async function generateActivitySummaryReport() {
    const filteredTasks = filterTasksByDateAndVendor();
    const filteredLogs = filterLogsByDateAndVendor();

    const activityData = analyzeActivitySummary(filteredTasks, filteredLogs);

    reportData = {
        type: 'activity_summary',
        title: 'Resumo de Atividades',
        data: activityData,
        charts: ['activityVolume', 'taskCompletion', 'activityTypes']
    };

    renderReportResults();
    updateReportCharts();
}

// Funções de análise de dados
function analyzeSalesPerformance(leads, tasks) {
    const vendors = [...new Set(leads.map(l => l.responsible))];
    const analysis = {};

    vendors.forEach(vendor => {
        const vendorLeads = leads.filter(l => l.responsible === vendor);
        const vendorTasks = tasks.filter(t => t.assignee === vendor);

        const totalLeads = vendorLeads.length;
        const convertedLeads = vendorLeads.filter(l => l.status === 'ganho').length;
        const totalRevenue = vendorLeads.filter(l => l.status === 'ganho').reduce((sum, l) => sum + (l.value || 0), 0);
        const completedTasks = vendorTasks.filter(t => t.status === 'completed').length;

        analysis[vendor] = {
            totalLeads,
            convertedLeads,
            conversionRate: totalLeads > 0 ? (convertedLeads / totalLeads * 100).toFixed(1) : 0,
            totalRevenue,
            averageTicket: convertedLeads > 0 ? (totalRevenue / convertedLeads).toFixed(2) : 0,
            completedTasks,
            taskCompletionRate: vendorTasks.length > 0 ? (completedTasks / vendorTasks.length * 100).toFixed(1) : 0
        };
    });

    return analysis;
}

function analyzePipelineTime(leads) {
    const stages = ['novo', 'contato', 'qualificado', 'proposta', 'negociacao', 'ganho'];
    const analysis = {};

    stages.forEach(stage => {
        const stageLeads = leads.filter(l => l.status === stage);
        const avgDaysInStage = stageLeads.length > 0 ?
            stageLeads.reduce((sum, lead) => {
                const daysSinceContact = lead.lastContact ?
                    Math.floor((new Date() - new Date(lead.lastContact)) / (1000 * 60 * 60 * 24)) : 0;
                return sum + daysSinceContact;
            }, 0) / stageLeads.length : 0;

        analysis[stage] = {
            count: stageLeads.length,
            averageDays: Math.round(avgDaysInStage),
            percentage: leads.length > 0 ? (stageLeads.length / leads.length * 100).toFixed(1) : 0
        };
    });

    return analysis;
}

function analyzeVendorPerformance(leads, tasks, logs) {
    const vendors = [...new Set(leads.map(l => l.responsible))];
    const analysis = {};

    vendors.forEach(vendor => {
        const vendorLeads = leads.filter(l => l.responsible === vendor);
        const vendorTasks = tasks.filter(t => t.assignee === vendor);
        const vendorLogs = logs.filter(l => l.userId === vendor);

        analysis[vendor] = {
            leads: {
                total: vendorLeads.length,
                converted: vendorLeads.filter(l => l.status === 'ganho').length,
                lost: vendorLeads.filter(l => l.status === 'perdido').length,
                active: vendorLeads.filter(l => !['ganho', 'perdido'].includes(l.status)).length
            },
            tasks: {
                total: vendorTasks.length,
                completed: vendorTasks.filter(t => t.status === 'completed').length,
                overdue: vendorTasks.filter(t => {
                    const dueDate = new Date(t.dueDate || t.dueDate);
                    return t.status === 'pending' && dueDate < new Date();
                }).length
            },
            activities: {
                total: vendorLogs.length,
                calls: vendorLogs.filter(l => l.type === 'call').length,
                meetings: vendorLogs.filter(l => l.type === 'meeting').length,
                emails: vendorLogs.filter(l => l.type === 'email').length
            },
            revenue: vendorLeads.filter(l => l.status === 'ganho').reduce((sum, l) => sum + (l.value || 0), 0)
        };
    });

    return analysis;
}

function analyzeConversionFunnel(leads) {
    const stages = ['novo', 'contato', 'qualificado', 'proposta', 'negociacao', 'ganho'];
    const funnel = {};

    stages.forEach((stage, index) => {
        const stageCount = leads.filter(l => l.status === stage).length;
        const previousStageCount = index > 0 ?
            leads.filter(l => stages.indexOf(l.status) >= index - 1).length : leads.length;

        funnel[stage] = {
            count: stageCount,
            conversionRate: previousStageCount > 0 ? (stageCount / previousStageCount * 100).toFixed(1) : 0,
            dropoffRate: previousStageCount > 0 ? ((previousStageCount - stageCount) / previousStageCount * 100).toFixed(1) : 0
        };
    });

    return funnel;
}

function analyzeLeadSources(leads) {
    const sources = [...new Set(leads.map(l => l.source))];
    const analysis = {};

    sources.forEach(source => {
        const sourceLeads = leads.filter(l => l.source === source);
        const convertedLeads = sourceLeads.filter(l => l.status === 'ganho');

        analysis[source] = {
            total: sourceLeads.length,
            converted: convertedLeads.length,
            conversionRate: sourceLeads.length > 0 ? (convertedLeads.length / sourceLeads.length * 100).toFixed(1) : 0,
            revenue: convertedLeads.reduce((sum, l) => sum + (l.value || 0), 0),
            averageValue: convertedLeads.length > 0 ?
                (convertedLeads.reduce((sum, l) => sum + (l.value || 0), 0) / convertedLeads.length).toFixed(2) : 0
        };
    });

    return analysis;
}

function analyzeActivitySummary(tasks, logs) {
    const today = new Date();
    const lastWeek = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000);
    const lastMonth = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);

    const analysis = {
        tasks: {
            total: tasks.length,
            completed: tasks.filter(t => t.status === 'completed').length,
            overdue: tasks.filter(t => {
                const dueDate = new Date(t.dueDate || t.dueDate);
                return t.status === 'pending' && dueDate < today;
            }).length,
            thisWeek: tasks.filter(t => new Date(t.created_at) >= lastWeek).length,
            thisMonth: tasks.filter(t => new Date(t.created_at) >= lastMonth).length
        },
        activities: {
            total: logs.length,
            thisWeek: logs.filter(l => new Date(l.timestamp) >= lastWeek).length,
            thisMonth: logs.filter(l => new Date(l.timestamp) >= lastMonth).length,
            byType: {}
        }
    };

    // Análise por tipo de atividade
    const activityTypes = [...new Set(logs.map(l => l.type))];
    activityTypes.forEach(type => {
        analysis.activities.byType[type] = logs.filter(l => l.type === type).length;
    });

    return analysis;
}

// Funções de filtro
function filterLeadsByDateAndVendor() {
    let filtered = [...leads];

    if (reportFilters.startDate) {
        filtered = filtered.filter(l =>
            new Date(l.lastContact || l.created_at) >= new Date(reportFilters.startDate));
    }

    if (reportFilters.endDate) {
        filtered = filtered.filter(l =>
            new Date(l.lastContact || l.created_at) <= new Date(reportFilters.endDate));
    }

    if (reportFilters.vendedor && reportFilters.vendedor !== '') {
        filtered = filtered.filter(l => l.responsible === reportFilters.vendedor);
    }

    return filtered;
}

function filterTasksByDateAndVendor() {
    let filtered = [...tasks];

    if (reportFilters.startDate) {
        filtered = filtered.filter(t =>
            new Date(t.created_at) >= new Date(reportFilters.startDate));
    }

    if (reportFilters.endDate) {
        filtered = filtered.filter(t =>
            new Date(t.created_at) <= new Date(reportFilters.endDate));
    }

    if (reportFilters.vendedor && reportFilters.vendedor !== '') {
        filtered = filtered.filter(t => t.assignee === reportFilters.vendedor);
    }

    return filtered;
}

function filterLogsByDateAndVendor() {
    let filtered = [...logs];

    if (reportFilters.startDate) {
        filtered = filtered.filter(l =>
            new Date(l.timestamp) >= new Date(reportFilters.startDate));
    }

    if (reportFilters.endDate) {
        filtered = filtered.filter(l =>
            new Date(l.timestamp) <= new Date(reportFilters.endDate));
    }

    if (reportFilters.vendedor && reportFilters.vendedor !== '') {
        filtered = filtered.filter(l => l.userId === reportFilters.vendedor);
    }

    return filtered;
}

// Renderização dos resultados
function renderReportResults() {
    const resultsContainer = document.getElementById('reportContent');

    let html = `
        <div class="report-header">
            <h2>${reportData.title}</h2>
            <div class="report-meta">
                <span><i class="fas fa-calendar"></i> Período: ${reportFilters.startDate || 'Início'} - ${reportFilters.endDate || 'Atual'}</span>
                ${reportFilters.vendedor ? `<span><i class="fas fa-user"></i> Vendedor: ${reportFilters.vendedor}</span>` : ''}
                <span><i class="fas fa-clock"></i> Gerado em: ${new Date().toLocaleString('pt-BR')}</span>
            </div>
        </div>
        
        <div class="report-summary">
            ${renderReportSummary()}
        </div>
        
        <div class="report-charts">
            ${renderReportChartsContainer()}
        </div>
        
        <div class="report-tables">
            ${renderReportTables()}
        </div>
    `;

    resultsContainer.innerHTML = html;
}

function renderReportSummary() {
    switch (currentReportType) {
        case 'sales_performance':
            return renderSalesPerformanceSummary();
        case 'pipeline_time':
            return renderPipelineTimeSummary();
        case 'vendor_analysis':
            return renderVendorAnalysisSummary();
        case 'conversion_funnel':
            return renderConversionFunnelSummary();
        case 'lead_source':
            return renderLeadSourceSummary();
        case 'activity_summary':
            return renderActivitySummaryContent();
        default:
            return '<p>Resumo não disponível</p>';
    }
}

function renderSalesPerformanceSummary() {
    const totalRevenue = Object.values(reportData.data).reduce((sum, vendor) => sum + vendor.totalRevenue, 0);
    const totalLeads = Object.values(reportData.data).reduce((sum, vendor) => sum + vendor.totalLeads, 0);
    const totalConverted = Object.values(reportData.data).reduce((sum, vendor) => sum + vendor.convertedLeads, 0);
    const avgConversion = totalLeads > 0 ? (totalConverted / totalLeads * 100).toFixed(1) : 0;

    return `
        <div class="summary-cards">
            <div class="summary-card">
                <h3>Receita Total</h3>
                <div class="summary-value">R$ ${formatCurrency(totalRevenue)}</div>
            </div>
            <div class="summary-card">
                <h3>Total de Leads</h3>
                <div class="summary-value">${totalLeads}</div>
            </div>
            <div class="summary-card">
                <h3>Leads Convertidos</h3>
                <div class="summary-value">${totalConverted}</div>
            </div>
            <div class="summary-card">
                <h3>Taxa de Conversão</h3>
                <div class="summary-value">${avgConversion}%</div>
            </div>
        </div>
    `;
}

function renderPipelineTimeSummary() {
    const totalLeads = Object.values(reportData.data).reduce((sum, stage) => sum + stage.count, 0);
    const avgTime = Object.values(reportData.data).reduce((sum, stage) => sum + stage.averageDays, 0) / Object.keys(reportData.data).length;

    return `
        <div class="summary-cards">
            <div class="summary-card">
                <h3>Total de Leads</h3>
                <div class="summary-value">${totalLeads}</div>
            </div>
            <div class="summary-card">
                <h3>Tempo Médio no Pipeline</h3>
                <div class="summary-value">${Math.round(avgTime)} dias</div>
            </div>
            <div class="summary-card">
                <h3>Estágio com Mais Leads</h3>
                <div class="summary-value">${Object.entries(reportData.data).sort((a, b) => b[1].count - a[1].count)[0][0]}</div>
            </div>
            <div class="summary-card">
                <h3>Estágio Mais Demorado</h3>
                <div class="summary-value">${Object.entries(reportData.data).sort((a, b) => b[1].averageDays - a[1].averageDays)[0][0]}</div>
            </div>
        </div>
    `;
}

function renderVendorAnalysisSummary() {
    const vendors = Object.keys(reportData.data);
    const topVendor = vendors.sort((a, b) => reportData.data[b].revenue - reportData.data[a].revenue)[0];
    const totalRevenue = vendors.reduce((sum, v) => sum + reportData.data[v].revenue, 0);

    return `
        <div class="summary-cards">
            <div class="summary-card">
                <h3>Total de Vendedores</h3>
                <div class="summary-value">${vendors.length}</div>
            </div>
            <div class="summary-card">
                <h3>Top Vendedor</h3>
                <div class="summary-value">${topVendor || 'N/A'}</div>
            </div>
            <div class="summary-card">
                <h3>Receita Total</h3>
                <div class="summary-value">R$ ${formatCurrency(totalRevenue)}</div>
            </div>
            <div class="summary-card">
                <h3>Média por Vendedor</h3>
                <div class="summary-value">R$ ${vendors.length > 0 ? formatCurrency(totalRevenue / vendors.length) : 0}</div>
            </div>
        </div>
    `;
}

function renderConversionFunnelSummary() {
    const stages = Object.keys(reportData.data);
    const totalLeads = Object.values(reportData.data).reduce((sum, stage) => sum + stage.count, 0);
    const finalConversion = reportData.data['ganho'] ? reportData.data['ganho'].conversionRate : 0;

    return `
        <div class="summary-cards">
            <div class="summary-card">
                <h3>Total de Leads</h3>
                <div class="summary-value">${totalLeads}</div>
            </div>
            <div class="summary-card">
                <h3>Conversão Final</h3>
                <div class="summary-value">${finalConversion}%</div>
            </div>
            <div class="summary-card">
                <h3>Estágios do Funil</h3>
                <div class="summary-value">${stages.length}</div>
            </div>
            <div class="summary-card">
                <h3>Maior Drop-off</h3>
                <div class="summary-value">${Object.entries(reportData.data).sort((a, b) => b[1].dropoffRate - a[1].dropoffRate)[0][0]}</div>
            </div>
        </div>
    `;
}

function renderLeadSourceSummary() {
    const sources = Object.keys(reportData.data);
    const totalLeads = Object.values(reportData.data).reduce((sum, source) => sum + source.total, 0);
    const bestSource = sources.sort((a, b) => reportData.data[b].conversionRate - reportData.data[a].conversionRate)[0];

    return `
        <div class="summary-cards">
            <div class="summary-card">
                <h3>Fontes de Lead</h3>
                <div class="summary-value">${sources.length}</div>
            </div>
            <div class="summary-card">
                <h3>Total de Leads</h3>
                <div class="summary-value">${totalLeads}</div>
            </div>
            <div class="summary-card">
                <h3>Melhor Fonte</h3>
                <div class="summary-value">${bestSource || 'N/A'}</div>
            </div>
            <div class="summary-card">
                <h3>Taxa Conversão Média</h3>
                <div class="summary-value">${sources.length > 0 ? (Object.values(reportData.data).reduce((sum, s) => sum + parseFloat(s.conversionRate), 0) / sources.length).toFixed(1) : 0}%</div>
            </div>
        </div>
    `;
}

function renderActivitySummaryContent() {
    const data = reportData.data;

    return `
        <div class="summary-cards">
            <div class="summary-card">
                <h3>Total de Tarefas</h3>
                <div class="summary-value">${data.tasks.total}</div>
            </div>
            <div class="summary-card">
                <h3>Tarefas Concluídas</h3>
                <div class="summary-value">${data.tasks.completed}</div>
            </div>
            <div class="summary-card">
                <h3>Total de Atividades</h3>
                <div class="summary-value">${data.activities.total}</div>
            </div>
            <div class="summary-card">
                <h3>Tarefas em Atraso</h3>
                <div class="summary-value">${data.tasks.overdue}</div>
            </div>
        </div>
    `;
}

function renderReportChartsContainer() {
    return `
        <div class="report-chart-container">
            <div id="reportChart" style="width: 100%; height: 400px;"></div>
        </div>
    `;
}

function renderReportTables() {
    switch (currentReportType) {
        case 'sales_performance':
            return renderSalesPerformanceTable();
        case 'vendor_analysis':
            return renderVendorAnalysisTable();
        case 'pipeline_time':
            return renderPipelineTimeTable();
        default:
            return '';
    }
}

function renderSalesPerformanceTable() {
    const vendors = Object.keys(reportData.data);

    return `
        <div class="report-table">
            <h3>Detalhamento por Vendedor</h3>
            <table class="table">
                <thead>
                    <tr>
                        <th>Vendedor</th>
                        <th>Leads</th>
                        <th>Convertidos</th>
                        <th>Taxa Conversão</th>
                        <th>Receita</th>
                        <th>Ticket Médio</th>
                        <th>Tarefas Concluídas</th>
                    </tr>
                </thead>
                <tbody>
                    ${vendors.map(vendor => {
        const data = reportData.data[vendor];
        return `
                            <tr>
                                <td>${vendor}</td>
                                <td>${data.totalLeads}</td>
                                <td>${data.convertedLeads}</td>
                                <td>${data.conversionRate}%</td>
                                <td>R$ ${formatCurrency(data.totalRevenue)}</td>
                                <td>R$ ${data.averageTicket}</td>
                                <td>${data.completedTasks}</td>
                            </tr>
                        `;
    }).join('')}
                </tbody>
            </table>
        </div>
    `;
}

function renderVendorAnalysisTable() {
    const vendors = Object.keys(reportData.data);

    return `
        <div class="report-table">
            <h3>Análise Completa por Vendedor</h3>
            <table class="table">
                <thead>
                    <tr>
                        <th>Vendedor</th>
                        <th>Leads Ativos</th>
                        <th>Leads Convertidos</th>
                        <th>Leads Perdidos</th>
                        <th>Tarefas Concluídas</th>
                        <th>Tarefas em Atraso</th>
                        <th>Atividades</th>
                        <th>Receita</th>
                    </tr>
                </thead>
                <tbody>
                    ${vendors.map(vendor => {
        const data = reportData.data[vendor];
        return `
                            <tr>
                                <td>${vendor}</td>
                                <td>${data.leads.active}</td>
                                <td>${data.leads.converted}</td>
                                <td>${data.leads.lost}</td>
                                <td>${data.tasks.completed}</td>
                                <td>${data.tasks.overdue}</td>
                                <td>${data.activities.total}</td>
                                <td>R$ ${formatCurrency(data.revenue)}</td>
                            </tr>
                        `;
    }).join('')}
                </tbody>
            </table>
        </div>
    `;
}

function renderPipelineTimeTable() {
    const stages = Object.keys(reportData.data);

    return `
        <div class="report-table">
            <h3>Tempo por Estágio do Pipeline</h3>
            <table class="table">
                <thead>
                    <tr>
                        <th>Estágio</th>
                        <th>Quantidade</th>
                        <th>Tempo Médio (dias)</th>
                        <th>Porcentagem do Total</th>
                    </tr>
                </thead>
                <tbody>
                    ${stages.map(stage => {
        const data = reportData.data[stage];
        return `
                            <tr>
                                <td>${getStatusLabel(stage)}</td>
                                <td>${data.count}</td>
                                <td>${data.averageDays}</td>
                                <td>${data.percentage}%</td>
                            </tr>
                        `;
    }).join('')}
                </tbody>
            </table>
        </div>
    `;
}

// Atualizar gráficos do relatório
function updateReportCharts() {
    const chartContainer = document.getElementById('reportChart');
    if (!chartContainer || !reportData) return;

    // Limpar gráfico anterior se existir
    chartContainer.innerHTML = '';

    let chartOptions = {};

    switch (currentReportType) {
        case 'sales_performance':
            chartOptions = createSalesPerformanceChart();
            break;
        case 'pipeline_time':
            chartOptions = createPipelineTimeChart();
            break;
        case 'vendor_analysis':
            chartOptions = createVendorAnalysisChart();
            break;
        case 'conversion_funnel':
            chartOptions = createConversionFunnelChart();
            break;
        case 'lead_source':
            chartOptions = createLeadSourceChart();
            break;
        case 'activity_summary':
            chartOptions = createActivitySummaryChart();
            break;
        default:
            chartOptions = createDefaultChart();
    }

    // Criar gráfico com ApexCharts
    const chart = new ApexCharts(chartContainer, chartOptions);
    chart.render();
}

// Criar gráfico de performance de vendas
function createSalesPerformanceChart() {
    const vendors = Object.keys(reportData.data);
    const revenues = vendors.map(v => reportData.data[v].totalRevenue);
    const conversions = vendors.map(v => parseFloat(reportData.data[v].conversionRate));

    return {
        series: [{
            name: 'Receita (R$)',
            type: 'column',
            data: revenues
        }, {
            name: 'Taxa de Conversão (%)',
            type: 'line',
            data: conversions
        }],
        chart: {
            height: 350,
            type: 'line',
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        stroke: {
            width: [0, 4]
        },
        title: {
            text: 'Performance de Vendas por Vendedor',
            style: {
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }
        },
        dataLabels: {
            enabled: true,
            enabledOnSeries: [1]
        },
        labels: vendors,
        xaxis: {
            type: 'category',
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: [{
            title: {
                text: 'Receita (R$)',
                style: {
                    color: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            },
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        }, {
            opposite: true,
            title: {
                text: 'Taxa de Conversão (%)',
                style: {
                    color: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            },
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        }],
        colors: ['#3b82f6', '#10b981'],
        theme: {
            mode: currentTheme
        }
    };
}

// Criar gráfico de tempo no pipeline
function createPipelineTimeChart() {
    const stages = Object.keys(reportData.data);
    const averageDays = stages.map(s => reportData.data[s].averageDays);
    const counts = stages.map(s => reportData.data[s].count);

    return {
        series: [{
            name: 'Tempo Médio (dias)',
            data: averageDays
        }],
        chart: {
            height: 350,
            type: 'bar',
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        plotOptions: {
            bar: {
                borderRadius: 4,
                horizontal: false,
            }
        },
        dataLabels: {
            enabled: false
        },
        title: {
            text: 'Tempo Médio por Estágio do Pipeline',
            style: {
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }
        },
        xaxis: {
            categories: stages.map(s => getStatusLabel(s)),
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            title: {
                text: 'Dias',
                style: {
                    color: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            },
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        colors: ['#6366f1'],
        theme: {
            mode: currentTheme
        }
    };
}

// Criar gráfico de análise por vendedor
function createVendorAnalysisChart() {
    const vendors = Object.keys(reportData.data);
    const totalLeads = vendors.map(v => reportData.data[v].leads.total);
    const convertedLeads = vendors.map(v => reportData.data[v].leads.converted);
    const revenues = vendors.map(v => reportData.data[v].revenue);

    return {
        series: [{
            name: 'Total de Leads',
            data: totalLeads
        }, {
            name: 'Leads Convertidos',
            data: convertedLeads
        }],
        chart: {
            height: 350,
            type: 'bar',
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        title: {
            text: 'Análise Comparativa por Vendedor',
            style: {
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }
        },
        xaxis: {
            categories: vendors,
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            title: {
                text: 'Quantidade de Leads',
                style: {
                    color: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            },
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        fill: {
            opacity: 1
        },
        colors: ['#3b82f6', '#10b981'],
        theme: {
            mode: currentTheme
        }
    };
}

// Criar gráfico de funil de conversão
function createConversionFunnelChart() {
    const stages = Object.keys(reportData.data);
    const counts = stages.map(s => reportData.data[s].count);

    return {
        series: [{
            name: 'Quantidade de Leads',
            data: counts
        }],
        chart: {
            height: 350,
            type: 'bar',
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        plotOptions: {
            bar: {
                borderRadius: 4,
                horizontal: true,
            }
        },
        dataLabels: {
            enabled: true
        },
        title: {
            text: 'Funil de Conversão - Leads por Estágio',
            style: {
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }
        },
        xaxis: {
            categories: stages.map(s => getStatusLabel(s)),
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        colors: ['#6366f1', '#f59e0b', '#10b981', '#8b5cf6', '#ef4444', '#06b6d4'],
        theme: {
            mode: currentTheme
        }
    };
}

// Criar gráfico de origem de leads
function createLeadSourceChart() {
    const sources = Object.keys(reportData.data);
    const totals = sources.map(s => reportData.data[s].total);

    return {
        series: totals,
        chart: {
            height: 350,
            type: 'donut',
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        title: {
            text: 'Distribuição de Leads por Origem',
            style: {
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }
        },
        labels: sources.map(s => s.charAt(0).toUpperCase() + s.slice(1)),
        dataLabels: {
            enabled: true,
            style: {
                colors: [currentTheme === 'dark' ? '#f1f5f9' : '#1e293b']
            }
        },
        legend: {
            position: 'bottom',
            labels: {
                colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
            }
        },
        colors: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ef4444'],
        theme: {
            mode: currentTheme
        }
    };
}

// Criar gráfico de resumo de atividades
function createActivitySummaryChart() {
    const data = reportData.data;
    const activityTypes = Object.keys(data.activities.byType);
    const activityCounts = activityTypes.map(t => data.activities.byType[t]);

    return {
        series: [{
            name: 'Quantidade',
            data: activityCounts
        }],
        chart: {
            height: 350,
            type: 'bar',
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        plotOptions: {
            bar: {
                borderRadius: 4,
                horizontal: false,
                distributed: true
            }
        },
        dataLabels: {
            enabled: true
        },
        title: {
            text: 'Resumo de Atividades por Tipo',
            style: {
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }
        },
        xaxis: {
            categories: activityTypes.map(t => t.charAt(0).toUpperCase() + t.slice(1)),
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            title: {
                text: 'Quantidade',
                style: {
                    color: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            },
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        colors: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ef4444', '#06b6d4'],
        theme: {
            mode: currentTheme
        },
        legend: {
            show: false
        }
    };
}

// Criar gráfico padrão quando não há dados
function createDefaultChart() {
    return {
        series: [{
            name: 'Sem dados',
            data: [0]
        }],
        chart: {
            height: 350,
            type: 'bar',
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        title: {
            text: 'Nenhum dado disponível para exibir',
            style: {
                color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
            }
        },
        xaxis: {
            categories: ['Sem dados'],
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        colors: ['#94a3b8'],
        theme: {
            mode: currentTheme
        }
    };
}

// Exportar relatório para PDF
async function exportToPDF() {
    try {
        // Simular geração de PDF
        showNotification('Preparando arquivo PDF...', 'info');

        // Em uma implementação real, usaríamos jsPDF ou similar
        const reportContent = document.getElementById('reportContent').innerHTML;

        // Simular download
        setTimeout(() => {
            const blob = new Blob([`
                <html>
                <head>
                    <title>${reportData.title}</title>
                    <style>
                        body { font-family: Arial, sans-serif; }
                        .summary-cards { display: flex; gap: 20px; margin: 20px 0; }
                        .summary-card { border: 1px solid #ddd; padding: 15px; border-radius: 8px; }
                        table { width: 100%; border-collapse: collapse; margin: 20px 0; }
                        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                        th { background-color: #f5f5f5; }
                    </style>
                </head>
                <body>
                    ${reportContent}
                </body>
                </html>
            `], { type: 'text/html' });

            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `relatorio_${currentReportType}_${new Date().toISOString().split('T')[0]}.html`;
            a.click();
            URL.revokeObjectURL(url);

            showNotification('Relatório exportado em PDF (simulação)!', 'success');
        }, 2000);

    } catch (error) {
        console.error('Erro ao exportar PDF:', error);
        showNotification('Erro ao exportar PDF', 'error');
    }
}

// Exportar relatório para Excel
async function exportToExcel() {
    try {
        showNotification('Preparando arquivo Excel...', 'info');

        // Simular geração de Excel
        const csvContent = generateCSVContent();

        setTimeout(() => {
            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `relatorio_${currentReportType}_${new Date().toISOString().split('T')[0]}.csv`;
            a.click();
            URL.revokeObjectURL(url);

            showNotification('Relatório exportado em Excel (CSV)!', 'success');
        }, 1500);

    } catch (error) {
        console.error('Erro ao exportar Excel:', error);
        showNotification('Erro ao exportar Excel', 'error');
    }
}

function generateCSVContent() {
    let csv = `${reportData.title}\n`;
    csv += `Período: ${reportFilters.startDate || 'Início'} - ${reportFilters.endDate || 'Atual'}\n`;
    csv += `Gerado em: ${new Date().toLocaleString('pt-BR')}\n\n`;

    switch (currentReportType) {
        case 'sales_performance':
            csv += 'Vendedor,Leads,Convertidos,Taxa Conversão,Receita,Ticket Médio\n';
            Object.entries(reportData.data).forEach(([vendor, data]) => {
                csv += `${vendor},${data.totalLeads},${data.convertedLeads},${data.conversionRate}%,R$ ${data.totalRevenue},R$ ${data.averageTicket}\n`;
            });
            break;
        case 'pipeline_time':
            csv += 'Estágio,Quantidade,Tempo Médio (dias),Porcentagem\n';
            Object.entries(reportData.data).forEach(([stage, data]) => {
                csv += `${getStatusLabel(stage)},${data.count},${data.averageDays},${data.percentage}%\n`;
            });
            break;
        default:
            csv += 'Dados não disponíveis para este tipo de relatório\n';
    }

    return csv;
}

// Imprimir relatório
function printReport() {
    const printWindow = window.open('', '_blank');
    const reportContent = document.getElementById('reportContent').innerHTML;

    printWindow.document.write(`
        <html>
        <head>
            <title>${reportData.title}</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                .summary-cards { display: flex; gap: 20px; margin: 20px 0; }
                .summary-card { border: 1px solid #ddd; padding: 15px; border-radius: 8px; flex: 1; }
                .summary-card h3 { margin: 0 0 10px 0; color: #333; }
                .summary-value { font-size: 24px; font-weight: bold; color: #0066cc; }
                table { width: 100%; border-collapse: collapse; margin: 20px 0; }
                th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                th { background-color: #f5f5f5; }
                .report-header h2 { color: #333; margin-bottom: 10px; }
                .report-meta { color: #666; margin-bottom: 20px; }
                .report-meta span { margin-right: 20px; }
                @media print { body { margin: 0; } }
            </style>
        </head>
        <body>
            ${reportContent}
        </body>
        </html>
    `);

    printWindow.document.close();
    printWindow.print();
}

// Função para popular dropdown de vendedores nos filtros
function populateVendedoresFilter() {
    const vendedorSelect = document.getElementById('reportVendedor');
    if (!vendedorSelect) return;

    vendedorSelect.innerHTML = '<option value="">Todos os Vendedores</option>';

    const vendedores = [...new Set(leads.map(l => l.responsible).filter(r => r))];
    vendedores.forEach(vendedor => {
        const option = document.createElement('option');
        option.value = vendedor;
        option.textContent = vendedor;
        vendedorSelect.appendChild(option);
    });
}

// Classic Reports Initialization
function initializeClassicReports() {
    console.log('Inicializando relatórios clássicos...');

    // Verificar se os elementos existem antes de criar os gráficos
    const funnelElement = document.getElementById('reportFunnelChart');
    const sourceElement = document.getElementById('reportSourceChart');
    const salesElement = document.getElementById('reportSalesChart');
    const conversionElement = document.getElementById('reportConversionChart');

    if (funnelElement) {
        initializeClassicFunnelChart();
    }

    if (sourceElement) {
        initializeClassicSourceChart();
    }

    if (salesElement) {
        initializeClassicSalesChart();
    }

    if (conversionElement) {
        initializeClassicConversionChart();
    }
}

function initializeClassicFunnelChart() {
    const element = document.getElementById('reportFunnelChart');
    if (!element || !leads) return;

    // Calcular dados reais do funil baseado nos leads
    const stageData = {};
    const stages = ['novo', 'contato', 'qualificado', 'proposta', 'negociacao', 'ganho'];

    stages.forEach(stage => {
        stageData[stage] = leads.filter(lead => lead.status === stage).length;
    });

    const options = {
        series: [{
            name: 'Quantidade de Leads',
            data: stages.map(stage => stageData[stage])
        }],
        chart: {
            type: 'bar',
            height: 300,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '60%',
                borderRadius: 4
            }
        },
        dataLabels: {
            enabled: true
        },
        colors: ['#6366f1', '#f59e0b', '#10b981', '#8b5cf6', '#ef4444', '#06b6d4'],
        xaxis: {
            categories: stages.map(stage => getStatusLabel(stage)),
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        },
        legend: {
            show: false
        }
    };

    if (charts.reportFunnel) {
        charts.reportFunnel.destroy();
    }
    charts.reportFunnel = new ApexCharts(element, options);
    charts.reportFunnel.render();
}

function initializeClassicSourceChart() {
    const element = document.getElementById('reportSourceChart');
    if (!element || !leads) return;

    // Calcular dados reais de origem baseado nos leads
    const sourceData = {};
    leads.forEach(lead => {
        const source = lead.source || 'unknown';
        sourceData[source] = (sourceData[source] || 0) + 1;
    });

    const sources = Object.keys(sourceData);
    const values = Object.values(sourceData);

    const options = {
        series: values,
        chart: {
            type: 'donut',
            height: 300,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        labels: sources.map(s => s.charAt(0).toUpperCase() + s.slice(1)),
        colors: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ef4444'],
        dataLabels: {
            enabled: true,
            style: {
                colors: [currentTheme === 'dark' ? '#f1f5f9' : '#1e293b']
            }
        },
        legend: {
            position: 'bottom',
            labels: {
                colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
            }
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '65%'
                }
            }
        },
        theme: {
            mode: currentTheme
        }
    };

    if (charts.reportSource) {
        charts.reportSource.destroy();
    }
    charts.reportSource = new ApexCharts(element, options);
    charts.reportSource.render();
}

function initializeClassicSalesChart() {
    const element = document.getElementById('reportSalesChart');
    if (!element || !leads) return;

    // Simular dados de vendas por mês (últimos 6 meses)
    const months = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'];
    const salesData = [];

    // Calcular vendas dos últimos meses baseado em leads ganhos
    const wonLeads = leads.filter(lead => lead.status === 'ganho');
    const currentMonth = new Date().getMonth();

    for (let i = 5; i >= 0; i--) {
        const monthIndex = (currentMonth - i + 12) % 12;
        // Simular distribuição de vendas ao longo dos meses
        const monthSales = Math.floor(wonLeads.length / 6) + Math.floor(Math.random() * 5);
        salesData.push(monthSales);
    }

    const options = {
        series: [{
            name: 'Vendas Realizadas',
            data: salesData
        }],
        chart: {
            type: 'line',
            height: 300,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        colors: ['#00ff88'],
        stroke: {
            curve: 'smooth',
            width: 5
        },
        markers: {
            size: 6,
            colors: ['#00ff88'],
            strokeColors: '#ffffff',
            strokeWidth: 2,
            hover: {
                size: 8
            }
        },
        fill: {
            type: 'gradient',
            gradient: {
                shadeIntensity: 1,
                opacityFrom: 0.4,
                opacityTo: 0.1,
                stops: [0, 90, 100]
            }
        },
        xaxis: {
            categories: months,
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        }
    };

    if (charts.reportSales) {
        charts.reportSales.destroy();
    }
    charts.reportSales = new ApexCharts(element, options);
    charts.reportSales.render();
}

function initializeClassicConversionChart() {
    const element = document.getElementById('reportConversionChart');
    if (!element || !leads) return;

    // Calcular taxa de conversão por vendedor
    const vendorData = {};
    leads.forEach(lead => {
        const vendor = lead.responsible || 'Não atribuído';
        if (!vendorData[vendor]) {
            vendorData[vendor] = { total: 0, converted: 0 };
        }
        vendorData[vendor].total++;
        if (lead.status === 'ganho') {
            vendorData[vendor].converted++;
        }
    });

    const vendors = Object.keys(vendorData);
    const conversionRates = vendors.map(vendor => {
        const data = vendorData[vendor];
        return data.total > 0 ? Math.round((data.converted / data.total) * 100) : 0;
    });

    const options = {
        series: [{
            name: 'Taxa de Conversão (%)',
            data: conversionRates
        }],
        chart: {
            type: 'bar',
            height: 300,
            background: 'transparent',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
        },
        colors: ['#3b82f6'],
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '60%',
                borderRadius: 4
            }
        },
        dataLabels: {
            enabled: true,
            formatter: function (val) {
                return val + '%';
            }
        },
        xaxis: {
            categories: vendors,
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                }
            }
        },
        yaxis: {
            max: 100,
            labels: {
                style: {
                    colors: currentTheme === 'dark' ? '#cbd5e1' : '#64748b'
                },
                formatter: function (value) {
                    return value + '%';
                }
            }
        },
        grid: {
            borderColor: currentTheme === 'dark' ? '#334155' : '#e2e8f0'
        },
        theme: {
            mode: currentTheme
        },
        legend: {
            show: false
        }
    };

    if (charts.reportConversion) {
        charts.reportConversion.destroy();
    }
    charts.reportConversion = new ApexCharts(element, options);
    charts.reportConversion.render();
}

// Export functions for global access
window.toggleTheme = toggleTheme;
window.openLeadModal = openLeadModal;
/*window.closeModal = closeModal;*/
window.submitLead = submitLead;
window.submitActivity = submitActivity;
window.submitNote = submitNote;
window.submitTask = submitTask;
window.openLeadDetails = openLeadDetails;
window.openEventModal = openEventModal;
window.openTaskModal = openTaskModal;
window.toggleTaskStatus = toggleTaskStatus;
window.editLead = editLead;
window.deleteLead = deleteLead;
window.scheduleActivity = scheduleActivity;
window.addNote = addNote;
window.changeLogsPage = changeLogsPage;
window.openTaskDetails = openTaskDetails;
window.applyLogsFilters = applyLogsFilters;
window.openNewCardModal = openNewCardModal;
window.submitNewCard = submitNewCard;
window.openLogDetails = openLogDetails;
window.changeTasksPage = changeTasksPage;
window.applyAdvancedFilters = applyAdvancedFilters;
window.clearAdvancedFilters = clearAdvancedFilters;
window.sortTasks = sortTasks;
window.toggleSortDirection = toggleSortDirection;
window.openTaskDetailsModal = openTaskDetailsModal;
window.updateTaskProgress = updateTaskProgress;
window.addTaskComment = addTaskComment;
window.deleteTaskWithConfirmation = deleteTaskWithConfirmation;
window.updateProgressDisplay = updateProgressDisplay;
window.editTaskInline = editTaskInline;
window.openTaskEditModal = openTaskEditModal;
window.loadImportantActions = loadImportantActions;

// Relatórios Customizáveis - Export functions
window.generateCustomReport = generateCustomReport;
window.exportToPDF = exportToPDF;
window.exportToExcel = exportToExcel;
window.printReport = printReport;
window.populateVendedoresFilter = populateVendedoresFilter;

// Lead Notes Management
async function loadAllLeadNotes() {
    try {
        // Fetch lead notes from the API
        const notes = await fetchFromAPI('/notes');

        // Iterate through each lead and assign its notes
        leads.forEach(lead => {
            lead.notes = notes.filter(note => note.leadId === lead.id).map(note => note.content).join('\n\n---\n\n') || '';
        });
    } catch (error) {
        console.error('Erro ao carregar notas dos leads:', error);
        showNotification('Erro ao carregar notas dos leads', 'error');
    }
}

// New Card Functionality
function openNewCardModal(status) {
    // Set the default status for the new card
    document.getElementById('newCardStatus').value = status;

    // Open the modal
    document.getElementById('newCardModal').style.display = 'block';
}

async function submitNewCard() {
    const form = document.getElementById('newCardForm');
    const formData = new FormData(form);

    const newLeadData = {
        name: formData.get('name'),
        company: formData.get('company'),
        email: formData.get('email'),
        phone: formData.get('phone'),
        position: formData.get('position'),
        status: formData.get('status'),
        source: formData.get('source'),
        responsible: 'Usuário Atual',
        score: 50,
        temperature: formData.get('temperature') || 'morno',
        value: parseFloat(formData.get('value')) || 0,
        notes: formData.get('notes'),
        lastContact: new Date().toISOString().split('T')[0]
    };

    try {
        // Create new lead
        const newLead = await fetchFromAPI('/leads', {
            method: 'POST',
            body: JSON.stringify(newLeadData)
        });

        // Add to local array
        leads.push(newLead);

        await addLog({
            type: 'lead',
            title: 'Novo lead criado',
            description: `Lead ${newLeadData.name} foi adicionado ao sistema via Kanban`,
            userId: 'Usuário Atual',
            leadId: newLead.id
        });

        showNotification('Lead criado com sucesso!', 'success');

        // Re-render components
        renderLeadsTable();
        renderKanbanBoard();

        // ✅ LÓGICA DE FECHAMENTO CORRIGIDA
        const modalEl = document.getElementById('newCardModal');
        if (modalEl) {
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            if (modalInstance) {
                modalInstance.hide();
            }
        }
        form.reset();

    } catch (error) {
        console.error('Erro ao salvar lead:', error);
        showNotification('Erro ao salvar lead', 'error');
    }
}

//Recent History
function renderRecentHistory() {
    const recentHistoryList = document.getElementById('recentHistoryList');
    if (!recentHistoryList) return;

    // Combine logs and calendar events (if calendar exists), sort by timestamp
    let combinedHistory = [...logs];

    if (calendar && calendar.getEvents) {
        const calendarEvents = calendar.getEvents().map(event => ({
            type: 'calendar',
            title: event.title,
            description: event.extendedProps?.description || '',
            timestamp: event.startStr,
            userId: 'Sistema'
        }));
        combinedHistory = [...logs, ...calendarEvents];
    }

    combinedHistory.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp));

    // Take the 5 most recent items
    const recentItems = combinedHistory.slice(0, 5);

    recentHistoryList.innerHTML = recentItems.map(item => `
        <div class="history-item">
            <i class="fas fa-${getHistoryIcon(item.type)}"></i>
            <div class="history-content">
                <div class="history-title">${item.title}</div>
                <div class="history-description">${item.description}</div>
                <div class="history-time">${formatDateTime(item.timestamp)}</div>
            </div>
        </div>
    `).join('');
}

function openLogDetails(logId) {
    const log = logs.find(l => l.id === logId);
    if (!log) {
        showNotification('Log não encontrado', 'error');
        return;
    }

    // Buscar informações do lead relacionado, se existir
    const lead = log.leadId ? leads.find(l => l.id === log.leadId) : null;

    // Definir cores baseadas no tipo de log
    const typeColors = {
        lead: '#3b82f6',
        email: '#f59e0b',
        call: '#10b981',
        task: '#8b5cf6',
        meeting: '#ef4444',
        note: '#06b6d4'
    };

    const typeLabels = {
        lead: 'Lead',
        email: 'Email',
        call: 'Ligação',
        task: 'Tarefa',
        meeting: 'Reunião',
        note: 'Nota'
    };

    const typeColor = typeColors[log.type] || '#6b7280';
    const typeLabel = typeLabels[log.type] || log.type;

    Swal.fire({
        title: 'Detalhes do Log',
        html: `
            <div style="text-align: left; padding: 20px;">
                <div style="margin-bottom: 20px; display: flex; align-items: center; gap: 12px;">
                    <div style="width: 40px; height: 40px; border-radius: 20px; background-color: ${typeColor}; color: white; display: flex; align-items: center; justify-content: center;">
                        <i class="fas fa-${getLogIcon(log.type)}"></i>
                    </div>
                    <div>
                        <h3 style="margin: 0; color: var(--text-primary); font-size: 18px;">${log.title}</h3>
                        <span style="background-color: ${typeColor}20; color: ${typeColor}; padding: 4px 8px; border-radius: 12px; font-size: 12px; font-weight: 600;">
                            ${typeLabel}
                        </span>
                    </div>
                </div>

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">Descrição:</strong><br>
                    <p style="margin: 8px 0; color: var(--text-secondary); background-color: var(--bg-secondary); padding: 12px; border-radius: 6px; border-left: 3px solid ${typeColor};">
                        ${log.description}
                    </p>
                </div>

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">Data e Hora:</strong><br>
                    <span style="color: var(--text-secondary); font-family: monospace;">
                        ${formatDateTime(log.timestamp)}
                    </span>
                </div>

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">Usuário:</strong><br>
                    <span style="color: var(--text-secondary);">
                        ${log.userId || log.userId || 'Não informado'}
                    </span>
                </div>

                ${lead ? `
                <div style="margin-bottom: 15px; padding: 12px; background-color: var(--bg-secondary); border-radius: 6px;">
                    <strong style="color: var(--text-primary);">Lead Relacionado:</strong><br>
                    <div style="margin-top: 8px;">
                        <span style="color: var(--primary-color); font-weight: 600; cursor: pointer;" onclick="openLeadDetails(${lead.id}); Swal.close();">
                            ${lead.name}
                        </span>
                        <br>
                        <span style="color: var(--text-secondary); font-size: 14px;">
                            ${lead.company} • ${lead.email}
                        </span>
                        <br>
                        <span class="status-badge status-${lead.status}" style="font-size: 11px; margin-top: 4px; display: inline-block;">
                            ${getStatusLabel(lead.status)}
                        </span>
                    </div>
                </div>
                ` : ''}

                <div style="margin-bottom: 15px;">
                    <strong style="color: var(--text-primary);">ID do Log:</strong><br>
                    <span style="color: var(--text-muted); font-family: monospace; font-size: 12px;">
                        #${log.id}
                    </span>
                </div>
            </div>
        `,
        icon: 'info',
        showConfirmButton: true,
        confirmButtonText: 'Fechar',
        confirmButtonColor: typeColor,
        width: '600px',
        background: currentTheme === 'dark' ? '#1e293b' : '#ffffff',
        color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b',
        customClass: {
            popup: 'log-details-modal'
        }
    });
}

function getHistoryIcon(type) {
    const icons = {
        lead: 'user-plus',
        email: 'envelope',
        call: 'phone',
        task: 'tasks',
        meeting: 'calendar-alt',
        note: 'sticky-note',
        calendar: 'calendar-alt'
    };
    return icons[type] || 'info-circle';
}

// Advanced Task Functions
function applyAdvancedFilters() {
    const startDate = document.getElementById('taskStartDate').value;
    const endDate = document.getElementById('taskEndDate').value;
    const assignee = document.getElementById('taskAssigneeFilter').value;
    const priority = document.getElementById('taskPriorityFilter').value;

    advancedFilters = {};
    if (startDate) advancedFilters.startDate = startDate;
    if (endDate) advancedFilters.endDate = endDate;
    if (assignee) advancedFilters.assignee = assignee;
    if (priority) advancedFilters.priority = priority;

    currentTasksPage = 1;
    renderTasksList();
    showNotification('Filtros aplicados com sucesso!', 'success');
}

function clearAdvancedFilters() {
    document.getElementById('taskStartDate').value = '';
    document.getElementById('taskEndDate').value = '';
    document.getElementById('taskAssigneeFilter').value = '';
    document.getElementById('taskPriorityFilter').value = '';

    advancedFilters = {};
    currentTasksPage = 1;
    renderTasksList();
    showNotification('Filtros removidos!', 'info');
}

function sortTasks() {
    const sortSelect = document.getElementById('taskSortBy');
    currentTaskSort = sortSelect.value;
    renderTasksList();
}

function toggleSortDirection() {
    sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
    const sortIcon = document.getElementById('sortIcon');
    sortIcon.className = sortDirection === 'asc' ? 'fas fa-sort-amount-up' : 'fas fa-sort-amount-down';
    renderTasksList();
}

function enableTaskDragAndDrop() {
    const tasksContainer = document.getElementById('sortableTasks');
    if (!tasksContainer) return;

    let draggedElement = null;

    tasksContainer.addEventListener('dragstart', (e) => {
        if (e.target.classList.contains('task-item')) {
            draggedElement = e.target;
            e.target.classList.add('dragging');
        }
    });

    tasksContainer.addEventListener('dragend', (e) => {
        if (e.target.classList.contains('task-item')) {
            e.target.classList.remove('dragging');
            draggedElement = null;
        }
    });

    tasksContainer.addEventListener('dragover', (e) => {
        e.preventDefault();
        const afterElement = getDragAfterElement(tasksContainer, e.clientY);
        if (draggedElement) {
            if (afterElement == null) {
                tasksContainer.appendChild(draggedElement);
            } else {
                tasksContainer.insertBefore(draggedElement, afterElement);
            }
        }
    });

    tasksContainer.addEventListener('drop', (e) => {
        e.preventDefault();
        if (draggedElement) {
            updateTaskOrder();
        }
    });
}

function getDragAfterElement(container, y) {
    const draggableElements = [...container.querySelectorAll('.task-item:not(.dragging)')];

    return draggableElements.reduce((closest, child) => {
        const box = child.getBoundingClientRect();
        const offset = y - box.top - box.height / 2;

        if (offset < 0 && offset > closest.offset) {
            return { offset: offset, element: child };
        } else {
            return closest;
        }
    }, { offset: Number.NEGATIVE_INFINITY }).element;
}

async function updateTaskOrder() {
    const tasksContainer = document.getElementById('sortableTasks');
    const taskItems = tasksContainer.querySelectorAll('.task-item');

    const updates = [];
    taskItems.forEach((item, index) => {
        const taskId = item.dataset.taskId;
        updates.push(updateTaskOrderAPI(taskId, index));
    });

    try {
        await Promise.all(updates);
        showNotification('Ordem das tarefas atualizada!', 'success');
    } catch (error) {
        console.error('Erro ao atualizar ordem:', error);
        showNotification('Erro ao atualizar ordem das tarefas', 'error');
    }
}

async function updateTaskOrderAPI(taskId, sortOrder) {
    return fetchFromAPI(`/tasks/${taskId}/order`, {
        method: 'PUT',
        body: JSON.stringify({ sortOrder })
    });
}

// ✅ VERSÃO CORRIGIDA
async function openTaskDetailsModal(taskId) {
    currentTaskId = taskId;
    const task = tasks.find(t => t.id === taskId);
    if (!task) {
        showNotification('Tarefa não encontrada', 'error');
        return;
    }

    // ✅ CORREÇÃO: Usar o ID correto do título do modal
    document.getElementById('taskDetailsModalLabel').textContent = `Detalhes: ${task.title}`;

    // Preenche o resto do modal
    document.getElementById('taskProgressSlider').value = task.progress || 0;
    document.getElementById('taskProgressText').textContent = `${task.progress || 0}%`;
    document.getElementById('taskProgressBar').style.width = `${task.progress || 0}%`;

    // Carrega dados dinâmicos
    await loadTaskComments(taskId);
    await loadTaskAttachments(taskId);

    // Abre o modal da forma correta com a API do Bootstrap
    const modalElement = document.getElementById('taskDetailsModal');
    if (modalElement) {
        const bootstrapModal = bootstrap.Modal.getOrCreateInstance(modalElement);
        bootstrapModal.show();
    }
}


// ✅ VERSÃO CORRIGIDA E MELHORADA
async function loadTaskComments(taskId) {
    const commentsList = document.getElementById('taskCommentsList');
    if (!commentsList) return;

    try {
        const comments = await fetchFromAPI(`/tasks/${taskId}/comments`);

        if (!comments || comments.length === 0) {
            commentsList.innerHTML = '<p class="text-muted text-center small mt-2">Nenhum comentário ainda.</p>';
            return;
        }

        commentsList.innerHTML = comments.map(comment => `
            <div class="comment-item">
                <div class="comment-header">
                    <span class="comment-author">${comment.userName || comment.userId}</span>
                    
                    <span class="comment-date">${formatDateTime(comment.createdAt)}</span> 
                </div>
                
                <div class="comment-text">${comment.content}</div> 
            </div>
        `).join('');

    } catch (error) {
        console.error('Erro ao carregar comentários:', error);
        commentsList.innerHTML = '<p class="text-danger text-center small mt-2">Erro ao carregar comentários.</p>';
    }
}


// ✅ FUNÇÃO FALTANTE
// ✅ VERSÃO FINAL E CORRIGIDA
async function loadTaskAttachments(taskId) {
    const attachmentsList = document.getElementById('taskAttachmentsList');
    if (!attachmentsList) return;

    try {
        const attachments = await fetchFromAPI(`/tasks/${taskId}/attachments`);

        if (!attachments || attachments.length === 0) {
            attachmentsList.innerHTML = '<p class="text-muted text-center small mt-2">Nenhum anexo para esta tarefa.</p>';
            return;
        }

        attachmentsList.innerHTML = attachments.map(attachment => {
            // ✅ CORREÇÃO: Usando suas duas funções em conjunto!
            const fileType = getFileTypeFromMime(attachment.mimeType);
            const icon = getFileIcon(fileType);

            return `
                <div class="attachment-item">
                    <div class="attachment-info">
                        <i class="fas fa-${icon} attachment-icon"></i>
                        <div>
                            <span class="attachment-name">${attachment.fileName}</span>
                            <span class="attachment-size">(${formatFileSize(attachment.fileSize)})</span>
                        </div>
                    </div>
                    <div class="attachment-actions">
                        <button class="btn-icon" onclick="downloadAttachment('${attachment.fileUrl}')" title="Download">
                            <i class="fas fa-download"></i>
                        </button>
                        <button class="btn-icon" onclick="deleteAttachment('${attachment.id}')" title="Excluir">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            `;
        }).join('');

    } catch (error) {
        console.error('Erro ao carregar anexos:', error);
        attachmentsList.innerHTML = '<p class="text-danger text-center small mt-2">Erro ao carregar anexos.</p>';
    }
}

async function updateTaskProgress(progress) {
    if (!currentTaskId) return;

    document.getElementById('taskProgressText').textContent = `${progress}%`;
    document.getElementById('taskProgressBar').style.width = `${progress}%`;

    try {
        await fetchFromAPI(`/tasks/${currentTaskId}/progress`, {
            method: 'PUT',
            body: JSON.stringify({ progress: parseInt(progress) })
        });

        // Update local tasks array
        const task = tasks.find(t => t.id === currentTaskId);
        if (task) {
            task.progress = parseInt(progress);
        }

        renderTasksList();
        showNotification('Progresso atualizado!', 'success');
    } catch (error) {
        console.error('Erro ao atualizar progresso:', error);
        showNotification('Erro ao atualizar progresso', 'error');
    }
}

// ✅ VERSÃO CORRIGIDA
async function addTaskComment() {
    const commentText = document.getElementById('newComment').value.trim();

    if (!commentText || !currentTaskId) {
        showNotification('Digite um comentário', 'warning');
        return;
    }

    try {
        await fetchFromAPI(`/tasks/${currentTaskId}/comments`, {
            method: 'POST',
            body: JSON.stringify({
                // ✅ CORREÇÃO: O nome da propriedade foi alterado para 'Content'.
                // O ASP.NET irá mapear isso para a propriedade Content da ViewModel.
                Content: commentText,
                UserId: 'Usuário Atual'
            })
        });

        document.getElementById('newComment').value = '';
        await loadTaskComments(currentTaskId);
        showNotification('Comentário adicionado!', 'success');

    } catch (error) {
        console.error('Erro ao adicionar comentário:', error);
        showNotification('Erro ao adicionar comentário. Verifique o console.', 'error');
    }
}

// Função para lidar com upload de arquivos
async function handleFileUpload() {
    const fileInput = document.getElementById('attachmentFile');
    const files = fileInput.files;

    if (!files || files.length === 0 || !currentTaskId) {
        showNotification('Selecione pelo menos um arquivo', 'warning');
        return;
    }

    for (let i = 0; i < files.length; i++) {
        const file = files[i];

        try {
            // Simular upload (em uma implementação real, você faria upload para um serviço de storage)
            const fakeUrl = `https://example.com/uploads/${file.name}`;

            const attachmentData = {
                filename: file.name,
                fileUrl: fakeUrl,
                fileSize: file.size,
                mimeType: file.type,
                uploadedBy: 'Usuário Atual'
            };

            await fetchFromAPI(`/tasks/${currentTaskId}/attachments`, {
                method: 'POST',
                body: JSON.stringify(attachmentData)
            });

            showNotification(`Arquivo "${file.name}" adicionado com sucesso!`, 'success');
        } catch (error) {
            console.error('Erro ao fazer upload do arquivo:', error);
            showNotification(`Erro ao fazer upload de "${file.name}"`, 'error');
        }
    }

    // Limpar input e recarregar anexos
    fileInput.value = '';
    await loadTaskAttachments(currentTaskId);

    // Recarregar grid de arquivos se estivermos na aba Files
    const currentTab = document.querySelector('.tab-content.active');
    if (currentTab && currentTab.id === 'files') {
        renderFilesGrid();
    }
}

// Função para abrir modal de upload de arquivos (para a aba Files)
function openFileUploadModal() {
    // Simular um modal de upload de arquivos
    const input = document.createElement('input');
    input.type = 'file';
    input.multiple = true;
    input.accept = '*/*';

    input.onchange = function (event) {
        const files = event.target.files;
        if (files && files.length > 0) {
            // Simular processamento de arquivos para a aba Files
            let filesList = [];
            for (let i = 0; i < files.length; i++) {
                filesList.push(files[i].name);
            }
            showNotification(`${files.length} arquivo(s) selecionado(s): ${filesList.join(', ')}`, 'info');

            // Em uma implementação real, você faria o upload aqui
            setTimeout(() => {
                showNotification('Upload simulado concluído! (Implementação completa requer serviço de storage)', 'success');
                renderFilesGrid(); // Atualizar grid de arquivos
            }, 2000);
        }
    };

    input.click();
}

// Função para filtrar arquivos na aba Files
async function filterFiles() {
    const searchTerm = document.getElementById('filesSearch')?.value.toLowerCase() || '';
    const typeFilter = document.getElementById('fileTypeFilter')?.value || '';
    const taskFilter = document.getElementById('taskFilter')?.value || '';

    try {
        const attachments = await fetchFromAPI('/attachments');

        let filteredFiles = attachments.filter(file => {
            // Filtro de busca por nome
            const matchesSearch = !searchTerm || file.filename.toLowerCase().includes(searchTerm);

            // Filtro por tipo
            const fileType = getFileTypeFromMime(file.mimeType);
            const matchesType = !typeFilter || fileType === typeFilter;

            // Filtro por tarefa
            const matchesTask = !taskFilter || file.taskId == taskFilter;

            return matchesSearch && matchesType && matchesTask;
        });

        const filesGrid = document.getElementById('filesGrid');
        if (!filesGrid) return;

        if (filteredFiles.length === 0) {
            filesGrid.innerHTML = `
                <div class="files-empty">
                    <i class="fas fa-search"></i>
                    <h3>Nenhum arquivo encontrado</h3>
                    <p>Nenhum arquivo corresponde aos filtros selecionados.</p>
                </div>
            `;
            return;
        }

        filesGrid.innerHTML = filteredFiles.map(file => {
            const fileType = getFileTypeFromMime(file.mimeType);
            const formattedSize = formatFileSize(file.fileSize);
            const uploadDate = formatDate(file.created_at);

            return `
                <div class="file-item">
                    <div class="file-icon">
                        <i class="fas fa-${getFileIcon(fileType)}"></i>
                    </div>
                    <div class="file-info">
                        <div class="file-name">${file.filename}</div>
                        <div class="file-meta">
                            <span class="file-size">${formattedSize}</span>
                            ${file.taskTitle ? `<span class="file-task">• ${file.taskTitle}</span>` : ''}
                            <span class="file-date">• ${uploadDate}</span>
                        </div>
                    </div>
                    <div class="file-actions">
                        <button class="btn-icon" onclick="downloadAttachment('${file.fileUrl}')" title="Download">
                            <i class="fas fa-download"></i>
                        </button>
                        <button class="btn-icon" onclick="deleteFileFromGrid(${file.id})" title="Excluir">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            `;
        }).join('');

        if (searchTerm || typeFilter || taskFilter) {
            showNotification(`${filteredFiles.length} arquivo(s) encontrado(s)`, 'info');
        }

    } catch (error) {
        console.error('Erro ao filtrar arquivos:', error);
        showNotification('Erro ao aplicar filtros', 'error');
    }
}

// Função para popular dropdown de tarefas na aba Files
function populateTasksFilter() {
    const taskFilter = document.getElementById('taskFilter');
    if (!taskFilter || !tasks) return;

    taskFilter.innerHTML = '<option value="">Todas as Tarefas</option>';
    tasks.forEach(task => {
        const option = document.createElement('option');
        option.value = task.id;
        option.textContent = task.title;
        taskFilter.appendChild(option);
    });
}

// Função para renderizar grid de arquivos
async function renderFilesGrid() {
    const filesGrid = document.getElementById('filesGrid');
    if (!filesGrid) return;

    // Popular dropdown de tarefas quando renderizar os arquivos
    populateTasksFilter();

    try {
        // Buscar arquivos reais do banco de dados
        const attachments = await fetchFromAPI('/attachments');

        if (attachments.length === 0) {
            filesGrid.innerHTML = `
                <div class="files-empty">
                    <i class="fas fa-folder-open"></i>
                    <h3>Nenhum arquivo encontrado</h3>
                    <p>Faça upload de arquivos através das tarefas para vê-los aqui.</p>
                </div>
            `;
            return;
        }

        filesGrid.innerHTML = attachments.map(file => {
            const fileType = getFileTypeFromMime(file.mimeType);
            const formattedSize = formatFileSize(file.fileSize);
            const uploadDate = formatDate(file.created_at);

            return `
                <div class="file-item">
                    <div class="file-icon">
                        <i class="fas fa-${getFileIcon(fileType)}"></i>
                    </div>
                    <div class="file-info">
                        <div class="file-name">${file.filename}</div>
                        <div class="file-meta">
                            <span class="file-size">${formattedSize}</span>
                            ${file.taskTitle ? `<span class="file-task">• ${file.taskTitle}</span>` : ''}
                            <span class="file-date">• ${uploadDate}</span>
                        </div>
                    </div>
                    <div class="file-actions">
                        <button class="btn-icon" onclick="downloadAttachment('${file.fileUrl}')" title="Download">
                            <i class="fas fa-download"></i>
                        </button>
                        <button class="btn-icon" onclick="deleteFileFromGrid(${file.id})" title="Excluir">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            `;
        }).join('');

    } catch (error) {
        console.error('Erro ao carregar arquivos:', error);
        filesGrid.innerHTML = `
            <div class="files-empty">
                <i class="fas fa-exclamation-triangle"></i>
                <h3>Erro ao carregar arquivos</h3>
                <p>Não foi possível carregar a lista de arquivos.</p>
            </div>
        `;
        showNotification('Erro ao carregar arquivos', 'error');
    }
}

function getFileIcon(type) {
    const icons = {
        document: 'file-alt',
        image: 'image',
        archive: 'file-archive',
        video: 'file-video',
        audio: 'file-audio',
        pdf: 'file-pdf',
        excel: 'file-excel',
        word: 'file-word',
        powerpoint: 'file-powerpoint',
        other: 'file'
    };
    return icons[type] || 'file';
}

function getFileTypeFromMime(mimeType) {
    if (!mimeType) return 'other';

    if (mimeType.startsWith('image/')) return 'image';
    if (mimeType.startsWith('video/')) return 'video';
    if (mimeType.startsWith('audio/')) return 'audio';
    if (mimeType === 'application/pdf') return 'pdf';
    if (mimeType.includes('excel') || mimeType.includes('spreadsheet')) return 'excel';
    if (mimeType.includes('word') || mimeType.includes('document')) return 'word';
    if (mimeType.includes('powerpoint') || mimeType.includes('presentation')) return 'powerpoint';
    if (mimeType.includes('zip') || mimeType.includes('rar') || mimeType.includes('archive')) return 'archive';
    if (mimeType.startsWith('text/') || mimeType.includes('document')) return 'document';

    return 'other';
}

function downloadFile(filename) {
    showNotification(`Download de "${filename}" iniciado (simulação)`, 'info');
}

async function deleteFileFromGrid(fileId) {
    const result = await Swal.fire({
        title: 'Confirmar Exclusão',
        text: 'Tem certeza que deseja excluir este arquivo?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sim, excluir!',
        cancelButtonText: 'Cancelar',
        background: currentTheme === 'dark' ? '#1e293b' : '#ffffff',
        color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
    });

    if (result.isConfirmed) {
        try {
            await fetchFromAPI(`/attachments/${fileId}`, {
                method: 'DELETE'
            });

            showNotification('Arquivo excluído com sucesso!', 'success');
            renderFilesGrid(); // Recarregar grid de arquivos
        } catch (error) {
            console.error('Erro ao excluir arquivo:', error);
            showNotification('Erro ao excluir arquivo', 'error');
        }
    }
}

function deleteFile(fileId) {
    // Manter compatibilidade com código antigo
    deleteFileFromGrid(fileId);
}


function formatFileSize(bytes) {
    if (!bytes) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function downloadAttachment(url) {
    window.open(url, '_blank');
}

async function deleteAttachment(attachmentId) {
    const result = await Swal.fire({
        title: 'Confirmar Exclusão',
        text: 'Tem certeza que deseja excluir este anexo?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sim, excluir!',
        cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
        try {
            await fetchFromAPI(`/tasks/${currentTaskId}/attachments/${attachmentId}`, {
                method: 'DELETE'
            });
            await loadTaskAttachments(currentTaskId);
            showNotification('Anexo excluído!', 'success');
        } catch (error) {
            console.error('Erro ao excluir anexo:', error);
            showNotification('Erro ao excluir anexo', 'error');
        }
    }
}



// Função para confirmar e deletar uma tarefa (Versão Corrigida)
async function deleteTaskWithConfirmation(taskId = null) {
    const targetTaskId = taskId || currentTaskId;
    const task = tasks.find(t => t.id === targetTaskId);

    if (!task) {
        showNotification('Tarefa não encontrada', 'error');
        return;
    }

    const result = await Swal.fire({
        title: 'Confirmar Exclusão',
        text: `Tem certeza que deseja excluir a tarefa "${task.title}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sim, excluir!',
        cancelButtonText: 'Cancelar',
        background: currentTheme === 'dark' ? '#1e293b' : '#ffffff',
        color: currentTheme === 'dark' ? '#f1f5f9' : '#1e293b'
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        await fetchFromAPI(`/tasks/${targetTaskId}`, {
            method: 'DELETE'
        });

        // Remove a tarefa do array local de dados
        tasks = tasks.filter(t => t.id !== targetTaskId);

        // Se a exclusão foi chamada de dentro do modal de detalhes (ou seja, sem um taskId direto),
        // nós o fechamos programaticamente.
        if (!taskId && currentTaskId) {
            const modalEl = document.getElementById('taskDetailsModal');
            if (modalEl) {
                const modalInstance = bootstrap.Modal.getInstance(modalEl);
                if (modalInstance) {
                    modalInstance.hide();
                }
            }
        }

        // Re-renderiza a lista de tarefas para refletir a exclusão
        renderTasksList();

        showNotification('Tarefa excluída com sucesso!', 'success');

    } catch (error) {
        try {
            // Se a resposta contém JSON com erro
            if (error.message && error.message.includes('{')) {
                const errorStart = error.message.indexOf('{');
                const errorJson = error.message.substring(errorStart);
                const parsedError = JSON.parse(errorJson);
                if (parsedError.error) {
                    userMessage = parsedError.error;
                }
            }
            // Se é uma mensagem direta que contém "possui"
            else if (error.message && error.message.includes('possui')) {
                userMessage = error.message;
            }
        } catch (parseError) {
            console.error('Erro ao processar mensagem:', parseError);
        }

        console.error('Erro ao excluir tarefa:', error);
        showNotification('Erro ao excluir tarefa', 'error');
    }
}


function updateProgressDisplay(value) {
    const display = document.getElementById('progressDisplay');
    if (display) {
        display.textContent = `${value}%`;
    }
}

function editTaskInline(taskId) {
    // Garante que o clique no botão não dispare o clique da linha <tr>
    if (event) {
        event.stopPropagation();
    }
    openTaskEditModal(taskId);
}

function openTaskEditModal(taskId) {
    const task = tasks.find(t => t.id === taskId);
    if (!task) {
        showNotification('Tarefa não encontrada', 'error');
        return;
    }

    const form = document.getElementById('taskForm');
    form.reset();

    // Preencher os campos de texto do formulário
    document.getElementById('taskId').value = task.id;
    document.getElementById('taskModalTitle').textContent = `Editar Tarefa: ${task.title}`;
    document.getElementById('taskTitle').value = task.title;
    document.getElementById('taskDescription').value = task.description || '';

    // Formatar a data para o input type="date"
    if (task.dueDate) {
        try {
            const date = new Date(task.dueDate);
            if (!isNaN(date.getTime())) {
                document.getElementById('taskDueDate').value = date.toISOString().split('T')[0];
            }
        } catch (e) {
            console.error("Erro ao formatar a data da tarefa:", e);
        }
    }

    document.getElementById('taskPriority').value = task.priority.toLowerCase();
    document.getElementById('taskAssignee').value = task.assignee;
    document.getElementById('taskProgress').value = task.progress || 0;
    updateProgressDisplay(task.progress || 0);

    // ✅ CÓDIGO RESTAURADO E CORRIGIDO PARA POPULAR O DROPDOWN DE LEADS
    const leadSelect = document.getElementById('taskLeadId');
    leadSelect.innerHTML = '<option value="">Nenhum lead relacionado</option>'; // Limpa e adiciona a opção padrão

    if (leads && leads.length > 0) {
        leads.forEach(lead => {
            const option = document.createElement('option');
            option.value = lead.id; // O ID do lead (Guid como string)
            option.textContent = `${lead.name} - ${lead.company}`;

            // Compara o lead da iteração com o leadId da tarefa para pré-selecionar
            if (lead.id === task.leadId) {
                option.selected = true;
            }

            leadSelect.appendChild(option);
        });
    }
    // FIM DO BLOCO CORRIGIDO

    // Abrir o modal usando a API do Bootstrap
    const modalElement = document.getElementById('taskModal');
    if (modalElement) {
        const bootstrapModal = bootstrap.Modal.getOrCreateInstance(modalElement);
        bootstrapModal.show();
    }
}
