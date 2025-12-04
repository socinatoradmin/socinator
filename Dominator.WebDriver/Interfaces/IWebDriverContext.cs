namespace Dominator.WebDriver
{
    public interface IWebDriverContext
    {
        IWebDriver Create(bool headless = true);
    }
}