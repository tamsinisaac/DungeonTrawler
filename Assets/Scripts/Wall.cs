public class Wall
{

    public int x;
    public int y;
    public int length;

    public Direction direction;
    public enum Direction { Vertical, Horizontal };

    public Wall(int x2, int y2, int len, Direction dir)
    {
        x = x2;
        y = y2;
        length = len;
        direction = dir;
    }

}
