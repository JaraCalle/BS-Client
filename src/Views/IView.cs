public interface IView
{
    Task RenderAsync();
    void SetController(IController controller);
}