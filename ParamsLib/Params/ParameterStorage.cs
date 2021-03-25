using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Params.Types;

namespace Params
{
    public class ParameterStorage
    {
        /// <summary>
        /// Список параметров
        /// </summary>
        public List<Parameter> Params = new List<Parameter>();

        /// <summary>
        /// Перечисления - в критической секции
        /// </summary>
        private Object searchLock = new Object();

        /// <summary>
        /// Изменилось состояние?
        /// </summary>
        private bool Changed = false;

        /// <summary>
        /// Изменилось ли состояние устройства
        /// </summary>
        public bool Updated
        {
            get { if (Changed) { Changed = false; return true; } else return false; }
        }

        /// <summary>
        /// Известить об изменениях
        /// </summary>
        protected virtual void notifyChanged()
        {
            Changed = true;
        }

        /// <summary>
        /// Индексированный вариант доступа
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        private Parameter this[string Index]
        {
            get
            {
                lock (searchLock)
                {
                    foreach (Parameter P in Params)
                        if (P.Name.CompareTo(Index) == 0) return P;
                }

                return null;
            }
            set
            {
                value.Name = Index;
                lock (searchLock)
                {
                    for (int i = 0; i < Params.Count; i++)
                    {
                        if (Params[i].Name.CompareTo(Index) == 0)
                        {
                            Params[i] = value;
                            return;
                        }
                    }

                    Params.Add(value);
                }
            }
        }

        /// <summary>
        /// Есть ли определённый параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool hasParameter(string Name)
        {
            return this[Name] != null;
        }

        /// <summary>
        /// Удалить параметр
        /// </summary>
        /// <param name="Name"></param>
        public void delete(string Name, bool Update)
        {
            Parameter P = this[Name];

            if (P != null)
            {
                lock (searchLock) { Params.Remove(P); }
                if (Update) notifyChanged();
            }
        }

        /// <summary>
        /// Получить целочисленный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int getInteger(string Name)
        {
            Parameter P = this[Name];
            if (P == null) return 0;
            if (P.GetType() != typeof(IntParameter))
            {
                if (P.GetType() == typeof(DoubleParameter))
                    return Convert.ToInt32((P as DoubleParameter).Value);
                else
                    return 0;
            }

            return (P as IntParameter).Value;
        }

        /// <summary>
        /// Записать целочисленный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setIntegerT(string Name, int Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new IntParameter(Name, Value, true));
                }
                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(IntParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new IntParameter(Name, Value, true));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if ((P as IntParameter).Value != Value)
                {
                    (P as IntParameter).Value = Value;
                    if (Update) notifyChanged();
                }
            }

        }

        /// <summary>
        /// Записать целочисленный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setIntegerT(string Name, int Value)
        {
            setIntegerT(Name, Value, true);
        }

        /// <summary>
        /// Записать целочисленный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setInteger(string Name, int Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new IntParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(IntParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new IntParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if ((P as IntParameter).Value != Value)
                {
                    (P as IntParameter).Value = Value;
                    if(Update) notifyChanged();
                }
            }

        }

        /// <summary>
        /// Записать целочисленный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setInteger(string Name, int Value)
        {
            setInteger(Name, Value, true);
        }

        /// <summary>
        /// Получить текстовый параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string getString(string Name)
        {
            Parameter P = this[Name];
            if (P == null) return null;

            return P.ToString();
        }

        /// <summary>
        /// Установить временное (временный)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="Update"></param>
        public void setStringT(string Name, string Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new StringParameter(Name, Value, true));
                }
                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(StringParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new StringParameter(Name, Value, true));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if ((P as StringParameter).Value.CompareTo(Value) != 0)
                {
                    (P as StringParameter).Value = Value;
                    if (Update) notifyChanged();
                }
            }
        }

        /// <summary>
        /// Записать строковый параметр (временный)
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setStringT(string Name, string Value)
        {
            setStringT(Name, Value, true);
        }

        /// <summary>
        /// Записать строковый параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setString(string Name, string Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new StringParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(StringParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new StringParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if  ((P as StringParameter).Value.CompareTo(Value) != 0)
                {
                    (P as StringParameter).Value = Value;
                    if (Update) notifyChanged();
                }
            }
        }

        /// <summary>
        /// Записать строковый параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setString(string Name, string Value)
        {
            setString(Name, Value, true);
        }

        /// <summary>
        /// Получить численный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public double getDouble(string Name)
        {
            Parameter P = this[Name];
            if (P == null) return 0.0;
            if (P.GetType() != typeof(DoubleParameter))
            {
                if (P.GetType() == typeof(IntParameter))
                    return (P as IntParameter).Value;
                else
                    return 0.0;
            }

            return (P as DoubleParameter).Value;
        }

        /// <summary>
        /// Записать численный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setDoubleT(string Name, double Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new DoubleParameter(Name, Value, true));
                }
                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(DoubleParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new DoubleParameter(Name, Value, true));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if ((P as DoubleParameter).Value != Value)
                {
                    (P as DoubleParameter).Value = Value;
                    if (Update) notifyChanged();
                }
            }
        }

        /// <summary>
        /// Записать численный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setDoubleT(string Name, double Value)
        {
            setDouble(Name, Value, true);
        }

        /// <summary>
        /// Записать численный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setDouble(string Name, double Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new DoubleParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(DoubleParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new DoubleParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if ((P as DoubleParameter).Value != Value)
                {
                    (P as DoubleParameter).Value = Value;
                    if (Update) notifyChanged();
                }
            }
        }

        /// <summary>
        /// Записать численный параметр
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setDouble(string Name, double Value)
        {
            setDouble(Name, Value, true);
        }

        /// <summary>
        /// Получить объект
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public object getObject(string Name)
        {
            Parameter P = this[Name];
            if (P == null) return null;
            if (P.GetType() != typeof(ObjectParameter))
            {
                return null;
            }

            return (P as ObjectParameter).Value;
        }

        /// <summary>
        /// Записать объект
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setObjectT(string Name, object Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new ObjectParameter(Name, Value, true));
                }

                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(ObjectParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new ObjectParameter(Name, Value, true));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if (!(P as ObjectParameter).Value.Equals(Value))
                {
                    (P as ObjectParameter).Value = Value;
                    if (Update) notifyChanged();
                }
            }

        }

        /// <summary>
        /// Записать объект
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setObjectT(string Name, object Value)
        {
            setObjectT(Name, Value, true);
        }

        /// <summary>
        /// Записать объект
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setObject(string Name, object Value, bool Update)
        {
            Parameter P = this[Name];
            if (P == null)
            {
                lock (searchLock)
                {
                    Params.Add(new ObjectParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else if (P.GetType() != typeof(ObjectParameter))
            {
                lock (searchLock)
                {
                    Params.Remove(P);
                    Params.Add(new ObjectParameter(Name, Value));
                }
                if (Update) notifyChanged();
            }
            else
            {
                if (!(P as ObjectParameter).Value.Equals(Value))
                {
                    (P as ObjectParameter).Value = Value;
                    if (Update) notifyChanged();
                }
            }

        }

        /// <summary>
        /// Записать объект
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void setObject(string Name, object Value)
        {
            setObject(Name, Value, true);
        }
    }
}
