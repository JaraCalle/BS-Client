using Spectre.Console;

public class LobbyView : IView
{
    private LobbyController _controller = null!;

    public void SetController(IController controller)
    {
        _controller = (LobbyController)controller;
    }

    public async Task RenderAsync()
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold yellow1]BATTLESHIP - LOBBY[/]")
            .Border(BoxBorder.Heavy).Centered();
        AnsiConsole.Write(rule);
        
        var usersList = await _controller.ListUsersAsync();
        
        var user = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title($"[bold green]Usuarios conectados[/]")
            .PageSize(10)
            .AddChoices(usersList)
        );
        
        await _controller.InviteUserAsync(user);
    }
}