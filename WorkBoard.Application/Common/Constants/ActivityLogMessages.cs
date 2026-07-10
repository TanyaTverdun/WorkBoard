namespace WorkBoard.Application.Common.Constants;

public static class ActivityLogMessages
{
    public static string AttachedFile(string fileName) =>
        $"Attached file '{fileName}'";

    public static string DeletedAttachment(string fileName) =>
        $"Deleted attachment '{fileName}'";

    public static string CreatedCard(string sectionName) =>
        $"Created this card in section '{sectionName}'";

    public static string RenamedCard(string oldTitle, string newTitle) =>
        $"Renamed card from '{oldTitle}' to '{newTitle}'";

    public static string UpdateCardDescription =>
        $"Updated card description";

    public static string MovedCard(string sectionName) => 
        $"Moved card to section '{sectionName}'";

    public static string SetDueDate(DateTime? dueDate) =>
        dueDate != null ? $"Changed due date to {dueDate:yyyy-MM-dd HH:mm}" : "Removed due date";

    public static string AddedAssignee(string userName) =>
        $"Added user {userName} to card assignees";

    public static string RemovedAssignee(string userName) =>
        $"Removed user {userName} from card assignees";

    public static string CreatedChecklist(string name) => 
        $"Created checklist '{name}'";

    public static string DeletedChecklist(string name) => 
        $"Deleted checklist '{name}'";

    public static string RenamedChecklist(string oldName, string newName) => 
        $"Renamed checklist from '{oldName}' to '{newName}'";

    public static string AddedChecklistItem(string title) => 
        $"Added item '{title}' to checklist";

    public static string DeletedChecklistItem(string title) => 
        $"Deleted item '{title}' from checklist";

    public static string RenamedChecklistItem(
        string oldTitle, 
        string newTitle) => 
        $"Renamed checklist item from '{oldTitle}' to '{newTitle}'";

    public static string ChangedChecklistItemStatus(
        string title, 
        bool isDone) =>
        $"Marked checklist item '{title}' as {(isDone ? "completed" : "incomplete")}";

    public const string CreatedComment = 
        "Left a comment";

    public static string AddedLabelToCard(string labelName) => 
        $"Added label '{labelName}' to the card";

    public static string RemovedLabelFromCard(string labelName) => 
        $"Removed label '{labelName}' from the card";
}
