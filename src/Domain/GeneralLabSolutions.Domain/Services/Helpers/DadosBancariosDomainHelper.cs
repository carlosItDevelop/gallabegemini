using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class DadosBancariosDomainHelper
    {
        public static DadosBancarios AdicionarDadosBancariosGenerico<T>(
            T aggregateRoot,
            string banco,
            string agencia,
            string conta,
            TipoDeContaBancaria tipoConta)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novo = new DadosBancarios(banco, agencia, conta, tipoConta, aggregateRoot.Pessoa.Id);

            aggregateRoot.Pessoa.DadosBancarios.Add(novo);

            aggregateRoot.AdicionarEvento(new DadosBancariosAdicionadosEvent(
                aggregateRoot.Id,
                dadosBancariosId: novo.Id,
                banco: banco,
                agencia: agencia,
                conta: conta,
                tipoDeContaBancaria: tipoConta
            ));

            return novo;
        }

        public static void AtualizarDadosBancariosGenerico<T>(
            T aggregateRoot,
            Guid dadosBancariosId,
            string banco,
            string agencia,
            string conta,
            TipoDeContaBancaria tipoConta)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.DadosBancarios is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Dados Bancários associada ao {typeof(T).Name} não foi carregada.");

            var dadosBancariosParaAtualizar = aggregateRoot.Pessoa.DadosBancarios.FirstOrDefault(db => db.Id == dadosBancariosId);

            if (dadosBancariosParaAtualizar is null)
            {
                throw new InvalidOperationException($"Dados bancários com ID {dadosBancariosId} não encontrados para este {typeof(T).Name.ToLower()}.");
            }

            dadosBancariosParaAtualizar.SetBanco(banco);
            dadosBancariosParaAtualizar.SetAgencia(agencia);
            dadosBancariosParaAtualizar.SetConta(conta);
            dadosBancariosParaAtualizar.SetTipoDeContaBancaria(tipoConta);

            aggregateRoot.AdicionarEvento(new DadosBancariosAtualizadosEvent(
               aggregateId: aggregateRoot.Id,
               dadosBancariosId: dadosBancariosId,
               banco: banco,
               agencia: agencia,
               conta: conta,
               tipoDeContaBancaria: tipoConta
           ));
        }

        public static DadosBancarios RemoverDadosBancariosGenerico<T>(T aggregateRoot, Guid dadosBancariosId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.DadosBancarios is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Dados Bancários associada ao {typeof(T).Name} não foi carregada.");

            var db = aggregateRoot.Pessoa.DadosBancarios.FirstOrDefault(d => d.Id == dadosBancariosId);

            if (db is null)
            {
                throw new InvalidOperationException($"Dados bancários com ID {dadosBancariosId} não encontrados para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.DadosBancarios.Remove(db);

            aggregateRoot.AdicionarEvento(new DadosBancariosRemovidosEvent(aggregateRoot.Id, dadosBancariosId));
            return db;
        }
    }
}