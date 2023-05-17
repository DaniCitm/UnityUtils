public static class ExtensionMethods
{
    public static float Remap(this float input, float inputMin, float inputMax, float min, float max)
    {
        return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
    }
    public static float Remap01(this float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
}