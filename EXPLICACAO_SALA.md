# Explicacao para apresentar ao professor

## O que estava errado antes

Antes, os services montavam SQL manualmente com `CreateCommand`, `CommandText`, `SELECT`, `INSERT` e `UPDATE`.

Isso deixava parte da validacao e da regra misturada com comando de banco.
O professor pediu para seguir o padrao da aula, onde as validacoes ficam nos Models usando `DataAnnotations`.

## O que foi corrigido

Agora os Models possuem as validacoes:

- `Cliente` usa `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Range]`, `[CpfValido]` e `[DataNascimentoValida]`.
- `Divida` usa `[Required]`, `[Range]` e `[StringLength]`.

Os services chamam:

```csharp
Validator.TryValidateObject(objeto, contexto, erros, true);
```

Esse e o mesmo modelo usado no exemplo da Loja:

```csharp
var validation = new ValidationContext(categoria);
Validator.TryValidateObject(categoria, validation, erros);
```

## Como explicar a divisao

Validacao do Model:

- Campo obrigatorio.
- Tamanho do texto.
- Faixa de valor.
- Formato de e-mail.
- CPF valido.
- Data de nascimento valida.

Regra de negocio no Service:

- CPF nao pode repetir, porque precisa consultar clientes ja salvos.
- Cliente nao pode ter duas dividas abertas, porque precisa consultar dividas ja salvas.

Banco de dados:

- Ficou responsavel por guardar os dados.
- O acesso ao banco passou a ser feito com Entity Framework e LINQ.
- Nao ha mais `CommandText` nem SQL manual dentro dos services.

## Frase curta para a apresentacao

> Eu corrigi a estrutura para deixar as validacoes nos Models com DataAnnotations, igual ao exemplo da Loja. O service apenas chama o Validator e executa regras que dependem do banco, como CPF unico e apenas uma divida aberta por cliente.
