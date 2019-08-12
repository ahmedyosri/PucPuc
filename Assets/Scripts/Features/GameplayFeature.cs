
public class GameplayFeature : Feature
{
    public GameplayFeature(Contexts contexts) : base("Gameplay Feature")
    {
        Add(new GameInitializerSystem(contexts));
        Add(new MovementSystem(contexts));
        Add(new UpdatePositionSystem(contexts));

        Add(new EmitInputSystem(contexts));
        Add(new FireBallSystem(contexts));
        Add(new BoardBallObjectSystem(contexts));
    }
}
