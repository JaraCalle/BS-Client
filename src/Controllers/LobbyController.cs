public class LobbyController(Router router) : IController
{
    public async Task ExecuteAsync()
    {
        await router.NavigateTo<LobbyController>();
    }

    public async Task NavigateToInviteUsers()
    {
        await router.NavigateTo<InviteUsersController>();
    }

    public async Task NavigateToReceiveInvitations()
    {
        await router.NavigateTo<ReceiveInvitationsController>();
    }
}