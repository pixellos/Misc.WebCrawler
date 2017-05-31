using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;
using Pixel.Results;

namespace Pixel.DataSource
{
    public class Repository<T> : IEnumerable<T>, IDisposable, IRepository<T> where T : IDatabaseModel
    {
        protected LiteCollection<T> Collection => this.Database.GetCollection<T>();
        protected LiteDatabase Database { get; }

        public Repository(string databasePath)
        {
            this.Database = new LiteDatabase(databasePath);
        }

        protected IResult Add(T value, DateTime addTime)
        {
            if (value == null)
            {
                return new ArgumentError<T>(value, "NotNull");
            }
            if (this.Any(x => x.Id == value.Id))
            {
                return new Exist<T>(value, "You're trying to add existing entry.");
            }
            using (var trans = this.Database.BeginTrans())
            {
                value.Id = Guid.NewGuid().ToString();
                value.Added = addTime;
                this.Collection.Insert(value);
                trans.Commit();
            }
            return new Succeeded();
        }

        public IResult Add(T value)
        {
            return this.Add(value, DateTime.Now);
        }

        public IResult Edit(T value)
        {
            var items = this.Where(x => x.Id == value.Id);
            if (!items.Any())
            {
                return new NotFound<T>(value);
            }
            using (var trans = this.Database.BeginTrans())
            {
                foreach (var item in items)
                {
                    item.State = ModelState.Updated;
                    this.Collection.Update(item);
                }
                var result = this.Add(value);
                trans.Commit();
                return result;
            }
        }

        public IResult Hide(T value)
        {
            var items = this.Where(x => x.Id == value.Id);
            if (!items.Any())
            {
                return new NotFound<T>(value, "Not found");
            }
            using (var trans = this.Database.BeginTrans())
            {
                foreach (var item in items)
                {
                    item.State = ModelState.Deleted;
                    this.Collection.Update(item);
                }
                trans.Commit();
                return new Succeeded();
            }
        }

        public void CopyTo(Array array, int index)
        {
            var collection = this.Collection.FindAll();
            var remaining = collection.Count() - index;
            Array.Copy(collection.ToArray(), index, array, 0, remaining);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Collection.Find(x => x.State == ModelState.Ok).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerator<T>)this);
        }

        public void Dispose()
        {
            this.Database.Dispose();
        }
    }
}