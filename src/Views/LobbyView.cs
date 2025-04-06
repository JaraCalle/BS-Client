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
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold yellow1]BATTLESHIP - LOBBY[/]").Border(BoxBorder.Heavy).Centered());

            var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[bold green]Selecciona una opción:[/]")
                .AddChoices("Invitar Usuarios", "Ver Invitaciones", "Salir")
            );

            switch (option)
            {
                case "Invitar Usuarios":
                    await _controller.NavigateToInviteUsers();
                    break;
                case "Ver Invitaciones":
                    await _controller.NavigateToReceiveInvitations();
                    break;
                case "Salir":
                    return;
            }
        }
    }
}