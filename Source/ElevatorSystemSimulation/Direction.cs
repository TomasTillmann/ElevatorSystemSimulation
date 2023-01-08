namespace ElevatorSystemSimulation
{
    public enum Direction
    {
        Up = 1,
        NoDirection = 0,
        Down = -1,
    }

    public static class DirectionExtensions
    {
        public static Direction ToOpposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;

                case Direction.Down:
                    return Direction.Up;

                case Direction.NoDirection:
                    return Direction.NoDirection;
            }

            throw new Exception("Direction problem");
        }
    }
}
