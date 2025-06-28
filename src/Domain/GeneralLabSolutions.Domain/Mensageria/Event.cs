using MediatR;

namespace GeneralLabSolutions.Domain.Mensageria
{
    public abstract class Event : Message, INotification
    {
        public DateTime TimesTamp { get; private set; }

        protected Event()
        {
            TimesTamp = DateTime.Now;
        }
    }
}
