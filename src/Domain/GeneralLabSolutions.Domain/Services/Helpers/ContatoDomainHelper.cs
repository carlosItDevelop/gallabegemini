using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class ContatoDomainHelper
    {
        public static Contato AdicionarContatoGenerico<T>(
            T aggregateRoot,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novoContato = new Contato(
                nome,
                email,
                telefone,
                tipoDeContato,
                aggregateRoot.Pessoa.Id
            )
            {
                EmailAlternativo = emailAlternativo ?? string.Empty,
                TelefoneAlternativo = telefoneAlternativo ?? string.Empty,
                Observacao = observacao ?? string.Empty
            };

            aggregateRoot.Pessoa.Contatos.Add(novoContato);

            aggregateRoot.AdicionarEvento(new ContatoAdicionadoEvent(
                aggregateId: aggregateRoot.Id,
                contatoId: novoContato.Id,
                nome: nome,
                email: email,
                telefone: telefone,
                tipoDeContato: tipoDeContato
            ));

            return novoContato;
        }

        public static void AtualizarContatoGenerico<T>(
            T aggregateRoot,
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Contatos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Contatos associada ao {typeof(T).Name} não foi carregada.");

            var contatoParaAtualizar = aggregateRoot.Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaAtualizar is null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            contatoParaAtualizar.AtualizarNome(nome);
            contatoParaAtualizar.AtualizarEmail(email);
            contatoParaAtualizar.AtualizarTelefone(telefone);
            contatoParaAtualizar.DefineTipoDeContato(tipoDeContato);
            contatoParaAtualizar.AtualizarEmailAlternativo(emailAlternativo);
            contatoParaAtualizar.AtualizarTelefoneAlternativo(telefoneAlternativo);
            contatoParaAtualizar.AtualizarObservacao(observacao);

            aggregateRoot.AdicionarEvento(new ContatoAtualizadoEvent(
                aggregateId: aggregateRoot.Id,
                contatoId: contatoId,
                nome: nome,
                email: email,
                telefone: telefone,
                tipoDeContato: tipoDeContato
            ));
        }

        public static Contato RemoverContatoGenerico<T>(T aggregateRoot, Guid contatoId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Contatos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Contatos associada ao {typeof(T).Name} não foi carregada.");

            var contatoParaRemover = aggregateRoot.Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaRemover is null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.Contatos.Remove(contatoParaRemover);

            aggregateRoot.AdicionarEvento(new ContatoRemovidoEvent(
                aggregateId: aggregateRoot.Id,
                contatoId: contatoId
            ));

            return contatoParaRemover;
        }
    }
}