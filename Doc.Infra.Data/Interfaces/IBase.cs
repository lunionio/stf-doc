using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Doc.Infra.Data.Entidades
{
    interface IBase<T> where T : class
    {
        IList<T> GetAll();
        IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        int Add(params T[] items);
        void Update(params T[] items);
        void Remove(params T[] items);
    }
}
