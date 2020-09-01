using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bb.Expresssions.Statements
{

    public abstract class Statement
    {

        public Statement(SourceCode parent)
        {
            this._parent = parent;
        }

        public abstract Expression GetExpression(HashSet<string> variableParent);


        protected readonly SourceCode _parent;

    }

}
