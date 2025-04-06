using Spectre.Console;

public class InviteUsersView : IView
{
    private InviteUsersController _controller = null!;
    private string[] _users = [];

    public void SetController(IController controller)
    {
        _controller = (InviteUsersController)controller;
    }

    public async Task RenderAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold yellow]BATTLESHIP - INVITAR JUGADORES[/]").Border(BoxBorder.Heavy).Centered());
        
        while (true)
        {
            _users = await _controller.GetUserListAsync();
            _users = _users
                .Append("[yellow]Refrescar lista de usuarios[/]")
                .Append("[yellow]Volver al menú[/]")
                .ToArray();
            
            var user = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[bold green]Selecciona el usuario que deseas invitar[/]")
                .PageSize(10)
                .AddChoices(_users)
            );
            
            switch (user)
            {
                case "[yellow]Refrescar lista de usuarios[/]":
                    break;
                case "[yellow]Volver al menú[/]":
                    await _controller.NavigateToLobby();
                    break;
                default:
                    await _controller.InviteUserAsync(user);
                    break;
            }
        }
    }
}