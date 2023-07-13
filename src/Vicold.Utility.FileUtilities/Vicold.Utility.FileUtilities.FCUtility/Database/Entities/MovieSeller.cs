using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Database.Entities
{
    [Serializable]
    public class MovieSeller
    {
        public ObjectId Id { get; set; }

        public string? Name { get; set; }

        public int Level { get; set; }
    }
}
