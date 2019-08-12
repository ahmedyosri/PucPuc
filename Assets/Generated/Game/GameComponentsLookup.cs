//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentLookupGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public static class GameComponentsLookup {

    public const int Ball = 0;
    public const int BoardBall = 1;
    public const int BoardManager = 2;
    public const int GameObject = 3;
    public const int Moving = 4;
    public const int Position = 5;
    public const int PrimaryBall = 6;
    public const int ReachedTarget = 7;
    public const int SecondaryBall = 8;
    public const int TargetPosition = 9;
    public const int Velocity = 10;

    public const int TotalComponents = 11;

    public static readonly string[] componentNames = {
        "Ball",
        "BoardBall",
        "BoardManager",
        "GameObject",
        "Moving",
        "Position",
        "PrimaryBall",
        "ReachedTarget",
        "SecondaryBall",
        "TargetPosition",
        "Velocity"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(BallComponent),
        typeof(BoardBall),
        typeof(BoardManager),
        typeof(GameObjectComponent),
        typeof(MovingComponent),
        typeof(PositionComponent),
        typeof(PrimaryBall),
        typeof(ReachedTargetComponent),
        typeof(SecondaryBall),
        typeof(TargetPositionComponent),
        typeof(VelocityComponent)
    };
}
