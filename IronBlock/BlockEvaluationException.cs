// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;

namespace IronBlock
{
    public class BlockEvaluationException : Exception
    {
        public BlockEvaluationException(IBlock sourceBlock, string errorType, object errorArg)
            : base($"Block '{sourceBlock}' evaluation failed: {errorType}")
        {
            SourceBlock = sourceBlock;
            BlockErrorType = errorType;
            BlockErrorArgument = errorArg;
        }

        public IBlock SourceBlock { get; }
        public string BlockErrorType { get; }
        public object BlockErrorArgument { get; }
    }
}