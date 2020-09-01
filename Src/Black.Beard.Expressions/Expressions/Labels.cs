using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Expresssions
{


    public class Labels
    {

        public Labels()
        {
            this._labels = new Dictionary<string, Label>();
        }

        internal void Add(Label label)
        {

            if (label.Kind != KindLabelEnum.Default && this._labels.Values.Any(c => c.Kind == label.Kind))
                throw new Exceptions.DuplicatedArgumentNameMethodReferenceException($"the bloc contains already label of type {label.Kind.ToString()}");

            this._labels.Add(label.Name, label);

        }

        internal Label GetByName(string name)
        {

            if (!this._labels.TryGetValue(name, out Label label))
            {

            }

            return label;
        }

        internal static string GetNewName()
        {
            return $"label_{Guid.NewGuid().ToString()}";
        }

        internal void RemoveByName(string name)
        {
            if (this._labels.ContainsKey(name))
                this._labels.Remove(name);
        }

        internal IEnumerable<Label> Items { get => this._labels.Values; }

        private readonly Dictionary<string, Label> _labels;

        internal void Merge(Labels labels)
        {
            foreach (var item in labels.Items)
                this.Add(item);
        }

    }

}
