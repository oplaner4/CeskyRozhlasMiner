using Microsoft.DSX.ProjectTemplate.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace CeskyRozhlasMiner.WebApp.Data.Utilities
{
    public class ValidationHelper<T>
    {
        private readonly Type _objType;
        private readonly T _obj;

        public ValidationHelper(T obj)
        {
            _obj = obj;
            _objType = typeof(T);
        }

        public IEnumerable<ValidationResult> CheckStringValidEmailAdress(string prop)
        {
            string email = (string)_objType.GetProperty(prop).GetValue(_obj);
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!regex.Match(email).Success)
            {
                yield return new ValidationResult($"is not valid email adress", new[] { prop });
            }
        }

        public IEnumerable<ValidationResult> CheckValidPassword(string prop)
        {
            string value = (string)_objType.GetProperty(prop).GetValue(_obj);

            if (!value.Any(char.IsDigit) || value.All(char.IsLetterOrDigit))
            {
                yield return new ValidationResult($"must be minimum {Constants.MimimumLengths.Password} characters, contain at least " +
                    $"one digit and one special character", new[] { prop });
            }
        }

        public IEnumerable<ValidationResult> CheckStringNotEmptyAndCorrectLength(string prop)
        {
            string value = (string)_objType.GetProperty(prop).GetValue(_obj);

            if (string.IsNullOrWhiteSpace(value))
            {
                yield return new ValidationResult($"cannot be null or empty", new[] { prop });
            }
            else if (value.Length > Constants.MaximumLengths.StringColumn)
            {
                yield return new ValidationResult($"must be maximum {Constants.MaximumLengths.StringColumn} characters", new[] { prop });
            }
        }

        public IEnumerable<ValidationResult> CheckStringsNotEmptyAndCorrectLength(params string[] props)
        {
            foreach (var prop in props)
            {
                foreach (var result in CheckStringNotEmptyAndCorrectLength(prop))
                {
                    yield return result;
                }
            }
        }
    }
}
