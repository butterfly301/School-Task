using UnityEngine;

public class MinionEnemy : Enemy
{
    public override void OnRequestedFromPool()
    {
        Set();
    }
}
