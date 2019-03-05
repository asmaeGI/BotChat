// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Demonstrates the following concepts:
    /// - Use a subclass of ComponentDialog to implement a multi-turn conversation
    /// - Use a Waterflow dialog to model multi-turn conversation flow
    /// - Use custom prompts to validate user input
    /// - Store conversation and user state.
    /// </summary>
    public class ProductDialog : ComponentDialog
    {
        // User state for greeting dialog
        private const string ProductStateProperty = "productState";

        private const string NameValue = "productName";

        // Prompts names
        private const string NamePrompt = "namePrompt";

        // Minimum length requirements for city and name
        private const int NameLengthMinValue = 3;

        // Dialog IDs
        private const string ProfileDialog = "profileDialog";

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDialog"/> class.
        /// </summary>
        /// <param name="botServices">Connected services used in processing.</param>
        /// <param name="botState">The <see cref="UserState"/> for storing properties at user-scope.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> that enables logging and tracing.</param>
        public ProductDialog(IStatePropertyAccessor<ProductState> userProfileStateAccessor, ILoggerFactory loggerFactory)
            : base(nameof(ProductDialog))
        {
            UserProfileAccessor = userProfileStateAccessor ?? throw new ArgumentNullException(nameof(userProfileStateAccessor));

            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                    InitializeStateStepAsync,
                    ProductPrice,
                    GoodByeUser,
            };
            AddDialog(new WaterfallDialog(ProfileDialog, waterfallSteps));
        }

        public IStatePropertyAccessor<ProductState> UserProfileAccessor { get; }

        private async Task<DialogTurnResult> InitializeStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var productState = await UserProfileAccessor.GetAsync(stepContext.Context, () => null);
            if (productState == null)
            {
                var productStateOpt = stepContext.Options as ProductState;
                if (productStateOpt != null)
                {
                    await UserProfileAccessor.SetAsync(stepContext.Context, productStateOpt);
                }
                else
                {
                    await UserProfileAccessor.SetAsync(stepContext.Context, new ProductState());
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> ProductPrice(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var productState = await UserProfileAccessor.GetAsync(stepContext.Context);

            var context = stepContext.Context;

            // Display their profile information and end dialog.
            await context.SendActivityAsync($" is " + productState.Name);
            return await stepContext.EndDialogAsync();
        }

        private async Task<DialogTurnResult> GoodByeUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            // Display their profile information and end dialog.
            await context.SendActivityAsync($"Good by!");
            return await stepContext.EndDialogAsync();
        }
    }
}
