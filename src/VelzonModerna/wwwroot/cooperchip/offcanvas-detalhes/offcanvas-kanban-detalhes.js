// wwwroot/js/offcanvas-kanban-detalhes.js
// (Conteúdo do seu script original, que parece correto para gerar o HTML que estamos estilizando)

document.addEventListener('DOMContentLoaded', function () {
    const offcanvasUser = document.getElementById('offcanvasUser');
    if (!offcanvasUser) {
        return;
    }

    const offcanvasTabData = {
        atividade: `
            <div class="task-progress">
                <div class="mb-2">Trending contents <span class="float-end text-secondary" style="font-size:0.93em;">Últimas 24 horas</span></div>
                <ul class="list-unstyled activity-list mb-3">
                    <li><span class="badge badge-assignment">Assignment</span>Mobile & Desktop Screen Pattern
                        <span class="float-end text-secondary">60% <span class="ms-1">(5h)</span></span>
                        <div class="progress mt-1" style="height:6px;">
                            <div class="progress-bar" role="progressbar" style="width: 60%"></div>
                        </div>
                    </li>
                    <li><span class="badge badge-quiz">Quiz</span>Creating Engaging Learning Journeys
                        <span class="float-end text-secondary">30% <span class="ms-1">(2h)</span></span>
                        <div class="progress mt-1" style="height:6px;">
                            <div class="progress-bar" role="progressbar" style="width: 30%"></div>
                        </div>
                    </li>
                    <li><span class="badge bg-secondary">Other</span>Other task
                        <span class="float-end text-secondary">10% <span class="ms-1">(30min)</span></span>
                        <div class="progress mt-1" style="height:6px;">
                            <div class="progress-bar bg-info" role="progressbar" style="width: 10%"></div>
                        </div>
                    </li>
                </ul>
            </div>
            <div class="mb-4">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <div class="fw-semibold">Log</div>
                    <div>
                        <input type="text" class="form-control form-control-sm search-box d-inline-block" placeholder="Buscar..." style="width:120px;">
                    </div>
                </div>
                <div class="timeline">
                    <div class="timeline-item">
                        <div class="timeline-time">12 Mar 2023 · 10:30am</div>
                        <div>Started a Course <b>A Designer's Toolkit for Crafting Exceptional Learning Management</b></div>
                    </div>
                    <div class="timeline-item">
                        <div class="timeline-time">10 Mar 2023 · 10:30am</div>
                        <div>Completed the Quiz <b>Creating Engaging Learning Journeys</b> with <span style="color:#ffe066;">★ 95 points</span></div>
                    </div>
                    <div class="timeline-item">
                        <div class="timeline-time">09 Mar 2023 · 11:30am</div>
                        <div>Uploaded Attachment in Assignment <b>Style Direction</b></div>
                    </div>
                </div>
            </div>
            <div class="d-flex justify-content-end">
                <button class="btn btn-outline-info btn-sm">Ver mais</button>
            </div>
        `,
        atribuido: `
            <div class="mb-4">
                <div class="fw-bold mb-2">Tarefas atribuídas</div>
                <ul class="list-group list-group-flush">
                    <li class="list-group-item">
                        <span class="badge bg-info me-2">UI</span>
                        Revisar protótipo mobile <span class="float-end text-secondary">Pendente</span>
                    </li>
                    <li class="list-group-item">
                        <span class="badge bg-warning text-dark me-2">QA</span>
                        Testar fluxo de cadastro <span class="float-end text-success">Em andamento</span>
                    </li>
                    <li class="list-group-item">
                        <span class="badge bg-success me-2">UX</span>
                        Pesquisar jornadas do usuário <span class="float-end text-muted">Concluído</span>
                    </li>
                </ul>
            </div>
            <div class="text-center text-secondary mt-5">
                <em>Nenhuma outra tarefa atribuída no momento.</em>
            </div>
        `,
        revisar: `
            <div class="mb-3">
                <div class="fw-bold mb-2">Itens que precisam de revisão</div>
                <ul class="list-group list-group-flush">
                    <li class="list-group-item">
                        <span class="badge bg-danger me-2">Urgente</span>
                        Revisar feedback do cliente <span class="float-end text-danger">Até amanhã</span>
                    </li>
                    <li class="list-group-item">
                        <span class="badge bg-primary me-2">Doc</span>
                        Atualizar documentação <span class="float-end text-secondary">Em aberto</span>
                    </li>
                </ul>
            </div>
            <div class="alert alert-warning bg-transparent border-warning text-warning mt-4" role="alert">
                <b>Atenção:</b> Existem 2 itens aguardando revisão!
            </div>
        `
    };

    function renderOffcanvasTab(tabName) {
        const contentId = `${tabName}-content`;
        const tabContentElement = document.getElementById(contentId);

        if (tabContentElement) {
            tabContentElement.innerHTML = `<div class='tab-loader'><div class='spinner-border text-info' role='status'><span class='visually-hidden'>Loading...</span></div></div>`;
            setTimeout(() => {
                if (offcanvasTabData[tabName]) {
                    tabContentElement.innerHTML = offcanvasTabData[tabName];
                } else {
                    tabContentElement.innerHTML = "<div class='empty-state'>Conteúdo não encontrado.</div>";
                }
            }, 320);
        }
    }

    const activeTabInitially = offcanvasUser.querySelector('.nav-link.active');
    if (activeTabInitially) {
        const initialTabId = activeTabInitially.getAttribute('data-bs-target').replace('#', '');
        renderOffcanvasTab(initialTabId);
    }

    const tabButtons = offcanvasUser.querySelectorAll('.nav-link[data-bs-toggle="tab"]');
    tabButtons.forEach(btn => {
        btn.addEventListener('show.bs.tab', function (event) {
            let tabId = event.target.getAttribute('data-bs-target').replace('#', '');
            renderOffcanvasTab(tabId);
        });
    });
});