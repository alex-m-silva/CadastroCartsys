# CadastroCartsys

Sistema desktop para controle de clientes desenvolvido em C# .NET 8 utilizando WinForms, arquitetura MVP e SQL Server 2022.

---

## 📋 Sobre o Projeto

Aplicação desenvolvida como desafio técnico com foco em:

- Arquitetura limpa e organizada (MVP)
- Separação de responsabilidades
- Testabilidade
- Boas práticas de desenvolvimento
- Performance para grandes volumes de dados (25.000+ registros)
- Auditoria e rastreabilidade

---

## 🏗️ Arquitetura

O sistema segue o padrão **MVP (Model-View-Presenter)** com divisão clara em camadas:

- **Core/Domain** → Entidades, DTOs, interfaces e regras de negócio
- **Data** → Repositórios e acesso a dados via Dapper
- **Infrastructure** → Serviços externos (ViaCEP, Auditoria)
- **Presentation** → WinForms + Presenters
- **Tests** → Testes unitários (xUnit)

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
- Microsoft Dependency Injection

---

## 🗄 Banco de Dados

Banco: **SQL Server 2022**

Tabelas principais:

- CLIENTE
- CIDADE
- ESTADO
- AUDITORIA_CLIENTE

### Diferencial técnico
- Utilização de **SEQUENCE** para geração de ID
- Auditoria automática via INSERT/UPDATE/DELETE
- Registro de usuário Windows e estação de trabalho

---

## 🚀 Funcionalidades Implementadas

### ✔ Cadastro de Clientes
- Inserção
- Alteração
- Exclusão
- Pesquisa dinâmica
- Validações de negócio

### ✔ Integração ViaCEP
- Consulta automática ao sair do campo CEP
- Preenchimento automático de endereço
- Tratamento de CEP inválido

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
- Filtros por ID inicial/final, Cidade/Estado ou Todos
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
