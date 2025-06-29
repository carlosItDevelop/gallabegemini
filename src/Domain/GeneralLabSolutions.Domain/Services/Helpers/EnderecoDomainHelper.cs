using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class EnderecoDomainHelper
    {
        public static Endereco AdicionarEnderecoGenerico<T>(
            T aggregateRoot,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novoEndereco = new Endereco(
                aggregateRoot.Pessoa.Id, // FK para Pessoa
                paisCodigoIso,
                linhaEndereco1,
                cidade,
                codigoPostal,
                tipoDeEndereco,
                linhaEndereco2,
                estadoOuProvincia,
                informacoesAdicionais
            );

            aggregateRoot.Pessoa.Enderecos.Add(novoEndereco);

            aggregateRoot.AdicionarEvento(new EnderecoAdicionadoEvent(
                aggregateId: aggregateRoot.Id,
                enderecoId: novoEndereco.Id,
                paisCodigoIso: paisCodigoIso,
                linhaEndereco1: linhaEndereco1,
                cidade: cidade,
                codigoPostal: codigoPostal,
                tipoDeEndereco: tipoDeEndereco
            ));

            return novoEndereco;
        }

        public static void AtualizarEnderecoGenerico<T>(
            T aggregateRoot,
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Enderecos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Endereços associada ao {typeof(T).Name} não foi carregada.");

            var enderecoParaAtualizar = aggregateRoot.Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaAtualizar is null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            enderecoParaAtualizar.SetPaisCodigoIso(paisCodigoIso);
            enderecoParaAtualizar.SetLinhaEndereco1(linhaEndereco1);
            enderecoParaAtualizar.SetLinhaEndereco2(linhaEndereco2);
            enderecoParaAtualizar.SetCidade(cidade);
            enderecoParaAtualizar.SetEstadoOuProvincia(estadoOuProvincia);
            enderecoParaAtualizar.SetCodigoPostal(codigoPostal);
            enderecoParaAtualizar.SetTipoDeEndereco(tipoDeEndereco);
            enderecoParaAtualizar.SetInformacoesAdicionais(informacoesAdicionais);

            aggregateRoot.AdicionarEvento(new EnderecoAtualizadoEvent(
                aggregateId: aggregateRoot.Id,
                enderecoId: enderecoId,
                paisCodigoIso: paisCodigoIso,
                linhaEndereco1: linhaEndereco1,
                cidade: cidade,
                codigoPostal: codigoPostal,
                tipoDeEndereco: tipoDeEndereco
            ));
        }

        public static Endereco RemoverEnderecoGenerico<T>(T aggregateRoot, Guid enderecoId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Enderecos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Endereços associada ao {typeof(T).Name} não foi carregada.");

            var enderecoParaRemover = aggregateRoot.Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaRemover is null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.Enderecos.Remove(enderecoParaRemover);

            aggregateRoot.AdicionarEvento(new EnderecoRemovidoEvent(
                aggregateId: aggregateRoot.Id,
                enderecoId: enderecoId
            ));

            return enderecoParaRemover;
        }
    }
}