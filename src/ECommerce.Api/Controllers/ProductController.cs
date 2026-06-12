using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Catalog;
using ECommerce.Application.Features.Catalog.AddProductVariant;
using ECommerce.Application.Features.Catalog.CreateProduct;
using ECommerce.Application.Features.Catalog.DeleteProduct;
using ECommerce.Application.Features.Catalog.DeleteProductVariant;
using ECommerce.Application.Features.Catalog.GetProductById;
using ECommerce.Application.Features.Catalog.GetProductBySlug;
using ECommerce.Application.Features.Catalog.GetProducts;
using ECommerce.Application.Features.Catalog.UpdateProduct;
using ECommerce.Application.Features.Catalog.UpdateProductVariant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ISender _mediator;

    public ProductController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/product — paged catalog with keyword/category filter and sort.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetList(
        [FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(query, cancellationToken));

    /// <summary>GET /api/product/{id} — product detail by id (admin: includes inactive).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>GET /api/product/slug/{slug} — storefront product detail (active only).</summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDetailDto>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new GetProductBySlugQuery(slug), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>POST /api/product — create a product.</summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>PUT /api/product/{id} — update a product.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id mismatch.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>DELETE /api/product/{id} — soft-delete a product (BR-16).</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }

    // ---- Variants (nested under a product) ----

    /// <summary>POST /api/product/{productId}/variants — add a variant (+ inventory row).</summary>
    [HttpPost("{productId:guid}/variants")]
    public async Task<ActionResult<Guid>> AddVariant(
        Guid productId, AddProductVariantCommand command, CancellationToken cancellationToken)
    {
        if (productId != command.ProductId)
            return BadRequest("Route productId and body ProductId mismatch.");

        var variantId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = productId }, variantId);
    }

    /// <summary>PUT /api/product/{productId}/variants/{variantId} — update a variant.</summary>
    [HttpPut("{productId:guid}/variants/{variantId:guid}")]
    public async Task<IActionResult> UpdateVariant(
        Guid productId, Guid variantId, UpdateProductVariantCommand command, CancellationToken cancellationToken)
    {
        if (variantId != command.Id)
            return BadRequest("Route variantId and body Id mismatch.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>DELETE /api/product/{productId}/variants/{variantId} — delete a variant.</summary>
    [HttpDelete("{productId:guid}/variants/{variantId:guid}")]
    public async Task<IActionResult> DeleteVariant(Guid productId, Guid variantId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductVariantCommand(variantId), cancellationToken);
        return NoContent();
    }
}
