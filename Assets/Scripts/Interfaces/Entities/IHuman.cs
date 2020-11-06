namespace Interfaces
{
    public enum Proffesion 
    { 
        Policeman,
        Baker,
        Beekeeper,
        Fisherman
    };
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

        Proffesion Proffesion
        {
            get;
        }
    }
}