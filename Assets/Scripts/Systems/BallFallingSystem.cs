using Entitas;
using UnityEngine;

public class BallFallingSystem : IExecuteSystem
{
    GameContext gameContext;
    IGroup<GameEntity> fallingEntities;
    Vector2 tmpV;
    public BallFallingSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        fallingEntities = gameContext.GetGroup(GameMatcher.Fall);
    }

    public void Execute()
    {
        GameEntity[] entities = fallingEntities.GetEntities();
        foreach(GameEntity e in entities)
        {
            e.fall.initialForce.y -= 30 * Time.deltaTime*e.fall.gravityMultiplier;
            tmpV = e.fall.initialForce * Time.deltaTime;
            tmpV += e.position.pos;
            e.gameObject.gameobject.transform.Rotate(Vector3.back, e.fall.initialForce.x*Time.deltaTime*30);
            e.ReplacePosition(tmpV);

            if (e.position.pos.y < -7)
            {
                GameplayManager.Instance.DeleteBall(e.gameObject.gameobject);
                e.RemoveFall();
                e.Destroy();
            }
        }
    }

}
