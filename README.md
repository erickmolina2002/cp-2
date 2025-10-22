# Sistema Loja

Sistema de gerenciamento de loja desenvolvido em C# com SQL Server.

## Como Iniciar

### Inicialização Completa

Execute o comando abaixo para iniciar toda a aplicação:

```bash
docker-compose up --build
```

### Interagir com a Aplicação, abra outro terminal

```bash
docker exec -it sistema_loja_app dotnet SistemaLoja.dll
```

## Parar a Aplicação

```bash
docker-compose down
```