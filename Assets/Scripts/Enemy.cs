using System.Collections.Generic;

public class Enemy
{
    public List<EnemyState> Pattern { get; }

    public Enemy(List<EnemyState> pattern)
    {
        Pattern = pattern;
    }
}
