using System;
using System.Runtime.Serialization;

namespace EM
{
    [Serializable]
    public class ArticleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        public ArticleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ArticleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message.
        /// </summary>
		/// <param name="messageFormat">The exception message format.</param>
		/// <param name="args">The exception message arguments.</param>
        public ArticleException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        protected ArticleException(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ArticleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class ArticleArgumentNullException : ArgumentNullException
    {
        public ArticleArgumentNullException(string paramName) : base(paramName)
        {
        }

        public ArticleArgumentNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ArticleArgumentNullException(string paramName, string message) : base(paramName, message)
        { }
    }

    /// <summary>
    /// waring msg
    /// </summary>
    [Serializable]
    public class ArticleWarningException : Exception
    {
        public ArticleWarningException(string msg) : base(msg)
        {
        }

        public ArticleWarningException(string msg, Exception ex) : base(msg, ex)
        { }
    }

    [Serializable]
    public class ArticleInfoException : Exception
    {
        public ArticleInfoException(string msg) : base(msg)
        {
        }

        public ArticleInfoException(string msg, Exception ex) : base(msg, ex)
        { }
    }
}