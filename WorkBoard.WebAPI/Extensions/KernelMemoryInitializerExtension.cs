using Microsoft.KernelMemory;
using WorkBoard.WebAPI.Constants;

namespace WorkBoard.WebAPI.Extensions;

public static class KernelMemoryInitializer
{
    public static async Task SeedKnowledgeBaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var memory = scope.ServiceProvider.GetRequiredService<IKernelMemory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting knowledge base initialization...");

            await SeedDocumentAsync(
                memory,
                logger,
                KnowledgeBaseConstants.General.DocumentId,
                KnowledgeBaseConstants.General.FileName,
                KnowledgeBaseConstants.Tags.AccessLevelGeneral);

            await SeedDocumentAsync(
                memory,
                logger,
                KnowledgeBaseConstants.Admin.DocumentId,
                KnowledgeBaseConstants.Admin.FileName,
                KnowledgeBaseConstants.Tags.AccessLevelOwner);

            logger.LogInformation("Knowledge base initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the Kernel Memory knowledge base.");
        }
    }

    private static async Task SeedDocumentAsync(
        IKernelMemory memory,
        ILogger logger,
        string documentId,
        string fileName,
        string accessLevelTag)
    {
        var isReady = await memory.IsDocumentReadyAsync(
            documentId: documentId);

        if (isReady)
        {
            logger.LogInformation(
                "Document '{DocumentId}' is already indexed.", 
                documentId);
            return;
        }

        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            KnowledgeBaseConstants.DirectoryName,
            fileName);

        if (!File.Exists(filePath))
        {
            logger.LogWarning(
                "Knowledge base file not found at {FilePath}", 
                filePath);
            return;
        }

        logger.LogInformation(
            "Importing '{DocumentId}' into Kernel Memory...", 
            documentId);

        await memory.ImportDocumentAsync(
            filePath: filePath,
            documentId: documentId,
            tags: new TagCollection
            {
                { KnowledgeBaseConstants.Tags.TypeKey, 
                    KnowledgeBaseConstants.Tags.TypeDocumentation },
                { KnowledgeBaseConstants.Tags.ProjectKey,
                    KnowledgeBaseConstants.Tags.ProjectWorkboard },
                { KnowledgeBaseConstants.Tags.AccessLevelKey, 
                    accessLevelTag }
            });

        logger.LogInformation(
            "Document '{DocumentId}' successfully imported!",
            documentId);
    }
}
