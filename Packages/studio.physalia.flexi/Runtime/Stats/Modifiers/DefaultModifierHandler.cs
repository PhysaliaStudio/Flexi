namespace Physalia.Flexi
{
    public class DefaultModifierHandler
    {
        private readonly AddendModifierHandler addendModifierHandler = new();
        private readonly MultiplierModifierHandler multiplierModifierHandler = new();

        public void ApplyModifiers(StatOwner statOwner)
        {
            addendModifierHandler.ApplyModifiers(statOwner);
            multiplierModifierHandler.ApplyModifiers(statOwner);
        }
    }
}
