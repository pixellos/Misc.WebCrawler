using System;
using System.Collections.Generic;

namespace Pixel.DataSource
{
    public interface IDatabaseModel
    {
        string Id { get; set; }
        DateTime Added { get; set; }
        ModelState State{ get; set; }
    }
}