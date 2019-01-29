﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting.Rules;
using Microsoft.CodeAnalysis.Options;

namespace Microsoft.CodeAnalysis.CSharp.Formatting
{
    internal class StructuredTriviaFormattingRule : BaseFormattingRule
    {
        internal const string Name = "CSharp Structured Trivia Formatting Rule";

        public override AdjustNewLinesOperation GetAdjustNewLinesOperation(SyntaxToken previousToken, SyntaxToken currentToken, OptionSet optionSet, ref NextOperation<AdjustNewLinesOperation> nextOperation)
        {
            if (previousToken.Parent is StructuredTriviaSyntax || currentToken.Parent is StructuredTriviaSyntax)
            {
                return null;
            }

            return nextOperation.Invoke();
        }

        public override AdjustSpacesOperation GetAdjustSpacesOperation(SyntaxToken previousToken, SyntaxToken currentToken, OptionSet optionSet, ref NextOperation<AdjustSpacesOperation> nextOperation)
        {
            if (previousToken.Parent is StructuredTriviaSyntax || currentToken.Parent is StructuredTriviaSyntax)
            {
                // this doesn't take care of all cases where tokens belong to structured trivia. this is only for cases we care
                return GetAdjustSpacesOperation(previousToken, currentToken, in nextOperation);
            }

            return nextOperation.Invoke();
        }

        private AdjustSpacesOperation GetAdjustSpacesOperation(SyntaxToken previousToken, SyntaxToken currentToken, in NextOperation<AdjustSpacesOperation> nextOperation)
        {
            if (previousToken.Kind() == SyntaxKind.HashToken && SyntaxFacts.IsPreprocessorKeyword(currentToken.Kind()))
            {
                return CreateAdjustSpacesOperation(space: 0, option: AdjustSpacesOption.ForceSpacesIfOnSingleLine);
            }

            if (previousToken.Kind() == SyntaxKind.RegionKeyword && currentToken.Kind() == SyntaxKind.EndOfDirectiveToken)
            {
                return CreateAdjustSpacesOperation(space: 0, option: AdjustSpacesOption.PreserveSpaces);
            }

            if (currentToken.Kind() == SyntaxKind.EndOfDirectiveToken)
            {
                return CreateAdjustSpacesOperation(space: 0, option: AdjustSpacesOption.ForceSpacesIfOnSingleLine);
            }

            return nextOperation.Invoke();
        }
    }
}
