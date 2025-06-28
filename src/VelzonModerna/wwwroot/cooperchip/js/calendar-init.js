/*
Template Name: Velzon - Admin & Dashboard Template
Author: Themesbrand
Website: https://Themesbrand.com/
Contact: Themesbrand@gmail.com
File: Calendar init js (TOM System - Simplificado: Direto para Edição)
*/

// --- Seletores Globais ---
const modalElement = document.getElementById('event-modal');
const calendarElement = document.getElementById('calendar');
const formEvent = document.getElementById('form-event'); // O elemento <form>
const eventDetailsSection = document.querySelector('.event-details'); // A div de detalhes (será sempre oculta na edição)
const modalTitle = document.getElementById('modal-title');
const btnNewEvent = document.getElementById('btn-new-event');
const btnDeleteEvent = document.getElementById('btn-delete-event'); // Botão Deletar (dentro do form)
const btnSaveEvent = document.getElementById('btn-save-event');   // Botão Salvar/Atualizar (dentro do form)
// const editEventBtn = document.getElementById('edit-event-btn'); // Não será mais usado neste fluxo
const externalEventContainerEl = document.getElementById('external-events');
const upcomingEventList = document.getElementById('upcoming-event-list');

// --- Inputs do Formulário ---
const eventIdInput = document.getElementById("eventid");
const eventTitleInput = document.getElementById("event-title");
const eventCategorySelect = document.getElementById("event-category");
const eventStartDateInput = document.getElementById("event-start-date");
const timepicker1Input = document.getElementById("timepicker1");
const timepicker2Input = document.getElementById("timepicker2");
const eventLocationInput = document.getElementById("event-location");
const eventDescriptionInput = document.getElementById("event-description");
const eventTimeSection = document.getElementById("event-time");

// --- Spans de Detalhes (Preenchidos mas não exibidos na edição) ---
const eventStartDateTag = document.getElementById("event-start-date-tag");
const eventTimepicker1Tag = document.getElementById("event-timepicker1-tag");
const eventTimepicker2Tag = document.getElementById("event-timepicker2-tag");
const eventLocationTag = document.getElementById("event-location-tag");
const eventDescriptionTag = document.getElementById("event-description-tag");

// --- Instâncias e Estado ---
let addEventModalInstance = null;
let calendarInstance = null;
let eventCategoryChoice = null;
let startDateFlatpickr = null;
let timepicker1Flatpickr = null;
let timepicker2Flatpickr = null;
let selectedEvent = null;
let currentEventData = null;

// --- Inicialização Principal ---
document.addEventListener("DOMContentLoaded", function () {
    if (!modalElement || !calendarElement) { console.error("Elementos calendário não encontrados."); return; }
    addEventModalInstance = new bootstrap.Modal(modalElement, { keyboard: false });
    initializeFlatpickr();
    if (eventCategorySelect) { eventCategoryChoice = new Choices(eventCategorySelect, { searchEnabled: false, allowHTML: false }); }
    if (externalEventContainerEl) { /* ... inicializa Draggable ... */
        new FullCalendar.Draggable(externalEventContainerEl, { itemSelector: '.external-event', eventData: (el) => ({ title: el.innerText.trim() || 'Novo Evento', classNames: [CalendarEventDtoClient.mapCategoryToClassName(el.dataset.class)], extendedProps: { category: el.dataset.class } }) });
    }
    initializeFullCalendar();
    loadUpcomingEvents();
    setupEventListeners(); // Configura listeners para os botões que ainda existem (Save, Delete, New)
    modalElement.addEventListener('hidden.bs.modal', () => resetFormAndModal()); // Limpa ao fechar
});

// --- Funções de Inicialização ---
function initializeFlatpickr() { /* ... (código sem mudanças) ... */
    const commonConfig = { altInput: !0, altFormat: "j F Y", dateFormat: "Y-m-d", locale: "pt" }, timeConfig = { enableTime: !0, noCalendar: !0, dateFormat: "H:i", time_24hr: !0, locale: "pt" }; eventStartDateInput && (startDateFlatpickr = flatpickr(eventStartDateInput, { ...commonConfig, mode: "range", onChange: (e) => toggleTimeFields(e.length <= 1) })), timepicker1Input && (timepicker1Flatpickr = flatpickr(timepicker1Input, timeConfig)), timepicker2Input && (timepicker2Flatpickr = flatpickr(timepicker2Input, timeConfig));
}
function initializeFullCalendar() { /* ... (código sem mudanças) ... */
    if (!calendarElement) return; const e = { timeZone: "local", editable: !0, droppable: !0, selectable: !0, navLinks: !0, initialView: getInitialView(), themeSystem: "bootstrap5", headerToolbar: { left: "prev,next today", center: "title", right: "dayGridMonth,timeGridWeek,timeGridDay,listMonth" }, buttonText: { today: "Hoje", month: "Mês", week: "Semana", day: "Dia", list: "Lista" }, locale: "pt-br", weekNumbers: !1, dayMaxEvents: !0, events: { url: "/Calendar/GetEvents", method: "GET", failure: (e) => { console.error("Erro buscar eventos:", e), showNotification("Falha carregar eventos.", "error") } }, windowResize: (e) => { calendarInstance.changeView(getInitialView()) }, dateClick: handleDateClick, select: handleDateSelect, eventClick: handleEventClick, eventDrop: (e) => { handleEventUpdate(e.event, e.oldEvent, e.revert) }, eventResize: (e) => { handleEventUpdate(e.event, e.oldEvent, e.revert) }, eventReceive: handleExternalDrop }; calendarInstance = new FullCalendar.Calendar(calendarElement, e), calendarInstance.render();
}
function setupEventListeners() {
    // Configura apenas os listeners necessários no fluxo simplificado
    if (btnNewEvent) btnNewEvent.addEventListener("click", handleNewEventButtonClick);
    // if (editEventBtn) editEventBtn.addEventListener("click", handleEditButtonClick); // Removido
    if (formEvent) formEvent.addEventListener("submit", handleFormSubmit);
    if (btnDeleteEvent) btnDeleteEvent.addEventListener("click", handleDeleteButtonClick);
}

// --- Handlers de Interação FullCalendar ---
function handleDateClick(info) { prepareModalForCreate(info.dateStr, info.allDay); }
function handleDateSelect(info) { prepareModalForCreate(info.startStr, info.allDay, info.endStr); }

function handleEventClick(info) {
    info.jsEvent.preventDefault();
    console.log('--- handleEventClick Triggered (Direct Edit Flow) ---'); console.log('Event ID:', info.event.id);
    selectedEvent = info.event;
    if (!selectedEvent || !selectedEvent.id) { console.error("ID evento ausente!"); showNotification("Erro: ID evento ausente.", "error"); return; }
    loadEventDetailsAndShowModal(selectedEvent.id); // Carrega e vai direto para edição
}
function handleEventUpdate(event, oldEvent, revertFunc) { /* ... (código sem mudanças) ... */
    console.log("Event updated:", event); const t = { id: event.id, start: event.start.toISOString(), end: event.end ? event.end.toISOString() : null, allDay: event.allDay }; console.log("Sending update:", t), updateEventTimeOnServer(t, revertFunc);
}
function handleExternalDrop(info) { /* ... (código sem mudanças) ... */
    console.log("External event received:", info), prepareModalForCreateFromDrop(info.date, info.draggedEl.dataset.class, info.draggedEl.innerText), info.remove();
}

// --- Lógica do Modal e Formulário (Fluxo Simplificado) ---

function prepareModalForCreate(startDateStr, isAllDay, endDateStr = null) {
    resetFormAndModal();
    modalTitle.innerText = 'Adicionar Novo Evento';
    showEditFormDirectly(); // Configura a visibilidade do formulário

    // Ajusta botões para o modo CRIAR
    if (btnSaveEvent) btnSaveEvent.innerText = 'Adicionar Evento';
    if (btnDeleteEvent) btnDeleteEvent.style.display = 'none';   // Oculta delete ao criar

    const initialDate = endDateStr ? [startDateStr, endDateStr] : startDateStr;
    if (startDateFlatpickr) startDateFlatpickr.setDate(initialDate, true);
    toggleTimeFields(isAllDay || endDateStr ? false : true);
    if (!(isAllDay || endDateStr)) {
        const timePart = startDateStr.includes('T') ? startDateStr.split('T')[1].substring(0, 5) : null;
        if (timePart && timepicker1Flatpickr) timepicker1Flatpickr.setDate(timePart);
    }
    addEventModalInstance.show();
}

function prepareModalForCreateFromDrop(date, category, title) {
    resetFormAndModal();
    modalTitle.innerText = 'Adicionar Novo Evento';
    showEditFormDirectly(); // Configura a visibilidade do formulário

    // Ajusta botões para o modo CRIAR
    if (btnSaveEvent) btnSaveEvent.innerText = 'Adicionar Evento';
    if (btnDeleteEvent) btnDeleteEvent.style.display = 'none';

    if (eventTitleInput) eventTitleInput.value = title.trim();
    if (eventCategoryChoice) eventCategoryChoice.setChoiceByValue(category);
    if (startDateFlatpickr) startDateFlatpickr.setDate(date, true);
    toggleTimeFields(true); // Assume hora inicialmente
    addEventModalInstance.show();
}

function loadEventDetailsAndShowModal(eventId) { // Vai direto para edição
    console.log(`loadEventDetailsAndShowModal (Direct Edit) called for ID: ${eventId}`);
    showLoadingInModal();
    fetch(`/Calendar/GetEventDetails/${eventId}`)
        .then(response => {
            if (!response.ok) { if (response.status === 404) throw new Error(`Evento ${eventId} não encontrado.`); return response.text().then(text => { throw new Error(`HTTP error ${response.status}: ${text}`); }); }
            console.log("Response OK, parsing JSON...");
            return response.json();
        })
        .then(eventData => {
            console.log("Parsed data:", eventData);
            currentEventData = eventData;
            if (!currentEventData || !currentEventData.id) { throw new Error("Dados evento inválidos."); }
            console.log("Data valid, populating form for direct edit...");

            hideLoadingInModal();
            populateModalWithData(currentEventData); // << Popula os INPUTS do form
            showEditFormDirectly();               // << CONFIGURA visibilidade para edição direta
            addEventModalInstance.show();         // MOSTRA O MODAL (com form visível)
        })
        .catch(error => {
            console.error('Erro carregar detalhes:', error);
            hideLoadingInModal();
            showNotification(error.message || 'Erro carregar detalhes.', 'error');
            addEventModalInstance.hide();
        });
}

// Preenche os campos (principalmente inputs do form)
function populateModalWithData(data) {
    console.log("--- Entering populateModalWithData ---", data);
    if (!data) { console.error("populateModal null data!"); return; }

    // Define o título do modal
    if (modalTitle) modalTitle.innerText = data.title || 'Editar Evento'; // Título adequado

    // Preenche Inputs do Formulário
    if (eventIdInput) eventIdInput.value = data.id;
    if (eventTitleInput) eventTitleInput.value = data.title;
    if (eventLocationInput) eventLocationInput.value = data.extendedProps?.location || '';
    if (eventDescriptionInput) eventDescriptionInput.value = data.extendedProps?.description || '';
    if (eventCategoryChoice && data.extendedProps?.category) { try { eventCategoryChoice.setChoiceByValue(data.extendedProps.category); } catch (e) { console.error("Erro set category:", e); } }
    try { /* ... Lógica Flatpickr (sem mudanças) ... */
        const startDate = data.allDay ? data.start : data.start.split('T')[0]; let endDate = null; if (data.end) { endDate = data.allDay ? data.end : data.end.split('T')[0]; if (data.allDay) { const tempEnd = new Date(endDate); tempEnd.setDate(tempEnd.getDate() - 1); endDate = tempEnd.toISOString().split('T')[0]; } } const dateValue = endDate && endDate !== startDate ? [startDate, endDate] : startDate; if (startDateFlatpickr) { startDateFlatpickr.setDate(dateValue, false); } if (!data.allDay) { const startTime = data.start.includes('T') ? data.start.split('T')[1].substring(0, 5) : null; const endTime = data.end?.includes('T') ? data.end.split('T')[1].substring(0, 5) : null; if (timepicker1Flatpickr && startTime) timepicker1Flatpickr.setDate(startTime); else if (timepicker1Flatpickr) timepicker1Flatpickr.clear(); if (timepicker2Flatpickr && endTime) timepicker2Flatpickr.setDate(endTime); else if (timepicker2Flatpickr) timepicker2Flatpickr.clear(); } toggleTimeFields(!data.allDay);
    } catch (e) { console.error("Erro set flatpickr:", e); }

    console.log("--- Exiting populateModalWithData ---");
}

// Limpa modal e reseta estado
function resetFormAndModal() {
    if (formEvent) { formEvent.reset(); formEvent.classList.remove('was-validated'); }
    if (eventIdInput) eventIdInput.value = "";
    if (startDateFlatpickr) startDateFlatpickr.clear();
    if (timepicker1Flatpickr) timepicker1Flatpickr.clear();
    if (timepicker2Flatpickr) timepicker2Flatpickr.clear();
    toggleTimeFields(true);
    if (eventCategoryChoice) eventCategoryChoice.destroy();
    if (eventCategorySelect) { eventCategoryChoice = new Choices(eventCategorySelect, { searchEnabled: false, allowHTML: false }); }
    selectedEvent = null; currentEventData = null; modalTitle.innerText = 'Evento';
    // Garante que tudo está oculto ao resetar
    eventDetailsSection?.classList.add("d-none");
    formEvent?.classList.add("d-none");
    // if(editEventBtn) editEventBtn.style.display = 'none'; // Botão removido do fluxo
    if (btnSaveEvent) btnSaveEvent.style.display = 'none';
    if (btnDeleteEvent) btnDeleteEvent.style.display = 'none';
}

// *** NOVA FUNÇÃO ***: Configura visibilidade para EDIÇÃO DIRETA
function showEditFormDirectly() {
    console.log("--- Setting up direct Edit Mode ---");
    formEvent?.classList.remove("d-none");              // MOSTRA Formulário
    eventDetailsSection?.classList.add("d-none");       // ESCONDE Detalhes (se existir)

    // Configura botões para edição de evento existente
    if (btnSaveEvent) {
        btnSaveEvent.style.display = 'inline-block';    // MOSTRA Botão Atualizar
        btnSaveEvent.innerText = 'Atualizar Evento';
    }
    if (btnDeleteEvent) {
        btnDeleteEvent.style.display = 'inline-block'; // MOSTRA Botão Deletar
    }
    // O botão Edit/Cancel (#edit-event-btn) não é mais necessário neste fluxo
    // if(editEventBtn) {
    //      editEventBtn.style.display = 'none';
    // }

    // Garante Choices inicializado
    if (!eventCategoryChoice && eventCategorySelect) {
        eventCategoryChoice = new Choices(eventCategorySelect, { searchEnabled: false, allowHTML: false });
    }
    console.log("--- Direct Edit Mode setup complete ---");
}

// Funções switchToViewMode e switchToEditMode (versões antigas) podem ser removidas ou comentadas
/*
function switchToViewMode() { ... }
function switchToEditMode() { ... }
*/

function toggleTimeFields(show) { if (eventTimeSection) { eventTimeSection.style.display = show ? 'block' : 'none'; } }

// --- Handlers de Botões e Formulário ---
function handleNewEventButtonClick(e) { e.preventDefault(); const today = new Date().toISOString().split('T')[0]; prepareModalForCreate(today, false); }
// function handleEditButtonClick(e) { ... } // Removido/Comentado

function handleFormSubmit(e) { /* ... (código sem mudanças) ... */
    e.preventDefault(); formEvent.classList.add("was-validated"); if (!formEvent.checkValidity()) { showNotification("Corrija os erros.", "warning"); return } const t = { id: eventIdInput.value || null, title: eventTitleInput.value, category: eventCategorySelect.value, eventDate: eventStartDateInput.value, startTime: timepicker1Input.value || null, endTime: timepicker2Input.value || null, location: eventLocationInput.value || null, description: eventDescriptionInput.value || null }; console.log("Submitting:", t), saveEventOnServer(t);
}
function handleDeleteButtonClick(e) { /* ... (código sem mudanças) ... */
    e.preventDefault(); const t = eventIdInput.value; if (!t) { showNotification("Nenhum evento selecionado.", "warning"); return } showConfirmationDialog("Deletar este evento?", () => { deleteEventOnServer(t) });
}

// --- Chamadas AJAX ---
// (Funções getAntiForgeryToken, saveEventOnServer, deleteEventOnServer, updateEventTimeOnServer, loadUpcomingEvents sem mudanças)
function getAntiForgeryToken() { const e = document.getElementById("RequestVerificationToken"); return e ? e.value : null }
function saveEventOnServer(eventData) { const t = getAntiForgeryToken(); if (!t) { showNotification("Erro segurança.", "error"); return } showLoadingButton(btnSaveEvent); const n = !!eventData.id; fetch("/Calendar/Save", { method: "POST", headers: { "Content-Type": "application/json", RequestVerificationToken: t }, body: JSON.stringify(eventData) }).then(e => (hideLoadingButton(btnSaveEvent, n ? "Atualizar Evento" : "Adicionar Evento"), e.ok ? e.json() : e.json().then(e => { throw e }))).then(e => { e.success ? (showNotification(`Evento ${n ? "atualizado" : "criado"}!`, "success"), addEventModalInstance.hide(), calendarInstance.refetchEvents(), loadUpcomingEvents()) : (() => { throw new Error(e.message || "Falha salvar.") })() }).catch(e => { hideLoadingButton(btnSaveEvent, n ? "Atualizar Evento" : "Adicionar Evento"), console.error("Erro salvar:", e), handleSaveError(e) }); }
function deleteEventOnServer(eventId) { const t = getAntiForgeryToken(); if (!t) { showNotification("Erro segurança.", "error"); return } showLoadingButton(btnDeleteEvent); fetch(`/Calendar/Delete/${eventId}`, { method: "POST", headers: { RequestVerificationToken: t } }).then(e => (hideLoadingButton(btnDeleteEvent, "Deletar"), e.ok ? e.json() : e.json().then(e => { throw e }))).then(e => { e.success ? (showNotification("Evento deletado!", "success"), addEventModalInstance.hide(), calendarInstance.refetchEvents(), loadUpcomingEvents()) : (() => { throw new Error(e.message || "Falha deletar.") })() }).catch(e => { hideLoadingButton(btnDeleteEvent, "Deletar"), console.error("Erro deletar:", e), showNotification(e.message || "Erro deletar.", "error") }); }
function updateEventTimeOnServer(eventData, revertFunc) { const t = getAntiForgeryToken(); if (!t) { showNotification("Erro segurança.", "error"); revertFunc && revertFunc(); return } fetch("/Calendar/UpdateEventTime", { method: "POST", headers: { "Content-Type": "application/json", RequestVerificationToken: t }, body: JSON.stringify(eventData) }).then(e => e.ok ? e.json() : (revertFunc && revertFunc(), e.json().then(e => { throw e }))).then(e => { e.success ? (showNotification("Evento atualizado!", "success"), loadUpcomingEvents()) : (revertFunc && revertFunc(), (() => { throw new Error(e.message || "Falha atualizar.") })()) }).catch(e => { console.error("Erro atualizar tempo:", e), showNotification(e.message || "Erro atualizar.", "error"), revertFunc && revertFunc() }); }
function loadUpcomingEvents() { if (!upcomingEventList) return; upcomingEventList.innerHTML = '<div class="text-center p-3"><div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Carregando...</span></div></div>'; fetch("/Calendar/GetUpcomingEvents?count=5").then(e => { if (!e.ok) throw new Error(`HTTP error! status: ${e.status}`); return e.text() }).then(e => { upcomingEventList.innerHTML = e, typeof SimpleBar != "undefined" && upcomingEventList.closest("[data-simplebar]") && new SimpleBar(upcomingEventList.closest("[data-simplebar]")) }).catch(e => { console.error("Erro próximos eventos:", e), upcomingEventList.innerHTML = '<p class="text-danger p-3">Erro carregar.</p>' }); }

// --- Funções Utilitárias e Feedback ---
function getInitialView() { return window.innerWidth >= 768 && window.innerWidth < 1200 ? "timeGridWeek" : window.innerWidth <= 768 ? "listMonth" : "dayGridMonth"; }
function showLoadingInModal() { const e = modalElement.querySelector(".modal-body"); e && !e.querySelector(".modal-loading-overlay") && e.insertAdjacentHTML("afterbegin", '<div class="modal-loading-overlay" style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255,255,255,0.8); z-index: 10; display: flex; align-items: center; justify-content: center;"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Carregando...</span></div></div>'); }
function hideLoadingInModal() { const e = modalElement.querySelector(".modal-loading-overlay"); e && e.remove(); }
function showLoadingButton(buttonElement, loadingText = "Salvando...") { buttonElement && (buttonElement.disabled = !0, buttonElement.dataset.originalText = buttonElement.innerHTML, buttonElement.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> ${loadingText}`); }
function hideLoadingButton(buttonElement, originalText = null) { buttonElement && (buttonElement.disabled = !1, buttonElement.innerHTML = originalText || buttonElement.dataset.originalText || "Salvar"); }
function showNotification(message, type = 'info') { console.log(`[${type.toUpperCase()}] ${message}`); /* TODO: Implementar Toastr/SweetAlert */ }
function showConfirmationDialog(message, confirmCallback) { if (confirm(message)) { confirmCallback && confirmCallback(); } /* TODO: Implementar SweetAlert */ }
function handleSaveError(error) { let t = "Ocorreu um erro inesperado."; error && "object" == typeof error && !Array.isArray(error) ? error.errors ? (t = "Erros:\n", Object.keys(error.errors).forEach(n => { error.errors.hasOwnProperty(n) && (t += `- ${error.errors[n].join(", ")}\n`) })) : error.message ? t = error.message : error.title && (t = error.title) : "string" == typeof error && (t = error), showNotification(t, "error"); }
function formatDateRangeForDisplay(startIso, endIso, isAllDay) { const t = new Date(startIso), n = endIso ? new Date(endIso) : null, o = { day: "numeric", month: "short", year: "numeric" }; if (isAllDay) { const e = t.toLocaleDateString("pt-BR", o); if (n) { const s = new Date(n); s.setDate(s.getDate() - 1); const a = s.toLocaleDateString("pt-BR", o); return e === a ? e : `${e} a ${a}` } return e } return t.toLocaleDateString("pt-BR", o); }
function formatTimeForDisplay(isoDateTime) { if (!isoDateTime) return ""; const t = new Date(isoDateTime); return t.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit", hour12: !1 }); }
const CalendarEventDtoClient = { mapCategoryToClassName: function (e) { const t = e?.toLowerCase(); switch (t) { case "success": return "bg-success-subtle text-success"; case "danger": return "bg-danger-subtle text-danger"; case "warning": return "bg-warning-subtle text-warning"; case "info": return "bg-info-subtle text-info"; case "primary": return "bg-primary-subtle text-primary"; case "dark": return "bg-dark-subtle text-dark"; default: return "bg-primary-subtle text-primary" } } };