using System.Collections.Generic;

public class Enemy
{
    public List<EnemyState> Pattern { get; }

    public Enemy(List<EnemyState> pattern)
    {
        Pattern = pattern;
    }

    public override string ToString()
    {
        string s = "Enemy\n" + "# of states = " + Pattern.Count + "\n";

        for (int i = 0; i < Pattern.Count; i++)
        {
            EnemyState enemyState = Pattern[i];

            s += (i+1) + ") pos=" + enemyState.Position + ", rot=" + enemyState.Rotation + "°, vislength=" + enemyState.VisionLength + "\n";
        }

        return s;
    }
}
