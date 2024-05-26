namespace Framework.Player
{
    public interface IPlayerMovement
    {
        public float InitialMoveSpeed { get; set; }
        public float CurrentMoveSpeed { get; set; }
        public float SlidingFactor { get; set; }
        public float DirectionChangeSpeed { get; set; }
    }
}