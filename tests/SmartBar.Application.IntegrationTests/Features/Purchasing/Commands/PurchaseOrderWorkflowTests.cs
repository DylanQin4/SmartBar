using Microsoft.EntityFrameworkCore;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddPurchaseLine;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddSupplierPayment;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.CancelPurchaseOrder;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.CreatePurchaseOrder;
using SmartBar.Application.Features.Purchasing.Suppliers.Commands.CreateSupplier;
using SmartBar.Application.Features.Catalog.Products.Commands.CreateProduct;
using SmartBar.Application.IntegrationTests.Infrastructure;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Purchasing.Enums;

namespace SmartBar.Application.IntegrationTests.Features.Purchasing.Commands;

/// <summary>
/// Tests the full purchase workflow: create supplier → create order → add lines → pay → verify state.
/// Each handler call uses a fresh DbContext to mirror real scoped DI behavior.
/// </summary>
public class PurchaseOrderWorkflowTests : IntegrationTestBase
{
    private async Task<Guid> SeedSupplier()
    {
        using var ctx = Database.CreateContext();
        var handler = new CreateSupplierCommandHandler(ctx);
        return await handler.Handle(
            new CreateSupplierCommand { Name = "Fournisseur Test", Phone = "034 00 000 00" },
            CancellationToken.None);
    }

    private async Task<Guid> SeedProduct()
    {
        using var ctx = Database.CreateContext();
        var handler = new CreateProductCommandHandler(ctx);
        return await handler.Handle(
            new CreateProductCommand
            {
                Name = $"Produit-{Guid.NewGuid():N}",
                Category = ProductCategory.Beverage,
                IsSellable = true,
                IsIngredient = false,
                SellingPrice = 3000,
                BaseUnit = UnitOfMeasure.Piece,
                LowStockThreshold = 5
            },
            CancellationToken.None);
    }

    [Fact]
    public async Task FullWorkflow_CreateOrder_AddLine_Pay_StatusTransitions()
    {
        var supplierId = await SeedSupplier();
        var productId = await SeedProduct();

        // 1. Create purchase order
        Guid orderId;
        {
            using var ctx = Database.CreateContext();
            var handler = new CreatePurchaseOrderCommandHandler(ctx, User);
            orderId = await handler.Handle(
                new CreatePurchaseOrderCommand { SupplierId = supplierId, PurchaseDate = DateTime.UtcNow },
                CancellationToken.None);
        }

        // Verify initial state
        {
            using var ctx = Database.CreateContext();
            var order = await ctx.PurchaseOrders.FirstAsync(o => o.Id == orderId);
            Assert.Equal(PurchaseStatus.Pending, order.Status);
        }

        // 2. Add line (10 units × 1000 MGA = 10,000 total)
        {
            using var ctx = Database.CreateContext();
            var handler = new AddPurchaseLineCommandHandler(ctx);
            await handler.Handle(
                new AddPurchaseLineCommand
                {
                    PurchaseOrderId = orderId,
                    ProductId = productId,
                    Quantity = 10,
                    Unit = UnitOfMeasure.Piece,
                    UnitPrice = 1000
                },
                CancellationToken.None);
        }

        // Verify after line added
        {
            using var ctx = Database.CreateContext();
            var order = await ctx.PurchaseOrders
                .Include(o => o.Lines)
                .FirstAsync(o => o.Id == orderId);
            Assert.Equal(10000m, order.TotalAmount.Amount);
            Assert.Single(order.Lines);
        }

        // 3. Partial payment (5,000)
        {
            using var ctx = Database.CreateContext();
            var handler = new AddSupplierPaymentCommandHandler(ctx);
            await handler.Handle(
                new AddSupplierPaymentCommand
                {
                    PurchaseOrderId = orderId,
                    Amount = 5000,
                    PaymentMethod = PaymentMethod.Cash,
                    PaidAt = DateTime.UtcNow
                },
                CancellationToken.None);
        }

        // Verify partial
        {
            using var ctx = Database.CreateContext();
            var order = await ctx.PurchaseOrders.FirstAsync(o => o.Id == orderId);
            Assert.Equal(PurchaseStatus.PartiallyPaid, order.Status);
            Assert.Equal(5000m, order.RemainingAmount.Amount);
        }

        // 4. Full payment (remaining 5,000)
        {
            using var ctx = Database.CreateContext();
            var handler = new AddSupplierPaymentCommandHandler(ctx);
            await handler.Handle(
                new AddSupplierPaymentCommand
                {
                    PurchaseOrderId = orderId,
                    Amount = 5000,
                    PaymentMethod = PaymentMethod.MobileMoney,
                    PaidAt = DateTime.UtcNow
                },
                CancellationToken.None);
        }

        // Verify fully paid
        {
            using var ctx = Database.CreateContext();
            var order = await ctx.PurchaseOrders.FirstAsync(o => o.Id == orderId);
            Assert.Equal(PurchaseStatus.Paid, order.Status);
            Assert.Equal(0m, order.RemainingAmount.Amount);
        }
    }

    [Fact]
    public async Task AddPurchaseLine_PersistsLineWithMoneyAndRecalculatesTotal()
    {
        var supplierId = await SeedSupplier();
        var productId = await SeedProduct();

        // Create order
        Guid orderId;
        {
            using var ctx = Database.CreateContext();
            var handler = new CreatePurchaseOrderCommandHandler(ctx, User);
            orderId = await handler.Handle(
                new CreatePurchaseOrderCommand { SupplierId = supplierId, PurchaseDate = DateTime.UtcNow },
                CancellationToken.None);
        }

        // Add 2 lines via separate handler calls
        {
            using var ctx = Database.CreateContext();
            var handler = new AddPurchaseLineCommandHandler(ctx);
            await handler.Handle(
                new AddPurchaseLineCommand
                {
                    PurchaseOrderId = orderId, ProductId = productId,
                    Quantity = 24, Unit = UnitOfMeasure.Bottle, UnitPrice = 3000
                },
                CancellationToken.None);
        }

        {
            using var ctx = Database.CreateContext();
            var handler = new AddPurchaseLineCommandHandler(ctx);
            await handler.Handle(
                new AddPurchaseLineCommand
                {
                    PurchaseOrderId = orderId, ProductId = productId,
                    Quantity = 10, Unit = UnitOfMeasure.Pack, UnitPrice = 5000
                },
                CancellationToken.None);
        }

        // Verify: 2 lines, correct Money VOs, recalculated total
        {
            using var ctx = Database.CreateContext();
            var order = await ctx.PurchaseOrders
                .Include(o => o.Lines)
                .FirstAsync(o => o.Id == orderId);

            Assert.Equal(2, order.Lines.Count);
            Assert.Equal(122000m, order.TotalAmount.Amount); // (24×3000) + (10×5000)
            Assert.Equal("MGA", order.TotalAmount.Currency);
            Assert.Equal(122000m, order.RemainingAmount.Amount);
            Assert.Equal(PurchaseStatus.Pending, order.Status);

            var bottleLine = order.Lines.First(l => l.Unit == UnitOfMeasure.Bottle);
            Assert.Equal(72000m, bottleLine.LineTotal.Amount);
            Assert.Equal(3000m, bottleLine.UnitPrice.Amount);
            Assert.Equal(24m, bottleLine.RemainingQuantity);
        }
    }

    [Fact]
    public async Task Cancel_PendingOrder_Succeeds()
    {
        var supplierId = await SeedSupplier();

        Guid orderId;
        {
            using var ctx = Database.CreateContext();
            var handler = new CreatePurchaseOrderCommandHandler(ctx, User);
            orderId = await handler.Handle(
                new CreatePurchaseOrderCommand { SupplierId = supplierId, PurchaseDate = DateTime.UtcNow },
                CancellationToken.None);
        }

        {
            using var ctx = Database.CreateContext();
            var handler = new CancelPurchaseOrderCommandHandler(ctx);
            await handler.Handle(new CancelPurchaseOrderCommand(orderId), CancellationToken.None);
        }

        {
            using var ctx = Database.CreateContext();
            var order = await ctx.PurchaseOrders.FirstAsync(o => o.Id == orderId);
            Assert.Equal(PurchaseStatus.Cancelled, order.Status);
        }
    }
}
