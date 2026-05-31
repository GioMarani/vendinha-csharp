# Vendinha Backend

Projeto em C# com .NET 8 para controlar clientes e dívidas de uma vendinha.

## Padrão usado

O projeto foi organizado no padrão pedido em aula:

- Controller recebe a requisição
- Controller chama o Service
- Service faz validação e regra de negócio
- Banco SQLite aberto pelo DBeaver
- Swagger para testar as rotas

## Programas usados

- Visual Studio
- .NET 8
- DBeaver
- SQLite

## Como abrir no Visual Studio

1. Abra o Visual Studio.
2. Clique em Open a project or solution.
3. Escolha o arquivo VendinhaBackend.sln ou VendinhaBackend.csproj.
4. Aperte F5.
5. O navegador deve abrir direto no Swagger.

## Como abrir o banco no DBeaver

1. Abra o DBeaver.
2. Clique em Nova conexão.
3. Escolha SQLite.
4. Selecione o arquivo Database/vendinha.db.
5. Abra as tabelas Clientes e Dividas.

## CRUDs do projeto

Clientes:

- GET /api/clientes
- GET /api/clientes/{id}
- POST /api/clientes
- PUT /api/clientes/{id}
- DELETE /api/clientes/{id}

Dívidas:

- GET /api/dividas/cliente/{clienteId}
- POST /api/dividas
- PUT /api/dividas/{id}/pagar
- DELETE /api/dividas/{id}

## Exemplo de cliente para testar no Swagger

{
  "nomeCompleto": "Ana Silva",
  "cpf": "52998224725",
  "dataNascimento": "2000-05-10",
  "email": "ana@email.com"
}

## Exemplo de dívida para testar no Swagger

{
  "clienteId": 1,
  "valor": 150.75
}

## Validações principais

- Nome obrigatório
- CPF válido
- CPF único
- Data de nascimento precisa ser anterior ao dia atual
- E-mail precisa conter @ quando informado
- Dívida precisa ter valor maior que zero
- Cliente só pode ter uma dívida em aberto
