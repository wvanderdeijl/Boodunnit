
namespace Interfaces
{
    public interface ISnappable
    {
        bool IsSnapLocationValid();
        void InstantiateNearestSnapLocation();
        void Snap();
    }
}