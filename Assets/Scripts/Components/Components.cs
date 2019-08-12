using Entitas;
using Entitas.CodeGeneration.Attributes;
using System.Collections.Generic;
using UnityEngine;

[Game]
public class PositionComponent : IComponent
{
    public Vector2 pos;
}

[Game]
public class VelocityComponent : IComponent
{
    public Vector2 direction;
}

[Game]
public class GameObjectComponent : IComponent
{
    public GameObject gameobject;
}



[Game]
public class BallComponent : IComponent
{
    public string value;
}

[Game]
public class MovingComponent : IComponent
{
}

[Game]
public class TargetPositionComponent : IComponent
{
    public Vector2 position;
}

[Game]
public class ReachedTargetComponent : IComponent
{
}

[Game]
public class PrimaryBall : IComponent
{ }

[Game]
public class SecondaryBall : IComponent
{ }

[Game]
public class BoardBall : IComponent
{
    public Vector2 boardIdx;
    public int value;
    public bool shifted;
}

[Game, Unique]
public class BoardManager : IComponent
{
    public GameEntity[,] entities;
    public const int width = 6;
    public const int length = 10;

    public BoardManager()
    {
        entities = new GameEntity[length, width];
    }
}

[Input, Unique]
public class MouseLeftComponent : IComponent
{
}

[Input]
public class MouseDownComponent : IComponent
{
    public Vector2 position;
}

[Input]
public class MousePressedComponent : IComponent
{
    public Vector2 position;
}

[Input]
public class MouseUpComponent : IComponent
{
    public Vector2 position;
}