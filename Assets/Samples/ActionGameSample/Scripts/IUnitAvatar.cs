namespace Physalia.Flexi.Samples.ActionGame
{
    public interface IUnitAvatar
    {
        bool HasMontage(string name);
        void PlayMontage(string name);
        bool IsMontagePlayedAndFinished(string name);

        void Move(float x, float z);
    }
}
