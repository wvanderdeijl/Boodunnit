namespace Interfaces
{
    public interface IEntity : IEmotion
    {
        //Dialogue Dialogue
        //{
        //    get;
        //}

        void CheckSurroundings();
        
        void UseFirstAbility();

        void UseSecondAbility();
    }
}