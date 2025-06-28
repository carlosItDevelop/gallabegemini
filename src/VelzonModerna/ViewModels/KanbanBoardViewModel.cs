namespace VelzonModerna.ViewModels
{
    public class KanbanBoardViewModel
    {
        public IEnumerable<KanbanTaskViewModel> Tasks { get; set; }
        public IEnumerable<ParticipanteViewModel> Participantes { get; set; }
    }

}
