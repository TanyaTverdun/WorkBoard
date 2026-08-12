using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;
using WorkBoard.WebAPI.Constants;

namespace WorkBoard.Infrastructure.AI.Plugins;

public class KnowledgeBasePlugin
{
    private readonly IKernelMemory _memory;
    private readonly IUserContext _userContext;

    public KnowledgeBasePlugin(
        IKernelMemory memory,
        IUserContext userContext)
    {
        _memory = memory;
        _userContext = userContext;
    }

    [KernelFunction]
    [Description("""
        Searches the WorkBoard knowledge base for user guides, system rules,
        and configurations. Use this tool to answer 'how-to' questions 
        about Workspaces, Boards, Kanban cards, or to retrieve specific 
        platform limits and admin secrets.
        """)]
    public async Task<string> SearchKnowledgeBaseAsync(
        [Description("""
                    The specific query or keywords to extract relevant 
                    documentation from the knowledge base
                    """)] string query)
    {
        var filter = new MemoryFilter();

        if (_userContext.CurrentWorkspaceRole != WorkspaceRole.Owner)
        {
            filter.ByTag(
                KnowledgeBaseConstants.Tags.AccessLevelKey,
                KnowledgeBaseConstants.Tags.AccessLevelGeneral);
        }

        var searchResult = await _memory.SearchAsync(query, filter: filter);

        if (searchResult.NoResult || !searchResult.Results.Any())
        {
            return "No relevant information was found in the knowledge base.";
        }

        var contexts = searchResult.Results
            .SelectMany(result => result.Partitions)
            .Select(partition => partition.Text);

        return string.Join("\n---\n", contexts);
    }
}
