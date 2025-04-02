public class LobbyController : IController
{
    private readonly Router _router;

    public LobbyController(Router router)
    {
        _router = router;
    }
    
    public async Task ExecuteAsync()
    {
        await _router.NavigateTo<LobbyController>();
    }

    public async Task NavigateToInviteUsers()
    {
        await _router.NavigateTo<InviteUsersController>();
    }

    public async Task NavigateToReceiveInvitations()
    {
        await _router.NavigateTo<ReceiveInvitationsController>();
    }
}