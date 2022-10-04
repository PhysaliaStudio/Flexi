namespace Physalia.AbilitySystem.Tests
{
    public static class CustomAbility
    {
        public const string HELLO_WORLD =
            "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
            "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilitySystem.StartNode\"}," +
            "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
            "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
            "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"World!\"}," +
            "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"Hello\"}]," +
            "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
            "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
            "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
            "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
    }
}
