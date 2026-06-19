
using Resize.MeadowCompat;

namespace Resize;
public static class RSFunc
{
    public static bool IsLocal(AbstractPhysicalObject abstractPhysicalObject)
    {
        if (Plugin.meadowEnabled && abstractPhysicalObject != null)
        {
            return MeadowFunc.IsLocal(abstractPhysicalObject);
        }
        return true;
    }
    public static bool IsLocal(PhysicalObject physicalObject)
    {
        if (Plugin.meadowEnabled && physicalObject != null)
        {
            return MeadowFunc.IsLocal(physicalObject);
        }
        return true;
    }
}