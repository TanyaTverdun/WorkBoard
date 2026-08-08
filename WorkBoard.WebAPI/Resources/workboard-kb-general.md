# User Guide: Managing Boards

## 1. Viewing the Boards List

Boards are displayed in the Sidebar under the "BOARDS" section.

- **Workspace Dependency:** To view the list of boards, you must first select a Workspace. If no workspace is selected, the system will display the message "Select a workspace...".
- **Navigation:** The boards section can be collapsed or expanded by clicking the "BOARDS" header (chevron icon). The active board you are currently viewing is visually highlighted in the list.

## 2. Creating a New Board

To create a new board, you must be an **Owner** or a **Member** of the selected Workspace.

1. Click the **"New Board"** button (plus icon) at the bottom of the boards list.
2. In the "Create New Board" window, enter the Board Name.
    - _Constraint:_ The maximum length for the name is 50 characters.
3. Click "Create" to save changes. During saving, the button's state will change to "Creating...".

## 3. Editing a Board Name

Workspace Owners and Members can change the name of an existing board:

1. Hover your cursor over the desired board in the list.
2. Click the pencil icon (Edit Board).
3. In the "Edit Board" modal window, enter the new name.
4. Click "Save" to save your changes (during processing, it will display "Saving...").

## 4. Deleting a Board

Deleting a board is an **irreversible action** (the data cannot be restored). To delete a board, you must be an **Owner** or a **Member** of the selected Workspace:

1. Hover your cursor over the board in the sidebar and click the trash can icon (Delete Board).
2. A confirmation window will appear on the screen with the warning: _"Are you sure you want to delete the board [Board Name]? This action cannot be undone."_
3. To permanently delete it, click the "Delete" button (while the operation is running, it will display "Deleting..."). If you change your mind, click "Cancel" or close the window.

## 5. Access Permissions and Error Messages

- **Action Visibility Constraints:** The create ("New Board"), edit (pencil), and delete (trash can) buttons are displayed **only** for users who are either an **Owner** or a **Member** of the current workspace. Users with restricted roles, such as **Observers**, will only see the list of boards without management options.
- **Errors:** If a system error occurs during the creation, updating, or deletion of a board, a red pop-up notification describing the issue will appear in the bottom right corner of the screen.

# User Guide: Managing Workspaces

## 1. Viewing the Workspaces List

Workspaces are displayed in the Sidebar under the "WORKSPACES" section.

- **Navigation:** The workspaces section can be collapsed or expanded by clicking the "WORKSPACES" header (chevron icon). The active workspace you are currently in is visually highlighted in the list.
- **Loading State:** While the workspaces are being fetched, the system will display a "loading..." indicator.

## 2. Creating a New Workspace

Any user can create a new workspace.

1. Click the **"New Workspace"** button (plus icon) at the bottom of the workspaces list.
2. In the "Create New Workspace" window, enter the Workspace Name (e.g., Corp Engineering).
    - _Constraint:_ The maximum length for the name is 50 characters.
3. Click "Create" to save changes. During saving, the button's state will change to "Creating...".

## 3. Editing a Workspace Name

To change the name of an existing workspace, you must be the **Owner** of that workspace:

1. Hover your cursor over the desired workspace in the list.
2. Click the pencil icon (Edit Workspace).
3. In the "Edit Workspace" modal window, enter the new name.
4. Click "Save" to save your changes (during processing, it will display "Saving...").

## 4. Deleting a Workspace

Deleting a workspace is an **irreversible action** (the data cannot be restored). To delete a workspace, you must be the **Owner** of that workspace:

1. Hover your cursor over the workspace in the sidebar and click the trash can icon (Delete Workspace).
2. A confirmation window will appear on the screen with the warning: _"Are you sure you want to delete the workspace [Workspace Name]? This action cannot be undone."_
3. To permanently delete it, click the "Delete" button (while the operation is running, it will display "Deleting..."). If you change your mind, click "Cancel" or close the window.

## 5. Access Permissions and Error Messages

- **Action Visibility Constraints:** The edit (pencil) and delete (trash can) buttons are displayed **only** for users who hold the **Owner** role for that specific workspace. Regular members will only see the workspace name and can select it, but management options will be hidden.
- **Errors:** If a system error occurs during the creation, updating, or deletion of a workspace, a red pop-up notification (Toast) describing the issue will appear in the bottom right corner of the screen.

# User Guide: Sidebar Navigation and Profile Menu

## 1. Overview of the Sidebar

The sidebar serves as the main navigation hub for the WorkBoard application. It is permanently visible on the left side of the screen and provides quick access to your profile, search functionality, workspaces, boards, and system tools. At the very top of the sidebar, you can see your current user avatar and full name.

## 2. Searching

The sidebar includes a dedicated search box with the placeholder _"Search boards..."_. You can use this input field to quickly find specific boards within the application.

## 3. Main Navigation Links

Below the search bar, the sidebar contains the following quick access links for system management:

- **Roles & Members:** Access the system's role and member management settings.
- **Archivation Tracker:** Opens the dashboard to monitor the status of your boards (Active, Archiving, Archived, or Restore Pending).
- **Subscriptions:** Access your billing and subscription details.

## 4. Workspaces and Boards Sections

The middle section of the sidebar contains dedicated, collapsible menus for **BOARDS** and **WORKSPACES**. In these sections, you can switch between your workspaces, navigate to specific boards, and use the management buttons (create, edit, delete) if you have the appropriate permissions (such as Owner or Member).

## 5. Profile Menu and Account Actions

At the bottom of the sidebar, there is a **"Profile"** button. Clicking this button opens a pop-up menu with your account details and available actions:

- **Account Details:** The menu displays your current Avatar (photo or colored initials), Full Name, and Email address.
- **Edit Profile:** Clicking the "Edit Profile" button redirects you to the profile settings page, where you can upload a new profile photo or change your default avatar color.
- **Sign Out:** To securely log out of your WorkBoard account, click the red "Sign Out" button located at the bottom of the profile menu.

# User Guide: Using the Kanban Board

## 1. Overview of the Board Layout

The board interface is divided into two main areas:

- **Board Header (Top):** Displays the board name, board members, and filtering/reordering controls.
- **Kanban Container (Bottom):** Displays the vertical sections (columns) and the task cards within them.

## 2. Managing Board Members

The board header displays a group of circular avatars representing the current board members.

- **View Members:** Clicking on the avatar group opens the "Board Members" popover, showing a list of all assigned users, their emails, and their roles.
- **Add a Member:** If you are a Member of Board, you can add new users by clicking the "Add Member" icon. Type an email address in the search box to find a user, assign them a role (Member or Observer), and click "Add".
- **Change Roles:** Users with the **Member** role in Board can change the role of other participants using the dropdown next to their name. _Note: You cannot change your own role, and Observers cannot change any roles._
- **Remove a Member:** Users with the **Member** role in Board can remove a user from the board, click the trash can/remove icon next to their name. The system will ask for confirmation before completing the removal. Observers cannot remove members or change roles.

## 3. Filtering Task Cards

To help you find specific tasks, the board includes a robust filtering system. Click the "Filter" button in the top right of the board header to open the filter popover. You can filter cards by:

- **Member:** Select one or multiple members to see only the cards assigned to them. You can also select "No Assignee" to find unassigned cards.
- **Due Date:** Filter tasks based on their deadlines (e.g., Overdue, Due Soon).
- **Label:** Select specific colored labels to filter tasks by category, or select "No Labels".

When filters are active, the "Filter" button displays the number of active filters in parentheses. You can clear all filters at once by clicking the "X" button next to the filter count.

## 4. Managing Sections (Columns)

Sections represent the stages of your workflow. Users with the **Member** role can manage these sections:

- **Add a Section:** Click the "Add Section" button on the far right of the board. Enter a name (up to 50 characters) and save.
- **Rename a Section:** Click the "More" (three dots) menu in the section header and select "Rename". Type the new name and click the checkmark to save.
- **Delete a Section:** Click the "More" menu and select "Delete". A confirmation button will appear to prevent accidental deletion.
- **Reorder Sections:** Click the "Reorder Sections" button (layered icons) in the top right header. A popover will appear where you can drag and drop the section names to change their order on the board. Click "Apply Order" to save the new layout.
  _Note: Users with the **Observer** role cannot create, rename, delete, or reorder sections._

## 5. Working with Task Cards

Cards represent individual tasks and are displayed within sections.

- **Add a Card:** If you are a Member of Board, click "Add Card" at the bottom of any section. Type a title and confirm to create a new task.(Observers cannot add new cards).
- **Move a Card:** You can drag and drop a card to move it up or down within its current section, or move it to a completely different section. (Observers cannot move cards).
- **View Details:** Clicking anywhere on a card opens its detailed view (Card Details Modal).

## 6. Understanding Card Indicators

The front of a task card displays quick information without needing to open it:

- **Title & Labels:** The name of the task and any colored categorizations.
- **Due Date:** If a deadline is set, it appears here. Overdue dates are highlighted in red with an error icon, while upcoming dates are orange with a clock icon.
- **Checklist Progress:** If the task contains a checklist, a progress bar shows how many items are completed versus the total (e.g., 2/5).
- **Attachments & Comments:** Icons at the bottom left indicate if the card contains files (paperclip icon) or comments (speech bubble icon).
- **Assignees:** Avatars at the bottom right show who is responsible for completing the task.

# User Guide: Managing Task Cards (Card Details)

## 1. Overview and Basic Editing

Clicking on any card on the Kanban board opens the **Card Details Modal**. This view contains all the information and settings related to a specific task.

- **Section Status:** The top left corner displays the current column/section the card belongs to.
- **Title & Description:** To edit the card's title or description, simply click on the text. This will switch the field into edit mode, allowing you to type. Once finished, click **"Save"** to apply changes or **"Cancel"** to discard them.
- _Note: Users with the **Observer** role cannot edit the title or description._

## 2. Managing Due Dates and Assignees

On the right-side panel of the card, you can manage schedules and responsibilities.

- **Due Date:** Click the "Due Date" field to open a calendar picker and set a deadline.
    - If the date is in the future, it is highlighted in a warning color (orange).
    - If the date is overdue, it is highlighted in red.
    - To remove a due date, click the "X" icon next to it.
- **Assignees:** You can assign team members to the card. Click **"Add member"** to open a search popover. You can search by name or email and click a user to assign or unassign them. To quickly remove an assignee, click the "X" on their name tag.
- _Note: **Observers** can view dates and assignees but cannot add, change, or remove them._

## 3. Using Labels

Labels are colored tags used to categorize tasks. They appear at the top of the card details.

- **Add/Remove Labels:** Click the **"Label"** button to open the labels menu. You can search for existing labels and click them to toggle them on or off for the current card.
- **Create New Labels:** Inside the label menu, click **"New Label"**. Enter a name, pick a color from the color palette, and click **"Create"**.
- **Edit/Delete Labels:** Hover over an existing label in the menu to reveal the Edit (pencil) and Delete (trash can) icons. Deleting a label removes it from all cards on the board.
- _Note: **Observers** can view labels applied to the card but cannot open the label menu, create, edit, or remove labels._

## 4. Checklists

Checklists help break down a task into smaller actionable steps.

- **Add a Checklist:** Click **"Add checklist"** to create a new list.
- **Progress Tracking:** The checklist displays a visual progress bar and a fraction (e.g., 2/5) showing how many items are completed.
- **Manage Items:** Click **"Add item"** to add a new task to the list. You can check/uncheck items, edit their text (using the pencil icon), or delete them (using the trash can icon).
- **Edit/Delete Checklist:** You can rename the entire checklist by clicking the edit icon next to its title. To remove the whole checklist, click the delete icon and confirm.
- _Note: **Observers** can only view the checklist and its progress. They cannot check off items, add items, or delete the checklist._

## 5. Attachments

You can attach files to a card for quick access.

- **Upload Files:** Click **"Upload file"** to browse your computer and attach a document or image.
- **View Files:** Attached files are listed with their format icon, name, and size. Clicking on a file opens it in a new browser tab.
- **Delete Files:** To remove a file, click the "X" icon next to it and click "Confirm" when prompted.
- _Note: **Observers** can view and download attachments but cannot upload or delete them._

## 6. Communication and History

The bottom section of the card contains a tabbed interface for tracking communication and changes.

- **Comments Tab:** Users can write comments to communicate with team members. Type your message in the input field at the bottom and click the send icon. Your own comments are visually aligned differently and labeled as "You" for easy reading.
- **Activity Log Tab:** This tab automatically records a history of changes made to the card (e.g., who changed the status, added a label, or changed the due date) along with timestamps.
- _Note: **Observers** are allowed to view the Activity Log, read comments but not post comments._

## 7. Deleting a Card

If a task is no longer needed, it can be deleted entirely.

- Click the red **"Delete Card"** button at the bottom of the right-side panel.
- The button will change to ask **"Confirm Delete?"**. Click it again to permanently delete the card.
- _Note: Deleting a card is irreversible. **Observers** do not see the delete button and cannot delete cards._

# User Guide: Archivation Status Tracker

## 1. Overview of the Archivation Tracker

The Archivation Status Tracker is a dedicated page for monitoring and managing the lifecycle of your boards. You can access it via the "Archivation Tracker" link in the main Sidebar.

- **Board Visibility:** This page displays **only the boards that you have permission to manage** (specifically, boards belonging to workspaces where you are an **Owner** or a **Member**). Boards from spaces where you do not have management rights are not shown here.
- **Summary Cards:** The top of the page features summary cards displaying the total count of your manageable boards currently in each status.

## 2. Archivation Statuses Explained

Boards in the system can be in one of four states:

- **Active:** The board is currently in use, and you can interact with it normally. These boards are marked with a blue "ACTIVE" badge.
- **Archiving:** The board is in the process of being archived. It is temporarily locked. These boards are marked with a yellow "ARCHIVING" badge.
- **Archived:** The board has been successfully archived. It is removed from your regular workspace view. These boards are marked with a green "ARCHIVED" badge.
- **Restore Pending:** The board is currently being restored from the archive back to an active state. These boards are marked with an orange "RESTORE PENDING" badge.

## 3. Filtering Boards

You can quickly find boards by their status using the summary cards or the dropdown filter:

- **Summary Cards:** Click on any of the four summary cards at the top of the screen (Active, Archiving, Archived, Restore Pending) to instantly filter the list below. The selected card will be highlighted.
- **Dropdown Filter:** Use the dropdown menu located on the right side above the board list. You can select a specific status or choose "All" to view every board regardless of its state. The page will also indicate how many boards you are currently viewing out of the total.

## 4. Managing Board Lifecycle (Actions)

Each board in the list displays its Name, Workspace Name, Status Badge, and an action button on the far right. The available actions depend on the board's current status:

- **To Archive a Board:** If a board is "Active", an **"Archive"** button will be visible on the right. Clicking this will initiate the archiving process.
- **To Restore a Board:** If a board is "Archived", a **"Restore"** button will be visible. Clicking this will initiate the restoration process, moving the board back to your active workspace.
- **In Progress States:** If a board is currently "Archiving" or "Restore Pending", no action buttons will be available. Instead, you will see a status text such as _"In progress..."_ or _"Processing..."_ indicating that the system is working on your request.

# User Guide: Profile Settings

## 1. Overview of the Profile Page

The Profile page allows you to view and customize your account's personal information and visual appearance. It displays your current Avatar, Full Name, and Email address at the top.

## 2. Managing Profile Photos

You can upload a custom profile image to replace your default color avatar:

- **Uploading a Photo:** Click the **"Upload file"** button. The system accepts image files in `.png`, `.jpg`, and `.jpeg` formats.
- **Preview & Changes:** Once a new photo is uploaded, it will be displayed as your avatar preview.
- _Note:_ Until you click "Save changes", uploading a photo will temporarily disable the live image preview toggle to indicate unsaved changes.

## 3. Customizing Avatar Color

If you do not set a custom photo, the system displays your initials on a colored background. You can customize this color:

- **Predefined Swatches:** Click on any of the color swatches provided in the list to select a predefined accent color for your avatar.
- **Custom Color Picker:** Click on the rainbow/add swatch to open the advanced color picker popover. You can pick any custom shade you prefer using the static color picker interface.
- **Live Preview:** As soon as you select a new color swatch or custom color, the avatar preview updates immediately on the screen. However, these changes are staged and will only be permanently saved when you click **"Save changes"**.
- A checkmark icon indicates the currently selected color swatch.

## 4. Saving or Canceling Changes

Whenever you modify your profile photo or avatar color, an action bar will appear at the bottom of the page indicating **"HasUnsavedChanges"**:

- **Save Changes:** Click **"Save changes"** to permanently apply your new profile photo or color settings.
- **Cancel Changes:** Click **"Cancel"** to revert all modifications back to their last saved state.
