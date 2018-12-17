/***********************************
 * Coder：EminemJK
 * Date：2018-12-17
 **********************************/

namespace Banana.Uow.Interface
{
    /// <summary>
    /// ISqlBuilder
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// SQL
        /// </summary>
        string SQL { get; }

        /// <summary>
        /// args
        /// </summary>
        object Arguments { get; }
    }
}
