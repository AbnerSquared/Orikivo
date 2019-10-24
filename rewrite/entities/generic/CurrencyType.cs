﻿namespace Orikivo
{
    // TODO: Create the currencies and find out how to utilize each of them.
    /// <summary>
    /// Defines a type of currency that can be stored within a wallet.
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>
        /// Marks the currency type as 'Orite', the default standard.
        /// </summary>
        Generic = 1,

        /// <summary>
        /// Marks the currency type as 'Voken', the currency earned from voting.
        /// </summary>
        Vote = 2,

        /// <summary>
        /// Marks the currency type as 'Etiro', the currency used for debt.
        /// </summary>
        Debt = 3
    }
}