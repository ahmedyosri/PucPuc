
public class GameplayFeature : Feature
{
    public GameplayFeature(Contexts contexts) : base("Gameplay Feature")
    {
        Add(new GameInitializerSystem(contexts));
        Add(new BallCreatorSystem(contexts));
        Add(new BallMovementSystem(contexts));
        Add(new UpdatePositionSystem(contexts));

        Add(new EmitInputSystem(contexts));
        Add(new FireBallSystem(contexts));
        Add(new BoardBallObjectSystem(contexts));
        Add(new AimVisualizerSystem(contexts));
        Add(new BallColliderSystem(contexts));
    }
}
