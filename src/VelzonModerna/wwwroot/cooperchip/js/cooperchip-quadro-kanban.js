
/* cooperchip-quadro-kanban.js */

// =======================================================
// CONSTANTES E VARIÁVEIS GLOBAIS
// =======================================================
const BASE_URL = "/QuadroKanban";
let tasks = [];       // Array local das tarefas
let participants = []; // Array local dos participantes

// Undo/Redo (Não implementado nesta versão, mas estrutura presente)
const MAX_HISTORY = 20;
let doneActions = [];


// Filtro e ordenação
let searchQuery = "";
let sortOption = "none";

// Token CSRF (se necessário, descomentar e garantir que está presente no HTML)
// const token = $('input[name="__RequestVerificationToken"]').val();

// =======================================================
// FUNÇÕES DE API (FETCH)
// =======================================================

// ------------ Tarefas ------------
async function apiGetTasks() {
    try {
        const response = await fetch(`${BASE_URL}/Tasks`);
        if (!response.ok) {
            console.error("Falha ao obter tarefas:", response.status, response.statusText);
            // Tentar ler o corpo do erro se disponível
            try {
                const errorBody = await response.json();
                console.error("Detalhes do erro (tarefas):", errorBody);
            } catch (e) {
                // Ignorar se não conseguir parsear o corpo do erro
            }
            return [];
        }
        return await response.json();
    } catch (error) {
        console.error("Erro de rede ao obter tarefas:", error);
        return [];
    }
}

async function apiCreateTask(task) {
    try {
        const response = await fetch(`${BASE_URL}/Tasks`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                // 'RequestVerificationToken': token // Descomentar se usar CSRF token
            },
            body: JSON.stringify(task)
        });
        if (!response.ok) {
            const error = await response.json().catch(() => ({ message: "Erro desconhecido ao criar tarefa." }));
            console.error("Erro ao criar task:", response.status, error);
            return null;
        }
        return await response.json();
    } catch (error) {
        console.error("Erro de rede ao criar tarefa:", error);
        return null;
    }
}

async function apiUpdateTask(task) {
    try {
        const response = await fetch(`${BASE_URL}/Tasks/${task.id}`, {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
                // 'RequestVerificationToken': token // Descomentar se usar CSRF token
            },
            body: JSON.stringify(task)
        });
        if (!response.ok) {
            const error = await response.json().catch(() => ({ message: "Erro desconhecido ao atualizar tarefa." }));
            console.error("Erro ao atualizar task:", response.status, error);
            return null;
        }
        return await response.json();
    } catch (error) {
        console.error("Erro de rede ao atualizar tarefa:", error);
        return null;
    }
}

async function apiDeleteTask(taskId) {
    try {
        const response = await fetch(`${BASE_URL}/Tasks/${taskId}`, {
            method: "DELETE",
            headers: {
                // 'RequestVerificationToken': token // Descomentar se usar CSRF token
            }
        });
        if (!response.ok) {
            const error = await response.json().catch(() => ({ message: "Erro desconhecido ao excluir tarefa." }));
            console.error("Erro ao excluir task:", response.status, error);
            return false;
        }
        return true;
    } catch (error) {
        console.error("Erro de rede ao excluir tarefa:", error);
        return false;
    }
}

// ------------ Participantes ------------
async function apiGetParticipants() {
    try {
        const response = await fetch(`${BASE_URL}/Participantes`);
        if (!response.ok) {
            console.error("Falha ao obter participantes:", response.status, response.statusText);
            try {
                const errorBody = await response.json();
                console.error("Detalhes do erro (participantes):", errorBody);
            } catch (e) {
                // Ignorar
            }
            return [];
        }
        return await response.json();
    } catch (error) {
        console.error("Erro de rede ao obter participantes:", error);
        return [];
    }
}


async function apiCreateParticipant(participant) {
    try {
        const response = await fetch(`${BASE_URL}/Participantes`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                // 'RequestVerificationToken': token // Descomentar se usar CSRF token
            },
            body: JSON.stringify(participant)
        });
        if (!response.ok) {
            const error = await response.json().catch(() => ({ message: "Erro desconhecido ao criar participante." }));
            console.error("Erro ao criar participante:", response.status, error);
            return null;
        }
        return await response.json();
    } catch (error) {
        console.error("Erro de rede ao criar participante:", error);
        return null;
    }
}

async function apiDeleteParticipant(participantId) {
    try {
        const response = await fetch(`${BASE_URL}/Participantes/${participantId}`, {
            method: "DELETE",
            headers: {
                // 'RequestVerificationToken': token // Descomentar se usar CSRF token
            }
        });
        if (!response.ok) {
            const error = await response.json().catch(() => ({ message: "Erro desconhecido ao remover participante." }));
            console.error("Erro ao remover participante:", response.status, error);
            return false;
        }
        return true;
    } catch (error) {
        console.error("Erro de rede ao remover participante:", error);
        return false;
    }
}

// =======================================================
// FUNÇÕES DE INICIALIZAÇÃO
// =======================================================
async function initKanban() {
    // Carregar tasks do servidor
    tasks = await apiGetTasks();
    // Carregar participantes do servidor
    participants = await apiGetParticipants();
    
    renderParticipantsList(); // Renderiza a lista de participantes no modal de participantes
    renderTasks(); // Renderiza as tarefas no quadro Kanban
}

// Chamamos a init quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', initKanban);

// =======================================================
// FUNÇÕES DO KANBAN
// =======================================================

// 1) Renderização
function renderTasks() {
    const columns = {
        todo: document.getElementById("todo"),
        review: document.getElementById("review"),
        progress: document.getElementById("progress"),
        done: document.getElementById("done"),
    };
    
    // Limpa as colunas antes de renderizar novamente
    Object.values(columns).forEach(col => {
        if (col) col.innerHTML = ""; else console.warn("Uma coluna do Kanban não foi encontrada no DOM.");
    });
    
    const visibleTasks = getFilteredSortedTasks();
    
    visibleTasks.forEach(task => {
        if (!columns[task.column]) {
            console.warn(`Coluna "${task.column}" não encontrada para a tarefa "${task.title}". Pulando renderização.`);
            return;
        }
        
        const card = document.createElement("div");
        // Adiciona a classe de prioridade. Ex: "priority-alta", "priority-media", "priority-baixa"
        // Isso é usado pelo CSS para a borda lateral colorida.
        card.className = `task-card priority-${(task.priority || 'media').toLowerCase()}`;
        card.id = task.id;
        card.draggable = true;
        card.ondragstart = drag;
        
        let cardHTML = `<h6>${safeHTML(task.title)}</h6>`;
        if (task.description) {
            cardHTML += `<p>${safeHTML(task.description)}</p>`;
        }
        if (task.dueDate) {
            // Formatar a data para dd/mm/yyyy se desejar, ou manter o formato do backend
            let formattedDate = task.dueDate;
            try {
                const dateObj = new Date(task.dueDate);
                // Adiciona um dia para corrigir possível problema de fuso horário na exibição
                dateObj.setDate(dateObj.getDate() + 1);
                if (!isNaN(dateObj.getTime())) {
                    formattedDate = dateObj.toLocaleDateString('pt-BR');
                }
            } catch (e) {
                // Mantém a data original se houver erro na formatação
            }
            cardHTML += `<div class="due-date"><i class="bi bi-calendar"></i> ${safeHTML(formattedDate)}</div>`;
        }
        
        // Participantes
        cardHTML += `<div class="participants mt-2 mb-2">`; // Adicionado mb-2 para espaço antes do ícone
        if (task.participants && task.participants.length > 0) {
            task.participants.forEach(pid => {
                const foundParticipant = participants.find(p => p.id === pid);
                if (foundParticipant) {
                    cardHTML += `
                        <span class="badge participant-badge bg-dark">
                            <i class="bi bi-person-fill"></i> ${safeHTML(foundParticipant.name)}
                        </span>
                    `;
                }
            });
        }
        cardHTML += `</div>`;
        
        // Ícone de Edição (substitui o botão "Editar")
        // O CSS .task-edit-icon cuidará do posicionamento no canto superior direito.
        cardHTML += `<i class="bi bi-pencil-fill task-edit-icon" onclick="showModal('${task.id}')" title="Editar Tarefa"></i>`;
        
        card.innerHTML = cardHTML;
        columns[task.column].appendChild(card);
    });
    
    updateTaskCounts();
    
    // Mensagem para colunas vazias
    for (const columnId in columns) {
        const colElement = columns[columnId];
        if (colElement && colElement.children.length === 0) {
            const emptyDiv = document.createElement("div");
            emptyDiv.className = "empty-state text-center text-muted mt-4"; // Adicionadas classes de estilo
            emptyDiv.innerHTML = `
                <i class="bi bi-inbox fs-2"></i>
                <p class="mt-2"><em>Nenhuma tarefa aqui.</em></p>
            `;
            colElement.appendChild(emptyDiv);
        }
    }
}

function updateTaskCounts() {
    document.querySelectorAll(".kanban-column").forEach(columnElement => {
        const tasksContainer = columnElement.querySelector(".tasks");
        // Conta apenas os .task-card, ignorando .empty-state
        const count = tasksContainer ? tasksContainer.querySelectorAll(".task-card").length : 0;
        const countElement = columnElement.querySelector(".task-count");
        if (countElement) {
            countElement.textContent = count;
        }
    });
}

// 2) Filtro e Ordenação
function getFilteredSortedTasks() {
    let filtered = tasks.filter(t => matchesSearch(t, searchQuery));
    
    if (sortOption === "date") {
        filtered.sort((a, b) => {
            const dA = new Date(a.dueDate || "1970-01-01");
            const dB = new Date(b.dueDate || "1970-01-01");
            return dB - dA; // mais recentes primeiro
        });
    } else if (sortOption === "priority") {
        const priorityOrder = { alta: 1, media: 2, baixa: 3 };
        filtered.sort((a, b) => (priorityOrder[a.priority.toLowerCase()] || 3) - (priorityOrder[b.priority.toLowerCase()] || 3));
    }
    return filtered;
}

function matchesSearch(task, query) {
    if (!query) return true;
    const q = query.toLowerCase();
    const titleMatch = task.title?.toLowerCase().includes(q);
    const descMatch = task.description?.toLowerCase().includes(q);
    
    let participantMatch = false;
    if (task.participants && task.participants.length > 0) {
        for (const pid of task.participants) {
            const foundParticipant = participants.find(p => p.id === pid);
            if (foundParticipant && foundParticipant.name.toLowerCase().includes(q)) {
                participantMatch = true;
                break;
            }
        }
    }
    return (titleMatch || descMatch || participantMatch);
}

// 3) Drag & Drop
function allowDrop(ev) {
    ev.preventDefault();
}
function drag(ev) {
    // Certifica-se de que está pegando o ID do .task-card
    const taskCardElement = ev.target.closest(".task-card");
    if (taskCardElement) {
        ev.dataTransfer.setData("text", taskCardElement.id);
    } else {
        console.warn("Dragstart em elemento não esperado:", ev.target);
    }
}
async function drop(ev) {
    ev.preventDefault();
    const taskId = ev.dataTransfer.getData("text");
    const taskElement = document.getElementById(taskId);
    const targetColumnTasksContainer = ev.target.closest(".tasks"); // O drop deve ser na área .tasks
    
    if (taskElement && targetColumnTasksContainer) {
        const originalColumnId = taskElement.closest('.kanban-column').querySelector('.tasks').id;
        const newColumnId = targetColumnTasksContainer.id;
        
        // Adiciona visualmente à nova coluna
        targetColumnTasksContainer.appendChild(taskElement);
        updateTaskCounts(); // Atualiza contagem da coluna de origem e destino
        
        // Se a coluna mudou, atualiza no backend
        if (originalColumnId !== newColumnId) {
            await updateTaskPosition(taskId, newColumnId);
        }
    } else {
        console.warn("Drop não pôde ser completado. Elemento ou coluna alvo não encontrados.");
    }
}

async function updateTaskPosition(taskId, newColumnId) {
    const taskIndex = tasks.findIndex(t => t.id === taskId);
    if (taskIndex === -1) {
        console.error(`Tarefa com ID ${taskId} não encontrada localmente.`);
        return;
    }
    
    const taskToUpdate = { ...tasks[taskIndex], column: newColumnId };
    
    const result = await apiUpdateTask(taskToUpdate);
    if (!result) {
        console.warn("Falha ao atualizar posição da tarefa no servidor. Revertendo visualmente...");
        // Para reverter, precisaríamos recarregar as tarefas ou mover o elemento de volta.
        // Por simplicidade, vamos apenas recarregar tudo. Em um app mais complexo,
        // uma reversão mais granular seria melhor.
        await initKanban(); // Recarrega tudo para garantir consistência
        return;
    }
    
    // Atualiza a tarefa no array local com a resposta do servidor (que pode ter timestamps atualizados, etc.)
    tasks[taskIndex] = result;
    // Não precisa chamar renderTasks() aqui se a atualização visual já foi feita no drop e
    // a contagem foi atualizada. Se houver mais dados a serem atualizados no card, renderTasks() seria necessário.
    // Por enquanto, a mudança de coluna é a principal.
    renderTasks(); // Renderiza para garantir que tudo está correto, especialmente se houver ordenação.
}

// 4) CRUD a partir do Modal
function showModal(taskId = null, columnFromButton = null) {
    const modalElement = document.getElementById("taskModal");
    if (!modalElement) {
        console.error("Elemento do modal de tarefa não encontrado.");
        return;
    }
    const taskModal = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
    
    const currentTask = tasks.find(t => t.id === taskId);
    const taskData = currentTask || { // Valores padrão para nova tarefa
        id: "", // Será null ou string vazia para nova tarefa
        title: "",
        description: "",
        participants: [], // No modal, esperamos participantIds
        column: columnFromButton || "todo", // Coluna padrão se nova tarefa
        priority: "media",
        dueDate: ""
    };
    
    document.getElementById("taskId").value = taskData.id || ""; // taskId pode ser null
    document.getElementById("taskTitle").value = taskData.title;
    document.getElementById("taskDescription").value = taskData.description;
    document.getElementById("taskPriority").value = taskData.priority.toLowerCase();
    document.getElementById("taskDueDate").value = taskData.dueDate ? taskData.dueDate.split('T')[0] : ""; // Formato YYYY-MM-DD para input date
    document.getElementById("taskColumn").value = taskData.column;
    
    
    const deleteBtn = document.getElementById("deleteBtn");
    if (taskId && currentTask) { // Só mostra o botão de deletar se for uma tarefa existente
        deleteBtn.style.display = "inline-block"; // Ou 'block' dependendo do seu layout de botões
        deleteBtn.onclick = async () => {
            // TODO: Substituir confirm por um modal de confirmação customizado
            if (confirm("Tem certeza que deseja excluir esta tarefa?")) {
                await removeTaskInternal(taskId);
                taskModal.hide();
            }
        };
    } else {
        deleteBtn.style.display = "none";
    }
    
    loadParticipantsIntoSelect(taskData.participants || []); // taskData.participants deve ser um array de IDs
    taskModal.show();
}

function loadParticipantsIntoSelect(selectedParticipantIds = []) {
    const select = document.getElementById("taskParticipants");
    if (!select) return;
    
    select.innerHTML = ""; // Limpa opções anteriores
    if (participants.length === 0) {
        const option = document.createElement("option");
        option.disabled = true;
        option.textContent = "Nenhum participante cadastrado";
        select.appendChild(option);
        return;
    }
    participants.forEach(p => {
        const option = document.createElement("option");
        option.value = p.id;
        option.textContent = p.name + (p.email ? ` (${p.email})` : "");
        if (selectedParticipantIds.includes(p.id)) {
            option.selected = true;
        }
        select.appendChild(option);
    });
}

async function saveTask() {
    const taskId = document.getElementById("taskId").value; // Pode ser string vazia se nova tarefa
    const title = document.getElementById("taskTitle").value.trim();
    if (!title) {
        // TODO: Substituir alert por uma notificação/validação no modal
        alert("O título da tarefa é obrigatório.");
        document.getElementById("taskTitle").focus();
        return;
    }
    
    const selectedOptions = document.getElementById("taskParticipants").selectedOptions;
    const selectedParticipantIds = Array.from(selectedOptions).map(opt => opt.value);
    
    const taskDataToSave = {
        id: taskId || "00000000-0000-0000-0000-000000000000", // Backend espera um Guid para novo item
        title: title,
        description: document.getElementById("taskDescription").value.trim(),
        participantIds: selectedParticipantIds, // Alterado para participantIds para corresponder ao backend
        column: document.getElementById("taskColumn").value,
        priority: document.getElementById("taskPriority").value,
        dueDate: document.getElementById("taskDueDate").value || null // Envia null se vazio
    };
    
    let savedTask;
    if (!taskId) { // Criar nova tarefa
        savedTask = await apiCreateTask(taskDataToSave);
        if (savedTask) {
            tasks.push(savedTask); // Adiciona a nova tarefa (com ID do backend) ao array local
        }
    } else { // Atualizar tarefa existente
        // É importante enviar o ID correto para a API de atualização
        taskDataToSave.id = taskId;
        savedTask = await apiUpdateTask(taskDataToSave);
        if (savedTask) {
            const index = tasks.findIndex(t => t.id === taskId);
            if (index !== -1) {
                tasks[index] = savedTask; // Atualiza a tarefa no array local
            } else {
                tasks.push(savedTask); // Caso raro: tarefa não encontrada localmente mas atualizada
            }
        }
    }
    
    if (savedTask) {
        const taskModalElement = document.getElementById("taskModal");
        const taskModalInstance = bootstrap.Modal.getInstance(taskModalElement);
        if (taskModalInstance) {
            taskModalInstance.hide();
        }
        renderTasks(); // Re-renderiza todas as tarefas para refletir a mudança
    } else {
        console.error("Falha ao salvar task no servidor.");
        // TODO: Mostrar mensagem de erro para o usuário no modal
    }
}


async function removeTaskInternal(taskId) {
    const index = tasks.findIndex(t => t.id === taskId);
    if (index === -1) {
        console.error(`Tarefa com ID ${taskId} não encontrada para remoção.`);
        return;
    }
    
    const success = await apiDeleteTask(taskId);
    if (success) {
        tasks.splice(index, 1); // Remove do array local
        renderTasks(); // Re-renderiza
    } else {
        console.error(`Falha ao excluir tarefa ${taskId} no servidor.`);
        // TODO: Informar usuário sobre a falha
    }
}

// =======================================================
// PARTICIPANTES (Modal de Gerenciamento)
// =======================================================
function showParticipantsModal() {
    const modalElement = document.getElementById("participantsModal");
    if (!modalElement) return;
    const participantsModal = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
    document.getElementById("participantName").value = "";
    document.getElementById("participantEmail").value = "";
    renderParticipantsList(); // Atualiza a lista ao abrir o modal
    participantsModal.show();
}

async function addParticipant() {
    const nameField = document.getElementById("participantName");
    const emailField = document.getElementById("participantEmail");
    
    const nameVal = nameField.value.trim();
    const emailVal = emailField.value.trim();
    if (!nameVal) {
        // TODO: Substituir alert por validação no modal
        alert("Por favor, insira ao menos o nome do participante.");
        nameField.focus();
        return;
    }
    
    const newParticipantData = {
        id: "00000000-0000-0000-0000-000000000000", // Backend atribui o ID
        name: nameVal,
        email: emailVal
    };
    
    const result = await apiCreateParticipant(newParticipantData);
    if (result) {
        participants.push(result); // Adiciona o novo participante (com ID do backend)
        nameField.value = "";
        emailField.value = "";
        renderParticipantsList(); // Atualiza a lista no modal
        loadParticipantsIntoSelect(); // Atualiza o select no modal de tarefas, se estiver aberto ou for aberto depois
    } else {
        console.error("Falha ao criar participante.");
        // TODO: Mostrar erro no modal de participantes
    }
}

async function removeParticipant(id) {
    // TODO: Substituir confirm por um modal de confirmação customizado
    if (!confirm("Deseja realmente remover este participante? As tarefas associadas não serão alteradas automaticamente.")) return;
    
    const success = await apiDeleteParticipant(id);
    if (success) {
        const idx = participants.findIndex(p => p.id === id);
        if (idx !== -1) {
            participants.splice(idx, 1);
            renderParticipantsList(); // Atualiza a lista no modal de participantes
            loadParticipantsIntoSelect(); // Atualiza o select no modal de tarefas
            renderTasks(); // Re-renderiza tarefas, pois os nomes dos participantes podem ter sumido de alguns cards
        }
    } else {
        console.error(`Falha ao remover participante ${id}.`);
        // TODO: Mostrar erro no modal de participantes
    }
}

function renderParticipantsList() {
    const listContainer = document.getElementById("participantsList");
    if (!listContainer) return;
    
    listContainer.innerHTML = ""; // Limpa
    if (participants.length === 0) {
        listContainer.innerHTML = `
            <li class="list-group-item bg-dark text-white text-center p-3">
                Nenhum participante cadastrado.
            </li>
        `;
        return;
    }
    
    participants.forEach(part => {
        const li = document.createElement("li");
        li.className = "list-group-item d-flex justify-content-between align-items-center bg-dark text-white";
        li.innerHTML = `
            <div>
                <strong>${safeHTML(part.name)}</strong>
                ${part.email ? ` - <small class="text-muted">${safeHTML(part.email)}</small>` : ""}
            </div>
            <button class="btn btn-sm btn-outline-danger" onclick="removeParticipant('${part.id}')" title="Remover Participante">
                <i class="bi bi-trash"></i>
            </button>
        `;
        listContainer.appendChild(li);
    });
}

// =======================================================
// UTILITÁRIOS E EVENT HANDLERS GERAIS
// =======================================================
function safeHTML(str) {
    if (str === null || typeof str === 'undefined') return "";
    return String(str).replace(/[&<>"']/g, function (match) {
        switch (match) {
            case "&": return "&amp;";
            case "<": return "&lt;";
            case ">": return "&gt;";
            case '"': return "&quot;";
            case "'": return "&#x27;"; // Mais seguro que &#39;
        }
    });
}

// Handlers para filtro e ordenação
function onSearchInput() {
    const input = document.getElementById("searchInput");
    searchQuery = input.value.trim();
    renderTasks();
}

function onSortChange() {
    const select = document.getElementById("sortSelect");
    sortOption = select.value;
    renderTasks();
}

// Event listener para fechar modal de tarefa com ESC (já estava no seu código, mantido)
const taskModalElement = document.getElementById("taskModal");
if (taskModalElement) {
    taskModalElement.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            const instance = bootstrap.Modal.getInstance(taskModalElement);
            if (instance) instance.hide();
        }
    });
}

// Adiciona listeners aos inputs de busca e select de ordenação se existirem
const searchInputEl = document.getElementById("searchInput");
if (searchInputEl) searchInputEl.addEventListener('input', onSearchInput);

const sortSelectEl = document.getElementById("sortSelect");
if (sortSelectEl) sortSelectEl.addEventListener('change', onSortChange);

