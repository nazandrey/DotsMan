namespace DotsMan
{
    public enum PhysicsLayer
    {
        Player = (1 << 0),
        Wall = (1 << 1),
        Ground = (1 << 2),
        Enemy = (1 << 3)
    }
}