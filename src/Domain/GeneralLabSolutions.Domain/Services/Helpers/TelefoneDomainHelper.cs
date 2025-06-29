using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class TelefoneDomainHelper
    {
        public static Telefone AdicionarTelefoneGenerico<T>(T aggregateRoot, string ddd, string numero, TipoDeTelefone tipoTelefone)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novoTelefone = new Telefone(ddd, numero, tipoTelefone, aggregateRoot.Pessoa.Id);

            aggregateRoot.Pessoa.Telefones.Add(novoTelefone);

            aggregateRoot.AdicionarEvento(new TelefoneAdicionadoEvent(
                aggregateId: aggregateRoot.Id,
                telefoneId: novoTelefone.Id,
                ddd: ddd,
                numero: numero,
                tipoDeTelefone: tipoTelefone
            ));

            return novoTelefone;
        }

        public static void AtualizarTelefoneGenerico<T>(T aggregateRoot, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Telefones is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Telefones associada ao {typeof(T).Name} não foi carregada.");

            var telefoneParaAtualizar = aggregateRoot.Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaAtualizar is null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            telefoneParaAtualizar.SetDDD(ddd);
            telefoneParaAtualizar.SetNumero(numero);
            telefoneParaAtualizar.SetTipoDeTelefone(tipoTelefone);

            aggregateRoot.AdicionarEvento(new TelefoneAtualizadoEvent(
                aggregateId: aggregateRoot.Id,
                telefoneId: telefoneId,
                ddd: ddd,
                numero: numero,
                tipoDeTelefone: tipoTelefone
            ));
        }

        public static Telefone RemoverTelefoneGenerico<T>(T aggregateRoot, Guid telefoneId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var telefoneParaRemover = aggregateRoot.Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaRemover is null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.Telefones.Remove(telefoneParaRemover);

            aggregateRoot.AdicionarEvento(new TelefoneRemovidoEvent(
                aggregateId: aggregateRoot.Id,
                telefoneId: telefoneId
            ));

            return telefoneParaRemover;
        }
    }
}