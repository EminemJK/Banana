﻿/***********************************
 * Developer: Lio.Huang
 * Create Date：2018-12-06
 * 
 * Last Update：2018-12-18
 **********************************/
using System;

namespace Banana.Uow.Models
{
    /// <summary>
    /// Defines the name of a table to use in Dapper.Contrib commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Creates a table mapping to a specific name for Dapper.Contrib commands
        /// </summary>
        /// <param name="tableName">The name of this table in the database.</param>
        public TableAttribute(string tableName)
        {
            Name = tableName;
        }

        /// <summary>
        /// The name of the table in the database
        /// </summary>
        public string Name { get; set; }
    }
}
