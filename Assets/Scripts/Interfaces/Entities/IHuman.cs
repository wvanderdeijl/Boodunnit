namespace Interfaces
{
    public enum NPCCharacter
    { 
        Emmie ,
        SirBoonkle,
        BusinessTycoon,
        Policeman

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

        NPCCharacter Character
        {
            get;
            set;
        }
    }
}