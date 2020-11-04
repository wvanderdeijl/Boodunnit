namespace Interfaces
{
    public interface IHuman : IEntity
    {
        //Human distinct things.
        Dialogue Dialogue
        {
            get;
        }

        Question Question
        {
            get;
        }

        string Name
        {
            get;
        }
    }
}