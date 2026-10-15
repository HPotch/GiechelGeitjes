public static class Util 
{
    public static float Remap(this float value, float fromSource, float toSource, float fromTarget, float toTarget) 
    {
        return fromTarget + (value - fromSource) * (toTarget - fromTarget) / (toSource - fromSource);
    }
}