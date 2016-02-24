using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool
{
    class ToolAction
    {
        // Команда программатору
        // {device} - название контроллера
        // {option} - опции
        // {filename} - имя файла
        public string Command = "";

        // Для отдельных действий
        public string Name = "";

        // Свой путь до ПО (для дополнительных программ)
        public string CustomToolPath = null;

        // Определение ошибки в выводе утилиты
        public List<string> ErrorMask = new List<string>();

        // Определение предупреждений в выводе утилиты
        public List<string> WarningMask = new List<string>();

        // Команды, передаваемые консольной программе в stdin
        public List<string> Write = new List<string>();

        // Доступные опции
        public Options.OptionListItem Defaults = new Options.OptionListItem("defaults");
    }
}
