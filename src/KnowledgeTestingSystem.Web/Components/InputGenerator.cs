using System;
using System.Collections.Generic;
using System.Linq;
using static KnowledgeTestingSystem.Web.Components.Pages.TaskEditorPage;

namespace KnowledgeTestingSystem.Web.Components
{
    public class InputGenerator
    {
        private readonly Random _random = new Random();

        public string GenerateValue(string varType, VariableGenType genType, VariableConfig config)
        {
            // Удаляем квадратные скобки из типа для базовой обработки
            bool isArray = varType.EndsWith("[]");
            string baseType = varType.Replace("[]", "");

            switch (baseType.ToLower())
            {
                case "int":
                    return isArray ? GenerateIntArray(genType, config) : GenerateInt(genType, config);
                case "float":
                    return isArray ? GenerateFloatArray(genType, config) : GenerateFloat(genType, config);
                case "double":
                    return isArray ? GenerateDoubleArray(genType, config) : GenerateDouble(genType, config);
                case "string":
                    return GenerateString(genType, config);
                case "char":
                    return GenerateChar(genType, config);
                default:
                    throw new NotSupportedException($"Тип {varType} не поддерживается.");
            }
        }

        private string GenerateInt(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "0";
            }
            else if (genType == VariableGenType.Random || genType == VariableGenType.Boundary)
            {
                int min = config.MinValue ?? -100;
                int max = config.MaxValue ?? 100;
                return _random.Next(min, max + 1).ToString();
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип генерации.");
            }
        }

        private string GenerateFloat(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "0.0";
            }
            else if (genType == VariableGenType.Random || genType == VariableGenType.Boundary)
            {
                float min = config.MinValue ?? -100f;
                float max = config.MaxValue ?? 100f;
                return (min + (_random.NextDouble() * (max - min))).ToString("F2");
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип генерации.");
            }
        }

        private string GenerateDouble(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "0.0";
            }
            else if (genType == VariableGenType.Random || genType == VariableGenType.Boundary)
            {
                double min = config.MinValue ?? -100.0;
                double max = config.MaxValue ?? 100.0;
                return (min + (_random.NextDouble() * (max - min))).ToString("F2");
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип генерации.");
            }
        }

        private string GenerateString(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "";
            }
            else if (genType == VariableGenType.Random)
            {
                int length = _random.Next(1, 11); // Длина от 1 до 10
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            }
            else // Boundary
            {
                int minLength = config.MinValue ?? 0;
                int maxLength = config.MaxValue ?? 10;
                int length = _random.Next(minLength, maxLength + 1);
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                return length == 0 ? "" : new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            }
        }

        private string GenerateChar(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "a";
            }
            else if (genType == VariableGenType.Random || genType == VariableGenType.Boundary)
            {
                char min = (char)(config.MinValue ?? 'a');
                char max = (char)(config.MaxValue ?? 'z');
                return ((char)_random.Next(min, max + 1)).ToString();
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип генерации.");
            }
        }

        private string GenerateIntArray(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "1, 2, 3";
            }
            else if (genType == VariableGenType.Random || genType == VariableGenType.Boundary)
            {
                int length = _random.Next(2, 16); // Длина от 1 до 10
                int min = config.MinValue ?? -100;
                int max = config.MaxValue ?? 100;
                var values = Enumerable.Range(0, length)
                    .Select(_ => _random.Next(min, max + 1));
                return string.Join(", ", values);
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип генерации.");
            }
        }

        private string GenerateFloatArray(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "1.0, 2.0, 3.0";
            }
            else if (genType == VariableGenType.Random || genType == VariableGenType.Boundary)
            {
                int length = _random.Next(2, 16);
                float min = config.MinValue ?? -100f;
                float max = config.MaxValue ?? 100f;
                var values = Enumerable.Range(0, length)
                    .Select(_ => (float)(min + (_random.NextDouble() * (max - min))))
                    .Select(v => v.ToString("F2"));
                return string.Join(", ", values);
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип генерации.");
            }
        }

        private string GenerateDoubleArray(VariableGenType genType, VariableConfig config)
        {
            if (genType == VariableGenType.Fixed)
            {
                return config.FixedValue ?? "1.0, 2.0, 3.0";
            }
            else if (genType == VariableGenType.Random || genType == VariableGenType.Boundary)
            {
                int length = _random.Next(2, 16);
                double min = config.MaxValue ?? -100.0;
                double max = config.MaxValue ?? 100.0;
                var values = Enumerable.Range(0, length)
                    .Select(_ => min + (_random.NextDouble() * (max - min)))
                    .Select(v => v.ToString("F2"));
                return string.Join(", ", values);
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип генерации.");
            }
        }
    }
}