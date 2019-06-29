namespace NastyEngine
{
    public class GTime
    {
        public static float Time { get; private set; }

        public static float Delta;

        public static void UpdateTime(float delta)
        {
            Time += delta;
        }
    }
}
