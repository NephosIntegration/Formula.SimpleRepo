using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text.Json;

namespace Formula.SimpleRepo.Tests;

public class RepositoryBasePagedListTests
{

    [Fact]
    public async Task PagedList_get_filtered_and_sorting_and_get_page_2_()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(100);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var constraints = new Hashtable { { "Owner", "system" } };
        var orderBy = "Id DESC, TestData DESC";
        var pageNumber = 2;
        var rowsPerPage = 10;

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, constraints, orderBy);

        // assert
        Assert.Equal(rowsPerPage, actual.Count());
        Assert.All(actual, item => Assert.Equal("system", item.Owner));
        Assert.Equal(79, actual.ElementAt(0).Id);
        Assert.True(actual.ElementAt(0).Id > actual.ElementAt(1).Id);
    }

    [Fact]
    public async Task PagedList_get_page_2_empty()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(1);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 2;
        var rowsPerPage = 10;

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task PagedList_get_all_items_in_one_page_with_extra_rowsPerPage()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 100;

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Equal(10, actual.Count());
    }

    [Fact]
    public async Task PagedList_get_all_items_in_one_page_with_equals_rowsPerPage()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 10;

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Equal(10, actual.Count());
    }

    [Fact]
    public async Task PagedList_get_items_with_pageNumber_0()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 0;
        var rowsPerPage = 10;

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Equal(10, actual.Count());
    }

    [Fact]
    public async Task PagedList_get_items_with_Hashtable()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 10;
        var constraints = new Hashtable { { "Owner", "system" } };

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, constraints);

        // assert
        Assert.Equal(5, actual.Count());
    }

    [Fact]
    public async Task PagedList_get_items_with_list()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 10;
        var constraints = new List<Constraint>
        {
            new Constraint
            {
                Column = "Owner",
                DatabaseColumnName = "ownedBy",
                DataType = TypeCode.String,
                Value = "system",
                Comparison = Comparison.Equals
            }
        };

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, constraints);

        // assert
        Assert.Equal(5, actual.Count());
    }

    [Fact]
    public async Task PagedList_get_items_with_JObject()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 10;
        var constraints = JObject.FromObject(new { Owner = "system" });

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, constraints);

        // assert
        Assert.Equal(5, actual.Count());
    }


    [Fact]
    public async Task PagedList_get_items_with_json()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 10;
        var constraints = JsonSerializer.Serialize(new { Owner = "system" });

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, constraints);

        // assert
        Assert.Equal(5, actual.Count());
    }

    [Fact]
    public async Task PagedList_get_items_with_rowsPerPage_0()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var pageNumber = 1;
        var rowsPerPage = 0;

        // act
        var actual = await target.GetPagedListAsync(pageNumber, rowsPerPage, new Hashtable());

        // assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task RecordCount_count_all_items()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);

        // act
        var actual = await target.GetRecordCountAsync();

        // assert
        Assert.Equal(10, actual);
    }

    [Fact]
    public async Task RecordCount_count_filtered_items()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var constraints = new Hashtable { { "Owner", "system" } };

        // act
        var actual = await target.GetRecordCountAsync(constraints);

        // assert
        Assert.Equal(5, actual);
    }

    [Fact]
    public async Task RecordCount_count_filtered_with_no_items()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);
        var constraints = new Hashtable { { "Owner", "system123" } };

        // act
        var actual = await target.GetRecordCountAsync(constraints);

        // assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task RecordCount_with_conditions()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);

        // act
        var actual = await target.GetRecordCountAsync("where ownedBy='system'");

        // assert
        Assert.Equal(5, actual);
    }

    [Fact]
    public async Task RecordCount_with_whereConditions()
    {
        // arrange
        using var connection = DatabasePrimer.CreateTestDatabase(10);
        var target = new PagedListRepository(SettingsHelper.Configuration);

        // act
        var actual = await target.GetRecordCountAsync(new  { Owner = "system" });

        // assert
        Assert.Equal(5, actual);
    }

}
