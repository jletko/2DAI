namespace Examples.Chase
{
    public class HunterAgent : ChaseAgentBase
    {
        protected override string[] GetDetectableObjectTags()
        {
            return new[] { Tags.OBSTACLE, Tags.HUNTED, Tags.HUNTER, Tags.HUNTER_HEAD };
        }
    }
}