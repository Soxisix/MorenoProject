namespace MorenoSystem.Entities
{
    public class CouncilMembers
    {
        public int Id { get; set; }
        public virtual Student Student { get; set; }
        public virtual CouncilPosition CouncilPosition { get; set; }
        public virtual PartyList PartyList { get; set; }
    }
}