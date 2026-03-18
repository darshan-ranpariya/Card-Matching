public interface IGameEventHandler
{
    void OnItemFlipped(Item item);
    void OnItemDestroyed(int siblingIndex);
}
