using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class BallMovementSystem : IExecuteSystem
{
    GameContext gameContext;
    IGroup<GameEntity> firedBallEntities;

    float speed = 15.0f;
    //float borderLimit = 2.5f;
    Vector3 pos;
    Vector3 vel;

    public BallMovementSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        firedBallEntities = gameContext.GetGroup(GameMatcher.Moving);
    }

    public void Execute()
    {
        foreach(var e in firedBallEntities.GetEntities())
        {
            pos = new Vector3(e.position.pos.x, e.position.pos.y);
            vel = e.targetPositions.positions[0] - pos;
            vel.Normalize();

            e.ReplacePosition(pos + vel * Time.deltaTime * speed);

            if(Vector3.Distance(pos, e.targetPositions.positions[0]) < 0.2f)
            {
                e.ReplacePosition(e.targetPositions.positions[0]);
                e.targetPositions.positions.RemoveAt(0);
            }

            if(e.targetPositions.positions.Count == 0)
            {
                e.ReplaceBoardBall(e.boardBall.boardIdx, e.boardBall.value, e.boardBall.shifted);
                e.isMoving = false;
                gameContext.boardManager.entities[(int)e.boardBall.boardIdx.x, (int)e.boardBall.boardIdx.y] = e;
            }
        }
    }

}
