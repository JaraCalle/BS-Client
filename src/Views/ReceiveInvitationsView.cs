using Spectre.Console;

public class ReceiveInvitationsView : IView
{
    private ReceiveInvitationsController _controller = null!;

    public void SetController(IController controller)
    {
        _controller = (ReceiveInvitationsController)controller;
        _controller.OnInvitationReceived += ShowInvitationPopup;
    }

    public async Task RenderAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold yellow1]BATTLESHIP - ESPERANDO INVITACIONES[/]").Border(BoxBorder.Heavy).Centered());
        
        var panel = new Panel("[cyan]Recibiendo todas las invitaciones entrantes[/]")
            .Border(BoxBorder.Double)
            .Expand();

        AnsiConsole.Write(panel);
        
        await _controller.ListenForInvitationsAsync();
    }

    private void ShowInvitationPopup(string sender)
    {
        Task.Run(async () =>
        {
            var accept = AnsiConsole.Confirm($"Has recibido una invitación de [bold green1]{sender}[/]. ¿Aceptarla?");
            await _controller.AcceptInvitationAsync(sender, accept);
        });
    }
}