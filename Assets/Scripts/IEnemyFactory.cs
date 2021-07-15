using System.Collections.Generic;

public interface IEnemyFactory
{
    public int NOfStates { get; }

    public Enemy GenerateEnemy(Map map, List<Enemy> enemies);
}
