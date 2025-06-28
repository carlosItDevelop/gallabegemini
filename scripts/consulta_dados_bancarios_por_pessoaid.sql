select c.Nome, c.PessoaId, d.Banco, d.Agencia, d.Conta
from Cliente c inner join DadosBancarios d
on c.PessoaId = d.PessoaId 
where c.PessoaId = 'BD44BAA8-2FD8-49AC-BFBA-AD1603E163E7'
order by c.Nome;