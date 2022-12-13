using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transaction
{
    public enum TransactionTypes
    {
        BEGIN,
        COMMIT,
        ROLLBACK,
    }
}
