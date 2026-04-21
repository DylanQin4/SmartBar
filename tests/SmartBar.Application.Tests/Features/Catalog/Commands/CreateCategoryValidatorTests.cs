using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Catalog.Categories.Commands.CreateCategory;
using SmartBar.Application.Tests.Common.Fixtures;
using SmartBar.Domain.Features.Catalog.Entities;

namespace SmartBar.Application.Tests.Features.Catalog.Commands;

public class CreateCategoryValidatorTests
{
    private readonly IApplicationDbContext _context;
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryValidatorTests()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _validator = new CreateCategoryCommandValidator(_context);
    }

    private void SetupExistingCategories(params string[] names)
    {
        var categories = names.Select(n => Category.Create(n)).AsQueryable();
        var dbSet = Substitute.For<DbSet<Category>, IQueryable<Category>, IAsyncEnumerable<Category>>();

        ((IQueryable<Category>)dbSet).Provider.Returns(new TestAsyncQueryProvider<Category>(categories.Provider));
        ((IQueryable<Category>)dbSet).Expression.Returns(categories.Expression);
        ((IQueryable<Category>)dbSet).ElementType.Returns(categories.ElementType);
        ((IQueryable<Category>)dbSet).GetEnumerator().Returns(categories.GetEnumerator());

        _context.Categories.Returns(dbSet);
    }

    [Fact]
    public async Task Validate_EmptyName_Fails()
    {
        SetupExistingCategories();
        var command = new CreateCategoryCommand { Name = "", Description = null };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_NameTooLong_Fails()
    {
        SetupExistingCategories();
        var command = new CreateCategoryCommand { Name = new string('x', 101) };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_DuplicateName_Fails()
    {
        SetupExistingCategories("Boissons");
        var command = new CreateCategoryCommand { Name = "Boissons" };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "Unique");
    }

    [Fact]
    public async Task Validate_ValidCommand_Succeeds()
    {
        SetupExistingCategories();
        var command = new CreateCategoryCommand { Name = "Boissons", Description = "Drinks" };

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_DescriptionTooLong_Fails()
    {
        SetupExistingCategories();
        var command = new CreateCategoryCommand { Name = "Valid", Description = new string('x', 251) };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }
}
