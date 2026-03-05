# CadastroCartsys

Sistema desktop para controle de clientes desenvolvido em C# .NET 8 utilizando WinForms, arquitetura MVP e SQL Server 2022.

---

## 📋 Sobre o Projeto

Aplicação desenvolvida como desafio técnico com foco em:

- Arquitetura limpa e organizada (MVP)
- Separação de responsabilidades
- Testabilidade
- Auditoria

---

## 🏗️ Arquitetura

O sistema segue o padrão **MVP (Model-View-Presenter):**

- **Core/Domain** 
- **Data**
- **Infrastructure**
- **Presentation**
- **Tests**

---

## 🛠 Tecnologias Utilizadas

- C# / .NET 8
- WinForms
- SQL Server 2022
- Dapper
- FastReport OpenSource
- xUnit
- NSubstitute
- FluentAssertions

---

## 🗄 Banco de Dados

Banco: **SQL Server 2022**

Tabelas principais:

- CLIENTE
- CIDADE
- ESTADO
- AUDITORIA_CLIENTE

---

## 🚀 Funcionalidades Implementadas

### ✔ Cadastro de Clientes
- Inserção
- Alteração
- Exclusão

### ✔ Integração ViaCEP
- Consulta automática ao sair do campo CEP
- Preenchimento automático de endereço

### ✔ Pesquisa Otimizada
- Cache em memória
- Filtro por qualquer campo
- Busca sem acentuação
- Debounce para performance

### ✔ Regras de Negócio
- Nome obrigatório
- CPF/CNPJ obrigatório (11 ou 14 dígitos)
- CEP opcional, mas validado se preenchido
- Cidade obrigatória
- IDs protegidos (1, 5, 8, 10, 15) não podem ser excluídos
- Confirmação antes da exclusão

### ✔ Relatório em PDF
- Gerado via FastReport
- Filtros por Código inicial/final, Cidade/Estado ou Todos
- Cabeçalho com descrição do filtro aplicado

---

## 🧪 Testes Unitários

Testes desenvolvidos com:

- xUnit
- NSubstitute
- FluentAssertions

Cobrem:

- Validações de campos
- Regras de exclusão protegida
- Fluxo de confirmação
- Registro de auditoria
- Cenários válidos e inválidos

Executar testes:

```bash
dotnet test
```

## 📋 Pré-requisitos

Para configurar e executar este projeto localmente, você precisará das seguintes ferramentas:

* **[Docker Desktop](https://www.docker.com/products/docker-desktop/)**: Necessário para rodar o container do SQL Server 2022 de forma isolada, sem precisar instalar o banco na máquina.
* **[SQL Server Management Studio (SSMS)](https://learn.microsoft.com/pt-br/sql/ssms/download-sql-server-management-studio-ssms)**: Para conectar ao banco de dados e executar o script de criação e população das tabelas.
* **[.NET 8 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)**: Para compilar e executar a aplicação.

---

## ⚙️ Como Configurar e Executar

Para facilitar a avaliação, o projeto foi estruturado para rodar rapidamente utilizando Docker e os executáveis já estão compilados.

### 1. Banco de Dados (Docker + SQL Server 2022)
Para não precisar instalar o SQL Server localmente, você pode subir uma instância via Docker com o comando abaixo:

```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=cartsys@123" -p 1433:1433 --name mssql -d mcr.microsoft.com/mssql/server:2022-latest
```
# CadastroCartsys

Sistema de cadastro de clientes desenvolvido em .NET.

[![Download](https://img.shields.io/badge/Download-Release-blue?style=for-the-badge)](https://github.com/alex-m-silva/CadastroCartsys/releases/tag/v1.0.0)
