namespace Interfaces
{
    public interface IEntity : IEmotion
    {
        void CheckSurroundings();
        
        void UseFirstAbility();

        void UseSecondAbility();
    }
}