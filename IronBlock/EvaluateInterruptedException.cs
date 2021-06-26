// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;

namespace IronBlock
{
    public class EvaluateInterruptedException : Exception
    {
        public EvaluateInterruptedException(IBlock blockInterrupted, bool blockWasEvaluated, object evaluationResult)
            : base($"Block evaluation was interrupted: {blockInterrupted}")
        {
            BlockInterrupted = blockInterrupted;
            BlockWasEvaluated = blockWasEvaluated;
            EvaluationResult = evaluationResult;
        }

        public IBlock BlockInterrupted { get; }
        public bool BlockWasEvaluated { get; }
        public object EvaluationResult { get; }
    }
}