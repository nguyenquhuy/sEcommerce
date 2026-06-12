using ECommerce.Application.Features.Addresses;
using ECommerce.Application.Features.Addresses.AddAddress;
using ECommerce.Application.Features.Addresses.DeleteAddress;
using ECommerce.Application.Features.Addresses.GetMyAddresses;
using ECommerce.Application.Features.Addresses.UpdateAddress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly ISender _mediator;

    public AddressController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/address — my saved addresses.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AddressDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetMyAddressesQuery(), cancellationToken));

    /// <summary>POST /api/address — add an address.</summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(AddAddressCommand command, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(command, cancellationToken));

    /// <summary>PUT /api/address/{id} — update an address.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route id and body id mismatch.");
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>DELETE /api/address/{id} — delete an address.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAddressCommand(id), cancellationToken);
        return NoContent();
    }
}
