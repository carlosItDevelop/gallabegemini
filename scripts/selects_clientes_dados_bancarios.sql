--Select * from Cliente;

Select c.Nome, d.Agencia, d.Banco, d.Conta, d.TipoDeContaBancaria from Cliente c inner join DadosBancarios d 
on c.PessoaId = d.PessoaId where c.PessoaId = 'BD44BAA8-2FD8-49AC-BFBA-AD1603E163E7' order by c.Nome;

--SELECT c.Nome,
--       STRING_AGG(
--           CONCAT(d.Banco, ' - ', d.Agencia, ' / ', d.Conta,
--                  ' (', d.TipoDeContaBancaria, ')'),
--           '; '
--       ) AS Contas
--FROM   Cliente        AS c
--JOIN   DadosBancarios AS d ON d.PessoaId = c.PessoaId
--GROUP  BY c.Nome
--ORDER  BY c.Nome;


