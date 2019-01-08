using Banana.Uow.Models.QueryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Lambda.ExperssionTress
{
    class LikeNode : Node
    {
        public ELikeMethod Method { get; set; }
        public MemberNode MemberNode { get; set; }
        public string Value { get; set; }
    }
}
