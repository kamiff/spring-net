using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Context.Support
{
    #region Context Schema Constants

    /// <summary>
    /// Constants defining the structure and values associated with the
    /// schema for laying out Spring.NET contexts in XML.
    /// </summary>
    internal class ContextSchema
    {
        /// <summary>
        /// Defines a single
        /// <see cref="Spring.Context.IApplicationContext"/>.
        /// </summary>
        public const string ContextElement = "context";

        /// <summary>
        /// Specifies a context name.
        /// </summary>
        public const string NameAttribute = "name";

        /// <summary>
        /// Specifies if context should be case sensitive or not. Default is <c>true</c>.
        /// </summary>
        public const string CaseSensitiveAttribute = "caseSensitive";

        /// <summary>
        /// Specifies a <see cref="System.Type"/>.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Does not have to be fully assembly qualified, but its generally regarded
        /// as better form if the <see cref="System.Type"/> names of one's objects
        /// are specified explicitly.
        /// </p>
        /// </remarks>
        public const string TypeAttribute = "type";

        /// <summary>
        /// Specifies whether context should be lazy initialized.
        /// </summary>
        public const string LazyAttribute = "lazy";

        /// <summary>
        /// Defines an <see cref="Spring.Core.IO.IResource"/>
        /// </summary>
        public const string ResourceElement = "resource";

        /// <summary>
        /// Specifies the URI for an
        /// <see cref="Spring.Core.IO.IResource"/>.
        /// </summary>
        public const string URIAttribute = "uri";
    }

    #endregion
}
