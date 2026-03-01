using System;
using System.Text.RegularExpressions;
using FitnessTracker.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class UserTests
{
    public bool UserValidator(string user)
    {
        Regex inputtedData = new Regex(@"^[a-zA-Z]+$");
        return inputtedData.IsMatch(user); 
    }

    [TestMethod]
    public void InputtedDataValidation()
    {
        User user = new User
        {
            SportAg = "Fencing",
            Date = DateTime.Now,
            Period = 30,
            Location = "Gym"
        };

        bool result = UserValidator(user.SportAg) && UserValidator(user.Location);
        Assert.IsTrue(result && user.Date != default(DateTime) && user.Period > 0);
    }
    [TestMethod]
    public void InputtedDataValidation2()
    {
        User user = new User
        {
            SportAg = "Fencing ",
            Date = DateTime.Now,
            Period = 30 ,
            Location = "Gym "
        };

        bool result = UserValidator(user.SportAg) && UserValidator(user.Location);
        Assert.IsFalse(result && user.Date != default(DateTime) && user.Period > 0);
    }
    [TestMethod]
    public void InputtedDataValidatio3()
    {
        User user = new User
        {
            SportAg = "",
            Date = DateTime.Now,
            Period = 3,
            Location = "Gym"
        };

        bool result = UserValidator(user.SportAg) && UserValidator(user.Location);
        Assert.IsFalse(result && user.Date != default(DateTime) && user.Period > 0);
    }

    [TestMethod]
    public void CsvTest()
    {
        User user = new User
        {
            SportAg = "Fencing",
            Date = new DateTime(2025, 3, 3),
            Period = 30,
            Location = "Gym"
        };
        string csvLine = $"{user.SportAg},{user.Date},{user.Period},{user.Location}";
        string[] csvSplit = csvLine.Split(',');

        User splittedData = new User
        {
            SportAg = csvSplit[0],
            Date = DateTime.Parse(csvSplit[1]),
            Period = int.Parse(csvSplit[2]),
            Location = csvSplit[3]
        };

        Assert.AreEqual(user.SportAg, splittedData.SportAg);
        Assert.AreEqual(user.Date, splittedData.Date);
        Assert.AreEqual(user.Period, splittedData.Period);
        Assert.AreEqual(user.Location, splittedData.Location);
    }
    [TestMethod]
    public void CsvTest2()
    {
        User user = new User
        {
            SportAg = "Fencing",
            Date = new DateTime(2025, 3, 3),
            Period = 30,
            Location = "Gym"
        };
        string csvLine = $"{user.SportAg},{user.Date},{user.Period},{user.Location}";
        string[] csvSplit = csvLine.Split(',');

        User splittedData = new User
        {
            SportAg = csvSplit[0] + " ",
            Date = DateTime.Parse(csvSplit[1]).AddDays(1),
            Period = int.Parse(csvSplit[2]) + 1,
            Location = csvSplit[3] + " "
        };

        Assert.AreNotEqual(user.SportAg, splittedData.SportAg);
        Assert.AreNotEqual(user.Date, splittedData.Date);
        Assert.AreNotEqual(user.Period, splittedData.Period);
        Assert.AreNotEqual(user.Location, splittedData.Location);
    }
}