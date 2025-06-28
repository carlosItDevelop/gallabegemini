using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using GeneralLabSolutions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.ViewModels;
using GeneralLabSolutions.Domain.Services.Concreted;
using VelzonModerna.Controllers.Base;
using Microsoft.EntityFrameworkCore;

namespace VelzonModerna.Controllers
{
    [Route("[controller]")]
    public class QuadroKanbanController : BaseMvcController
    {
        private readonly INotificador _notificador;
        private readonly IMapper _mapper;
        private readonly IKanbanTaskDomainService _taskDomainService;
        private readonly IParticipanteDomainService _participanteDomainService;
        private readonly IKanbanTaskRepository _taskRepo;
        private readonly IParticipanteRepository _participanteRepo;
        private readonly IGenericRepository<KanbanTask, Guid> _taskQuery;
        private readonly IGenericRepository<Participante, Guid> _participanteQuery;

        public QuadroKanbanController(
            INotificador notificador,
            IMapper mapper,
            IKanbanTaskDomainService taskDomainService,
            IParticipanteDomainService participanteDomainService,
            IKanbanTaskRepository taskRepo,
            IParticipanteRepository participanteRepo,
            IGenericRepository<KanbanTask, Guid> taskQuery,
            IGenericRepository<Participante, Guid> participanteQuery
        )
        : base(notificador)
        {
            _mapper = mapper;
            _taskDomainService = taskDomainService;
            _participanteDomainService = participanteDomainService;
            _taskRepo = taskRepo;
            _participanteRepo = participanteRepo;
            _taskQuery = taskQuery;
            _participanteQuery = participanteQuery;
            _notificador = notificador;
        }

        // ============================================
        // Views MVC
        // ============================================
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var tasks = await _taskQuery.GetAllAsync();
            var participantes = await _participanteQuery.GetAllAsync();

            var taskViewModels = _mapper.Map<IEnumerable<KanbanTaskViewModel>>(tasks);
            var participantViewModels = _mapper.Map<IEnumerable<ParticipanteViewModel>>(participantes);

            var viewModel = new KanbanBoardViewModel
            {
                Tasks = taskViewModels,
                Participantes = participantViewModels
            };

            return View(viewModel);
        }



        // ============================================
        // Endpoints JSON para Tarefas
        // ============================================
        [HttpGet("Tasks")]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _taskQuery.GetAllAsync();
            var listVM = _mapper.Map<IEnumerable<KanbanTaskViewModel>>(tasks);
            return Ok(listVM);
        }

        [HttpPost("Tasks")]
        //[ValidateAntiForgeryToken] // Validação do AntiForgery Token
        [IgnoreAntiforgeryToken] // Testar
        public async Task<IActionResult> CreateTask([FromBody] KanbanTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    var fieldName = kvp.Key;
                    var errors = kvp.Value.Errors;
                    foreach (var err in errors)
                    {
                        Console.WriteLine($"Field: {fieldName}, Error: {err.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }


            // Mapeia a KanbanTask (mas vamos ignorar Participantes via AutoMapper)
            var entity = _mapper.Map<KanbanTask>(model);

            // Carrega os Participantes do banco
            if (model.ParticipantIds != null && model.ParticipantIds.Any())
            {
                var participantsFromDb = await _participanteRepo
                    .BuscaSemNoTraking(p => model.ParticipantIds.Contains(p.Id));

                // Adiciona-os ao KanbanTask
                foreach (var part in participantsFromDb)
                        entity.Participantes.Add(part);

            }

            // Salva
            await _taskDomainService.AddkanbanTaskAsync(entity);

            if (!OperacaoValida())
                return BadRequest(new { errors = GetNotificacoes() });

            //entity.Id = Guid.NewGuid();

            await _taskRepo.UnitOfWork.CommitAsync();

            // Agora, se quiser retornar algo ao front-end,
            // pode mapear de volta:
            var result = _mapper.Map<KanbanTaskViewModel>(entity);
            return Ok(result);
        }

        
        [HttpPut("Tasks/{id:guid}")]
        //[ValidateAntiForgeryToken] // Validação do AntiForgery Token
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] KanbanTaskViewModel model)
        {
            if (id != model.Id)
                return BadRequest("IDs não coincidem.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _taskQuery.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Tarefa não encontrada.");

            _mapper.Map(model, existing);
            await _taskRepo.UpdateAsync(existing);

            if (!OperacaoValida())
                return BadRequest(new { errors = GetNotificacoes() });

            await _taskRepo.UnitOfWork.CommitAsync();
            var updatedVM = _mapper.Map<KanbanTaskViewModel>(existing);
            return Ok(updatedVM);
        }

        // DELETE /QuadroKanban/Tasks/{id}
        [HttpDelete("Tasks/{id:guid}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var existing = await _taskQuery.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Tarefa não encontrada.");

            await _taskRepo.DeleteAsync(existing);

            if (!OperacaoValida())
                return BadRequest(new { errors = GetNotificacoes() });

            await _taskRepo.UnitOfWork.CommitAsync();
            return Ok("Tarefa excluída com sucesso!");
        }

        // ============================================
        // Endpoints JSON para Participantes
        // ============================================
        [HttpGet("Participantes")]
        public async Task<IActionResult> GetParticipantes()
        {
            var parts = await _participanteQuery.GetAllAsync();
            var listVM = _mapper.Map<IEnumerable<ParticipanteViewModel>>(parts);
            return Ok(listVM);
        }

        [HttpPost("Participantes")]
        [IgnoreAntiforgeryToken] // Validação do AntiForgery Token
        public async Task<IActionResult> CreateParticipante([FromBody] ParticipanteViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<Participante>(model);
            await _participanteDomainService.AddParticipanteAsync(entity);

            if (!OperacaoValida())
                return BadRequest(new { errors = GetNotificacoes() });

            await _participanteRepo.UnitOfWork.CommitAsync();
            var resultVM = _mapper.Map<ParticipanteViewModel>(entity);
            return Ok(resultVM);
        }

        [HttpDelete("Participantes/{id:guid}")]
        public async Task<IActionResult> DeleteParticipante(Guid id)
        {
            var existing = await _participanteQuery.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Participante não encontrado.");

            await _participanteRepo.DeleteAsync(existing);
            if (!OperacaoValida())
                return BadRequest(new { errors = GetNotificacoes() });

            await _participanteRepo.UnitOfWork.CommitAsync();
            return Ok("Participante excluído com sucesso!");
        }

        // Helper para obter as notificações do Notificador
        private IEnumerable<Notificacao> GetNotificacoes()
        {
            return _notificador.ObterNotificacoes();
        }
    }
}
