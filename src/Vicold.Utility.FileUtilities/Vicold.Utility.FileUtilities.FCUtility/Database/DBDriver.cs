using LibVLCSharp.Shared;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Linq;
using Vicold.Utility.FileUtilities.FCUtility.Database.Entities;

namespace Vicold.Utility.FileUtilities.FCUtility.Database
{
    internal class DBDriver : IDisposable
    {
        public static readonly string ConnectionString = AppDomain.CurrentDomain.BaseDirectory + @"data\\data.db";
        private static readonly string _codeTable = "CodeTable";
        private static readonly string _movieSellerTable = "MovieSellerTable";
        private static readonly string _movieStarTable = "MovieStarTable";
        private LiteDatabase _db;
        internal DBDriver()
        {
            _db = new LiteDatabase(ConnectionString);
            BsonMapper.Global.IncludeNonPublic = true;
        }

        public bool HasDataChanged { get; private set; } = false;

        #region CodeTable

        public int CountAll()
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            return data.Count();
        }

        public int Count(LevelType levelType, TypeType typeType)
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            return data.Count(v => v.Level == levelType && v.Type == typeType);
        }

        public IEnumerable<CodeTable>? SearchAll()
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            var result = data.FindAll();
            return result.ToList();
        }

        public IEnumerable<CodeTable>? SearchAllOld(DateTime dateTime)
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            var result = data.Find(x => x.UpdateTime < dateTime);
            return result.ToList();
        }

        public CodeTable? Search(long code)
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            var result = data.FindOne(v => v.Code == code);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<CodeTable> SearchLike(string name)
        {
            var result = new List<CodeTable>();
            if (long.TryParse(name, out var code))
            {
                var table1 = Search(code);
                if (table1 is { })
                {
                    result.Add(table1);
                }

            }

            var table = _db.GetCollection<CodeTable>(_codeTable);

            var sellerData = SearchSellers(name);
            if (sellerData is { })
            {

                foreach (var item in sellerData)
                {
                    if (item.Id is not { })
                    {
                        continue;
                    }

                    var codeTable = table.Find(v => v.SellerId == item.Id);
                    if (codeTable is { })
                    {
                        result.AddRange(codeTable);
                    }
                }
            }

            var starData = SearchStars(name);
            if (starData is { })
            {
                foreach (var item in starData)
                {
                    if (item.Id is not { })
                    {
                        continue;
                    }

                    var codeTable = table.FindAll().Where(v => ContainsIn(v.StarIds, item.Id));
                    if (codeTable is { })
                    {
                        result.AddRange(codeTable);
                    }
                }
            }

            return result;
        }




        public CodeTable? Search(CodeTable table)
        {
            return Search(table.Code);
        }

        public IEnumerable<CodeTable>? Search(LevelType levelType)
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            var result = data.Find(v => v.Level == levelType);
            return result;
        }

        public IEnumerable<CodeTable>? Search(TypeType typeType)
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            var result = data.Find(v => v.Type == typeType);
            return result;
        }

        public void Update(CodeTable codeTable)
        {
            var existedTable = Search(codeTable);
            if (existedTable is null)
            {
                throw new Exception("没有找到对应的CodeTable");
            }

            existedTable.Level = codeTable.Level;
            existedTable.Type = codeTable.Type;
            existedTable.UpdateTime = DateTime.Now;
            existedTable.FilePath = codeTable.FilePath;
            var data = _db.GetCollection<CodeTable>(_codeTable);
            data.EnsureIndex(x => x.Code, true);
            data.Update(existedTable);
            HasDataChanged = true;
        }

        //public void Update(long code, LevelType levelType, TypeType typeType)
        //{
        //    Update(new CodeTable()
        //    {
        //        Code = code,
        //        Level = levelType,
        //        Type = typeType,
        //    });
        //}

        public void Insert(CodeTable codeTable)
        {
            var data = _db.GetCollection<CodeTable>(_codeTable);
            codeTable.UpdateTime = DateTime.Now;
            data.EnsureIndex(x => x.Code, true);
            data.Insert(codeTable);
            HasDataChanged = true;
        }

        public void Insert(long code, LevelType levelType, TypeType typeType, DateTime? createTime = null)
        {
            var table = new CodeTable()
            {
                Code = code,
                Level = levelType,
                Type = typeType,
            };

            if (createTime is { })
            {
                table.CreateTime = createTime.Value;
            }

            Insert(table);
        }

        public void InsertOrUpdate(long code, LevelType levelType, TypeType typeType, DateTime? createTime = null)
        {
            var table = new CodeTable()
            {
                Code = code,
                Level = levelType,
                Type = typeType,
            };

            if (createTime is { })
            {
                table.CreateTime = createTime.Value;
            }

            InsertOrUpdate(table);
        }

        public void InsertOrUpdate(CodeTable codeTable)
        {
            var existedTable = Search(codeTable);
            var data = _db.GetCollection<CodeTable>(_codeTable);
            if (existedTable is null)
            {
                codeTable.UpdateTime = DateTime.Now;
                data.EnsureIndex(x => x.Code, true);
                data.Insert(codeTable);
            }
            else
            {
                existedTable.Level = codeTable.Level;
                existedTable.Type = codeTable.Type;
                existedTable.UpdateTime = DateTime.Now;
                existedTable.FilePath = codeTable.FilePath;
                data.EnsureIndex(x => x.Code, true);
                data.Update(existedTable);
            }
            HasDataChanged = true;
        }

        public void InsertIfNotExist(long code, LevelType levelType, TypeType typeType, DateTime? createTime = null)
        {
            var table = new CodeTable()
            {
                Code = code,
                Level = levelType,
                Type = typeType,
            };

            if (createTime is { })
            {
                table.CreateTime = createTime.Value;
            }

            InsertIfNotExist(table);
        }

        public void InsertIfNotExist(CodeTable codeTable)
        {
            var existedTable = Search(codeTable);
            var data = _db.GetCollection<CodeTable>(_codeTable);
            if (existedTable is null)
            {
                codeTable.UpdateTime = DateTime.Now;
                data.EnsureIndex(x => x.Code, true);
                data.Insert(codeTable);
                HasDataChanged = true;
            }
        }

        #endregion

        #region seller

        public IEnumerable<MovieStar>? SearchStars(string name)
        {

            var starTable = _db.GetCollection<MovieStar>(_movieStarTable);
            var starData = starTable.FindAll().Where(v => LikeIn(v.GetNames(), name));
            return starData;
        }

        public MovieStar? SearchStarById(ObjectId id)
        {
            var starTable = _db.GetCollection<MovieStar>(_movieStarTable);
            return starTable.FindById(id);
        }

        #endregion

        #region seller

        public IEnumerable<MovieSeller>? SearchSellers(string name)
        {
            var sellerTable = _db.GetCollection<MovieSeller>(_movieSellerTable);
            var sellerData = sellerTable.FindAll().Where(v => Like(v.Name, name));
            return sellerData;
        }

        public MovieSeller? SearchSellerById(ObjectId id)
        {
            var sellerTable = _db.GetCollection<MovieSeller>(_movieSellerTable);
            return sellerTable.FindById(id);
        }

        #endregion

        #region helper

        private bool ContainsIn(IList<ObjectId>? ids, ObjectId id)
        {
            if (ids is null)
            {
                return false;
            }
            return ids.Contains(id);
        }

        private bool Like(string? name, string searchName)
        {
            if (name is not { })
            {
                return false;
            }

            return name.Contains(searchName);
        }

        private bool LikeIn(IList<string> names, string searchName)
        {
            foreach (var item in names)
            {
                if (item.Contains(searchName))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
