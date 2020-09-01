using Bb.Expresssions.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bb.Expresssions
{

    public partial class SourceCode 
    {


        public SourceCode(SourceCode parent)
        {

            this._parent = parent;

            if (parent != null)
            {
                _variables = parent._variables;
                _labels = parent._labels;
            }
            else
            {
                _variables = new Variables();
                _labels = new Labels();
            }

        }






        public SourceCode Add(Expression expression)
        {
            this.Add(new ExpressionStatement(this) { Expression = expression });
            return this;
        }

        public SourceCode AddRange(IEnumerable<Expression> expressions)
        {
            var s2 = expressions.Select(c => new ExpressionStatement(this) { Expression = c });
            _statements.AddRange(s2);
            return this;
        }

        public SourceCode Add(Statement statement)
        {
            _statements.Add(statement);
            this.LastStatement = statement;
            return this;
        }

        public SourceCode AddRange(IEnumerable<Statement> statements)
        {
            foreach (var statement in statements)
                Add(statement);

            return this;
        }

        internal Expression GetExpression(HashSet<string> variableParent)
        {
            Expression expression = null;

            if (this._statements.Count == 1)
                expression = this._statements[0].GetExpression(variableParent);
            else
                expression = GetBlock(variableParent);

            if (expression.CanReduce)
                expression = expression.Reduce();

            return expression;

        }

        public IEnumerable<ParameterExpression> Variables { get => this._variables.Items.Select(c => c.Instance); }

        public Statement LastStatement { get; private set; }

        public ParameterExpression LastVariable { get; private set; }

        private BlockExpression GetBlock(HashSet<string> variableParent)
        {

            ParameterExpression[] __variables = CleanVariables(variableParent);

            var __list = new List<Expression>(this._statements.Count + 10);
            foreach (Statement statement in this._statements)
                __list.Add(statement.GetExpression(variableParent));

            return Expression.Block(__variables, __list.ToArray());

        }

        public void Merge(SourceCode source)
        {

            this._variables.Merge(source._variables);
            this._labels.Merge(source._labels);


            foreach (var item in source._statements)
                this.Add(item);

        }

        protected ParameterExpression[] CleanVariables(HashSet<string> variableParent)
        {

            var v = this._variables.Items.ToList();
            foreach (var item in v)
                if (!(variableParent.Add(item.Name)))
                    this._variables.RemoveByName(item.Name);

            var __variables = this._variables.Items.Select(c => c.Instance).ToArray();

            return __variables;

        }

        protected Variables _variables;
        protected Labels _labels;
        internal SourceCode _parent;
        private List<Statement> _statements = new List<Statement>();

    }

}
