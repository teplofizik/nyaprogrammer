using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Params;

namespace Programmer.Tool.Options
{
    class OptionListItem: ParameterStorage
    {
        private string Label = "";

        /// <summary>
        /// Элемент по умолчанию
        /// </summary>
        public bool Default = false;

        public OptionListItem(string L)
        {
            Label = L;
        }

        public override string ToString()
        {
            return (Label != null) ? Label : base.ToString();
        }

        /// <summary>
        /// Получить значение аргумента или null
        /// </summary>
        /// <param name="Argument"></param>
        /// <returns></returns>
        public string GetValue(string Argument)
        {
            return (hasParameter(Argument)) ? getString(Argument) : null;
        }

        /// <summary>
        /// Добавить значение в список
        /// </summary>
        /// <param name="Argument"></param>
        /// <param name="Value"></param>
        public void addValue(string Argument, string Value)
        {
            setString(Argument, Value);
        }

        public string[] getNames()
        {
            var Res = new List<string>();
            foreach (var P in Params) Res.Add(P.Name);

            return Res.ToArray();
        }
    }

    class OptionList : Option
    {
        private List<OptionListItem> mStorage = new List<OptionListItem>();

        public OptionList(string Name) : base(Name)
        {

        }

        /// <summary>
        /// Список вариантов
        /// </summary>
        public OptionListItem[] Items
        {
            get { return mStorage.ToArray(); }
        }

        /// <summary>
        /// Добавить элемент в список
        /// </summary>
        /// <param name="I"></param>
        public void AddItem(OptionListItem I)
        {
            mStorage.Add(I);
        }
    }
}
