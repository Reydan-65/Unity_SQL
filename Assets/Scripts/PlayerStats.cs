[System.Serializable]
public class PlayerStats
{
    public int Gold;
    public int Level;

    public PlayerStats() { }

    public PlayerStats(int gold, int level)
    {
        Gold = gold;
        Level = level;
    }
}
