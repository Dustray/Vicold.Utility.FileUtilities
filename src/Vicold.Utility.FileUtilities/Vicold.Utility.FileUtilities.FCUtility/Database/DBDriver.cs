using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Utility.FileUtilities.FCUtility.Database.Entities;

namespace Vicold.Utility.FileUtilities.FCUtility.Database
{
    internal class DBDriver : IDisposable
    {
        public static readonly string ConnectionString = AppDomain.CurrentDomain.BaseDirectory + @"data\\data.db";
        private static readonly string _codeTable = "CodeTable";
        private LiteDatabase _db;
        internal DBDriver()
        {
            _db = new LiteDatabase(ConnectionString);
            BsonMapper.Global.IncludeNonPublic = true;
        }

        public bool HasDataChanged { get; private set; } = false;


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
            var data = _db.GetCollection<CodeTable>(_codeTable);
            data.EnsureIndex(x => x.Code, true);
            data.Update(existedTable);
            HasDataChanged = true;
        }

        public void Update(long code, LevelType levelType, TypeType typeType)
        {
            Update(new CodeTable()
            {
                Code = code,
                Level = levelType,
                Type = typeType,
            });
        }

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

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
