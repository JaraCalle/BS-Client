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

        _users = await _controller.GetUserListAsync();

        while (true)
        {
            if (_users.Length == 0)
            {
                AnsiConsole.MarkupLine("[red]No hay jugadores disponibles...[/]");
                await Task.Delay(2000);
                return;
            }

            var user = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[bold green]Selecciona el usuario que deseas invitar[/]")
                .PageSize(10)
                .AddChoices(_users)
            );

            await _controller.InviteUserAsync(user);
        }
    }
}