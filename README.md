# Vendinha Backend

Projeto feito em C# com .NET 8 para controlar clientes e dívidas de uma vendinha.

## O que o projeto tem

- Cadastro, consulta, alteração e exclusão de clientes
- Cadastro, consulta, pagamento e exclusão de dívidas
- Banco de dados SQLite
- Integração com DBeaver
- Controllers com rotas HTTP
- Validação de CPF
- Validação simples de e-mail
- Idade calculada pela data de nascimento
- Regra para permitir apenas uma dívida aberta por cliente
- Busca de cliente por nome
- Paginação de 10 em 10
- Ordenação pelo cliente que mais deve

## Programas usados

- Visual Studio
- .NET 8
- DBeaver
- SQLite

## Como abrir no Visual Studio

1. Abra o Visual Studio.
2. Clique em Open a project or solution.
3. Escolha o arquivo VendinhaBackend.csproj.
4. Aguarde carregar o projeto.
5. Aperte F5 para executar.
6. Copie o endereço que aparecer no navegador ou terminal.

## Como testar no navegador

Depois de executar, use o endereço do projeto com as rotas.

Exemplos:

```text
/api/clientes
/api/clientes/1
/api/dividas/cliente/1
```

## Como abrir o banco no DBeaver

1. Execute o projeto uma vez.
2. O arquivo vendinha.db será criado na pasta do projeto.
3. Abra o DBeaver.
4. Clique em Nova conexão.
5. Escolha SQLite.
6. Selecione o arquivo vendinha.db.
7. Abra as tabelas Clientes e Dividas.

## Script do banco

O script está na pasta:

```text
Database/schema.sql
```

## Rotas de clientes

```text
GET /api/clientes
GET /api/clientes/{id}
POST /api/clientes
PUT /api/clientes/{id}
DELETE /api/clientes/{id}
```

Exemplo para cadastrar cliente:

```json
{
  "nomeCompleto": "Ana Silva",
  "cpf": "52998224725",
  "dataNascimento": "2000-05-10",
  "email": "ana@email.com"
}
```

## Rotas de dívidas

```text
GET /api/dividas/cliente/{clienteId}
POST /api/dividas
PUT /api/dividas/{id}/pagar
DELETE /api/dividas/{id}
```

Exemplo para cadastrar dívida:

```json
{
  "clienteId": 1,
  "valor": 150.75
}
```

## Explicação curta

O projeto é uma API em C# usando Controllers. O Controller recebe uma requisição HTTP, executa uma regra do sistema e devolve uma resposta. O banco de dados é SQLite e pode ser aberto no DBeaver. O projeto possui dois CRUDs: clientes e dívidas.
