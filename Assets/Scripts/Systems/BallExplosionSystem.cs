using Entitas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BallExplosionSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;
    IGroup<GameEntity> explodingGroup;
    List<GameEntity> explodingEnts;
    SpriteRenderer spriteComp;

    public BallExplosionSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
        explodingGroup = gameContext.GetGroup(GameMatcher.Exploding);
        explodingEnts = new List<GameEntity>();
    }
    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Exploding));
    }

    protected override void Execute(List<GameEntity> entities)
    {
        GameEntity e = entities[0];
        e.isExploding = false;

        if (spriteComp == null)
        {
            spriteComp = e.gameObject.gameobject.GetComponent<SpriteRenderer>();
        }

        if (spriteComp.color != Color.white)
        {
            spriteComp.color = Color.Lerp(spriteComp.color, Color.white, Time.deltaTime * 20);
            e.isExploding = true;
            return;
        }
        float addedScore = 2000;
        var ps = GameplayManager.Instance.explosionPS;
        ps.transform.position = e.position.pos;
        ps.Play();

        GameEntity[,] boardEntities = gameContext.boardManager.entities;
        var neighborIdxs = GameUtils.GetNeighborsFor(e);
        foreach (var nIdx in neighborIdxs)
        {
            int x = (int)(e.boardBall.boardIdx.x + nIdx.x);
            int y = (int)(e.boardBall.boardIdx.y + nIdx.y);
            var n = boardEntities[x, y];
            if (n == null)
                continue;
            addedScore += Mathf.Pow(2, n.boardBall.value);
            n.gameObject.gameobject.GetComponent<ParticleSystem>().Play();
            Vector3 dir = n.position.pos - e.position.pos;
            dir.Normalize();
            n.isAddToBoard = false;
            n.ReplaceFall(dir*20, 2);
            boardEntities[x, y] = null;
        }

        boardEntities[(int)e.boardBall.boardIdx.x, (int)e.boardBall.boardIdx.y] = null;
        spriteComp = null;
        e.isExploding = false;
        GameplayManager.Instance.DeleteBall(e.gameObject.gameobject);
        e.Destroy();
        GameplayManager.Instance.AddScore(addedScore);
        GameUtils.CheckForFallingEntities();
    }
}

