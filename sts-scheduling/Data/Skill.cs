using System;
namespace sts_scheduling.Data
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {

            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(SkillStaff))
            {
                return this.Id == ((SkillStaff)obj).Id;
            }

            if (obj.GetType() == typeof(Skill))
            {
                return this.Id == ((Skill)obj).Id;
            }

            if (obj.GetType() == typeof(int))
            {
                return this.Id == ((int)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
