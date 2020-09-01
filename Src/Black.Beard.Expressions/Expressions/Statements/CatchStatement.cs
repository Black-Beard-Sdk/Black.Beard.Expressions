using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bb.Expresssions.Statements
{

    public class CatchStatement : Statement
    {

        public CatchStatement(SourceCode parent)
            : base(parent)
        {
            this.Body = new SourceCode(_parent);
        }

        public SourceCode Body { get; set; }


        public Type TypeToCatch { get; set; }


        public ParameterExpression Parameter { get; set; }


        public override Expression GetExpression(HashSet<string> variableParent)
        {
            return Body.GetExpression(new HashSet<string>(variableParent));
        }

    }

}
