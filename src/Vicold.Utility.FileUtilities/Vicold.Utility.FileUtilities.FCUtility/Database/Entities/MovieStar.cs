using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Database.Entities
{
    [Serializable]
    public class MovieStar
    {
        public ObjectId? Id { get; set; }

        public string? Name { get; set; }

        public IList<string>? SubName { get; set; }

        public int Level { get; set; }

        public IList<string> GetNames()
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(Name))
            {
                result.Add(Name);
            }

            if (SubName != null && SubName.Count > 0)
            {
                result.AddRange(SubName);
            }

            return result;
        }
    }
}
