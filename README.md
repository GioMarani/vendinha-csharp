# Vendinha Backend

Projeto em C# com .NET 8 para controlar clientes e dividas de uma vendinha.

## Objetivo

A aplicacao permite:

- CRUD de clientes.
- Cadastro, listagem, pagamento e exclusao de dividas.
- Busca de cliente por nome.
- Paginacao de clientes de 10 em 10.
- Ordenacao dos clientes do que mais deve para o que menos deve.
- Validacao dos dados usando Models com `DataAnnotations`.
- Persistencia em banco SQLite.

## Padrao usado

O projeto foi organizado no padrao trabalhado em aula:

- `Controller` recebe a chamada da API.
- `Service` executa a regra de negocio.
- `Model` possui as validacoes com `[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]` e validadores proprios.
- `VendinhaDbContext` representa o banco usando Entity Framework.
- O service chama `Validator.TryValidateObject`, como no exemplo da Loja.

## Validacoes nos Models

Cliente:

- Nome completo obrigatorio.
- CPF obrigatorio, com 11 numeros e CPF valido.
- Data de nascimento obrigatoria e anterior ao dia atual.
- Idade calculada pela data de nascimento.
- E-mail validado quando informado.

Divida:

- Cliente obrigatorio.
- Valor maior que zero.
- Situacao obrigatoria.
- Data de criacao obrigatoria.

As regras que dependem de dados ja gravados continuam no service:

- Nao permitir CPF duplicado.
- Nao permitir mais de uma divida aberta para o mesmo cliente.

Essas duas regras precisam consultar o banco, entao nao ficam somente no atributo do Model.

## Programas usados

- Visual Studio
- .NET 8
- SQLite
- Swagger
- Entity Framework Core

## Como abrir no Visual Studio

1. Abra o Visual Studio.
2. Clique em `Open a project or solution`.
3. Escolha `VendinhaBackend.sln` ou `VendinhaBackend.csproj`.
4. Aperte F5.
5. O navegador deve abrir no Swagger.

## Banco de dados

O banco fica em:

```text
Database/vendinha.db
```

O script de criacao esta em:

```text
database.sql
```

## Rotas principais

Clientes:

- `GET /api/clientes`
- `GET /api/clientes/{id}`
- `POST /api/clientes`
- `PUT /api/clientes/{id}`
- `DELETE /api/clientes/{id}`

Dividas:

- `GET /api/dividas/cliente/{clienteId}`
- `POST /api/dividas`
- `PUT /api/dividas/{id}/pagar`
- `DELETE /api/dividas/{id}`

## Exemplo de cliente

```json
{
  "nomeCompleto": "Ana Silva",
  "cpf": "52998224725",
  "dataNascimento": "2000-05-10",
  "email": "ana@email.com"
}
```

## Exemplo de divida

```json
{
  "clienteId": 1,
  "valor": 150.75
}
```
