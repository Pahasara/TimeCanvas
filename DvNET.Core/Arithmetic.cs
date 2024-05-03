namespace DvNET.Core
{
    public class Arithmetic
    {
        public static float GetPercentage(float progress, float total)
        {
            if (total == 0) { return 0; }

            float percentage = progress / total * 100;

            return percentage;
        }
    }
}
