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

        public readonly IReadOnlyCollection<CaseException> Errors;

        #endregion

        #region C-tor

        internal CaseResult()
        {
            Succeeded = true;
            Errors = null;
        }

        internal CaseResult([NotNull] string error) : this(new CaseException(error))
        {
        }

        internal CaseResult([NotNull] Exception exception) : this(exception is CaseException cex ? cex : new CaseException(exception))
        {
        }

        internal CaseResult([NotNull] string error, [NotNull] Exception exception) : this(new CaseException(error, exception))
        {
        }

        internal CaseResult([NotNull] CaseException exception)
        {
            Succeeded = false;
            Errors = new List<CaseException> {exception};
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
                Errors = result.Errors != null && result.Errors.Any() ? result.Errors.Select(q => new CaseException(q.Description)).ToList() : null;
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

    public sealed class CaseException : Exception
    {
        #region Fields

        public readonly string LocalizedMessage;

        #endregion

        #region C-tor

        internal CaseException([NotNull] string message) : this(message, new Exception(message))
        {
        }

        internal CaseException([NotNull] Exception exception) : this(exception.Message, exception)
        {
        }

        internal CaseException([NotNull] string message, [NotNull] string exception) : this(message, new Exception(exception.Trim()))
        {
        }

        internal CaseException([NotNull] string message, [NotNull] Exception exception) : base(exception.Message.Trim(), exception)
        {
            LocalizedMessage = !string.IsNullOrWhiteSpace(message) ? message.Trim() : exception.Message;
        }

        #endregion
    }

    #endregion
}