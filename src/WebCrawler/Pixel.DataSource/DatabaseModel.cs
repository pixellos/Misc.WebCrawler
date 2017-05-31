using System;
using System.Collections.Generic;
using System.Text;

namespace Pixel.DataSource
{
    public class DatabaseModel: IDatabaseModel
    {
        public string Id { get; set; }
        public DateTime Added { get; set; }
        public ModelState State { get; set; }
    }
}
