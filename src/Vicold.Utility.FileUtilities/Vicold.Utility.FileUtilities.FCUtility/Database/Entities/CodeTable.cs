using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Database.Entities
{
	[Serializable]
    public class CodeTable
    {
        public ObjectId Id { get; set; }

        public long Code { get; set; }

        public LevelType Level { get; set; } = LevelType.Unset;

        public TypeType Type { get; set; } = TypeType.Unknown;

        public DateTime CreateTime { get; set; } = DateTime.Now;
        
        public DateTime UpdateTime { get; set; } = DateTime.Now;

    }
}
