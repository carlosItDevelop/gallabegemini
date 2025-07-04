﻿using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Entities.Base
{
    public abstract class EntityBase
    {
        protected EntityBase()
        {
            Id = Guid.NewGuid();
        }


        #region: Notifications and maintenance of the Events

        private List<Event> _notificacoes;
        public IReadOnlyCollection<Event>? Notificacoes => _notificacoes?.AsReadOnly();

        public void AdicionarEvento(Event evento)
        {
            _notificacoes = _notificacoes ?? new List<Event>();
            _notificacoes.Add(evento);
        }

        public void RemoverEvento(Event eventItem) => _notificacoes?.Remove(eventItem);
        public void LimparEventos() => _notificacoes?.Clear();

        #endregion


        #region: Comparações, Validação, ToString
        public virtual bool EhValido() => throw new NotImplementedException();

        public Guid Id { get; set; }

        public override bool Equals(object obj)
        {
            var compareTo = obj as EntityBase;

            if (ReferenceEquals(this, compareTo))
                return true;
            if (ReferenceEquals(null, compareTo))
                return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }
        public static bool operator !=(EntityBase a, EntityBase b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }
        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }
        #endregion
    }
}
