using System.Collections;

namespace Formula.SimpleRepo.Tests;

public class RepositoryBaseListPagedTests
{

    [Fact]
    public async Task ListPaged_get_filtered_and_sorting_and_get_page_2_()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(100);
        var target = new ListPagedRepository(SettingsHelper.Configuration);
        var constraints = new Hashtable { { "Owner", "system" } };
        var orderBy = "Id DESC";
        var pageNumber = 2;
        var rowsPerPage = 10;

        // act
        var actual = await target.GetListPagedAsync(pageNumber, rowsPerPage, constraints, orderBy);
        connection.Close();

        // assert
        Assert.Equal(rowsPerPage, actual.Count());
        Assert.All(actual, item => Assert.Equal("system", item.Owner));
        Assert.Equal(79, actual.ElementAt(0).Id);
        Assert.True(actual.ElementAt(0).Id > actual.ElementAt(1).Id);
    }

    [Fact]
    public async Task ListPaged_get_page_2_empty()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(1);
        var target = new ListPagedRepository(SettingsHelper.Configuration);
        var pageNumber = 2;
        var rowsPerPage = 10;

        // act
        var actual = await target.GetListPagedAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task ListPaged_get_all_items_in_one_page_with_extra_rowsPerPage()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new ListPagedRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 100;

        // act
        var actual = await target.GetListPagedAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Equal(10, actual.Count());
    }

    [Fact]
    public async Task ListPaged_get_all_items_in_one_page_with_equals_rowsPerPage()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new ListPagedRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 10;

        // act
        var actual = await target.GetListPagedAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Equal(10, actual.Count());
    }

    [Fact]
    public async Task RecordCount_count_all_items()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new ListPagedRepository(SettingsHelper.Configuration);

        // act
        var actual = await target.RecordCountAsync();

        // assert
        Assert.Equal(10, actual);
    }

    [Fact]
    public async Task RecordCount_count_filtered_items()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new ListPagedRepository(SettingsHelper.Configuration);
        var constraints = new Hashtable { { "Owner", "system" } };

        // act
        var actual = await target.RecordCountAsync(constraints);

        // assert
        Assert.Equal(5, actual);
    }

    [Fact]
    public async Task RecordCount_count_filtered_with_no_items()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new ListPagedRepository(SettingsHelper.Configuration);
        var constraints = new Hashtable { { "Owner", "system123" } };

        // act
        var actual = await target.RecordCountAsync(constraints);

        // assert
        Assert.Equal(0, actual);
    }


}
