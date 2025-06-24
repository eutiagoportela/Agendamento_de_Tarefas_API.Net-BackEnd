using AutoMapper;
using Tarefa.Comunicacao.Requests;
using Tarefa.Comunicacao.Requests.Tarefa;
using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Comunicacao.Responses.Usuario;
using Tarefa.Domain.Entidades;

namespace Tarefa.Aplicacao.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigurarMapeamentosUsuario();
        ConfigurarMapeamentosTarefa();
    }

    private void ConfigurarMapeamentosUsuario()
    {
        // ===== USUARIO ENTITY PARA RESPONSES =====
        CreateMap<Usuarios, UsuarioResponse>();
        // ✅ Agora mapeia DataAtualizacao normalmente

        CreateMap<Usuarios, PerfilUsuarioResponse>()
            .ForMember(dest => dest.TotalTarefas, opt => opt.MapFrom(src =>
                src.Tarefas != null ? src.Tarefas.Count : 0))
            .ForMember(dest => dest.TarefasPendentes, opt => opt.MapFrom(src =>
                src.Tarefas != null ? src.Tarefas.Count(t => !t.Concluida) : 0))
            .ForMember(dest => dest.TarefasConcluidas, opt => opt.MapFrom(src =>
                src.Tarefas != null ? src.Tarefas.Count(t => t.Concluida) : 0))
            .ForMember(dest => dest.UltimaAtividade, opt => opt.MapFrom(src =>
                src.Tarefas != null && src.Tarefas.Any()
                    ? src.Tarefas.Max(t => t.DataAtualizacao) 
                    : (DateTime?)null));

        // ===== REQUESTS PARA USUARIO ENTITY =====
        CreateMap<CriarUsuarioRequest, Usuarios>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore()) 
            .ForMember(dest => dest.Tarefas, opt => opt.Ignore())
            .ForMember(dest => dest.SenhaHash, opt => opt.Ignore()) 
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrWhiteSpace(dest.Nome))
                    dest.Nome = dest.Nome.Trim();

                if (!string.IsNullOrWhiteSpace(dest.Email))
                    dest.Email = dest.Email.ToLowerInvariant().Trim();

                dest.DataCriacao = DateTime.UtcNow;
                dest.DataAtualizacao = DateTime.UtcNow; 
            });
    }

    private void ConfigurarMapeamentosTarefa()
    {
        // ===== TAREFA ENTITY PARA RESPONSES =====
        CreateMap<Tarefas, TarefaResponse>()
            .ForMember(dest => dest.PrioridadeTexto, opt => opt.MapFrom(src => src.Prioridade.ToString()))
            .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src =>
                src.Usuario != null ? src.Usuario.Nome : string.Empty))
            .ForMember(dest => dest.EstaAtrasada, opt => opt.MapFrom(src =>
                !src.Concluida && src.DataConclusao < DateTime.UtcNow))
            .ForMember(dest => dest.DiasRestantes, opt => opt.MapFrom(src =>
                (int)(src.DataConclusao - DateTime.UtcNow).TotalDays));


        // ===== REQUESTS PARA TAREFA ENTITY =====
        CreateMap<CriarTarefaRequest, Tarefas>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore()) 
            .ForMember(dest => dest.Concluida, opt => opt.Ignore())
            .ForMember(dest => dest.LembreteEnviado, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrWhiteSpace(dest.Titulo))
                    dest.Titulo = dest.Titulo.Trim();

                dest.Concluida = false;
                dest.LembreteEnviado = false;
                dest.DataCriacao = DateTime.UtcNow;
                dest.DataAtualizacao = DateTime.UtcNow;
            });

        // ===== MAPEAMENTO PARA ATUALIZAR TAREFA =====
        CreateMap<AtualizarTarefaRequest, Tarefas>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore()) 
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.LembreteEnviado, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrWhiteSpace(dest.Titulo))
                    dest.Titulo = dest.Titulo.Trim();

                if (!string.IsNullOrWhiteSpace(dest.Descricao))
                    dest.Descricao = dest.Descricao.Trim();

                dest.DataAtualizacao = DateTime.UtcNow; 
            });
    }
}