namespace Physalia.Flexi.Samples.CardGame
{
    public interface IUnitData : IHasGameId
    {
        string Name { get; }
        UnitAvatarAnimation AvatarPrefab { get; }

        UnitType UnitType { get; }
    }
}
