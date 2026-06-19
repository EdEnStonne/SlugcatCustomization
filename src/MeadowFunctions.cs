using RainMeadow;

namespace Resize.MeadowCompat;
public static class MeadowFunc
{
    public static bool IsLocal(AbstractPhysicalObject abstractPhysicalObject)
    {
        return abstractPhysicalObject.IsLocal();
    }
    public static bool IsLocal(PhysicalObject physicalObject)
    {
        return physicalObject.IsLocal();
    }
}