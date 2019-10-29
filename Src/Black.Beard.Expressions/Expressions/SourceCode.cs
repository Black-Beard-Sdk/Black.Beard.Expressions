using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bb.Expresssions
{
    public class SourceCode : List<Statement>
    {

        public SourceCode(SourceCode parent = null)
        {
            this._parent = parent;
        }

        #region Labels 

        public Label AddLabel(string name = null, KindLabelEnum kind = KindLabelEnum.Default)
        {

            if (string.IsNullOrEmpty(name))
                name = this._variables.GetNewName();

            var instance = Expression.Label(Labels.GetNewName());
            var label = new Label() { Instance = instance, Kind = kind };
            this.AddLabel(label);

            return label;
        }

        public SourceCode AddLabel(Label label)
        {
            this._labels.Add(label);
            return this;
        }

        #endregion Labels 

        #region variables

        public SourceCode AddVarIfNotExists(ParameterExpression parameter)
        {

            var vari = this.GetVar(parameter.Name);
            if (vari == null)
                this.AddVar(parameter);

            return this;

        }

        public ParameterExpression AddVarIfNotExists(Type type, string name)
        {

            var variable = this.GetVar(name);
            if (variable != null)
                return variable;

            if (string.IsNullOrEmpty(name))
                name = this._variables.GetNewName();

            var instance = Expression.Parameter(type, name);
            this.AddVar(instance);

            return instance;

        }

        public ParameterExpression AddVar(Type type, string name = null)
        {

            if (string.IsNullOrEmpty(name))
                name = this._variables.GetNewName();

            var instance = Expression.Parameter(type, name);
            this.AddVar(instance);

            return instance;
        }

        public SourceCode AddVar(ParameterExpression arg)
        {

            var vari = this._variables.GetByName(arg.Name);
            if (vari != null)
            {
                if (vari.Instance != arg)
                    throw new Exceptions.DuplicatedArgumentNameMethodReferenceException($"parameter {arg.Name} already exists");
            }
            else
            {
                var variable = new Variable() { Name = arg.Name, Instance = arg };
                this._variables.Add(variable);
                this.LastVariable = arg;
            }

            return this;

        }

        public virtual ParameterExpression GetVar(string name)
        {
            var variable = _variables.GetByName(name);
            if (variable == null)
            {
                if (_parent != null)
                    return _parent.GetVar(name);
            }
            else
                return variable.Instance;

            return null;

        }

        #endregion variables

        #region Goto

        public SourceCode Break()
        {
            var label = this.GetLabelImpl(KindLabelEnum.Break);
            Add(new GotoStatement() { Label = label });
            return this;
        }

        public SourceCode Continue()
        {
            var label = this.GetLabelImpl(KindLabelEnum.Continue);
            Add(new GotoStatement() { Label = label });
            return this;

        }

        public SourceCode Return(Expression @return)
        {
            var label = this.GetLabelImpl(KindLabelEnum.Return);
            Add(new GotoStatement() { Label = label, Expression = @return, });
            return this;

        }

        private Label GetLabelImpl(KindLabelEnum kind)
        {

            Label label = this._labels.Items.FirstOrDefault(c => c.Kind == kind);

            if (label == null && _parent == null)
                label = _parent.GetLabelImpl(kind);

            if (label == null)
                throw new Exceptions.InvalidArgumentNameMethodReferenceException($"no label of {kind.ToString()} defined");

            return label;

        }

        #endregion Goto

        public SourceCode If(Expression e, params Expression[] codes)
        {

            var _then = new SourceCode()
            {

            };
            _then.AddRange(codes.Select(c => new ExpressionStatement() { Expression = c }));

            this.If(e, _then, null);

            return this;
        }


        public SourceCode If(Expression e, SourceCode @then, SourceCode @else)
        {

            var n = new ConditionalStatement()
            {
                Expression = e,
                Then = @then,
                Else = @else
            };

            this.Add(n);

            return this;
        }


        public SourceCode Call(MethodInfo self, params Expression[] arguments)
        {
            Add(self.Call(arguments));
            return this;
        }


        public SourceCode NewObject(Type type, params Expression[] args)
        {
            Add(type.Create(args));
            return this;
        }

        #region Exceptions


        public SourceCode Try(Expression e, params CatchStatement[] catchs)
        {
            return Try(new SourceCode(this).Add(e), catchs);
        }

        public SourceCode Try(SourceCode self, params CatchStatement[] catchs)
        {

            self._parent = this;

            var tryStatement = new TryStatement()
            {
                Try = self,
            };

            foreach (var item in catchs)
            {
                item.Body._parent = this;
                tryStatement.Catchs.Add(item);
            }

            this.Add(tryStatement);

            return this;

        }

        public SourceCode Try(SourceCode self, SourceCode @finally, params CatchStatement[] catchs)
        {

            self._parent = this;
            @finally._parent = this;

            var tryStatement = new TryStatement()
            {
                Try = self,
                Finally = @finally
            };

            foreach (var item in catchs)
            {
                item.Body._parent = this;
                tryStatement.Catchs.Add(item);
            }

            this.Add(tryStatement);

            return this;

        }

        public SourceCode Throw(Type type, params Expression[] args)
        {
            Add(type.Throw(args));
            return this;
        }

        public SourceCode Throw(ParameterExpression arg)
        {
            Add(arg);
            return this;
        }

        public SourceCode ReThrow()
        {
            Add(Expression.Rethrow());
            return this;
        }

        #endregion Exceptions


        public SourceCode Assign(Expression left, Expression right)
        {
            Add(left.AssignFrom(right));
            return this;
        }


        public SourceCode Add(Expression expression)
        {
            this.Add(new ExpressionStatement() { Expression = expression });
            return this;
        }

        public SourceCode AddRange(IEnumerable<Expression> expressions)
        {
            var s2 = expressions.Select(c => new ExpressionStatement() { Expression = c });
            base.AddRange(s2);
            return this;
        }

        public new SourceCode Add(Statement statement)
        {
            base.Add(statement);
            this.LastStatement = statement;
            return this;
        }

        public new SourceCode AddRange(IEnumerable<Statement> statements)
        {
            foreach (var statement in statements)
                this.Add(statement);

            return this;
        }

        internal Expression GetExpression(HashSet<string> variableParent)
        {
            Expression expression = null;

            if (this.Count == 1)
                expression = this[0].GetExpression(variableParent);
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

            var __list = new List<Expression>(this.Count + 10);
            foreach (Statement statement in this)
                __list.Add(statement.GetExpression(variableParent));

            return Expression.Block(__variables, __list.ToArray());

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

        protected Variables _variables = new Variables();
        protected Labels _labels = new Labels();
        internal SourceCode _parent;
    }

}
