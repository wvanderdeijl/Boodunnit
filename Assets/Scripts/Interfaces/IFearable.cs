using DefaultNamespace.Enums;

namespace Interfaces
{
    public interface IFearable
    {
        float Duration { get; set; }
        TerrifyState TerrifyState { get; set; }
        
        void Fear();
    }
}