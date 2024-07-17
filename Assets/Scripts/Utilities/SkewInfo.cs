
[System.Serializable]
public struct SkewInfo
{
    public float curve;
    public float shear;
    public float xOverridePos;
    public float yOverridePos;
    public static SkewInfo Default => new(30, 10);

    public SkewInfo(float curve = -1, float shear = -1, float xOverride = 0, float yOverride = 0)
    {
        this.curve = curve;
        this.shear = shear;
        this.xOverridePos = xOverride;  
        this.yOverridePos = yOverride;
    }

    public readonly bool PositionChanged()
    {
        return (xOverridePos != 0 || yOverridePos != 0);
    }
}
