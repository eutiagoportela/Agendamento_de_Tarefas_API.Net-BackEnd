# Desafio: Sistema de Agendamento de Tarefas
### **📋 FUNCIONALIDADES IMPLEMENTADAS:**

#### **🔐 AUTENTICAÇÃO:**
- ✅ Registro de usuário
- ✅ Login com JWT
- ✅ Middleware de autorização
- ✅ Hash de senhas (BCrypt)

#### **👤 USUÁRIOS:**
- ✅ CRUD completo
- ✅ Perfil do usuário
- ✅ Validação de email único
- ✅ Validações robustas

#### **📋 TAREFAS:**
- ✅ CRUD completo
- ✅ Sistema de prioridades (1-4)
- ✅ Filtros avançados:
  - Todas as tarefas
  - Tarefas pendentes
  - **Tarefas concluídas** ✅ NOVO!
  - Por prioridade
- ✅ Marcar como concluída
- ✅ Controle de vencimento
- ✅ Sistema de lembretes automáticos

#### **🔔 SISTEMA DE LEMBRETES:**
- ✅ Background service
- ✅ Verificação automática (5 em 5 minutos)
- ✅ Logs estruturados
- ✅ Controle de lembretes enviados

#### **🛡️ SEGURANÇA:**
- ✅ JWT Authentication
- ✅ Authorization em todos endpoints
- ✅ Validação de propriedade (usuário só acessa suas tarefas)
- ✅ Hash seguro de senhas
- ✅ Tratamento de exceções

#### **📊 QUALIDADE DE CÓDIGO:**
- ✅ Logs estruturados (ILogger)
- ✅ Tratamento de exceções customizadas
- ✅ Validações de entrada
- ✅ Responses padronizadas
- ✅ Documentação OpenAPI/Swagger
- ✅ Clean Architecture
- ✅ SOLID principles

---

## 🎯 ENDPOINTS FINAIS:

### **🔐 Autenticação (2):**
- `POST /api/auth/login`

### **👤 Usuários (5):**
- `POST /api/usuario/registrar`
- `GET /api/usuario/{id}`
- `PUT /api/usuario/{id}`
- `DELETE /api/usuario/{id}`

### **📋 Tarefas (10):**
- `POST /api/tarefa`
- `GET /api/tarefa`
- `GET /api/tarefa/{id}`
- `GET /api/tarefa/pendentes`
- `GET /api/tarefa/concluidas` ✅ **NOVO!**
- `GET /api/tarefa/prioridade/{prioridade}`
- `PUT /api/tarefa/{id}`
- `PATCH /api/tarefa/{id}/concluir`
- `DELETE /api/tarefa/{id}`

**Total: 17 endpoints funcionais** 🚀

---

## 📚 ARQUIVOS DE CONFIGURAÇÃO:

### **🐳 Docker:**
- `docker-compose.yml` - PostgreSQL configurado

### **⚙️ Configurações:**
- `appsettings.json` - Base
- `appsettings.Development.json` - Desenvolvimento  
- `appsettings.Production.json` - Produção

### **📖 Documentação:**
- `README.md` - Setup completo
- Swagger UI - Documentação interativa

---

### **✅ IMPLEMENTADO:**
- Clean Architecture
- Entity Framework Core + PostgreSQL  
- JWT Authentication
- AutoMapper
- CRUD completo de Usuários e Tarefas
- Sistema de filtros
- Background Service para lembretes
- Logs estruturados
- Tratamento de exceções
- Validações robustas
- Documentação completa


# 📊 ARQUITETURA VISUAL - CLEAN ARCHITECTURE

```
┌─────────────────────────────────────────────────────────────────┐
│                    🎯 CAMADA DE APRESENTAÇÃO                     │
│                         Tarefa.Api                              │
├─────────────────────────────────────────────────────────────────┤
│  Controllers/        Extensions/        Configuration/          │
│  • AuthController    • DependencyInjection   • Program.cs      │
│  • TarefaController  • JwtAuthentication      • appsettings.*   │
│  • UsuarioController                                            │
└─────────────────────────────────────────────────────────────────┘
                                  ⬇️
┌─────────────────────────────────────────────────────────────────┐
│                     🧠 CAMADA DE APLICAÇÃO                       │
│                      Tarefa.Aplicacao                           │
├─────────────────────────────────────────────────────────────────┤
│  UseCases/              AutoMapper/         Services/           │
│  📁 Login/              • MappingProfile    • LembreteService   │
│    └── DoLogin                                                  │
│  📁 Usuario/ (7 use cases)                                      │
│    ├── CriarUsuario                                             │
│    ├── ObterUsuario                                             │
│    ├── AtualizarUsuario                                         │
│    ├── DeletarUsuario                                           │
│    ├── ListarTodosUsuarios                                      │
│    ├── ObterPerfilUsuario                                       │
│    └── VerificarEmailExiste                                     │
│  📁 Tarefa/ (8 use cases)                                       │
│    ├── CriarTarefa                                              │
│    ├── ObterTarefa                                              │
│    ├── AtualizarTarefa                                          │
│    ├── DeletarTarefa                                            │
│    ├── ObterTarefasUsuario                                      │
│    ├── ObterTarefasPendentes                                    │
│    ├── ObterTarefasConcluidas ✅ NOVO!                          │
│    ├── ObterTarefasPorPrioridade                                │
│    └── MarcarTarefaComoConcluida                                │
└─────────────────────────────────────────────────────────────────┘
                                  ⬇️
┌─────────────────────────────────────────────────────────────────┐
│                    🏛️ CAMADA DE DOMÍNIO                          │
│                       Tarefa.Domain                             │
├─────────────────────────────────────────────────────────────────┤
│  Entidades/          Enum/              Repositorios/           │
│  • Usuarios.cs       • PrioridadeTarefa  Interfaces/           │
│  • Tarefas.cs                            • IUsuarioRepository   │
│                                          • ITarefaRepository    │
└─────────────────────────────────────────────────────────────────┘
                                  ⬇️
┌─────────────────────────────────────────────────────────────────┐
│                   🔧 CAMADA DE INFRAESTRUTURA                    │
│                     Tarefa.Infraestrutura                       │
├─────────────────────────────────────────────────────────────────┤
│  Repositorios/       Security/           Database/              │
│  Implementacoes/     • PasswordHasher    • PostgreSqlDbContext  │
│  • UsuarioRepository • JwtTokenGenerator • Migrations/          │
│  • TarefaRepository  • LembreteService                          │
└─────────────────────────────────────────────────────────────────┘
                                  ⬇️
┌─────────────────────────────────────────────────────────────────┐
│                      🗄️ BANCO DE DADOS                           │
│                     PostgreSQL (Docker)                         │
├─────────────────────────────────────────────────────────────────┤
│  Tabelas:                                                       │
│  • Usuarios (Id, Nome, Email, SenhaHash, DataCriacao...)       │
│  • Tarefas (Id, Titulo, Descricao, Prioridade, Concluida...)   │
│  • __EFMigrationsHistory (Controle de versões)                 │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                   📡 CAMADAS TRANSVERSAIS                        │
├─────────────────────────────────────────────────────────────────┤
│  Tarefa.Comunicacao/        Tarefa.Exceptions/                 │
│  • DTOs                     • AcessoNegadoException             │
│  • Requests                 • EmailJaExisteException            │
│  • Responses                • TarefaNaoEncontradaException      │
│                            • UsuarioNaoEncontradoException      │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔄 FLUXO DE DADOS:

```
📱 Cliente (Swagger/Frontend)
        ⬇️ HTTP Request
🎯 Controller (TarefaController)
        ⬇️ Valida entrada
🧠 Use Case (ObterTarefasConcluidasUseCase)
        ⬇️ Regras de negócio
🏛️ Repository Interface (ITarefaRepository)
        ⬇️ Implementação
🔧 Repository Implementation (TarefaRepository)
        ⬇️ Entity Framework
🗄️ PostgreSQL Database
        ⬇️ Dados retornados
🔧 AutoMapper (Entity → DTO)
        ⬇️ Response formatada
📱 Cliente (JSON Response)
