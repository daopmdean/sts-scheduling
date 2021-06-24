using System;
namespace sts_scheduling.Data
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Skill obj2 = (Skill)obj;
            return Id == obj2.Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
