
public class GameplayFeature : Feature
{
    public GameplayFeature(Contexts contexts) : base("Gameplay Feature")
    {
        Add(new GameInitializerSystem(contexts));
        Add(new AutoDestroySystem(contexts));
        Add(new BallCreatorSystem(contexts));

        Add(new UpdatePositionSystem(contexts));
        Add(new EmitInputSystem(contexts));
        Add(new MoveToTargetSystem(contexts));

        Add(new FireBallSystem(contexts));
        Add(new BoardBallObjectSystem(contexts));
        Add(new AimVisualizerSystem(contexts));
        Add(new BallColliderSystem(contexts));

        Add(new BallAdditionSystem(contexts));
        Add(new BallMergeStartingSystem(contexts));
        Add(new BallMergingSystem(contexts));
        Add(new BallFallingSystem(contexts));
        Add(new BoardBalancerSystem(contexts));
        
        Add(new PreparePrimaryBallSystem(contexts));
    }
}
