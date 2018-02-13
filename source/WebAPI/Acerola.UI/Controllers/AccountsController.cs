﻿namespace Acerola.UI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using Acerola.Application.Queries;
    using Acerola.Application.Commands.Close;
    using Acerola.Application.Commands.Withdraw;
    using Acerola.Application.Commands.Deposit;
    using Acerola.UI.Requests;
    using Acerola.Domain.Customers.Accounts;
    using Acerola.UI.Model;

    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IDepositHandler depositHandler;
        private readonly IWithdrawHandler withdrawHandler;
        private readonly ICloseHandler closeHandler;
        private readonly IAccountsQueries accountsQueries;

        public AccountsController(
            IDepositHandler depositHandler, 
            IWithdrawHandler withdrawHandler,
            ICloseHandler closeHandler,
            IAccountsQueries accountsQueries)
        {
            this.depositHandler = depositHandler;
            this.withdrawHandler = withdrawHandler;
            this.closeHandler = closeHandler;
            this.accountsQueries = accountsQueries;
        }

        /// <summary>
        /// Deposit from an account
        /// </summary>
        [HttpPatch("Deposit")]
        public async Task<IActionResult> Deposit([FromBody]DepositRequest request)
        {
            var command = new DepositCommand(
                request.AccountId,
                request.Amount);

            DepositResult depositResult = await depositHandler.Handle(command);

            if (depositResult == null)
            {
                return new NoContentResult();
            }

            DepositModel model = new DepositModel(
                depositResult.Transaction.Amount,
                depositResult.Transaction.Description,
                depositResult.Transaction.TransactionDate,
                depositResult.UpdatedBalance
            );

            return new ObjectResult(model);
        }

        /// <summary>
        /// Withdraw from an account
        /// </summary>
        [HttpPatch("Withdraw")]
        public async Task<IActionResult> Withdraw([FromBody]WithdrawRequest request)
        {
            var command = new WithdrawCommand(
                request.AccountId,
                request.Amount);

            WithdrawResult depositResult = await withdrawHandler.Handle(command);

            if (depositResult == null)
            {
                return new NoContentResult();
            }

            WithdrawModel model = new WithdrawModel(
                depositResult.Transaction.Amount,
                depositResult.Transaction.Description,
                depositResult.Transaction.TransactionDate,
                depositResult.UpdatedBalance
            );

            return new ObjectResult(model);
        }

        /// <summary>
        /// Close an account
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Close([FromBody]CloseRequest request)
        {
            var command = new CloseCommand(
                request.AccountId);

            CloseResult closeResult = await closeHandler.Handle(command);

            if (closeResult == null)
            {
                return new NoContentResult();
            }

            return Ok();
        }

        /// <summary>
        /// Get an account balance
        /// </summary>
        [HttpGet("{id}", Name = "GetAccount")]
        public async Task<IActionResult> Get(Guid id)
        {
            var account = await accountsQueries.GetAccount(id);

            return Ok(account);
        }
    }
}