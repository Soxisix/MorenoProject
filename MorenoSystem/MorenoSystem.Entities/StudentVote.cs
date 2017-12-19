namespace MorenoSystem.Entities
{
    public class StudentVote
    {
        public int Id { get; set; }
        public virtual CouncilPosition Position { get; set; }
        public virtual Student VotedStudent { get; set; }
        public virtual Student Student { get; set; }    
        public virtual PartyList PartyList { get; set; }
    }
}