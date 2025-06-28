using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.Controllers.Base;
using VelzonModerna.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Interfaces.CRM;
using VelzonModerna.ViewModels.CRM;

namespace VelzonModerna.Controllers
{
    [Route("Crm")]
    public class CrmController : BaseMvcController
    {
        private readonly IMapper _mapper;
        // Injetando todos os serviços e repositórios necessários para o módulo
        private readonly ILeadRepository _leadRepository;
        private readonly ILeadDomainService _leadDomainService;
        private readonly ICrmTaskRepository _taskRepository;
        private readonly ICrmTaskDomainService _taskDomainService;
        private readonly IActivityRepository _activityRepository;
        private readonly IGenericRepository<LeadNote, Guid> _noteRepository;
        private readonly IGenericRepository<Log, Guid> _logRepository;

        private readonly IGenericRepository<CrmTaskComment, Guid> _commentRepository;
        private readonly IGenericRepository<CrmTaskAttachment, Guid> _attachmentRepository;

        public CrmController(
            INotificador notificador,
            IMapper mapper,
            ILeadRepository leadRepository,
            ILeadDomainService leadDomainService,
            ICrmTaskRepository taskRepository,
            ICrmTaskDomainService taskDomainService,
            IActivityRepository activityRepository,
            IGenericRepository<LeadNote, Guid> noteRepository,
            IGenericRepository<Log, Guid> logRepository,
            IGenericRepository<CrmTaskComment, Guid> commentRepository,
            IGenericRepository<CrmTaskAttachment, Guid> attachmentRepository)
            : base(notificador)
        {
            _mapper = mapper;
            _leadRepository = leadRepository;
            _leadDomainService = leadDomainService;
            _taskRepository = taskRepository;
            _taskDomainService = taskDomainService;
            _activityRepository = activityRepository;
            _noteRepository = noteRepository;
            _logRepository = logRepository;
            _commentRepository = commentRepository;
            _attachmentRepository = attachmentRepository;
        }

        // Action principal que serve a "casca" da nossa Single Page Application
        [HttpGet]
        public IActionResult Index() => View();

        #region API Endpoints para Leads

        [HttpGet("api/leads")]
        public async Task<IActionResult> GetLeads()
        {
            var leads = await _leadRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<LeadViewModel>>(leads));
        }

        [HttpPost("api/leads")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateLead([FromBody] LeadViewModel leadViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var lead = _mapper.Map<Lead>(leadViewModel);
            await _leadDomainService.AddLeadAsync(lead);
            if (!OperacaoValida())
                return BadRequest(new { errors = "Erro ao tentar adicionar Lead" });
            if (!await _leadRepository.UnitOfWork.CommitAsync())
                return BadRequest(new { errors = "Ocorreu um erro ao salvar o Lead." });
            return CreatedAtAction(nameof(GetLeads), new { id = lead.Id }, _mapper.Map<LeadViewModel>(lead));
        }

        [HttpPut("api/leads/{id:guid}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateLead(Guid id, [FromBody] LeadViewModel leadViewModel)
        {
            if (id != leadViewModel.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var lead = await _leadRepository.GetByIdAsync(id);
            if (lead == null)
                return NotFound();
            _mapper.Map(leadViewModel, lead);
            await _leadDomainService.UpdateLeadAsync(lead);
            if (!OperacaoValida())
                return BadRequest(new { errors = "Erro ao tentar atualizar Lead" });
            if (!await _leadRepository.UnitOfWork.CommitAsync())
                return BadRequest(new { errors = "Ocorreu um erro ao atualizar o Lead." });
            return Ok(_mapper.Map<LeadViewModel>(lead));
        }

        [HttpDelete("api/leads/{id:guid}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteLead(Guid id)
        {
            await _leadDomainService.DeleteLeadAsync(id);
            if (!OperacaoValida())
                return BadRequest(new { errors = "Erro ao tentar exluir Lead" });
            if (!await _leadRepository.UnitOfWork.CommitAsync())
                return BadRequest(new { errors = "Ocorreu um erro ao excluir o Lead." });
            return NoContent();
        }

        #endregion

        #region API Endpoints para CrmTasks

        [HttpGet("api/tasks")]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CrmTaskViewModel>>(tasks));
        }

        [HttpPost("api/tasks")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateTask([FromBody] CrmTaskViewModel taskViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var task = _mapper.Map<CrmTask>(taskViewModel);
            await _taskDomainService.AddTaskAsync(task);
            if (!OperacaoValida())
                return BadRequest(new { errors = "Erro ao tentar adicionar Tarefa" });
            if (!await _taskRepository.UnitOfWork.CommitAsync())
                return BadRequest(new { errors = "Ocorreu um erro ao salvar a Tarefa." });
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, _mapper.Map<CrmTaskViewModel>(task));
        }

        [HttpPut("api/tasks/{id:guid}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] CrmTaskViewModel taskViewModel)
        {
            if (id != taskViewModel.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return NotFound();
            _mapper.Map(taskViewModel, task);
            await _taskDomainService.UpdateTaskAsync(task);
            if (!OperacaoValida())
                return BadRequest(new { errors = "Erro ao tentar atualizar Tarefa" });
            if (!await _taskRepository.UnitOfWork.CommitAsync())
                return BadRequest(new { errors = "Ocorreu um erro ao atualizar a Tarefa." });
            return Ok(_mapper.Map<CrmTaskViewModel>(task));
        }

        [HttpDelete("api/tasks/{id:guid}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await _taskDomainService.DeleteTaskAsync(id);
            if (!OperacaoValida())
                return BadRequest(new { errors = "Erro ao tentar exluir Tarefa" });
            if (!await _taskRepository.UnitOfWork.CommitAsync())
                return BadRequest(new { errors = "Ocorreu um erro ao excluir a Tarefa." });
            return NoContent();
        }

        #endregion

        #region API Endpoints para Activities

        [HttpGet("api/activities")]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await _activityRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ActivityViewModel>>(activities));
        }

        [HttpPost("api/activities")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityViewModel activityViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var activity = _mapper.Map<Activity>(activityViewModel);
            await _activityRepository.AddAsync(activity); // Usando repositório genérico por simplicidade
            if (!await _activityRepository.UnitOfWork.CommitAsync())
                return BadRequest(new { errors = "Ocorreu um erro ao salvar a Atividade." });
            return Ok(_mapper.Map<ActivityViewModel>(activity));
        }

        #endregion

        #region API Endpoints para Logs e Notas (usando IGenericRepository)

        [HttpGet("api/logs")]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _logRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<LogViewModel>>(logs));
        }

        [HttpGet("api/notes")]
        public async Task<IActionResult> GetNotes()
        {
            var notes = await _noteRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<LeadNoteViewModel>>(notes));
        }


        [HttpPost("api/logs")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateLog([FromBody] LogViewModel logViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var log = _mapper.Map<Log>(logViewModel);

            // Adiciona usando o repositório genérico e faz o commit
            await _logRepository.AddAsync(log);
            await _logRepository.UnitOfWork.CommitAsync();

            // Retorna o log criado (com o novo ID do banco, se necessário)
            var resultViewModel = _mapper.Map<LogViewModel>(log);
            return Ok(resultViewModel);
        }

        #endregion


        // ADICIONE ESTES MÉTODOS DENTRO DA CrmController.cs

        #region API Endpoints para Comentários e Anexos de Tarefas

        // GET /Crm/api/tasks/{taskId}/comments
        [HttpGet("api/tasks/{taskId:guid}/comments")]
        public async Task<IActionResult> GetComments(Guid taskId)
        {
            var comments = await _commentRepository.SearchAsync(c => c.CrmTaskId == taskId);
            return Ok(_mapper.Map<IEnumerable<CrmTaskCommentViewModel>>(comments));
        }

        // GET /Crm/api/tasks/{taskId}/attachments
        [HttpGet("api/tasks/{taskId:guid}/attachments")]
        public async Task<IActionResult> GetAttachments(Guid taskId)
        {
            var attachments = await _attachmentRepository.SearchAsync(a => a.CrmTaskId == taskId);
            return Ok(_mapper.Map<IEnumerable<CrmTaskAttachmentViewModel>>(attachments));
        }

        // POST /Crm/api/tasks/{taskId}/comments
        [HttpPost("api/tasks/{taskId:guid}/comments")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateComment(Guid taskId, [FromBody] CrmTaskCommentViewModel commentViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = _mapper.Map<CrmTaskComment>(commentViewModel);
            comment.CrmTaskId = taskId; // Associa o comentário ao ID da tarefa da rota

            await _commentRepository.AddAsync(comment);
            if (!await _commentRepository.UnitOfWork.CommitAsync())
            {
                return BadRequest(new { errors = "Ocorreu um erro ao salvar o comentário." });
            }

            var resultViewModel = _mapper.Map<CrmTaskCommentViewModel>(comment);
            return Ok(resultViewModel);
        }

        #endregion


        #region API Endpoints para Anexos de Tarefas

        [HttpPost("api/tasks/{taskId:guid}/attachments")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateAttachment(Guid taskId, [FromBody] CrmTaskAttachmentViewModel attachmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attachment = _mapper.Map<CrmTaskAttachment>(attachmentViewModel);
            attachment.CrmTaskId = taskId; // Associa o anexo ao ID da tarefa da rota

            await _attachmentRepository.AddAsync(attachment);

            if (!await _attachmentRepository.UnitOfWork.CommitAsync())
            {
                return BadRequest(new { errors = "Ocorreu um erro ao salvar o anexo." });
            }

            var resultViewModel = _mapper.Map<CrmTaskAttachmentViewModel>(attachment);
            return Ok(resultViewModel);
        }

        #endregion


        #region Atualizar eventos na Agenda

        [HttpPut("api/activities/{id:guid}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] ActivityViewModel activityViewModel)
        {
            // Garante que o ID da rota é o mesmo do corpo da requisição
            if (id != activityViewModel.Id)
            {
                return BadRequest(new { errors = "Os IDs da rota e do corpo da requisição não coincidem." });
            }

            // Valida o modelo recebido com base nas suas DataAnnotations
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Busca a atividade existente no banco de dados
            var activity = await _activityRepository.GetByIdAsync(id);
            if (activity == null)
            {
                return NotFound(new { errors = "Atividade não encontrada." });
            }

            // Mapeia os dados atualizados da ViewModel para a entidade que já está sendo rastreada pelo EF Core
            _mapper.Map(activityViewModel, activity);

            // Atualiza a entidade no contexto do EF Core
            await _activityRepository.UpdateAsync(activity);

            // Salva as mudanças no banco de dados
            if (!await _activityRepository.UnitOfWork.CommitAsync())
            {
                return BadRequest(new { errors = "Ocorreu um erro ao atualizar a atividade." });
            }

            // Retorna a atividade atualizada
            return Ok(_mapper.Map<ActivityViewModel>(activity));
        }

        #endregion

        //// Helper para obter notificações (pode ser útil nos retornos de erro)
        //private List<string> ObterNotificacoes()
        //{
        //    return _notificador.ObterNotificacoes().Select(n => n.Mensagem).ToList();
        //}
    }
}