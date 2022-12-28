namespace Formula.SimpleRepo.Tests;

public class BindableTests
{
    [Fact]
    public void Bindable_Parameters()
    {
        var bindable = new Bindable();
        Assert.NotNull(bindable.Parameters);
    }
}
