using Microsoft.DSX.ProjectTemplate.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace CeskyRozhlasMiner.WebApp.Data.Utilities
{
    /// <summary>
    /// Utility which provides methods for validation of DTO models.
    /// </summary>
    /// <typeparam name="T">The type of DTO model</typeparam>
    public class ValidationHelper<T>
    {
        private readonly Type _objType;
        private readonly T _obj;

        /// <summary>
        /// Initializes class.
        /// </summary>
        /// <param name="obj">DTO model instance</param>
        public ValidationHelper(T obj)
        {
            _obj = obj;
            _objType = typeof(T);
        }

        /// <summary>
        /// Checks whether given property is a valid email adress.
        /// </summary>
        /// <param name="prop">The name of property to check</param>
        /// <param name="readableName">Human readable name of the property. If null then 
        /// <paramref name="prop"/> is used.</param>
        /// <returns>Empty enumerable if valid, otherwise enumerable containing
        /// one validation result.</returns>
        public IEnumerable<ValidationResult> CheckStringValidEmailAdress(string prop, string readableName = null)
        {
            readableName ??= prop;

            string email = (string)_objType.GetProperty(prop).GetValue(_obj);
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!regex.Match(email).Success)
            {
                yield return new ValidationResult($"{readableName} is not valid adress.", new[] { prop });
            }
        }

        /// <summary>
        /// Checks whether given property is a valid password.
        /// </summary>
        /// <param name="prop">The name of property to check</param>
        /// <param name="readableName">Human readable name of the property. If null then 
        /// <paramref name="prop"/> is used.</param>
        /// <returns>Empty enumerable if valid, otherwise enumerable containing
        /// validation results.</returns>
        public IEnumerable<ValidationResult> CheckValidPassword(string prop, string readableName = null)
        {
            string value = (string)_objType.GetProperty(prop).GetValue(_obj);
            readableName ??= prop;

            if (value.Length < Constants.MimimumLengths.Password)
            {
                yield return new ValidationResult($"{readableName} must be minimum {Constants.MimimumLengths.Password} characters.", new[] { prop });
            }

            if (!value.Any(char.IsDigit))
            {
                yield return new ValidationResult($"{readableName} must contain at least one digit.", new[] { prop });
            }

            if (value.All(char.IsLetterOrDigit))
            {
                yield return new ValidationResult($"{readableName} must contain one special character.", new[] { prop });
            }
        }

        /// <summary>
        /// Checks whether given property is a valid general string.
        /// </summary>
        /// <param name="prop">The name of property to check</param>
        /// <param name="readableName">Human readable name of the property. If null then 
        /// <paramref name="prop"/> is used.</param>
        /// <returns>Empty enumerable if valid, otherwise enumerable containing
        /// validation results.</returns>
        public IEnumerable<ValidationResult> CheckStringNotEmptyAndCorrectLength(string prop, string readableName = null)
        {
            string value = (string)_objType.GetProperty(prop).GetValue(_obj);
            readableName ??= prop;

            if (string.IsNullOrWhiteSpace(value))
            {
                yield return new ValidationResult($"{readableName} cannot be null or empty.", new[] { prop });
            }

            foreach (var result in CheckStringCorrectLength(prop, readableName))
            {
                yield return result;
            }
        }

        /// <summary>
        /// Checks whether given property is a valid short string (empty allowed).
        /// </summary>
        /// <param name="prop">The name of property to check</param>
        /// <param name="readableName">Human readable name of the property. If null then 
        /// <paramref name="prop"/> is used.</param>
        /// <returns>Empty enumerable if valid, otherwise enumerable containing
        /// validation result.</returns>
        public IEnumerable<ValidationResult> CheckStringCorrectLength(string prop, string readableName = null)
        {
            string value = (string)_objType.GetProperty(prop).GetValue(_obj);
            readableName ??= prop;

             if (value != null && value.Length > Constants.MaximumLengths.StringColumn)
            {
                yield return new ValidationResult($"{readableName} must be maximum {Constants.MaximumLengths.StringColumn} characters.", new[] { prop });
            }
        }

        /// <summary>
        /// Checks whether given properties are valid general strings.
        /// </summary>
        /// <param name="prop">The name of properties to check</param>
        /// <returns>Empty enumerable if valid, otherwise enumerable containing
        /// validation results.</returns>
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
