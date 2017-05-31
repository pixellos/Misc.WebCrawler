using System.Collections.Generic;
using Pixel.Results;
using System;

namespace Pixel.DataSource
{
    public interface IRepository<T> : IDisposable, IEnumerable<T>
        where T : IDatabaseModel
    {
        IResult Add(T value);
        IResult Edit(T value);
        IResult Hide(T value);
    }
}