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
        
        await _controller.ListenForInvitationsAsync();
    }

    private void ShowInvitationPopup(string sender)
    {
        Task.Run(async () =>
        {
            var accept = AnsiConsole.Confirm($"Has recibido una invitación de [bold]{sender}[/]. ¿Aceptarla?");
            if (accept)
            {
                await _controller.AcceptInvitationAsync(sender);
            }
        });
    }
}