using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace WebLicense.Logic.Auxiliary
{
    public class CaseResult
    {
        #region Fields

        public readonly bool Succeeded;

        public readonly IReadOnlyCollection<CaseError> Errors;

        #endregion

        #region C-tor

        internal CaseResult()
        {
            Succeeded = true;
            Errors = null;
        }

        internal CaseResult([NotNull] string error) : this(error, new Exception(error))
        {
        }

        internal CaseResult([NotNull] Exception exception) : this(exception.Message, exception)
        {
        }

        internal CaseResult([NotNull] string error, [NotNull] Exception exception)
        {
            Succeeded = false;
            Errors = new List<CaseError> {new(error, exception)};
        }

        internal CaseResult([NotNull] IdentityResult result)
        {
            if (result == null || result.Succeeded && (result.Errors == null || !result.Errors.Any()))
            {
                Succeeded = true;
                Errors = null;
            }
            else
            {
                Succeeded = false;
                Errors = result.Errors != null && result.Errors.Any() ? result.Errors.Select(q => new CaseError(q.Description)).ToList() : null;
            }
        }

        #endregion
    }

    public sealed class CaseResult<T> : CaseResult
    {
        #region Fields

        public readonly T Data;

        #endregion

        #region C-tor

        internal CaseResult()
        {
            Data = default;
        }

        internal CaseResult(T data)
        {
            Data = data;
        }

        internal CaseResult(T data, string error = null) : base(error)
        {
            Data = data;
        }

        internal CaseResult(Exception exception = null) : base(exception)
        {
            Data = default;
        }

        internal CaseResult(T data, IdentityResult result = null) : base(result)
        {
            Data = data;
        }

        #endregion
    }

    #region Auxiliary classes

    public sealed class CaseError
    {
        #region Fields

        public readonly string Message;

        public readonly Exception Exception;

        #endregion

        #region C-tor

        internal CaseError([NotNull] string message)
        {
            Message = !string.IsNullOrWhiteSpace(message) ? message.Trim() : null;
            Exception = !string.IsNullOrWhiteSpace(message) ? new Exception(message.Trim()) : null;
        }

        internal CaseError([NotNull] Exception exception)
        {
            Message = exception?.Message;
            Exception = exception;
        }

        internal CaseError([NotNull] string message, [NotNull] string exception)
        {
            Message = !string.IsNullOrWhiteSpace(message) ? message.Trim() : null;
            Exception = !string.IsNullOrWhiteSpace(exception) ? new Exception(exception.Trim()) : null;
        }

        internal CaseError([NotNull] string message, [NotNull] Exception exception)
        {
            Message = !string.IsNullOrWhiteSpace(message) ? message.Trim() : null;
            Exception = exception;
        }

        #endregion
    }

    #endregion
}