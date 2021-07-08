using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sts_scheduling.Data
{
    public class SkillStaff
    {
        public int SkillId { get; set; }
        public int Level { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            
            if (obj == null )
            {
                return false;
            }
            if (obj.GetType() == typeof(SkillStaff))
            {
                return SkillId == ((SkillStaff)obj).SkillId;
            }

            if (obj.GetType() ==  typeof(Skill))
            {
                return SkillId == ((Skill)obj).Id;
            }

            if (obj.GetType() == typeof(int))
            {
                return SkillId == ((int)obj);
            }

            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }   
    }
}
