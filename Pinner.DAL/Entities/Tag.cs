using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinner.DAL.Entities
{
    public class Tag : Entity
    {
        public string Query { get; set; }
        public Topic Topic { get; set; }
    }
}
