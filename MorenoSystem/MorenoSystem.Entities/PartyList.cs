using System.Collections.Generic;

namespace MorenoSystem.Entities
{
    public class PartyList
    {
        public PartyList()
        {
            CouncilMembers = new HashSet<CouncilMembers>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CouncilMembers> CouncilMembers { get; set; }
    }
}