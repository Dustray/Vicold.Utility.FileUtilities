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
        public ObjectId? Id { get; set; }

        public long Code { get; set; }

        public LevelType Level { get; set; } = LevelType.Unset;

        public TypeType Type { get; set; } = TypeType.Unknown;

        public string? FilePath { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public ObjectId? SellerId { get; set; }

        public IList<ObjectId>? StarIds { get; set; }

        public void Update(CodeTable otherCodeTable)
        {
            this.Code = otherCodeTable.Code;
            this.Level = otherCodeTable.Level;
            this.Type = otherCodeTable.Type;
            this.FilePath = otherCodeTable.FilePath;
            this.UpdateTime = DateTime.Now;
            this.SellerId = otherCodeTable.SellerId;
            this.StarIds = otherCodeTable.StarIds;
        }


        public MovieSeller? GetMovieSeller()
        {
            if (SellerId == null)
            {
                return null;
            }

            return App.Current.DataCore.DB.SearchSellerById(SellerId);
        }

        public IList<MovieStar> GetMovieStars()
        {
            var result = new List<MovieStar>();
            if (StarIds == null || StarIds.Count == 0)
            {
                return result;
            }

            foreach (var id in StarIds)
            {
                var star = App.Current.DataCore.DB.SearchStarById(id);
                if (star != null)
                {
                    result.Add(star);
                }
            }

            return result;
        }
    }
}
