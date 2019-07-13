public class Player
{
    public static Player CurrentPlayer { get; private set; } = null;

    public static Player CreatePlayer()
    {
        return new Player();
    }

    public static Player LoadPlayer()
    {
        return new Player();
    }


    public Inventory Inventory { get; private set; }


    private Player()
    {
        Inventory = new Inventory();
    }
}
